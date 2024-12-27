using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using chess_frontend.Controls.Chess.Piece;
using chess_frontend.Util;

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
    /// </summary>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public static PawnReplacementDialog CreateAndPrompt(Pawn pawn)
    {
        var dialog = new PawnReplacementDialog(pawn);
        var board = pawn.GetChessboard();
        
        Canvas.SetTop(dialog, pawn.ParentTile.Bounds.Y);
        Canvas.SetLeft(dialog, pawn.ParentTile.Bounds.X);
        
        board.OverlayCanvas.Children.Add(dialog);
        board.IsLocked = true;
        
        return dialog;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        var size = Pawn.GetChessboard().TileSize;
        this.ApplyGridDefinitions(AcceptablePieces.Length, 1, size);
        
        for (var i = 0; i < AcceptablePieces.Length; i++)
        {
            var piece = new DummyChessPiece(AcceptablePieces[i], Pawn.PlayerType, size);
            SetRow(piece, i);
            Children.Add(piece);
        }
    }
}