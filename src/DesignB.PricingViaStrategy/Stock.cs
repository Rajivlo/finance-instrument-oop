namespace DesignB.PricingViaStrategy;

/// <summary>A common stock — pure data; pricing is delegated to a pricer.</summary>
public sealed class Stock : Instrument
{
    /// <summary>Last observed market price.</summary>
    public decimal MarketPrice { get; }

    /// <summary>Initialises a new <see cref="Stock"/>.</summary>
    public Stock(string name, decimal marketPrice) : base(name)
    {
        MarketPrice = marketPrice;
    }
}
