using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extension {


    public static IEnumerable<T> ExecuteAction<T>(this IEnumerable<T> list, params Action<T>[] method)
    {
        foreach (var item in list)
        {
            foreach (var executeMethod in method)
            {
                executeMethod?.Invoke(item);
            }
        }
        return list;
    }

    public static IEnumerator ExecuteAction<T>(this IEnumerable<T> list, params Func<T, IEnumerator>[] method)
    {
        foreach (var item in list)
        {
            foreach (var executeMethod in method)
            {
                yield return executeMethod(item);
            }
        }

    }

    public static Vector2Int ToVector2Int(this Vector3 vector3)
    {
        return new Vector2Int((int)vector3.x, (int)vector3.y);
    }

    public static bool IsWithinRadiusOf(this Vector3 pos, Vector3 origin, float radius)
    {

        Bounds bounds = new Bounds();
        bounds.center = origin;
        bounds.size = new Vector3(radius, radius, radius);
        return bounds.Contains(pos);
    }
}

