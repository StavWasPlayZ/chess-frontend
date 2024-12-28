using System.Threading.Tasks;
using chess_frontend.Controls.Chess.Comm;
using chess_frontend.Util;
using chess_frontend.Views;

namespace chess_frontend.Controls.Chess.Piece;

public class Pawn(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.Pawn, chessboard, parentTile, playerType)
{
    protected override void OnPieceMoved(MoveResult moveResult, ChessPoint srcPos)
    {
        base.OnPieceMoved(moveResult, srcPos);

        var dest = PlayerType == PlayerType.White ? 0 : Chessboard.ChessboardSize - 1;
        
        if (Position.Y == dest)
        {
            PawnReplacementDialog.CreateAndPrompt(this)
                .PieceTypeSelected += async pieceType => await SwitchForPiece(pieceType);
        }

        if (moveResult == MoveResult.EnPassant)
        {
            // It must be the piece currently beneath us.
            GetChessboard().GetTileAt(Position + (
                PlayerType == PlayerType.White
                    ? new ChessPoint(0, 1)
                    : new ChessPoint(0, -1)
            )).ContainedChessPiece!.RemoveFromBoard();
        }
    }

    private async Task SwitchForPiece(Type pieceType)
    {
        var msg = $"{(int)pieceType}{(int)PlayerType}";
        MainWindow.Instance!.LogToPanel($"Committing move: {msg}", LogType.Info);
        
        await MainWindow.Pipe.SendMsgAsync(msg);

        if ((await Utils.FetchMoveResult())?.IsLegalMove() != true)
            return;
        
        var oldParentTile = ParentTile;
        RemoveFromBoard();
        
        oldParentTile.ContainedChessPiece = Create(
            pieceType,
            GetChessboard(),
            ParentTile,
            PlayerType
        );
    }
}