using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using chess_frontend.Util.Comm;

namespace chess_frontend.Views;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }
    public static readonly INamedPipe Pipe = new NamedPipeLinuxImpl();
    
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();

        MainChessboard.OverlayCanvas = OverlayCanvas;
        MainChessboard.AttachedToVisualTree += (_, _) => MainChessboard.PopulateBoard();
        
        ConnectBackendBtn.Click += async (_, _) => await OnConnectBackendBtnClicked();
        
        SizeChanged += OnSizeChanged;
        Closing += OnClosing;
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        _ = Pipe.SendMsgAsync("ext");
    }

    private async Task OnConnectBackendBtnClicked()
    {
        if (!Pipe.Exists)
        {
            LogToPanel("Backend pipe not found! Start the backend and try again.", LogType.Error);
            return;
        }
        
        LogToPanel("Backend found. Starting session...", LogType.Info);
        await Pipe.SendMsgAsync("rdy");

        if (await Pipe.WaitForMsgAsync() != "rdy")
        {
            LogToPanel("Frontend connection failed: Unexpected message received", LogType.Error);
            return;
        }
        
        LogToPanel("Successfully connected to backend", LogType.Success);
        MainChessboard.OnBackendConnected();
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.HeightChanged)
        {
            MainChessboard.Size = e.NewSize.Height - 2 * MainChessboard.Margin.Left;
        }
    }

    public void LogToPanel(string text, LogType logType, IBrush? brush = null)
    {
        brush ??= logType switch
        {
            LogType.Error => Brushes.Red,
            LogType.Warning => Brushes.Orange,
            LogType.Info => Brushes.LightGray,
            LogType.Success => Brushes.Green,
            _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
        };

        LogOutputStack.Children.Add(new TextBlock
        {
            Text = text,
            FontSize = 13,
            Foreground = brush,
            Margin = new Thickness(0, 5)
        });

        LogScroller.ScrollToEnd();
    }
}

public enum LogType
{
    Info,
    Warning,
    Error,
    Success
}