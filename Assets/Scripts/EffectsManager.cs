using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {
    private static EffectsManager instance;
    [SerializeField]
    List<AudioFile> listOfAudioFiles = new List<AudioFile>();
    [SerializeField]
    List<ParticleEffect> listOfParticleEffects = new List<ParticleEffect>();

    public static EffectsManager GetInstance
    {
        get
        {
            instance = instance ?? FindObjectOfType<EffectsManager>() ?? new GameObject("Effects Manager").AddComponent<EffectsManager>();
            return instance;
        }
    }

    public List<AudioFile> CurrentAudioFiles => listOfAudioFiles;
    public List<ParticleEffect> CurrentParticleEffects => listOfParticleEffects;

    public AudioSource CurrentBackgroundMusic { get; internal set; }

    // Start is called before the first frame update
    private void Awake()
    {
        CurrentBackgroundMusic = GetComponent<AudioSource>();
       listOfAudioFiles.ExecuteAction(a => a.CreateAudioSource(gameObject));
        listOfParticleEffects.ExecuteAction(b => b.CreateParticleSystem(gameObject));
       

    }

    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]
public class AudioFile {

    public string name { get => file.name; }

    public AudioClip file;

    public bool loop, playOnAwake;

    public float pitch, volume;
    public int priority;

    AudioSource playerRef;
    public AudioSource Player => playerRef;

    public void CreateAudioSource(GameObject owner)
    {
        playerRef = owner.AddComponent<AudioSource>();
        playerRef.clip = file;
        playerRef.loop = loop;
        playerRef.playOnAwake = playOnAwake;

        playerRef.pitch = pitch;
        playerRef.volume = volume;
        playerRef.priority = priority;
    }

    public void Play()
    {
        playerRef.Play();
    }

    public void Stop()
    {
        playerRef.Stop();
    }
}


[System.Serializable]
public class ParticleEffect {


    public string particleName;
    public ParticleSystem prefab;

    public void CreateParticleSystem(GameObject owner)
    {
        prefab = UnityEngine.Object.Instantiate(prefab, owner.transform);



    }


    public void PlayEffect(Vector3 position)
    {
        if (prefab.isEmitting)
        {

            ParticleSystem particleSystem = ObjectPooler.GetPooledObject<ParticleSystem>(prefab.gameObject);
            particleSystem.gameObject.SetActive(true);
            particleSystem.transform.position = position;
            particleSystem.Play();


            return;
        }


        prefab.transform.position = position;
        prefab.Play();
    }



}