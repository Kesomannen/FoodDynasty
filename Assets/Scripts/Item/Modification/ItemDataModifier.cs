using System;
using System.Reflection;

[Serializable]
public class ItemDataModifier {
    public ItemModifierOperationType OperationType;
    public ItemDataType DataType;
    public string FieldName;
    
    public Modifier Modifier;
    
    public int IntValue;
    public float FloatValue;
    public bool BoolValue;
    
    public void Apply(Item item) {
        var type = ItemDataUtil.GetDataType(DataType);
        var data = item.EnforceData(type);
        var field = ReflectionHelpers.GetField(type, FieldName);

        switch (OperationType) {
            case ItemModifierOperationType.Set:
                SetField(field, data); break;
            case ItemModifierOperationType.Modifier:
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

public enum ItemModifierOperationType {
    Set,
    Modifier
}