using UnityEngine;
using System.Collections;

public class DisableMobileButton : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        if (Application.platform != RuntimePlatform.Android)
            this.gameObject.SetActive(false);
    }
}
