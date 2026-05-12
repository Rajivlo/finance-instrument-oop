namespace DesignB.PricingViaStrategy;

/// <summary>Prices a <see cref="Stock"/> by returning its observed market price.</summary>
public sealed class StockPricer : IPricer
{
    /// <inheritdoc />
    public decimal Price(Instrument instrument)
    {
        if (instrument is not Stock s)
        {
            throw new ArgumentException(
                $"{nameof(StockPricer)} only prices {nameof(Stock)}, got {instrument.GetType().Name}.",
                nameof(instrument));
        }
        return s.MarketPrice;
    }
}
