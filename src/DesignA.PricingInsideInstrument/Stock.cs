namespace DesignA.PricingInsideInstrument;

/// <summary>
/// A common stock priced by its observed market spot price.
/// </summary>
public sealed class Stock : Instrument
{
    /// <summary>Last observed market price of one share, in USD.</summary>
    public decimal MarketPrice { get; }

    /// <summary>Initialises a new <see cref="Stock"/>.</summary>
    /// <param name="name">Ticker or display name.</param>
    /// <param name="marketPrice">Observed market spot price.</param>
    public Stock(string name, decimal marketPrice) : base(name)
    {
        MarketPrice = marketPrice;
    }

    /// <inheritdoc />
    /// <remarks>For Design A a stock simply prices at its market quote.</remarks>
    public override decimal Price() => MarketPrice;
}
