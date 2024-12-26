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
    public static readonly INamedPipe Pipe = INamedPipe.Create();
    
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

    private static void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        Pipe.Dispose();
    }

    private async Task OnConnectBackendBtnClicked()
    {
        LogToPanel("Attempting to fetch backend pipe...", LogType.Info);
        
        if (!await Pipe.OpenAndHandshake())
        {
            Instance!.LogToPanel(
                "Backend pipe not found! Start the backend and try again.",
                LogType.Error
            );
            return;
        }
        
        LogToPanel("Backend found. Starting session...", LogType.Info);

        if (await Pipe.WaitForMsgAsync() != INamedPipe.ReadyCommand)
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