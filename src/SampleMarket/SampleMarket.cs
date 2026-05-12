namespace SampleMarket;

/// <summary>
/// Hard-coded market data shared by both pricing designs.
/// Centralised so Design A and Design B always price against identical inputs,
/// making the design comparison meaningful (price differences come from the
/// design, not the data).
/// </summary>
public static class Market
{
    /// <summary>Current spot price of the single sample stock, in USD.</summary>
    public const decimal SpotPrice = 100m;

    /// <summary>Annual continuously-compounded flat risk-free / discount rate.</summary>
    public const decimal RiskFreeRate = 0.05m;

    /// <summary>Annualised volatility of the sample stock (used by Monte Carlo).</summary>
    public const double Volatility = 0.20;

    /// <summary>Annual coupon rate of the sample bond.</summary>
    public const decimal CouponRate = 0.04m;

    /// <summary>Face value of the sample bond, in USD.</summary>
    public const decimal FaceValue = 1000m;

    /// <summary>Years to maturity for the sample bond.</summary>
    public const int BondMaturityYears = 5;

    /// <summary>Strike price of the sample European call, in USD.</summary>
    public const decimal OptionStrike = 100m;

    /// <summary>Years to maturity for the sample European call.</summary>
    public const double OptionMaturityYears = 1.0;

    /// <summary>
    /// Fixed seed used by Monte Carlo so test runs are reproducible.
    /// Real production code would not use a fixed seed.
    /// </summary>
    public const int MonteCarloSeed = 42;
}
