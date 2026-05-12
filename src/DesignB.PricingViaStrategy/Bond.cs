namespace DesignB.PricingViaStrategy;

/// <summary>A fixed-coupon bond — pure data; pricing is delegated to a pricer.</summary>
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
}
