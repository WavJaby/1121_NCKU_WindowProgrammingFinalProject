using System.Numerics;
using Project.Collision;

namespace Project.GameHelper;

public abstract class Collider {
    internal GameObject ForGameObject = null!;

    public static bool CheckCollision(Vector3 aPos, SphereCollider a, Vector3 bPos, SphereCollider b) {
        // Calculate the inverse of the combined radii
        Vector3 invRadii = new Vector3(1.0f / a.Radius.X + 1.0f / b.Radius.X,
            1.0f / a.Radius.Y + 1.0f / b.Radius.Y,
            1.0f / a.Radius.Z + 1.0f / b.Radius.Z);

        // Apply the inverse radii scaling to both ellipsoid centers
        Vector3 scaledCenter1 = Vector3.Multiply(aPos, invRadii);
        Vector3 scaledCenter2 = Vector3.Multiply(bPos, invRadii);

        // Calculate the distance between the scaled centers
        Vector3 distance = scaledCenter2 - scaledCenter1;
        float distanceLength = distance.Length();

        // Check for collision (since both are unit spheres after scaling, check if distance is less than 2)
        return distanceLength < 4.0f;
    }

    public static bool CheckCollision(Vector3 aPos, Vector3 aRotation, BoxCollider a, Vector3 bPos, Vector3 bRotation, BoxCollider b) {
        // Calculate the AABB of the first rotated box
        var aAABB = CalculateRotatedAABB(a, aRotation);
        // Calculate the AABB of the second rotated box
        var bAABB = CalculateRotatedAABB(b, bRotation);

        // Check for overlap in all three dimensions
        return (aPos.X + aAABB.MaxExtents.X >= bPos.X + bAABB.MinExtents.X && aPos.X + aAABB.MinExtents.X <= bPos.X + bAABB.MaxExtents.X) &&
               (aPos.Y + aAABB.MaxExtents.Y >= bPos.Y + bAABB.MinExtents.Y && aPos.Y + aAABB.MinExtents.Y <= bPos.Y + bAABB.MaxExtents.Y) &&
               (aPos.Z + aAABB.MaxExtents.Z >= bPos.Z + bAABB.MinExtents.Z && aPos.Z + aAABB.MinExtents.Z <= bPos.Z + bAABB.MaxExtents.Z);
    }

    private static BoxCollider CalculateRotatedAABB(BoxCollider box, Vector3 rotation) {
        // Create a new AABB that encloses the rotated box
        Quaternion rotationQuat = Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);

        // Transform all eight corners of the box
        Vector3[] corners = GetBoxCorners(box);
        for (int i = 0; i < corners.Length; i++) {
            corners[i] = RotatePoint(corners[i], rotationQuat);
        }

        // Find min and max extents from the transformed corners
        Vector3 min = corners[0];
        Vector3 max = corners[0];
        foreach (var corner in corners) {
            min = Vector3.Min(min, corner);
            max = Vector3.Max(max, corner);
        }

        return new BoxCollider(min, max);
    }

    private static Vector3[] GetBoxCorners(BoxCollider box) {
        return new Vector3[] {
            new Vector3(box.MinExtents.X, box.MinExtents.Y, box.MinExtents.Z),
            new Vector3(box.MinExtents.X, box.MinExtents.Y, box.MaxExtents.Z),
            new Vector3(box.MinExtents.X, box.MaxExtents.Y, box.MinExtents.Z),
            new Vector3(box.MinExtents.X, box.MaxExtents.Y, box.MaxExtents.Z),
            new Vector3(box.MaxExtents.X, box.MinExtents.Y, box.MinExtents.Z),
            new Vector3(box.MaxExtents.X, box.MinExtents.Y, box.MaxExtents.Z),
            new Vector3(box.MaxExtents.X, box.MaxExtents.Y, box.MinExtents.Z),
            new Vector3(box.MaxExtents.X, box.MaxExtents.Y, box.MaxExtents.Z)
        };
    }

    private static Vector3 RotatePoint(Vector3 point, Quaternion rotation) {
        // Convert point to Quaternion with w = 0
        Quaternion pointQuat = new Quaternion(point.X, point.Y, point.Z, 0);

        // Rotate the point
        Quaternion rotatedPoint = rotation * pointQuat * Quaternion.Inverse(rotation);

        // Return the rotated point as a Vector3
        return new Vector3(rotatedPoint.X, rotatedPoint.Y, rotatedPoint.Z);
    }

    public abstract bool CheckCollision(GameObject otherGameObject);

    // public abstract bool CheckInclude(Vector3 point);
}