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
    public decimal Price(Instrument instrument)
    {
        if (instrument is not EuropeanOption o)
        {
            throw new ArgumentException(
                $"{nameof(EuropeanOptionPricer)} only prices {nameof(EuropeanOption)}, got {instrument.GetType().Name}.",
                nameof(instrument));
        }
        return o.Type switch
        {
            OptionType.Call => Math.Max(o.Spot - o.Strike, 0m),
            OptionType.Put => Math.Max(o.Strike - o.Spot, 0m),
            _ => throw new InvalidOperationException($"Unknown option type: {o.Type}"),
        };
    }
}
