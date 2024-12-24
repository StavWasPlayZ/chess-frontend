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
}