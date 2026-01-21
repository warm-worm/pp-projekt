namespace Simulator;

public class Orc : Creature
{
    private int _rage;
    private int _huntCount = 0;

    public int Rage
    {
        get => _rage;
        set => _rage = Validator.Limiter(value, 0, 20);
    }

    public Orc() : base() 
    {
        CalculatePower = () => 7 * Level + 3 * Rage;
    }

    public int AttackStrength => Rage * 2 + Level; //do ataku używamy Rage i Level

    public override void Attack(Creature target) //atak na inna istote
    {
        if (IsDead) return;
        target.TakeDamage(AttackStrength);
    }

    public Orc(string name, int level = 1, int rage = 1) : base(name, level)//tu zmiany, żeby biome wpływał na rage
    { 
        Rage = rage;
        CalculatePower = () => {
            var (_, rageMod) = WorldSettings.GetModifiers();
            // Power bierze pod uwagę bazowy Rage + bonus z biomu
            int effectiveRage = Validator.Limiter(Rage + rageMod, 0, 20);
            return 7 * Level + 3 * effectiveRage;
        };
    }

    public void Hunt()
    {
        _huntCount++;
        if (_huntCount % 2 == 0)
            _rage = Validator.Limiter(Rage + 1, 0, 10);

        //Console.WriteLine($"{Name} is hunting.");
    }

    public override string Greeting() => $"Hi, I'm {Name}, my level is {Level}, my rage is {Rage}.";

    public override string Info => $"{Name} [{Level}][{Rage}]";

    public override char MapSymbol => 'O';
}
