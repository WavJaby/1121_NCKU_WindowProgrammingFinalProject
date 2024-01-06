using System.Numerics;
using Project.defaultObjects;

namespace Project.GameHelper;

public abstract class GameObject : BaseGameObject {
    internal Matrix4x4 TransformMatrix;
    private Vector3 _position, _rotation, _scale = new(1);
    private Collider? _collider;

    public Group? Parent;
    public Vector3 Velocity;
    public float Friction;

    public Vector3 Position {
        get => _position;
        set {
            _position = value;
            UpdateMatrix();
        }
    }

    public Vector3 Rotation {
        get => _rotation;
        set {
            _rotation = value;
            UpdateMatrix();
        }
    }

    public Vector3 Scale {
        get => _scale;
        set {
            _scale = value;
            UpdateMatrix();
        }
    }

    public Collider? Collider {
        get => _collider;
        set {
            _collider = value;
            if (_collider != null)
                _collider.ForGameObject = this;
        }
    }

    protected GameObject() {
        UpdateMatrix();
    }

    private void UpdateMatrix() {
        TransformMatrix = Matrix4x4.CreateScale(_scale) *
                          Matrix4x4.CreateFromYawPitchRoll(_rotation.Y, _rotation.X, _rotation.Z) *
                          Matrix4x4.CreateTranslation(_position);
    }

    public void UpdateVelocity(double deltaTime) {
        if (Velocity is {X: 0, Y: 0, Z: 0})
            return;

        Position += Velocity * (float) deltaTime;
        if (Friction == 0)
            return;
        Vector3 frictionForce = Friction * Velocity * (float) deltaTime;
        Velocity -= frictionForce;
        if (MathF.Abs(Velocity.X) < MathF.Abs(frictionForce.X)) Velocity.X = 0;
        if (MathF.Abs(Velocity.Y) < MathF.Abs(frictionForce.Y)) Velocity.Y = 0;
        if (MathF.Abs(Velocity.Z) < MathF.Abs(frictionForce.Z)) Velocity.Z = 0;
    }
}