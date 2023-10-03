using System;
using System.Collections.Generic;

namespace Dynasty.Library {

public interface IStatusProvider {
    IEnumerable<EntityInfo> GetStatus();
    event Action<IStatusProvider> OnStatusChanged;
}

}