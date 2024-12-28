using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using chess_frontend.Controls.Chess.Comm;
using chess_frontend.Util;
using chess_frontend.Views;

namespace chess_frontend.Controls.Chess.Piece;

/// <summary>
/// A chess piece with player logic.
/// </summary>
public abstract class ChessPiece : DummyChessPiece
{
    private const int PressScaleAddition = 5;
    
    private readonly Chessboard _chessboard;
    
    /// <summary>
    /// The tile containing this chess piece.
    /// If null, then this piece is no longer on-board.
    /// </summary>
    public ChessboardTile ParentTile { get; set; }

    public ChessPoint Position => ParentTile.Position;

    protected ChessPiece(
        Type pieceType,
        Chessboard chessboard, ChessboardTile parentTile,
        PlayerType playerType) : base(pieceType, playerType, chessboard.TileSize)
    {
        _chessboard = chessboard;
        ParentTile = parentTile;
        
        PointerPressed += OnPointerPressed;
    }

    public static ChessPiece Create(Type pieceType,
        Chessboard chessboard, ChessboardTile parentTile,
        PlayerType playerType)
    {
        return pieceType switch
        {
            Type.Pawn => new Pawn(chessboard, parentTile, playerType),
            Type.Knight => new Knight(chessboard, parentTile, playerType),
            Type.Queen => new Queen(chessboard, parentTile, playerType),
            Type.Rook => new Rook(chessboard, parentTile, playerType),
            Type.King => new King(chessboard, parentTile, playerType),
            Type.Bishop => new Bishop(chessboard, parentTile, playerType),
            _ => throw new ArgumentException($"Unknown piece type: {pieceType}")
        };
    }
    

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (GetChessboard().IsLocked)
            if (!Chessboard.DebugMode)
                return;
        
        // While this should really be a thing,
        // one of the backend checks is this.
        // So we shan't. I guess.
        // if (PlayerType != GetChessboard().PlayerTurn)
        //     return;
        
        var canvas = GetChessboard().OverlayCanvas;
        var window = MainWindow.Instance!;
        
        ParentTile.Children.Remove(this);
        canvas.Children.Add(this);
        
        window.PointerMoved += OnPointerMoved;
        window.PointerReleased += OnPointerReleased;

        // Make it a lil larger
        Width = Height = Width + PressScaleAddition;
        
        PositionToPointer(e);
    }
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        PositionToPointer(e);
    }
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var canvas = GetChessboard().OverlayCanvas;
        var window = MainWindow.Instance!;
        
        canvas.Children.Remove(this);
        
        window.PointerMoved -= OnPointerMoved;
        window.PointerReleased -= OnPointerReleased;
        
        Width = Height = Width - PressScaleAddition;
        
        // Figure out where to place now
        var dest = e.GetPosition(GetChessboard());
        dest /= GetChessboard().Size;
        dest *= Chessboard.ChessboardSize;
        
        _ = TryMoveTo(dest.AsChessPoint());
    }
    
    private void PositionToPointer(PointerEventArgs e)
    {
        var point = e.GetPosition(MainWindow.Instance);
        point -= new Point(1, 1) * (Width / 2);
        
        Canvas.SetLeft(this, point.X);
        Canvas.SetTop(this, point.Y);
    }


    protected async Task TryMoveTo(ChessPoint destination)
    {
        // Lock board while waiting for response
        GetChessboard().IsLocked = true;
        var moveResult = await ValidateMove(destination);
        GetChessboard().IsLocked = false;
        
        if (moveResult?.IsLegalMove() != true)
        {
            ParentTile.ContainedChessPiece = this;
            return;
        }

        var srcPos = Position;
        var destTile = GetChessboard().GetTileAt(destination);
        
        // Also check if dest is source because otherwise it will
        // create a self-eating paradox.
        if (destTile.ContainedChessPiece != null && destTile != ParentTile)
        {
            destTile.ContainedChessPiece.RemoveFromBoard();
            //TODO: OnPieceDevoured
            // And fetch player score (maybe)
        }

        ParentTile.ContainedChessPiece = null;
        destTile.ContainedChessPiece = this;
        
        ParentTile = destTile;
        OnPieceMoved(moveResult.Value, srcPos);
        GetChessboard().OnPieceMoved(this, srcPos);
    }

    protected virtual void OnPieceMoved(MoveResult moveResult, ChessPoint srcPos) {}

    public void RemoveFromBoard()
    {
        PointerPressed -= OnPointerPressed;
        ParentTile.ContainedChessPiece = null;
    }

    /// <summary>
    /// Checks the provided move against the backend.
    /// </summary>
    /// <returns>The backend's response</returns>
    protected async Task<MoveResult?> ValidateMove(ChessPoint destination)
    {
        // Debug purposes
        if (Chessboard.DebugMode)
            return MoveResult.LegalMove;
        
        // While the backend does check for out-of-bounds behaviour,
        // we won't actually be able to determine where it will be on the board.
        if (destination.IsOutOfBounds)
        {
            MainWindow.Instance!.LogToPanel("BAD MOVE: Destination out of bounds", LogType.Error);
            return MoveResult.OutOfBounds;
        }
        
        var srcCN = Position.AsChessNotation;
        var destCN = destination.AsChessNotation;
        MainWindow.Instance!.LogToPanel($"Committing move: {srcCN + destCN}", LogType.Info);
        
        await MainWindow.Pipe.SendMsgAsync(srcCN + destCN + (int)PlayerType);
        
        return await Utils.FetchMoveResult();
    }


    public Chessboard GetChessboard() => _chessboard;
    
    
    public enum Type
    {
        Bishop,
        King,
        Queen,
        Rook,
        Knight,
        Pawn
    }
}