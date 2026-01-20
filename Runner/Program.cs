using Simulator;
using Simulator.Maps;

namespace Runner;

internal class Program
{
    /*static void Main(string[] args)
    {
       List<Creature> creatures =
       [
            new Elf("Elrond", 5, 7),
            new Orc("Gorgul", 4, 6),
            new Elf("Legolas", 6, 8),
            new Orc("Thrall", 5, 7),
            new Elf("Tauriel", 4, 5),
            new Orc("Azog", 6, 9)
        ];
        creatures.ForEach(c => Console.WriteLine($"{c,-20} power: {c.Power}"));

        Console.WriteLine("\nPosortowane wg mocy:\n");

        creatures.Sort((x, y) => y.Power - x.Power);
        creatures.ForEach(c => Console.WriteLine($"{c,-20} power: {c.Power}"));

        Console.WriteLine("\nPosortowane wg imion:\n");
        creatures.Sort((x, y) => x.Name.CompareTo(y.Name));
        creatures.ForEach(c => Console.WriteLine($"{c,-20} name: {c.Power}"));
    }   */

    //test walki 
    /*static void Main(string[] args)
    {
        Console.WriteLine("=== TEST 1: WZAJEMNA WALKA ===");
        TestMutualAttack();

        Console.WriteLine("\n=== TEST 2: ŚMIERĆ I BRAK ATAKU ZZA GROBU ===");
        TestInstaKill();
    }

    static void TestMutualAttack()
    {
        // Tworzymy mapę i dwie silne jednostki
        var map = new SmallTorusMap(8, 6);
        var orc = new Orc("Gorgul", 1, 5); // Power 22
        var elf = new Elf("Elrond", 1, 5); // Power 18

        Point p1 = new Point(2, 2);
        Point p2 = new Point(2, 3);

        orc.InitMapAndPosition(map, p1);
        elf.InitMapAndPosition(map, p2);

        Console.WriteLine($"Start: {orc.Name} HP: {orc.Health} Power:{orc.Power}, {elf.Name} HP: {elf.Health} Power:{elf.Power}" );

        // Orc wchodzi na pole Elfa (ruch z (2,2) na (2,3), zatem Orc atakuje Elfa)
        Console.WriteLine($"{orc.Name} idzie na {p2}...");
        map.Move(orc, p1, p2);

        Console.WriteLine($"Po walce: {orc.Name} HP: {orc.Health}, {elf.Name} HP: {elf.Health}");

        if (orc.Health < 100 && elf.Health < 100)
            Console.WriteLine("WYNIK: Obaj odnieśli obrażenia.");
    }

    static void TestInstaKill()
    {
        var map = new SmallTorusMap(8, 6);
        // Orc jest super silny, Elf ma 1 HP
        var opOrc = new Orc("OP-Gorgul", 10, 10);
        var weakElf = new Elf("Słaby-Elf", 1, 1);
        weakElf.Health = 1; // Ustawiamy mu ręcznie 1 HP dla testu

        Point p1 = new Point(0, 0);
        Point p2 = new Point(0, 1);

        opOrc.InitMapAndPosition(map, p1);
        weakElf.InitMapAndPosition(map, p2);

        int orcHpBefore = opOrc.Health;
        Console.WriteLine($"Start: Orc HP: {orcHpBefore}, Elf HP: {weakElf.Health}");

        // Orc wchodzi i zabija Elfa jednym hitem
        map.Move(opOrc, p1, p2);

        Console.WriteLine($"Po walce: Elf IsDead: {weakElf.IsDead}, Orc HP: {opOrc.Health}");

        if (weakElf.IsDead && opOrc.Health == orcHpBefore)
            Console.WriteLine("WYNIK: Sukces! Yay! Elf zginął i nie zdążył oddać (Orc nie stracił HP).");
        else
            Console.WriteLine("WYNIK: Coś nie tak – Orc stracił HP mimo że zabił przeciwnika jednym ciosem.");
    }*/

    //testowanie biomów

        static void Main(string[] args)
        {
            // 1. Inicjalizacja
            var map = new SmallTorusMap(10, 10);
            var orc = new Orc("Gorgul", 1, 5);  // Bazowy Rage 5
            var elf = new Elf("Elrond", 1, 5);  // Bazowy Agility 5

            Point pOrc = new Point(2, 2);
            Point pElf = new Point(2, 3);

            orc.InitMapAndPosition(map, pOrc);
            elf.InitMapAndPosition(map, pElf);

            Console.WriteLine("======= TEST PRZEPŁYWU BIOMÓW =======");

            // Testujemy po kolei 3 biomy
            ExecuteFullTurn(map, orc, elf, Biome.Forest, pOrc, pElf);
            ExecuteFullTurn(map, orc, elf, Biome.Mountains, pOrc, pElf);
            ExecuteFullTurn(map, orc, elf, Biome.Snowland, pOrc, pElf);

            Console.WriteLine("======= KONIEC TESTÓW =======");
        }

        static void ExecuteFullTurn(Map map, Orc orc, Elf elf, Biome biome, Point startPos, Point targetPos)
        {
            Console.WriteLine($"\n>>> ZMIANA BIOMU NA: {biome.ToString().ToUpper()} <<<");
            WorldSettings.CurrentBiome = biome;

            // KROK 1: Pokaż wpływ samego biomu zanim postacie się ruszą
            Console.WriteLine("\n[1. Stan po zmianie biomu (przed ruchem)]:");
            PrintStats(orc, elf);

            // KROK 2: Walka
            Console.WriteLine($"\n[2. AKCJA: {orc.Name} wchodzi na pole {targetPos}, gdzie stoi {elf.Name}]");
            map.Move(orc, orc.Position, targetPos);

            // KROK 3: Wynik walki
            Console.WriteLine("\n[3. Stan po starciu na tym biomie]:");
            PrintStats(orc, elf);

            // KROK 4: Wyjście z pola (przygotowanie do następnego biomu)
            // Musimy ich rozdzielić, żeby w następnym teście znów mogli na siebie "wejść"
            Console.WriteLine("\n[4. Sprzątanie: Rozdzielanie postaci i leczenie...]");
            map.Move(orc, orc.Position, startPos); // Orc wraca na (2,2)
            orc.Health = 100; // Leczymy ich, żeby testy były czytelne
            elf.Health = 100;
            Console.WriteLine(new string('=', 45));
        }

        static void PrintStats(Orc orc, Elf elf)
        {
            // Pobieramy aktualne modyfikatory tylko do wyświetlenia w logu
            var (aMod, rMod) = WorldSettings.GetModifiers();

            Console.WriteLine($"  {orc.Name,-10} | HP: {orc.Health,3} | Power: {orc.Power,3} | (Rage: {orc.Rage} {rMod:+#;-#;0} z biomu)");
            Console.WriteLine($"  {elf.Name,-10} | HP: {elf.Health,3} | Power: {elf.Power,3} | (Agility: {elf.Agility} {aMod:+#;-#;0} z biomu)");
        }
    }


