using Simulator.Maps;

namespace Simulator;

// dodajemy implementacje interfejsu
public abstract class Creature : IMappable
{
    private Map? _map; // mapa na ktorej jest stwor
    private Point _point; // pozycja stwora

    private string _name = "Unknown"; // wartosc domyslna name
    private int _level = 1;           // wartosc domyslna level
    private int _health = 100;            //domyślne hp

    public string Name
    {
        get { return _name; }
        init
        {
            _name = Validator.Shortener(value, 3, 25, '#');
        }
    }

    public int Level
    {
        get { return _level; }
        init
        {
            _level = Validator.Limiter(value, 1, 10);
        }
    }

    public int Health
    {
        get => _health;
        set => _health = Validator.Limiter(value, 0, 200); // Max 200 HP
    }

    public abstract void Attack(Creature target); // abstrakcyjna metoda ataku
    public Map? Map => _map; // wlasciwosc do odczytu mapy
    public Point Position => _point; // wlasciwosc do odczytu pozycji

    // symbol abstrakcyjny, bo ork i elf maja inny (wymagany przez IMappable)
    public abstract char MapSymbol { get; }

    public bool IsDead => Health <= 0;

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (IsDead)
        {
            // usunięcie z mapy jak sraci całe hp
            Map?.Remove(this, Position);
        }
    }



    public void InitMapAndPosition(Map map, Point startingPosition)
    {
        if (map == null) throw new ArgumentNullException(nameof(map)); // zabezpieczenie

        if (!map.Exist(startingPosition)) // czy punkt startowy jest ok
            throw new ArgumentOutOfRangeException(nameof(startingPosition), "Point out of map range.");

        map.Add(this, startingPosition); // dodajemy sie do mapy

        _map = map;
        _point = startingPosition;
    }

    public Creature()
    {
    }

    public Creature(string name, int level = 1)
    {
        Name = name;
        Level = level;
    }

    public abstract string Greeting();

    public void Upgrade()
    {
        if (_level < 10) _level++;
    }


    public Func<int> CalculatePower { get; set; } = () => 0;
    public int Power => CalculatePower();

    public abstract string Info { get; }

    public override string ToString()
    {
        return $"{GetType().Name.ToUpper()}: {Info}";
    }

    // pojedynczy ruch (implementacja z interfejsu)
    public void Go(Direction direction)
    {
        if (_map == null) return; // jak nie ma mapy to nie idzie

        Point nextPoint = _map.Next(_point, direction); // gdzie chce isc

        _map.Move(this, _point, nextPoint); // przesuwamy na mapie
        _point = nextPoint; // aktualizujemy wlasna pozycje
    }

    public void Go(List<Direction> directions)
    {
        foreach (var dir in directions)
        {
            Go(dir);
        }
    }

    public void Go(string moves)
    {
        Go(DirectionParser.Parse(moves));
    }
}