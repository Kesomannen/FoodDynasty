using System;

[Serializable]
public struct Modifier {
    public double Base;
    public double Percentual;
    public double Additive;
    public double Multiplicative;

    public Modifier(double @base = 0, double percentual = 0, double additive = 0, double multiplicative = 1) {
        Base = @base;
        Percentual = percentual;
        Additive = additive;
        Multiplicative = multiplicative;
    }

    public float Delta => Apply(1);

    public double Apply(double value) {
        return (value + Base) * (1 + Percentual) * Multiplicative + Additive;
    }

    public int Apply(int value) {
        return (int)Apply((double)value);
    }

    public void Bake() {
        Base = Delta;
        Percentual = 0;
        Additive = 0;
        Multiplicative = 1;
    }

    public void Modify(double value, ModifierType type, bool bake = false) {
        switch (type) {
            case ModifierType.Base: Base += value; break;
            case ModifierType.Percentual: Percentual += value; break;
            case ModifierType.Additive: Additive += value; break;
            case ModifierType.Multiplicative:
                if (value < 0) {
                    Multiplicative /= Math.Abs(value);
                } else {
                    Multiplicative *= value;
                }
                break;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (bake) {
            Bake();
        }
    }
    
    public static Modifier operator +(Modifier a, Modifier b) {
        return new Modifier(
            a.Base + b.Base,
            a.Percentual + b.Percentual,
            a.Additive + b.Additive,
            a.Multiplicative * b.Multiplicative
        );
    }
    
    public static Modifier operator -(Modifier a, Modifier b) {
        return new Modifier(
            a.Base - b.Base,
            a.Percentual - b.Percentual,
            a.Additive - b.Additive,
            a.Multiplicative / b.Multiplicative
        );
    }

    public override string ToString() {
        return $"x{Delta:0.##}";
    }
}

public enum ModifierType {
    Base,
    Percentual,
    Additive,
    Multiplicative
}