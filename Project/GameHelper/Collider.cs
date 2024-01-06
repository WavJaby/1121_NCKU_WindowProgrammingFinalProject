using System.Numerics;
using Project.Collision;

namespace Project.GameHelper;

public abstract class Collider {
    internal GameObject ForGameObject = null!;
    public bool IsTrigger;

    public delegate void OnCollisionHandler(GameObject self, GameObject other);
    public event OnCollisionHandler? OnCollision;
    
    public void OnCollision_(GameObject self, GameObject other) {
        OnCollision?.Invoke(self, other);
    }

    public static bool CheckCollision(Vector3 aPos, SphereCollider a, Vector3 bPos, SphereCollider b) {
        return (bPos - aPos).Length() < a.Radius + b.Radius;
    }

    public abstract bool CheckCollision(GameObject otherGameObject);
}