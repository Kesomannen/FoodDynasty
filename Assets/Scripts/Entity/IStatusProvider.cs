using System;
using System.Collections.Generic;

public interface IStatusProvider {
    IEnumerable<EntityInfo> GetStatus();
    event Action<IStatusProvider> OnStatusChanged;
}