using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeechBubbleSequences : MonoBehaviour
{

    public System.Collections.Generic.Dictionary<int, SpeechBubbleSequence> sequences;

    // Use this for initialization
    void Start()
    {
        sequences = new System.Collections.Generic.Dictionary<int, SpeechBubbleSequence>();
        // Add each child object (SpeechBubbleSequence) to the sequences.
        foreach (Transform child in transform)
        {
            SpeechBubbleSequence s = child.gameObject.GetComponent<SpeechBubbleSequence>();
            Debug.Log("Add Sequence with id=" + s.id);
            sequences.Add(s.id, s);
        }
    }

    public void ReplaceSequences(Dictionary<int,SpeechBubbleSequence> dict)
    {
        sequences = dict;
    }

    public SpeechBubbleSequence GetSequence(int id)
    {
        return sequences[id];
    }

    public int Size()
    {
        return sequences.Values.Count;
    }
}
