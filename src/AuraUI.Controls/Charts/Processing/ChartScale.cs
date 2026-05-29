namespace AuraUI.Controls.Charts.Processing;

/// <summary>
/// Provides scale transformations for mapping data values to pixel coordinates.
/// Supports linear, logarithmic, and time-based scales.
/// </summary>
public abstract class ChartScale
{
    /// <summary>The minimum data value of the domain.</summary>
    public double DomainMin { get; set; }

    /// <summary>The maximum data value of the domain.</summary>
    public double DomainMax { get; set; }

    /// <summary>The minimum pixel value of the range.</summary>
    public double RangeMin { get; set; }

    /// <summary>The maximum pixel value of the range.</summary>
    public double RangeMax { get; set; }

    /// <summary>Map a data value to a pixel position.</summary>
    public abstract double Map(double value);

    /// <summary>Map a pixel position back to a data value.</summary>
    public abstract double Invert(double pixel);

    /// <summary>Generate tick values for this scale.</summary>
    public abstract double[] GetTicks(int maxTicks = 10);

    /// <summary>Create a linear scale.</summary>
    public static ChartScale Linear(double domainMin, double domainMax, double rangeMin, double rangeMax)
        => new LinearScale { DomainMin = domainMin, DomainMax = domainMax, RangeMin = rangeMin, RangeMax = rangeMax };

    /// <summary>Create a logarithmic scale (base 10).</summary>
    public static ChartScale Logarithmic(double domainMin, double domainMax, double rangeMin, double rangeMax)
        => new LogarithmicScale { DomainMin = domainMin, DomainMax = domainMax, RangeMin = rangeMin, RangeMax = rangeMax };

    /// <summary>Create a time scale (values are Unix timestamps in milliseconds).</summary>
    public static ChartScale Time(double domainMin, double domainMax, double rangeMin, double rangeMax)
        => new TimeScale { DomainMin = domainMin, DomainMax = domainMax, RangeMin = rangeMin, RangeMax = rangeMax };
}

/// <summary>
/// Linear scale: maps values proportionally from domain to range.
/// </summary>
public class LinearScale : ChartScale
{
    public override double Map(double value)
    {
        var domainRange = DomainMax - DomainMin;
        if (Math.Abs(domainRange) < 1e-10) return RangeMin;
        var normalized = (value - DomainMin) / domainRange;
        return RangeMin + normalized * (RangeMax - RangeMin);
    }

    public override double Invert(double pixel)
    {
        var rangeSpan = RangeMax - RangeMin;
        if (Math.Abs(rangeSpan) < 1e-10) return DomainMin;
        var normalized = (pixel - RangeMin) / rangeSpan;
        return DomainMin + normalized * (DomainMax - DomainMin);
    }

    public override double[] GetTicks(int maxTicks = 10)
    {
        return NiceScale.Compute(DomainMin, DomainMax, maxTicks).ticks;
    }
}

/// <summary>
/// Logarithmic scale: maps values using log10 transformation.
/// Useful for data spanning multiple orders of magnitude.
/// </summary>
public class LogarithmicScale : ChartScale
{
    public override double Map(double value)
    {
        if (value <= 0) return RangeMin;
        var logMin = Math.Log10(Math.Max(DomainMin, 1e-10));
        var logMax = Math.Log10(Math.Max(DomainMax, 1e-10));
        var logVal = Math.Log10(value);
        var logRange = logMax - logMin;
        if (Math.Abs(logRange) < 1e-10) return RangeMin;
        var normalized = (logVal - logMin) / logRange;
        return RangeMin + normalized * (RangeMax - RangeMin);
    }

    public override double Invert(double pixel)
    {
        var rangeSpan = RangeMax - RangeMin;
        if (Math.Abs(rangeSpan) < 1e-10) return DomainMin;
        var normalized = (pixel - RangeMin) / rangeSpan;
        var logMin = Math.Log10(Math.Max(DomainMin, 1e-10));
        var logMax = Math.Log10(Math.Max(DomainMax, 1e-10));
        var logVal = logMin + normalized * (logMax - logMin);
        return Math.Pow(10, logVal);
    }

    public override double[] GetTicks(int maxTicks = 10)
    {
        var logMin = Math.Log10(Math.Max(DomainMin, 1e-10));
        var logMax = Math.Log10(Math.Max(DomainMax, 1e-10));
        var ticks = new List<double>();
        var start = Math.Floor(logMin);
        var end = Math.Ceiling(logMax);
        for (var exp = start; exp <= end; exp++)
        {
            var value = Math.Pow(10, exp);
            if (value >= DomainMin && value <= DomainMax)
                ticks.Add(value);
        }
        return ticks.ToArray();
    }
}

/// <summary>
/// Time scale: maps Unix timestamp values to pixel positions.
/// Generates tick marks at appropriate time intervals.
/// </summary>
public class TimeScale : ChartScale
{
    public override double Map(double value)
    {
        var domainRange = DomainMax - DomainMin;
        if (Math.Abs(domainRange) < 1e-10) return RangeMin;
        var normalized = (value - DomainMin) / domainRange;
        return RangeMin + normalized * (RangeMax - RangeMin);
    }

    public override double Invert(double pixel)
    {
        var rangeSpan = RangeMax - RangeMin;
        if (Math.Abs(rangeSpan) < 1e-10) return DomainMin;
        var normalized = (pixel - RangeMin) / rangeSpan;
        return DomainMin + normalized * (DomainMax - DomainMin);
    }

    public override double[] GetTicks(int maxTicks = 10)
    {
        var rangeMs = DomainMax - DomainMin;
        if (rangeMs <= 0) return Array.Empty<double>();

        // Time intervals in milliseconds
        long[] intervals =
        [
            1000,           // 1 second
            5000,           // 5 seconds
            10000,          // 10 seconds
            30000,          // 30 seconds
            60000,          // 1 minute
            300000,         // 5 minutes
            600000,         // 10 minutes
            1800000,        // 30 minutes
            3600000,        // 1 hour
            7200000,        // 2 hours
            21600000,       // 6 hours
            43200000,       // 12 hours
            86400000,       // 1 day
            604800000,      // 1 week
            2592000000,     // 30 days
            7776000000,     // 90 days
            31536000000     // 1 year
        ];

        // Find the interval that gives closest to maxTicks
        long bestInterval = intervals[^1];
        foreach (var interval in intervals)
        {
            if (rangeMs / interval <= maxTicks)
            {
                bestInterval = interval;
                break;
            }
        }

        var ticks = new List<double>();
        var start = Math.Ceiling(DomainMin / bestInterval) * bestInterval;
        for (var t = start; t <= DomainMax; t += bestInterval)
            ticks.Add(t);

        return ticks.ToArray();
    }
}
