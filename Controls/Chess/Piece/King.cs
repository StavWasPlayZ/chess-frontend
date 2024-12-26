namespace chess_frontend.Controls.Chess.Piece;

public class King(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.King, chessboard, parentTile, playerType);