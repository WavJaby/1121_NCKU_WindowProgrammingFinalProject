using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using Project.GameHelper;
using Project.lib;
using Project.Lighting;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using Monitor = Silk.NET.Windowing.Monitor;
using Thread = System.Threading.Thread;
using Window = Silk.NET.Windowing.Window;

namespace Project;

public class GameWindow {
    private readonly IWindow _window;
    private GL _gl = null!;
    private ImGuiController _guiControl = null!;
    private Scene _scene = null!;

    private const double InfoDeltaTime = 0.05;
    private double _renderInfoDelta, _updateInfoDelta;
    public static string Text = "";

    private Thread _renderThread;

    private readonly ValuesAvgCalculator
        _fpsAvg = new(10),
        _deltaTime = new(10),
        _renderTime = new(10),
        _updateTime = new(10);

    public GameWindow() {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 500);
        options.Position = (Monitor.GetMainMonitor(null).Bounds.Size - options.Size) / 2;
        options.Title = "Game";
        options.WindowBorder = WindowBorder.Fixed;
        options.ShouldSwapAutomatically = true;
        options.TransparentFramebuffer = true;

        options.VSync = true;
        options.UpdatesPerSecond = 60;
        // options.IsEventDriven = false;

        // var main = Window.Create(options);
        // main.Initialize();
        // options.SharedContext = main.GLContext;
        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Update += OnUpdate;
        _window.Move += OnWindowMove;
        // _window.Closing += OnClose;
        _window.Initialize();
        while (!_window.IsClosing) {
            _window.DoEvents();
        }
        _window.DoEvents();
        _renderThread!.Join();
        Console.WriteLine("Window exit");
        Dispose();

        _window.Reset();
        _window.Dispose();
    }

    private void OnLoad() {
        Console.WriteLine("OnLoad");
        IInputContext input = _window.CreateInput();
        _gl = GL.GetApi(_window);
        _guiControl = new ImGuiController(
            _gl,
            _window,
            input
        );

        // Setup
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.BlendFunc(GLEnum.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        _gl.Enable(EnableCap.DepthTest);
        _gl.Enable(EnableCap.Blend);
        DefaultShader.LoadDefaultShader(_gl);
        UIShader.LoadDefaultShader(_gl);

        _scene = new GameScene(_window, input, _gl);
        _scene.Start();

        // Render thread
        _renderThread = new Thread(() => {
            _window.MakeCurrent();
            _window.Run(() => {
                if (!_window.IsClosing)
                    _window.DoUpdate();
                if (_window.IsClosing)
                    return;
                _window.DoRender();
            });
            _window.ClearContext();
        });
        _window.ClearContext();
        _renderThread.Start();
    }
    
    private void OnWindowMove(Vector2D<int> pos) {
        _scene.OnWindowMove(pos);
    }
    
    private void OnUpdate(double deltaTime) {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        _scene.Update(deltaTime);
        sw.Stop();
        long renderTimeMicro = sw.ElapsedTicks / (Stopwatch.Frequency / 1000_000L);
        _updateInfoDelta += deltaTime;
        if (_updateInfoDelta > InfoDeltaTime) {
            _updateInfoDelta = 0;
            _updateTime.AddAndGet(renderTimeMicro / 1000f);
            _deltaTime.AddAndGet((float) deltaTime * 1000);
        }
        // Console.WriteLine("Update " + deltaTime);
    }

    private void OnRender(double deltaTime) {
        // Console.WriteLine("OnRender " + deltaTime);
        Stopwatch sw = new Stopwatch();
        sw.Start();

        _scene.BeforeRender_(deltaTime);
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        // Render gameObjects
        foreach (var gameObject in _scene.GameObjects)
            gameObject.Render(_scene);
        _scene.AfterRender(deltaTime);

        sw.Stop();
        long renderTimeMicro = sw.ElapsedTicks / (Stopwatch.Frequency / 1000_000L);
        RenderImGui(deltaTime, renderTimeMicro);
    }

    private void RenderImGui(double deltaTime, long renderTimeMicro) {
        _guiControl.Update((float) deltaTime);
        ImGui.Begin("Debug");
        ImGui.SetWindowPos(Vector2.Zero);
        _renderInfoDelta += deltaTime;
        if (_renderInfoDelta > InfoDeltaTime) {
            _renderInfoDelta = 0;
            _fpsAvg.AddAndGet((float) (1.0f / deltaTime));
            _renderTime.AddAndGet(renderTimeMicro / 1000f);
        }
        ImGui.Text($"Fps: {_fpsAvg.Value:F2}");
        ImGui.Text($"Delta: {_deltaTime.Value:F3}");
        ImGui.Text($"Update: {_updateTime.Value:F3}");
        ImGui.Text($"Render: {_renderTime.Value:F3}");
        ImGui.Text(Text);

        // Enable mouse if click outside
        // if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) &&
        //     !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow)) {
        //     if (!_camera.FreeCamera) {
        //         _camera.EnableFreeCamera();
        //     }
        // }
        _guiControl.Render();
    }

    private void Dispose() {
        // Console.WriteLine("OnClose");
        _window.MakeCurrent();
        _guiControl.Dispose();
        foreach (var gameObject in _scene.GameObjects) {
            gameObject.Dispose();
        }
        DefaultShader.DisposeDefaultShader(_gl);
        UIShader.DisposeDefaultShader(_gl);
    }
}