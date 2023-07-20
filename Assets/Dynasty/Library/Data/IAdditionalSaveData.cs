namespace Dynasty.Library.Data {

/// <summary>
/// Implement this interface to create persistent data on your machine component.
/// </summary>
public interface IAdditionalSaveData {
    /// <summary>
    /// Called after the data has been loaded from storage.
    /// </summary>
    /// <remarks>Will only be called once in a component's lifespan. If the data is not found, no calls will be made.</remarks>
    void OnAfterLoad(object data);
    
    /// <summary>
    /// Called before the data is saved to storage.
    /// </summary>
    /// <remarks>May be called multiple times in a component's lifespan</remarks>
    object GetSaveData();
}

/// <summary>
/// Generic implementation of <see cref="IAdditionalSaveData"/>
/// </summary>
public interface IAdditionalSaveData<T> : IAdditionalSaveData {
    /// <inheritdoc cref="IAdditionalSaveData.OnAfterLoad"/>
    void OnAfterLoad(T data);
    
    /// <inheritdoc cref="IAdditionalSaveData.GetSaveData"/>
    new T GetSaveData();

    void IAdditionalSaveData.OnAfterLoad(object data) => OnAfterLoad((T)data);
    object IAdditionalSaveData.GetSaveData() => GetSaveData();
}

}