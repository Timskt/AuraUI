namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Interface for data transforms that operate on a sequence of data rows.
/// Implementations should be stateless and pure (no side effects).
/// </summary>
public interface IDataTransform
{
    /// <summary>
    /// Apply this transform to the input rows and return the transformed output.
    /// </summary>
    IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows);
}
