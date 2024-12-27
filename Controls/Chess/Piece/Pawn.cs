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
    protected override void OnPieceMoved(ChessPoint srcPos)
    {
        base.OnPieceMoved(srcPos);

        var dest = PlayerType == PlayerType.White ? 0 : Chessboard.ChessboardSize - 1;
        
        if (Position.Y == dest)
        {
            PawnReplacementDialog.CreateAndPrompt(this)
                .PieceTypeSelected += async pieceType => await SwitchForPiece(pieceType);
        }
    }

    private async Task SwitchForPiece(Type pieceType)
    {
        var msg = $"{(int)pieceType}{(int)PlayerType}";
        MainWindow.Instance!.LogToPanel($"Committing move: {msg}", LogType.Info);
        
        await MainWindow.Pipe.SendMsgAsync(msg);

        if ((await Utils.FetchMoveResult())?.IsLegalMove() != true)
            return;
        
        OnPieceRemoved();
            
        ParentTile.ContainedChessPiece = Create(
            pieceType,
            GetChessboard(),
            ParentTile,
            PlayerType
        );
    }
}