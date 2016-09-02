using UnityEngine;
using System.Collections;

public class Platformer_SeagullBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private Platformer_Movement ms;
    private float speed = 2;
    public GameObject goal;
    public GameObject start;
    private LevelControllerPlatform levelController;
    private SoundManager soundManager;

    // Use this for initialization
    void Start()
    {
        levelController = FindObjectOfType<LevelControllerPlatform>();
        rb = GetComponent<Rigidbody2D>();
        ms = levelController.GetPlayerMovementScript();
        speed = Random.Range(1, 4);
        soundManager = FindObjectOfType<SoundManager>();
    }
	
    // Update is called once per frame
    void FixedUpdate()
    {
        //When the Bird flies out of bounds, respawn in a random Range between x=5..20, and oldY+-1s
//        Debug.Log("name=" + gameObject.name + " | Pos=" + transform.position.x);
        if (transform.position.x <= start.transform.position.x - 5)
        {
            float yModifier = Random.Range(-1.0f, 1.0f);
			transform.position = new Vector3(goal.transform.position.x + 55, yModifier + transform.position.y, transform.position.z);
        }
        rb.velocity = new Vector2(-speed, 0);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject collider = coll.gameObject;

        if (coll.name == "Player" && ms != null)
        {
            soundManager.PlaySound(SoundManager.SoundType.PlayerDead);
            levelController.LevelFailed();
            this.gameObject.SetActive(false);
        }
    }
}
