using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using chess_frontend.Controls.Chess.Piece;
using chess_frontend.Util;

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

    public ChessPoint Position { get; }

    private readonly SolidColorBrush _defColor;
    private readonly SolidColorBrush _moveColor;
    
    public ChessboardTile(Chessboard board, ChessPoint position)
    {
        Position = position;
        _chessboard = board;
        Width = Height = board.TileSize;
        
        Grid.SetRow(this, Position.Y);
        Grid.SetColumn(this, Position.X);

        if ((Position.X + Position.Y) % 2 == 0)
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
        
        // Letter & numbers on sides
        var textColor = _defColor == DefColorDark ? DefColorLight : DefColorDark;
        
        if (Position.Y == Chessboard.ChessboardSize - 1)
        {
            Children.Add(new TextBlock
            {
                Text = ((char)(Position.X + 'a')).ToString(),
                    
                FontSize = 18,
                Foreground = textColor,
                FontWeight = FontWeight.Medium,
                
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
            });
        }
        if (Position.X == 0)
        {
            Children.Add(new TextBlock
            {
                Text = (Chessboard.ChessboardSize - Position.Y).ToString(),
                    
                FontSize = 18,
                Foreground = textColor,
                FontWeight = FontWeight.Medium,
                
                Margin = new Thickness(5)
            });
        }
        
        GetChessboard().SizeChanged += OnChessboardSizeChanged;
    }
    
    
    private void OnChessboardSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        this.SetSize(GetChessboard().TileSize);
        ContainedChessPiece?.SetSize(GetChessboard().TileSize);
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
            if (_containedChessPiece != null)
            {
                Children.Remove(_containedChessPiece);
                _containedChessPiece.ParentTile = null;
            }

            if (value == null)
            {
                _containedChessPiece = null;
                return;
            }
            
            Children.Add(value);
            value.ParentTile = this;
            _containedChessPiece = value;
        }
    }

    public Chessboard GetChessboard() => _chessboard;
}