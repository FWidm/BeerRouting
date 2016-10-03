using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PathAdjustOrder : MonoBehaviour
{
    private Renderer objRenderer;
    public bool displayDebugText = true;

    // Use this for initialization
    void Start()
    {
        GameObject backgroundObj = GetBackgroundDimensionSecure().gameObject;
        float yPosBackground = backgroundObj.transform.position.y;

        float yMax = 2.0f * yPosBackground;

        float posY = gameObject.transform.position.y;

        //Structure:
        // Path -> PathSprite (-> Label, Highlight), PathCost, PathDestinationText
        SpriteRenderer[] components = GetComponentsInChildren<SpriteRenderer>();

        if (displayDebugText)
            Debug.Log(name + " -------------------");

        foreach (SpriteRenderer component in components)
        {
            component.sortingOrder = Mathf.Abs((int)(10 * (yMax - posY)));
            if (component is SpriteRenderer)
            {
                if (displayDebugText)
                    Debug.Log("Spriterenderer name=" + component.name + " Layer: name=" + component.sortingLayerName + "| order=" + component.sortingOrder);
                if (component.name.Equals("Label"))
                {
                    component.sortingOrder++;
                }

            }

        }
    }

    /// <summary>
    /// Returns the instance of BackgroundDimensions for the currently active level.
    /// </summary>
    /// <returns>An instance of BackgroundDimensions.</returns>
    private BackgroundDimensions GetBackgroundDimensionSecure()
    {
        GameObject[] bg = GameObject.FindGameObjectsWithTag("BackgroundBounds");

        BackgroundDimensions bgDim = null;
        foreach (var item in bg)
        {
            if (bgDim == null)
            {
                bgDim = item.GetComponent<BackgroundDimensions>();
            }
        }
        return bgDim;
    }
}