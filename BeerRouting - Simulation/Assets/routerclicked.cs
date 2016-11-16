using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class routerclicked : MonoBehaviour
{
    public Texture2D texture;
    private bool show = true;
    PathScript[] pathScripts;
    SpriteRenderer sr;

    void Start()
    {
        pathScripts = FindObjectsOfType<PathScript>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 goPos = gameObject.transform.position;
        goPos.y += .3f;
        Debug.Log(">> mouseclicked on router=" + this.name + " show=" + show + " dist=" + Vector2.Distance(goPos, mouse));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //this works, as clicking on a router doesnt find an object that is hit. Don't know why.
        if (!Physics.Raycast(ray, out hit, 200))
        {
            Cursor.SetCursor(texture, new Vector2(texture.width / 2, texture.height / 2), CursorMode.Auto);
            ShowAllPaths(true);
            sr.color = Color.red;
        }

    }

    void ShowAllPaths(bool show)
    {
        foreach (var path in pathScripts)
        {
            path.PathHighlight = show;
        }
    }


    void OnMouseUp()
    {
        StartCoroutine(ExecuteAfterTime(1.5f));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Cursor.SetCursor(null, new Vector2(), CursorMode.Auto);
        ShowAllPaths(false);
        sr.color = Color.white;
    }
}
