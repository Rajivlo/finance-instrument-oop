namespace Tests;

/// <summary>
/// Closed-form Black–Scholes prices for vanilla European options.
/// Used by the test suite as a stable analytical reference for Monte Carlo accuracy.
/// </summary>
internal static class BlackScholes
{
    /// <summary>Price of a vanilla European call under Black–Scholes.</summary>
    public static double Call(double s, double k, double r, double sigma, double t)
    {
        double sqrtT = Math.Sqrt(t);
        double d1 = (Math.Log(s / k) + (r + 0.5 * sigma * sigma) * t) / (sigma * sqrtT);
        double d2 = d1 - sigma * sqrtT;
        return s * NormCdf(d1) - k * Math.Exp(-r * t) * NormCdf(d2);
    }

    /// <summary>
    /// Standard-normal CDF via the Abramowitz &amp; Stegun 7.1.26 approximation.
    /// Accurate to ~7.5e-8 — plenty for a 5% test tolerance.
    /// </summary>
    private static double NormCdf(double x)
    {
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;

        int sign = x < 0 ? -1 : 1;
        double absX = Math.Abs(x) / Math.Sqrt(2.0);
        double tt = 1.0 / (1.0 + p * absX);
        double y = 1.0 - (((((a5 * tt + a4) * tt) + a3) * tt + a2) * tt + a1) * tt * Math.Exp(-absX * absX);
        return 0.5 * (1.0 + sign * y);
    }
}
