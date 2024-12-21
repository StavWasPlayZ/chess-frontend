using System;
using Avalonia.Controls;

namespace chess_frontend.Controls.Chess;

public class Chessboard : Grid
{
    public const int ChessboardSize = 8;

    public PlayerType PlayerTurn { get; private set; } = PlayerType.White;
    
    public Canvas OverlayCanvas { get; set; }

    public double Size
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    private readonly ChessboardTile[,] _tiles = new ChessboardTile[ChessboardSize,ChessboardSize];
    
    public ChessboardTile this[int row, int column] => _tiles[row, column];

    
    public Chessboard(Canvas overlayCanvas)
    {
        OverlayCanvas = overlayCanvas;
        AttachedToVisualTree += (_, _) => InitializeChessboard();
    }

    /// Constructs a Chessboard with no overlay canvas.
    /// <p><b>This canvas MUST later be initialized.</b></p>
    public Chessboard()
    {
        AttachedToVisualTree += (_, _) => InitializeChessboard();
    }

    
    private ChessboardTile? srcOld, destOld;

    public void OnPieceCommittedMove(ChessPiece piece, int srcRow, int srcColumn)
    {        
        var srcTile = _tiles[srcRow, srcColumn];
        var destTile = piece.ParentTile;
        
        // Apply move coloring
        // Disable old
        srcOld?.BackgroundToDefault();
        destOld?.BackgroundToDefault();
        
        // Activate new
        srcTile.BackgroundToMoved();
        destTile.BackgroundToMoved();
        
        // Cache 'em
        srcOld = srcTile;
        destOld = destTile;
        
        // Apply next turn
        PlayerTurn = 1 - PlayerTurn;
    }
    
    

    private void InitializeChessboard()
    {
        // Apply row/column definitions
        RowDefinitions = [];
        ColumnDefinitions = [];
        
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });                
            }
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }
        
        // Add chess tiles
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                var tile = new ChessboardTile(this, i, j);
                _tiles[i, j] = tile;
                Children.Add(tile);
            }
        }
    }
    
    
    /**
     * Checks if the goal was reached from either the
     * beginning or end of an array.
     */
    private static bool MirrorCheck(int index, int goal, int size = ChessboardSize)
    {
        return index == goal || index + 1 == size - goal;
    }

    public void PopulateBoard()
    {
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                PopulateColumn(i, j);
            }
        }
        for (var i = ChessboardSize - 2; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                PopulateColumn(i, j);
            }
        }
    }

    private void PopulateColumn(int row, int column)
    {
        ChessPiece.Type pieceType;

        if (MirrorCheck(row, 0))
        {
            if (MirrorCheck(column, 0))
                pieceType = ChessPiece.Type.Rook;
            else if (MirrorCheck(column, 1))
                pieceType = ChessPiece.Type.Knight;
            else if (MirrorCheck(column, 2))
                pieceType = ChessPiece.Type.Bishop;
            else if (column == 3)
                pieceType = ChessPiece.Type.King;
            else if (column == 4)
                pieceType = ChessPiece.Type.Queen;
            else
                throw new InvalidOperationException("Invalid piece type");
        }
        else
        {
            pieceType = ChessPiece.Type.Pawn;
        }
                
        _tiles[row, column].ContainedChessPiece = new ChessPiece(
            this,
            _tiles[row, column],
            pieceType,
            (row < ChessboardSize / 2) ? PlayerType.Black : PlayerType.White
        );
    }
}