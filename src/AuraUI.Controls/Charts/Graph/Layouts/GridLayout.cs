using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Layouts;

/// <summary>
/// Arranges nodes in a regular grid pattern.
/// </summary>
public class GridLayout
{
    /// <summary>Number of rows. 0 = auto (computed from node count and cols).</summary>
    public int Rows { get; set; }

    /// <summary>Number of columns. 0 = auto (computed from node count and rows).</summary>
    public int Cols { get; set; }

    /// <summary>Width of each cell. 0 = auto (computed from bounds).</summary>
    public double CellWidth { get; set; }

    /// <summary>Height of each cell. 0 = auto (computed from bounds).</summary>
    public double CellHeight { get; set; }

    /// <summary>
    /// If true, removes empty rows/columns by condensing the layout.
    /// </summary>
    public bool Condense { get; set; }

    /// <summary>Padding inside each cell (between cell boundary and node center).</summary>
    public Thickness CellPadding { get; set; } = new(10);

    /// <summary>
    /// Applies the grid layout to the given nodes within the bounds.
    /// </summary>
    public void Apply(IList<GraphNode> nodes, Rect bounds)
    {
        if (nodes.Count == 0) return;

        var count = nodes.Count;

        // Auto-compute rows/cols
        int cols = Cols;
        int rows = Rows;

        if (cols <= 0 && rows <= 0)
        {
            cols = (int)Math.Ceiling(Math.Sqrt(count));
            rows = (int)Math.Ceiling((double)count / cols);
        }
        else if (cols <= 0)
        {
            cols = (int)Math.Ceiling((double)count / rows);
        }
        else if (rows <= 0)
        {
            rows = (int)Math.Ceiling((double)count / cols);
        }

        // Compute cell dimensions
        var cellW = CellWidth > 0 ? CellWidth : bounds.Width / cols;
        var cellH = CellHeight > 0 ? CellHeight : bounds.Height / rows;

        var pad = CellPadding;
        for (int i = 0; i < count; i++)
        {
            var col = i % cols;
            var row = i / cols;

            var x = bounds.X + col * cellW + cellW / 2;
            var y = bounds.Y + row * cellH + cellH / 2;

            // Apply cell padding
            var halfW = (cellW - pad.Left - pad.Right) / 2;
            var halfH = (cellH - pad.Top - pad.Bottom) / 2;
            x = Math.Clamp(x, bounds.X + pad.Left + halfW, bounds.Right - pad.Right - halfW);
            y = Math.Clamp(y, bounds.Y + pad.Top + halfH, bounds.Bottom - pad.Bottom - halfH);

            nodes[i].Position = new Point(x, y);
        }
    }
}
