using System;
using Avalonia.Controls;

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

        Path = "/Assets/pieces/"
               + $"{playerType.ToString().ToLower()}_{pieceType.ToString().ToLower()}.svg";
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