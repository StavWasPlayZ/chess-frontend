using System;
using chess_frontend.Util;

namespace chess_frontend.Controls.Chess.Piece;

public class Pawn(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.Pawn, chessboard, parentTile, playerType)
{
    protected override void OnPieceMoved(ChessPoint srcPos)
    {
        base.OnPieceMoved(srcPos);

        var dest = PlayerType == PlayerType.White ? 0 : Chessboard.ChessboardSize - 1;
        
        if (Position.Y == dest)
        {
            PawnReplacementDialog.CreateAndPrompt(this).PieceTypeSelected += OnNewPieceTypeSelected;
        }
    }

    private void OnNewPieceTypeSelected(Type pieceType)
    {
        Console.WriteLine(pieceType.ToString());
    }
}