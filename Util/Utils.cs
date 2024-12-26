using chess_frontend.Controls.Chess;

namespace chess_frontend.Util;

public static class Utils
{
    /**
     * Checks if the goal was reached from either the
     * beginning or end of an array.
     */
    public static bool MirrorCheck(int index, int goal, int size = Chessboard.ChessboardSize)
    {
        return index == goal || index + 1 == size - goal;
    }
}