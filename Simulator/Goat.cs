namespace Simulator;

public class Goat : PackAnimal
{
    public override char MapSymbol => 'G';
    public Goat() { Description = "Goat Pack"; Count = 4; }
    public override void Go(Direction d)
    {
        // Sprawdzamy czy jest mapa
        if (Map == null) return;

        // (NA SKOS)
        var next = Map.NextDiagonal(Position, d);
        Map.Move(this, Position, next);

        //Aktualizujemy współrzędne wewnątrz Kozy
        _position = next;
    }
}