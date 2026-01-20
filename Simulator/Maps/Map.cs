namespace Simulator.Maps;

using Simulator;

/// <summary>
/// Map of points.
/// </summary>
public abstract class Map // klasa abstrakcyjna, baza dla innych map
{
    // slownik przechowujacy obiekty na polach: Klucz to Punkt, Wartosc to Lista obiektow IMappable
    private readonly Dictionary<Point, List<IMappable>> _fields = new();

    public readonly int SizeX; // rozmiar mapy w osi X
    public readonly int SizeY; // rozmiar mapy w osi Y
    public readonly Rectangle area;

    protected Map(int sizeX, int sizeY) //protected - widoczny tylko dla dzieci
    {
        if (sizeX < 5)
            throw new ArgumentOutOfRangeException(nameof(sizeX), "SizeX must be greater than 5.");
        if (sizeY < 5)
            throw new ArgumentOutOfRangeException(nameof(sizeY), "SizeY must be greater than 5.");
        SizeX = sizeX;
        SizeY = sizeY;
        area = new Rectangle(0, 0, SizeX - 1, SizeY - 1);
    }

    /// <summary>
    /// Check if give point belongs to the map.
    /// </summary>
    /// <param name="p">Point to check.</param>
    /// <returns></returns>
    public virtual bool Exist(Point p) => area.Contains(p); // czy punkt jest na mapie

    /// <summary>
    /// Next position to the point in a given direction.
    /// </summary>
    /// <param name="p">Starting point.</param>
    /// <param name="d">Direction.</param>
    /// <returns>Next point.</returns>
    public abstract Point Next(Point p, Direction d); // musi byc zaimplementowane: ruch

    /// <summary>
    /// Next diagonal position to the point in a given direction
    /// rotated 45 degrees clockwise.
    /// </summary>
    /// <param name="p">Starting point.</param>
    /// <param name="d">Direction.</param>
    /// <returns>Next point.</returns>
    public abstract Point NextDiagonal(Point p, Direction d); // musi byc zaimplementowane: ruch po skosie

    /// <summary>
    /// Add creature to map.
    /// </summary>
    /// <param name="mappable">Creature to place on map</param>
    /// <param name="p">Point for creature.</param>
    public void Add(IMappable mappable, Point p)
    {
        if (!Exist(p)) return; // jesli punkt nie istnieje, nic nie robimy

        if (!_fields.ContainsKey(p)) // jesli w tym punkcie nie ma jeszcze listy
        {
            _fields[p] = new List<IMappable>(); // tworzymy nowa liste
        }
        _fields[p].Add(mappable); // dodajemy obiekt do listy
    }

    /// <summary>
    /// Remove creature from map.
    /// </summary>
    /// <param name="mappable">Creature to remove</param>
    /// <param name="p">Point where creature is</param>
    public void Remove(IMappable mappable, Point p)
    {
        if (!_fields.ContainsKey(p)) return; // jesli puste pole, to nic nie robimy

        _fields[p].Remove(mappable); // usuwamy obiekt z listy

        if (_fields[p].Count == 0) // jesli lista pusta
        {
            _fields.Remove(p); // usuwamy wpis ze slownika zeby nie zasmiecac
        }
    }

    /// <summary>
    /// Move creature from one point to another.
    /// </summary>

    public void Move(IMappable mappable, Point from, Point here)
    {
        Remove(mappable, from);

        var residents = At(here).ToList();

        if (mappable is Creature attacker && !attacker.IsDead)
        {
            // ZASADA SNOWLAND: Jeśli jest zima, nie walczymy
            if (WorldSettings.CurrentBiome == Biome.Snowland)
            {
                // Tutaj nie wywołujemy Attack(), postacie po prostu stoją na jednym polu (jest za zimno by walczyć XDD)
            }
            else
            {
                foreach (var resident in residents)
                {
                    if (resident is Creature defender && !defender.IsDead)
                    {
                        // Atak 1
                        attacker.Attack(defender);

                        // ZASADA FOREST: Elf atakuje drugi raz
                        if (WorldSettings.CurrentBiome == Biome.Forest && attacker is Elf && !defender.IsDead)
                        {
                            attacker.Attack(defender);
                        }

                        // Kontratak
                        if (!defender.IsDead)
                        {
                            defender.Attack(attacker);
                        }
                    }
                }
            }
        }

        // Dodanie na mapę (tylko jeśli żyje)
        if (mappable is Creature c && c.IsDead) return;
        Add(mappable, here);
    }

    /// <summary>
    /// Get List of creatures.
    /// </summary>
    /// <param name="p">Point to check.</param>
    /// <returns>List of creatures at given point.</returns>
    public List<IMappable> At(Point p)
    {
        if (_fields.ContainsKey(p)) // jesli cos tu jest
        {
            return _fields[p]; // zwracamy liste
        }
        return new List<IMappable>(); // zwracamy pusta liste zamiast nulla
    }

    /// <summary>
    /// Get list of creatures.
    /// </summary>
    /// <param name="x">Point to check x coordinate</param>
    /// <param name="y">Point to check y coordinate </param>
    /// <returns></returns>
    public List<IMappable> At(int x, int y) => At(new Point(x, y));
}