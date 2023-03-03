namespace Headway.Dynamo.Runtime;

/// <summary>
/// Encapsulates the value of a primary key
/// </summary>
public sealed class PrimaryKeyValue
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public PrimaryKeyValue()
    {
    }

    /// <summary>
    /// Constructs a <see cref="PrimaryKeyValue"/> give an
    /// object value
    /// </summary>
    /// <param name="value">Value stored in the primary key</param>
    public PrimaryKeyValue(object value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets or sets the value of the key
    /// </summary>
    public object Value
    {
        get;
        set;
    }
}
