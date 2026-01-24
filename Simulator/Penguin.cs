namespace Simulator;

public class Penguin : Birds
{
    public override char MapSymbol => 'P';
    public Penguin() { Description = "Penguin"; CanFly = false; } // Nielot

    // Ruch na skos z aktualizacją Mapy
    public override void Go(Direction d)
    {
        if (Map == null) return;

        var next = Map.NextDiagonal(Position, d);
        Map.Move(this, Position, next);
        _position = next;
    }
}