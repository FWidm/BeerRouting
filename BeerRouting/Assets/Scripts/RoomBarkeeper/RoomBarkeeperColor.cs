using UnityEngine;
using System.Collections;

public class RoomBarkeeperColor : MonoBehaviour {

    // Use this for initialization
    void Awake()
    {
        Transform parentTransform = this.transform;
        Color color = RandomizeColor();

        GetComponent<SpriteRenderer>().color = color;

        foreach (Transform childTransform in parentTransform)
        {
            string name = childTransform.name;
            if (name == "arm_left" || name == "arm_right")
            {
                childTransform.gameObject.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    private Color RandomizeColor()
    {
        float r = Random.Range(0.0f, 1.0f), g = Random.Range(0.0f, 1.0f), b = Random.Range(0.0f, 1.0f);
        return new Color(r, g, b);
    }
}
