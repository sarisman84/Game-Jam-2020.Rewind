
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


    public void RecordAction(PlayerController player)
    {

        RecordedAction action = new RecordedAction(player.transform.position, player.aimGameObject.transform.GetChild(1).position, player.aimGameObject.transform.rotation, Time.time);






        //Add position and rotation of the player to a list.
        allRecordedActions.AddFirst(action);

        CreatePlayerGhost(PlayerReference, action);
        //STOP CRASHING
    }
    Coroutine rewind, nextWave;
    public void ConfirmAllEnemyDeaths()
    {

        if (LevelManager.GetInstance.waveManager.areAllEnemiesDead)
        {
            if (rewind != null)
                PlayerReference.StopCoroutine(rewind);
            rewind = PlayerReference.StartCoroutine(PlayRecordedActions(PlayerReference));


        }
    }

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
        yield return null;
        areRewindsDone = new bool[allRecordedActions.Count];
        index = 0;

        yield return PlayRecordedActionsOfListElement(allRecordedActions, player, 0.5f);
        if (nextWave != null)
            player.StopCoroutine(nextWave);
        nextWave = player.StartCoroutine(LevelManager.GetInstance.waveManager.DeployNextWave());
        player.ResetPositionToSpawn();

    }

    private bool AreRewindsDone()
    {
        return false;
    }

    IEnumerator PlayRecordedActionsOfListElement(LinkedList<RecordedAction> queue, PlayerController player, float initialDelay)
    {


        LinkedListNode<RecordedAction> node = queue.First;
        float reductionAmm = 0.05f;
        EffectsManager.GetInstance.CurrentBackgroundMusic.pitch = -1;

        while (node != null)
        {



            isRewinding = true;
            yield return new WaitForSeconds(initialDelay);

            RecordedAction action = node.Value;
            player.InitializeBullet(action.playerFirePosition, action.playerAimRotation);


            node = node.Next;
            initialDelay -= reductionAmm;
            initialDelay = Mathf.Clamp(initialDelay, 0.15f, float.MaxValue);
            reductionAmm -= 0.005f;
            reductionAmm = Mathf.Clamp(reductionAmm, 0.01f, float.MaxValue);


        }
        if (areRewindsDone.Length != 0)
        {
            areRewindsDone[this.index] = true;
            this.index++;
            this.index = Mathf.Clamp(index, 0, areRewindsDone.Length - 1);
        }

        yield return null;


    }

    public void ClearRecordings()
    {
        allRecordedActions.ExecuteAction(r => UnityEngine.Object.Destroy(r.playerClone));
        allRecordedActions.Clear();
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
