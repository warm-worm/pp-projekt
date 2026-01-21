namespace Simulator;

public class Goat : PackAnimal
{
    public override char MapSymbol => 'G';
    public Goat() { Description = "Goat Pack"; Count = 4; }
    public override void Go(Direction d) => _position = Map?.NextDiagonal(Position, d) ?? Position;
}