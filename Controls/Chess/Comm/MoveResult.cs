using System;
using System.Linq;

namespace chess_frontend.Controls.Chess.Comm;

public enum MoveResult
{
    LegalMove,
    Check,
        
    // Errors
    NoTool,
    FriendlyFire,
    SelfCheck,
    OutOfBounds,
    IllegalMove,
    SamePlace,

    Checkmate
}

public static class MoveResultExtensions
{
    private static readonly MoveResult[] LegalResults =
    [
        MoveResult.LegalMove,
        MoveResult.Check,
        MoveResult.Checkmate
    ];
        
    public static bool IsLegalMove(this MoveResult moveResult)
    {
        return LegalResults.Contains(moveResult);
    }

    public static string Description(this MoveResult moveResult)
    {
        return moveResult switch
        {
            MoveResult.LegalMove => "Legal move",
            MoveResult.Check => "Check",
            
            // Errors
            MoveResult.NoTool => "No tool in current position",
            MoveResult.FriendlyFire => "Attempted to devour one's own piece",
            MoveResult.SelfCheck => "Self check",
            MoveResult.OutOfBounds => "Out of bounds",
            MoveResult.IllegalMove => "Illegal move",
            MoveResult.SamePlace => "Same place",
            MoveResult.Checkmate => "Checkmate",
            
            _ => throw new ArgumentOutOfRangeException(nameof(moveResult), moveResult, null)
        };
    }
}