using Avalonia.Controls;
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

    /// <summary>
    /// Applies the specified row definitions with a fixed size.
    /// </summary>
    public static void ApplyGridDefinitions(this Grid grid, int rows, int columns, double size)
    {
        grid.RowDefinitions = [];
        grid.ColumnDefinitions = [];

        var gSize = new GridLength(size);
        
        for (var j = 0; j < columns; j++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = gSize });
        }
        for (var i = 0; i < rows; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = gSize });
        }
    }
}