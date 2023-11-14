using Dynasty.Library;

namespace Dynasty.Machines {

public interface IBoostable {
    Modifier Modifier { get; set; }
}

public interface IBoostableProperty : IBoostable {
    FloatDataProperty BoostableProperty { get; }

    Modifier IBoostable.Modifier {
        get => BoostableProperty.Modifier;
        set {
            BoostableProperty.Modifier = value;
            OnModifierChanged();
        }
    }

    protected virtual void OnModifierChanged() { }
}

}