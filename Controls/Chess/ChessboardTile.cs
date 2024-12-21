using Avalonia.Controls;
using Avalonia.Media;

namespace chess_frontend.Controls.Chess;

public class ChessboardTile : Panel
{
    private static readonly SolidColorBrush
        DefColorLight = SolidColorBrush.Parse("#ebecd0"),
        DefColorDark = SolidColorBrush.Parse("#739552"),
            
        MoveColorLight = SolidColorBrush.Parse("#f5f682"),
        MoveColorDark = SolidColorBrush.Parse("#b9ca43")
    ;
    
    private readonly Chessboard _chessboard;
    public readonly int Row, Column;

    private readonly SolidColorBrush _defColor;
    private readonly SolidColorBrush _moveColor;
    
    public ChessboardTile(Chessboard board, int row, int column)
    {
        _chessboard = board;
        Width = Height = board.Size / Chessboard.ChessboardSize;
        
        Row = row;
        Column = column;
        Grid.SetRow(this, row);
        Grid.SetColumn(this, column);

        if ((row + column) % 2 == 0)
        {
            _defColor = DefColorLight;
            _moveColor = MoveColorLight;
        }
        else
        {
            _defColor = DefColorDark;
            _moveColor = MoveColorDark;
        }

        BackgroundToDefault();
    }

    public void BackgroundToDefault()
    {
        Background = _defColor;
    }
    public void BackgroundToMoved()
    {
        Background = _moveColor;
    }
    
    
    private ChessPiece? _containedChessPiece;
    
    public ChessPiece? ContainedChessPiece
    {
        get => _containedChessPiece;
        set
        {
            Children.Clear();
            if (value == null)
            {
                if (_containedChessPiece != null)
                {
                    Children.Remove(_containedChessPiece);
                    _containedChessPiece = null;
                }
                return;
            }
            
            Children.Add(value);
            value.ParentTile = this;
        }
    }

    public Chessboard GetChessboard() => _chessboard;
}