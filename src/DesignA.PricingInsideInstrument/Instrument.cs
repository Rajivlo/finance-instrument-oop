namespace DesignA.PricingInsideInstrument;

/// <summary>
/// Base type for every financial instrument in Design A.
/// Each concrete subclass owns its pricing logic by overriding <see cref="Price"/>.
/// </summary>
/// <remarks>
/// This is the "pricing inside instrument" design: behaviour and data live
/// together on the same object. Compare with Design B, where pricing is
/// extracted into an external <c>IPricer</c> strategy.
/// </remarks>
public abstract class Instrument
{
    /// <summary>Human-readable identifier for printing and test assertions.</summary>
    public string Name { get; }

    /// <summary>Initialises a new instrument with the given display name.</summary>
    /// <param name="name">Non-null display name.</param>
    protected Instrument(string name)
    {
        Name = name;
    }

    /// <summary>Computes the current fair price of this instrument, in USD.</summary>
    /// <returns>The instrument's price as a decimal.</returns>
    public abstract decimal Price();
}
