using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using chess_frontend.Controls.Chess.Piece;
using chess_frontend.Util;
using chess_frontend.Views;

namespace chess_frontend.Controls.Chess;

public class Chessboard : Grid
{
    /// <summary>
    /// When DebugMode is true, ChessPiece.ValidateMove will
    /// always return true and the board is never locked.
    /// </summary>
    public const bool DebugMode = true;
    
    public const int ChessboardSize = 8;

    public PlayerType PlayerTurn { get; private set; } = PlayerType.White;
    
    public Canvas OverlayCanvas { get; set; }

    public double Size
    {
        get => GetValue(WidthProperty);
        set
        {
            SetValue(WidthProperty, value);
            SetValue(HeightProperty, value);
            UpdateChessboardSize();
        }
    }

    private readonly ChessboardTile[,] _tiles = new ChessboardTile[ChessboardSize,ChessboardSize];
    
    public ChessboardTile GetTileAt(ChessPoint point) =>
        _tiles[point.Y, point.X];
    public void SetTileAt(ChessPoint point, ChessboardTile tile) =>
        _tiles[point.Y, point.X] = tile;
    
    public double TileSize { get; private set; }

    // We lock because we might not yet be connected to anything
    public bool IsLocked { get; set; } = true;

    
    public Chessboard(Canvas overlayCanvas) : this()
    {
        OverlayCanvas = overlayCanvas;
    }

    /// Constructs a Chessboard with no overlay canvas.
    /// <p><b>This canvas MUST later be initialized.</b></p>
    public Chessboard()
    {
        AttachedToVisualTree += (_, _) => SetupChessboard();
    }

    
    private ChessboardTile? _srcOld, _destOld;

    public void OnPieceMoved(ChessPiece piece, ChessPoint source)
    {        
        var srcTile = GetTileAt(source);
        var destTile = piece.ParentTile;
        
        // Apply move coloring
        // Disable old
        _srcOld?.BackgroundToDefault();
        _destOld?.BackgroundToDefault();
        
        // Activate new
        srcTile.BackgroundToMoved();
        destTile.BackgroundToMoved();
        
        // Cache 'em
        _srcOld = srcTile;
        _destOld = destTile;
        
        // Apply next turn
        PlayerTurn = 1 - PlayerTurn;
    }

    public void OnBackendConnected()
    {
        IsLocked = false;
    }
    
    

    private void SetupChessboard()
    {
        TileSize = Size / ChessboardSize;
        this.ApplyGridDefinitions(ChessboardSize, ChessboardSize, TileSize);
        
        // Add chess tiles
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                var pos = new ChessPoint(i, j);
                
                var tile = new ChessboardTile(this, pos);
                SetTileAt(pos, tile);
                Children.Add(tile);
            }
        }

        MainWindow.Instance!.LogToPanel("Waiting for backend connection...", LogType.Info);
        MainWindow.Instance .LogToPanel("Press \"Connect to Backend\" to begin.", LogType.Info);
    }

    private void UpdateChessboardSize()
    {
        if (!this.IsAttachedToVisualTree())
            return;
        
        TileSize = Size / ChessboardSize;
        var gSize = new GridLength(TileSize);
        
        for (var i = 0; i < ChessboardSize; i++)
        {
            for (var j = 0; j < ChessboardSize; j++)
            {
                ColumnDefinitions[j].Width = gSize;
            }
            RowDefinitions[i].Height = gSize;
        }
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

        if (Utils.MirrorCheck(row, 0))
        {
            if (Utils.MirrorCheck(column, 0))
                pieceType = ChessPiece.Type.Rook;
            else if (Utils.MirrorCheck(column, 1))
                pieceType = ChessPiece.Type.Knight;
            else if (Utils.MirrorCheck(column, 2))
                pieceType = ChessPiece.Type.Bishop;
            else if (column == 3)
                pieceType = ChessPiece.Type.Queen;
            else if (column == 4)
                pieceType = ChessPiece.Type.King;
            else
                throw new InvalidOperationException("Invalid piece type");
        }
        else
        {
            pieceType = ChessPiece.Type.Pawn;
        }
        
        _tiles[row, column].ContainedChessPiece = ChessPiece.Create(
            pieceType,
            this,
            _tiles[row, column],
            (row < ChessboardSize / 2) ? PlayerType.Black : PlayerType.White
        );
    }
}