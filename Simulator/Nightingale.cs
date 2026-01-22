namespace Simulator;

public class Nightingale : Birds
{
    public override char MapSymbol => 'N';
    public Nightingale() { Description = "Nightingale"; CanFly = true; }

}
