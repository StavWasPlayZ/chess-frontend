using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using chess_frontend.Util;
using chess_frontend.Views;

namespace chess_frontend.Controls.Chess;

public class ChessPiece : Avalonia.Svg.Skia.Svg
{
    private const int PressScaleAddition = 5;
    private static readonly Uri BaseUri = new("avares://chess_frontend/");
    
    private readonly Chessboard _chessboard;
    public readonly Type PieceType;
    public readonly PlayerType PlayerType;
    
    public ChessboardTile ParentTile { get; set; }

    public ChessPoint Position => ParentTile.Position;
    
    public ChessPiece(Chessboard chessboard, ChessboardTile parentTile,
            Type pieceType, PlayerType playerType) :
        base(BaseUri)
    {
        _chessboard = chessboard;
        ParentTile = parentTile;
        
        PieceType = pieceType;
        PlayerType = playerType;
        
        Width = Height = _chessboard.Size / Chessboard.ChessboardSize;

        Path = "/Assets/pieces/"
               + $"{playerType.ToString().ToLower()}_{pieceType.ToString().ToLower()}.svg";
        
        PointerPressed += OnPointerPressed;
    }
    

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
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
        
        //Figure out where to place now
        var dest = e.GetPosition(GetChessboard());
        dest /= GetChessboard().Size;
        dest *= Chessboard.ChessboardSize;
        
        TryMoveTo(dest.AsChessPoint());
    }
    
    private void PositionToPointer(PointerEventArgs e)
    {
        var point = e.GetPosition(MainWindow.Instance);
        point -= new Point(1, 1) * (Width / 2);
        
        Canvas.SetLeft(this, point.X);
        Canvas.SetTop(this, point.Y);
    }


    protected void TryMoveTo(ChessPoint destination)
    {
        if (!ValidateMove(destination))
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
            destTile.ContainedChessPiece.PointerPressed -= OnPointerPressed;
            //TODO: And add OnPieceToDevour
            // And Fetch new score for player
        }

        ParentTile.ContainedChessPiece = null;
        destTile.ContainedChessPiece = this;
        
        ParentTile = destTile;
        GetChessboard().OnPieceCommittedMove(this, srcPos);
    }

    protected bool ValidateMove(ChessPoint destination)
    {
        var srcCN = Position.AsChessNotation;
        var destCN = destination.AsChessNotation;
        MainWindow.Instance!.LogToPanel($"Committing move: {srcCN + destCN}", LogType.Info);

        // While the backend does check for out-of-bounds behaviour,
        // we won't actually be able to determine where it will be on the board.
        if (destination.IsOutOfBounds)
        {
            MainWindow.Instance.LogToPanel("BAD MOVE: Destination out of bounds", LogType.Error);
            return false;
        }
        
        //TODO backend call

        var statusCode = 0;
        // TODO: Log out different stuff.
        MainWindow.Instance.LogToPanel($"Backend returned OK ({statusCode})", LogType.Success);
        return true;
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