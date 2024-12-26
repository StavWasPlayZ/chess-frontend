namespace chess_frontend.Controls.Chess.Piece;

public class Bishop(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.Bishop, chessboard, parentTile, playerType);