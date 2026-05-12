namespace DesignB.PricingViaStrategy;

/// <summary>
/// Prices a <see cref="Bond"/> as the sum of discounted coupons plus face value at maturity
/// under a flat continuously-compounded curve.
/// </summary>
public sealed class BondPricer : IPricer
{
    /// <inheritdoc />
    public decimal Price(Instrument instrument)
    {
        if (instrument is not Bond b)
        {
            throw new ArgumentException(
                $"{nameof(BondPricer)} only prices {nameof(Bond)}, got {instrument.GetType().Name}.",
                nameof(instrument));
        }
        decimal coupon = b.FaceValue * b.CouponRate;
        decimal pv = 0m;
        for (int t = 1; t <= b.MaturityYears; t++)
        {
            decimal df = (decimal)Math.Exp((double)(-b.DiscountRate) * t);
            pv += coupon * df;
        }
        pv += b.FaceValue * (decimal)Math.Exp((double)(-b.DiscountRate) * b.MaturityYears);
        return pv;
    }
}
