using UnityEngine;
using System.Collections;

public class GlassShardTriggerQTE: MonoBehaviour
{
    public bool debugging = true;

    /// <summary>
    /// Raises the trigger enter2 d event.
    /// </summary>
    /// <param name="coll">Collider</param>
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (debugging)
            Debug.Log(name + " entered by: " + coll.name);
        
        QuickTimeScript qts = coll.GetComponent<QuickTimeScript>();
        qts.SetActive(true);
    }

    /// <summary>
    /// When the collider is left, deactivate the QTE.
    /// </summary>
    /// <param name="coll">Collider</param>
    void OnTriggerExit2D(Collider2D coll)
    {
        if (debugging)
            Debug.Log(name + " exited by: " + coll.name);

        foreach (var collider in transform.parent.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }

        QuickTimeScript qts = coll.GetComponent<QuickTimeScript>();
        qts.SetActive(false);
    }

    /// <summary>
    /// Activates the collider.
    /// </summary>
    public void ActivateCollider()
    {
        this.GetComponent<Collider2D>().enabled = true;
    }
}
