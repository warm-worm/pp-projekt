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
        WorldSettings.IsCatMode = IsCatMode;
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
            HttpContext.Session.SetInt32("IsCatMode", !isCat ? 1 : 0);

            // Aktualizujemy od razu
            WorldSettings.IsCatMode = !isCat;
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
    // W pliku Index.cshtml.cs

    // W Index.cshtml.cs

    // Podmieñ tê metodê w pliku Index.cshtml.cs

    private SimulationLog GetSimulationLog()
    {
        if (_cachedLog != null) return _cachedLog;

        SmallTorusMap map = new(8, 6);
        List<IMappable> mappables = new();
        List<Point> points = new();

        // CENTRUM WYDARZEÑ TO PUNKT (3,3)
        // Ustawiamy postacie bardzo blisko, ¿eby od razu by³a akcja.

        // 1. ORK (Startuje na 2,3 - krok od centrum)
        mappables.Add(new Orc("Gorbag", 1, 5));
        points.Add(new(2, 3));

        // 2. ELF (Startuje na 4,3 - krok od centrum)
        mappables.Add(new Elf("Legolas", 1, 5));
        points.Add(new(4, 3));

        // 3. ZWIERZÊTA (Startuj¹ góra/dó³ od centrum)
        switch (WorldSettings.CurrentBiome)
        {
            case Biome.Forest:
                mappables.Add(new Rabbit()); points.Add(new(3, 2)); // Królik
                mappables.Add(new Nightingale()); points.Add(new(3, 4)); // S³owik
                break;
            case Biome.Mountains:
                mappables.Add(new Goat()); points.Add(new(3, 2)); // Koza
                mappables.Add(new Eagle()); points.Add(new(3, 4)); // Orze³
                break;
            case Biome.Snowland:
                mappables.Add(new Penguin()); points.Add(new(3, 2));
                mappables.Add(new Penguin()); points.Add(new(3, 4));
                break;
        }

        // --- CHOREOGRAFIA RUCHÓW (TANIEC) ---
        // Sekwencja: r (Ork), l (Elf), u (Zwierzê1), d (Zwierzê2)

        // Tura 1 "DO ŒRODKA": rlud -> Wszyscy wchodz¹ na (3,3). Ork atakuje!
        // Tura 2 "NA ZEWN¥TRZ": lrdu -> Wszyscy wracaj¹ na start.
        // Powtarzamy to 5 razy.

        string approach = "rlud"; // Wszyscy id¹ do (3,3)
        string retreat = "lrdu"; // Wszyscy wracaj¹ na swoje miejsca

        string moves = "";
        for (int i = 0; i < 5; i++)
        {
            moves += approach + retreat;
        }

        Simulation simulation = new(map, mappables, points, moves);
        _cachedLog = new SimulationLog(simulation);

        return _cachedLog;
    }
}