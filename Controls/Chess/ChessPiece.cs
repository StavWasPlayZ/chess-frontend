using System;
using Avalonia.Controls;

namespace chess_frontend.Controls.Chess;

public class ChessPiece : Panel
{
    private readonly Chessboard _chessboard;
    public readonly Type PieceType;
    public readonly PlayerType PlayerType;
    
    public ChessPiece(Chessboard chessboard, Type pieceType, PlayerType playerType)
    {
        _chessboard = chessboard;
        PieceType = pieceType;
        PlayerType = playerType;
        
        Children.Add(new Avalonia.Svg.Skia.Svg(new Uri("avares://chess_frontend/"))
        {
            Path = "/Assets/pieces/"
                   + $"{playerType.ToString().ToLower()}_{pieceType.ToString().ToLower()}.svg"
        });
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