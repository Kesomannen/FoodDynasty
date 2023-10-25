namespace Dynasty.Library {

/// <summary>
/// Simple interface for providing data.
/// </summary>
public interface IDataProvider<out T> {
    T Data { get; }
}

}