
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class TimeHandler {

    LinkedList<RecordedAction> allRecordedActions = new LinkedList<RecordedAction>();
    public int latestList { get; set; } = 0;

    //Singleton pattern
    static TimeHandler instance;
    public static TimeHandler GetInstance
    {
        get
        {
            instance = instance ?? new TimeHandler();
            return instance;
        }
    }


    public bool isRewinding { get; set; }

    Transform replayList;



    public PlayerController PlayerReference { private get; set; }

    public TimeHandler()
    {
        replayList = new GameObject("Recorded Actions").transform;

    }

    bool isCleaningRecordings;

    public void RecordAction(PlayerController player)
    {
        if (isCleaningRecordings) return;
        RecordedAction action = new RecordedAction(player.transform.position, player.aimGameObject.transform.GetChild(1).position, player.aimGameObject.transform.rotation, Time.time);






        //Add position and rotation of the player to a list.
        allRecordedActions.AddFirst(action);

        CreatePlayerGhost(PlayerReference, action);
        //STOP CRASHING
    }
    Coroutine rewind;

    LevelManager levelManagerRef;


    public bool AttemptToRewind(LevelManager levelManager)
    {
        levelManagerRef = levelManager;
        if (PlayerReference.gameObject.activeSelf)
            if (countdownUntilRewind == MAXCOUNTDOWN && allRecordedActions.Count != 0)
            {
                if (rewind != null)
                    levelManager.StopCoroutine(rewind);
                rewind = levelManager.StartCoroutine(PlayRecordedActions(PlayerReference));

                countdownUntilRewind = 0;
                return true;
            }


        return false;
    }

    public float countdownUntilRewind;

    public const int MAXCOUNTDOWN = 5;

    void CreatePlayerGhost(PlayerController player, RecordedAction action)
    {
        //Visualise said position and rotation with a model.
        action.playerClone = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Player/Rewind Player Clone"), replayList);

        //action.playerClone.GetComponent<PlayerController>().enabled = false;
        //action.playerClone.GetComponents<Collider>().ExecuteAction(c => c.enabled = false);

        action.playerClone.transform.position = action.playerPosition;
        action.playerClone.transform.GetChild(0).rotation = action.playerAimRotation;




    }
    int index = 0;
    bool[] areRewindsDone;
    public IEnumerator PlayRecordedActions(PlayerController player)
    {

        areRewindsDone = new bool[allRecordedActions.Count];
        index = 0;

        yield return PlayRecordedActionsOfListElement(allRecordedActions, player, 0.5f);
        StopRewindEffects();

        countdownUntilRewind = 0;
        levelManagerRef.hasAlreadyRewinded = false;


    }

    private void StopRewindEffects()
    {
        EffectsManager.GetInstance.CurrentBackgroundMusic.time = 0;
        EffectsManager.GetInstance.CurrentBackgroundMusic.pitch = 1;
        PostProcessingManager.GetInstance.DisableTimeRewindPP();
    }

    IEnumerator PlayRecordedActionsOfListElement(LinkedList<RecordedAction> queue, PlayerController player, float initialDelay)
    {


        LinkedListNode<RecordedAction> node = queue.First;

        PostProcessingManager.GetInstance.EnableTimeRewindPP();
        EffectsManager.GetInstance.CurrentBackgroundMusic.pitch = -1;

        while (node != null)
        {



            isRewinding = true;
            yield return new WaitForSeconds(initialDelay);

            RecordedAction action = node.Value;
            BulletBehaivour.InitializeBullet(PlayerReference.gameObject, action.playerFirePosition, action.playerAimRotation);


            node = node.Next;

            initialDelay = Mathf.Clamp(0.15f / queue.Count, Mathf.Max(0.1f, 0.15f / queue.Count), float.MaxValue);



        }
        if (areRewindsDone.Length != 0)
        {
            areRewindsDone[this.index] = true;
            this.index++;
            this.index = Mathf.Clamp(index, 0, areRewindsDone.Length - 1);
        }
        isRewinding = false;
        yield return null;


    }

    public void ClearRecordings()
    {
        allRecordedActions.ExecuteAction(r => UnityEngine.Object.Destroy(r.playerClone));
        allRecordedActions.Clear();
    }

    public IEnumerator DelayedClearRecordings()
    {
        levelManagerRef.hasAlreadyRewinded = true;
        levelManagerRef.StopCoroutine(rewind);
        StopRewindEffects();
        isCleaningRecordings = true;
        yield return new WaitForEndOfFrame();
        IEnumerator RemoveRecording(RecordedAction r)
        {
            UnityEngine.Object.Destroy(r.playerClone);
            EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("BulletDeath", r.playerClone.transform.position);
            yield return new WaitForSeconds(0.15f / allRecordedActions.Count);
        }

        yield return allRecordedActions.ExecuteAction(RemoveRecording);
        allRecordedActions.Clear();
        levelManagerRef.hasAlreadyRewinded = false;
        isCleaningRecordings = false;
    }



    /// <summary>
    /// Contains information for a recorded action.
    /// </summary>
    class RecordedAction {
        public Vector3 playerPosition;
        public Quaternion playerAimRotation;
        public Vector3 playerFirePosition;
        public float timeSinceFired;

        public GameObject playerClone;
        public RecordedAction(Vector3 position, Vector3 firePosition, Quaternion rotation, float _timeSinceFired)
        {
            playerPosition = position;
            playerAimRotation = rotation;
            playerFirePosition = firePosition;
            timeSinceFired = _timeSinceFired;




        }
    }
}
