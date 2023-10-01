using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynasty.Persistent {

public abstract class SaveLoader : ScriptableObject {
    public abstract Task Save(Dictionary<string, object> state, int saveId);
    public abstract Task<Dictionary<string, object>> Load(int saveId);
    public abstract Task Delete(int saveId);
    
    public abstract Task<IEnumerable<SaveSlot>> GetSaves();
}

}