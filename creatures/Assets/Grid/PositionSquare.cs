using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSquare : MonoBehaviour
{

    [SerializeField] Color color = Color.white;

    [SerializeField] Sprite sprite;

    [SerializeField] Cell currentCell;

    SpriteRenderer spriteRenderer;

    Vector2 realPosition;

    GameObject positionSquare;

    [SerializeField] bool useObjectPosition;

    public Vector2 RealPosition { set => realPosition = value; }


    private void Awake()
    {
        positionSquare = new GameObject();
        spriteRenderer = positionSquare.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    private void Start()
    {
        positionSquare.transform.localScale = Vector2.one * GridScript.GridSize;
    }
    
    private void Update()
    {
        
        if (useObjectPosition)
        {
            realPosition = gameObject.transform.position;
        }

        currentCell = GridScript.GetCellCoords(realPosition);

        positionSquare.transform.position = GridScript.GetRealWorldCoords(currentCell);

        if (GridScript.IsCellOccupied(currentCell))
        {

            positionSquare.GetComponent<SpriteRenderer>().color = Color.red;

        } else
        {
            positionSquare.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }



}
