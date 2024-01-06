using System.Numerics;
using Project.GameHelper;

namespace Project.Collision;

public class SphereCollider : Collider {
    public float Radius;

    public SphereCollider(float radius, bool isTrigger = false) {
        Radius = radius;
        IsTrigger = isTrigger;
    }

    public override bool CheckCollision(GameObject otherGameObject) {
        if (otherGameObject.Collider is SphereCollider sphereCollider) {
            return Collider.CheckCollision(ForGameObject.Position, this, otherGameObject.Position, sphereCollider);
        }
        // TODO: Support other collider
        return false;
    }
}