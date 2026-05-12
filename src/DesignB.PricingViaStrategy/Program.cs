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

var pricers = new Dictionary<Type, IPricer>
{
    [typeof(Stock)] = new StockPricer(),
    [typeof(Bond)] = new BondPricer(),
    [typeof(EuropeanOption)] = new EuropeanOptionPricer(),
};

Console.WriteLine("Design B — pricing via strategy");
foreach (var i in portfolio)
{
    var pricer = pricers[i.GetType()];
    Console.WriteLine($"  {i.Name,-20} {pricer.Price(i),12:F4}");
}

