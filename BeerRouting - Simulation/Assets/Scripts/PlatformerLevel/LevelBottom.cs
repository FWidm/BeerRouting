using UnityEngine;
using System.Collections;

public class LevelBottom : MonoBehaviour
{

    private SoundManager soundManager;

    // Use this for initialization
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject collider = coll.gameObject;

        if (coll.gameObject.name == "Player")
        {
            soundManager.PlaySound(SoundManager.SoundType.PlayerWater);
            FindObjectOfType<LevelControllerPlatform>().LevelFailed();
        }
    }
}
