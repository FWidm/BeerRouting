using UnityEngine;
using System.Collections;
public class RandomizeSpriteRendererColor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = RandomizeColor();
    }

    private Color RandomizeColor()
    {
        float r = Random.Range(0.0f, 1.0f), g = Random.Range(0.0f, 1.0f), b = Random.Range(0.0f, 1.0f);
        return new Color(r, g, b);
    }
}
