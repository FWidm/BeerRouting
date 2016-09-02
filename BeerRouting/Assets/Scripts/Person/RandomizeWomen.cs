using UnityEngine;
using System.Collections;
using System.Security.Policy;

public class RandomizeWomen : MonoBehaviour
{
    public bool debug = false;
    private bool hasHair = false;
    private bool hasBeer = false;
    private float r, g, b, a;
    private Color hairColor;

    // Use this for initialization
    void Start()
    {
        SpriteRenderer[] spriterenderers = GetComponentsInChildren<SpriteRenderer>();
        hairColor = RandomizeColor();
        if (debug)
            Debug.Log("before fore each ->  " + hairColor);
        foreach (var spriterenderer in spriterenderers)
        {

            if (debug)
                Debug.Log(spriterenderer.name + "-> " + hairColor.Equals(spriterenderer.color));
            if (spriterenderer.gameObject.name == "hat")
            {
                spriterenderer.gameObject.SetActive(Random.Range(0, 2) < 1);
                spriterenderer.flipX = Random.Range(0, 2) < 1;
            }

            if (spriterenderer.gameObject.name == "hair_0")
            {
                hasHair = Random.Range(0, 2) == 1;                
                spriterenderer.gameObject.SetActive(hasHair);
                spriterenderer.color = hairColor;
                 
            }

            if (spriterenderer.gameObject.name == "hair_1")
            {
                spriterenderer.gameObject.SetActive(!hasHair);
                spriterenderer.color = hairColor;
            }

            if (spriterenderer.gameObject.name == "hair_2" && !hasHair)
            {
                spriterenderer.gameObject.SetActive(Random.Range(0, 2) == 1);
                spriterenderer.color = hairColor;
            }
            else if (spriterenderer.gameObject.name == "hair_2" && hasHair)
            {
                spriterenderer.gameObject.SetActive(false);
            }

            if (spriterenderer.gameObject.name == "Arm_right_0")
            {
                hasBeer = Random.Range(0, 2) == 1;
                spriterenderer.gameObject.SetActive(!hasBeer);
            }

            if (spriterenderer.gameObject.name == "Arm_right_1")
            {
                spriterenderer.gameObject.SetActive(hasBeer);
            }

            if (spriterenderer.gameObject.name == "beer")
            {
                spriterenderer.gameObject.SetActive(hasBeer);
            }
        }
    }

    private Color RandomizeColor()
    {
        float r = Random.Range(0.0f, 1.0f), g = Random.Range(0.0f, 1.0f), b = Random.Range(0.0f, 1.0f);
        return new Color(r, g, b);
    }
}
