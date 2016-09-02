using UnityEngine;
using System.Collections;

public class RandomizeBeerpath : MonoBehaviour {
    //public float scaleGraphics = .3f;
    public void RandomizeGraphics()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        //Load sprites from the Resources Folder
        Sprite[] sprites = Resources.LoadAll<Sprite>("beerPuddle");
        //set min and max for the Randomize Function
        int min = 0, max = sprites.Length;
        //determine the index of the image that will be used
        int n = (int)Random.Range(min, max);
        //assign the sprite
        spriteRenderer.sprite = sprites[n];
        //scale it down
        //transform.localScale = new Vector3(scaleGraphics, scaleGraphics, scaleGraphics);
    }

}
