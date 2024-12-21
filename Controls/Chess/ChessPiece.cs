using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
        
        int row = (int) dest.Y, column = (int) dest.X;

        if (row < 0 || column < 0 || row >= Chessboard.ChessboardSize || column >= Chessboard.ChessboardSize)
        {
            ParentTile.Children.Add(this);
        }
        else
        {
            var newPrentTile = GetChessboard()[row, column];
            ParentTile = newPrentTile;
            newPrentTile.Children.Add(this);
        }
    }

    private void PositionToPointer(PointerEventArgs e)
    {
        var point = e.GetPosition(MainWindow.Instance);
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