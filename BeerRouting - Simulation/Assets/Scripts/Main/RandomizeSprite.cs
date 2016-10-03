using UnityEngine;
using System.Collections;

public class RandomizeSprite : MonoBehaviour
{
	private Sprite[] sprites;
	public string resName;

	// Use this for initialization
	void Start ()
	{
		SpriteRenderer renderer = this.GetComponent<SpriteRenderer> ();
		sprites = Resources.LoadAll<Sprite> (resName);
		int min = 0, max = sprites.Length;

		int n = (int)Random.Range (min, max);
		//Debug.Log (sprites + " n=" + n);
		renderer.sprite = sprites [n];
	}

}
