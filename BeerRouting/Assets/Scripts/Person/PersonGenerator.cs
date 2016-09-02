using UnityEngine;
using System.Collections;

public class PersonGenerator : MonoBehaviour
{
    //private int randomArmAndBodyColor;
    private readonly string TAG_HAT = "PersonHat";
    private readonly string TAG_HAIR = "PersonHair";
    private readonly string TAG_TONGUE = "PersonTongue";
    private readonly string TAG_BODY = "PersonBody";
    private readonly string TAG_BEER = "PersonBeer";
    private readonly string TAG_RIGHT_HAND = "PersonRightHand";
    private readonly string TAG_RIGHT_HAND_OPEN = "PersonRightHandOpen";
    //private readonly string TAG_LEFT_ARM = "PersonLeftArm";
    private readonly string TAG_RIGHT_ARM = "PersonRightArm";


    // Initilizeses a random Person
    void Start()
    {
        //randomArmAndBodyColor = getRandomNumberBetweenRange(0, 3);
        //SetRandomHeadpiece();
        SetRandomHeadpieces();
        SetGameObjectsActive(TAG_BODY);
        //ToggleTongue();
        //ToggleBeer();
        //setAllOpenHandsInactive();
        ToggleTongues();
        ToggleBeers2();
        //ToggleBeers();
        ToggleHeadPieces();
    }

    private void setAllOpenHandsInactive()
    {
        GameObject[] openHands = GameObject.FindGameObjectsWithTag(TAG_RIGHT_HAND_OPEN);
        for (int i = 0; i < openHands.Length; i++)
        {
            openHands[i].SetActive(false);
        }
    }

    // Sets the gameobject (hat or hair) active and choses a random color
    private void SetGameObjectActive(string tag)
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag(tag);
        ArrayList list = new ArrayList();
        int numChildren = gameObject.transform.childCount;
        GameObject[] gameObjects = new GameObject[numChildren];

        foreach (Transform child in gameObject.transform)
        {
            list.Add(child.gameObject);
        }

        for (int i = 0; i < numChildren; i++)
        {
            gameObjects[i] = list[i] as GameObject;
            gameObjects[i].SetActive(false);
        }

        int randomColor = GetRandomNumberBetweenRange(0, numChildren - 1);
        gameObjects[randomColor].SetActive(true);
    }

    // Sets the gameobjects (hat or hair) active and choses a random color
    private void SetGameObjectsActive(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        for (int o = 0; o < objects.Length; o++)
        {
            ArrayList list = new ArrayList();
            GameObject gameObject = objects[o];
            int numChildren = gameObject.transform.childCount;
            GameObject[] gameObjects = new GameObject[numChildren];

            foreach (Transform child in gameObject.transform)
            {
                list.Add(child.gameObject);
            }

            for (int i = 0; i < numChildren; i++)
            {
                gameObjects[i] = list[i] as GameObject;
                gameObjects[i].SetActive(false);
            }

            int randomColor = GetRandomNumberBetweenRange(0, numChildren - 1);
            gameObjects[randomColor].SetActive(true);
            /*
            ArrayList arms = new ArrayList();
            GameObject go = gameObjects[randomColor].transform.Find("RightArm/RightArm").gameObject;
            print("--->" + go.ToString());
            
            foreach (Transform t in gameObjects[randomColor].transform.Find("RightArm/RightArm"))
            {
                print("---_>" + t.ToString());
                arms.Add(t.gameObject);
            }
            GameObject arm = (GameObject)arms[0];
            //arm.SetActive(false);
            */
            if (TAG_BODY == tag)
            {
                //ToggleBeer(o);
            }

        }

    }

    // Sets either a random hats or random hairs
    private void SetRandomHeadpieces()
    {
        SetGameObjectsActive(TAG_HAT);
        SetGameObjectsActive(TAG_HAIR);
    }

    // Sets either a random hat or ranom hair
    private void SetRandomHeadpiece()
    {
        if (1 == GetRandomNumberBetweenRange(0, 1))
        {
            SetGameObjectActive(TAG_HAT);
        }
        else
        {
            SetGameObjectActive(TAG_HAIR);
        }
    }

    // Sets the Tongue active or inaktive
    private void ToggleTongue()
    {
        GameObject tongue = GameObject.FindGameObjectWithTag(TAG_TONGUE);
        tongue.SetActive(1 == GetRandomNumberBetweenRange(0, 1));
    }

    // Sets the Tongue active or inaktive
    private void ToggleTongues()
    {
        GameObject[] tongues = GameObject.FindGameObjectsWithTag(TAG_TONGUE);
        //print("ja jetzt" + tongues.Length);
        for (int t = 0; t < tongues.Length; t++)
        {
            tongues[t].SetActive(1 == GetRandomNumberBetweenRange(0, 1));
        }

    }

    // Sets the Beers active or inaktive
    private void ToggleBeers()
    {
        GameObject[] beers = GameObject.FindGameObjectsWithTag(TAG_BEER);
        int count = 0;
        for (int b = 0; b < beers.Length; b++)
        {
            GameObject beer = beers[b];
            GameObject[] rightHands = GameObject.FindGameObjectsWithTag(TAG_RIGHT_HAND);
            GameObject[] rightHandsOpen = GameObject.FindGameObjectsWithTag(TAG_RIGHT_HAND_OPEN);
            bool boolean = 1 == GetRandomNumberBetweenRange(0, 1);
            beer.SetActive(boolean);
            //rightHands[b].SetActive(boolean);
            //rightHandsOpen[b-count].SetActive(!boolean);

            //setAllOpenHandsInactive();

            print("num beers: " + beers.Length + " num hands" + rightHands.Length + " " + rightHandsOpen.Length + " " + b + " " + (b - count));
        }
    }


    // Sets the Beers active or inaktive
    private void ToggleBeers2()
    {
        GameObject[] rightArms = GameObject.FindGameObjectsWithTag(TAG_RIGHT_ARM);


        for (int i = 0; i < rightArms.Length; i++)
        {
            ArrayList arms = new ArrayList();
            foreach (Transform t in rightArms[i].transform)
            {
                arms.Add(t.gameObject);
            }
            if (GetRandomNumberBetweenRange(0, 1) > 0)
            {
                ((GameObject)arms[0]).SetActive(true);
                ((GameObject)arms[1]).SetActive(true);
                ((GameObject)arms[2]).SetActive(false);
            }
            else
            {
                ((GameObject)arms[0]).SetActive(false);
                ((GameObject)arms[1]).SetActive(false);
                ((GameObject)arms[2]).SetActive(true);
            }

        }
    }

    private void ToggleHeadPieces()
    {
        GameObject[] hairs = GameObject.FindGameObjectsWithTag(TAG_HAIR);
        GameObject[] hat = GameObject.FindGameObjectsWithTag(TAG_HAT);
        for (int i = 0; i < hairs.Length; i++)
        {
            hairs[i].SetActive(GetRandomNumberBetweenRange(0, 1) == 1);
        }
        for (int i = 0; i < hat.Length; i++)
        {
            hat[i].SetActive(GetRandomNumberBetweenRange(0, 1) == 1);
        }
    }

    // Sets the Beer active or inaktive
    private void ToggleBeer()
    {
        GameObject beer = GameObject.FindGameObjectWithTag(TAG_BEER);
        GameObject rightHand = GameObject.FindGameObjectWithTag(TAG_RIGHT_HAND);
        GameObject rightHandOpen = GameObject.FindGameObjectWithTag(TAG_RIGHT_HAND_OPEN);
        bool boolean = 1 == GetRandomNumberBetweenRange(0, 1);
        beer.SetActive(boolean);
        rightHand.SetActive(boolean);
        rightHandOpen.SetActive(!boolean);
    }

    // Sets the Beer active or inaktive
    private void ToggleBeer(int id)
    {
        GameObject[] beers = GameObject.FindGameObjectsWithTag(TAG_BEER);
        GameObject[] rightHands = GameObject.FindGameObjectsWithTag(TAG_RIGHT_HAND);
        GameObject[] rightHandsOpen = GameObject.FindGameObjectsWithTag(TAG_RIGHT_HAND_OPEN);
        bool boolean = 1 == GetRandomNumberBetweenRange(0, 1);
        beers[id].SetActive(boolean);
        rightHands[id].SetActive(boolean);
        rightHandsOpen[id].SetActive(!boolean);
    }

    // Generates a random Number between the given range where start and end are inclusive
    private int GetRandomNumberBetweenRange(int start, int end)
    {
        int r = Random.Range(start, end + 1); // +1 that end is inclusive
        return r;
    }



    /*
    private int CountArmAndBodyColor()
    {
    int count = 0;
    GameObject arm = GameObject.FindGameObjectWithTag(TAG_LEFT_ARM);
    return count;
    }
    */

}
