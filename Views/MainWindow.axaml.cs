using Avalonia.Controls;
using Avalonia.Media;

namespace chess_frontend.Views;

public partial class MainWindow : Window
{
    private const int ChessboardSize = 8;
    
    public MainWindow()
    {
        InitializeComponent();
        
        var chessboard = this.FindControl<Grid>("Chessboard");
        if (chessboard is not null)
        {
            InitializeChessboard(chessboard);
        }
    }

    private static void InitializeChessboard(Grid grid)
    {        
        // Apply row/column definitions
        grid.RowDefinitions = [];
        grid.ColumnDefinitions = [];
        
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });                
            }
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }
        
        // Add panels
        var size = grid.Width / ChessboardSize;
        
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                var panel = new Panel
                {
                    Background = SolidColorBrush.Parse((i + j) % 2 == 0
                        ? "#ebecd0"
                        : "#739552"
                    ),
                    Width = size,
                    Height = size
                };
                Grid.SetRow(panel, i);
                Grid.SetColumn(panel, j);
                
                grid.Children.Add(panel);
            }
        }
    }
}