using UnityEngine;
using System.Collections;

public class Platformer_Collectibles : MonoBehaviour
{

    SpriteRenderer sr;
    Platformer_Movement ms;
    LevelControllerPlatform levelController;
    SoundManager soundManager;

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        levelController = FindObjectOfType<LevelControllerPlatform>();
        ms = levelController.GetPlayerMovementScript();
        soundManager = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, 1));
    }

    /// <summary>
    /// Determines the action we want to execute depending on the collectible's type.
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject collider = coll.gameObject;        

        if (coll.gameObject.name == "Player" && ms != null)
        {
            if (this.gameObject.name.Contains("Goal"))
            {
                soundManager.PlaySound(SoundManager.SoundType.RunComplete);
                levelController.OnGoal();                
            }
            else if (this.gameObject.name.Contains("Point"))
            {
                levelController.AddPoints(ScoreText.Plus5);
                soundManager.PlaySound(SoundManager.SoundType.Collectible1);
            }
            else if (this.gameObject.name.Contains("Money"))
            {
                levelController.AddPoints(ScoreText.Plus10);
                soundManager.PlaySound(SoundManager.SoundType.Collectible2);
            }
            else if (this.gameObject.name.Contains("Sunglasses"))
            {                
                OnSunglasses();
                soundManager.PlaySound(SoundManager.SoundType.Collectible3);
            }
            else
                Debug.Log("Collision!");

            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Raises the sunglasses.
    /// </summary>
    private void OnSunglasses()
    {
        FindObjectOfType<PlayerPlatformController>().SetSunglasses(true);
        levelController.AddPoints(ScoreText.Plus20);
    }
}
