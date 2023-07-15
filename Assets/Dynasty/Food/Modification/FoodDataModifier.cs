using System;
using System.Reflection;
using Dynasty.Food.Data;
using Dynasty.Food.Instance;
using Dynasty.Library.Classes;
using Dynasty.Library.Helpers;

namespace Dynasty.Food.Modification {

[Serializable]
public class FoodDataModifier {
    public FoodModifierOperation OperationType;
    public FoodDataType DataType;
    public string FieldName;
    
    public Modifier Modifier;
    
    public int IntValue;
    public float FloatValue;
    public bool BoolValue;
    
    public void Apply(FoodBehaviour food) {
        var type = FoodDataUtil.GetDataType(DataType);
        var data = food.EnforceData(type);
        var field = ReflectionHelpers.GetField(type, FieldName);

        switch (OperationType) {
            case FoodModifierOperation.Set:
                SetField(field, data); break;
            case FoodModifierOperation.Modify:
                ModifyField(field, data); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    void SetField(FieldInfo field, object data) {
        if (field.FieldType == typeof(int)) {
            field.SetValue(data, IntValue);
        } else if (field.FieldType == typeof(float)) {
            field.SetValue(data, FloatValue);
        } else if (field.FieldType == typeof(bool)) {
            field.SetValue(data, BoolValue);
        } else {
            throw new ArgumentOutOfRangeException();
        }
    }

    void ModifyField(FieldInfo field, object data) {
        var value = field.GetValue(data);
        if (value is int intValue) {
            field.SetValue(data, Modifier.Apply(intValue));
        } else {
            field.SetValue(data, Modifier.Apply((float)value));
        }
    }
}

}

public enum FoodModifierOperation {
    Set,
    Modify
}