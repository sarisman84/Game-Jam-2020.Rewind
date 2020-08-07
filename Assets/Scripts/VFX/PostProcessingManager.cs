using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour {
    public Volume timeRewindPP;
    BoxCollider col;
    static PostProcessingManager ins;
    public static PostProcessingManager GetInstance
    {
        get
        {
            ins = ins ?? GameObject.FindObjectOfType<PostProcessingManager>();
            return ins;
        }
    }
    void Awake()
    {
        col = GetComponent<BoxCollider>();
    }
    public void EnableTimeRewindPP()
    {
        StartCoroutine(LerpSize(200, 3.2f));


    }

    private IEnumerator LerpSize(float target, float rate)
    {
        Vector3 size = col.size;
        while (size.y != target)
        {
            size.y = Mathf.Lerp(size.y, target, rate * Time.deltaTime);
            col.size = size;
            yield return new WaitForEndOfFrame();
        }
       
    }

    public void DisableTimeRewindPP()
    {
        StartCoroutine(LerpSize(1, 3.2f));
    }
}
