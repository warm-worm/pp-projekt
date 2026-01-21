namespace Simulator;

public class Penguin : Birds
{
    public override char MapSymbol => 'P';
    public Penguin() { Description = "Penguin"; CanFly = false; } // Nielot
    public override void Go(Direction d) => _position = Map?.NextDiagonal(Position, d) ?? Position;
}