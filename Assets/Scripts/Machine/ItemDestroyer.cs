public class ItemDestroyer : ItemMachineComponent {
    protected override void OnItemEntered(Item item) {
        Destroy(item.gameObject);
    }
}