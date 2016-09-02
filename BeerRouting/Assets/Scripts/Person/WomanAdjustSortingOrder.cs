using UnityEngine;
using System.Collections;

public class WomanAdjustSortingOrder : MonoBehaviour
{


    public bool displayDebugText = false;

    public string layerName = "Player";

    private SpriteRenderer[] renderers;
    private int iteratedChilds = 0;

    /// <summary>
    /// The max y position of the scene.
    /// </summary>
    private float yMax;

    // Use this for initialization
    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        //Debug.Log(">>> NAME!" + renderers[0].transform.name);

        GameObject backgroundGameObj = GetBackgroundDimensionSecure().gameObject;
        if (backgroundGameObj != null)
            yMax = 2.0f * backgroundGameObj.transform.position.y;
    }

    void Update()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        float posY = gameObject.transform.position.y;
        int baseOrder = Mathf.Abs((int)(10 * (yMax - posY)));
        int bodyPartAdjustment = 0;
        iteratedChilds = 0;

        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            spriteRenderer.sortingLayerName = layerName;
            switch (spriteRenderer.transform.name)
            {
                case "Head":
                    bodyPartAdjustment = 2;
                    break;
                case "Hair_0":
                case "Hair_1":
                    bodyPartAdjustment = 3;
                    break;
                case "hair_2":
                    bodyPartAdjustment = 2;
                    break;
                case "hat":
                    bodyPartAdjustment = 4;
                    break;
                case "skirt_0":
                    bodyPartAdjustment = 0;
                    break;
                case "skirt_1":
                    bodyPartAdjustment = 1;
                    break;
                case "Arm_left":
                case "Arm_right_0":
                case "Arm_right_1":
                    bodyPartAdjustment = -1;
                    break;
                case "beer":
                    bodyPartAdjustment = 2;
                    break;
                case "leg_left":

                case "leg_right":
                    bodyPartAdjustment = -1;
                    break;
                default:
                    bodyPartAdjustment = 0;
                    break;

            }

            spriteRenderer.sortingOrder = baseOrder + bodyPartAdjustment;
            bodyPartAdjustment = 0;
            iteratedChilds++;
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

    // Displays the router's name on the screen on top of the router sprite.
    void OnGUI()
    {
        if (displayDebugText)
        {
            Vector3 getPixelPos = Camera.main.WorldToScreenPoint(transform.position);
            getPixelPos.y = Screen.height - getPixelPos.y;
            GUI.Label(new Rect(getPixelPos.x, getPixelPos.y, 150.0f, 50.0f), " Order=" + gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder);
            GUI.Label(new Rect(getPixelPos.x, getPixelPos.y + 10, 150.0f, 50.0f), "No Children=" + iteratedChilds);
        }
    }
}
