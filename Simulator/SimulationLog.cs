namespace Simulator;

using Simulator.Maps;
using System.Linq; // Potrzebne do LINQ (Select, OrderBy, Where)

public class SimulationLog
{
    private Simulation _simulation { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public List<TurnLog> TurnLogs { get; } = new();

    public SimulationLog(Simulation simulation)
    {
        _simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        SizeX = _simulation.Map.SizeX;
        SizeY = _simulation.Map.SizeY;
        Run();
    }

    private void Run()
    {
        // 1. Log stanu początkowego (Tura 0)
        TurnLogs.Add(new TurnLog(
            "START", "INIT",
            GetMapSymbols(),
            new List<string> { "START SYMULACJI" },
            CollectStats(_simulation.Mappables)
        ));

        // 2. Pętla symulacji
        while (!_simulation.Finished)
        {
            // Zapamiętujemy kto i jak się rusza ZANIM wykonamy Turn()
            var currentMappable = _simulation.CurrentMappable.ToString();
            var currentMove = _simulation.CurrentMoveName;

            _simulation.Turn();

            TurnLogs.Add(new TurnLog(
                currentMappable,
                currentMove,
                GetMapSymbols(),
                _simulation.Map.GetTurnLogs(),
                CollectStats(_simulation.Mappables)
            ));
        }
    }

    private Dictionary<Point, string> GetMapSymbols()
    {
        var symbols = new Dictionary<Point, string>();

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                var objects = _simulation.Map.At(x, y); // Pobieramy wszystko co jest na polu

                // Filtrujemy: pomijamy martwe stwory i zniszczone zwierzęta
                var activeObjects = objects.Where(m =>
                {
                    if (m is Creature c && c.IsDead) return false;
                    if (m is Animals a && a.Health <= 0) return false; // Animals mają Health=0 jak zginą? Albo Count=0
                    return true;
                }).ToList();

                if (activeObjects.Count > 0)
                {
                    // Sortujemy symbole alfabetycznie, żeby np. "OE" i "EO" zawsze wyglądało tak samo ("EO")
                    var shortSymbols = activeObjects
                        .Select(o => o.MapSymbol.ToString())
                        .OrderBy(s => s)
                        .ToList();

                    // Łączymy w jeden string, np "R" albo "OEG"
                    string code = string.Join("", shortSymbols);
                    symbols.Add(new Point(x, y), code);
                }
            }
        }
        return symbols;
    }

    // Metoda pomocnicza do zbierania statystyk tekstowych
    private List<string> CollectStats(List<IMappable> mappables)
    {
        var stats = new List<string>();
        foreach (var m in mappables)
        {
            if (m is Creature c)
            {
                // Format: "Nazwa: HP X, Stat Y"
                string info = $"{c.Name}: HP {c.Health}";

                if (c is Orc o) info += $", Rage {o.Rage}";
                if (c is Elf e) info += $", Agi {e.Agility}";

                if (c.IsDead) info += " 💀"; // Oznaczenie martwego
                stats.Add(info);
            }
            else if (m is PackAnimal p)
            {
                // Dla zwierząt stadnych pokazujemy liczebność
                if (p.Count > 0)
                    stats.Add($"{p.Description}: {p.Count} szt.");
            }
            // Możesz tu dodać else if dla Birds, jeśli chcesz
        }
        return stats;
    }
}
public class TurnLog
{
    public string Mappable { get; } // Kto się ruszył
    public string Move { get; }     // Jaki ruch wykonał
    public Dictionary<Point, string> Symbols { get; }
    public List<string> Events { get; }
    public List<string> Stats { get; }

    public TurnLog(string mappable, string move, Dictionary<Point, string> symbols, List<string> events, List<string> stats)
    {
        Mappable = mappable;
        Move = move;
        Symbols = symbols;
        Events = events;
        Stats = stats;
    }
}