namespace Project;

internal abstract class Project {
    private static void Main(string[] args) {
        new Thread(() => new GameWindow()).Start();
    }
}