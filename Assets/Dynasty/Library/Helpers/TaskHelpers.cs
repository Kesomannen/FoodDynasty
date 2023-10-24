using System.Threading.Tasks;

namespace Dynasty.Library.Helpers {

public static class TaskHelpers {
    public static Task Wait(float seconds) {
        return Task.Delay((int) (seconds * 1000));
    }
}

}