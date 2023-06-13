public class ItemDisposer : ItemMachineComponent {
    protected override void OnTriggered(Item item) {
        item.Dispose();
    }
}