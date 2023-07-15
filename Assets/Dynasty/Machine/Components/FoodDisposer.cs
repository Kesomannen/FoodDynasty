using Dynasty.Food.Instance;
using Dynasty.Machine.Internal;

namespace Dynasty.Machine.Components {

public class FoodDisposer : FoodMachineComponent {
    protected override void OnTriggered(FoodBehaviour food) {
        food.Dispose();
    }
}

}