namespace Simulator;

public class Nightingale : Birds
{
    public override char MapSymbol => 'N';
    public Nightingale() { Description = "Nightingale"; CanFly = true; }
    public override void Go(Direction d) => _position = Map?.Next(Position, d) ?? Position;
}
