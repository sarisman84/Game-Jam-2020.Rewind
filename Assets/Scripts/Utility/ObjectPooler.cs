using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

public static class ObjectPooler {

    static Dictionary<int, List<GameObject>> dictionaryOfPooledObjects = new Dictionary<int, List<GameObject>>();

    /// <summary>
    /// Adds a gameObject to a pool of gameObjects for later use.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefab">The object in question to add into the pool.</param>
    /// <param name="amount">The amount of objects created that is going to be added into the pool.</param>
    public static void PoolGameObject<T>(T prefab, int amount) where T : MonoBehaviour
    {
        if (prefab == null) return;
        List<GameObject> poolofObjects = new List<GameObject>();

        Transform parent = new GameObject($"{prefab.gameObject.name}'s list").transform;
        if (dictionaryOfPooledObjects.ContainsKey(prefab.GetInstanceID()))
        {

            poolofObjects = dictionaryOfPooledObjects[prefab.GetInstanceID()];
            parent = poolofObjects[0].transform.parent;
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Object.Instantiate(prefab.gameObject, parent);
            obj.gameObject.SetActive(false);
            poolofObjects.Add(obj.gameObject);

        }

        if (dictionaryOfPooledObjects.ContainsKey(prefab.GetInstanceID()))
        {
            dictionaryOfPooledObjects[prefab.GetInstanceID()] = poolofObjects;
        }
        else
        {
            dictionaryOfPooledObjects.Add(prefab.GetInstanceID(), poolofObjects);
        }

    }

    public static void PoolGameObject(GameObject prefab, int amount)
    {

        List<GameObject> poolofObjects = new List<GameObject>();

        if (prefab == null) return;

        Transform parent = new GameObject($"{prefab.gameObject.name}'s list").transform;
        if (dictionaryOfPooledObjects.ContainsKey(prefab.GetInstanceID()))
        {

            poolofObjects = dictionaryOfPooledObjects[prefab.GetInstanceID()];
            parent = poolofObjects[0].transform.parent;
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Object.Instantiate(prefab.gameObject, parent);
            obj.gameObject.SetActive(false);
            poolofObjects.Add(obj.gameObject);

        }

        if (dictionaryOfPooledObjects.ContainsKey(prefab.GetInstanceID()))
        {
            dictionaryOfPooledObjects[prefab.GetInstanceID()] = poolofObjects;
        }
        else
        {
            dictionaryOfPooledObjects.Add(prefab.GetInstanceID(), poolofObjects);
        }

    }

    /// <summary>
    /// Gets the first inactive gameObject from a pool of gameObjects by searching using a component Type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An inactive gameObject. If all objects in a pool are used, it creates a new active gameObject and adds it into the pool.</returns>
    public static T GetPooledObject<T>(int prefabID = 0)
    {
        foreach (KeyValuePair<int, List<GameObject>> item in dictionaryOfPooledObjects)
        {
            bool isValid = true;
            if (prefabID != 0)
                isValid = item.Key == prefabID;


            if (item.Value[0].GetComponent<T>() != null && isValid)
            {
                foreach (GameObject element in item.Value)
                {
                    if (!element.activeSelf)
                        return element.gameObject.GetComponent<T>();
                }

                GameObject obj = Object.Instantiate(item.Value[0].gameObject, item.Value[0].gameObject.transform.parent);
                item.Value.Add(obj);

                return obj.GetComponent<T>();

            }
        }
        return default(T);
    }





    public static T GetPooledObject<T>(T prefab) where T : MonoBehaviour
    {
        T result = GetPooledObject<T>();
        if (result != null)
        {
            return result;
        }
        PoolGameObject(prefab, 300);
        return GetPooledObject<T>();
    }


    public static T GetPooledObject<T>(GameObject prefab)
    {
        T result = GetPooledObject<T>(prefab.GetInstanceID());
        if (result != null)
        {
            return result;
        }
        PoolGameObject(prefab, 300);
        return GetPooledObject<T>(prefab.GetInstanceID());
    }


    /// <summary>
    ///  Gets the first inactive gameObject from a pool of gameObjects by searching using a prefab instance ID.
    /// </summary>
    /// <param name="prefabID">The Instance id of the original gameObject that was used to pool its copies.</param>
    /// <returns>An inactive gameObject. If all objects in a pool are used, it creates a new active gameObject and adds it into the pool.</returns>
    public static GameObject GetPooledObject(int prefabID)
    {
        foreach (KeyValuePair<int, List<GameObject>> item in dictionaryOfPooledObjects)
        {
            if (item.Key == prefabID)
            {
                foreach (GameObject element in item.Value)
                {
                    if (!element.activeSelf)
                        return element.gameObject;
                }

                GameObject obj = Object.Instantiate(item.Value[0].gameObject, item.Value[0].gameObject.transform.parent);
                item.Value.Add(obj);

                return obj;

            }
        }
        return null;
    }
}