using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardClick : MonoBehaviour
{
    public Transform tokenOgj;

    private void OnMouseDown()
    {
        Debug.Log(Input.mousePosition);
        Instantiate(tokenOgj, new Vector3((int)(Input.mousePosition.x / 64) - 0.5f, (int)(Input.mousePosition.y / 64) - 8.5f, 0), tokenOgj.rotation);
    }
}
