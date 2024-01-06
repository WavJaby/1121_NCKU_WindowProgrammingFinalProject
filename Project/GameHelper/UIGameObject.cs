using System.Numerics;

namespace Project.GameHelper;

public abstract class UIGameObject : BaseGameObject {
    public Vector3 Position;
    public Vector3 Scale = new(1);
}