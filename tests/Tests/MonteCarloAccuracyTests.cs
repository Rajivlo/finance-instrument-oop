using DesignB.PricingViaStrategy;
using SampleMarket;

namespace Tests;

/// <summary>
/// Verifies that <see cref="MonteCarloOptionPricer"/> agrees with a closed-form
/// Black–Scholes reference to within 5% on a vanilla European call —
/// the assignment's stated acceptance criterion.
/// </summary>
/// <remarks>
/// The intrinsic-value <see cref="EuropeanOptionPricer"/> is unsuitable as a
/// reference for an at-the-money option (intrinsic = 0 there, so a 5% tolerance
/// is degenerate). We therefore compare MC against a closed-form Black–Scholes
/// price held inside the test assembly.
/// </remarks>
public class MonteCarloAccuracyTests
{
    [Fact]
    public void MonteCarlo_AgreesWithBlackScholes_WithinFivePercent_AtTheMoney()
    {
        var option = new EuropeanOption(
            name: "ATM-CALL",
            spot: Market.SpotPrice,
            strike: Market.OptionStrike,
            timeToExpiry: Market.OptionMaturityYears,
            type: OptionType.Call,
            volatility: Market.Volatility,
            riskFreeRate: Market.RiskFreeRate);

        var mc = new MonteCarloOptionPricer().Price(option);

        double bs = BlackScholes.Call(
            s: (double)option.Spot,
            k: (double)option.Strike,
            r: (double)option.RiskFreeRate,
            sigma: option.Volatility,
            t: option.TimeToExpiry);

        double relativeError = Math.Abs((double)mc - bs) / bs;
        Assert.True(
            relativeError <= 0.05,
            $"MC={mc:F4}, BS={bs:F4}, relative error={relativeError:P2} exceeds 5%.");
    }

    [Fact]
    public void MonteCarlo_AgreesWithBlackScholes_WithinFivePercent_InTheMoney()
    {
        // ITM call where time value is small relative to intrinsic — tighter check.
        var option = new EuropeanOption(
            name: "ITM-CALL",
            spot: 120m,
            strike: 100m,
            timeToExpiry: 1.0,
            type: OptionType.Call,
            volatility: 0.20,
            riskFreeRate: 0.05m);

        var mc = new MonteCarloOptionPricer(seed: 123).Price(option);
        double bs = BlackScholes.Call(120.0, 100.0, 0.05, 0.20, 1.0);

        double relativeError = Math.Abs((double)mc - bs) / bs;
        Assert.True(
            relativeError <= 0.05,
            $"MC={mc:F4}, BS={bs:F4}, relative error={relativeError:P2} exceeds 5%.");
    }
}
