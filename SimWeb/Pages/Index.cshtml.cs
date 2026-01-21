namespace SimWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simulator;
using Simulator.Maps;

public class IndexModel : PageModel
{
    // cache symulacji - statyczny, zeby nie liczyc tego samego co chwile
    private static SimulationLog? _cachedLog;

    // --- NOWE POLA DO WIDOKU ---
    // Czy w³¹czony jest tryb kota?
    public bool IsCatMode { get; private set; }
    // Jaki biom jest wybrany (do kolorowania guzików)
    public string CurrentBiomeName { get; private set; } = "Forest";

    // przechowujemy biezaca ture (z logu) do wyswietlenia
    public TurnLog CurrentTurn { get; private set; } = default!;

    // numer tury
    public int TurnIndex { get; private set; }

    // calkowita liczba tur
    public int TotalTurns { get; private set; }

    // rozmiary mapy zeby wiedziec jak rysowac
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }

    public void OnGet()
    {
        // pobieramy indeks tury z sesji (jesli nie ma, to 0)
        TurnIndex = HttpContext.Session.GetInt32("TurnIndex") ?? 0;

        // --- ODCZYT USTAWIEÑ ---
        IsCatMode = (HttpContext.Session.GetInt32("IsCatMode") ?? 0) == 1;
        string biomeStr = HttpContext.Session.GetString("Biome") ?? "Forest";
        CurrentBiomeName = biomeStr;

        if (Enum.TryParse(biomeStr, out Biome biomeEnum))
        {
            WorldSettings.CurrentBiome = biomeEnum;
        }

        // przygotowujemy symulacje
        var simulationLog = GetSimulationLog();

        // zabezpieczamy zakresy (zeby nie wyjsc poza liste)
        TotalTurns = simulationLog.TurnLogs.Count - 1;
        if (TurnIndex < 0) TurnIndex = 0;
        if (TurnIndex > TotalTurns) TurnIndex = TotalTurns;

        // pobieramy dane dla aktualnej tury
        CurrentTurn = simulationLog.TurnLogs[TurnIndex];
        SizeX = simulationLog.SizeX;
        SizeY = simulationLog.SizeY;
    }

    // obsluga przyciskow (next / prev / zmiany mapy / koty)
    public IActionResult OnPost(string action, string map, string toggleCats)
    {
        // 1. ZMIANA MAPY (BIOMU)
        if (!string.IsNullOrEmpty(map))
        {
            HttpContext.Session.SetString("Biome", map);
            // Resetujemy cache, ¿eby przy prze³adowaniu stworzy³a siê nowa mapa z nowymi zwierzêtami!
            _cachedLog = null;
            HttpContext.Session.SetInt32("TurnIndex", 0); // Wracamy na start

            // Aktualizujemy WorldSettings od razu
            if (Enum.TryParse(map, out Biome biomeEnum))
            {
                WorldSettings.CurrentBiome = biomeEnum;
            }
        }

        // 2. PRZE£¥CZNIK KOTÓW
        if (toggleCats == "true")
        {
            bool isCat = (HttpContext.Session.GetInt32("IsCatMode") ?? 0) == 1;
            // Zmieniamy na przeciwny (w³¹cz/wy³¹cz)
            HttpContext.Session.SetInt32("IsCatMode", !isCat ? 1 : 0);
        }

        // 3. NAWIGACJA (NEXT/PREV)
        // pobieramy aktualny stan
        int current = HttpContext.Session.GetInt32("TurnIndex") ?? 0;

        // musimy wiedziec ile jest max tur
        var log = GetSimulationLog();
        int max = log.TurnLogs.Count - 1;

        // zmieniamy licznik w zaleznosci od przycisku
        if (action == "next")
        {
            if (current < max) current++;
        }
        else if (action == "prev")
        {
            if (current > 0) current--;
        }

        // zapisujemy nowy stan w sesji
        HttpContext.Session.SetInt32("TurnIndex", current);

        // przeladowujemy strone (wzorzec PRG - Post Redirect Get)
        return RedirectToPage();
    }

    // metoda pomocnicza tworzaca konkretna symulacje
    private SimulationLog GetSimulationLog()
    {
        // jak juz mamy policzone to zwracamy gotowca
        if (_cachedLog != null) return _cachedLog;

        SmallTorusMap map = new(8, 6);
        List<IMappable> mappables = new();
        List<Point> points = new();

        // 1. Bohaterowie (s¹ zawsze, niezale¿nie od mapy)
        mappables.Add(new Orc("Gorbag")); points.Add(new(2, 2));
        mappables.Add(new Elf("Elandor")); points.Add(new(3, 1));

        // 2. Zwierzêta - zale¿ne od BIOMU
        switch (WorldSettings.CurrentBiome)
        {
            case Biome.Forest:
                // Las: Króliki, Or³y, Strusie
                mappables.Add(new Animals { Description = "Rabbits", Size = 10 }); points.Add(new(5, 5));
                mappables.Add(new Birds { Description = "Eagles", CanFly = true }); points.Add(new(7, 3));
                mappables.Add(new Birds { Description = "Ostriches", CanFly = false }); points.Add(new(0, 4));
                break;

            case Biome.Mountains:
                // Góry: Kozy (zamiast królików) i Or³y
                mappables.Add(new Animals { Description = "Goats", Size = 5 }); points.Add(new(6, 5));
                mappables.Add(new Birds { Description = "Eagles", CanFly = true }); points.Add(new(1, 1));
                break;

            case Biome.Snowland:
                // Œnieg: Pingwiny i Lodowe Wilki
                mappables.Add(new Birds { Description = "Penguins", CanFly = false }); points.Add(new(4, 4));
                mappables.Add(new Animals { Description = "IceWolf", Size = 15 }); points.Add(new(7, 0));
                break;

            default: // Domyœlnie Las
                mappables.Add(new Animals { Description = "Rabbits" }); points.Add(new(5, 5));
                break;
        }

        string moves = "dlrludluddlrulr"; // 15 rund

        Simulation simulation = new(map, mappables, points, moves);

        // log (symulacja wykona sie w tle cala od razu) i zapisujemy do statica
        _cachedLog = new SimulationLog(simulation);

        return _cachedLog;
    }
}