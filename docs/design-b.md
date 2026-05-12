# Design B — Pricing via Strategy

Class diagram for the strategy-based design where instruments hold only
data and pricing logic lives in `IPricer` implementations.

```mermaid
classDiagram
    direction TB

    class Instrument {
        <<abstract>>
        +string Name
    }

    class Stock {
        +decimal MarketPrice
    }

    class Bond {
        +decimal FaceValue
        +decimal CouponRate
        +int MaturityYears
        +decimal DiscountRate
    }

    class EuropeanOption {
        +decimal Spot
        +decimal Strike
        +double TimeToExpiry
        +OptionType Type
        +double Volatility
        +decimal RiskFreeRate
    }

    class OptionType {
        <<enum>>
        Call
        Put
    }

    class IPricer {
        <<interface>>
        +Price(Instrument) decimal
    }

    class StockPricer {
        +Price(Instrument) decimal
    }

    class BondPricer {
        +Price(Instrument) decimal
    }

    class EuropeanOptionPricer {
        +Price(Instrument) decimal
    }

    class MonteCarloOptionPricer {
        +const int PathCount = 1000
        +Price(Instrument) decimal
    }

    Instrument <|-- Stock
    Instrument <|-- Bond
    Instrument <|-- EuropeanOption
    EuropeanOption ..> OptionType : uses

    IPricer <|.. StockPricer
    IPricer <|.. BondPricer
    IPricer <|.. EuropeanOptionPricer
    IPricer <|.. MonteCarloOptionPricer

    StockPricer ..> Stock : prices
    BondPricer ..> Bond : prices
    EuropeanOptionPricer ..> EuropeanOption : prices
    MonteCarloOptionPricer ..> EuropeanOption : prices
```

### Notes

* `Instrument` and its subclasses are pure data — no behaviour.
* Each pricer accepts the abstract `Instrument` and type-checks the
  concrete type it expects.
* `EuropeanOption` is priced by **two** different `IPricer`s
  (`EuropeanOptionPricer` and `MonteCarloOptionPricer`) without any
  change to the instrument class. Adding a third pricer (e.g. binomial
  tree, Heston model, …) is the same one-class change.
* Tests can mock `IPricer` directly to exercise portfolio code in
  isolation from any specific pricing model.
