using Avalonia;

namespace AuraUI.Controls.Charts.Charts3D;

/// <summary>
/// Provides 3D-to-2D projection math for chart rendering.
/// Uses a perspective camera model with configurable rotation and distance.
///
/// Coordinate system:
///   X = right, Y = up, Z = toward viewer (right-handed)
///   The camera orbits around the origin at a configurable distance and angle.
///
/// The projection pipeline:
///   1. Model space -> World space (identity for simple charts)
///   2. World space -> Camera space (rotate by camera angles, translate by distance)
///   3. Camera space -> Screen space (perspective divide + viewport mapping)
/// </summary>
public class Projection3D
{
    /// <summary>Camera rotation around the vertical (Y) axis in degrees.</summary>
    public double RotationY { get; set; } = 30;

    /// <summary>Camera rotation around the horizontal (X) axis in degrees.</summary>
    public double RotationX { get; set; } = 25;

    /// <summary>Distance from the camera to the scene origin.</summary>
    public double Distance { get; set; } = 5.0;

    /// <summary>Perspective field of view in degrees. Larger = more perspective distortion.</summary>
    public double FieldOfView { get; set; } = 45;

    /// <summary>The viewport rectangle in screen pixels where the 3D scene is rendered.</summary>
    public Rect Viewport { get; set; } = new(0, 0, 600, 400);

    /// <summary>
    /// Project a 3D point to 2D screen coordinates.
    /// </summary>
    /// <param name="point">The 3D point in model/world space.</param>
    /// <returns>The projected 2D point in screen coordinates.</returns>
    public Point Project(Point3D point)
    {
        // Step 1: Apply camera rotation
        var rotated = ApplyRotation(point);

        // Step 2: Translate by camera distance (move scene away from camera)
        var z = rotated.Z - Distance;

        // Avoid division by zero or behind-camera clipping
        if (z >= -0.01) z = -0.01;

        // Step 3: Perspective projection
        var fovFactor = 1.0 / Math.Tan(FieldOfView * Math.PI / 360.0);
        var screenX = rotated.X * fovFactor / (-z);
        var screenY = -rotated.Y * fovFactor / (-z); // Flip Y for screen coordinates

        // Step 4: Map to viewport
        var cx = Viewport.X + Viewport.Width / 2;
        var cy = Viewport.Y + Viewport.Height / 2;
        var scale = Math.Min(Viewport.Width, Viewport.Height) / 2;

        return new Point(cx + screenX * scale, cy + screenY * scale);
    }

    /// <summary>
    /// Get the depth (Z in camera space) of a 3D point for depth sorting.
    /// Lower values = further from camera.
    /// </summary>
    public double GetDepth(Point3D point)
    {
        var rotated = ApplyRotation(point);
        return rotated.Z - Distance;
    }

    /// <summary>
    /// Project multiple points and return them with depth information for sorting.
    /// </summary>
    public (Point screen, double depth)[] ProjectWithDepth(Point3D[] points)
    {
        var result = new (Point screen, double depth)[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            var rotated = ApplyRotation(points[i]);
            var z = rotated.Z - Distance;
            if (z >= -0.01) z = -0.01;

            var fovFactor = 1.0 / Math.Tan(FieldOfView * Math.PI / 360.0);
            var screenX = rotated.X * fovFactor / (-z);
            var screenY = -rotated.Y * fovFactor / (-z);

            var cx = Viewport.X + Viewport.Width / 2;
            var cy = Viewport.Y + Viewport.Height / 2;
            var scale = Math.Min(Viewport.Width, Viewport.Height) / 2;

            result[i] = (
                new Point(cx + screenX * scale, cy + screenY * scale),
                z
            );
        }
        return result;
    }

    /// <summary>
    /// Apply the camera rotation matrix to a 3D point.
    /// Rotation order: first around Y (yaw), then around X (pitch).
    /// </summary>
    private Point3D ApplyRotation(Point3D point)
    {
        var radX = RotationX * Math.PI / 180.0;
        var radY = RotationY * Math.PI / 180.0;

        var cosX = Math.Cos(radX);
        var sinX = Math.Sin(radX);
        var cosY = Math.Cos(radY);
        var sinY = Math.Sin(radY);

        // Rotate around Y axis (yaw)
        var x1 = point.X * cosY + point.Z * sinY;
        var y1 = point.Y;
        var z1 = -point.X * sinY + point.Z * cosY;

        // Rotate around X axis (pitch)
        var x2 = x1;
        var y2 = y1 * cosX - z1 * sinX;
        var z2 = y1 * sinX + z1 * cosX;

        return new Point3D(x2, y2, z2);
    }

    /// <summary>
    /// Get the camera's forward direction vector (from camera toward scene).
    /// </summary>
    public Point3D GetForwardDirection()
    {
        var radX = RotationX * Math.PI / 180.0;
        var radY = RotationY * Math.PI / 180.0;
        return new Point3D(
            -Math.Sin(radY) * Math.Cos(radX),
            Math.Sin(radX),
            -Math.Cos(radY) * Math.Cos(radX)
        );
    }

    /// <summary>
    /// Get the camera position in world space.
    /// </summary>
    public Point3D GetCameraPosition()
    {
        var forward = GetForwardDirection();
        return new Point3D(
            -forward.X * Distance,
            -forward.Y * Distance,
            -forward.Z * Distance
        );
    }
}

/// <summary>
/// A simple 3D point/vector.
/// </summary>
public struct Point3D
{
    public double X;
    public double Y;
    public double Z;

    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Point3D operator +(Point3D a, Point3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Point3D operator -(Point3D a, Point3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Point3D operator *(Point3D a, double s) => new(a.X * s, a.Y * s, a.Z * s);

    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    public Point3D Normalize()
    {
        var len = Length;
        return len > 0 ? new Point3D(X / len, Y / len, Z / len) : default;
    }

    /// <summary>Cross product of two vectors.</summary>
    public static Point3D Cross(Point3D a, Point3D b) => new(
        a.Y * b.Z - a.Z * b.Y,
        a.Z * b.X - a.X * b.Z,
        a.X * b.Y - a.Y * b.X);

    /// <summary>Dot product of two vectors.</summary>
    public static double Dot(Point3D a, Point3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
}
