using Avalonia.Controls;
using Avalonia.Media;

namespace chess_frontend.Controls;

public class Chessboard : Grid
{
    private const int ChessboardSize = 8;

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
        
        // Add panels
        var size = Size / ChessboardSize;
        
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
                SetRow(panel, i);
                SetColumn(panel, j);
                
                Children.Add(panel);
            }
        }
    }
}