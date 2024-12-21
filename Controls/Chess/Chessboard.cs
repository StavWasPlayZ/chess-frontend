using Avalonia.Controls;
using Avalonia.Media;

namespace chess_frontend.Controls.Chess;

public class Chessboard : Grid
{
    public const int ChessboardSize = 8;

    public double Size
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public Chessboard()
    {
        AttachedToVisualTree += (sender, args) => InitializeChessboard();
    }

    private void InitializeChessboard()
    {
        // Apply row/column definitions
        RowDefinitions = [];
        ColumnDefinitions = [];
        
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });                
            }
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }
        
        // Add chess tiles
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {   
                Children.Add(new ChessboardTile(this, i, j));
            }
        }
    }
}