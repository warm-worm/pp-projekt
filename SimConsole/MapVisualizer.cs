namespace SimConsole;

using Simulator;
using Simulator.Maps;
using System.Text;

public class MapVisualizer
{
    private readonly Map _map;

    public MapVisualizer(Map map)
    {
        _map = map;
    }

    public void Draw()
    {
        // gora ramki
        Console.Write(Box.TopLeft);
        for (int x = 0; x < _map.SizeX - 1; x++)
        {
            Console.Write($"{Box.Horizontal}{Box.TopMid}");
        }
        Console.WriteLine($"{Box.Horizontal}{Box.TopRight}");

        // srodek
        for (int y = _map.SizeY - 1; y >= 0; y--) // rysujemy od gory (czyli od max Y)
        {
            Console.Write(Box.Vertical);
            for (int x = 0; x < _map.SizeX; x++)
            {
                // to lista IMappable, wiec zmienna mappables
                var mappables = _map.At(x, y);
                char symbol = ' '; // puste pole

                if (mappables.Count == 1)
                {
                    symbol = mappables[0].MapSymbol; // jeden obiekt
                }
                else if (mappables.Count > 1)
                {
                    symbol = 'X'; // wiele obiektow
                }

                Console.Write($"{symbol}{Box.Vertical}");
            }
            Console.WriteLine();

            // linia oddzielajaca (jesli to nie ostatni wiersz)
            if (y > 0)
            {
                Console.Write(Box.MidLeft);
                for (int x = 0; x < _map.SizeX - 1; x++)
                {
                    Console.Write($"{Box.Horizontal}{Box.Cross}");
                }
                Console.WriteLine($"{Box.Horizontal}{Box.MidRight}");
            }
        }

        // dol ramki
        Console.Write(Box.BottomLeft);
        for (int x = 0; x < _map.SizeX - 1; x++)
        {
            Console.Write($"{Box.Horizontal}{Box.BottomMid}");
        }
        Console.WriteLine($"{Box.Horizontal}{Box.BottomRight}");
    }
}

// klasa pomocnicza z znakami ramek
public static class Box
{
    public const char
        Horizontal = '\u2500',
        Vertical = '\u2502',
        Cross = '\u253C',
        TopLeft = '\u250C',
        TopRight = '\u2510',
        TopMid = '\u252C',
        BottomLeft = '\u2514',
        BottomMid = '\u2534',
        BottomRight = '\u2518',
        MidLeft = '\u251C',
        MidRight = '\u2524';
}