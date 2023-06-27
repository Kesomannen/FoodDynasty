public interface IAdditionalSaveData {
    void OnAfterLoad(object data);
    object GetSaveData();
}