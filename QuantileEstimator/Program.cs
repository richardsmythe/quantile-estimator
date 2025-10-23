using System.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        double p = 0.75;
        bool enableDebug = false;      
        RunTest(enableDebug,p);  
        RunConvergenceTest(p);
    }

    private static void RunTest(bool enableDebug,double p)
    {
        int dataset = 10000;
        int iterations =200;
        double cumulative_sum = 0.0;
        List<double> aggregatedSequence = new List<double>();
        
        Console.WriteLine($"---- TESTS ----");
        Console.WriteLine($"Iterations: {iterations}");
        Console.WriteLine($"Dataset: {dataset}");
        Console.WriteLine($"p={p}");

        for (int i = 1; i <= iterations; i++)
        {
            QuantileEstimator estimator = new QuantileEstimator(p, enableDebug);         
            List<double> sequence = GetLargerAndLessRandomSequence(i, dataset);
            
            aggregatedSequence.AddRange(sequence);

            foreach (var value in sequence)
            {
                estimator.Add(value);
            }

            double estimate = estimator.Quantile;
            cumulative_sum += estimate;            

            if (i % 10 == 0 || i == 1)
            {
                Console.WriteLine($"\nIteration {i}:");
                Console.WriteLine($"  Current average: {cumulative_sum / i}");
                Console.WriteLine($"  Final estimate for p={p}: {estimate}");

            }
        }

        double exactQuantile = GetExactQuantile(aggregatedSequence, p);
        Console.WriteLine($"\nExact quantile for p={p}: {exactQuantile}");
        Console.WriteLine($"Average quantile estimate: {cumulative_sum / iterations}");
        Console.WriteLine($"Difference: {Math.Abs(exactQuantile - (cumulative_sum / iterations))}");
        Console.WriteLine();
    }
    
    private static void RunConvergenceTest(double p)
    {
        //create a single estimator and feed it increasing amounts of data
        Console.WriteLine($"---- CONVERGENCE TEST (p={p}) -----");
        Console.WriteLine($"Testing how it converges with more data...");
        
        QuantileEstimator estimator = new QuantileEstimator(p);
        List<double> allData = new List<double>();
        
        // intervals to measure convergence
        int[] intervals = { 100, 500, 1000, 2500, 5000, 10000};
        int maxDataPoints = intervals.Max();        

        Random rng = new Random(1); // fixed seed to ensure deterministic and reproducable
        List<double> sequence = Enumerable.Range(1, maxDataPoints)
             .OrderBy(_ => rng.NextDouble())
             .Select(x => (double)x)
             .ToList();
        
        int processedPoints = 0;
        foreach (var interval in intervals)
        {
            for (int i = processedPoints; i < interval; i++)
            {
                double value = sequence[i];
                estimator.Add(value);
                allData.Add(value);
            }
            processedPoints = interval;         

            double estimate = estimator.Quantile;
            double exactQuantile = GetExactQuantile(allData, p);           
            var markers = estimator.GetCurrentMarkers();
            
            Console.WriteLine($"\nAfter {interval} data points:");
            Console.WriteLine($"  Markers: [{markers.Min}, {markers.Q1}, {markers.Median}, {markers.Q3}, {markers.Max}]");
            Console.WriteLine($"  Current estimate: {estimate}");
            Console.WriteLine($"  Exact quantile: {exactQuantile}");
            Console.WriteLine($"  Difference: {Math.Abs(exactQuantile - estimate)}");
            Console.WriteLine($"  Relative error: {Math.Abs((exactQuantile - estimate) / exactQuantile):P2}\n");
        }
    }

    private static List<double> GetLargerAndLessRandomSequence(int seed, int dataset)
    {
        Random rng = new Random(seed);
        var samples = Enumerable.Range(1, dataset).OrderBy(_ => rng.NextDouble()).Select(x => (double)x).ToList();
        return samples;
    }

    private static double GetExactQuantile(List<double> data, double p)
    {
        var sorted = data.OrderBy(x => x).ToList();
        double pos = (sorted.Count - 1) * p;
        int lower = (int)Math.Floor(pos);
        int upper = (int)Math.Ceiling(pos);
        if (lower == upper) return sorted[lower];
        return sorted[lower] + (sorted[upper] - sorted[lower]) * (pos - lower);
    }
}
