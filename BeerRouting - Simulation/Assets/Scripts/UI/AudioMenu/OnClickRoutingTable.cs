using UnityEngine;
using System.Collections;

public class OnClickRoutingTable : MonoBehaviour
{

    public GameObject panel;
    public GameObject table;
    private AudioSource buttonClick;

    // Use this for initialization
    void Start()
    {
        buttonClick = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleMenu()
    {
        if (buttonClick != null)
            buttonClick.Play();
        if (!panel.activeSelf)
        {
            panel.SetActive(true);
            table.SetActive(true);
        }
        else
            StartCoroutine(CloseAfterTime());
    }

    IEnumerator CloseAfterTime()
    {
        // Wait a short time.
        if (buttonClick != null)
            yield return new WaitForSeconds(buttonClick.clip.length);
        // Then close panel.
        panel.SetActive(!panel.activeSelf);
        table.SetActive(!table.activeSelf);
    }
}
