using System.Collections.Generic;

namespace Dynasty.Library {

public interface IInfoProvider {
    IEnumerable<EntityInfo> GetInfo();
}

}