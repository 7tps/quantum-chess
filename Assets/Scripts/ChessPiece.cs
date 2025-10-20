using UnityEngine;

public class ChessPiece : MonoBehaviour
{

    public enum pieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public ChessEngine engine;
    
    public SpriteRenderer sr;
    public Sprite sprite;

    public bool isWhite = true;
    public pieceType type;
    
    public int row;
    public int column;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isWhite)
        {
            sprite = engine.whiteChessSprites[(int) type];
        }
        else
        {
            sprite = engine.blackChessSprites[(int) type];
        }
        
        sr.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (engine != null)
        {
            engine.onPieceClicked(this);
        }
    }
}
