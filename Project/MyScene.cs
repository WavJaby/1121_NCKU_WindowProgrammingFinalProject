using System.Numerics;
using Project.Collision;
using Project.defaultObjects;
using Project.GameHelper;
using Project.lib;
using Project.Lighting;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Color = System.Drawing.Color;
using Plane = Project.defaultObjects.Plane;
using RectangleF = System.Drawing.RectangleF;
using Texture = Project.lib.Texture;

namespace Project;

public class MyScene : Scene {
    private const int MaxPointLights = 100;
    private const int PixelPerUnit = 50;
    private readonly GL _gl;
    private readonly List<Vector3> _pointLightSources = new();
    private readonly Dictionary<string, Texture> _textures = new();

    // Game settings
    private const int WindowMinWidth = 300;

    private const int WindowMinHeight = 300;

    private const float WindowShrinkRate = 30;
    // private const float WindowShrinkRate = 0;

    private const float BulletSpeed = 10;

    private const float BulletHitBorderPower = 300;
    // private const float BulletHitBorderPower = 0;

    private RectangleF _windowRect;
    private float _windowLeftVelocity, _windowRightVelocity, _windowTopVelocity, _windowBottomVelocity;

    private readonly Group _bullets = new();
    private Plane _basePlane = null!;
    private GameObject _player = null!;
    private GameObject _enemy = null!;


    public MyScene(IWindow window, IInputContext input, GL gl) : base(window, input) {
        _gl = gl;
        // Camera.SetDirection(-90, -89.9f);
        Camera.Position = new Vector3(0, 30, 0);
        Camera.SetDirection(-90, -89.999f);
        // Camera.EnableFreeCamera();
        _windowRect = new RectangleF(window.Position.X, window.Position.Y, window.Size.X, window.Size.Y);

        if (Camera.PrimaryKeyboard != null)
            Camera.PrimaryKeyboard.KeyDown += KeyDown;
    }

    public override void Start() {
        _textures.Add("texture1", new Texture(_gl, "resources/image.jpg"));
        _textures.Add("chess_board", new Texture(_gl, "resources/chess_board.png"));
        _textures.Add("silk", new Texture(_gl, "resources/silk.png"));
        _textures.Add("silkSpecular", new Texture(_gl, "resources/silkSpecular.png"));
        _textures.Add("cube", new Texture(_gl, "models/cube.jpg"));

        UpdateCameraPosition();

        _basePlane = new Plane(_gl, new Material(Color.FromArgb(0,200,200, 200), 50f, 0b10));
        _basePlane.Rotation = new Vector3(MathHelper.DegToRad(90), 0, 0);
        Vector2D<int> monitorSize = Window.Monitor!.Bounds.Size;
        _basePlane.Scale = new Vector3(monitorSize.X / PixelPerUnit, monitorSize.Y / PixelPerUnit, 1);
        _basePlane.Position = new Vector3(_basePlane.Scale.X / 2, 0, _basePlane.Scale.Y / 2);
        GameObjects.Add(_basePlane);

        _player = new Sphere(_gl, new Material(_textures["texture1"], null, 32f), 20, 20);
        // _player = new Cube(_gl, new Material(_textures["texture1"], null, 32f));
        _player.Position = Camera.Position with {Y = 0};
        _player.Friction = 1;
        _player.Collider = new SphereCollider(_player.Scale / 2);
        // _player.Collider = new BoxCollider(_player.Scale);
        GameObjects.Add(_player);

        var model = new ModelObject(_gl, "models/cube.obj", _textures["silk"], _textures["silkSpecular"]);
        model.Position = new Vector3(10, 0.5f, 10);
        // model.Position = new Vector3(_player.Position.X-5, 0.5f, _player.Position.Z-5);
        GameObjects.Add(model);

        _enemy = new Sphere(_gl, new Material(Color.Aquamarine, 32f), 30, 20);
        // _enemy = new Cube(_gl, new Material(Color.Aquamarine, 32f));
        _enemy.Rotation = Vector3.UnitY * MathHelper.DegToRad(45f) + Vector3.UnitX * MathHelper.DegToRad(0f);
        _enemy.Scale = new Vector3(0.8f);
        // _enemy.Position = new Vector3((int) _player.Position.X, 0, (int) _player.Position.Z);
        _enemy.Position = _player.Position - Vector3.UnitZ * 1.5f + Vector3.UnitX * 0.8f;
        _enemy.Velocity = new Vector3(0, 0, 0.2f);
        // _enemy.Scale = new Vector3(0.5f);
        _enemy.Collider = new SphereCollider(_enemy.Scale / 2);
        // _enemy.Collider = new BoxCollider(_enemy.Scale);
        GameObjects.Add(_enemy);

        // GameObjects.Add(new Line(_gl, _player.Position, _player.Position + Vector3.UnitY * 1 + Vector3.UnitX * 5));

        GameObjects.Add(_bullets);
    }

    private float count;

    public override void OnWindowMove(Vector2D<int> position) {
        if (position.X != (int) _windowRect.X || position.Y != (int) _windowRect.Y)
            Window.Position = new Vector2D<int>((int) _windowRect.X, (int) _windowRect.Y);
    }

    public override void BeforeRender(double deltaTime) {
        Vector2 windowOffsetInWorldPos = new Vector2(
            _windowRect.Location.X / PixelPerUnit,
            _windowRect.Location.Y / PixelPerUnit);
        Vector2 windowSizeInWorldSize = new Vector2(
            _windowRect.Size.Width / PixelPerUnit,
            _windowRect.Size.Height / PixelPerUnit);

        Camera.Offset = (new Vector2(_player.Position.X, _player.Position.Z) - windowOffsetInWorldPos) / windowSizeInWorldSize - new Vector2(0.5f);
        UpdateWindowBound(deltaTime);

        // Calculate collision
        Vector3 playerDir = (_player.Position - _enemy.Position) with {Y = 0};
        _enemy.Velocity += Vector3.Normalize(playerDir) * 10f * (float) deltaTime;
        _enemy.Friction = 1f;
        bool enemyCollision = _player.Collider!.CheckCollision(_enemy);
        if (enemyCollision) {
            _enemy.Position = _player.Position - Vector3.Normalize(playerDir) *
                (((SphereCollider) _player.Collider).Radius + ((SphereCollider) _enemy.Collider!).Radius);
            // _enemy.Velocity -= Vector3.Normalize(playerDir) * 200 * (float) deltaTime;
        }
        // GameWindow.Text = enemyCollision.ToString();
        // GameWindow.Text = Camera.Offset.ToString();

        // Calculate force
        foreach (var gameObject in GameObjects) {
            if (gameObject is Group group) {
                foreach (var child in group.GameObjects)
                    child.UpdateVelocity(deltaTime);
            }
            if (gameObject is GameObject worldGameObject)
                worldGameObject.UpdateVelocity(deltaTime);
        }
    }
    
    // Game update
    public override void Update(double deltaTime) {
        Camera.MainUpdate(deltaTime);

        Vector2 mousePos = Camera.Mouse.Position;
        Vector3 mousePosInWorldPos = new Vector3(
            (_windowRect.X + mousePos.X) / PixelPerUnit, 0,
            (_windowRect.Y + mousePos.Y) / PixelPerUnit);
        Vector3 windowOffsetInWorldPos = new Vector3(
            _windowRect.Location.X / PixelPerUnit, 0,
            _windowRect.Location.Y / PixelPerUnit);
        Vector2 windowSizeInWorldSize = new Vector2(
            _windowRect.Size.Width / PixelPerUnit,
            _windowRect.Size.Height / PixelPerUnit);

        // float scale = _basePlane.Scale.X / 4;
        // _basePlane.Position = new Vector3(
        //     (int) (_player.Position.X / scale) * scale, 0,
        //     (int) (_player.Position.Z / scale) * scale);

        count += (float) deltaTime;
        if (count > 0.5f) {
            count = 0;
            var bullet = new Sphere(_gl, new Material(_textures["cube"], null, 32f), 10, 10);
            // cube.Rotation = Vector3.UnitX * (Random.Shared.NextSingle() * 2 * (float) Math.PI) +
            //                 Vector3.UnitY * (Random.Shared.NextSingle() * 2 * (float) Math.PI) +
            //                 Vector3.UnitZ * (Random.Shared.NextSingle() * 2 * (float) Math.PI);
            bullet.Scale = new Vector3(0.5f);
            bullet.Position = _player.Position with {Y = 0.25f};
            bullet.Friction = 0;
            bullet.Velocity = Vector3.Normalize(mousePosInWorldPos - bullet.Position with {Y = 0}) * BulletSpeed;
            _bullets.AddGameObject(bullet);
        }
        List<int> removeList = new();
        int index = 0;
        _pointLightSources.Clear();
        foreach (var bullet in _bullets.GameObjects) {
            bool deleteBullet;
            float influenceScope = 0.1f;
            float bulletRadius = bullet.Scale.Z / 2;
            Vector3 topLeftOffset = bullet.Position - windowOffsetInWorldPos;
            // If bullet hit wall
            if (topLeftOffset.X - bulletRadius < 0 || topLeftOffset.X + bulletRadius > windowSizeInWorldSize.X ||
                topLeftOffset.Z - bulletRadius < 0 || topLeftOffset.Z + bulletRadius > windowSizeInWorldSize.Y) {
                deleteBullet = true;
                Vector3 velocity = Vector3.Normalize(bullet.Velocity);
                if (velocity.X < 0 && topLeftOffset.X < windowSizeInWorldSize.X * influenceScope)
                    _windowLeftVelocity = velocity.X * BulletHitBorderPower;
                if (velocity.X > 0 && topLeftOffset.X > windowSizeInWorldSize.X * (1 - influenceScope))
                    _windowRightVelocity = velocity.X * BulletHitBorderPower;
                if (velocity.Z < 0 && topLeftOffset.Z < windowSizeInWorldSize.Y * influenceScope)
                    _windowTopVelocity = velocity.Z * BulletHitBorderPower;
                if (velocity.Z > 0 && topLeftOffset.Z > windowSizeInWorldSize.Y * (1 - influenceScope))
                    _windowBottomVelocity = velocity.Z * BulletHitBorderPower;
            } else
                deleteBullet = false;

            if (deleteBullet) {
                removeList.Add(index++);
            } else {
                _pointLightSources.Add(bullet.Position);
                ++index;
            }
        }
        _bullets.RemoveGameObjectAt(removeList);

        if (Camera.PrimaryKeyboard != null) {
            IKeyboard keyboard = Camera.PrimaryKeyboard;
            float speed = (float) (10 * deltaTime);
            if (keyboard.IsKeyPressed(Key.W))
                _player.Velocity -= Vector3.UnitZ * speed;
            else if (keyboard.IsKeyPressed(Key.S))
                _player.Velocity += Vector3.UnitZ * speed;
            if (keyboard.IsKeyPressed(Key.D))
                _player.Velocity += Vector3.UnitX * speed;
            else if (keyboard.IsKeyPressed(Key.A))
                _player.Velocity -= Vector3.UnitX * speed;
        }
        _player.Rotation += (Vector3.UnitX * 1f + Vector3.UnitY * 1f) * (float) deltaTime;

        // Calculate collision
        if (_player.Position.X - _player.Scale.X / 2 < windowOffsetInWorldPos.X) {
            _player.Position = _player.Position with {X = windowOffsetInWorldPos.X + _player.Scale.X / 2};
            if (_player.Velocity.X < 0) _player.Velocity = _player.Velocity with {X = 0};
        }
        if (_player.Position.X + _player.Scale.X / 2 > windowOffsetInWorldPos.X + windowSizeInWorldSize.X) {
            _player.Position = _player.Position with {X = windowOffsetInWorldPos.X + windowSizeInWorldSize.X - _player.Scale.X / 2};
            if (_player.Velocity.X > 0) _player.Velocity = _player.Velocity with {X = 0};
        }
        if (_player.Position.Z - _player.Scale.Z / 2 < windowOffsetInWorldPos.Z) {
            _player.Position = _player.Position with {Z = windowOffsetInWorldPos.Z + _player.Scale.Z / 2};
            if (_player.Velocity.Z < 0) _player.Velocity = _player.Velocity with {Z = 0};
        }
        if (_player.Position.Z + _player.Scale.Z / 2 > windowOffsetInWorldPos.Z + windowSizeInWorldSize.Y) {
            _player.Position = _player.Position with {Z = windowOffsetInWorldPos.Z + windowSizeInWorldSize.Y - _player.Scale.Z / 2};
            if (_player.Velocity.Z > 0) _player.Velocity = _player.Velocity with {Z = 0};
        }
    }

    private void UpdateWindowBound(double deltaTime) {
        bool change = false;

        // Expand
        if (_windowLeftVelocity != 0 || _windowRightVelocity != 0 || _windowTopVelocity != 0 || _windowBottomVelocity != 0) {
            const float friction = 5;
            const float stop = 4;
            change = true;

            // Left
            float leftChange = (float) (_windowLeftVelocity * deltaTime);
            float leftFrictionForce = (float) (friction * _windowLeftVelocity * deltaTime);
            _windowLeftVelocity -= leftFrictionForce;
            _windowRect.X += leftChange;
            _windowRect.Width -= leftChange;
            if (MathF.Abs(_windowLeftVelocity) < MathF.Abs(leftFrictionForce) + stop) _windowLeftVelocity = 0;
            // Right
            float rightChange = (float) (_windowRightVelocity * deltaTime);
            float rightFrictionForce = (float) (friction * _windowRightVelocity * deltaTime);
            _windowRightVelocity -= rightFrictionForce;
            _windowRect.Width += rightChange;
            if (MathF.Abs(_windowRightVelocity) < MathF.Abs(rightFrictionForce) + stop) _windowRightVelocity = 0;
            // Top
            float topChange = (float) (_windowTopVelocity * deltaTime);
            float topFrictionForce = (float) (friction * _windowTopVelocity * deltaTime);
            _windowTopVelocity -= topFrictionForce;
            _windowRect.Y += topChange;
            _windowRect.Height -= topChange;
            if (MathF.Abs(_windowTopVelocity) < MathF.Abs(topFrictionForce) + stop) _windowTopVelocity = 0;
            // Bottom
            float bottomChange = (float) (_windowBottomVelocity * deltaTime);
            float bottomFrictionForce = (float) (friction * _windowBottomVelocity * deltaTime);
            _windowBottomVelocity -= bottomFrictionForce;
            _windowRect.Height += bottomChange;
            if (MathF.Abs(_windowBottomVelocity) < MathF.Abs(bottomFrictionForce) + stop) _windowBottomVelocity = 0;
        }

        // Shrink
        if (_windowRect.Width > WindowMinWidth) {
            if (_windowRightVelocity == 0) {
                _windowRect.Width -= WindowShrinkRate * (float) deltaTime;
                change = true;
            }
            if (_windowLeftVelocity == 0) {
                _windowRect.X += WindowShrinkRate * (float) deltaTime;
                _windowRect.Width -= WindowShrinkRate * (float) deltaTime;
                change = true;
            }
            if (_windowRect.Width < WindowMinWidth) {
                _windowRect.Width = WindowMinWidth;
            }
        }
        if (_windowRect.Height > WindowMinHeight) {
            if (_windowBottomVelocity == 0) {
                _windowRect.Height -= WindowShrinkRate * (float) deltaTime;
                change = true;
            }
            if (_windowTopVelocity == 0) {
                _windowRect.Y += WindowShrinkRate * (float) deltaTime;
                _windowRect.Height -= WindowShrinkRate * (float) deltaTime;
                change = true;
            }
            if (_windowRect.Height < WindowMinHeight) {
                _windowRect.Height = WindowMinHeight;
            }
        }

        if (change) {
            Window.Position = new Vector2D<int>((int) _windowRect.X, (int) _windowRect.Y);
            Window.Size = new Vector2D<int>((int) _windowRect.Width, (int) _windowRect.Height);
            _gl.Viewport(Window.Size);
            Camera.UpdateAspectRatio(Window.Size);
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition() {
        if (Camera.FreeCamera)
            return;

        Vector2D<int> windowSize = Window.Size;
        Vector3 cameraPosition = Camera.Position;
        Camera.Fov = MathHelper.RadToDeg(Math.Atan(((double) windowSize.Y / 2 / PixelPerUnit) / cameraPosition.Y)) * 2;
        double baseSize = cameraPosition.Y * Math.Tan(MathHelper.DegToRad(Camera.Fov / 2)) * 2;
        double w = baseSize * Camera.AspectRatio;
        double h = w / Camera.AspectRatio;
        double offsetX = Window.Position.X * (w / windowSize.X);
        double offsetZ = Window.Position.Y * (h / windowSize.Y);
        Camera.Position = new Vector3(
            (float) (w / 2 + offsetX),
            cameraPosition.Y,
            (float) (h / 2 + offsetZ));
    }

    public override void ApplyCameraAndLighting(DefaultShader shader) {
        // Setup the coordinate systems for our view
        shader.SetViewport(Camera);

        // Add light sources
        var pointLightColor = new Vector4(0, 1, 1, 1);
        int i = 0;
        foreach (var pointLight in _pointLightSources) {
            string pointPos = "pointLights[" + i + "]";
            shader.SetUniform(pointPos + ".localPos", pointLight);
            shader.SetUniform(pointPos + ".base.color", pointLightColor);
            shader.SetUniform(pointPos + ".base.ambientIntensity", 0.0001f);
            shader.SetUniform(pointPos + ".base.diffuseIntensity", 1.0f);
            shader.SetUniform(pointPos + ".atten.constant", 0.0f);
            shader.SetUniform(pointPos + ".atten.linear", 0.0f);
            shader.SetUniform(pointPos + ".atten.exp", 0.4f);
            if (++i >= MaxPointLights)
                break;
        }
        shader.SetUniform("pointLightsLength", i);
        // Sun light
        var dirLightColor = new Vector4(1.0f);
        var dirLightDirection = new Vector3(0, MathHelper.DegToRad(-90), 0);
        shader.SetSun(dirLightDirection, dirLightColor, 0.02f, 0.1f);
    }

    private void KeyDown(IKeyboard keyboard, Key key, int arg3) {
        if (key == Key.Escape && !Camera.FreeCamera)
            Window.Close();
    }
}