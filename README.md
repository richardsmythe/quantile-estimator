## Quantile estimator

This project implements a quantile estimator for streams of data. It implements the P² algorithm quite faithfully. It efficiently estimates a given _p_ from large or streaming datasets without storing all data points.

## Usage
Run the program and set the desired quantile in `Program.cs`. The estimator will output the estimated quantile and compare it to the exact value for validation.

## Example
- Estimate the 25th, 50th, or 75th percentile of a dataset
- See convergence and error metrics for different quantiles

## References
- [P² Algorithm for Dynamic Calculation of Quantiles and Histograms Without Storing Observations](https://www.cse.wustl.edu/~jain/papers/ftp/psqr.pdf)

## Example of output

Iterations: 200
Dataset: 10000
p=0.75

Iteration 1:
  Current average: 7288
  Final estimate for p=0.75: 7288

Iteration 10:
  Current average: 6404.6
  Final estimate for p=0.75: 7493

Iteration 20:
  Current average: 6527
  Final estimate for p=0.75: 7219

Iteration 30:
  Current average: 6471.833333333333
  Final estimate for p=0.75: 8413

Iteration 40:
  Current average: 6556.1
  Final estimate for p=0.75: 6862

Iteration 50:
  Current average: 6454.84
  Final estimate for p=0.75: 5835

Iteration 60:
  Current average: 6710.25
  Final estimate for p=0.75: 8170

Iteration 70:
  Current average: 6690.385714285714
  Final estimate for p=0.75: 8482

Iteration 80:
  Current average: 6629.55
  Final estimate for p=0.75: 3586

Iteration 90:
  Current average: 6625.366666666667
  Final estimate for p=0.75: 5593

Iteration 100:
  Current average: 6488.89
  Final estimate for p=0.75: 9048

Iteration 110:
  Current average: 6426.572727272727
  Final estimate for p=0.75: 6586

Iteration 120:
  Current average: 6442.2
  Final estimate for p=0.75: 9216

Iteration 130:
  Current average: 6448.830769230769
  Final estimate for p=0.75: 7724

Iteration 140:
  Current average: 6523.771428571428
  Final estimate for p=0.75: 7353

Iteration 150:
  Current average: 6529.806666666666
  Final estimate for p=0.75: 5286

Iteration 160:
  Current average: 6446.81875
  Final estimate for p=0.75: 4669

Iteration 170:
  Current average: 6434.529411764706
  Final estimate for p=0.75: 5028

Iteration 180:
  Current average: 6405.422222222222
  Final estimate for p=0.75: 9340

Iteration 190:
  Current average: 6421.510526315789
  Final estimate for p=0.75: 6033

Iteration 200:
  Current average: 6444.85
  Final estimate for p=0.75: 6238

Exact quantile for p=0.75: 7500.25
Average quantile estimate: 6444.85
Difference: 1055.3999999999996

---- CONVERGENCE TEST (p=0.75) ----- <br>
Testing how it converges with more data...

After 100 data points:
  Markers: [88, 4474, 6338, 7288, 9743]
  Current estimate: 7288
  Exact quantile: 7517.25
  Difference: 229.25
  Relative error: 3.05%


After 500 data points:
  Markers: [11, 4474, 6338, 7288, 9982]
  Current estimate: 7288
  Exact quantile: 7377.5
  Difference: 89.5
  Relative error: 1.21%


After 1000 data points:
  Markers: [11, 4474, 6338, 7288, 9982]
  Current estimate: 7288
  Exact quantile: 7430.25
  Difference: 142.25
  Relative error: 1.91%


After 2500 data points:
  Markers: [2, 4474, 6338, 7288, 9997]
  Current estimate: 7288
  Exact quantile: 7447.5
  Difference: 159.5
  Relative error: 2.14%


After 5000 data points:
  Markers: [1, 4474, 6338, 7288, 9997]
  Current estimate: 7288
  Exact quantile: 7483
  Difference: 195
  Relative error: 2.61%


After 10000 data points:
  Markers: [1, 4474, 6338, 7288, 10000]
  Current estimate: 7288
  Exact quantile: 7500.25
  Difference: 212.25
  Relative error: 2.83%

