using System.Collections.Generic;

namespace Dynasty.Library.Entity {

public interface IInfoProvider {
    IEnumerable<EntityInfo> GetInfo();
}

}