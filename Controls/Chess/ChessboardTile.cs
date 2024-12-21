using Avalonia.Controls;
using Avalonia.Media;

namespace chess_frontend.Controls.Chess;

public class ChessboardTile : Panel
{
    public readonly int row, column;
    
    public ChessboardTile(Chessboard board, int row, int column)
    {
        Width = Height = board.Size / Chessboard.ChessboardSize;
        
        this.row = row;
        this.column = column;
        Grid.SetRow(this, row);
        Grid.SetColumn(this, column);

        Background = SolidColorBrush.Parse((row + column) % 2 == 0
            ? "#ebecd0"
            : "#739552"
        );
    }
}