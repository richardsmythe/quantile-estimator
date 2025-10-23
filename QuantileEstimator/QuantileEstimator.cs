using System;

public class QuantileEstimator
{
    private readonly double p;
    private int N = 0;
    private double Q0, Q1, Q2, Q3, Q4;
    private double pos0 = 1, pos1 = 2, pos2 = 3, pos3 = 4, pos4 = 5;
    private double desPos0, desPos1, desPos2, desPos3, desPos4;
    private double inc0, inc1, inc2, inc3, inc4;

    private bool debugMode = false;
    private int debugInterval = 1000;

    /// <summary>Final quantile estimate.</summary>
    public double Quantile => GetQuantile();

    /// <summary>Quantile estimator class that estimates the pth quantile of a distribution.</summary>
    public QuantileEstimator(double p, bool enableDebug = false)
    {
        if (p < 0 || p > 1) throw new ArgumentException("p must be between 0 and 1.");
        this.p = p;
        this.debugMode = enableDebug;
        SetIncrements(p);
    }

    private void SetIncrements(double p)
    {
        desPos0 = pos0;
        desPos1 = pos1;
        desPos2 = pos2;
        desPos3 = pos3;
        desPos4 = pos4;
        inc0 = 0;
        inc1 = p / 2;
        inc2 = p;
        inc3 = (1 + p) / 2;
        inc4 = 1;

        if (debugMode)
        {
            Console.WriteLine($"Initial setup:");
            Console.WriteLine($"  Markers: Q0-Q4 = [uninitialized]");
            Console.WriteLine($"  Positions: pos0-pos4 = [{pos0}, {pos1}, {pos2}, {pos3}, {pos4}]");
            Console.WriteLine($"  Increments: inc0-inc4 = [{inc0}, {inc1}, {inc2}, {inc3}, {inc4}]");
        }
    }

    /// <summary>
    /// Adds a sample to the estimator and updates the markers dynamically.
    /// </summary>
    public void Add(double s)
    {
        N++;
        if (N <= 5)
        {
            switch (N)
            {
                case 1: Q0 = s; break;
                case 2: Q1 = s; break;
                case 3: Q2 = s; break;
                case 4: Q3 = s; break;
                case 5: 
                    Q4 = s; 
                    SortMarkers();
                    if (debugMode)
                    {
                        Console.WriteLine($"Initial 5 markers sorted:");
                        Console.WriteLine($"  Q0-Q4 = [{Q0}, {Q1}, {Q2}, {Q3}, {Q4}]");
                    }
                    break;
            }
            return;
        }
        UpdateMarkers(s);
        if (debugMode && (N % debugInterval == 0))
        {
            Console.WriteLine($"After {N} samples:");
            Console.WriteLine($"  Markers: Q0-Q4 = [{Q0}, {Q1}, {Q2}, {Q3}, {Q4}]");
            Console.WriteLine($"  Positions: pos0-pos4 = [{pos0}, {pos1}, {pos2}, {pos3}, {pos4}]");
            Console.WriteLine($"  Current quantile estimate for p={p}: {GetQuantile()}");
        }
    }

    private void SortMarkers()
    {
        double[] markers = { Q0, Q1, Q2, Q3, Q4 };
        Array.Sort(markers);
        Q0 = markers[0];
        Q1 = markers[1];
        Q2 = markers[2];
        Q3 = markers[3];
        Q4 = markers[4];
    }

    private void UpdateMarkers(double s)
    {
        bool markerValueChanged = false;
        double prevQ0 = Q0, prevQ1 = Q1, prevQ2 = Q2, prevQ3 = Q3, prevQ4 = Q4;

        if (s < Q0)
        {
            Q0 = s;
            pos1++;
            pos2++;
            pos3++;
            pos4++;
            markerValueChanged = true;
        }
        else if (s < Q1)
        {
            pos1++;
            pos2++;
            pos3++;
            pos4++;
        }
        else if (s < Q2)
        {
            pos2++;
            pos3++;
            pos4++;
        }
        else if (s < Q3)
        {
            pos3++;
            pos4++;
        }
        else if (s < Q4)
        {
            pos4++;
        }
        else
        {
            Q4 = s;
            markerValueChanged = true;
        }

        desPos0 += inc0;
        desPos1 += inc1;
        desPos2 += inc2;
        desPos3 += inc3;
        desPos4 += inc4;

        // adjust the marker positions based on difference between desired and actual position
        if (Math.Abs(desPos1 - pos1) >= 1) pos1 += Math.Sign(desPos1 - pos1);
        if (Math.Abs(desPos2 - pos2) >= 1) pos2 += Math.Sign(desPos2 - pos2);
        if (Math.Abs(desPos3 - pos3) >= 1) pos3 += Math.Sign(desPos3 - pos3);
        if (Math.Abs(desPos4 - pos4) >= 1) pos4 += Math.Sign(desPos4 - pos4);

        ValidateMarkerPositions();

        if (debugMode && markerValueChanged && (N % (debugInterval / 10) == 0))
        {
            Console.WriteLine($"Sample {N}: Direct marker update for new value {s}");
            if (s < prevQ0)
                Console.WriteLine($"  Q0 updated: {prevQ0} -> {Q0}");
            else if (s > prevQ4)
                Console.WriteLine($"  Q4 updated: {prevQ4} -> {Q4}");
        }
        AdjustMarkers();
    }

    private void ValidateMarkerPositions()
    {
        double oldPos1 = pos1, oldPos2 = pos2, oldPos3 = pos3, oldPos4 = pos4;
        pos1 = Math.Max(pos1, pos0 + 1);
        pos2 = Math.Max(pos2, pos1 + 1);
        pos3 = Math.Max(pos3, pos2 + 1);
        pos4 = Math.Max(pos4, pos3 + 1);
        if (debugMode && (oldPos1 != pos1 || oldPos2 != pos2 || oldPos3 != pos3 || oldPos4 != pos4) && (N % debugInterval == 0))
        {
            Console.WriteLine($"Sample {N}: Marker positions validated");
            if (oldPos1 != pos1) Console.WriteLine($"  pos1 adjusted: {oldPos1} -> {pos1}");
            if (oldPos2 != pos2) Console.WriteLine($"  pos2 adjusted: {oldPos2} -> {pos2}");
            if (oldPos3 != pos3) Console.WriteLine($"  pos3 adjusted: {oldPos3} -> {pos3}");
            if (oldPos4 != pos4) Console.WriteLine($"  pos4 adjusted: {oldPos4} -> {pos4}");
        }
    }

    private void AdjustMarkers()
    {
        double[] positions = { pos0, pos1, pos2, pos3, pos4 };
        double[] markers = { Q0, Q1, Q2, Q3, Q4 };
        double[] originalMarkers = { Q0, Q1, Q2, Q3, Q4 };
        for (int i = 1; i < 4; i++)
        {
            double delta = 0;
            switch (i)
            {
                case 1: delta = desPos1 - pos1; break;
                case 2: delta = desPos2 - pos2; break;
                case 3: delta = desPos3 - pos3; break;
            }
            if ((delta >= 1 && positions[i + 1] > positions[i] + 1) ||
                (delta <= -1 && positions[i - 1] < positions[i] - 1))
            {
                delta = Math.Sign(delta);
                double parabolicValue = CalculateParabolicValue(i, markers, positions, delta);
                double newValue;
                if (markers[i - 1] < parabolicValue && parabolicValue < markers[i + 1])
                {
                    newValue = parabolicValue;
                }
                else
                {
                    newValue = markers[i] + delta * (markers[i + (int)delta] - markers[i]) / (positions[i + (int)delta] - positions[i]);
                }
                positions[i] += delta;
                markers[i] = newValue;
                if (debugMode && Math.Abs(markers[i] - originalMarkers[i]) > 0.1 && (N % debugInterval == 0))
                {
                    Console.WriteLine($"Sample {N}: Marker Q{i} adjusted");
                    Console.WriteLine($"  Q{i}: {originalMarkers[i]} -> {markers[i]}");
                }
            }
        }
        Q0 = markers[0];
        Q1 = markers[1];
        Q2 = markers[2];
        Q3 = markers[3];
        Q4 = markers[4];
        pos0 = positions[0];
        pos1 = positions[1];
        pos2 = positions[2];
        pos3 = positions[3];
        pos4 = positions[4];
    }

    private double CalculateParabolicValue(int i, double[] markers, double[] positions, double delta)
    {
        double parabolicInterpolation = markers[i] + delta / (positions[i + 1] - positions[i - 1]) *
                              ((markers[i + 1] - markers[i]) * (positions[i] - positions[i - 1] + delta) +
                              (markers[i] - markers[i - 1]) * (positions[i + 1] - positions[i] - delta));
        return parabolicInterpolation;
    }

    /// <summary>
    /// Gets the quantile estimate based on the adjusted markers.
    /// </summary>
    public double GetQuantile()
    {
        if (p <= 0.0) return Q0;
        if (p >= 1.0) return Q4;
        if (p <= 0.25)
        {
            double t = 4 * p;
            return Q0 + t * (Q1 - Q0);
        }
        else if (p <= 0.5)
        {
            double t = 4 * (p - 0.25);
            return Q1 + t * (Q2 - Q1);
        }
        else if (p <= 0.75)
        {
            double t = 4 * (p - 0.5);
            return Q2 + t * (Q3 - Q2);
        }
        else
        {
            double t = 4 * (p - 0.75);
            return Q3 + t * (Q4 - Q3);
        }
    }

    public (double Min, double Q1, double Median, double Q3, double Max) GetCurrentMarkers()
    {
        return (Q0, Q1, Q2, Q3, Q4);
    }
}
