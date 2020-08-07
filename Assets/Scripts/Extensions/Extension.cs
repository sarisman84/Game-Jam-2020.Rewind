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



    public static void PlayAudioClip(this List<AudioFile> list, string name, params Action<AudioFile>[] customEffects)
    {
        AudioFile file = list.Find(f => f.name.Contains(name));
        if (file == null) return;
        foreach (var item in customEffects)
        {
            item?.Invoke(file);
        }
        file.Play();
    }


    public static void PlayParticleEffectAt(this List<ParticleEffect> list, string name, Vector3 position, bool follow = false, params Action<ParticleEffect>[] customEffects)
    {
        ParticleEffect effect = list.Find(f => f.particleName.Contains(name));
        if (effect == null) return;
        foreach (var item in customEffects)
        {
            item?.Invoke(effect);
        }
        effect.PlayEffect(position, follow);
    }


    public static bool ContainsOnlyComponent<T>(this GameObject obj)
    {
        Component[] components = obj.GetComponents<Component>();
        return Array.Find(components, p => p is T t) != null && components.Length == 2;
    }

    public static float CountTime(this float value, float timeLimit)
    {
        value += Time.deltaTime;
        value = Mathf.Clamp(value, 0, timeLimit);

        return value;
    }
}

