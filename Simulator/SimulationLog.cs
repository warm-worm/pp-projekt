namespace Simulator;

using Simulator.Maps;

public class SimulationLog
{
    private Simulation _simulation { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public List<TurnLog> TurnLogs { get; } = [];

    public SimulationLog(Simulation simulation)
    {
        _simulation = simulation ??
            throw new ArgumentNullException(nameof(simulation)); // zabezpieczenie
        SizeX = _simulation.Map.SizeX;
        SizeY = _simulation.Map.SizeY;
        Run(); // odpalamy symulacje od razu
    }

    private void Run()
    {
        // 1. zapisujemy stan poczatkowy (indeks 0, przed ruchami)
        TurnLogs.Add(new TurnLog
        {
            Mappable = "START",
            Move = "INIT",
            Symbols = GetMapSymbols()
        });

        // 2. petla symulacji - robimy ruchy az do konca
        while (!_simulation.Finished)
        {
            // zapamietujemy kto i gdzie sie ruszy (zanim to zrobi)
            var currentMappable = _simulation.CurrentMappable.ToString();
            var currentMove = _simulation.CurrentMoveName;

            // wykonujemy ruch
            _simulation.Turn();

            // zapisujemy log po ruchu (stan mapy i opis ruchu)
            TurnLogs.Add(new TurnLog
            {
                Mappable = currentMappable,
                Move = currentMove,
                Symbols = GetMapSymbols()
            });
        }
    }

    // metoda pomocnicza do zbierania symboli z mapy
    private Dictionary<Point, char> GetMapSymbols()
    {
        var symbols = new Dictionary<Point, char>();

        // przelatujemy po calej mapie
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                
                var objects = _simulation.Map.At(x, y) // filtrujemy tylko te obiekty, które nie są martwymi stworami (co tu stoi i czy jest żywe)
                    .Where(m => m is not Creature c || !c.IsDead)
                    .ToList();
                if (objects.Count == 1)
                {
                    symbols.Add(new Point(x, y), objects[0].MapSymbol); // jeden obiekt
                }
                else if (objects.Count > 1)
                {
                    symbols.Add(new Point(x, y), 'X'); // kolizja
                }
            }
        }
        return symbols;
    }
}

/// <summary>
/// State of map after single simulation turn.
/// </summary>
public class TurnLog
{
    /// <summary>
    /// Text representation of moving object in this turn.
    /// CurrentMappable.ToString()
    /// </summary>
    public required string Mappable { get; init; }
    /// <summary>
    /// Text representation of move in this turn.
    /// CurrentMoveName.ToString();
    /// </summary>
    public required string Move { get; init; }
    /// <summary>
    /// Dictionary of IMappable.Symbol on the map in this turn.
    /// </summary>
    public required Dictionary<Point, char> Symbols { get; init; }
}