namespace SimConsole;

using Simulator;
using Simulator.Maps;
using System.Text;

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8; // zeby dzialaly znaki ramek

        Console.WriteLine("Select simulation:"); //mam nadzieje ze po angielsku jest ok..
        Console.WriteLine("1. Creatures");
        Console.WriteLine("2. Birds & Animals");
        Console.WriteLine("3. Simulation History (Log)"); // nowa opcja

        var key = Console.ReadKey();
        Console.WriteLine();

        if (key.KeyChar == '1') Sim1();
        else if (key.KeyChar == '2') Sim2();
        else if (key.KeyChar == '3') Sim3();
        else Console.WriteLine("Invalid choice.");
    }

    static void Sim1()
    {
        SmallSquareMap map = new(5);
        List<IMappable> creatures = [new Orc("Gorbag"), new Elf("Elandor")];
        List<Point> points = [new(2, 2), new(3, 1)];
        string moves = "dlrludl";

        Simulation simulation = new(map, creatures, points, moves);
        MapVisualizer visualizer = new(simulation.Map);
        Run(simulation, visualizer);
    }

    static void Sim2()
    {
        SmallTorusMap map = new(8, 6);
        List<IMappable> mappables = [
            new Elf("Elandor"),
            new Orc("Gorbag"),
            new Animals { Description = "Rabbits", Size = 10 },
            new Birds { Description = "Eagles", CanFly = true },
            new Birds { Description = "Ostriches", CanFly = false }
        ];
        List<Point> points = [new(0, 0), new(7, 5), new(2, 2), new(4, 4), new(6, 1)];
        string moves = "urdluuuuurrrddlldd";

        Simulation simulation = new(map, mappables, points, moves);
        MapVisualizer visualizer = new(simulation.Map);
        Run(simulation, visualizer);
    }

    static void Sim3()
    {
        Console.WriteLine("Running simulation in background...");

        // setup taki sam jak w Sim2
        SmallTorusMap map = new(8, 6);
        List<IMappable> mappables = [
            new Elf("Elandor"),
            new Orc("Gorbag"),
            new Animals { Description = "Rabbits", Size = 10 },
            new Birds { Description = "Eagles", CanFly = true },
            new Birds { Description = "Ostriches", CanFly = false }
        ];
        List<Point> points = [new(0, 0), new(7, 5), new(2, 2), new(4, 4), new(6, 1)];
        string moves = "urdluuuuurrrddlldd";

        Simulation simulation = new(map, mappables, points, moves);

        // tworzymy log (to wykona cala symulacje od razu w tle)
        SimulationLog log = new(simulation);
        LogVisualizer logVisualizer = new(log);

        
        Console.WriteLine("\n--- HISTORY VIEWER ---");

        logVisualizer.Draw(5);
        Console.WriteLine("\nPress any key for turn 10...");
        Console.ReadKey();

        logVisualizer.Draw(10);
        Console.WriteLine("\nPress any key for turn 15...");
        Console.ReadKey();

        logVisualizer.Draw(15);
        Console.WriteLine("\nPress any key for turn 20...");
        Console.ReadKey();

        logVisualizer.Draw(20);

        Console.WriteLine("\nEnd of history.");
    }

    static void Run(Simulation simulation, MapVisualizer visualizer)
    {
        visualizer.Draw();
        while (!simulation.Finished)
        {
            Console.WriteLine("Press any key...");
            Console.ReadKey();
            Console.WriteLine($"\nTurn: {simulation.CurrentMappable.ToString()} goes {simulation.CurrentMoveName}");
            simulation.Turn();
            visualizer.Draw();
        }
        Console.WriteLine("Finished!");
    }
}