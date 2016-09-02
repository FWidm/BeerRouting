using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
//this compiler flag enables unity to execute the update method everytime something changes
[ExecuteInEditMode]
public class PathDisplayScript : MonoBehaviour
{
    public bool debugging = false;
    // is used to display a path between two routers while in the editor.
    public Material editorDisplayMaterial;
    public Color colorFrom;
    public Color colorTo;
    private bool active = false;
    private PathScript pathScript;

    // Use this for initialization
    #if UNITY_EDITOR
    void Start()
    {
        if (EditorApplication.isPlaying)
            return;
        pathScript = GetComponent<PathScript>();
        if (pathScript != null)
            active = true;
        DeleteAllRenderers();
    }

    #endif
    #if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (EditorApplication.isPlaying)
            return;


        if (active)
        {
            pathScript.displayDestinationRouter();

            //        HideLines();
            if (this.transform.GetComponentsInChildren<LineRenderer>(true).Length == 0)
            {
                GameObject displayPathInEditor = new GameObject("LineRenderer " + name, typeof(LineRenderer));
                displayPathInEditor.transform.parent = this.transform;

                LineRenderer lr = displayPathInEditor.GetComponent<LineRenderer>();
                Vector3[] pos = { pathScript.to.transform.position, pathScript.from.transform.position };
                lr.SetPositions(pos);
                lr.SetColors(colorFrom, colorTo);
                lr.SetWidth(0, .5f);
                lr.enabled = false;

                lr.material = editorDisplayMaterial;
                if (debugging)
                    Debug.Log("lr=" + lr);
            }
            else
            {
                LineRenderer lr = GetComponentInChildren<LineRenderer>(true);

                if (lr != null)
                {
//                    if (Time.time % 60 == 0)
//                    {
//                        Debug.Log("Time=" + Time.time);
//                        DestroyImmediate(lr.gameObject);
//                        return;
//                    }
                        
                    
                    lr.enabled = true;
                    if (debugging)
                        Debug.Log(lr.transform.name + " | " + lr.enabled);
                    lr.SetWidth(0, .5f);

                    lr.SetColors(colorFrom, colorTo);
                }
            }
        }
    }
    #endif

    void DeleteAllRenderers()
    {
//        Debug.Log("Delete all Linerenderes called!");
        LineRenderer[] lrs = GetComponentsInChildren<LineRenderer>(true);
        foreach (LineRenderer lr in lrs)
        {
            DestroyImmediate(lr.gameObject);
        }
    }

}
