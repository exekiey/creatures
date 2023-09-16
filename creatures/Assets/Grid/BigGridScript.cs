using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGridScript : MonoBehaviour
{
    [SerializeField] bool drawGrid;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {


        float gridSize = GetComponent<GridScript>().GridSize1;

        float bigGridSize = gridSize * 2;

        float cameraHeight = Camera.main.orthographicSize;

        float cameraWidth = cameraHeight * Camera.main.aspect;

        float offSet = gridSize / 2;


        Gizmos.color = Color.red;

        if (drawGrid)
        {
            if (bigGridSize > 0)
            {
                for (float i = 0; i > -cameraHeight; i -= bigGridSize)
                {
                    Gizmos.DrawLine(new Vector2(-cameraWidth + offSet, i + offSet), new Vector3(cameraWidth + offSet, i + offSet));
                }

                for (float i = 0; i < cameraHeight; i += bigGridSize)
                {
                    Gizmos.DrawLine(new Vector2(-cameraWidth + offSet, i + offSet), new Vector3(cameraWidth + offSet, i + offSet));
                }

                for (float i = 0; i > -cameraWidth; i -= bigGridSize)
                {
                    Gizmos.DrawLine(new Vector2(i + offSet, -cameraHeight + offSet), new Vector3(i + offSet, cameraHeight + offSet));
                }

                for (float i = 0; i < cameraWidth; i += bigGridSize)
                {
                    Gizmos.DrawLine(new Vector2(i + offSet, -cameraHeight + offSet), new Vector3(i + offSet, cameraHeight + offSet));
                }
            }
        }


    }
}
