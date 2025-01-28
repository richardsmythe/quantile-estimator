using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        double p = 0.25;
        QuantileEstimator estimator = new QuantileEstimator(p);
        List<double> sequence = GetHardcodedSequence();
        foreach (var value in sequence)
        {
            Console.Write(value + " ");
            estimator.Add(value);
        }
        Console.WriteLine();
        Console.WriteLine($"\nFinal estimate for p={p}: {estimator.Quantile}");
    }

    private static List<double> GetHardcodedSequence()
    {
        var samples = Enumerable.Range(1, 100)
                 .OrderBy(x => Guid.NewGuid())
                 .Select(x => (double)x)
                 .ToList();
        return samples;
    }
}
