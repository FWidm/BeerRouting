using UnityEngine;

public class PlayerAdjustSortingOrder : MonoBehaviour
{
    public bool displayDebugText = false;

    private SpriteRenderer[] renderers;
    private int iteratedChilds = 0;
    private Canvas canvasBeer;
    private Animator animator;

    /// <summary>
    /// The max y position of the scene.
    /// </summary>
    private float yMax;

    // Use this for initialization
    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        //        Debug.Log(">>> NAME!" + renderers[0].transform.name);

        GameObject backgroundGameObj = GetBackgroundDimensionSecure().gameObject;
        yMax = 2.0f * backgroundGameObj.transform.position.y;
        animator = GetComponent<Animator>();
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
            switch (spriteRenderer.transform.name)
            {
                case "arm_left":
                case "arm_left_long":
                case "leg_right":
                case "beer":
                    bodyPartAdjustment = -2;
                    break;
                case "leg_left":
                    bodyPartAdjustment = -3;
                    break;
                case "hand_left":
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("DrinkBeer"))
                    {
                        // Show beer above head while drink animation is playing.
                        bodyPartAdjustment = 3;
                    }
                    else
                    {
                        bodyPartAdjustment = 1;
                    }
                    break;
                case "hand_right":
                case "mouth_normal":
                case "mouth_happy":
                case "mouth_angry":
                case "mouth_smile":
                case "mouth_open":
                case "mouth_rabbit":
                case "mouth_large":
                case "mouth_small":
                    bodyPartAdjustment = 2;
                    break;
                case "arm_right":
                case "head":
                    bodyPartAdjustment = 1;
                    break;
                default:
                    bodyPartAdjustment = 0;
                    break;

            }

            spriteRenderer.sortingOrder = baseOrder + bodyPartAdjustment;

            if (spriteRenderer.transform.name == "hand_left")
            {
                Canvas canvasBeer = GetComponentInChildren<Canvas>();
                if (canvasBeer != null)
                {
                    // The canvas beer should have the sorting order of the hand_left object minus 1.
                    canvasBeer.sortingOrder = baseOrder + bodyPartAdjustment - 1;
                }
            }

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
