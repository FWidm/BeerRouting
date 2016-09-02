using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class QuickTimeScript: MonoBehaviour
{
    public bool debugging = true;
    public bool active = false;
    public bool pressing = false;
    public int stages = 3;
    int countStages = -1;
    private GameObject displayQTE;
    private String resName = "SpaceBar";
    private float resScale = .3f;
    private Sprite sprite;


    /// <summary>
    /// Event that is fired if the user has succeeded in the quick time event.
    /// </summary>
    UnityEvent e_qteSuccessful;

    /// <summary>
    /// Event that is fired if the user has failed in the quick time event.
    /// </summary>
    UnityEvent e_qteFailed;

    // Use this for initialization
    void Awake()
    {
        if ((Application.platform == RuntimePlatform.Android))
        {
            resName = "TouchIcon";
            resScale = .15f;
        }
        else
        {
            resName = "SpaceBar";
            resScale = .3f;
        }
        e_qteFailed = new UnityEvent();
        e_qteSuccessful = new UnityEvent();
        sprite = Resources.LoadAll<Sprite>(resName)[0];
        displayQTE = new GameObject("QTEDisplay", typeof(SpriteRenderer));
//        displayQTE.transform.parent = this.transform;
        SpriteRenderer sr = displayQTE.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        displayQTE.transform.localScale = new Vector3(resScale, resScale, resScale);
        sr.sortingLayerName = "Professor";
        sr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        //when the mouse is near the player and he clicks, also trigger the QTE activity
        
        bool checkMouseActivation = Vector2.Distance(transform.position, mousePosition) < 1 && Input.GetMouseButton((0));
        if (active && (Input.GetKeyDown(KeyCode.Space) || checkMouseActivation))
        {
            this.GetComponent<Animator>().SetBool("Jump", true);
            countStages = -1;
            if (e_qteSuccessful != null)
                e_qteSuccessful.Invoke();
            active = false;

            // Change player face to happy (rabbit).
            this.GetComponent<PlayerController>().SetMouth(5);
        }
        if (!active && countStages >= 0 && countStages < stages)
        {
            countStages = -1;
            if (e_qteFailed != null)
                e_qteFailed.Invoke();

            // Change player face to angry.
            this.GetComponent<PlayerController>().SetMouth(2);
        }
        SpriteRenderer sr = displayQTE.GetComponent<SpriteRenderer>();

        if (active)
        {
            displayQTE.transform.position = this.transform.position + new Vector3(0, 1.7f, 0);
            sr.enabled = true;
        }
        else
            sr.enabled = false;
    }

    //    void OnGUI()
    //    {
    //        if (stages > 0)
    //        {
    //            Vector3 getPixelPos = Camera.main.WorldToScreenPoint(transform.position);
    //            getPixelPos.y = Screen.height - getPixelPos.y;
    //            GUI.Label(new Rect(getPixelPos.x, getPixelPos.y, 150.0f, 50.0f), msg);
    //        }
    //    }

    public void SetActive(bool active)
    {
        this.active = active;
        if (active)
        {
            countStages = 0;
        }

        if (debugging)
            Debug.Log("QTE enabled? " + active);
    }

    /// <summary>
    /// Subscribes to the qte failed event.
    /// </summary>
    /// <param name="callbackFunction">Callback function.</param>
    public void SubscribeToQteFailedEvent(UnityAction callbackFunction)
    {
        if (e_qteFailed != null)
            e_qteFailed.AddListener(callbackFunction);
    }

    /// <summary>
    /// Unsubscribes from the qte failed event.
    /// </summary>
    /// <param name="callbackFunction">Callback function.</param>
    public void UnsubscribeFromQteFailedEvent(UnityAction callbackFunction)
    {
        if (e_qteFailed != null)
            e_qteFailed.RemoveListener(callbackFunction);
    }

    /// <summary>
    /// Subscribes to the qte succeeded event.
    /// </summary>
    /// <param name="callbackFunction">Callback function.</param>
    public void SubscribeToQteSucceededEvent(UnityAction callbackFunction)
    {
        if (e_qteSuccessful != null)
            e_qteSuccessful.AddListener(callbackFunction);
    }

    /// <summary>
    /// Unsubscribes from the qte succeeded event.
    /// </summary>
    /// <param name="callbackFunction">Callback function.</param>
    public void UnsubscribeFromQteSucceededEvent(UnityAction callbackFunction)
    {
        if (e_qteSuccessful != null)
            e_qteSuccessful.RemoveListener(callbackFunction);
    }
}
