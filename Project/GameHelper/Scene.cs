using System.Numerics;
using Project.Lighting;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Project.GameHelper;

public abstract class Scene {
    public readonly List<BaseGameObject> GameObjects = new();
    public readonly Camera Camera;
    public readonly IWindow Window;
    private readonly IInputContext _input;

    protected Scene(IWindow window, IInputContext input) {
        Window = window;
        _input = input;
        // Input
        Camera = new Camera(
            Vector3.UnitY,
            Window,
            input);
    }

    public abstract void ApplyCameraAndLighting(DefaultShader shader);
    public virtual void Start() { }
    public virtual void Update(double deltaTime) { }
    public virtual void BeforeRender(double deltaTime) { }
    public virtual void OnWindowMove(Vector2D<int> position) { }
}