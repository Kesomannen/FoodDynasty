using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ModifierField : BindableElement {
    public new class UxmlFactory : UxmlFactory<ModifierField, UxmlTraits> { }
    
    public new class UxmlTraits : BindableElement.UxmlTraits {
        readonly UxmlStringAttributeDescription _label = new() { name = "Label", defaultValue = "Modifier" };
        readonly UxmlDoubleAttributeDescription _flat = new() { name = "Flat", defaultValue = 0 };
        readonly UxmlDoubleAttributeDescription _percentual = new() { name = "Percentual", defaultValue = 0 };
        readonly UxmlDoubleAttributeDescription _multiplicative = new() { name = "Multiplicative", defaultValue = 1 };
        readonly UxmlDoubleAttributeDescription _base = new() { name = "Base", defaultValue = 0 };

        public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc) {
            base.Init(element, bag, cc);
            
            var field = (ModifierField) element;
            field.Label = _label.GetValueFromBag(bag, cc);
            field.Flat = _flat.GetValueFromBag(bag, cc);
            field.Percentual = _percentual.GetValueFromBag(bag, cc);
            field.Multiplicative = _multiplicative.GetValueFromBag(bag, cc);
            field.Base = _base.GetValueFromBag(bag, cc);
        }
    }
    
    Label _label;
    string _labelValue;
    
    DoubleField _flatField;
    DoubleField _percentualField;
    DoubleField _multiplicativeField;
    DoubleField _baseField;
    
    double _flat;
    double _percentual;
    double _multiplicative;
    double _base;

    public ModifierField() {
        _label = new Label();
        _label.AddToClassList("unity-base-field__label");
        Add(_label);
        
        Add(_flatField = new DoubleField("Flat") { bindingPath = "_flat" });
        Add(_percentualField = new DoubleField("Percentual") { bindingPath = "_percentual" });
        Add(_multiplicativeField = new DoubleField("Multiplicative") { bindingPath = "_multiplicative" });
        Add(_baseField = new DoubleField("Base") { bindingPath = "_base" });
        
        AddToClassList("modifier-field");
    }

    public string Label {
        get => _labelValue;
        set {
            _labelValue = value;

            _label.text = _labelValue;
            _label.style.display = string.IsNullOrWhiteSpace(_labelValue) ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }

    public double Flat {
        get => _flat;
        set {
            _flat = value;
            _flatField.SetValueWithoutNotify(_flat);
        }
    }

    public double Percentual {
        get => _percentual;
        set {
            _percentual = value;
            _percentualField.SetValueWithoutNotify(_percentual);
        }
    }

    public double Multiplicative {
        get => _multiplicative;
        set {
            _multiplicative = value;
            _multiplicativeField.SetValueWithoutNotify(_multiplicative);
        }
    }

    public double Base {
        get => _base;
        set {
            _base = value;
            _baseField.SetValueWithoutNotify(_base);
        }
    }

    public Modifier Value => new(Flat, Percentual, Multiplicative, Base);
}