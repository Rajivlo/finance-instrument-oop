namespace DesignB.PricingViaStrategy;

/// <summary>Prices a <see cref="Stock"/> by returning its observed market price.</summary>
public sealed class StockPricer : IPricer
{
    /// <inheritdoc />
    public decimal Price(Instrument instrument) => throw new NotImplementedException();
}
