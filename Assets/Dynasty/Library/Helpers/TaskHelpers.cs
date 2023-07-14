using System.Threading.Tasks;

namespace Dynasty.Library.Helpers {

public static class TaskHelpers {
    public static Task Delay(float seconds) {
        return Task.Delay((int) (seconds * 1000));
    }
}

}