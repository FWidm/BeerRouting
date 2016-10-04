using UnityEngine;
using System.Collections;

public class SpeechBubbleState : MonoBehaviour
{
    public int id;
    [Range(350, 1550)]
    public float width;
    [TextArea(3, 10)]
    public string text;

    private string textWithPlaceholders;
    private DijkstraManager dijkstraManager;
    private TutorialControllerDijkstra tutorialController;
    private LevelController levelController;

    public string ToString()
    {
        return "SpeechBubbleState[id=" + id + ", width=" + width + ", text=" + text + "]";
    }
    // Use this for initialization
    void Start()
    {
        dijkstraManager = FindObjectOfType<DijkstraManager>();

        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();

        text = text.Replace("<PlayerName>", Constants.playerName);
        text = text.Replace("<ProfessorName>", Constants.professorName);
        textWithPlaceholders = text;
    }

    public void UpdatePlaceholders()
    {
        text = textWithPlaceholders;
        string name;
        if (text.Contains("<ClickedPathName>"))
        {
            text = text.Replace("<ClickedPathName>", levelController.GetClickedPathToName());
        }
        if (text.Contains("<CurrentRouterName>"))
        {
            name = levelController.GetCurrentRouterName();
            text = text.Replace("<CurrentRouterName>", name);
        }
        if (text.Contains("<SourceRouterName>"))
        {
            name = levelController.GetClickedPathFromName();
            text = text.Replace("<SourceRouterName>", name);
        }
        if (text.Contains("<DestinationRouterName>"))
        {
            name = levelController.GetClickedPathToName();
            text = text.Replace("<DestinationRouterName>", name);
        }
        if (text.Contains("<PreviousSourceRouterName>"))
        {
            name = levelController.GetPrevRouterName();
            text = text.Replace("<PreviousSourceRouterName>", name);
        }
        if (text.Contains("<NeighbourRouterName1>"))
        {
            name = dijkstraManager.GetNeighborPathsOfCurrentRouter()[0].to.GetComponent<RouterScript>().routerName;
            text = text.Replace("<NeighbourRouterName1>", name);
        }
        if (text.Contains("<NeighbourRouterName2>"))
        {
            name = dijkstraManager.GetNeighborPathsOfCurrentRouter()[1].to.GetComponent<RouterScript>().routerName;
            text = text.Replace("<NeighbourRouterName2>", name);
        }
        if (text.Contains("<NeighbourRouterName3>"))
        {
            name = dijkstraManager.GetNeighborPathsOfCurrentRouter()[2].to.GetComponent<RouterScript>().routerName;
            text = text.Replace("<NeighbourRouterName3>", name);
        }
        if (text.Contains("<NeighbourRouterCost1>"))
        {
            name = dijkstraManager.GetNeighborPathsOfCurrentRouter()[0].pathCosts.ToString();
            text = text.Replace("<NeighbourRouterCost1>", name);
        }
        if (text.Contains("<NeighbourRouterCost2>"))
        {
            name = dijkstraManager.GetNeighborPathsOfCurrentRouter()[1].pathCosts.ToString();
            text = text.Replace("<NeighbourRouterCost2>", name);
        }
        if (text.Contains("<NeighbourRouterCost3>"))
        {
            name = dijkstraManager.GetNeighborPathsOfCurrentRouter()[2].pathCosts.ToString();
            text = text.Replace("<NeighbourRouterCost3>", name);
        }
        if (text.Contains("<LevelName>"))
        {
            name = FindObjectOfType<LevelProperties>().levelName;
            text = text.Replace("<LevelName>", name);
        }
    }

}
