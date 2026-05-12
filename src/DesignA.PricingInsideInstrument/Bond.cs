namespace DesignA.PricingInsideInstrument;

/// <summary>
/// A fixed-coupon bond priced as the sum of discounted cash flows under a flat yield curve.
/// </summary>
public sealed class Bond : Instrument
{
    /// <summary>Face / par value, paid back at maturity.</summary>
    public decimal FaceValue { get; }

    /// <summary>Annual coupon rate (e.g. 0.04 for 4%).</summary>
    public decimal CouponRate { get; }

    /// <summary>Whole years until maturity.</summary>
    public int MaturityYears { get; }

    /// <summary>Flat continuously-compounded discount rate.</summary>
    public decimal DiscountRate { get; }

    /// <summary>Initialises a new <see cref="Bond"/>.</summary>
    public Bond(string name, decimal faceValue, decimal couponRate, int maturityYears, decimal discountRate)
        : base(name)
    {
        FaceValue = faceValue;
        CouponRate = couponRate;
        MaturityYears = maturityYears;
        DiscountRate = discountRate;
    }

    /// <inheritdoc />
    public override decimal Price() => throw new NotImplementedException();
}
