using Avalonia.Controls;
using chess_frontend.Controls;
using chess_frontend.Controls.Chess;

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

        // this.OverlayCanvas = this.FindControl<Chessboard>("OverlayCanvas");
        // OverlayCanvas
        // if (chessboard is not null)
        // {
        //     InitializeChessboard(chessboard);
        // }
    }
}