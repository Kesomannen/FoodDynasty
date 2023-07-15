using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynasty.Persistent.Core {

public abstract class SaveLoader : ScriptableObject {
    public abstract Task Save(Dictionary<string, object> state, int slotIndex);
    public abstract Task<Dictionary<string, object>> Load(int slotIndex);
    public abstract Task Delete(int slotIndex);
}

}