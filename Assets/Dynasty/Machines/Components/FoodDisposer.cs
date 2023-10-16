using Dynasty.Food;

namespace Dynasty.Machines {

public class FoodDisposer : FoodMachineComponent {
    protected override void OnTriggered(FoodBehaviour food) {
        food.Dispose();
    }
}

}