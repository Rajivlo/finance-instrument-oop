using SampleMarket;
using A = DesignA.PricingInsideInstrument;
using B = DesignB.PricingViaStrategy;

namespace Tests;

/// <summary>
/// Verifies the central research question of the project:
/// when both designs implement the same pricing rules,
/// do they produce the same prices for an identical portfolio?
/// </summary>
public class PortfolioEquivalenceTests
{
    /// <summary>Stock price must agree between Design A and Design B.</summary>
    [Fact]
    public void Stock_PricesAgreeAcrossDesigns()
    {
        var a = new A.Stock("ACME", Market.SpotPrice);
        var b = new B.Stock("ACME", Market.SpotPrice);
        var bPricer = new B.StockPricer();

        Assert.Equal(a.Price(), bPricer.Price(b));
    }

    /// <summary>Bond price must agree between Design A and Design B.</summary>
    [Fact]
    public void Bond_PricesAgreeAcrossDesigns()
    {
        var a = new A.Bond("ACME-5Y", Market.FaceValue, Market.CouponRate, Market.BondMaturityYears, Market.RiskFreeRate);
        var b = new B.Bond("ACME-5Y", Market.FaceValue, Market.CouponRate, Market.BondMaturityYears, Market.RiskFreeRate);
        var bPricer = new B.BondPricer();

        Assert.Equal(a.Price(), bPricer.Price(b));
    }

    /// <summary>European call intrinsic value must agree between Design A and Design B.</summary>
    [Fact]
    public void EuropeanOption_PricesAgreeAcrossDesigns()
    {
        var a = new A.EuropeanOption(
            "ACME-CALL-100",
            Market.SpotPrice,
            Market.OptionStrike,
            Market.OptionMaturityYears,
            A.OptionType.Call);

        var b = new B.EuropeanOption(
            "ACME-CALL-100",
            Market.SpotPrice,
            Market.OptionStrike,
            Market.OptionMaturityYears,
            B.OptionType.Call,
            Market.Volatility,
            Market.RiskFreeRate);

        var bPricer = new B.EuropeanOptionPricer();

        Assert.Equal(a.Price(), bPricer.Price(b));
    }

    /// <summary>
    /// Whole-portfolio equivalence: the total NAV of the three sample instruments
    /// must be identical when summed using Design A's intrinsic methods or
    /// Design B's analytical pricers.
    /// </summary>
    [Fact]
    public void Portfolio_TotalPriceAgreesAcrossDesigns()
    {
        // Design A
        A.Instrument[] aPortfolio =
        {
            new A.Stock("ACME", Market.SpotPrice),
            new A.Bond("ACME-5Y", Market.FaceValue, Market.CouponRate, Market.BondMaturityYears, Market.RiskFreeRate),
            new A.EuropeanOption("ACME-CALL-100", Market.SpotPrice, Market.OptionStrike, Market.OptionMaturityYears, A.OptionType.Call),
        };
        decimal aTotal = aPortfolio.Sum(x => x.Price());

        // Design B
        B.Instrument[] bPortfolio =
        {
            new B.Stock("ACME", Market.SpotPrice),
            new B.Bond("ACME-5Y", Market.FaceValue, Market.CouponRate, Market.BondMaturityYears, Market.RiskFreeRate),
            new B.EuropeanOption(
                "ACME-CALL-100", Market.SpotPrice, Market.OptionStrike, Market.OptionMaturityYears,
                B.OptionType.Call, Market.Volatility, Market.RiskFreeRate),
        };
        var bPricers = new Dictionary<Type, B.IPricer>
        {
            [typeof(B.Stock)] = new B.StockPricer(),
            [typeof(B.Bond)] = new B.BondPricer(),
            [typeof(B.EuropeanOption)] = new B.EuropeanOptionPricer(),
        };
        decimal bTotal = bPortfolio.Sum(x => bPricers[x.GetType()].Price(x));

        Assert.Equal(aTotal, bTotal);
    }
}
