using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using chess_frontend.Views;

namespace chess_frontend.Controls.Chess;

public class ChessPiece : Avalonia.Svg.Skia.Svg
{
    private static readonly Uri BaseUri = new("avares://chess_frontend/");
    
    private readonly Chessboard _chessboard;
    public readonly Type PieceType;
    public readonly PlayerType PlayerType;
    
    public int X { get; set; }
    public int Y { get; set; }
    
    public ChessPiece(Chessboard chessboard, Type pieceType, PlayerType playerType, int x, int y) :
        base(BaseUri)
    {
        _chessboard = chessboard;
        PieceType = pieceType;
        PlayerType = playerType;
        
        X = x;
        Y = y;
        
        Width = Height = _chessboard.Size / Chessboard.ChessboardSize;

        Path = "/Assets/pieces/"
               + $"{playerType.ToString().ToLower()}_{pieceType.ToString().ToLower()}.svg";
    }


    public void StartFollowingPointer(PointerPressedEventArgs e)
    {
        var canvas = GetChessboard().OverlayCanvas;
        var window = MainWindow.Instance!;
        
        canvas.Children.Add(this);
        
        window.PointerMoved += OnPointerMoved;
        window.PointerReleased += OnPointerReleased;
        
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
        
        //TODO figure out where to place now
    }

    private void PositionToPointer(PointerEventArgs e)
    {
        var point = e.GetPosition(GetChessboard().OverlayCanvas);
        point -= new Point(1, 1) * (Width / 2);
        
        Canvas.SetLeft(this, point.X);
        Canvas.SetTop(this, point.Y);
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