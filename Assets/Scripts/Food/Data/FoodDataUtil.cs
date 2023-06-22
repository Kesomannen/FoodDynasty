using System;
using System.Collections.Generic;

public static class FoodDataUtil {
    static readonly Dictionary<FoodDataType, Type> _foodDataLookup = new() {
        { FoodDataType.Cookable, typeof(Cookable) },
        { FoodDataType.Spice, typeof(Spice) },
        { FoodDataType.Softened, typeof(Softened) }
    };
    
    public static Type GetDataType(FoodDataType type) {
        return _foodDataLookup[type];
    }
}

public enum FoodDataType {
    Cookable,
    Spice,
    Softened
}