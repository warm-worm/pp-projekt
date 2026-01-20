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
        _simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        SizeX = _simulation.Map.SizeX;
        SizeY = _simulation.Map.SizeY;
        Run();
    }

    private void Run()
    {
        TurnLogs.Add(new TurnLog
        {
            Mappable = "START",
            Move = "INIT",
            Symbols = GetMapSymbols()
        });

        while (!_simulation.Finished)
        {
            var currentMappable = _simulation.CurrentMappable.ToString();
            var currentMove = _simulation.CurrentMoveName;

            _simulation.Turn();

            TurnLogs.Add(new TurnLog
            {
                Mappable = currentMappable,
                Move = currentMove,
                Symbols = GetMapSymbols()
            });
        }
    }

    private Dictionary<Point, string> GetMapSymbols()
    {
        var symbols = new Dictionary<Point, string>();

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                var objects = _simulation.Map.At(x, y)
                    .Where(m => m is not Creature c || !c.IsDead)
                    .ToList();

                if (objects.Count > 0)
                {
                    var shortSymbols = objects.Select(o => o.MapSymbol.ToString())
                                              .OrderBy(s => s)
                                              .ToList();

                    string code = string.Join("", shortSymbols);

                    symbols.Add(new Point(x, y), code);
                }
            }
        }
        return symbols;
    }
}

public class TurnLog
{
    public required string Mappable { get; init; }
    public required string Move { get; init; }
    public required Dictionary<Point, string> Symbols { get; init; }
}