using Avalonia;
using chess_frontend.Controls.Chess;

namespace chess_frontend.Util;

public struct ChessPoint(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    
    public readonly string AsChessNotation => $"{(char)(X + 'a')}{Chessboard.ChessboardSize - Y}";

    public bool IsOutOfBounds =>
        Y < 0 || X < 0 || Y >= Chessboard.ChessboardSize || X >= Chessboard.ChessboardSize;
}

public static class PointExtensions
{
    public static ChessPoint AsChessPoint(this Point point) => new ChessPoint((int) point.X, (int) point.Y);
}