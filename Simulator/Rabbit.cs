namespace Simulator;

public class Rabbit : PackAnimal
{
    public override char MapSymbol => 'R';
    public Rabbit() { Description = "Rabbit Pack"; Count = 10; }

}