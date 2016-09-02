using UnityEngine;
using System.Collections;

public class RandomizeBubblePosition : MonoBehaviour
{

    public bool debugging = false;
    public float xOffset = 0;
    public float yOffset = 0;
	
    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.position += new Vector3(xOffset, yOffset, 0);
        if (debugging)
            Debug.Log(this.name + " | " + transform.position + " | xOff=" + xOffset + " yOff=" + yOffset);
    }
}
