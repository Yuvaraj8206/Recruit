using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class SwitchUI : MonoBehaviour
{
    public GameObject[] background;
    int index;
 
    void Start()
    {
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= 4)
            index = 4;

        if (index < 0)
            index = 0;

        if (index == 0)
        {
            background[0].gameObject.SetActive(true);
        }

    }
    public void Next()
    {
        index += 1;
        for (int i = 0; i < background.Length; i++)
        {
            background[i].gameObject.SetActive(false);
            background[index].gameObject.SetActive(true);
        }
        Debug.Log(index);
    }
    public void Previous()
    {
        index -= 1;
        for (int i = 0; i < background.Length; i++)
        {
            background[i].gameObject.SetActive(false);
            background[index].gameObject.SetActive(true);
        }
        Debug.Log(index);
    }
}
