using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

    // The delay after that the GameObject is destroyed.
    public float destroyDelay;
    // Indicates whether the GameObject should be destroyed directly after the awake taking the destroyDelay into account.
    public bool destroyOnAwake;

	void Awake () {
        if (destroyOnAwake)
        {
            // Destroy the GameObject after the delay.
            Destroy(gameObject, destroyDelay);
        }	
	}
	
    /// <summary>
    /// Destroys the GameObject after the specified delay time.
    /// </summary>
	public void DestroyDelayed () {
        // Destroy the GameObject after the delay.
        Destroy(gameObject, destroyDelay);
    }
}
