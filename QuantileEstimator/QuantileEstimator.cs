﻿public class QuantileEstimator
{
    /// <summary>Quantile to estimate (between 0 and 1).</summary>
    private readonly double p;
    /// <summary>Number of data points added.</summary>
    private int N = 0;
    /// <summary>Markers for the five quantiles (Q0 to Q4).</summary>
    private double Q0, Q1, Q2, Q3, Q4;
    /// <summary>Positions of the five markers (initially 1 through 5).</summary>
    private double pos0 = 1, pos1 = 2, pos2 = 3, pos3 = 4, pos4 = 5;
    /// <summary>Desired positions of the markers based on quantile estimates.</summary>
    private double desPos0, desPos1, desPos2, desPos3, desPos4;
    /// <summary>Increments for adjusting marker positions.</summary>
    private double inc0, inc1, inc2, inc3, inc4;
    /// <summary>Final quantile estimate.</summary>
    public double Quantile => GetQuantile();

    /// <summary> Quantile estimator class that estimates the pth quantile of a distribution.</summary>
    public QuantileEstimator(double p)
    {
        if (p < 0 || p > 1) throw new ArgumentException("p must be between 0 and 1.");
        this.p = p;
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
                case 5: Q4 = s; SortMarkers(); break;
            }
        }
        else
        {
            UpdateMarkers(s);
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
        // Update marker positions according to the original P² algorithm
        if (s < Q0)
        {
            Q0 = s;
            pos1++;
            pos2++;
            pos3++;
            pos4++;
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
        else // s >= Q4
        {
            Q4 = s;
        }

        desPos0 += inc0;
        desPos1 += inc1;
        desPos2 += inc2;
        desPos3 += inc3;
        desPos4 += inc4;

        // Ensure marker positions remain in order
        ValidateMarkerPositions();

        AdjustMarkers();
    }

    private void ValidateMarkerPositions()
    {
        // makes sure marker positions are in ascending order,
        // if they overlap it could mess up the how it represents the sample
        pos1 = Math.Max(pos1, pos0 + 1);
        pos2 = Math.Max(pos2, pos1 + 1);
        pos3 = Math.Max(pos3, pos2 + 1);
        pos4 = Math.Max(pos4, pos3 + 1);
    }

    private void AdjustMarkers()
    {
        // current positions of the markers
        double[] positions = { pos0, pos1, pos2, pos3, pos4 };
        double[] markers = { Q0, Q1, Q2, Q3, Q4 };

        for (int i = 1; i < 4; i++)
        {
            double delta = 0; // the difference between currentPos of a marker and it's desiredPos    
            switch (i)
            {
                case 1: delta = desPos1 - pos1; break;
                case 2: delta = desPos2 - pos2; break;
                case 3: delta = desPos3 - pos3; break;
            }

            // check if the difference is large enough to warrant adjustment based on surrounding positions
            if ((delta >= 1 && positions[i + 1] > positions[i] + 1) ||
                (delta <= -1 && positions[i - 1] < positions[i] - 1))
            {
                delta = Math.Sign(delta); // set the direction of the delta

                // get the parabolic value to adjust the marker
                double parabolicValue = CalculateMarker(i, markers, positions, delta, markers[i]);

                // if the parabolic value is within bounds of the neighbouring markers, apply it
                if (markers[i - 1] < parabolicValue && parabolicValue < markers[i + 1])
                {
                    markers[i] = parabolicValue;
                }
                else
                {
                    // apply linear interpolation if parabolic value is out of bounds
                    double linearValue = markers[i] + delta * (markers[i + (int)delta] - markers[i]) / (positions[i + (int)delta] - positions[i]);
                    markers[i] = linearValue;
                }
                positions[i] += delta;
            }
        }

        // update the original positions and markers
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

    private double CalculateMarker(int i, double[] markers, double[] positions, double delta, double parabolic)
    {
        if (markers[i - 1] < parabolic && parabolic < markers[i + 1])
        {
            // try parabolic
            double parabolicInterpolation = markers[i] + delta / (positions[i + 1] - positions[i - 1]) *
                                    ((markers[i + 1] - markers[i]) * (positions[i] - positions[i - 1] + delta) +
                                    (markers[i] - markers[i - 1]) * (positions[i + 1] - positions[i] - delta));
            return parabolicInterpolation;
        }
        else
        {
            // try linear
            double linearInterpolation = markers[i] + delta * (markers[i + (int)delta] - markers[i]) / (positions[i + (int)delta] - positions[i]);
            return linearInterpolation;
        }
    }

    /// <summary>
    /// Gets the quantile estimate based on the adjusted markers.
    /// </summary>
    public double GetQuantile()
    {
        // interpolate between markers
        if (p <= 0.0) return Q0;
        if (p >= 1.0) return Q4;
        if (p <= 0.25)
        {
            // between Q0 and Q1
            double t = p / 0.25;
            return Q0 + t * (Q1 - Q0);
        }
        else if (p <= 0.5)
        {
            // between Q1 and Q2
            double t = (p - 0.25) / 0.25;
            return Q1 + t * (Q2 - Q1);
        }
        else if (p <= 0.75)
        {
            // between Q2 and Q3
            double t = (p - 0.5) / 0.25;
            return Q2 + t * (Q3 - Q2);
        }
        else
        {
            // between Q3 and Q4
            double t = (p - 0.75) / 0.25;
            return Q3 + t * (Q4 - Q3);
        }
    }
}
