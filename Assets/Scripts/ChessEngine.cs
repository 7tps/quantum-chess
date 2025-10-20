using UnityEngine;

public class ChessEngine : MonoBehaviour
{

    public Sprite[] whiteChessSprites;
    public Sprite[] blackChessSprites;
    public GameObject highlightPrefab;

    public ChessPiece[] whitePieces;
    public ChessPiece[] blackPieces;

    private GameObject[] highlightObjects = new GameObject[64]; 
    private ChessPiece selectedPiece = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createHighlightPrefabs();
    }

    private void createHighlightPrefabs()
    {
        if (highlightPrefab == null) return;

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                int index = row * 8 + col;
                
                // Create highlight GameObject from prefab
                GameObject highlight = Instantiate(highlightPrefab, transform);
                highlight.name = $"Highlight_{col}_{row}";
                
                // Position the highlight (you may need to adjust this based on your grid setup)
                highlight.transform.position = new Vector3(col, row, -0.1f);
                
                // Initially hide the highlight
                highlight.SetActive(false);
                
                highlightObjects[index] = highlight;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool validatePosition()
    {
        // Check if any pieces are overlapping (same row and column)
        for (int i = 0; i < whitePieces.Length; i++)
        {
            for (int j = i + 1; j < whitePieces.Length; j++)
            {
                if (whitePieces[i] != null && whitePieces[j] != null &&
                    whitePieces[i].row == whitePieces[j].row && 
                    whitePieces[i].column == whitePieces[j].column)
                {
                    Debug.LogWarning($"White pieces overlapping at ({whitePieces[i].row}, {whitePieces[i].column})");
                    return false;
                }
            }
        }

        for (int i = 0; i < blackPieces.Length; i++)
        {
            for (int j = i + 1; j < blackPieces.Length; j++)
            {
                if (blackPieces[i] != null && blackPieces[j] != null &&
                    blackPieces[i].row == blackPieces[j].row && 
                    blackPieces[i].column == blackPieces[j].column)
                {
                    Debug.LogWarning($"Black pieces overlapping at ({blackPieces[i].row}, {blackPieces[i].column})");
                    return false;
                }
            }
        }

        // Check if any pieces are outside the board bounds (0-7 for both row and column)
        foreach (ChessPiece piece in whitePieces)
        {
            if (piece != null && (piece.row < 0 || piece.row > 7 || piece.column < 0 || piece.column > 7))
            {
                Debug.LogWarning($"White piece {piece.type} is outside board bounds at ({piece.row}, {piece.column})");
                return false;
            }
        }

        foreach (ChessPiece piece in blackPieces)
        {
            if (piece != null && (piece.row < 0 || piece.row > 7 || piece.column < 0 || piece.column > 7))
            {
                Debug.LogWarning($"Black piece {piece.type} is outside board bounds at ({piece.row}, {piece.column})");
                return false;
            }
        }

        // Check for pawns on invalid starting rows (pawns cannot be on first or last row)
        foreach (ChessPiece piece in whitePieces)
        {
            if (piece != null && piece.type == ChessPiece.pieceType.Pawn && piece.row == 0)
            {
                Debug.LogWarning($"White pawn cannot be on row 0");
                return false;
            }
        }

        foreach (ChessPiece piece in blackPieces)
        {
            if (piece != null && piece.type == ChessPiece.pieceType.Pawn && piece.row == 7)
            {
                Debug.LogWarning($"Black pawn cannot be on row 7");
                return false;
            }
        }

        return true;
    }

    public bool movePiece(ChessPiece piece, int newRow, int newColumn)
    {
        // Validate input parameters
        if (piece == null)
        {
            Debug.LogWarning("Cannot move null piece");
            return false;
        }

        if (newRow < 0 || newRow > 7 || newColumn < 0 || newColumn > 7)
        {
            Debug.LogWarning($"Target position ({newRow}, {newColumn}) is outside board bounds");
            return false;
        }

        // Check if target position is the same as current position
        if (piece.row == newRow && piece.column == newColumn)
        {
            Debug.LogWarning("Cannot move piece to the same position");
            return false;
        }

        // Check if target position is occupied by a piece of the same color
        ChessPiece targetPiece = getPieceAt(newRow, newColumn);
        if (targetPiece != null && targetPiece.isWhite == piece.isWhite)
        {
            Debug.LogWarning($"Cannot move {piece.type} to ({newRow}, {newColumn}) - occupied by friendly piece");
            return false;
        }

        // Store original position for potential rollback
        int originalRow = piece.row;
        int originalColumn = piece.column;

        // Move the piece
        piece.row = newRow;
        piece.column = newColumn;

        // Validate the new position
        if (!validatePosition())
        {
            // Rollback the move if it creates an invalid position
            piece.row = originalRow;
            piece.column = originalColumn;
            Debug.LogWarning($"Move of {piece.type} to ({newRow}, {newColumn}) creates invalid position - move cancelled");
            return false;
        }

        // If there was a piece at the target position, capture it
        if (targetPiece != null)
        {
            Debug.Log($"{piece.type} captured {targetPiece.type} at ({newRow}, {newColumn})");
            capturePiece(targetPiece);
        }

        Debug.Log($"Moved {piece.type} from ({originalRow}, {originalColumn}) to ({newRow}, {newColumn})");
        return true;
    }

    public ChessPiece getPieceAt(int row, int column)
    {
        // Check white pieces
        foreach (ChessPiece piece in whitePieces)
        {
            if (piece != null && piece.row == row && piece.column == column)
            {
                return piece;
            }
        }

        // Check black pieces
        foreach (ChessPiece piece in blackPieces)
        {
            if (piece != null && piece.row == row && piece.column == column)
            {
                return piece;
            }
        }

        return null;
    }

    private void capturePiece(ChessPiece piece)
    {
        if (piece.isWhite)
        {
            for (int i = 0; i < whitePieces.Length; i++)
            {
                if (whitePieces[i] == piece)
                {
                    whitePieces[i] = null;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < blackPieces.Length; i++)
            {
                if (blackPieces[i] == piece)
                {
                    blackPieces[i] = null;
                    break;
                }
            }
        }

        // Destroy the GameObject
        if (piece.gameObject != null)
        {
            Destroy(piece.gameObject);
        }
    }

    public void onPieceClicked(ChessPiece piece)
    {
        // Clear previous highlights
        clearHighlights();

        // If clicking the same piece, deselect it
        if (selectedPiece == piece)
        {
            selectedPiece = null;
            return;
        }

        // Select the new piece
        selectedPiece = piece;
        
        // Get valid moves for this piece
        Vector2Int[] validMoves = getValidMoves(piece);
        
        // Highlight valid move positions
        highlightValidMoves(validMoves);
    }

    private void clearHighlights()
    {
        for (int i = 0; i < highlightObjects.Length; i++)
        {
            if (highlightObjects[i] != null)
            {
                highlightObjects[i].SetActive(false);
            }
        }
    }

    private void highlightValidMoves(Vector2Int[] validMoves)
    {
        foreach (Vector2Int move in validMoves)
        {
            int index = move.y * 8 + move.x; // Convert 2D coordinates to 1D array index
            
            if (highlightObjects[index] != null)
            {
                highlightObjects[index].SetActive(true);
            }
        }
    }

    private Vector2Int[] getValidMoves(ChessPiece piece)
    {
        System.Collections.Generic.List<Vector2Int> validMoves = new System.Collections.Generic.List<Vector2Int>();

        switch (piece.type)
        {
            case ChessPiece.pieceType.Pawn:
                validMoves.AddRange(getPawnMoves(piece));
                break;
            case ChessPiece.pieceType.Rook:
                validMoves.AddRange(getRookMoves(piece));
                break;
            case ChessPiece.pieceType.Knight:
                validMoves.AddRange(getKnightMoves(piece));
                break;
            case ChessPiece.pieceType.Bishop:
                validMoves.AddRange(getBishopMoves(piece));
                break;
            case ChessPiece.pieceType.Queen:
                validMoves.AddRange(getQueenMoves(piece));
                break;
            case ChessPiece.pieceType.King:
                validMoves.AddRange(getKingMoves(piece));
                break;
        }

        return validMoves.ToArray();
    }

    private Vector2Int[] getPawnMoves(ChessPiece piece)
    {
        System.Collections.Generic.List<Vector2Int> moves = new System.Collections.Generic.List<Vector2Int>();
        
        int direction = piece.isWhite ? 1 : -1;
        int startRow = piece.isWhite ? 1 : 6;
        
        // Forward move
        int newRow = piece.row + direction;
        if (newRow >= 0 && newRow <= 7 && getPieceAt(newRow, piece.column) == null)
        {
            moves.Add(new Vector2Int(piece.column, newRow));
            
            // Double move from starting position
            if (piece.row == startRow)
            {
                newRow = piece.row + (2 * direction);
                if (newRow >= 0 && newRow <= 7 && getPieceAt(newRow, piece.column) == null)
                {
                    moves.Add(new Vector2Int(piece.column, newRow));
                }
            }
        }
        
        // Diagonal captures
        for (int colOffset = -1; colOffset <= 1; colOffset += 2)
        {
            int newCol = piece.column + colOffset;
            newRow = piece.row + direction;
            
            if (newRow >= 0 && newRow <= 7 && newCol >= 0 && newCol <= 7)
            {
                ChessPiece targetPiece = getPieceAt(newRow, newCol);
                if (targetPiece != null && targetPiece.isWhite != piece.isWhite)
                {
                    moves.Add(new Vector2Int(newCol, newRow));
                }
            }
        }
        
        return moves.ToArray();
    }

    private Vector2Int[] getRookMoves(ChessPiece piece)
    {
        System.Collections.Generic.List<Vector2Int> moves = new System.Collections.Generic.List<Vector2Int>();
        
        // Horizontal and vertical directions
        int[][] directions = { new int[] {0, 1}, new int[] {0, -1}, new int[] {1, 0}, new int[] {-1, 0} };
        
        foreach (int[] dir in directions)
        {
            for (int i = 1; i < 8; i++)
            {
                int newRow = piece.row + (dir[0] * i);
                int newCol = piece.column + (dir[1] * i);
                
                if (newRow < 0 || newRow > 7 || newCol < 0 || newCol > 7) break;
                
                ChessPiece targetPiece = getPieceAt(newRow, newCol);
                if (targetPiece == null)
                {
                    moves.Add(new Vector2Int(newCol, newRow));
                }
                else
                {
                    if (targetPiece.isWhite != piece.isWhite)
                    {
                        moves.Add(new Vector2Int(newCol, newRow));
                    }
                    break;
                }
            }
        }
        
        return moves.ToArray();
    }

    private Vector2Int[] getKnightMoves(ChessPiece piece)
    {
        System.Collections.Generic.List<Vector2Int> moves = new System.Collections.Generic.List<Vector2Int>();
        
        int[][] knightMoves = {
            new int[] {2, 1}, new int[] {2, -1}, new int[] {-2, 1}, new int[] {-2, -1},
            new int[] {1, 2}, new int[] {1, -2}, new int[] {-1, 2}, new int[] {-1, -2}
        };
        
        foreach (int[] move in knightMoves)
        {
            int newRow = piece.row + move[0];
            int newCol = piece.column + move[1];
            
            if (newRow >= 0 && newRow <= 7 && newCol >= 0 && newCol <= 7)
            {
                ChessPiece targetPiece = getPieceAt(newRow, newCol);
                if (targetPiece == null || targetPiece.isWhite != piece.isWhite)
                {
                    moves.Add(new Vector2Int(newCol, newRow));
                }
            }
        }
        
        return moves.ToArray();
    }

    private Vector2Int[] getBishopMoves(ChessPiece piece)
    {
        System.Collections.Generic.List<Vector2Int> moves = new System.Collections.Generic.List<Vector2Int>();
        
        // Diagonal directions
        int[][] directions = { new int[] {1, 1}, new int[] {1, -1}, new int[] {-1, 1}, new int[] {-1, -1} };
        
        foreach (int[] dir in directions)
        {
            for (int i = 1; i < 8; i++)
            {
                int newRow = piece.row + (dir[0] * i);
                int newCol = piece.column + (dir[1] * i);
                
                if (newRow < 0 || newRow > 7 || newCol < 0 || newCol > 7) break;
                
                ChessPiece targetPiece = getPieceAt(newRow, newCol);
                if (targetPiece == null)
                {
                    moves.Add(new Vector2Int(newCol, newRow));
                }
                else
                {
                    if (targetPiece.isWhite != piece.isWhite)
                    {
                        moves.Add(new Vector2Int(newCol, newRow));
                    }
                    break;
                }
            }
        }
        
        return moves.ToArray();
    }

    private Vector2Int[] getQueenMoves(ChessPiece piece)
    {
        System.Collections.Generic.List<Vector2Int> moves = new System.Collections.Generic.List<Vector2Int>();
        
        // Queen combines rook and bishop moves
        moves.AddRange(getRookMoves(piece));
        moves.AddRange(getBishopMoves(piece));
        
        return moves.ToArray();
    }

    private Vector2Int[] getKingMoves(ChessPiece piece)
    {
        System.Collections.Generic.List<Vector2Int> moves = new System.Collections.Generic.List<Vector2Int>();
        
        // King can move one square in any direction
        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                if (rowOffset == 0 && colOffset == 0) continue;
                
                int newRow = piece.row + rowOffset;
                int newCol = piece.column + colOffset;
                
                if (newRow >= 0 && newRow <= 7 && newCol >= 0 && newCol <= 7)
                {
                    ChessPiece targetPiece = getPieceAt(newRow, newCol);
                    if (targetPiece == null || targetPiece.isWhite != piece.isWhite)
                    {
                        moves.Add(new Vector2Int(newCol, newRow));
                    }
                }
            }
        }
        
        return moves.ToArray();
    }
}
