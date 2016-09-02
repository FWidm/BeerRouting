using UnityEngine;
using System.Collections;

public class BackgroundDimensions : MonoBehaviour
{
    public bool debug = true;
    private Vector3 start, end;
    // Use this for initialization
    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Vector3 size = renderer.bounds.size;
        start = transform.position - size / 2;
        end = transform.position + size / 2;
        if (debug)
            Debug.Log("Background position=" + transform.position + " size=" + size);
    }

    public Vector3 GetBackgroundStart()
    {
        return start;
    }

    public Vector3 GetBackgroundEnd()
    {
        return end;
    }

    public Vector3 GetPivot()
    {
        return transform.position;
    }
}
