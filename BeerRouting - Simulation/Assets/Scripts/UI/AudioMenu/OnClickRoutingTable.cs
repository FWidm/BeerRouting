using UnityEngine;
using System.Collections;

public class OnClickRoutingTable : MonoBehaviour
{

    public GameObject panel;
    public GameObject table;
    public GameObject description;

    private AudioSource buttonClick;
    private int buttonClickCount;

    // Use this for initialization
    void Start()
    {
        buttonClick = GetComponent<AudioSource>();
        buttonClickCount = 0;
    }

    public int GetClickCount()
    {
        return buttonClickCount;
    }

    public void ToggleMenu()
    {


        if (buttonClick != null)
            buttonClick.Play();
        if (!panel.activeSelf)
        {
            panel.SetActive(true);
            table.SetActive(true);
            description.SetActive(true);
            buttonClickCount++;
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
        description.SetActive(!description.activeSelf);
    }
}
