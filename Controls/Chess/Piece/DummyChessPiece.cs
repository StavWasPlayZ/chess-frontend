using System;

namespace chess_frontend.Controls.Chess.Piece;

/// <summary>
/// A chess piece with no player logic.
/// Purely for display purposes.
/// </summary>
public class DummyChessPiece : Avalonia.Svg.Skia.Svg
{
    private static readonly Uri BaseUri = new("avares://chess_frontend/");
    
    public readonly ChessPiece.Type PieceType;
    public readonly PlayerType PlayerType;

    public DummyChessPiece(ChessPiece.Type pieceType, PlayerType playerType, double size) : base(BaseUri)
    {
        PieceType = pieceType;
        PlayerType = playerType;
        
        Width = Height = size;

        Path = "/Assets/pieces/"
               + $"{playerType.ToString().ToLower()}_{pieceType.ToString().ToLower()}.svg";
    }
}