using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace chess_frontend.Controls.Chess;

public class ChessboardTile : Panel
{
    private readonly Chessboard _chessboard;
    public readonly int Row, Column;
    
    public ChessboardTile(Chessboard board, int row, int column)
    {
        _chessboard = board;
        Width = Height = board.Size / Chessboard.ChessboardSize;
        
        Row = row;
        Column = column;
        Grid.SetRow(this, row);
        Grid.SetColumn(this, column);

        Background = SolidColorBrush.Parse((row + column) % 2 == 0
            ? "#ebecd0"
            : "#739552"
        );
    }
    
    public ChessPiece? ChessPiece
    {
        get => (ChessPiece?) Children[0];
        set
        {
            Children.Clear();

            if (value != null)
            {
                Children.Add(value);
            }
        }
    }
    
    public Chessboard GetChessboard() => _chessboard;
}