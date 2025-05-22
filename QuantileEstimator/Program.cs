using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        double p = 0.75;
        int iterations = 8;
        for (int i = 1; i <= iterations; i++)
        {
            QuantileEstimator estimator = new QuantileEstimator(p);
            List<double> sequence = GetHardcodedSequence();
            foreach (var value in sequence)
            {
                estimator.Add(value);
            }
            double estimate = estimator.Quantile;
            double exactQuantile = GetExactQuantile(sequence, p);
            Console.WriteLine($"Iteration {i}:");
            Console.WriteLine($"  Final estimate for p={p}: {estimate}");
            Console.WriteLine($"  Exact quantile for p={p}: {exactQuantile}\n");
        }
    }

    private static List<double> GetHardcodedSequence()
    {
        var samples = Enumerable.Range(1, 1000)
                 .OrderBy(x => Guid.NewGuid())
                 .Select(x => (double)x)
                 .ToList();
        return samples;
    }

    private static double GetExactQuantile(List<double> data, double p)
    {
        var sorted = data.OrderBy(x => x).ToList();
        double pos = (sorted.Count - 1) * p;
        int lower = (int)Math.Floor(pos);
        int upper = (int)Math.Ceiling(pos);
        if (lower == upper)
            return sorted[lower];
        return sorted[lower] + (sorted[upper] - sorted[lower]) * (pos - lower);
    }
}
