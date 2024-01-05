using System.Numerics;
using Silk.NET.Input;

namespace Project;

public class MouseControl {
    private readonly IReadOnlyList<IMouse> _mice;
    private CursorMode _cursorMode;

    public CursorMode CursorMode {
        get => _cursorMode;
        set {
            _cursorMode = value;
            foreach (var mouse in _mice) {
                mouse.Cursor.CursorMode = _cursorMode;
            }
        }
    }

    public Vector2 Position => _mice.Count == 0 ? Vector2.Zero : _mice[0].Position;

    /// <summary>Called when a mouse button is pressed down.</summary>
    public event Action<IMouse, MouseButton>? MouseDown;

    /// <summary>Called when a mouse button is released.</summary>
    public event Action<IMouse, MouseButton>? MouseUp;

    /// <summary>Called when a single click is performed.</summary>
    public event Action<IMouse, MouseButton, Vector2>? Click;

    /// <summary>Called when a double click is performed.</summary>
    public event Action<IMouse, MouseButton, Vector2>? DoubleClick;

    /// <summary>Called when the mouse is moved.</summary>
    public event Action<IMouse, Vector2>? MouseMove;

    /// <summary>Called when the mouse wheel scrolls.</summary>
    public event Action<IMouse, ScrollWheel>? MouseWheel;

    public MouseControl(IInputContext input) {
        _mice = input.Mice;
        foreach (var mouse in _mice) {
            mouse.MouseDown += OnMouseDown;
            mouse.MouseUp += OnMouseUp;
            mouse.Click += OnClick;
            mouse.DoubleClick += OnDoubleClick;
            mouse.MouseMove += OnMouseMove;
            mouse.Scroll += OnScroll;
        }
    }


    public bool IsButtonPressed(MouseButton btn) {
        foreach (var mouse in _mice) {
            if (mouse.IsButtonPressed(btn))
                return true;
        }
        return false;
    }

    private void OnMouseDown(IMouse mouse, MouseButton mouseButton) =>
        MouseDown?.Invoke(mouse, mouseButton);

    private void OnMouseUp(IMouse mouse, MouseButton mouseButton) =>
        MouseUp?.Invoke(mouse, mouseButton);

    private void OnClick(IMouse mouse, MouseButton mouseButton, Vector2 position) =>
        Click?.Invoke(mouse, mouseButton, position);

    private void OnDoubleClick(IMouse mouse, MouseButton mouseButton, Vector2 position) =>
        DoubleClick?.Invoke(mouse, mouseButton, position);

    private void OnMouseMove(IMouse mouse, Vector2 position) =>
        MouseMove?.Invoke(mouse, position);

    private void OnScroll(IMouse mouse, ScrollWheel scrollWheel) =>
        MouseWheel?.Invoke(mouse, scrollWheel);
}