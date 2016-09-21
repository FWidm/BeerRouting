using UnityEngine;
using System.Collections;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class Localization : MonoBehaviour
{
    public Language language = Language.English;
    private string scene;
    // Use this for initialization
    void Start()
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("localization/localization");
        string json = txtAsset.text;
        //Debug.Log("Json=" + json);
        scene = SceneManager.GetActiveScene().name;

        Parse(json);
    }

    void Parse(string json)
    {
        var root = JSON.Parse(json);
        Debug.Log(root);
        Debug.Log("Scene=" + scene + " | Language=" + language.ToString().ToLower());
        Debug.Log(root[language.ToString().ToLower()]["ui"]["btn_close"].Value);
        string currLang = language.ToString().ToLower();

        switch (scene)
        {
            case "0DijkstraTutorial":
                {
                    Debug.Log("Dijkstra Tutorial!!!");
                    SpeechBubbleSequence[] sequences = FindObjectOfType<SpeechBubbleSequences>().GetComponentsInChildren<SpeechBubbleSequence>();
                    for (int j = 0; j < sequences.Length; j++)
                    {
                        SpeechBubbleState[] states = sequences[j].GetComponentsInChildren<SpeechBubbleState>();
                        for (int i = 0; i < states.Length; i++)
                        {

                            states[i].text = root[currLang]["prof"]["dijkstra_tutorial"]["seq_" + j + "_dijkstra_tut_" + i]["text"].Value;
                            Debug.Log("Setting sequence with id=" + sequences[j].id + ": using key=[seq" + j + "_dijkstra_tut_" + i + "]=" + root[currLang]["prof"]["dijkstra_tutorial"]["seq_" + j + "_dijkstra_tut_" + i]["text"].Value);
                            states[i].width = Mathf.Clamp(root[currLang]["prof"]["dijkstra_tutorial"]["seq_" + j + "_dijkstra_tut_" + i]["size"].AsInt, 350, 1550);
                        }
                    }

                    break;
                }
            case "1DijkstraLevel1":
            case "2DijkstraLevel2":
            case "3DijkstraLevel3":
            case "4DijkstraLevel4":
                {
                    Debug.Log("Dijkstra Level!!!");
                    
                    SpeechBubbleSequence[] sequences = FindObjectOfType<SpeechBubbleSequences>().GetComponentsInChildren<SpeechBubbleSequence>();
                    for (int j = 0; j < sequences.Length; j++)
                    {
                        SpeechBubbleState[] states = sequences[j].GetComponentsInChildren<SpeechBubbleState>();
                        Debug.Log("Seq=" + sequences.Length + " states=" + states.Length);

                        for (int i = 0; i < states.Length; i++)
                        {

                            states[i].text = root[currLang]["prof"]["dijkstra_1"]["seq_" + j + "_dijkstra_" + i]["text"].Value;
                            Debug.Log("Setting sequence with id=" + sequences[j].id + ": using key=[seq" + j + "_dijkstra_" + i + "]=" + root[currLang]["prof"]["dijkstra_1"]["seq_" + j + "_dijkstra_1_" + i]["text"].Value);
                            states[i].width = Mathf.Clamp(root[currLang]["prof"]["dijkstra_1"]["seq_" + j + "_dijkstra_" + i]["size"].AsInt, 350, 1550);
                        }
                    }

                    break;
                }
            case "24FunLevel":
                {
                    Debug.Log("FUN!!!");
                    SpeechBubbleState[] states = FindObjectOfType<SpeechBubbleSequence>().GetComponentsInChildren<SpeechBubbleState>();
                    Debug.Log(states.Length);

                    //foreach (SpeechBubbleState state in seq.states.Values)
                    for (int i = 0; i < states.Length; i++)
                    {
                        states[i].text = root[currLang]["prof"]["fun"]["seq_0_fun_" + i]["text"].Value;
                        Debug.Log("Getting seq_0_fun_" + i + "=" + root[currLang]["prof"]["fun"]["seq_fun_" + i]["text"].Value);
                        states[i].width = Mathf.Clamp(root[currLang]["prof"]["fun"]["seq_0_fun_" + i]["size"].AsInt, 350, 1550);
                    }

                    break;
                }

            default:
                Debug.Log("No Fun allowed!");
                break;
        }
    }
}
