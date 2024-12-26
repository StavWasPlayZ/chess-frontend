namespace chess_frontend.Controls.Chess.Piece;

public class Queen(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.Queen, chessboard, parentTile, playerType);