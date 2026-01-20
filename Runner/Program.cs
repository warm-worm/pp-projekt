using Simulator;
using Simulator.Maps;

namespace Runner;

internal class Program
{
    /*static void Main(string[] args)
    {
       List<Creature> creatures =
       [
            new Elf("Elrond", 5, 7),
            new Orc("Gorgul", 4, 6),
            new Elf("Legolas", 6, 8),
            new Orc("Thrall", 5, 7),
            new Elf("Tauriel", 4, 5),
            new Orc("Azog", 6, 9)
        ];
        creatures.ForEach(c => Console.WriteLine($"{c,-20} power: {c.Power}"));

        Console.WriteLine("\nPosortowane wg mocy:\n");

        creatures.Sort((x, y) => y.Power - x.Power);
        creatures.ForEach(c => Console.WriteLine($"{c,-20} power: {c.Power}"));

        Console.WriteLine("\nPosortowane wg imion:\n");
        creatures.Sort((x, y) => x.Name.CompareTo(y.Name));
        creatures.ForEach(c => Console.WriteLine($"{c,-20} name: {c.Power}"));
    }   */

    //test walki 
    static void Main(string[] args)
    {
        Console.WriteLine("=== TEST 1: WZAJEMNA WALKA ===");
        TestMutualAttack();

        Console.WriteLine("\n=== TEST 2: ŚMIERĆ I BRAK ATAKU ZZA GROBU ===");
        TestInstaKill();
    }

    static void TestMutualAttack()
    {
        // Tworzymy mapę i dwie silne jednostki
        var map = new SmallTorusMap(8, 6);
        var orc = new Orc("Gorgul", 1, 5); // Power 22
        var elf = new Elf("Elrond", 1, 5); // Power 18

        Point p1 = new Point(2, 2);
        Point p2 = new Point(2, 3);

        orc.InitMapAndPosition(map, p1);
        elf.InitMapAndPosition(map, p2);

        Console.WriteLine($"Start: {orc.Name} HP: {orc.Health} Power:{orc.Power}, {elf.Name} HP: {elf.Health} Power:{elf.Power}" );

        // Orc wchodzi na pole Elfa (ruch z (2,2) na (2,3), zatem Orc atakuje Elfa)
        Console.WriteLine($"{orc.Name} idzie na {p2}...");
        map.Move(orc, p1, p2);

        Console.WriteLine($"Po walce: {orc.Name} HP: {orc.Health}, {elf.Name} HP: {elf.Health}");

        if (orc.Health < 100 && elf.Health < 100)
            Console.WriteLine("WYNIK: Obaj odnieśli obrażenia.");
    }

    static void TestInstaKill()
    {
        var map = new SmallTorusMap(8, 6);
        // Orc jest super silny, Elf ma 1 HP
        var opOrc = new Orc("OP-Gorgul", 10, 10);
        var weakElf = new Elf("Słaby-Elf", 1, 1);
        weakElf.Health = 1; // Ustawiamy mu ręcznie 1 HP dla testu

        Point p1 = new Point(0, 0);
        Point p2 = new Point(0, 1);

        opOrc.InitMapAndPosition(map, p1);
        weakElf.InitMapAndPosition(map, p2);

        int orcHpBefore = opOrc.Health;
        Console.WriteLine($"Start: Orc HP: {orcHpBefore}, Elf HP: {weakElf.Health}");

        // Orc wchodzi i zabija Elfa jednym hitem
        map.Move(opOrc, p1, p2);

        Console.WriteLine($"Po walce: Elf IsDead: {weakElf.IsDead}, Orc HP: {opOrc.Health}");

        if (weakElf.IsDead && opOrc.Health == orcHpBefore)
            Console.WriteLine("WYNIK: Sukces! Yay! Elf zginął i nie zdążył oddać (Orc nie stracił HP).");
        else
            Console.WriteLine("WYNIK: Coś nie tak – Orc stracił HP mimo że zabił przeciwnika jednym ciosem.");
    }
}
