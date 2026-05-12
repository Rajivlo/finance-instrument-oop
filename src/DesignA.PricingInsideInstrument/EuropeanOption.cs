namespace DesignA.PricingInsideInstrument;

/// <summary>
/// Type of European option payoff at expiry.
/// </summary>
public enum OptionType
{
    /// <summary>Right to buy the underlying at <c>Strike</c>.</summary>
    Call,
    /// <summary>Right to sell the underlying at <c>Strike</c>.</summary>
    Put,
}

/// <summary>
/// European option priced (in Design A) by its undiscounted intrinsic value.
/// </summary>
/// <remarks>
/// Intrinsic value is a deliberately crude price model — it ignores time value
/// and volatility. The assignment focuses on OOP design, not quant accuracy.
/// </remarks>
public sealed class EuropeanOption : Instrument
{
    /// <summary>Strike price of the option.</summary>
    public decimal Strike { get; }

    /// <summary>Current spot price of the underlying.</summary>
    public decimal Spot { get; }

    /// <summary>Years to expiry (used by Monte Carlo in Design B).</summary>
    public double TimeToExpiry { get; }

    /// <summary>Call or put.</summary>
    public OptionType Type { get; }

    /// <summary>Initialises a new <see cref="EuropeanOption"/>.</summary>
    public EuropeanOption(string name, decimal spot, decimal strike, double timeToExpiry, OptionType type)
        : base(name)
    {
        Spot = spot;
        Strike = strike;
        TimeToExpiry = timeToExpiry;
        Type = type;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Returns the undiscounted intrinsic value:
    /// <list type="bullet">
    /// <item><c>max(spot - strike, 0)</c> for a call</item>
    /// <item><c>max(strike - spot, 0)</c> for a put</item>
    /// </list>
    /// </remarks>
    public override decimal Price() => Type switch
    {
        OptionType.Call => Math.Max(Spot - Strike, 0m),
        OptionType.Put => Math.Max(Strike - Spot, 0m),
        _ => throw new InvalidOperationException($"Unknown option type: {Type}"),
    };
}
