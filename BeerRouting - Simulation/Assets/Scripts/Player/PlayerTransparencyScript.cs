using UnityEngine;
using System.Collections;

public class PlayerTransparencyScript : MonoBehaviour {

    private bool isTransparent;

	// Use this for initialization
	void Start () {
        isTransparent = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.T))
        {
            if (!isTransparent)
            {
                var components = GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer renderer in components)
                {
                    Color color = renderer.color;
                    color.a = 0.5f;
                    renderer.color = color;
                }
                isTransparent = true;
            }
        }
        else
        {
            if (isTransparent)
            {
                var components = GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer renderer in components)
                {
                    Color color = renderer.color;
                    color.a = 1.0f;
                    renderer.color = color;
                }
                isTransparent = false;
            }
        }
	}
}
