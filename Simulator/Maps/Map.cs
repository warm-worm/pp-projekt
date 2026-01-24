namespace Simulator.Maps;

using Simulator;
using System.Collections.Generic;
using System.Linq;

public abstract class Map
{
    private readonly Dictionary<Point, List<IMappable>> _fields = new();
    public readonly int SizeX;
    public readonly int SizeY;
    public readonly Rectangle area;
    private List<string> _mapLogs = new();

    protected Map(int sizeX, int sizeY)
    {
        if (sizeX < 5) throw new ArgumentOutOfRangeException(nameof(sizeX), "SizeX must be greater than 5.");
        if (sizeY < 5) throw new ArgumentOutOfRangeException(nameof(sizeY), "SizeY must be greater than 5.");
        SizeX = sizeX;
        SizeY = sizeY;
        area = new Rectangle(0, 0, SizeX - 1, SizeY - 1);
    }

    public List<string> GetTurnLogs()
    {
        var logs = new List<string>(_mapLogs);
        _mapLogs.Clear();
        return logs;
    }

    public virtual bool Exist(Point p) => area.Contains(p);
    public abstract Point Next(Point p, Direction d);
    public abstract Point NextDiagonal(Point p, Direction d);

    public void Add(IMappable mappable, Point p)
    {
        if (!Exist(p)) return;
        if (!_fields.ContainsKey(p)) _fields[p] = new List<IMappable>();
        _fields[p].Add(mappable);
    }

    public void Remove(IMappable mappable, Point p)
    {
        if (!_fields.ContainsKey(p)) return;
        _fields[p].Remove(mappable);
        if (_fields[p].Count == 0) _fields.Remove(p);
    }

    // --- LOGIKA RUCHU I INTERAKCJI ---
    public void Move(IMappable mappable, Point from, Point here)
    {
        Remove(mappable, from);
        var residents = At(here).ToList();

        // 1. KIEDY ORK/ELF SIĘ RUSZA
        if (mappable is Creature attacker && !attacker.IsDead)
        {
            foreach (var resident in residents)
            {
                if (attacker is Orc orc && resident is PackAnimal pack)
                {
                    HuntInteraction(orc, pack, here);
                    if (pack.Count <= 0) continue;
                }
                if (attacker is Elf elf && resident is Birds bird)
                {
                    SingInteraction(elf, bird, here);
                    if (!(bird is Penguin)) continue;
                }
                if (resident is Creature defender && !defender.IsDead)
                {
                    BattleInteraction(attacker, defender);
                }
            }
        }
        // 2. KIEDY ZWIERZĘ SIĘ RUSZA
        else if (mappable is Animals movingAnimal)
        {
            foreach (var resident in residents)
            {
                if (resident is Orc orc && movingAnimal is PackAnimal pack)
                {
                    // Tutaj zmieniamy tekst w zależności od biomu/kota! <3
                    string location = GetBiomeLocationName();
                    if (WorldSettings.IsCatMode)
                        _mapLogs.Add($"Nieostrożny {pack.Description} wturlał się pod łapy {orc.Name}!");
                    else
                        _mapLogs.Add($"Nieostrożny {pack.Description} wszedł prosto w {location} {orc.Name}!");

                    HuntInteraction(orc, pack, here);
                    if (pack.Count <= 0) return;
                }
                if (resident is Elf elf && movingAnimal is Birds bird)
                {
                    string action = WorldSettings.IsCatMode ? "wskoczył na głowę" : "przyleciał do";
                    _mapLogs.Add($"{bird.Description} {action} {elf.Name}.");

                    SingInteraction(elf, bird, here);
                    if (!(bird is Penguin)) return;
                }
                if (resident is Animals residentAnimal)
                {
                    AnimalSocialInteraction(movingAnimal, residentAnimal);
                }
            }
        }

        if (mappable is Creature c && c.IsDead) return;
        if (mappable is Animals a && a.Health <= 0) return;
        if (mappable is PackAnimal p && p.Count <= 0) return;

        Add(mappable, here);
    }

    // --- SZCZEGÓŁOWE INTERAKCJE ---

    private void HuntInteraction(Orc orc, PackAnimal pack, Point here)
    {
        bool canHunt = (WorldSettings.CurrentBiome == Biome.Mountains && pack is Goat) ||
                       (WorldSettings.CurrentBiome == Biome.Forest && pack is Rabbit);

        if (canHunt)
        {
            int rageBonus = (pack is Goat) ? 2 : 1;
            int hpBonus = (pack is Goat) ? 10 : 5;

            orc.Rage = Validator.Limiter(orc.Rage + rageBonus, 0, 20);
            orc.Health = Math.Min(100, orc.Health + hpBonus);
            pack.Count--;

            string foodName = (pack is Goat) ? "kozę" : "królika";

            // --- RÓŻNE KOMUNIKATY ---
            if (WorldSettings.IsCatMode)
            {
                _mapLogs.Add($"😼 {orc.Name} złapał {foodName} i bawi się zdobyczą! (+{rageBonus} Rage)");
            }
            else
            {
                _mapLogs.Add($"⚔️ {orc.Name} brutalnie pożera {foodName}! (+{hpBonus} HP)");
            }

            if (pack.Count > 0)
            {
                string escapeMsg = WorldSettings.IsCatMode ? "uciekają w podskokach" : "uciekają w panice";
                _mapLogs.Add($">> Reszta stada ({pack.Count} szt.) {escapeMsg}.");
            }
            else
            {
                Remove(pack, here);
                _mapLogs.Add($"☠️ Ostatni {pack.Description} zginął! Gatunek wyeliminowany.");
            }
        }
        else
        {
            if (WorldSettings.IsCatMode) _mapLogs.Add($"{orc.Name} wącha {pack.Description}, ale woli Whiskas.");
            else _mapLogs.Add($"{orc.Name} ignoruje {pack.Description} - to nie jego dieta.");
        }
    }

    private void SingInteraction(Elf elf, Birds bird, Point here)
    {
        if (bird is Eagle) { elf.Agility = Validator.Limiter(elf.Agility + 1, 0, 20); elf.Health += 2; }
        else if (bird is Nightingale) { elf.Agility = Validator.Limiter(elf.Agility + 2, 0, 20); elf.Health += 1; }
        else if (bird is Penguin) { elf.Agility = Validator.Limiter(elf.Agility + 1, 0, 20); elf.Health += 6; }

        if (WorldSettings.IsCatMode)
            _mapLogs.Add($"😺 {elf.Name} mruczy w duecie z {bird.Description}!");
        else
            _mapLogs.Add($"🎵 {elf.Name} śpiewa pieśń z {bird.Description}!");

        if (!(bird is Penguin))
        {
            Remove(bird, here);
            string flyMsg = WorldSettings.IsCatMode ? "ucieka przed pazurami" : "odlatuje w siną dal";
            _mapLogs.Add($">> {bird.Description} {flyMsg}.");
        }
        else
        {
            _mapLogs.Add(">> Pingwin zostaje. I tak nie umie latać.");
        }
    }

    private void BattleInteraction(Creature attacker, Creature defender)
    {
        if (WorldSettings.CurrentBiome != Biome.Snowland)
        {
            if (WorldSettings.IsCatMode)
                _mapLogs.Add($"😾 {attacker.Name} syczy i drapie {defender.Name}!");
            else
                _mapLogs.Add($"⚔️ {attacker.Name} dobywa broni i atakuje {defender.Name}!");

            attacker.Attack(defender);

            if (WorldSettings.CurrentBiome == Biome.Forest && attacker is Elf && !defender.IsDead)
            {
                attacker.Attack(defender);
                string bonusMsg = WorldSettings.IsCatMode ? "Podwójne drapnięcie" : "Szybki atak zza drzewa";
                _mapLogs.Add($">> {bonusMsg} (Bonus Lasu)!");
            }

            if (defender.IsDead)
            {
                _mapLogs.Add($"☠️ {defender.Name} pada bez ducha!");
            }
            else
            {
                defender.Attack(attacker);
                string counterMsg = WorldSettings.IsCatMode ? "gryzie w odwecie" : "kontratakuje";
                _mapLogs.Add($">> {defender.Name} {counterMsg}!");

                if (attacker.IsDead) _mapLogs.Add($"☠️ {attacker.Name} ginie w walce!");
            }
        }
        else
        {
            if (WorldSettings.IsCatMode)
                _mapLogs.Add($"❄️ {attacker.Name} i {defender.Name} ocierają się futerkiem (Pokój w Snowland).");
            else
                _mapLogs.Add($"❄️ {attacker.Name} i {defender.Name} wymieniają ukłony (Rozejm w Snowland).");
        }
    }

    private void AnimalSocialInteraction(Animals one, Animals two)
    {
        // 1. Dwa stada
        if (one is PackAnimal && two is PackAnimal)
        {
            if (one.GetType() == two.GetType())
                _mapLogs.Add($"{one.Description} spotyka kuzynów. Stado się powiększa.");
            else
                _mapLogs.Add($"{one.Description} i {two.Description} walczą o najlepszą trawę.");
        }
        // 2. Ptak i Roślinożerca
        else if ((one is Birds && two is PackAnimal) || (one is PackAnimal && two is Birds))
        {
            var bird = one is Birds ? one : two;
            var land = one is PackAnimal ? one : two;

            if (bird is Eagle)
                _mapLogs.Add($"Orzeł pikuje nad {land.Description}. Panika w stadzie!");
            else if (bird is Nightingale)
                _mapLogs.Add($"Słowik umila czas {land.Description}.");
            else
                _mapLogs.Add($"Pingwin potyka się o {land.Description}.");
        }
        // 3. Dwa Ptaki
        else if (one is Birds && two is Birds)
        {
            _mapLogs.Add($"{one.Description} i {two.Description} robią hałas.");
        }
    }

    // Pomocnicza metoda do nazw miejsc
    private string GetBiomeLocationName()
    {
        return WorldSettings.CurrentBiome switch
        {
            Biome.Forest => "gęstwinę",
            Biome.Mountains => "jaskinię",
            Biome.Snowland => "igloo",
            _ => "obozowisko"
        };
    }

    public List<IMappable> At(Point p)
    {
        if (_fields.ContainsKey(p)) return _fields[p];
        return new List<IMappable>();
    }

    public List<IMappable> At(int x, int y) => At(new Point(x, y));
}