using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using chess_frontend.Controls.Chess.Piece;
using chess_frontend.Util;
using chess_frontend.Views;

namespace chess_frontend.Controls.Chess;

public class PawnReplacementDialog : Grid
{
    private static readonly ChessPiece.Type[] AcceptablePieces =
    [
        ChessPiece.Type.Queen,
        ChessPiece.Type.Rook,
        ChessPiece.Type.Bishop,
        ChessPiece.Type.Knight
    ];

    private readonly PieceTile[] _pieceTiles = new PieceTile[AcceptablePieces.Length];
    
    public Chessboard GetChessboard() => Pawn.GetChessboard();
    
    public readonly Pawn Pawn;
    
    public PawnReplacementDialog(Pawn pawn)
    {
        Pawn = pawn;

        Background = Brushes.White;
        
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    /// <summary>
    /// Creates a new Pawn Replacement dialog, pushes it
    /// into its Overlay Canvas and locks the chessboard.
    /// Wraps it in a Border for shadow elevation.
    /// </summary>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public static void CreateAndPrompt(Pawn pawn)
    {
        var dialog = new PawnReplacementDialog(pawn);
        var board = pawn.GetChessboard();

        var border = new Border
        {
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Color = new Color(255/4, 0, 0, 0),
                Blur = 7,
                Spread = 7
            }),
            Child = dialog
        };

        dialog.MoveToTile(border);
        
        board.OverlayCanvas.Children.Add(border);
        board.IsLocked = true;
        
        dialog.GetChessboard().SizeChanged += dialog.OnChessboardSizeChanged;
        dialog.GetChessboard().SizeChanged += (_, _) => dialog.MoveToTile(border);
    }
    
    private void OnChessboardSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        this.ApplyGridDefinitions(AcceptablePieces.Length, 1, GetChessboard().TileSize);
        
        foreach (var pieceTile in _pieceTiles)
        {
            pieceTile.Width = pieceTile.Height = GetChessboard().TileSize;
        }
    }

    private void MoveToTile(Control toMove)
    {
        var newCoords = Pawn.ParentTile.TranslatePoint(new Point(0, 0), MainWindow.Instance!)!.Value;
        
        Canvas.SetTop(toMove, newCoords.Y);
        Canvas.SetLeft(toMove, newCoords.X);
    }
    

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        var size = GetChessboard().TileSize;
        this.ApplyGridDefinitions(AcceptablePieces.Length, 1, size);
        
        for (var i = 0; i < AcceptablePieces.Length; i++)
        {
            var pieceTile = new PieceTile(AcceptablePieces[i], Pawn.PlayerType, i, size);
            SetRow(pieceTile, i);
            Children.Add(pieceTile);
            
            _pieceTiles[i] = pieceTile;
        }
    }

    
    /// <summary>
    /// A panel-tile specific to the Replacement dialog
    /// that contains a DummyChessPiece child.
    /// </summary>
    private class PieceTile : Panel
    {
        private static readonly SolidColorBrush BackgroundTint = new(new Color(10, 0, 0, 0));
        
        private readonly DummyChessPiece Piece;
        public readonly int Row;

        public PieceTile(ChessPiece.Type pieceType, PlayerType playerType, int row, double size)
        {
            Piece = new DummyChessPiece(pieceType, playerType, size);
            Row = row;

            Width = Height = size;
            
            Children.Add(Piece);
            SizeChanged += OnSizeChanged;
            
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
        }

        private void OnPointerExited(object? sender, PointerEventArgs e)
        {
            Background = Brushes.Transparent;
        }
        private void OnPointerEntered(object? sender, PointerEventArgs e)
        {
            Background = BackgroundTint;
        }

        private void OnSizeChanged(object? o, SizeChangedEventArgs sizeChangedEventArgs)
        {
            Piece.Width = Piece.Height = Height;
        }
    }
}