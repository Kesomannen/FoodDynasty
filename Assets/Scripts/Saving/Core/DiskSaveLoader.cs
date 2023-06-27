using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Saving/Loader/Disk")]
public class DiskSaveLoader : SaveLoader {
    static string GetSavePath(int slotIndex) {
        return Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.dynasty");
    }

    public override Task Save(Dictionary<string, object> state, int slotIndex) {
        var dataStream = new FileStream(GetSavePath(slotIndex), FileMode.Create);

        try {
            var formatter = new BinaryFormatter();
            formatter.Serialize(dataStream, state);
        } finally {
            dataStream.Close();   
        }
        
        return Task.CompletedTask;
    }

    public override Task<Dictionary<string, object>> Load(int slotIndex) {
        var savePath = GetSavePath(slotIndex);
        
        if (!File.Exists(savePath)) {
            return Task.FromResult(new Dictionary<string, object>());
        }
        
        var dataStream = new FileStream(savePath, FileMode.Open);

        try {
            var formatter = new BinaryFormatter();
            var data = (Dictionary<string, object>) formatter.Deserialize(dataStream);
            return Task.FromResult(data);
        } finally {
            dataStream.Close();
        }
    }

    public override Task Delete(int slotIndex) {
        var savePath = GetSavePath(slotIndex);
        
        if (File.Exists(savePath)) {
            File.Delete(savePath);
        }
        
        return Task.CompletedTask;
    }
}