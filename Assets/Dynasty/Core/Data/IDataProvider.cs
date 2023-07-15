namespace Dynasty.Core.Data {

public interface IDataProvider<out T> {
    T Data { get; }
}

}