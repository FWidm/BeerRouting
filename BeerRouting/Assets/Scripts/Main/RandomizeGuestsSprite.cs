using UnityEngine;
using System.Collections;

public class RandomizeGuestsSprite : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		SpriteRenderer renderer = this.GetComponent<SpriteRenderer> ();
		Sprite[] sprites = Resources.LoadAll<Sprite> ("betrunkene");
		int min = 0, max = sprites.Length;

		int n = (int)Random.Range (min, max);
		//Debug.Log (sprites + " n=" + n);
		renderer.sprite = sprites [n];
	}

}
