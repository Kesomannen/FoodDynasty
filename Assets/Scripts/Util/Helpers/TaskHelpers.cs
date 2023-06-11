using System.Threading.Tasks;

public static class TaskHelpers {
    public static Task Delay(float seconds) {
        return Task.Delay((int) (seconds * 1000));
    }
}