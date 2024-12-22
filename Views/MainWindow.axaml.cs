using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace chess_frontend.Views;

public partial class MainWindow : Window
{
    // public readonly Canvas OverlayCanvas;
    public static MainWindow? Instance { get; private set; }
    
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();

        MainChessboard.OverlayCanvas = OverlayCanvas;
        MainChessboard.AttachedToVisualTree += (_, _) => MainChessboard.PopulateBoard();
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