namespace Simulator;

public class Eagle : Birds
{
    public override char MapSymbol => 'J';
    public Eagle() { Description = "Eagle"; CanFly = true; }

}