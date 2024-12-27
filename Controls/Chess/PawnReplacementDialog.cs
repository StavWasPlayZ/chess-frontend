using Avalonia;
using Avalonia.Controls;
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

    private DummyChessPiece[] _pieces = new DummyChessPiece[AcceptablePieces.Length];
    
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
        
        foreach (var piece in _pieces)
        {
            piece.Width = piece.Height = GetChessboard().TileSize;
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
            var piece = new DummyChessPiece(AcceptablePieces[i], Pawn.PlayerType, size);
            SetRow(piece, i);
            Children.Add(piece);
            
            _pieces[i] = piece;
        }
    }
}