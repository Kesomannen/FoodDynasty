public interface IFilter<in T> {
    bool Check(T item);
}