namespace DesignB.PricingViaStrategy;

/// <summary>
/// Base type for every financial instrument in Design B.
/// Instruments are pure data — they carry no pricing behaviour.
/// </summary>
/// <remarks>
/// All pricing logic lives behind <see cref="IPricer"/>. Different pricing
/// strategies (analytical, Monte Carlo, …) can be swapped without touching
/// the instrument class hierarchy.
/// </remarks>
public abstract class Instrument
{
    /// <summary>Human-readable identifier.</summary>
    public string Name { get; }

    /// <summary>Initialises a new instrument with the given display name.</summary>
    protected Instrument(string name) => Name = name;
}
