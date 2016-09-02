using UnityEngine;
using System.Collections;

public class Platform_CameraFollowPlayer : MonoBehaviour
{
    public GameObject follow;

    //fancy camera panning
    public float dampTime = 0.4f;
    // Approximate time for the camera to refocus.
    private Vector3 moveVelocity;


    void FixedUpdate()
    {
        this.transform.position = Vector3.SmoothDamp(transform.position, follow.transform.position, ref moveVelocity, dampTime);
        this.transform.position += new Vector3(0, 0, -10);
    }
}
