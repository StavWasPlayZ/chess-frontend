namespace chess_frontend.Controls.Chess.Piece;

public class Pawn(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.Pawn, chessboard, parentTile, playerType);