using UnityEngine;

public class ClickOnObjectsButton : MonoBehaviour
{

    public GameObject table, barkeeper, prof, scoreBeer, beerBottle, guest, arrow, alt, navi, routing, mouse;


    void Start()
    {
        prof.SetActive(true);
    }

    public void OnButtonClick(string buttonName)
    {
        InactivateAllObjects();
        switch (buttonName)
        {
            case "ObjectTable":
                table.SetActive(true);
                break;
            case "ObjectBarkeeper":
                barkeeper.SetActive(true);
                //TODO: Einbauen von Animation
                //FindObjectOfType<PlayerController>().SetMouth(0);
                break;
            case "ObjectProfessor":
                //TODO: Einbauen von Animation
                prof.SetActive(true);
                break;
            case "ObjectScoreBeer":
                scoreBeer.SetActive(true);
                break;
            case "ObjectBeerBottle":
                beerBottle.SetActive(true);
                break;
            case "ObjectGuest":
                guest.SetActive(true);
                break;
            case "ObjectAlt":
                alt.SetActive(true);
                break;
            case "ObjectArrow":
                arrow.SetActive(true);
                break;
            case "ObjectNavi":
                navi.SetActive(true);
                break;
            case "ObjectRouting":
                routing.SetActive(true);
                break;
            case "ObjectMouse":
                mouse.SetActive(true);
                break;
        }
    }

    private void InactivateAllObjects()
    {
        table.SetActive(false);
        barkeeper.SetActive(false);
        prof.SetActive(false);
        scoreBeer.SetActive(false);
        beerBottle.SetActive(false);
        guest.SetActive(false);
        alt.SetActive(false);
        arrow.SetActive(false);
        navi.SetActive(false);
        routing.SetActive(false);
        mouse.SetActive(false);
    }
}
