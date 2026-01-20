namespace Simulator;

public class Elf : Creature
{
    private int _agility;
    private int _singCount = 0;

    public int Agility
    {
        get => _agility;
        init => _agility = Validator.Limiter(value, 0, 10);
    }

    public Elf() : this("Elf") { }
    public Elf(string name, int level = 1, int agility = 1) : base(name, level)
    {
        Agility = agility;
        CalculatePower = () => Level * 8 + Agility * 2;
    }


    public int AttackStrength => Agility * 3 + Level; //elf używa Agility i Level do ataku

    public override void Attack(Creature target) //atak na inna istote
    {
        if (IsDead) return;
        target.TakeDamage(AttackStrength);
    }

    public void Sing()
    {
        _singCount++;
        if (_singCount % 3 == 0)
            _agility = Validator.Limiter(Agility + 1, 0, 10);

        //Console.WriteLine($"{Name} is singing.");
    }

    public override string Greeting() => $"Hi, I'm {Name}, my level is {Level}, my agility is {Agility}.";
    public override string Info => $"{Name} [{Level}][{Agility}]";

    public override char MapSymbol => 'E';
}