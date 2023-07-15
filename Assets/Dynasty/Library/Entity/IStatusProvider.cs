using System;
using System.Collections.Generic;

namespace Dynasty.Library.Entity {

public interface IStatusProvider {
    IEnumerable<EntityInfo> GetStatus();
    event Action<IStatusProvider> OnStatusChanged;
}

}