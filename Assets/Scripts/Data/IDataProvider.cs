public interface IDataProvider<out T> {
    T Data { get; }
}