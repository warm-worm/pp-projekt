namespace SimConsole;

using Simulator;
using Simulator.Maps;
using System.Text;

internal class LogVisualizer
{
    SimulationLog Log { get; }

    public LogVisualizer(SimulationLog log)
    {
        Log = log;
    }

    public void Draw(int turnIndex)
    {
        if (turnIndex < 0 || turnIndex >= Log.TurnLogs.Count)
        {
            Console.WriteLine($"Turn {turnIndex} is out of range.");
            return;
        }

        var turn = Log.TurnLogs[turnIndex];

        Console.WriteLine($"\n--- TURN {turnIndex} ---");
        Console.WriteLine($"Action: {turn.Mappable} goes {turn.Move}");

        // Rysowanie mapy
        Console.Write(Box.TopLeft);
        for (int x = 0; x < Log.SizeX - 1; x++) Console.Write($"{Box.Horizontal}{Box.TopMid}");
        Console.WriteLine($"{Box.Horizontal}{Box.TopRight}");

        for (int y = Log.SizeY - 1; y >= 0; y--)
        {
            Console.Write(Box.Vertical);
            for (int x = 0; x < Log.SizeX; x++)
            {
                var p = new Point(x, y);
                string symbol = " ";

                if (turn.Symbols.TryGetValue(p, out string? mapSymbols))
                {
                    // Jeśli jest więcej niż jeden symbol, bierzemy pierwszy (np. z "EO" bierzemy 'E')
                    symbol = mapSymbols.Length > 1 ? mapSymbols[0].ToString() : mapSymbols;
                }

                Console.Write($"{symbol}{Box.Vertical}");
            }
            Console.WriteLine();

            if (y > 0)
            {
                Console.Write(Box.MidLeft);
                for (int x = 0; x < Log.SizeX - 1; x++) Console.Write($"{Box.Horizontal}{Box.Cross}");
                Console.WriteLine($"{Box.Horizontal}{Box.MidRight}");
            }
        }

        Console.Write(Box.BottomLeft);
        for (int x = 0; x < Log.SizeX - 1; x++) Console.Write($"{Box.Horizontal}{Box.BottomMid}");
        Console.WriteLine($"{Box.Horizontal}{Box.BottomRight}");

        // WYŚWIETLANIE DODATKOWYCH INFORMACJI
        Console.WriteLine("Events:");
        foreach (var e in turn.Events) Console.WriteLine($" - {e}");

        Console.WriteLine("Stats:");
        Console.WriteLine(string.Join(" | ", turn.Stats));
    }
}
