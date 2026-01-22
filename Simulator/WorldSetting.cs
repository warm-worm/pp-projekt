using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator;

public enum Biome
{
    Forest,
    Mountains,
    Snowland
}

public static class WorldSettings
{
    public static Biome CurrentBiome { get; set; } = Biome.Forest;
    public static bool IsCatMode { get; set; } = false;

    // Metoda pomocnicza do pobierania modyfikatorów
    public static (int agilityMod, int rageMod) GetModifiers() => CurrentBiome switch
    {
        Biome.Forest => (1, -1),
        Biome.Mountains => (-1, 1),
        Biome.Snowland => (-1, -1),
        _ => (0, 0)
    };
}
