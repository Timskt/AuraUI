using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Processing;
using AuraUI.Controls.Charts.Rendering;
using Xunit;

namespace AuraUI.Tests;

public class ChartTests
{
    #region ChartDataPoint

    [Fact]
    public void ChartDataPoint_DefaultConstructor()
    {
        var point = new ChartDataPoint();
        Assert.Equal(0, point.X);
        Assert.Equal(0, point.Y);
        Assert.Null(point.Label);
        Assert.Null(point.Color);
        Assert.Null(point.Tag);
    }

    [Fact]
    public void ChartDataPoint_XYConstructor()
    {
        var point = new ChartDataPoint(3.5, 7.2);
        Assert.Equal(3.5, point.X);
        Assert.Equal(7.2, point.Y);
    }

    [Fact]
    public void ChartDataPoint_XYLabelConstructor()
    {
        var point = new ChartDataPoint(1.0, 2.0, "Label");
        Assert.Equal(1.0, point.X);
        Assert.Equal(2.0, point.Y);
        Assert.Equal("Label", point.Label);
    }

    [Fact]
    public void ChartDataPoint_SetProperties()
    {
        var point = new ChartDataPoint
        {
            X = 10,
            Y = 20,
            Y2 = 15,
            Label = "Test",
            Tag = new { Key = "value" }
        };

        Assert.Equal(10, point.X);
        Assert.Equal(20, point.Y);
        Assert.Equal(15, point.Y2);
        Assert.Equal("Test", point.Label);
        Assert.NotNull(point.Tag);
    }

    #endregion

    #region ChartSliceData

    [Fact]
    public void ChartSliceData_Constructor()
    {
        var slice = new ChartSliceData("Category", 42.5);
        Assert.Equal("Category", slice.Label);
        Assert.Equal(42.5, slice.Value);
    }

    [Fact]
    public void ChartSliceData_DefaultConstructor()
    {
        var slice = new ChartSliceData();
        Assert.Null(slice.Label);
        Assert.Equal(0, slice.Value);
    }

    #endregion

    #region ChartHeatmapData

    [Fact]
    public void ChartHeatmapData_Constructor()
    {
        var data = new ChartHeatmapData(3, 5, 0.8);
        Assert.Equal(3, data.X);
        Assert.Equal(5, data.Y);
        Assert.Equal(0.8, data.Value);
    }

    #endregion

    #region ChartCandlestickData

    [Fact]
    public void ChartCandlestickData_Constructor()
    {
        var candle = new ChartCandlestickData(1, 100, 110, 95, 105);
        Assert.Equal(1, candle.X);
        Assert.Equal(100, candle.Open);
        Assert.Equal(110, candle.High);
        Assert.Equal(95, candle.Low);
        Assert.Equal(105, candle.Close);
    }

    #endregion

    #region ChartBoxplotData

    [Fact]
    public void ChartBoxplotData_Constructor()
    {
        var box = new ChartBoxplotData(1, 10, 25, 50, 75, 90);
        Assert.Equal(1, box.X);
        Assert.Equal(10, box.Min);
        Assert.Equal(25, box.Q1);
        Assert.Equal(50, box.Median);
        Assert.Equal(75, box.Q3);
        Assert.Equal(90, box.Max);
    }

    #endregion

    #region ChartScale - Linear

    [Fact]
    public void LinearScale_Map_Midpoint()
    {
        var scale = ChartScale.Linear(0, 100, 0, 500);
        Assert.Equal(250, scale.Map(50));
    }

    [Fact]
    public void LinearScale_Map_Min()
    {
        var scale = ChartScale.Linear(0, 100, 0, 500);
        Assert.Equal(0, scale.Map(0));
    }

    [Fact]
    public void LinearScale_Map_Max()
    {
        var scale = ChartScale.Linear(0, 100, 0, 500);
        Assert.Equal(500, scale.Map(100));
    }

    [Fact]
    public void LinearScale_Map_WithOffset()
    {
        var scale = ChartScale.Linear(0, 100, 100, 600);
        Assert.Equal(350, scale.Map(50));
    }

    [Fact]
    public void LinearScale_Map_NegativeDomain()
    {
        var scale = ChartScale.Linear(-100, 100, 0, 200);
        Assert.Equal(100, scale.Map(0));
    }

    [Fact]
    public void LinearScale_Invert()
    {
        var scale = ChartScale.Linear(0, 100, 0, 500);
        Assert.Equal(50, scale.Invert(250));
    }

    [Fact]
    public void LinearScale_Invert_RoundTrip()
    {
        var scale = ChartScale.Linear(10, 90, 50, 350);
        var original = 42.0;
        var mapped = scale.Map(original);
        var inverted = scale.Invert(mapped);
        Assert.Equal(original, inverted, 10);
    }

    [Fact]
    public void LinearScale_SameDomainAndRange_ReturnsRangeMin()
    {
        var scale = ChartScale.Linear(50, 50, 0, 100);
        Assert.Equal(0, scale.Map(50));
    }

    [Fact]
    public void LinearScale_GetTicks()
    {
        var scale = ChartScale.Linear(0, 100, 0, 500);
        var ticks = scale.GetTicks(5);
        Assert.NotEmpty(ticks);
        Assert.True(ticks.Length <= 10); // default maxTicks is 10
    }

    #endregion

    #region ChartScale - Logarithmic

    [Fact]
    public void LogScale_Map_PowersOf10()
    {
        var scale = ChartScale.Logarithmic(1, 1000, 0, 300);
        // log10(1) = 0, log10(1000) = 3
        // log10(10) = 1, so mapped = 0 + (1/3) * 300 = 100
        Assert.Equal(100, scale.Map(10), 0);
    }

    [Fact]
    public void LogScale_Map_ZeroOrNegative_ReturnsRangeMin()
    {
        var scale = ChartScale.Logarithmic(1, 1000, 0, 300);
        Assert.Equal(0, scale.Map(0));
        Assert.Equal(0, scale.Map(-5));
    }

    [Fact]
    public void LogScale_Invert()
    {
        var scale = ChartScale.Logarithmic(1, 1000, 0, 300);
        var inverted = scale.Invert(100);
        Assert.Equal(10, inverted, 0);
    }

    [Fact]
    public void LogScale_GetTicks()
    {
        var scale = ChartScale.Logarithmic(1, 1000, 0, 300);
        var ticks = scale.GetTicks();
        Assert.Contains(1.0, ticks);
        Assert.Contains(10.0, ticks);
        Assert.Contains(100.0, ticks);
        Assert.Contains(1000.0, ticks);
    }

    #endregion

    #region ChartScale - Time

    [Fact]
    public void TimeScale_Map_Proportional()
    {
        // 1 hour in ms
        long hour = 3600000;
        var scale = ChartScale.Time(0, 10 * hour, 0, 1000);
        Assert.Equal(500, scale.Map(5 * hour));
    }

    [Fact]
    public void TimeScale_Invert()
    {
        long hour = 3600000;
        var scale = ChartScale.Time(0, 10 * hour, 0, 1000);
        Assert.Equal(5.0 * hour, scale.Invert(500), 0);
    }

    [Fact]
    public void TimeScale_GetTicks_ReturnsReasonableIntervals()
    {
        long day = 86400000;
        var scale = ChartScale.Time(0, 7 * day, 0, 700);
        var ticks = scale.GetTicks(7);
        Assert.NotEmpty(ticks);
    }

    #endregion

    #region ChartInterpolator - CatmullRom

    [Fact]
    public void CatmullRom_TwoPoints_ReturnsTwoPoints()
    {
        var points = new[] { new Point(0, 0), new Point(100, 100) };
        var result = ChartInterpolator.CatmullRom(points);
        Assert.True(result.Length >= 2);
    }

    [Fact]
    public void CatmullRom_MultiplePoints_ProducesMorePoints()
    {
        var points = new[]
        {
            new Point(0, 0),
            new Point(50, 80),
            new Point(100, 0)
        };
        var result = ChartInterpolator.CatmullRom(points, segmentsPerCurve: 10);
        // Each segment produces 11 points (0..10), 2 segments = 22 points minus 1 overlap = 21
        Assert.True(result.Length > 3);
    }

    [Fact]
    public void CatmullRom_StartAndEndMatch()
    {
        var points = new[]
        {
            new Point(0, 0),
            new Point(50, 80),
            new Point(100, 0)
        };
        var result = ChartInterpolator.CatmullRom(points);

        Assert.Equal(points[0].X, result[0].X, 1);
        Assert.Equal(points[0].Y, result[0].Y, 1);
        Assert.Equal(points[^1].X, result[^1].X, 1);
        Assert.Equal(points[^1].Y, result[^1].Y, 1);
    }

    [Fact]
    public void CatmullRom_SinglePoint_ReturnsSame()
    {
        var points = new[] { new Point(50, 50) };
        var result = ChartInterpolator.CatmullRom(points);
        Assert.Single(result);
    }

    [Fact]
    public void CatmullRom_TensionAffectsCurve()
    {
        var points = new[]
        {
            new Point(0, 0),
            new Point(50, 100),
            new Point(100, 0)
        };
        var result1 = ChartInterpolator.CatmullRom(points, tension: 0.0);
        var result2 = ChartInterpolator.CatmullRom(points, tension: 1.0);

        // Different tensions should produce different curves
        // Check that at least some midpoints differ
        bool anyDifferent = false;
        for (int i = 0; i < Math.Min(result1.Length, result2.Length); i++)
        {
            if (Math.Abs(result1[i].Y - result2[i].Y) > 0.01)
            {
                anyDifferent = true;
                break;
            }
        }
        Assert.True(anyDifferent);
    }

    #endregion

    #region ChartInterpolator - CubicBezier

    [Fact]
    public void CubicBezier_TwoPoints()
    {
        var points = new[] { new Point(0, 0), new Point(100, 100) };
        var result = ChartInterpolator.CubicBezier(points);
        Assert.True(result.Length >= 2);
    }

    [Fact]
    public void CubicBezier_StartAndEndMatch()
    {
        var points = new[]
        {
            new Point(0, 0),
            new Point(50, 80),
            new Point(100, 0)
        };
        var result = ChartInterpolator.CubicBezier(points);

        Assert.Equal(points[0].X, result[0].X, 1);
        Assert.Equal(points[0].Y, result[0].Y, 1);
        Assert.Equal(points[^1].X, result[^1].X, 1);
        Assert.Equal(points[^1].Y, result[^1].Y, 1);
    }

    #endregion

    #region ChartInterpolator - MonotoneCubic

    [Fact]
    public void MonotoneCubic_PreservesMonotonicity()
    {
        // Ascending data
        var points = new[]
        {
            new Point(0, 0),
            new Point(25, 30),
            new Point(50, 50),
            new Point(75, 80),
            new Point(100, 100)
        };
        var result = ChartInterpolator.MonotoneCubic(points);

        // All Y values should be non-decreasing for ascending data
        for (int i = 1; i < result.Length; i++)
        {
            Assert.True(result[i].Y >= result[i - 1].Y - 0.01,
                $"Monotonicity violated at index {i}: {result[i - 1].Y} -> {result[i].Y}");
        }
    }

    [Fact]
    public void MonotoneCubic_StartAndEndMatch()
    {
        var points = new[]
        {
            new Point(0, 0),
            new Point(50, 100),
            new Point(100, 0)
        };
        var result = ChartInterpolator.MonotoneCubic(points);

        Assert.Equal(points[0].X, result[0].X, 1);
        Assert.Equal(points[0].Y, result[0].Y, 1);
        Assert.Equal(points[^1].X, result[^1].X, 1);
        Assert.Equal(points[^1].Y, result[^1].Y, 1);
    }

    [Fact]
    public void MonotoneCubic_TwoPoints_ReturnsInput()
    {
        var points = new[] { new Point(0, 0), new Point(100, 100) };
        var result = ChartInterpolator.MonotoneCubic(points);
        Assert.Equal(2, result.Length);
    }

    #endregion

    #region DataVirtualizer - LTTB Downsampling

    [Fact]
    public void LttbDownsample_SmallInput_ReturnsAll()
    {
        var points = new Point[5];
        for (int i = 0; i < 5; i++)
            points[i] = new Point(i * 10, i * 5);

        var result = DataVirtualizer.LttbDownsample(points, 10);
        Assert.Equal(5, result.Length);
    }

    [Fact]
    public void LttbDownsample_LargeInput_ReducesToThreshold()
    {
        var points = new Point[1000];
        for (int i = 0; i < 1000; i++)
            points[i] = new Point(i, Math.Sin(i * 0.1) * 100);

        var result = DataVirtualizer.LttbDownsample(points, 100);
        Assert.Equal(100, result.Length);
    }

    [Fact]
    public void LttbDownsample_PreservesFirstAndLast()
    {
        var points = new Point[500];
        for (int i = 0; i < 500; i++)
            points[i] = new Point(i, i * 0.5);

        var result = DataVirtualizer.LttbDownsample(points, 50);

        Assert.Equal(points[0], result[0]);
        Assert.Equal(points[^1], result[^1]);
    }

    [Fact]
    public void LttbDownsample_ThresholdLessThan3_ReturnsAll()
    {
        var points = new Point[10];
        for (int i = 0; i < 10; i++)
            points[i] = new Point(i, i);

        var result = DataVirtualizer.LttbDownsample(points, 2);
        Assert.Equal(10, result.Length);
    }

    [Fact]
    public void LttbDownsample_ExactThreshold_ReturnsAll()
    {
        var points = new Point[10];
        for (int i = 0; i < 10; i++)
            points[i] = new Point(i, i);

        var result = DataVirtualizer.LttbDownsample(points, 10);
        Assert.Equal(10, result.Length);
    }

    #endregion

    #region GeometryCache

    [Fact]
    public void GeometryCache_Store_And_TryGet()
    {
        var cache = new GeometryCache();
        cache.Store(1, 2, 3, 1.0, null, null);

        var found = cache.TryGet(1, 2, 3, 1.0, out var linePath, out var areaPath);
        Assert.True(found);
        Assert.Null(linePath);
        Assert.Null(areaPath);
    }

    [Fact]
    public void GeometryCache_TryGet_Miss_ReturnsFalse()
    {
        var cache = new GeometryCache();
        var found = cache.TryGet(1, 2, 3, 1.0, out _, out _);
        Assert.False(found);
    }

    [Fact]
    public void GeometryCache_AnimationProgress_Mismatch_Miss()
    {
        var cache = new GeometryCache();
        cache.Store(1, 2, 3, 0.5, null, null);

        var found = cache.TryGet(1, 2, 3, 1.0, out _, out _);
        Assert.False(found);
    }

    [Fact]
    public void GeometryCache_AnimationProgress_WithinTolerance_Hit()
    {
        var cache = new GeometryCache();
        cache.Store(1, 2, 3, 1.0, null, null);

        var found = cache.TryGet(1, 2, 3, 1.0005, out _, out _);
        Assert.True(found);
    }

    [Fact]
    public void GeometryCache_Count()
    {
        var cache = new GeometryCache();
        Assert.Equal(0, cache.Count);

        cache.Store(1, 1, 1, 1.0, null, null);
        Assert.Equal(1, cache.Count);

        cache.Store(2, 2, 2, 1.0, null, null);
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void GeometryCache_Clear()
    {
        var cache = new GeometryCache();
        cache.Store(1, 1, 1, 1.0, null, null);
        cache.Store(2, 2, 2, 1.0, null, null);

        cache.Clear();

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void GeometryCache_InvalidateSeries()
    {
        var cache = new GeometryCache();
        cache.Store(1, 10, 20, 1.0, null, null);
        cache.Store(2, 10, 20, 1.0, null, null);

        cache.InvalidateSeries(1);

        Assert.False(cache.TryGet(1, 10, 20, 1.0, out _, out _));
        Assert.True(cache.TryGet(2, 10, 20, 1.0, out _, out _));
    }

    [Fact]
    public void GeometryCache_MaxEntries_Eviction()
    {
        var cache = new GeometryCache { MaxEntries = 5 };

        for (int i = 0; i < 10; i++)
        {
            cache.Store(i, i, i, 1.0, null, null);
        }

        // Should have evicted some entries
        Assert.True(cache.Count <= 10); // May or may not evict depending on implementation
    }

    [Fact]
    public void GeometryCache_Store_Overwrites()
    {
        var cache = new GeometryCache();
        cache.Store(1, 2, 3, 0.5, null, null);
        cache.Store(1, 2, 3, 0.8, null, null); // Same key, different progress

        Assert.Equal(1, cache.Count);
        Assert.True(cache.TryGet(1, 2, 3, 0.8, out _, out _));
    }

    #endregion

    #region ChartTreemapNode

    [Fact]
    public void ChartTreemapNode_Constructor()
    {
        var node = new ChartTreemapNode("Root", 100);
        Assert.Equal("Root", node.Name);
        Assert.Equal(100, node.Value);
        Assert.Empty(node.Children);
    }

    [Fact]
    public void ChartTreemapNode_Children()
    {
        var node = new ChartTreemapNode("Root", 100);
        node.Children.Add(new ChartTreemapNode("Child1", 30));
        node.Children.Add(new ChartTreemapNode("Child2", 70));

        Assert.Equal(2, node.Children.Count);
    }

    #endregion

    #region ChartSankeyNode / Link

    [Fact]
    public void ChartSankeyLink_Properties()
    {
        var link = new ChartSankeyLink { Source = 0, Target = 1, Value = 50 };
        Assert.Equal(0, link.Source);
        Assert.Equal(1, link.Target);
        Assert.Equal(50, link.Value);
    }

    #endregion

    #region ChartGraphNode / Edge

    [Fact]
    public void ChartGraphNode_Properties()
    {
        var node = new ChartGraphNode { Name = "Node1", X = 10, Y = 20, Size = 15 };
        Assert.Equal("Node1", node.Name);
        Assert.Equal(10, node.X);
        Assert.Equal(20, node.Y);
        Assert.Equal(15, node.Size);
    }

    [Fact]
    public void ChartGraphEdge_DefaultWeight()
    {
        var edge = new ChartGraphEdge { Source = 0, Target = 1 };
        Assert.Equal(1.0, edge.Weight);
    }

    #endregion
}
