using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimation : MonoBehaviour {

    public AnimationObject[] gameObjects;
    // Start is called before the first frame update
    void Awake()
    {
        if (gameObjects.Length == 0) throw new NullReferenceException("List is empty");
    }


    private void Update()
    {

        foreach (var item in gameObjects)
        {
            item.gameObject.transform.rotation *= Quaternion.Euler(item.rotationDirection);
        }

    }

    [System.Serializable]
    public class AnimationObject {
        public GameObject gameObject;
        public Vector3 rotationDirection;
    }

}
