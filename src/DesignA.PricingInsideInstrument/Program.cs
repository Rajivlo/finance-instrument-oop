using DesignA.PricingInsideInstrument;
using SampleMarket;

// Skeleton: instruments throw NotImplementedException until "Design A complete" commit.
Instrument[] portfolio =
{
    new Stock("ACME", Market.SpotPrice),
    new Bond("ACME-5Y", Market.FaceValue, Market.CouponRate, Market.BondMaturityYears, Market.RiskFreeRate),
    new EuropeanOption("ACME-CALL-100", Market.SpotPrice, Market.OptionStrike, Market.OptionMaturityYears, OptionType.Call),
};

Console.WriteLine("Design A — pricing inside instrument (skeleton)");
foreach (var i in portfolio)
{
    Console.WriteLine($"  {i.Name}: (not yet priced)");
}

