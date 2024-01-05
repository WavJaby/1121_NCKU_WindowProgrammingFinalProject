namespace Project.GameHelper; 

public abstract class BaseGameObject : IDisposable {

    public abstract void Render(Scene scene);

    public abstract void Dispose();
}