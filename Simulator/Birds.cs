using Simulator.Maps;

namespace Simulator;

public class Birds : Animals
{
    public bool CanFly { get; set; } = true;

    public override char MapSymbol => CanFly ? 'B' : 'b'; // B - lata, b - nielot

    public override string Info => $"{Description} (fly{(CanFly ? "+" : "-")}) <{Size}>";

    public override void Go(Direction direction)
    {
        if (_map == null) return;

        Point next;

        if (CanFly)
        {
            // ptaki latajace: 2 pola w danym kierunku 
            next = _map.Next(_position, direction);
            next = _map.Next(next, direction);
        }
        else
        {
            // ptaki nieloty: chodza po skosie
            next = _map.NextDiagonal(_position, direction);
        }

        _map.Move(this, _position, next);
        _position = next;
    }
}