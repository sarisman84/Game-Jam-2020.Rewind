using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

public static class ObjectPooler {

    static Dictionary<Type, List<GameObject>> dictionaryOfPooledObjects = new Dictionary<Type, List<GameObject>> ();

    /// <summary>
    /// Adds a gameObject to a pool of gameObjects for later use.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefab">The object in question to add into the pool.</param>
    /// <param name="amount">The amount of objects created that is going to be added into the pool.</param>
    public static void PoolGameObject<T> (T prefab, int amount) {

        List<GameObject> poolofObjects = new List<GameObject> ();

        Transform parent = new GameObject ($"{(prefab as MonoBehaviour).gameObject.name}'s list").transform;

        if (dictionaryOfPooledObjects.ContainsKey (prefab.GetType ())) {

            poolofObjects = dictionaryOfPooledObjects[prefab.GetType ()];
            parent = poolofObjects[0].transform.parent;
        }

        for (int i = 0; i < amount; i++) {

            GameObject obj = Object.Instantiate ((prefab as MonoBehaviour).gameObject, parent);
            obj.gameObject.SetActive (false);
            poolofObjects.Add (obj.gameObject);

        }

        if (dictionaryOfPooledObjects.ContainsKey (prefab.GetType ())) {
            dictionaryOfPooledObjects[prefab.GetType ()] = poolofObjects;
        } else {
            dictionaryOfPooledObjects.Add (prefab.GetType (), poolofObjects);
        }

    }

    /// <summary>
    /// Gets the first inactive gameObject from a pool of gameObjects by searching using a component Type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An inactive gameObject. If all objects in a pool are used, it creates a new active gameObject and adds it into the pool.</returns>
    public static T GetPoolObject<T> () {
        foreach (KeyValuePair<Type, List<GameObject>> item in dictionaryOfPooledObjects) {
            if (item.Key == typeof(T)) {
                foreach (GameObject element in item.Value) {
                    if (!element.activeSelf)
                        return element.gameObject.GetComponent<T>();
                }

                GameObject obj = Object.Instantiate (item.Value[0].gameObject, item.Value[0].gameObject.transform.parent);
                item.Value.Add (obj);

                return obj.GetComponent<T>();

            }
        }
        return default(T);
    }

}