using System.Numerics;
using Project.Collision;
using Project.defaultObjects;
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

    public abstract void ApplyCameraUI(UIShader shader);
    public virtual void Start() { }
    public virtual void Update(double deltaTime) { }
    public virtual void BeforeRender(double deltaTime) { }
    public virtual void AfterRender(double deltaTime) { }
    public virtual void OnWindowMove(Vector2D<int> position) { }

    internal void BeforeRender_(double deltaTime) {
        BeforeRender(deltaTime);

        foreach (BaseGameObject gameObject in GameObjects) {
            if (gameObject is Group group) {
                foreach (GameObject child in group.GameObjects)
                    BeforeRenderGameObject(child, deltaTime);
            } else if (gameObject is GameObject g)
                BeforeRenderGameObject(g, deltaTime);
        }
    }

    private void BeforeRenderGameObject(GameObject gameObject, double deltaTime) {
        // Calculate collision
        foreach (BaseGameObject other in GameObjects) {
            if (other is Group group) {
                foreach (GameObject child in group.GameObjects) {
                    if (gameObject == child) continue;
                    CheckGameObjectCollision(gameObject, child);
                }
            } else if (other is GameObject otherG) {
                if (gameObject == otherG) continue;
                CheckGameObjectCollision(gameObject, otherG);
            }
        }

        // Calculate force
        gameObject.UpdateVelocity(deltaTime);
    }

    private void CheckGameObjectCollision(GameObject self, GameObject other) {
        if (self.Collider == null || other.Collider == null) return;
        if (self.Collider.CheckCollision(other)) {
            // Sphere collider
            if (!self.Collider.IsTrigger && !other.Collider.IsTrigger &&
                self.Collider is SphereCollider selfCollider && other.Collider is SphereCollider otherCollider) {
                Vector3 dir = Vector3.Normalize(other.Position - self.Position);
                self.Position = other.Position - dir *
                    (otherCollider.Radius + selfCollider.Radius);
            }

            self.Collider.OnCollision_(self, other);
            other.Collider.OnCollision_(other, self);
        }
    }
}