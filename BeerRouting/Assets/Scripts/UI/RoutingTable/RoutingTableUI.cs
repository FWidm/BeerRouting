using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RoutingTableUI : MonoBehaviour
{
    public bool isDebugging = false;

    public Color handledRouterColor = new Color(100, 100, 100);
    public Color unhandledRouterColor = new Color(251, 190, 0);
    public Color currentWorkingRouterColor = new Color(255, 255, 255);


    // A reference to the DijkstraManager of the scene.
    DijkstraManager dijkstraManager;

    // A reference to the UniformCostManager;
    UniformCostManager uniformCostManager;

    // UI Texts for Dijkstra routing table.
    public Text routingTableTextField;
    private Text uiTextUnhandledRouters;

    private bool enableTable = false;
    // Use this for initialization
    void Awake()
    {
        //TODO: replace with either a generic manager or a check if dijkstra is needed
        LevelProperties levelProp = FindObjectOfType<LevelProperties>();

        if (levelProp.gameType == LevelProperties.GameType.Dijkstra)
        {
            dijkstraManager = GameObject.FindGameObjectWithTag("Dijkstra").GetComponent<DijkstraManager>();
        }
        else if (levelProp.gameType == LevelProperties.GameType.UniformCost)
        {
            uniformCostManager = GameObject.FindGameObjectWithTag("UniformCosts").GetComponent <UniformCostManager>();
        }
        // UI text field for dijkstra routing table information.
        //routingTableTextField = GetComponentInChildren<Text>();
    }

    /// <summary>
    /// Updates the routing table that is displayed on the screen.
    /// </summary>
    public void UpdateRoutingTableUI()
    {
        // First retrieve the RouterScript objects of the routers that are currently involved.
        RouterScript[] routers = dijkstraManager.GetRoutersInGraph();

        // Sort according to the current priority of the router.
        routers = sortRouterArrayByPriority(routers);

        string output = string.Empty;
        output += createHandledRouterRoutingTableEntries(routers);
        output += createUnhandledRouterRoutingTableEntries(routers);

        if (routingTableTextField != null)
            routingTableTextField.text = output;

    }

    /// <summary>
    /// Create entries for the routers that have been handeld already by the dijkstra algorithm
    /// in the current run. The entries are concatenated to a string.
    /// </summary>
    /// <param name="routers">The list of all routers that are involved in the current dijkstra run.</param>
    /// <returns>A string containing the routing table entries of the already handled routers.</returns>
    private string createHandledRouterRoutingTableEntries(RouterScript[] routers)
    {
        System.Text.StringBuilder s = new System.Text.StringBuilder();

        for (int i = 0; i < routers.Length; i++)
        {
            // Check whether the router is already handled.
            if (dijkstraManager.IsRouterAlreadyHandled(routers[i]))
            {
                string currentRouter = string.Empty;

                // Request predecessor.
                RouterScript predecessorRouter = dijkstraManager.GetPredecessorRouterOnShortestPath(routers[i]);
                if (predecessorRouter != null)
                {
                    // Add entry to the routing table.
                    currentRouter = string.Format("Tisch {0}: ({1},{2})" + System.Environment.NewLine,
                        routers[i].GetRouterName(),
                        routers[i].GetPriority(),
                        predecessorRouter.GetRouterName());
                }

                // Is the router the current working router?
                if (routers[i] == dijkstraManager.GetCurrentWorkingRouter().GetComponent<RouterScript>())
                {
                    currentRouter = colorizeString(currentRouter, currentWorkingRouterColor);
                }

                s.Append(currentRouter);
            }
        }

        string output = s.ToString();
        output = colorizeString(output, handledRouterColor);

        if (isDebugging)
            Debug.Log(" In handled routers: " + output);

        return output;
    }

    /// <summary>
    /// Create entries for the routers that have not been handeld by the dijkstra algorithm so far
    /// in the current run. The entries are concatenated to a string.
    /// </summary>
    /// <param name="routers">The list of all routers that are involved in the current dijkstra run.</param>
    /// <returns>A string containing the routing table entries of the not yet handled routers.</returns>
    private string createUnhandledRouterRoutingTableEntries(RouterScript[] routers)
    {
        System.Text.StringBuilder s = new System.Text.StringBuilder();

        for (int i = 0; i < routers.Length; i++)
        {
            // Check whether the router is already handled.
            if (!dijkstraManager.IsRouterAlreadyHandled(routers[i]))
            {
                // Request predecessor.
                RouterScript predecessorRouter = dijkstraManager.GetPredecessorRouterOnShortestPath(routers[i]);
                if (predecessorRouter != null)
                {
                    // Add entry to the routing table.
                    s.Append(string.Format("Tisch {0}: ({1},{2})" + System.Environment.NewLine,
                            routers[i].GetRouterName(),
                            routers[i].GetPriority(),
                            predecessorRouter.GetRouterName()));
                }
                else
                {
                    // Add entry to the routing table without predecessor.
                    s.Append(string.Format("Tisch {0}: (inf,-)" + System.Environment.NewLine,
                            routers[i].GetRouterName()));
                }
            }
        }

        string output = s.ToString();
        output = colorizeString(output, unhandledRouterColor);

        if (isDebugging)
            Debug.Log(" In unhandled routers: " + output.Substring(1, output.Length - 1));

        return output;
    }

    /// <summary>
    /// Updates the routing table for the uniform costs algorithm.
    /// </summary>
    public void UpdateRoutingTableUniformCosts()
    {
        // First retrieve the RouterScript objects of the routers that are currently involved.
        RouterScript[] routers = uniformCostManager.GetAllRouterScripts();

        // Sort according to the current priority of the router.
        routers = sortRouterArrayByPriority(routers);

        string output = string.Empty;
        output += createHandledRouterEntriesUniformCost(routers);
        output += createUnhandledRouterEntriesUniformCost(routers);

        if (routingTableTextField)
            routingTableTextField.text = output;
    }

    /// <summary>
    /// Creates the entries for the unhandled routers in the current run for the routing table. The
    /// method is used in the Uniform Cost algorithm.
    /// </summary>
    /// <returns>The unhandled router entries for the uniform cost.</returns>
    /// <param name="routers">Routers.</param>
    private string createUnhandledRouterEntriesUniformCost(RouterScript[] routers)
    {
        System.Text.StringBuilder s = new System.Text.StringBuilder();

        foreach (var router in routers)
        {
            if (!uniformCostManager.IsRouterAlreadyHandled(router))
            {
                if (router.GetPriority() == int.MaxValue)
                {
                    // Add entry to the routing table without currently shortest path cost.
                    s.Append(string.Format("Tisch {0}: (-)" + System.Environment.NewLine,
                            router.GetRouterName()));
                }
                else
                {
                    // Add entry to the routing table with currently shortest path cost to this router.
                    s.Append(string.Format("Tisch {0}: ({1})" + System.Environment.NewLine,
                            router.GetRouterName(), router.GetPriority()));
                }
            }
        }

        string output = s.ToString();
        output = colorizeString(output, unhandledRouterColor);

        return output;
    }

    /// <summary>
    /// Creates the handled router entries uniform cost.
    /// </summary>
    /// <returns>The handled router entries uniform cost.</returns>
    /// <param name="routers">Routers.</param>
    private string createHandledRouterEntriesUniformCost(RouterScript[] routers)
    {
        System.Text.StringBuilder s = new System.Text.StringBuilder();

        foreach (var router in routers)
        {
            if (uniformCostManager.IsRouterAlreadyHandled(router))
            {
                // Add entry to the routing table with currently shortest path cost to this router.
                s.Append(string.Format("Tisch {0}: ({1})" + System.Environment.NewLine,
                        router.GetRouterName(), router.GetPriority()));
            }
        }

        string output = s.ToString();
        output = colorizeString(output, handledRouterColor);

        return output;
    }

    /// <summary>
    /// A helper method that sorts the entries of the array of RouterScript instances according
    /// to the current priority of their corresponding router. The entries are 
    /// sorted ascendingly.
    /// </summary>
    /// <param name="routers">The array of RouterScript instances that should be sorted.</param>
    /// <returns>A sorted version of the array that was passed as a parameter.</returns>
    private RouterScript[] sortRouterArrayByPriority(RouterScript[] routers)
    {
        bool isSorted = false;
        while (!isSorted)
        {
            // Assume it is sorted.
            isSorted = true;

            // Check if all elements are sorted.
            for (int i = 0; i < routers.Length - 1; i++)
            {
                // Sort by priority ascendingly.
                if (routers[i].GetPriority() > routers[i + 1].GetPriority())
                {
                    // Not sorted yet.
                    isSorted = false;

                    // Swap elements.
                    RouterScript tmp = routers[i];
                    routers[i] = routers[i + 1];
                    routers[i + 1] = tmp;
                }
            }
        }

        return routers;
    }

    /// <summary>
    /// Colorizes a string in the specified color using rich text markups.
    /// </summary>
    /// <param name="targetString">The string to be colored.</param>
    /// <param name="color">The color that should be used.</param>
    private string colorizeString(string targetString, Color color)
    {
        string output = string.Format("<color=#{0}>", ColorUtility.ToHtmlStringRGBA(color));
        output += targetString;
        output += "</color>";

        return output;
    }
}
