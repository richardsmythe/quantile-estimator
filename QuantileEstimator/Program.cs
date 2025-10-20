using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        double p = 0.75;
        int iterations = 800;
        double cumulative_sum = 0.0;
        List<double> aggregatedSequence = new List<double>();

        for (int i = 1; i <= iterations; i++)
        {
            QuantileEstimator estimator = new QuantileEstimator(p);
            List<double> sequence = GetRandomizedSequence();
            aggregatedSequence.AddRange(sequence);

            foreach (var value in sequence)
            {
                estimator.Add(value);
            }

            double estimate = estimator.Quantile;
            cumulative_sum += estimate;
            Console.WriteLine($"Iteration {i}:");
            Console.WriteLine($"Final estimate for p={p}: {estimate}\n");
        }

        double exactQuantile = GetExactQuantile(aggregatedSequence, p);
        Console.WriteLine($"\nExact quantile for p={p}: {exactQuantile}");
        Console.WriteLine($"Average quantil estimate: {cumulative_sum / iterations}");
    }

    private static List<double> GetRandomizedSequence()
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
