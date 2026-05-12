namespace DesignB.PricingViaStrategy;

/// <summary>Type of European option payoff at expiry.</summary>
public enum OptionType
{
    /// <summary>Right to buy.</summary>
    Call,
    /// <summary>Right to sell.</summary>
    Put,
}

/// <summary>A European option — pure data; pricing is delegated to a pricer.</summary>
public sealed class EuropeanOption : Instrument
{
    /// <summary>Strike price.</summary>
    public decimal Strike { get; }
    /// <summary>Spot price of the underlying.</summary>
    public decimal Spot { get; }
    /// <summary>Years to expiry.</summary>
    public double TimeToExpiry { get; }
    /// <summary>Call or put.</summary>
    public OptionType Type { get; }
    /// <summary>Annualised volatility of the underlying (used by Monte Carlo).</summary>
    public double Volatility { get; }
    /// <summary>Continuously-compounded risk-free rate (used by Monte Carlo).</summary>
    public decimal RiskFreeRate { get; }

    /// <summary>Initialises a new <see cref="EuropeanOption"/>.</summary>
    public EuropeanOption(
        string name,
        decimal spot,
        decimal strike,
        double timeToExpiry,
        OptionType type,
        double volatility,
        decimal riskFreeRate)
        : base(name)
    {
        Spot = spot;
        Strike = strike;
        TimeToExpiry = timeToExpiry;
        Type = type;
        Volatility = volatility;
        RiskFreeRate = riskFreeRate;
    }
}
