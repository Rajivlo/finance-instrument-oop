namespace DesignB.PricingViaStrategy;

/// <summary>
/// Prices a <see cref="Bond"/> as the sum of discounted coupons plus face value at maturity
/// under a flat continuously-compounded curve.
/// </summary>
public sealed class BondPricer : IPricer
{
    /// <inheritdoc />
    public decimal Price(Instrument instrument) => throw new NotImplementedException();
}
