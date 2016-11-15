﻿using UnityEngine;
using System.Collections;

public class routerclicked : MonoBehaviour {
    public Texture2D texture;
    
    void OnMouseDown()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("mouseclicked on router=" + this.name + " pos=" + mouse);

        Cursor.SetCursor(texture, new Vector2(texture.width/2, texture.height/2), CursorMode.Auto);
    }

    void OnMouseUp()
    {
        StartCoroutine(ExecuteAfterTime(1.5f));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Cursor.SetCursor(null, new Vector2(), CursorMode.Auto);

    }
}