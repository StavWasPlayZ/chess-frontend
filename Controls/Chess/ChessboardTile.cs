using System;
using Avalonia.Controls;
using Avalonia.Input;
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
    
    public ChessPiece? ContainedChessPiece
    {
        get => (ChessPiece?) Children[0];
        set
        {
            Children.Clear();

            if (value != null)
            {
                Children.Add(value);
                value.PointerPressed += OnChessPiecePressed;
            }
        }
    }

    private void OnChessPiecePressed(object? sender, PointerPressedEventArgs e)
    {
        var piece = ContainedChessPiece!;
        piece.PointerPressed -= OnChessPiecePressed;
        
        Children.Remove(piece);
        piece.StartFollowingPointer(e);
    }

    public Chessboard GetChessboard() => _chessboard;
}