using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynasty.Persistent.Core {

[CreateAssetMenu(menuName = "Saving/Loader/Disk")]
public class DiskSaveLoader : SaveLoader {
    static string GetSavePath(int saveId) {
        return Path.Combine(Application.persistentDataPath, $"save_{saveId}.dynasty");
    }

    public override Task Save(Dictionary<string, object> state, int saveId) {
        var dataStream = new FileStream(GetSavePath(saveId), FileMode.Create);

        try {
            var formatter = new BinaryFormatter();
            formatter.Serialize(dataStream, state);
        } finally {
            dataStream.Close();   
        }
        
        return Task.CompletedTask;
    }

    public override Task<Dictionary<string, object>> Load(int saveId) {
        var savePath = GetSavePath(saveId);
        
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

    public override Task Delete(int saveId) {
        var savePath = GetSavePath(saveId);
        
        if (File.Exists(savePath)) {
            File.Delete(savePath);
        }
        
        return Task.CompletedTask;
    }

    public override Task<IEnumerable<SaveSlot>> GetSaves() {
        return Task.FromResult(Directory.GetFiles(Application.persistentDataPath, "save_*.dynasty")
            .Select(path => {
                var id = int.Parse(Path.GetFileNameWithoutExtension(path).Split('_')[1]);
                var fileInfo = new FileInfo(path);
                return new SaveSlot {
                    Id = id,
                    LastPlayed = fileInfo.LastWriteTime
                };
            })
        );
    }
}

}