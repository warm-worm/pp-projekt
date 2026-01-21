namespace Simulator;

public class Eagle : Birds
{
    public override char MapSymbol => 'J';
    public Eagle() { Description = "Eagle"; CanFly = true; }
    public override void Go(Direction d) => _position = Map?.Next(Position, d) ?? Position;
}