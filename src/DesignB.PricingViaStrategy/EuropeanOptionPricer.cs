namespace DesignB.PricingViaStrategy;

/// <summary>
/// Analytical pricer for a <see cref="EuropeanOption"/>: returns the undiscounted intrinsic value.
/// </summary>
/// <remarks>
/// Mirrors Design A's intrinsic-value rule so the two designs produce identical
/// prices for the assignment's sample portfolio.
/// </remarks>
public sealed class EuropeanOptionPricer : IPricer
{
    /// <inheritdoc />
    public decimal Price(Instrument instrument) => throw new NotImplementedException();
}
