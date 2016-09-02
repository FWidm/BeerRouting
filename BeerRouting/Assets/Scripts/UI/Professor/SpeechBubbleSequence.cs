using UnityEngine;
using System.Collections;
using System;

public class SpeechBubbleSequence : MonoBehaviour {

    public int id;
    public System.Collections.Generic.Dictionary<int, SpeechBubbleState> states;

    // Use this for initialization
    void Start () {
        states = new System.Collections.Generic.Dictionary<int, SpeechBubbleState>();
        // Add each child object (SpeechBubbleState) to the states.
        foreach (Transform child in transform)
        {
            SpeechBubbleState s = child.gameObject.GetComponent<SpeechBubbleState>();
            states.Add(s.id, s);
        }
    }

    public void setNewVals(System.Collections.Generic.Dictionary<int, SpeechBubbleState> states)
    {
        this.states = states;
    }


}
