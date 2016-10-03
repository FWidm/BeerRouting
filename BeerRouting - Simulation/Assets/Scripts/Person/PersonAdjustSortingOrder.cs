using UnityEngine;

public class PersonAdjustSortingOrder : MonoBehaviour
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

        GameObject backgroundGameObj = GameObject.FindGameObjectWithTag("BackgroundBounds");
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
            /*
                case "RightHand":
                case "LeftHand":
                case "RightBeer":
                case "RightHandOpen":
                */
                case "RightArmWhite":
                case "RightArmGreen":
                case "RightArmBrown":
                case "RightArmRed":
                case "RightArmBlue":
                case "LeftArmWhite":
                case "LeftArmGreen":
                case "LeftArmBrown":
                case "LeftArmRed":
                case "LeftArmBlue":
                    bodyPartAdjustment = 1;
                    break;
                case "RightLeg":
                case "LeftLeg":
                case "HatLightBrown":
                case "HatGreen":
                case "HatBrown":
                    bodyPartAdjustment = -1;
                    break;
                case "Beer":
                    bodyPartAdjustment = -1;
                    break;
                case "Tongue":
                    bodyPartAdjustment = 2;
                    break;
                case "Head":
                    bodyPartAdjustment = 1;
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
