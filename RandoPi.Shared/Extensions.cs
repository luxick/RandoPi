namespace RandoPi.Shared;

public static class Extensions
{
    private static readonly Random Rng = new();
    
    public static List<T> Shuffle<T>(this List<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
        return list;
    }
}