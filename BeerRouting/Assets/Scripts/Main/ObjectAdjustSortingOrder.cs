using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectAdjustSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int offset = 0;
    public bool displayDebugText = false;

    // Use this for initialization
    void Start()
    {
        GameObject backgroundObj = GetBackgroundDimensionSecure().gameObject;
        float yPosBackground = backgroundObj.transform.position.y;

        float yMax = 2.0f * yPosBackground;

        float posY = gameObject.transform.position.y;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = Mathf.Abs((int)(10 * (yMax - posY))) + offset;
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

    // Displays the router's name on the screen on top of the router sprite.
    void OnGUI()
    {
        if (displayDebugText)
        {
            Vector3 getPixelPos = Camera.main.WorldToScreenPoint(transform.position);
            getPixelPos.y = Screen.height - getPixelPos.y;
            GUI.Label(new Rect(getPixelPos.x, getPixelPos.y, 50.0f, 50.0f), "" + spriteRenderer.sortingOrder);
        }
    }
}
