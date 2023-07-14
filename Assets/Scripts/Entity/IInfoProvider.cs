using System.Collections.Generic;

public interface IInfoProvider {
    IEnumerable<EntityInfo> GetInfo();
}