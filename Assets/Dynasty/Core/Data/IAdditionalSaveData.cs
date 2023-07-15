namespace Dynasty.Core.Data {

public interface IAdditionalSaveData {
    void OnAfterLoad(object data);
    object GetSaveData();
}

public interface IAdditionalSaveData<T> : IAdditionalSaveData {
    void OnAfterLoad(T data);
    new T GetSaveData();

    void IAdditionalSaveData.OnAfterLoad(object data) => OnAfterLoad((T)data);
    object IAdditionalSaveData.GetSaveData() => GetSaveData();
}

}