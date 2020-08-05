using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool xPos;
    bool yPos;
    // Start is called before the first frame update
    void Start()
    {
        board = new GameObject[8, 8];
        for(int x = 0; x < 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                if(x * 8 + y < 16) { 
                    board[x, y] = pieces.transform.GetChild(0).GetChild(x * 8 + y).gameObject;
                    Debug.Log(board[x, y]);
                }
                else if(x * 8 + y > 47)
                {
                    board[x, y] = pieces.transform.GetChild(1).GetChild((x-6) * 8 + y).gameObject;
                    Debug.Log(board[x, y]);
                }
                else
                {
                    board[x, y] = null;
                    Debug.Log(board[x, y]);
                }

            }
        }
        distanceForCellY = (board[7, 0].transform.position.y - board[0, 0].transform.position.y) / 7;
        distanceForCellX = (board[0, 7].transform.position.x - board[0, 0].transform.position.x) / 7;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && highlightedPiece != null)
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
            if(highlightedPiece.transform.tag == "Rook")
            {
                MoveRook(numberOfMovesX, numberOfMovesY);
            }
            else if(highlightedPiece.transform.tag == "Queen")
            {
                MoveQueen(numberOfMovesX, numberOfMovesY);
            }
            else
            {
                MoveQueen(numberOfMovesX, numberOfMovesY);
            }
            highlightedPiece = null;
        }
    }

    bool MoveRook(int numberOfMovesX, int numberOfMovesY)
    {
        if(!yPos)
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                Vector2Int coords = GetPeiceCoords();
                if(board[coords.y + 1, coords.x] == null)
                {
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y + distanceForCellY);
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                Vector2Int coords = GetPeiceCoords();
                Debug.Log(coords);
                if (board[coords.y - 1, coords.x] == null)
                {
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y - distanceForCellY);
                }
                else
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
                Vector2Int coords = GetPeiceCoords();
                if (board[coords.y, coords.x+1] == null)
                {
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX, highlightedPiece.transform.position.y);
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            for (int i = 0; i < numberOfMovesX; i++)
            {
                Vector2Int coords = GetPeiceCoords();
                if (board[coords.y, coords.x-1] == null)
                {
                    highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x - distanceForCellX, highlightedPiece.transform.position.y);
                }
                else
                {
                    return false;
                }
            }
            numberOfMovesX = -numberOfMovesX;
        }
        ChangeBoard(numberOfMovesX, numberOfMovesY);
        return true;
    }

    void MoveQueen(int numberOfMovesX, int numberOfMovesY)
    {
        if (!yPos)
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y + distanceForCellY);
            }
        }
        else
        {
            for (int i = 0; i < numberOfMovesY; i++)
            {
                highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y - distanceForCellY);
            }
            numberOfMovesY = -numberOfMovesY;
        }
        if (!xPos)
        {
            for (int i = 0; i < numberOfMovesX; i++)
            {
                highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x + distanceForCellX, highlightedPiece.transform.position.y);
            }
        }
        else
        {
            for (int i = 0; i < numberOfMovesX; i++)
            {
                highlightedPiece.transform.position = new Vector2(highlightedPiece.transform.position.x - distanceForCellX, highlightedPiece.transform.position.y);
            }
            numberOfMovesX = -numberOfMovesX;
        }
        ChangeBoard(numberOfMovesX, numberOfMovesY);
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
                       // Debug.Log(x + numberOfMovesY);
                       // Debug.Log(y + numberOfMovesX);
                        board[x + numberOfMovesY, y + numberOfMovesX] = highlightedPiece;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    Vector2Int GetPeiceCoords()
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
        highlightedPiece = piece;
    }

    //  var highlight = Instantiate(highlightPrefab, new Vector3(highlightedPiece.transform.position.x, highlightedPiece.transform.position.y + distanceForCellX * i, 0f), Quaternion.identity);
    // highlight.transform.SetParent(pieces.transform, false);


}
