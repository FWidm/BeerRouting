using UnityEngine;
using System.Collections;

public class LevelProperties : MonoBehaviour
{

    public int levelId;
    public string levelName;
    public int levelMaxScore;
    public GameType gameType;

    public bool debugging = true;

    public enum GameType
    {
        Dijkstra,
        Greedy,
        HotPotato,
        UniformCost,
        HopCount,
        Platform
    }

    // Use this for initialization
    void Start()
    {
        GameObject routingButton = GameObject.FindWithTag("RoutingTableButton");
        if (debugging)
            //Debug.Log("Routingbutton=" + routingButton);
        if (routingButton != null)
            routingButton.SetActive(gameType == GameType.Dijkstra || gameType == GameType.UniformCost);

        if (Application.platform != RuntimePlatform.Android)
        {
            GameObject highlightButton = GameObject.FindWithTag("ButtonHighlight");
            if (debugging)
                //Debug.Log("ButtonHighlight=" + highlightButton);
            if (highlightButton != null)
                highlightButton.SetActive(false);
        }
    }

    public void SetLevelMaxScore(int levelMaxScore)
    {
        this.levelMaxScore = levelMaxScore;
        if (debugging)
            Debug.Log("Setting Maximum Score for this level to: " + levelMaxScore);
    }
}
