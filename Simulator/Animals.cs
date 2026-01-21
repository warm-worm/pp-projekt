using Simulator.Maps;

namespace Simulator;

public class Animals : IMappable
{
    public int Health { get; set; } = 50;//domyślne hp zwierząt
    public int Count { get; set; } = 1;//liczba domyślna zwierząt
    private string _description = "Unnamed";
    protected Map? _map; // protected, aby dziedziczace ptaki mialy dostep
    protected Point _position;

    public string Description
    {
        get => _description;
        init => _description = Validator.Shortener(value, 3, 15, '#');
    }

    public uint Size { get; set; } = 3;

    public virtual string Info => $"{_description} <{Size}>";

    // implementacja IMappable
    public Map? Map => _map;
    public Point Position => _position;
    public virtual char MapSymbol => 'A';

    public virtual void Go(Direction direction)
    {
        if (_map == null) return;

        // zwykle zwierzeta ida o 1 pole (jak stwory)
        Point next = _map.Next(_position, direction);
        _map.Move(this, _position, next);
        _position = next;
    }

    public void InitMapAndPosition(Map map, Point startingPosition)
    {
        if (map == null) throw new ArgumentNullException(nameof(map));
        if (!map.Exist(startingPosition)) throw new ArgumentOutOfRangeException(nameof(startingPosition));

        map.Add(this, startingPosition);
        _map = map;
        _position = startingPosition;


    }

    public override string ToString()
    {
        return $"{GetType().Name.ToUpper()}: {Info}";
    }

}