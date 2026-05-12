using DesignB.PricingViaStrategy;
using SampleMarket;

Instrument[] portfolio =
{
    new Stock("ACME", Market.SpotPrice),
    new Bond("ACME-5Y", Market.FaceValue, Market.CouponRate, Market.BondMaturityYears, Market.RiskFreeRate),
    new EuropeanOption(
        "ACME-CALL-100",
        Market.SpotPrice,
        Market.OptionStrike,
        Market.OptionMaturityYears,
        OptionType.Call,
        Market.Volatility,
        Market.RiskFreeRate),
};

// Skeleton: pricers throw NotImplementedException until "Design B complete" commit.
Console.WriteLine("Design B — pricing via strategy (skeleton)");
foreach (var i in portfolio)
{
    Console.WriteLine($"  {i.Name}: (not yet priced)");
}

