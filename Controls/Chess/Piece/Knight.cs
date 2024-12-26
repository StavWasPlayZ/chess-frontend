namespace chess_frontend.Controls.Chess.Piece;

public class Knight(
    Chessboard chessboard,
    ChessboardTile parentTile,
    PlayerType playerType
) : ChessPiece(Type.Knight, chessboard, parentTile, playerType);