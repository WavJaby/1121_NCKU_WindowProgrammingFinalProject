using System.Numerics;
using Project.GameHelper;

namespace Project.Collision;

public class BoxCollider : Collider {
    public Vector3 MinExtents, MaxExtents;

    public BoxCollider(Vector3 minExtents, Vector3 maxExtents) {
        MinExtents = minExtents;
        MaxExtents = maxExtents;
    }

    public BoxCollider(Vector3 scale) {
        scale /= 2;
        MinExtents = -scale;
        MaxExtents = scale;
    }

    public override bool CheckCollision(GameObject otherGameObject) {
        if (otherGameObject.Collider == null)
            return false;
        // TODO: Support box collider

        // if (otherGameObject.Collider is BoxCollider sphereCollider) {
        //     return Collider.CheckCollision(ForGameObject.Position, ForGameObject.Rotation, this,
        //         otherGameObject.Position, otherGameObject.Rotation, sphereCollider);
        // }
        return false;
    }
}