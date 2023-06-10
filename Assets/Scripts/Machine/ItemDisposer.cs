using System;

public class ItemDisposer : ItemMachineComponent {
    protected override void OnTriggered(Item item) {
        if (item is not IDisposable disposable) return;
        disposable.Dispose();
    }
}