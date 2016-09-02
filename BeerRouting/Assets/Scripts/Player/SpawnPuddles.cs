using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPuddles : MonoBehaviour
{
	public List<GameObject> puddleList;
	public GameObject PuddlePrefab;

	// Use this for initialization
	void Start ()
	{
		puddleList = new List<GameObject> ();
	}

	/// <summary>
	/// Spawns a puddle at the given position. The graphics will be set to a random Sprite
	///  and it is added to a list of all the Puddles.
	/// </summary>
	/// <param name="pos"></param>
	public void Spawn (Vector2 pos)
	{
		GameObject puddle = (GameObject)Instantiate (PuddlePrefab, pos, new Quaternion ());
		puddle.transform.parent = this.transform.parent;
		RandomizeBeerpath spawn = puddle.GetComponent<RandomizeBeerpath> ();
		spawn.RandomizeGraphics ();
		puddleList.Add (puddle);
	}

	/// <summary>
	/// Sets the visibility of the puddles to the given parameter.
	/// </summary>
	/// <param name="visible"></param>
	public void setVisible (bool visible)
	{
		foreach (GameObject puddle in puddleList) {
			puddle.SetActive (visible);
		}
	}

	/// <summary>
	/// Destroys all puddles when the method is called.
	/// </summary>
	public void destroyAll ()
	{
		foreach (GameObject puddle in puddleList) {
			Destroy (puddle);
		}
	}
}
