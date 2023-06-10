using System;

public class ItemDisposer : ItemMachineComponent {
    protected override void OnItemEntered(Item item) {
        if (item is not IDisposable disposable) return;
        disposable.Dispose();
    }
}