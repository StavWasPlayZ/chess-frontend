using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using chess_frontend.Controls.Chess;
using chess_frontend.Controls.Chess.Comm;
using chess_frontend.Views;

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

    
    /// <summary>
    /// Fetches the move result from the backend.
    /// Automatically performs logging to the right panel.
    /// </summary>
    /// <returns>
    /// The move result, or null if there was an error
    /// with the result itself.
    /// </returns>
    public static async Task<MoveResult?> FetchMoveResult()
    {
        var response = await MainWindow.Pipe.WaitForMsgAsync();

        if (!int.TryParse(response, out var statusCode))
        {
            MainWindow.Instance!.LogToPanel($"Backend returned failure (literal: {response})", LogType.Error);
            return null;
        }

        if (statusCode < 0 || statusCode > Enum.GetValues(typeof(MoveResult)).Length - 1)
        {
            MainWindow.Instance!.LogToPanel($"Backend returned invalid result range ({statusCode})", LogType.Error);
            return null;
        }
        
        var moveResult = (MoveResult)statusCode;
        
        if (!moveResult.IsLegalMove())
        {
            MainWindow.Instance!.LogToPanel(
                $"Backend returned failure: {moveResult.Description()} ({statusCode})",
                LogType.Error
            );
            return moveResult;
        }

        MainWindow.Instance!.LogToPanel(
            $"Backend returned OK: {moveResult.Description()} ({statusCode})",
            LogType.Success
        );
        return moveResult;
    }
}