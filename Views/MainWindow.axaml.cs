using Avalonia.Controls;
using chess_frontend.Controls;

namespace chess_frontend.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        
        var chessboard = this.FindControl<Chessboard>("Chessboard");
        // if (chessboard is not null)
        // {
        //     InitializeChessboard(chessboard);
        // }
    }
}