namespace Simulator.Maps;

using Simulator; // zeby dzialalo Point i Direction i dziedziczylo po Map!
using static System.Net.Mime.MediaTypeNames;

public class SmallSquareMap : Map
{

    public SmallSquareMap(int size) : base(size, size) // wywolanie konstruktora klasy bazowej
    {
        if (size > 20) // rozmiar musi byc mniejszy niz 20
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be below 20."); // wyjatek jak zly rozmiar
        }
    }

    public override Point Next(Point p, Direction d)
    {
        Point next = p.Next(d); // obliczamy gdzie by poszedl
        
        return Exist(next) ? next : p;
        
    }

    public override Point NextDiagonal(Point p, Direction d)
    {
        Point next = p.NextDiagonal(d); // obliczamy gdzie by poszedl po skosie
        return Exist(next) ? next : p;
    }
}