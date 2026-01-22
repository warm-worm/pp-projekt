namespace Simulator;

public class Goat : PackAnimal
{
    public override char MapSymbol => 'G';
    public Goat() { Description = "Goat Pack"; Count = 4; }

    // POPRAWIONA METODA RUCHU:
    public override void Go(Direction d)
    {
        // 1. Sprawdzamy czy jest mapa
        if (Map == null) return;

        // 2. Obliczamy gdzie Koza chce iść (NA SKOS)
        var next = Map.NextDiagonal(Position, d);

        // 3. KLUCZOWE: Mówimy Mapie "Przesuń mnie stąd -> tam"
        Map.Move(this, Position, next);

        // 4. Aktualizujemy współrzędne wewnątrz Kozy
        _position = next;
    }
}