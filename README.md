# Financial Instruments — OOP Design Comparison

> **Research question.** When building a small financial-instrument pricing
> library in an object-oriented language, what are the practical trade-offs
> between embedding pricing logic inside each `Instrument` subclass
> (classical inheritance) versus extracting pricing into a separate
> `IPricer` family (the Strategy pattern)?

This repository implements the same toy domain twice and exposes a single
test suite that pits the two designs against each other.

---

## Layout

```
.
├── FinancialInstruments.sln
├── Directory.Build.props           # net8.0 + nullable enabled for every project
├── src/
│   ├── SampleMarket/               # shared hard-coded market data
│   ├── DesignA.PricingInsideInstrument/
│   │   ├── Instrument.cs           # abstract base with abstract Price()
│   │   ├── Stock.cs
│   │   ├── Bond.cs
│   │   ├── EuropeanOption.cs
│   │   └── Program.cs              # demo: prints prices
│   └── DesignB.PricingViaStrategy/
│       ├── Instrument.cs           # pure data, no Price()
│       ├── Stock.cs   Bond.cs   EuropeanOption.cs
│       ├── IPricer.cs              # decimal Price(Instrument)
│       ├── StockPricer.cs
│       ├── BondPricer.cs
│       ├── EuropeanOptionPricer.cs        # analytical (intrinsic value)
│       ├── MonteCarloOptionPricer.cs      # 1000-path GBM simulation
│       └── Program.cs
├── tests/Tests/                    # xUnit
│   ├── PortfolioEquivalenceTests.cs
│   ├── MonteCarloAccuracyTests.cs
│   └── BlackScholes.cs             # closed-form reference for MC test
└── docs/
    ├── design-a.md                 # Mermaid class diagram for Design A
    └── design-b.md                 # Mermaid class diagram for Design B
```

---

## Build & run

Prerequisites: **.NET 8 SDK**.

```bash
# Restore + build everything
dotnet build

# Run the two console demos
dotnet run --project src/DesignA.PricingInsideInstrument
dotnet run --project src/DesignB.PricingViaStrategy

# Run the xUnit suite
dotnet test
```

Expected demo output:

```
Design A — pricing inside instrument
  ACME                     100.0000
  ACME-5Y                  951.3730
  ACME-CALL-100              0.0000

Design B — pricing via strategy
  ACME                     100.0000
  ACME-5Y                  951.3730
  ACME-CALL-100              0.0000

Same option, swapped strategy:
  ACME-CALL-100             10.6031  (Monte Carlo, 1000 paths)
```

Expected test output:

```
Passed!  - Failed: 0, Passed: 6, Skipped: 0, Total: 6
```

---

## Sample market data

Both designs use the same hard-coded market in `src/SampleMarket/SampleMarket.cs`:

| Parameter           | Value | Used by               |
|---------------------|-------|------------------------|
| Spot price          | 100   | Stock, Option          |
| Flat risk-free rate | 5%    | Bond, MC option pricer |
| Volatility          | 20%   | MC option pricer       |
| Bond face value     | 1000  | Bond                   |
| Bond coupon         | 4%    | Bond                   |
| Bond maturity       | 5 yr  | Bond                   |
| Option strike       | 100   | Option                 |
| Option maturity     | 1 yr  | Option                 |

The math is intentionally crude — the project is about OOP design,
not quantitative finance.

---

## Design A — pricing inside the instrument

Every `Instrument` subclass owns its `decimal Price()` method.
Code that prices a portfolio just calls `instrument.Price()`.

**Pros:** discoverable, simple, polymorphic dispatch by type.
**Cons:** adding a *new way* to price an instrument (e.g. Monte Carlo
for options) forces editing the `Instrument` hierarchy, violating the
Open–Closed Principle. Mixing market data with valuation logic also
makes the class harder to mock in tests.

See [`docs/design-a.md`](docs/design-a.md) for the class diagram.

---

## Design B — pricing via Strategy

Instruments are pure data. Pricing lives behind `IPricer`.

```csharp
interface IPricer
{
    decimal Price(Instrument instrument);
}
```

We provide four pricers:

| Pricer                    | Instrument          | Model                            |
|---------------------------|---------------------|----------------------------------|
| `StockPricer`             | `Stock`             | Last market quote                |
| `BondPricer`              | `Bond`              | Discounted cash flows            |
| `EuropeanOptionPricer`    | `EuropeanOption`    | Intrinsic value (analytical ref) |
| `MonteCarloOptionPricer`  | `EuropeanOption`    | 1000-path GBM simulation         |

Two different pricers for the same instrument is the point — the
strategy pattern's extensibility shows directly in code.

See [`docs/design-b.md`](docs/design-b.md) for the class diagram.

---

## Test strategy

`tests/Tests` (xUnit) contains two suites:

1. **PortfolioEquivalenceTests.** Builds the same one-stock / one-bond /
   one-option portfolio under both designs and asserts identical prices,
   instrument-by-instrument and at the total NAV level. Confirms that
   the design choice does not change *what* the system computes.

2. **MonteCarloAccuracyTests.** Asserts that `MonteCarloOptionPricer`
   agrees with a closed-form Black–Scholes reference (held in
   `BlackScholes.cs` inside the test assembly) to within 5% on a vanilla
   European call, both at-the-money and in-the-money. The intrinsic-value
   pricer can't serve as the reference because it returns 0 for
   at-the-money options, making "within 5%" degenerate.

---

## Findings (short version)

* For a fixed pricing rule both designs are interchangeable - the
  portfolio-equivalence tests pass with `Assert.Equal` on `decimal`.
* The Strategy design (B) becomes strictly easier to extend the moment
  you want a *second* way to price the same instrument
  (`MonteCarloOptionPricer` is added without touching `EuropeanOption`).
* The inheritance design (A) is easier to read top-to-bottom for a
  reader new to the codebase: everything an `EuropeanOption` does
  lives in one file.

A fuller write-up — research question, method, findings, mapping to SIT771
unit learning outcomes, and reflection on the syntax / concept / process
focus areas — is in [`docs/REPORT.md`](docs/REPORT.md).
