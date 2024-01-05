using System.Numerics;
using Project.GameHelper;

namespace Project.Collision;

public class SphereCollider : Collider {
    public Vector3 Radius;

    public SphereCollider(float radius) {
        Radius = new Vector3(radius);
    }

    public SphereCollider(Vector3 radius) {
        Radius = radius;
    }

    public override bool CheckCollision(GameObject otherGameObject) {
        if (otherGameObject.Collider == null)
            return false;

        if (otherGameObject.Collider is SphereCollider sphereCollider) {
            return Collider.CheckCollision(ForGameObject.Position, this, otherGameObject.Position, sphereCollider);
        }
        // TODO: Support other collider
        return false;
    }
}