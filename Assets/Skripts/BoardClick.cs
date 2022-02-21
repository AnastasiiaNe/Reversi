using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardClick : MonoBehaviour
{
    public Transform tokenOgj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        Debug.Log(Input.mousePosition);
        Instantiate(tokenOgj, new Vector3((int)(Input.mousePosition.x / 64) - 0.5f, (int)(Input.mousePosition.y / 64) - 8.5f, 0), tokenOgj.rotation);
    }
}
