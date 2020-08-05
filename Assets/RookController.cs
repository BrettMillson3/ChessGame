using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookController : MonoBehaviour
{
    // Start is called before the first frame update
    public void MoveRook(GameObject piece, int numberSpaces, bool up, float distanceForCell)
    {
        for (int i = 0; i < numberSpaces; i++)
        {
            if (up)
            {
                piece.transform.position = new Vector2(piece.transform.position.x, piece.transform.position.y - distanceForCell);
            }
            else
            {
                piece.transform.position = new Vector2(piece.transform.position.x, piece.transform.position.y + distanceForCell);
            }
        }
    }
}
