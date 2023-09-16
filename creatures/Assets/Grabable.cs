using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Grabable : MonoBehaviour
{

    static LinkedList<Grabable> grabables = new LinkedList<Grabable>();

    public static int numberOfGrabables;

    [SerializeField] bool isSelected;

    private void Awake()
    {
        grabables.AddLast(this);
        numberOfGrabables++;
    }

    public Cell Cell { get => GridScript.GetCellCoords(transform.position); }

    public static void ResetGrabables()
    {

        foreach (Grabable grabable in grabables)
        {

            grabable.isSelected = false;

        }

    }

    public static Grabable ClosestGrabable(Vector2 position, Grabable grabable = null)
    {

        Grabable closest = null;

        float closestDistance = float.PositiveInfinity;

        foreach (Grabable currentGrabable in grabables)
        {

            float currentDistance = Vector3.Distance(position, currentGrabable.transform.position);

            if (currentDistance < closestDistance && !currentGrabable.isSelected || currentGrabable == grabable)
            {
                closest = currentGrabable;
                closestDistance = currentDistance;
            }

        }

        if (closest != null)
        {
            closest.isSelected = true;
        }
        return closest;
    }

    private void Update()
    {
        if (isSelected)
        {
            GetComponent<SpriteRenderer>().color = Color.white;

        } else
        {

            GetComponent<SpriteRenderer>().color = Color.red;

        }
    }
}
