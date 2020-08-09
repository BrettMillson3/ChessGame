using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField]
    public GameObject[,] board;
    public GameObject pieces;
    float distanceForCellX;
    float distanceForCellY;
    public GameObject rookObject;
    RookController rook;

    GameObject highlightedPiece;
    float distance = 3f;
    Vector3 worldPosition;

    public GameObject highlightPrefab;
    Transform moveToPos;
    public GameObject canvas;

    bool takenPiece;
    bool xPos;
    bool yPos;

    Vector2Int coords;
    Vector2Int newCoords;

    GameObject[] usedPieces = new GameObject[16];
    int numberOfPawnsMoved = 0;

    bool whiteTurn = true;
    bool playing = true;
    bool correctPlayer = false;

    public Text textTurn;

    // Start is called before the first frame update
    void Start()
    {
        board = new GameObject[8, 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (x * 8 + y < 16) {
                    board[x, y] = pieces.transform.GetChild(0).GetChild(x * 8 + y).gameObject;
                   // Debug.Log(board[x, y]);
                }
                else if (x * 8 + y > 47)
                {
                    board[x, y] = pieces.transform.GetChild(1).GetChild((x - 6) * 8 + y).gameObject;
                  //  Debug.Log(board[x, y]);
                }
                else
                {
                    board[x, y] = null;
                 //   Debug.Log(board[x, y]);
                }

            }
        }
        distanceForCellY = (board[7, 0].transform.position.y - board[0, 0].transform.position.y) / 7;
        distanceForCellX = (board[0, 7].transform.position.x - board[0, 0].transform.position.x) / 7;
    }

    private void Update()
    {
        if (whiteTurn)
        {
            textTurn.text = "White";
        }
        else
        {
            textTurn.text = "Black";
        }
        if (Input.GetButtonDown("Fire1") && highlightedPiece != null && playing)
        {
            if (correctPlayer)
            {
                Vector2 distanceToMouse = new Vector2(highlightedPiece.transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x, highlightedPiece.transform.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                int numberOfMovesX = 0;
                int numberOfMovesY = 0;
                float currentDistanceX = 0;
                float currentDistanceY = 0;
                if (distanceToMouse.x > 0)
                {
                    xPos = true;
                    while (currentDistanceX < distanceToMouse.x)
                    {
                        if (numberOfMovesX == 0)
                        {
                            currentDistanceX += distanceForCellX / 2;
                        }
                        else
                        {
                            currentDistanceX += distanceForCellX;
                        }
                        numberOfMovesX++;
                        if (numberOfMovesX > 1000)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    xPos = false;
                    while (currentDistanceX > distanceToMouse.x)
                    {
                        if (numberOfMovesX == 0)
                        {
                            currentDistanceX -= distanceForCellX / 2;
                        }
                        else
                        {
                            currentDistanceX -= distanceForCellX;
                        }
                        numberOfMovesX++;
                        if (numberOfMovesX > 1000)
                        {
                            break;
                        }
                    }
                }
                if (distanceToMouse.y > 0)
                {
                    yPos = true;
                    while (currentDistanceY < distanceToMouse.y)
                    {
                        if (numberOfMovesY == 0)
                        {
                            currentDistanceY += distanceForCellY / 2;
                        }
                        else
                        {
                            currentDistanceY += distanceForCellY;
                        }
                        numberOfMovesY++;
                        if (numberOfMovesY > 1000)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    yPos = false;
                    while (currentDistanceY > distanceToMouse.y)
                    {
                        if (numberOfMovesY == 0)
                        {
                            currentDistanceY -= distanceForCellY / 2;
                        }
                        else
                        {
                            currentDistanceY -= distanceForCellY;
                        }
                        numberOfMovesY++;
                        if (numberOfMovesY > 1000)
                        {
                            break;
                        }
                    }
                }
                numberOfMovesX--;
                numberOfMovesY--;
                if (highlightedPiece.transform.tag == "Rook")
                {
                    if(MoveRook(numberOfMovesX, numberOfMovesY))
                    {
                        whiteTurn = !whiteTurn;
                    }
                    
                }
                else if (highlightedPiece.transform.tag == "Queen")
                {
                    if (MoveQueen(numberOfMovesX, numberOfMovesY))
                    {
                        whiteTurn = !whiteTurn;
                    }
                }
                else if (highlightedPiece.transform.tag == "WhitePawn")
                {
                    if(MoveWhitePawn(numberOfMovesX, numberOfMovesY))
                    {
                        if (!usedPieces.Contains(highlightedPiece))
                        {
                            usedPieces[numberOfPawnsMoved] = highlightedPiece;
                            numberOfPawnsMoved++;
                        }
                        Debug.Log("White");
                        whiteTurn = !whiteTurn;
                    }
                }
                else if (highlightedPiece.transform.tag == "BlackPawn")
                {
                    if (MoveBlackPawn(numberOfMovesX, numberOfMovesY))
                    {
                        if (!usedPieces.Contains(highlightedPiece))
                        {
                            usedPieces[numberOfPawnsMoved] = highlightedPiece;
                            numberOfPawnsMoved++;
                        }
                        Debug.Log("Black");
                        whiteTurn = !whiteTurn;
                    }
                }
                else if (highlightedPiece.transform.tag == "Bishop")
                {
                    if (MoveBishop(numberOfMovesX, numberOfMovesY))
                    {
                        whiteTurn = !whiteTurn;
                    }
                }
                else if (highlightedPiece.transform.tag == "Knight")
                {
                    if (MoveKnight(numberOfMovesX, numberOfMovesY))
                    {
                        whiteTurn = !whiteTurn;
                    }
                }
                else if (highlightedPiece.transform.tag == "King")
                {
                    if (MoveKing(numberOfMovesX, numberOfMovesY))
                    {
                        whiteTurn = !whiteTurn;
                    }
                }
                highlightedPiece = null;
            }
            correctPlayer = false;
           
        }
        // CheckForCheck();
        if (CheckForWin())
        {
            playing = false;
        }
    }

    bool CheckForCheck()
    {
        Vector2Int whiteKingCoords = new Vector2Int(-1, -1);
        Vector2Int blackKingCoords = new Vector2Int(-1, -1);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(board[i, j] != null)
                {
                    if (board[i, j].transform.name.Contains("WhiteKing"))
                    {
                        whiteKingCoords = new Vector2Int(i, j);
                    }
                    if (board[i, j].transform.name.Contains("BlackKing"))
                    {
                        blackKingCoords = new Vector2Int(i, j);
                    }
                }
            }
        }
        for (int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (board[i, j] != null)
                {
                    CanSeeKing(board[i, j], whiteKingCoords, blackKingCoords, new Vector2Int(i, j));
                }
            }
        }
        return false;
    }

    bool CanSeeKing(GameObject piece, Vector2Int whiteKingCoords, Vector2Int blackKingCoords, Vector2Int pieceCoords)
    {
        for(int i = 0; i < 2; i++)
        {
            if (piece.transform.tag == "Pawn" && piece.transform.name.Contains("White"))
            {
                if(pieceCoords.x - blackKingCoords.x < 2 && pieceCoords.y - blackKingCoords.y < 2)
                {
                    Debug.Log("Check by pawn");
                }
            }
        }
        return false;
    }

    bool CheckForWin()
    {
        bool whiteKing = false;
        bool blackKing = false;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(board[i, j] != null)
                {
                    if (board[i, j].transform.name.Contains("WhiteKing"))
                    {
                        whiteKing = true;
                    }
                    if (board[i, j].transform.name.Contains("BlackKing"))
                    {
                        blackKing = true;
                    }
                }
            }
        }
        if(!whiteKing|| !blackKing)
        {
            return true;
        }
        return false;
    }

    bool MoveRook(int numberOfMovesX, int numberOfMovesY)
    {
        if (CanMoveRook(numberOfMovesX, numberOfMovesY))
        {
            highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX * (newCoords.y - coords.y), highlightedPiece.transform.position.y + distanceForCellY * (newCoords.x-coords.x));
            if(board[newCoords.x, newCoords.y] != null)
            {
                TakePiece(newCoords);
            }
            board[newCoords.x, newCoords.y] = highlightedPiece;
            board[coords.x, coords.y] = null;
        }
        return true;
    }

    bool CanMoveRook(int numberOfMovesX, int numberOfMovesY)
    {
        coords = GetPieceCoords();
        newCoords = coords;
        if (numberOfMovesX != 0 && numberOfMovesY != 0)
        {
            return false;
        }
        if(numberOfMovesX == 0 && numberOfMovesY == 0)
        {
            return false;
        }
        if (!yPos)
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                if (board[newCoords.x + 1, newCoords.y] == null)
                {
                    newCoords = new Vector2Int(newCoords.x + 1, newCoords.y);
                }
                else if (CanTake(new Vector2Int(newCoords.x + 1, newCoords.y)))
                {
                    newCoords = new Vector2Int(newCoords.x + 1, newCoords.y);
                }
                else if (!CanTake(new Vector2Int(newCoords.x + 1, newCoords.y)))
                {
                    return false;
                }
            }
        }
        else
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                if (board[newCoords.x - 1, newCoords.y] == null)
                {
                    newCoords = new Vector2Int(newCoords.x - 1, newCoords.y);
                }
                else if (CanTake(new Vector2Int(newCoords.x - 1, newCoords.y)))
                {
                    newCoords = new Vector2Int(newCoords.x - 1, newCoords.y);
                }
                else if (!CanTake(new Vector2Int(newCoords.x - 1, newCoords.y)))
                {
                    return false;
                }
            }
            numberOfMovesY = -numberOfMovesY;
        }
        if (!xPos)
        {
            for (int i = 0; i < numberOfMovesX; i++)
            {
                if (board[newCoords.x, newCoords.y + 1] == null)
                {
                    newCoords = new Vector2Int(newCoords.x, newCoords.y + 1);
                }
                else if (CanTake(new Vector2Int(newCoords.x, newCoords.y + 1)))
                {
                    newCoords = new Vector2Int(newCoords.x, newCoords.y + 1);
                }
                else if (!CanTake(new Vector2Int(newCoords.x, newCoords.y + 1)))
                {
                    return false;
                }
            }
        }
        else
        {
            for (int i = 0; i < numberOfMovesX; i++)
            {
                if (board[newCoords.x, newCoords.y - 1] == null)
                {
                    newCoords = new Vector2Int(newCoords.x, newCoords.y - 1);
                }
                else if (CanTake(new Vector2Int(newCoords.x, newCoords.y - 1)))
                {
                    newCoords = new Vector2Int(newCoords.x, newCoords.y - 1);
                }
                else if (!CanTake(new Vector2Int(newCoords.x, newCoords.y - 1)))
                {
                    return false;
                }
            }
            numberOfMovesX = -numberOfMovesX;
        }

        return true;
    }

    bool CanTake(Vector2Int coords)
    {
        if (!takenPiece)
        {
            if (highlightedPiece.transform.name.Contains("White"))
            {
                if (board[coords.x, coords.y].transform.name.Contains("White"))
                {
                    return false;
                }
                else
                {
                    takenPiece = true;
                    return true;
                }
            }
            else if (highlightedPiece.transform.name.Contains("Black"))
            {
                if (board[coords.x, coords.y].transform.name.Contains("Black"))
                {
                    return false;
                }
                else
                {
                    takenPiece = true;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        return false;
    } 

    bool MoveQueen(int numberOfMovesX, int numberOfMovesY)
    {
        if (CanMoveBishop(numberOfMovesX, numberOfMovesY) || CanMoveRook(numberOfMovesX, numberOfMovesY))
        {
            highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX * (newCoords.y - coords.y), highlightedPiece.transform.position.y + distanceForCellY * (newCoords.x - coords.x));
            if (board[newCoords.x, newCoords.y] != null)
            {
                TakePiece(newCoords);
            }
            board[newCoords.x, newCoords.y] = highlightedPiece;
            board[coords.x, coords.y] = null;
        }
        return true;
    }

    bool MoveKing(int numberOfMovesX, int numberOfMovesY)
    {
        if (CanMoveKing(numberOfMovesX, numberOfMovesY))
        {
            highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX * (newCoords.y - coords.y), highlightedPiece.transform.position.y + distanceForCellY * (newCoords.x - coords.x));
            if (board[newCoords.x, newCoords.y] != null)
            {
                TakePiece(newCoords);
            }
            board[newCoords.x, newCoords.y] = highlightedPiece;
            board[coords.x, coords.y] = null;
        }
        return true;
    }

    bool CanMoveKing(int numberOfMovesX, int numberOfMovesY)
    {
        coords = GetPieceCoords();
        newCoords = coords;
      //  Debug.Log(numberOfMovesX);
       // Debug.Log(numberOfMovesY);
        if(numberOfMovesX == 0 && numberOfMovesY == 0)
        {
            return false;
        }
        if(numberOfMovesX > 1 || numberOfMovesY > 1)
        {
            return false;
        }
        if (!yPos && !xPos)
        {
            if (board[newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        else if (!yPos && xPos)
        {
            if (board[newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        else if (yPos && !xPos)
        {
            if (board[newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        else if (yPos && xPos)
        {
            if (board[newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        return true;

        return true;
    }

    bool MoveKnight(int numberOfMovesX, int numberOfMovesY)
    {
        if (CanMoveKnight(numberOfMovesX, numberOfMovesY))
        {
            highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX * (newCoords.y - coords.y), highlightedPiece.transform.position.y + distanceForCellY * (newCoords.x - coords.x));
            if (board[newCoords.x, newCoords.y] != null)
            {
                TakePiece(newCoords);
            }
            board[newCoords.x, newCoords.y] = highlightedPiece;
            board[coords.x, coords.y] = null;
        }
        return true;
    }

    bool CanMoveKnight(int numberOfMovesX, int numberOfMovesY)
    {
        coords = GetPieceCoords();
        newCoords = coords;
       // Debug.Log(numberOfMovesX);
       // Debug.Log(numberOfMovesY);
      //  Debug.Log((float)numberOfMovesX / numberOfMovesY != 2f || (float)numberOfMovesY / numberOfMovesX != 2f);
        if((float) numberOfMovesX/numberOfMovesY != 2f && (float) numberOfMovesY/numberOfMovesX != 2f)
        {
            return false;
        }
        if(numberOfMovesX == 0 && numberOfMovesY == 0)
        {
            return false;
        }
        if (!yPos && !xPos)
        {
            if (board[newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        else if(!yPos && xPos)
        {
            if (board[newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x + numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        else if (yPos && !xPos)
        {
            if (board[newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y + numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        else if (yPos && xPos)
        {
            if (board[newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX] == null)
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else if (CanTake(new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX)))
            {
                newCoords = new Vector2Int(newCoords.x - numberOfMovesY, newCoords.y - numberOfMovesX);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    bool MoveBishop(int numberOfMovesX, int numberOfMovesY)
    {
        if (CanMoveBishop(numberOfMovesX, numberOfMovesY))
        {
            highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX * (newCoords.y - coords.y), highlightedPiece.transform.position.y + distanceForCellY * (newCoords.x - coords.x));
            if (board[newCoords.x, newCoords.y] != null)
            {
                TakePiece(newCoords);
            }
            board[newCoords.x, newCoords.y] = highlightedPiece;
            board[coords.x, coords.y] = null;
        }
        return true;
    }

    bool CanMoveBishop(int numberOfMovesX, int numberOfMovesY)
    {
        coords = GetPieceCoords();
        newCoords = coords;
       // Debug.Log(numberOfMovesX);
      //  Debug.Log(numberOfMovesY);
        if(numberOfMovesX != numberOfMovesY)
        {
            return false;
        }
        if(numberOfMovesX == 0 && numberOfMovesY == 0)
        {
            return false;
        }
        for(int i = 0; i < numberOfMovesX; i++)
        {
            if (!yPos && !xPos)
            {
                if (board[newCoords.x + 1, newCoords.y + 1] == null)
                {
                    newCoords = new Vector2Int(newCoords.x + 1, newCoords.y + 1);
                }
                else if (CanTake(new Vector2Int(newCoords.x+1, newCoords.y + 1)))
                {
                    newCoords = new Vector2Int(newCoords.x + 1, newCoords.y + 1);
                }
                else
                {
                    return false;
                }
            }
            else if (!yPos && xPos)
            {
                if (board[newCoords.x + 1, newCoords.y - 1] == null)
                {
                    newCoords = new Vector2Int(newCoords.x + 1, newCoords.y - 1);
                }
                else if (CanTake(new Vector2Int(newCoords.x + 1, newCoords.y - 1)))
                {
                    newCoords = new Vector2Int(newCoords.x + 1, newCoords.y - 1);
                }
                else
                {
                    return false;
                }
            }
            else if (yPos && !xPos)
            {
                if (board[newCoords.x - 1, newCoords.y + 1] == null)
                {
                    newCoords = new Vector2Int(newCoords.x - 1, newCoords.y + 1);
                }
                else if (CanTake(new Vector2Int(newCoords.x - 1, newCoords.y + 1)))
                {
                    newCoords = new Vector2Int(newCoords.x - 1, newCoords.y + 1);
                }
                else
                {
                    return false;
                }
            }
            else if (yPos && xPos)
            {
                if (board[newCoords.x - 1, newCoords.y - 1] == null)
                {
                    newCoords = new Vector2Int(newCoords.x - 1, newCoords.y - 1);
                }
                else if (CanTake(new Vector2Int(newCoords.x - 1, newCoords.y - 1)))
                {
                    newCoords = new Vector2Int(newCoords.x - 1, newCoords.y - 1);
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool MoveWhitePawn(int numberOfMovesX, int numberOfMovesY)
    {
        // Debug.Log(numberOfMovesX);
        //  Debug.Log(numberOfMovesY);
        Vector2Int coords = GetPieceCoords();
        if (!usedPieces.Contains(highlightedPiece))
        {
            if (numberOfMovesY > 2)
            {
                return false;
            }
        }
        if (yPos)
        {
            return false;
        }
        if(numberOfMovesY == 0)
        {
            return false;
        }
        if (highlightedPiece != null)
        {
            if (!yPos && !xPos && board[coords.x + numberOfMovesY, coords.y + numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("White") && board[coords.x + numberOfMovesY, coords.y + numberOfMovesX].transform.name.Contains("White"))
                {
                    return false;
                }
            }
            else if (yPos && !xPos && board[coords.x - numberOfMovesY, coords.y + numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("White") && board[coords.x - numberOfMovesY, coords.y + numberOfMovesX].transform.name.Contains("White"))
                {
                    return false;
                }
            }
            else if (!yPos && xPos && board[coords.x + numberOfMovesY, coords.y - numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("White") && board[coords.x + numberOfMovesY, coords.y - numberOfMovesX].transform.name.Contains("White"))
                {
                    return false;
                }
            }
            else if (yPos && xPos && board[coords.x - numberOfMovesY, coords.y - numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("White") && board[coords.x - numberOfMovesY, coords.y - numberOfMovesX].transform.name.Contains("White"))
                {
                    return false;
                }
            }

        }
        else
        {
            if (numberOfMovesY > 1)
            {
                return false;
            }
        }
        if (numberOfMovesY == 1 && numberOfMovesX == 1)
        {
            if (!xPos)
            {
                coords = GetPieceCoords();
                if (board[coords.x + 1, coords.y + 1] != null)
                {
                    TakePiece(new Vector2Int(coords.x + 1, coords.y + 1));
                    board[coords.x, coords.y] = null;
                    board[coords.x + 1, coords.y + 1] = highlightedPiece;
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX, highlightedPiece.transform.position.y + distanceForCellY);
                    return true;
                }
                else
                {
                    //   Debug.Log("Yo");
                    return false;
                }
            }
            else
            {
                coords = GetPieceCoords();
                if (board[coords.x + 1, coords.y - 1] != null)
                {
                    TakePiece(new Vector2Int(coords.x + 1, coords.y - 1));
                    board[coords.x, coords.y] = null;
                    board[coords.x + 1, coords.y - 1] = highlightedPiece;
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x - distanceForCellX, highlightedPiece.transform.position.y + distanceForCellY);
                    return true;
                }
                else
                {
                    //   Debug.Log("Yo");
                    return false;
                }
            }
        }
        else if (numberOfMovesX > 1)
        {
            return false;
        }
        else if (!yPos)
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                coords = GetPieceCoords();
            //    Debug.Log(coords);
             //   Debug.Log(board[coords.x + 1, coords.y]);
                if (board[coords.x + 1, coords.y] == null)
                {
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y + distanceForCellY);
                    ChangeBoard(1, 0);
                }
                else
                {
                    return false;
                }
            }
        }
  
        return true;
    }

    bool MoveBlackPawn(int numberOfMovesX, int numberOfMovesY)
    {
        //  Debug.Log(numberOfMovesY);
        Vector2Int coords = GetPieceCoords();
        if (numberOfMovesY > 2)
        {
            return false;
        }
        if (!yPos)
        {
            return false;
        }
        if (numberOfMovesY == 0)
        {
            return false;
        }
        if (highlightedPiece != null)
        {
            if (!yPos && !xPos && board[coords.x + numberOfMovesY, coords.y + numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("Black") && board[coords.x + numberOfMovesY, coords.y + numberOfMovesX].transform.name.Contains("Black"))
                {
                    return false;
                }
            }
            else if (yPos && !xPos && board[coords.x - numberOfMovesY, coords.y + numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("Black") && board[coords.x - numberOfMovesY, coords.y + numberOfMovesX].transform.name.Contains("Black"))
                {
                    return false;
                }
            }
            else if (!yPos && xPos && board[coords.x + numberOfMovesY, coords.y - numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("Black") && board[coords.x + numberOfMovesY, coords.y - numberOfMovesX].transform.name.Contains("Black"))
                {
                    return false;
                }
            }
            else if (yPos && xPos && board[coords.x - numberOfMovesY, coords.y - numberOfMovesX] != null)
            {
                if (highlightedPiece.transform.name.Contains("Black") && board[coords.x - numberOfMovesY, coords.y - numberOfMovesX].transform.name.Contains("Black"))
                {
                    return false;
                }
            }

        }
        if (numberOfMovesY == 1 && numberOfMovesX == 1)
        {
            if (xPos)
            {
                coords = GetPieceCoords();
                if (board[coords.x - 1, coords.y - 1] != null)
                {
                    TakePiece(new Vector2Int(coords.x - 1, coords.y - 1));
                    board[coords.x, coords.y] = null;
                    board[coords.x - 1, coords.y - 1] = highlightedPiece;
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x - distanceForCellX, highlightedPiece.transform.position.y - distanceForCellY);
                    return true;
                }
                else
                {
                 //   Debug.Log("Yo");
                    return false;
                }
            }
            else
            {
                coords = GetPieceCoords();
                if (board[coords.x - 1, coords.y + 1] != null && !board[coords.x - 1, coords.y + 1].transform.name.Contains("Black"))
                {
                    TakePiece(new Vector2Int(coords.x - 1, coords.y + 1));
                    board[coords.x, coords.y] = null;
                    board[coords.x - 1, coords.y + 1] = highlightedPiece;
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX, highlightedPiece.transform.position.y - distanceForCellY);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if (numberOfMovesX > 1)
        {
            return false;
        }
        else if (yPos)
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                coords = GetPieceCoords();
             //   Debug.Log(coords);
             //   Debug.Log(board[coords.x - 1, coords.y]);
                if (board[coords.x - 1, coords.y] == null)
                {
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y - distanceForCellY);
                    ChangeBoard(-1, 0);
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    void ShowPossiblePawnMove()
    {
        coords = GetPieceCoords();

    }

    void TakePiece(Vector2Int coords)
    {
        takenPiece = true;
        Destroy(board[coords.x, coords.y]);
    }

    bool ChangeBoard(int numberOfMovesX, int numberOfMovesY)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if(board[x, y] != null) 
                {
                    if (board[x, y].transform.name == highlightedPiece.transform.name)
                    {
                        board[x, y] = null;
                       // Debug.Log(x);
                       // Debug.Log(y);
                      //  Debug.Log(x + numberOfMovesX);
                      //  Debug.Log(y + numberOfMovesY);
                        board[x + numberOfMovesX, y + numberOfMovesY] = highlightedPiece;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    Vector2Int GetPieceCoords()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null)
                {
                    if (board[x, y].transform.name == highlightedPiece.transform.name)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public void ClickPiece(GameObject piece)
    {
 
        if (whiteTurn && piece.transform.name.Contains("White"))
        {
         //   whiteTurn = false;
            correctPlayer = true;
        }
        if (!whiteTurn && piece.transform.name.Contains("Black"))
        {
        //    whiteTurn = true;
            correctPlayer = true;
                
        }
        takenPiece = false;
        highlightedPiece = piece;

        /*   if (highlightedPiece != null)
           {
               if((highlightedPiece.transform.name.Contains("White") && piece.transform.name.Contains("Black")))
               {
                   takenPiece = false;
                   highlightedPiece = piece;
               }
               else if ((highlightedPiece.transform.name.Contains("Black") && piece.transform.name.Contains("White")))
               {
                   takenPiece = false;
                   highlightedPiece = piece;
               }
               else
               {
                   highlightedPiece = null;
               }
           }
           else
           {
               takenPiece = false;
               highlightedPiece = piece;
           }*/
    }

    //  var highlight = Instantiate(highlightPrefab, new Vector3(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y + distanceForCellX * i, 0f), Quaternion.identity);
    // highlight.transform.SetParent(pieces.transform, false);


}
