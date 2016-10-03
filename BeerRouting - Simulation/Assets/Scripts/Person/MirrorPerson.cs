using UnityEngine;
using System.Collections;

public class MirrorPerson : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (Random.Range(0, 2) == 1)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
}
