using UnityEngine;
using System.Collections;
using System;

public class SpeechBubbleSequence : MonoBehaviour
{

    public int id;
    public System.Collections.Generic.Dictionary<int, SpeechBubbleState> states;

    // Use this for initialization
    void Start()
    {
        states = new System.Collections.Generic.Dictionary<int, SpeechBubbleState>();
        // Add each child object (SpeechBubbleState) to the states.
        Debug.Log(">> No of Childs="+transform.GetChildCount());
        foreach (Transform child in transform)
        {
            SpeechBubbleState s = child.gameObject.GetComponent<SpeechBubbleState>();
            try { states.Add(s.id, s); }
            catch ( ArgumentException e){
                Debug.Log("Error while adding child (id=" + s.id + ") | text="+s.text+"; collection contains value for that id: "+states[id].text);
                
                Debug.LogError(e.StackTrace);
                }
        }
    }

    public void setNewVals(System.Collections.Generic.Dictionary<int, SpeechBubbleState> states)
    {
        this.states = states;
    }


}
