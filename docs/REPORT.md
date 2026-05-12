# SIT771 Task 7.4 — Something Awesome

**Title:** *Pricing Logic Inside the Object vs. Pricing as a Strategy — a Comparative Study of Two Object-Oriented Designs for a Toy Financial-Instrument Library*

**Student:** Hariharan Poru
**Unit:** SIT771 Object-Oriented Development
**Task:** 7.4 High Distinction — Something Awesome
**Date:** 2026-05-12
**Related material (source code, tests, diagrams):** https://github.com/Rajivlo/finance-instrument-oop

---

## 1. Why this, and what I claim

The brief invites a small research project. Most introductory OO exercises model a domain *once*, in *one* way, and stop there. That hides the question every working OO programmer eventually asks: **when the same problem can be modelled in more than one way, which model should I choose, and why?**

I picked a domain small enough to model end-to-end in a weekend — three financial instruments (a stock, a bond, a European option) priced from a hard-coded sample market — and built it twice. **Design A** uses classical inheritance: every `Instrument` subclass owns its `Price()` method. **Design B** uses the Strategy pattern: instruments are pure data, and pricing lives behind an `IPricer` interface. A shared xUnit suite then asks two questions of both designs, programmatically.

The claim of this report is that the two designs are *behaviourally equivalent for a fixed pricing rule* but *not equivalent under change*, and that the difference is measurable — not as folklore, but as a concrete diff in the codebase and in test setup.

---

## 2. Research question and sub-questions

> **RQ.** When implementing a small financial-instrument pricing library in C#, what are the practical trade-offs between embedding pricing logic inside each `Instrument` subclass (classical inheritance) versus extracting pricing into a separate `IPricer` family (the Strategy pattern)?

Sub-questions answered programmatically:

1. **Equivalence under no change.** Given identical inputs and identical pricing rules, do both designs return the same prices for every instrument and for the total portfolio NAV? *(Answered by `PortfolioEquivalenceTests`.)*
2. **Extensibility under change.** When a *second* way to price the same instrument is required (here, a 1000-path Monte Carlo simulation for the European option), what is the size and shape of the diff in each design? *(Answered by inspecting `MonteCarloOptionPricer.cs` against the alternative of editing `EuropeanOption.cs` directly.)*
3. **Numerical sanity of the extension.** Does the Monte Carlo pricer agree, within 5%, with a closed-form Black–Scholes reference at-the-money and in-the-money? *(Answered by `MonteCarloAccuracyTests`.)*

---

## 3. Method

The method is comparative implementation under controlled inputs:

1. **Shared market.** Both designs draw all parameters from the same `SampleMarket.Market` static class (spot 100, flat rate 5%, vol 20%, bond 4% coupon 5-year on face 1000, ATM 1-year call). The hard-coded market is the *only* permitted source of magic numbers — this is what makes the equivalence test a fair test.
2. **Independent implementations.** `DesignA.PricingInsideInstrument` and `DesignB.PricingViaStrategy` are separate C# projects in one solution. They share *no code* beyond `SampleMarket`. This is deliberate — sharing implementations would beg the question.
3. **Single arbiter — xUnit.** All claims about equivalence are decided by `Assert.Equal` on `decimal`; the Monte Carlo claim is decided by a 5% relative-error tolerance against a closed-form reference held *inside the test assembly*, not inside either production design.
4. **Reproducibility.** The Monte Carlo pricer takes an explicit RNG seed (default = `Market.MonteCarloSeed = 42`); the test that bites hardest (in-the-money) uses seed `123` so a marker re-running the suite gets bit-for-bit identical numbers.
5. **Process discipline.** Each design was built in its own sequence of focused commits (`feat(design-a): …`, `feat(design-b): …`, `test: …`, `docs: …`) so the git log itself is part of the evidence — the history shows the order in which decisions were made, not a single squashed dump.

This is a research method in miniature: a clear question, a controlled setup, an independent arbiter, reproducible numbers, and an audit trail.

---

## 4. Implementation overview

### 4.1 Design A — pricing inside the instrument

`Instrument` is an `abstract class` with `public abstract decimal Price()`. Each subclass overrides `Price()` with the appropriate math. Pricing the portfolio is `portfolio.Sum(x => x.Price())` — polymorphic dispatch by runtime type.

- `Stock.Price()` returns its market quote.
- `Bond.Price()` sums discounted coupons plus discounted face value under continuous compounding on a flat yield curve.
- `EuropeanOption.Price()` returns the undiscounted intrinsic value `max(Spot − Strike, 0)` (the analytical reference used by both designs).

### 4.2 Design B — pricing via Strategy

`Instrument` and its subclasses are *data only* — they hold fields and nothing else. Pricing lives behind:

```csharp
public interface IPricer
{
    decimal Price(Instrument instrument);
}
```

Four concrete pricers are provided:

| Pricer                    | Instrument          | Model                            |
|---------------------------|---------------------|----------------------------------|
| `StockPricer`             | `Stock`             | Last market quote                |
| `BondPricer`              | `Bond`              | Discounted cash flows            |
| `EuropeanOptionPricer`    | `EuropeanOption`    | Intrinsic value (matches A)      |
| `MonteCarloOptionPricer`  | `EuropeanOption`    | 1000-path GBM, risk-neutral      |

The Monte Carlo pricer simulates terminal prices under risk-neutral GBM, `S_T = S_0 · exp((r − ½σ²)T + σ√T · Z)`, draws `Z` via Box–Muller, averages the discounted payoffs, and returns the result as a `decimal`. The number of paths and the RNG seed are explicit and configurable.

### 4.3 Tests

`PortfolioEquivalenceTests` (4 tests) constructs the same one-stock / one-bond / one-option portfolio in both designs and asserts identical prices instrument-by-instrument and at the total-NAV level. `MonteCarloAccuracyTests` (2 tests) asserts that `MonteCarloOptionPricer` agrees with a closed-form Black–Scholes reference within 5% on ATM and ITM calls. Total: **6 passing tests, 0 warnings, 0 errors** under `TreatWarningsAsErrors=true`.

---

## 5. Findings

### 5.1 Behavioural equivalence under no change

All four `PortfolioEquivalenceTests` pass with `Assert.Equal` on `decimal` — that is, *exact* equality, not within-tolerance. **For a fixed pricing rule, the two designs are interchangeable.** The portfolio NAV is 1051.3730 either way.

This is the obvious-once-stated finding that I think is worth saying out loud: the design choice does **not** change *what* the system computes. It only changes the cost of changing it.

### 5.2 Extensibility under change — the actual difference

The Monte Carlo extension is the place where the two designs separate.

- **In Design B**, adding Monte Carlo is one new file — `MonteCarloOptionPricer.cs` — that implements `IPricer`. **Nothing else in the codebase moves.** `EuropeanOption` is untouched. `IPricer` is untouched. The existing `EuropeanOptionPricer` is untouched. Calling code chooses which pricer to use at the call site. This is the Open–Closed Principle showing up as a literal property of the diff.
- **In Design A**, adding Monte Carlo means either (a) editing `EuropeanOption.Price()` directly (which fuses two pricing rules into one method body and forces a runtime flag, breaking single-responsibility), (b) adding a parallel `decimal MonteCarloPrice()` method (which is no longer polymorphic — every caller now needs to know which method to call, and `Instrument` no longer has a uniform pricing surface), or (c) sub-subclassing into `MonteCarloEuropeanOption` (which conflates "what instrument is this" with "how do I price it" — the type system now carries pricing-strategy information, which is wrong).

None of (a)/(b)/(c) is *impossible*; each is *worse* than Design B's one-file extension. Strategy is not magic — it is just a refactor that moves the axis of variation out of the type hierarchy.

### 5.3 Numerical sanity of the extension

The Monte Carlo pricer agrees with closed-form Black–Scholes within 5% on both the ATM and ITM calls (1000 paths is enough for that tolerance with this volatility). This matters because it lets me make the second claim *seriously* — Design B's extra pricer is not a cardboard prop. It actually prices.

### 5.4 Where Design A wins

Design B is not strictly better. Design A's `EuropeanOption.cs` is **readable top-to-bottom** — a new reader sees the option's data and its pricing rule in one file. Design B forces a reader to chase from `EuropeanOption.cs` → `EuropeanOptionPricer.cs` to learn the same thing, and to know that the connection is established at the call site, not by the type system. For a domain where pricing is genuinely one fixed rule per instrument and will never change, Design A is simpler and shorter.

The judgement, then, is not "Strategy is better" but **"Strategy pays its complexity off the moment a second strategy appears for the same data."** That is exactly the OCP intuition, but I now have it grounded in a concrete diff rather than a slogan.

---

## 6. Mapping to SIT771 unit learning outcomes

I have phrased the ULOs in line with the unit guide; the marker should map them onto whichever exact wording applies in the current handbook.

### ULO — *Apply object-oriented programming concepts to design and develop programs*

- **Abstraction.** `Instrument` is an `abstract class` in both designs; in Design A it carries an `abstract` method `Price()` that every subclass must implement.
- **Inheritance.** `Stock`, `Bond`, `EuropeanOption` each derive from `Instrument` in both designs.
- **Polymorphism.** `aPortfolio.Sum(x => x.Price())` in Design A dispatches by runtime type; `bPricers[x.GetType()].Price(x)` in Design B dispatches by lookup. The fact that *both* produce identical NAV is the polymorphism story made concrete.
- **Encapsulation.** Instrument fields are `public get` / private set via constructor; the bond's coupon math and the option's intrinsic-value math live inside the relevant class rather than leaking to callers (Design A), or inside the relevant *pricer* (Design B). Either way, the caller never reaches into a discount-factor loop.
- **Interfaces.** `IPricer` in Design B is a textbook interface-based Strategy. Four concrete implementations live behind it.

### ULO — *Apply problem-solving processes to design and develop a solution to small problems*

- The problem was deliberately reduced: three instruments, one market, one currency, deterministic seeds. The interesting variation lives entirely in the **design** axis, not the **domain** axis. Choosing what to leave out is a problem-solving move on its own.
- I named the research question first, then built only what was needed to answer it. The portfolio is the smallest one that exercises all three subclasses.

### ULO — *Use appropriate skills, processes, and tools to test and debug programs*

- **Tooling.** .NET 8 SDK, xUnit, `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `<GenerateDocumentationFile>true</GenerateDocumentationFile>`, `<Nullable>enable</Nullable>` — i.e. the compiler is on my side, not just consulted.
- **Test design.** The MC test uses a closed-form Black–Scholes reference held *in the test assembly* rather than the intrinsic-value pricer, because intrinsic value is 0 at the money, which makes a 5% relative-error check degenerate. Recognising and routing around that degeneracy is part of the testing skill being assessed.
- **Reproducibility.** Fixed RNG seed; numbers are reproducible across machines.
- **Version control.** 9+ small commits on `main` describing each phase — see `git log --oneline`. The history is part of the evidence.

### ULO — *Communicate the use of programming concepts and the design of code*

- Two Mermaid class diagrams (`docs/design-a.md`, `docs/design-b.md`) — one per design, side-by-side.
- A README with the research question stated up front, a layout map, a build-and-run section, a sample-market table, and a short findings section.
- This report itself, which states the question, the method, the findings, and the trade-offs in prose.

---

## 7. Reflection on the three focus areas in the brief

### 7.1 Syntax

Concrete syntax decisions that I think are worth flagging:

- **`decimal` for money, `double` for math.** All instrument prices and market values are `decimal` (no binary-floating-point surprises in equality tests). All Monte Carlo internals are `double` (because `Math.Exp` and `Math.Log` are `double`-typed and the loss of precision is the right trade for speed at 1000 paths). The cast from `double` back to `decimal` happens at exactly one boundary.
- **`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`** in `Directory.Build.props` so the build only goes green when *every* compiler warning has been dealt with. This caught a real defect during development — a `<see cref="MonteCarloOptionPricer"/>` doc reference written before the class existed broke the build via CS1574 — and I fixed it correctly (placeholder, then `cref` restored when the class arrived) rather than disabling the warning.
- **Nullable reference types enabled** repo-wide. Every reference parameter and field is one of "definitely not null" or "explicitly nullable", and the compiler enforces it.
- **Pattern matching for type checks** in pricers: `if (instrument is not EuropeanOption o) throw new ArgumentException(...)` — narrows the type and binds the variable in one line, more readable than `as` + null-check.
- **`sealed` on every concrete pricer.** They are not designed to be inherited from; the compiler should know that.
- **XML doc comments** on every public type and method, with `<see cref="…"/>` cross-references and an `<example>` block on `IPricer`. `<GenerateDocumentationFile>true</GenerateDocumentationFile>` makes the build fail if any are missing or malformed.

### 7.2 Concept

The concept I most want a marker to notice is that **the Strategy pattern is not a separate "thing" you learn after OO basics — it falls out of taking abstraction and polymorphism seriously and asking "what is the axis of variation here?"** Once "how do I price this instrument?" is recognised as an axis of variation, it gets its own abstract type (`IPricer`), and the original instrument hierarchy collapses to pure data. The same observation drives Open–Closed: a new strategy is a new class, not an edit. The same observation drives testability: a unit test can plug in a fake `IPricer` without constructing a real `EuropeanOption`.

So the unit's concepts — abstraction, inheritance, polymorphism, encapsulation, interfaces — are not five independent boxes. In this project they cooperate, and the cooperation is what produces a design with the open–closed property. I think this is the *actual* point of an OO unit, and it is the point that this project is built to demonstrate.

### 7.3 Process

The process choices I want to flag:

- **Iterative commits, not one big drop.** `git log --oneline` shows: skeleton → Design A pricers → Design B data-only → Design B analytical pricers → Design B Monte Carlo → tests → docs. Each step builds on the previous and stays green. This is the order in which I actually thought about the problem.
- **Build-and-test-locally before pushing.** Every commit passed `dotnet build` and `dotnet test`. The remote did not see anything that did not pass locally.
- **Wrote the test for the equivalence claim before writing the second design's pricers.** Designed-to-test, not tested-after.
- **Refused to share code between the two designs.** It would have been "DRY" to extract a shared coupon-discount helper. I deliberately didn't, because the *point* of the comparison is that the two designs are independent. Premature DRY would have hidden the very thing I was trying to measure.

---

## 8. Limitations and honest caveats

- The math is intentionally crude. Real bond pricers use a discount *curve*, not a flat rate; real option pricers handle dividends, American exercise, and stochastic volatility. The point was the OO design comparison, not the quant.
- Two designs is not a sample size. The conclusions transfer to "inheritance-vs-strategy in pricing-style domains"; they don't transfer to all OO design questions.
- 1000 Monte Carlo paths is enough for a 5% tolerance, not for production. The test tolerance was chosen accordingly.
- Performance was not measured. Both designs should be roughly equivalent in this size of code; whether one is faster at scale is a separate question.

---

## 9. Conclusion

For a fixed pricing rule, both designs produce identical prices — the test suite says so under exact `decimal` equality. The designs separate the moment a second pricing rule appears for the same instrument: Strategy absorbs the change as one new file with no edits to existing types, while inheritance forces a choice between mutating a method body, adding a parallel method, or sub-subclassing. The right answer therefore depends on whether the domain has *one* pricing rule per instrument forever (where inheritance is shorter and more readable) or *more than one* rule is plausible (where Strategy pays off). The unit's core OO concepts cooperate to make this difference legible in code, and the test suite makes it measurable. That cooperation is what I most want this project to demonstrate.

---

## 10. Related material

- **Source code, commit history, tests, and class diagrams:** https://github.com/Rajivlo/finance-instrument-oop
- **Class diagrams:** [`docs/design-a.md`](design-a.md), [`docs/design-b.md`](design-b.md)
- **Top-level overview and build instructions:** [`README.md`](../README.md)
- **Test results:** `dotnet test` — 6 passed, 0 failed, 0 skipped.
