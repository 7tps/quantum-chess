using UnityEngine;

public class ChessEngine : MonoBehaviour
{

    public Sprite[] whiteChessSprites;
    public Sprite[] blackChessSprites;

    public ChessPiece[] whitePieces;
    public ChessPiece[] blackPieces;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
