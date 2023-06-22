public class FoodDisposer : FoodMachineComponent {
    protected override void OnTriggered(Food food) {
        food.Dispose();
    }
}