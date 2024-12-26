using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using chess_frontend.Controls.Chess.Comm;
using chess_frontend.Util;
using chess_frontend.Views;

namespace chess_frontend.Controls.Chess.Piece;

public abstract class ChessPiece : Avalonia.Svg.Skia.Svg
{
    private const int PressScaleAddition = 5;
    private static readonly Uri BaseUri = new("avares://chess_frontend/");
    
    private readonly Chessboard _chessboard;
    public readonly Type PieceType;
    public readonly PlayerType PlayerType;
    
    public ChessboardTile ParentTile { get; set; }

    public ChessPoint Position => ParentTile.Position;

    protected ChessPiece(
        Type pieceType,
        Chessboard chessboard, ChessboardTile parentTile,
        PlayerType playerType) : base(BaseUri)
    {
        _chessboard = chessboard;
        ParentTile = parentTile;
        PieceType = pieceType;
        PlayerType = playerType;
        
        Width = Height = _chessboard.TileSize;

        Path = "/Assets/pieces/"
               + $"{playerType.ToString().ToLower()}_{pieceType.ToString().ToLower()}.svg";
        
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
        var mayMove = await ValidateMove(destination);
        GetChessboard().IsLocked = false;
        
        if (!mayMove)
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

    protected async Task<bool> ValidateMove(ChessPoint destination)
    {
        // While the backend does check for out-of-bounds behaviour,
        // we won't actually be able to determine where it will be on the board.
        if (destination.IsOutOfBounds)
        {
            MainWindow.Instance!.LogToPanel("BAD MOVE: Destination out of bounds", LogType.Error);
            return false;
        }
        
        var srcCN = Position.AsChessNotation;
        var destCN = destination.AsChessNotation;
        MainWindow.Instance!.LogToPanel($"Committing move: {srcCN + destCN}", LogType.Info);
        
        await MainWindow.Pipe.SendMsgAsync(srcCN + destCN + (int)PlayerType);
        var response = await MainWindow.Pipe.WaitForMsgAsync();

        if (!int.TryParse(response, out var statusCode))
        {
            MainWindow.Instance.LogToPanel($"Backend returned failure (literal: {response})", LogType.Error);
            return false;
        }

        if (statusCode < 0 || statusCode > Enum.GetValues(typeof(MoveResult)).Length - 1)
        {
            MainWindow.Instance.LogToPanel($"Backend returned invalid result range ({statusCode})", LogType.Error);
            return false;
        }
        
        var moveResult = (MoveResult)statusCode;
        if (!moveResult.IsLegalMove())
        {
            MainWindow.Instance.LogToPanel(
                $"Backend returned failure: {moveResult.Description()} ({statusCode})",
                LogType.Error
            );
            return false;
        }

        MainWindow.Instance.LogToPanel(
            $"Backend returned OK: {moveResult.Description()} ({statusCode})",
            LogType.Success
        );
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