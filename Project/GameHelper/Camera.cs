using System.Numerics;
using Project.lib;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Project.GameHelper;

public class Camera {
    private const float NearPlaneDistance = 0.1f, FarPlaneDistance = 1000.0f;
    public readonly Vector3 Up;
    public Vector3 Front { get; private set; }
    private float _aspectRatio;
    public float AspectRatio => _aspectRatio;
    public Vector2 Offset;
    private Vector3 _position;

    public Vector3 Position {
        get => _position;
        set {
            _position = value;
            UpdateViewMatrix();
        }
    }

    private float _fov;

    public float Fov {
        get => _fov;
        set {
            _fov = value;
            UpdateProjectionMatrix();
        }
    }

    private float _yaw, _pitch;


    //Used to track change in mouse movement to allow for moving of the Camera
    private Vector2 _lastMousePosition;
    private bool _freeCamera;
    public bool FreeCamera => _freeCamera;
    private readonly IKeyboard? _primaryKeyboard;
    public IKeyboard? PrimaryKeyboard => _primaryKeyboard;
    public readonly MouseControl Mouse;
    public Matrix4x4 ViewMatrix { get; private set; }
    public Matrix4x4 ProjectionMatrix { get; private set; }

    public Camera(Vector3 up, IWindow window, IInputContext input) {
        _aspectRatio = (float) window.Size.X / window.Size.Y;
        Up = up;
        _yaw = 0;
        _fov = 45f;
        UpdateViewMatrix();
        UpdateProjectionMatrix();
        // User input

        _primaryKeyboard = input.Keyboards.FirstOrDefault();
        if (_primaryKeyboard != null)
            _primaryKeyboard.KeyUp += KeyUp;
        Mouse = new MouseControl(input);
    }

    public void ChangeZoom(float zoomAmount) {
        //We don't want to be able to zoom in too close or too far away so clamp to these values
        _fov = Math.Clamp(_fov - zoomAmount, 1.0f, 45f);
        UpdateProjectionMatrix();
    }

    public void SetDirection(float yaw, float pitch) {
        _yaw = yaw;
        _pitch = pitch;

        UpdateViewMatrix();
    }

    public void ModifyDirection(float yaw, float pitch) {
        SetDirection(_yaw + yaw, Math.Clamp(_pitch - pitch, -89.999f, 89.999f));
    }

    public void UpdateAspectRatio(Vector2D<int> windowSize) {
        _aspectRatio = (float) windowSize.X / windowSize.Y;
        UpdateProjectionMatrix();
    }

    private void UpdateViewMatrix() {
        var cameraDirection = Vector3.Zero;
        cameraDirection.X = MathF.Cos(MathHelper.DegToRad(_yaw)) * MathF.Cos(MathHelper.DegToRad(_pitch));
        cameraDirection.Y = MathF.Sin(MathHelper.DegToRad(_pitch));
        cameraDirection.Z = MathF.Sin(MathHelper.DegToRad(_yaw)) * MathF.Cos(MathHelper.DegToRad(_pitch));

        Front = Vector3.Normalize(cameraDirection);
        ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + Front, Up);

        ViewMatrix = ViewMatrix with {
            M21 = -Offset.X,
            M22 = Offset.Y
        };
    }

    private void UpdateProjectionMatrix() {
        ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegToRad(_fov), _aspectRatio, NearPlaneDistance, FarPlaneDistance);
    }

    public void MainUpdate(double deltaTime) {
        // User Controls
        if (_primaryKeyboard == null || !_freeCamera)
            return;
        var moveSpeed = (_primaryKeyboard.IsKeyPressed(Key.ShiftLeft) ? 20 : 4) * (float) deltaTime;

        if (_primaryKeyboard.IsKeyPressed(Key.W))
            Position += moveSpeed * Front;
        if (_primaryKeyboard.IsKeyPressed(Key.S))
            Position -= moveSpeed * Front;
        if (_primaryKeyboard.IsKeyPressed(Key.A))
            Position -= Vector3.Normalize(Vector3.Cross(Front, Up)) * moveSpeed;
        if (_primaryKeyboard.IsKeyPressed(Key.D))
            Position += Vector3.Normalize(Vector3.Cross(Front, Up)) * moveSpeed;
        if (_primaryKeyboard.IsKeyPressed(Key.Space))
            Position += Up * moveSpeed;
        if (_primaryKeyboard.IsKeyPressed(Key.ControlLeft))
            Position -= Up * moveSpeed;
    }

    public void EnableFreeCamera() {
        _freeCamera = true;
        _lastMousePosition = default;
        // Hide mouse
        Mouse.CursorMode = CursorMode.Raw;
        Mouse.MouseMove += OnMouseMove;
        Mouse.MouseWheel += OnMouseWheel;
    }

    private void DisableFreeCamera() {
        _freeCamera = false;
        // Reset mouse
        Mouse.CursorMode = CursorMode.Normal;
        Mouse.MouseMove -= OnMouseMove;
        Mouse.MouseWheel -= OnMouseWheel;
    }

    private void OnMouseMove(IMouse mouse, Vector2 position) {
        if (_freeCamera) {
            var lookSensitivity = 0.1f;
            if (_lastMousePosition == default) {
                _lastMousePosition = position;
            } else {
                var xOffset = (position.X - _lastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - _lastMousePosition.Y) * lookSensitivity;
                _lastMousePosition = position;

                ModifyDirection(xOffset, yOffset);
            }
        }
    }

    private void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel) {
        if (_freeCamera)
            ChangeZoom(scrollWheel.Y);
    }

    private void KeyUp(IKeyboard keyboard, Key key, int arg3) {
        if (key == Key.Escape) {
            if (_freeCamera)
                DisableFreeCamera();
        }
    }
}