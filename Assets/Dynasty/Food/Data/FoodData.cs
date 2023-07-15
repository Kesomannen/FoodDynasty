namespace Dynasty.Food.Data {

public struct Bread { }

public struct Cookable {
    public bool Cooked;
}

public struct Softened {
    public int Amount;
}

public struct Spice {
    public bool AddedBeforeCooking;
    public bool AddedAfterCooking;
}

public struct WholeWheat {
    public bool Added;
}

public struct Cleaned {
    public bool Demolded;
    public bool Derustified;
}

}