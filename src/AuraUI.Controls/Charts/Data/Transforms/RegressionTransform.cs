namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Computes regression curves from data points. Supports linear, polynomial,
/// exponential, logarithmic regression and moving average smoothing.
/// </summary>
public class RegressionTransform : IDataTransform
{
    /// <summary>
    /// The X field for regression.
    /// </summary>
    public string XField { get; set; } = string.Empty;

    /// <summary>
    /// The Y field for regression.
    /// </summary>
    public string YField { get; set; } = string.Empty;

    /// <summary>
    /// The type of regression to perform.
    /// </summary>
    public RegressionType Type { get; set; } = RegressionType.Linear;

    /// <summary>
    /// Polynomial degree (only used when Type is Polynomial). Default 2 (quadratic).
    /// </summary>
    public int Degree { get; set; } = 2;

    /// <summary>
    /// Window size for moving average (only used when Type is MovingAverage). Default 5.
    /// </summary>
    public int WindowSize { get; set; } = 5;

    /// <summary>
    /// Number of output points to generate for the regression curve.
    /// When null, uses the same number as input points.
    /// </summary>
    public int? OutputCount { get; set; }

    /// <summary>
    /// Output field name for predicted Y values.
    /// </summary>
    public string OutputYField { get; set; } = "y_predicted";

    /// <summary>
    /// Whether to include regression coefficients in the first output row's Tag.
    /// </summary>
    public bool IncludeCoefficients { get; set; }

    /// <summary>
    /// Computed regression coefficients (available after Apply).
    /// </summary>
    public double[]? Coefficients { get; private set; }

    /// <summary>
    /// R-squared value (available after Apply for linear regression).
    /// </summary>
    public double RSquared { get; private set; }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var rowList = rows.ToList();
        var xValues = rowList.Select(r => r.GetDouble(XField)).ToArray();
        var yValues = rowList.Select(r => r.GetDouble(YField)).ToArray();

        if (xValues.Length == 0) return rowList;

        return Type switch
        {
            RegressionType.Linear => ApplyLinear(rowList, xValues, yValues),
            RegressionType.Polynomial => ApplyPolynomial(rowList, xValues, yValues),
            RegressionType.Exponential => ApplyExponential(rowList, xValues, yValues),
            RegressionType.Logarithmic => ApplyLogarithmic(rowList, xValues, yValues),
            RegressionType.MovingAverage => ApplyMovingAverage(rowList, xValues, yValues),
            _ => rowList
        };
    }

    private IEnumerable<DataRow> ApplyLinear(List<DataRow> rows, double[] x, double[] y)
    {
        var n = x.Length;
        var sumX = x.Sum();
        var sumY = y.Sum();
        var sumXY = x.Zip(y, (a, b) => a * b).Sum();
        var sumX2 = x.Sum(a => a * a);

        var denom = n * sumX2 - sumX * sumX;
        if (Math.Abs(denom) < 1e-10)
        {
            Coefficients = new[] { 0.0, 0.0 };
            RSquared = 0;
            return rows;
        }

        var slope = (n * sumXY - sumX * sumY) / denom;
        var intercept = (sumY - slope * sumX) / n;
        Coefficients = new[] { intercept, slope };

        // R-squared
        var meanY = y.Average();
        var ssTot = y.Sum(v => (v - meanY) * (v - meanY));
        var ssRes = x.Zip(y, (xi, yi) => Math.Pow(yi - (intercept + slope * xi), 2)).Sum();
        RSquared = ssTot > 0 ? 1 - ssRes / ssTot : 0;

        return GenerateOutput(rows, x, xi => intercept + slope * xi);
    }

    private IEnumerable<DataRow> ApplyPolynomial(List<DataRow> rows, double[] x, double[] y)
    {
        var coefficients = FitPolynomial(x, y, Degree);
        Coefficients = coefficients;

        return GenerateOutput(rows, x, xi =>
        {
            var result = 0.0;
            for (int i = 0; i < coefficients.Length; i++)
                result += coefficients[i] * Math.Pow(xi, i);
            return result;
        });
    }

    private IEnumerable<DataRow> ApplyExponential(List<DataRow> rows, double[] x, double[] y)
    {
        // y = a * e^(b*x) => ln(y) = ln(a) + b*x
        var positiveY = y.Where(v => v > 0).ToArray();
        var positiveX = x.Zip(y, (xi, yi) => (xi, yi)).Where(p => p.yi > 0).Select(p => p.xi).ToArray();

        if (positiveY.Length < 2)
        {
            Coefficients = new[] { 0.0, 0.0 };
            return rows;
        }

        var lnY = positiveY.Select(v => Math.Log(v)).ToArray();
        var n = positiveX.Length;
        var sumX = positiveX.Sum();
        var sumLnY = lnY.Sum();
        var sumXLnY = positiveX.Zip(lnY, (a, b) => a * b).Sum();
        var sumX2 = positiveX.Sum(a => a * a);

        var denom = n * sumX2 - sumX * sumX;
        if (Math.Abs(denom) < 1e-10)
        {
            Coefficients = new[] { 0.0, 0.0 };
            return rows;
        }

        var b = (n * sumXLnY - sumX * sumLnY) / denom;
        var lnA = (sumLnY - b * sumX) / n;
        var a = Math.Exp(lnA);

        Coefficients = new[] { a, b };

        return GenerateOutput(rows, x, xi => a * Math.Exp(b * xi));
    }

    private IEnumerable<DataRow> ApplyLogarithmic(List<DataRow> rows, double[] x, double[] y)
    {
        // y = a + b * ln(x)
        var positiveX = x.Zip(y, (xi, yi) => (xi, yi)).Where(p => p.xi > 0).ToArray();

        if (positiveX.Length < 2)
        {
            Coefficients = new[] { 0.0, 0.0 };
            return rows;
        }

        var lnX = positiveX.Select(p => Math.Log(p.xi)).ToArray();
        var posY = positiveX.Select(p => p.yi).ToArray();
        var n = lnX.Length;
        var sumLnX = lnX.Sum();
        var sumY = posY.Sum();
        var sumLnXY = lnX.Zip(posY, (a, b) => a * b).Sum();
        var sumLnX2 = lnX.Sum(a => a * a);

        var denom = n * sumLnX2 - sumLnX * sumLnX;
        if (Math.Abs(denom) < 1e-10)
        {
            Coefficients = new[] { 0.0, 0.0 };
            return rows;
        }

        var b = (n * sumLnXY - sumLnX * sumY) / denom;
        var a = (sumY - b * sumLnX) / n;

        Coefficients = new[] { a, b };

        return GenerateOutput(rows, x, xi => xi > 0 ? a + b * Math.Log(xi) : a);
    }

    private IEnumerable<DataRow> ApplyMovingAverage(List<DataRow> rows, double[] x, double[] y)
    {
        var half = WindowSize / 2;
        var results = new List<DataRow>();

        for (int i = 0; i < rows.Count; i++)
        {
            var start = Math.Max(0, i - half);
            var end = Math.Min(rows.Count - 1, i + half);
            var avg = 0.0;
            for (int j = start; j <= end; j++)
                avg += y[j];
            avg /= (end - start + 1);

            var newRow = rows[i].Clone();
            newRow[OutputYField] = avg;
            results.Add(newRow);
        }

        return results;
    }

    private IEnumerable<DataRow> GenerateOutput(List<DataRow> rows, double[] x, Func<double, double> predict)
    {
        var count = OutputCount ?? rows.Count;
        var results = new List<DataRow>();

        if (OutputCount.HasValue && OutputCount.Value != rows.Count)
        {
            // Generate evenly spaced output points
            var xMin = x.Min();
            var xMax = x.Max();
            var step = (xMax - xMin) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                var xi = xMin + i * step;
                var newRow = new DataRow();
                newRow[XField] = xi;
                newRow[OutputYField] = predict(xi);
                results.Add(newRow);
            }
        }
        else
        {
            for (int i = 0; i < rows.Count; i++)
            {
                var newRow = rows[i].Clone();
                newRow[OutputYField] = predict(x[i]);
                results.Add(newRow);
            }
        }

        return results;
    }

    /// <summary>
    /// Fit a polynomial of given degree using least squares.
    /// </summary>
    private static double[] FitPolynomial(double[] x, double[] y, int degree)
    {
        var n = x.Length;
        var m = degree + 1;

        // Build Vandermonde matrix and solve normal equations
        var ata = new double[m, m];
        var aty = new double[m];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                for (int k = 0; k < m; k++)
                    ata[j, k] += Math.Pow(x[i], j + k);
                aty[j] += y[i] * Math.Pow(x[i], j);
            }
        }

        // Solve using Gaussian elimination
        return SolveLinearSystem(ata, aty, m);
    }

    private static double[] SolveLinearSystem(double[,] a, double[] b, int n)
    {
        // Gaussian elimination with partial pivoting
        var aug = new double[n, n + 1];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                aug[i, j] = a[i, j];
            aug[i, n] = b[i];
        }

        for (int col = 0; col < n; col++)
        {
            // Find pivot
            var maxVal = Math.Abs(aug[col, col]);
            var maxRow = col;
            for (int row = col + 1; row < n; row++)
            {
                if (Math.Abs(aug[row, col]) > maxVal)
                {
                    maxVal = Math.Abs(aug[row, col]);
                    maxRow = row;
                }
            }

            // Swap rows
            for (int j = 0; j <= n; j++)
            {
                (aug[col, j], aug[maxRow, j]) = (aug[maxRow, j], aug[col, j]);
            }

            // Eliminate
            if (Math.Abs(aug[col, col]) < 1e-12) continue;

            for (int row = col + 1; row < n; row++)
            {
                var factor = aug[row, col] / aug[col, col];
                for (int j = col; j <= n; j++)
                    aug[row, j] -= factor * aug[col, j];
            }
        }

        // Back substitution
        var result = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            result[i] = aug[i, n];
            for (int j = i + 1; j < n; j++)
                result[i] -= aug[i, j] * result[j];
            if (Math.Abs(aug[i, i]) > 1e-12)
                result[i] /= aug[i, i];
        }

        return result;
    }
}

/// <summary>
/// Supported regression types.
/// </summary>
public enum RegressionType
{
    /// <summary>Linear regression (y = a + b*x).</summary>
    Linear,

    /// <summary>Polynomial regression (y = a0 + a1*x + a2*x^2 + ...).</summary>
    Polynomial,

    /// <summary>Exponential regression (y = a * e^(b*x)).</summary>
    Exponential,

    /// <summary>Logarithmic regression (y = a + b*ln(x)).</summary>
    Logarithmic,

    /// <summary>Moving average smoothing with configurable window size.</summary>
    MovingAverage
}
