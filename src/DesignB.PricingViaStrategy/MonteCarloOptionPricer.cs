using SampleMarket;

namespace DesignB.PricingViaStrategy;

/// <summary>
/// Prices a <see cref="EuropeanOption"/> by simulating <see cref="PathCount"/>
/// terminal stock prices under geometric Brownian motion in the risk-neutral measure,
/// averaging the discounted payoff.
/// </summary>
/// <remarks>
/// <para>
/// This pricer exists to demonstrate the Strategy pattern's extensibility:
/// adding a second way to price options does NOT require changing
/// <see cref="EuropeanOption"/>, <see cref="IPricer"/>, or any other pricer.
/// </para>
/// <para>
/// Model: <c>S_T = S_0 * exp((r - 0.5 * σ²) * T + σ * sqrt(T) * Z)</c>
/// with <c>Z ~ N(0, 1)</c>. Payoff is then discounted by <c>exp(-r * T)</c>.
/// </para>
/// </remarks>
public sealed class MonteCarloOptionPricer : IPricer
{
    /// <summary>Number of simulation paths.</summary>
    public const int PathCount = 1000;

    private readonly Random _rng;

    /// <summary>
    /// Initialises a new <see cref="MonteCarloOptionPricer"/> with a fixed,
    /// reproducible random seed (<see cref="Market.MonteCarloSeed"/>).
    /// </summary>
    public MonteCarloOptionPricer() : this(Market.MonteCarloSeed) { }

    /// <summary>
    /// Initialises a new <see cref="MonteCarloOptionPricer"/> with the given seed.
    /// </summary>
    /// <param name="seed">Seed for the internal RNG; pass a fixed value for reproducibility.</param>
    public MonteCarloOptionPricer(int seed)
    {
        _rng = new Random(seed);
    }

    /// <inheritdoc />
    public decimal Price(Instrument instrument)
    {
        if (instrument is not EuropeanOption o)
        {
            throw new ArgumentException(
                $"{nameof(MonteCarloOptionPricer)} only prices {nameof(EuropeanOption)}, got {instrument.GetType().Name}.",
                nameof(instrument));
        }

        double s0 = (double)o.Spot;
        double k = (double)o.Strike;
        double r = (double)o.RiskFreeRate;
        double sigma = o.Volatility;
        double t = o.TimeToExpiry;

        double drift = (r - 0.5 * sigma * sigma) * t;
        double diffusion = sigma * Math.Sqrt(t);

        double payoffSum = 0.0;
        for (int i = 0; i < PathCount; i++)
        {
            double z = SampleStandardNormal();
            double sT = s0 * Math.Exp(drift + diffusion * z);
            double payoff = o.Type switch
            {
                OptionType.Call => Math.Max(sT - k, 0.0),
                OptionType.Put => Math.Max(k - sT, 0.0),
                _ => throw new InvalidOperationException($"Unknown option type: {o.Type}"),
            };
            payoffSum += payoff;
        }

        double mean = payoffSum / PathCount;
        double discounted = Math.Exp(-r * t) * mean;
        return (decimal)discounted;
    }

    /// <summary>
    /// Box–Muller transform: turns two uniform draws into one standard-normal sample.
    /// </summary>
    private double SampleStandardNormal()
    {
        double u1 = 1.0 - _rng.NextDouble(); // (0, 1] to avoid log(0)
        double u2 = 1.0 - _rng.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}
