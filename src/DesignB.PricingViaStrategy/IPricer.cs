namespace DesignB.PricingViaStrategy;

/// <summary>
/// Strategy interface for pricing an <see cref="Instrument"/>.
/// Implementations decide which concrete instrument types they support
/// — see e.g. <see cref="StockPricer"/>, <see cref="BondPricer"/>,
/// <see cref="EuropeanOptionPricer"/>, <see cref="MonteCarloOptionPricer"/>.
/// </summary>
public interface IPricer
{
    /// <summary>Computes the fair price of <paramref name="instrument"/>.</summary>
    /// <param name="instrument">The instrument to price; must be a type this pricer supports.</param>
    /// <returns>The price as a <see cref="decimal"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the pricer does not support the runtime type of <paramref name="instrument"/>.
    /// </exception>
    decimal Price(Instrument instrument);
}
