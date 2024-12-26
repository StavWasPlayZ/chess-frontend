namespace chess_frontend.Controls.Chess.Piece;

public class Rook(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.Rook, chessboard, parentTile, playerType);