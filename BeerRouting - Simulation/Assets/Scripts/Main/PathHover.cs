using UnityEngine;

public class PathHover : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private LevelController levelController;

    // Use this for initialization
    void Start()
    {
        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (renderer.name.Equals("PathHighlight"))
                spriteRenderer = renderer;
        }
    }

    void OnMouseEnter()
    {
        if (levelController.IsGameInputEnabled())
        {
            spriteRenderer.enabled = true;
        }
    }

    void OnMouseExit()
    {
        if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
            spriteRenderer.enabled = false;
    }
}
