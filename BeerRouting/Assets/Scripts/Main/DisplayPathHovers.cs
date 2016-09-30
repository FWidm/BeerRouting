using UnityEngine;
using System.Collections;

public class DisplayPathHovers : MonoBehaviour
{
    public bool show
    {
        get;
        set;
    }

    public void ClickButtonHighlight()
    {
        show = !show;
        Debug.Log("Button Highlight clicked!" + show);

    }
}
