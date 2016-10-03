using UnityEngine;
using System.Collections;

public class TextScript : MonoBehaviour {

    private TextMesh textField;

    private TextOutline outlineRenderer;

	// Use this for initialization
	void Start () {
        // Debug.Log("Start initialization of TextScript.");
	}

    void Awake()
    {
        outlineRenderer = GetComponent<TextOutline>();
        
        // Debug.Log("Awake TextScript.");
        var parent = transform.parent;

        //// get the layerId and sorting order for the sprite
        //// the sprite is the first child of the parent game object
        //var spriteRenderer = parent.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        //var renderer = GetComponent<Renderer>();
        //renderer.sortingLayerID = spriteRenderer.sortingLayerID;
        //renderer.sortingOrder = spriteRenderer.sortingOrder + 1;

        textField = GetComponent<TextMesh>();
        textField.transform.localScale = new Vector3(0.1f,0.1f,1.0f);
        //Debug.Log("Initialized the text mesh: " + textField);

        var spriteTransform = parent.transform.GetChild(0);
        // Set the position of the text relative to the parent position
        Vector3 pos = spriteTransform.position;
        pos.y = pos.y + 2.235f * (textField.fontSize / 100.0f) + 0.07f;

        textField.transform.position = pos;
        textField.text = "";
    }

    /// <summary>
    /// Set the text to the pathText field.
    /// </summary>
    /// <param name="pathText">The new text.</param>
    public void SetPathText(string pathText)
    {
        var parent = transform.parent;

        // Get the layerId and sorting order for the sprite.
        // The sprite is the first child of the parent game object.
        var spriteRenderer = parent.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        var renderer = GetComponent<Renderer>();
        renderer.sortingLayerID = spriteRenderer.sortingLayerID;
        renderer.sortingOrder = spriteRenderer.sortingOrder + 2;

        // Debug.Log("* Sorting layer id of main renderer is: " + renderer.sortingLayerID + " and sorting order is: " + renderer.sortingOrder);
        // Update sorting order also at outline renderer.
        outlineRenderer.AdjustSortingLayerOrderOfOutlineRenderer();

        textField.text = pathText;
    }
}
