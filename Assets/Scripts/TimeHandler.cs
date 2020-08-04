using Assets.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class TimeHandler {

    Queue<RecordedAction> allRecordedActions = new Queue<RecordedAction>();

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


    Transform replayList;

    public List<Enemy> currentEnemies { private get; set; }

    public PlayerController PlayerReference { private get; set; }

    public TimeHandler()
    {
        replayList = new GameObject("Recorded Actions").transform;
        replayList.SetParent(LevelManager.GetInstance.transform);
    }


    public void RecordAction(PlayerController player)
    {
        RecordedAction action = new RecordedAction(player.transform.position, player.aimGameObject.transform.GetChild(1).position, player.aimGameObject.transform.rotation, Time.time);

        //The queue is "full"
        //Dequeue()

        if (allRecordedActions.Count == 3)
        {
            RecordedAction oldAction = allRecordedActions.Dequeue();
            oldAction.playerClone.SetActive(false);
        }


        //Add position and rotation of the player to a list.
        allRecordedActions.Enqueue(action);

        CreatePlayerGhost(PlayerReference, action);

    }

    public void ConfirmAllEnemyDeaths()
    {

        if (currentEnemies.FindAll(e => e.obj.activeSelf == true).Count == 1)
        {

            PlayerReference.StartCoroutine(PlayRecordedActions(PlayerReference));


        }
    }

    void CreatePlayerGhost(PlayerController player, RecordedAction action)
    {
        //Visualise said position and rotation with a model.
        action.playerClone = UnityEngine.Object.Instantiate(player.gameObject, replayList);
        action.playerClone.GetComponent<PlayerController>().enabled = false;
        action.playerClone.GetComponent<Collider>().enabled = false;
        action.playerClone.transform.position = action.playerPosition;

        action.playerClone.transform.GetChild(1).rotation = action.playerAimRotation;


        

    }

    public IEnumerator PlayRecordedActions(PlayerController player)
    {

        int index = 0;
        int count = allRecordedActions.Count;
        float previousTimeSinceFired = Time.time, delay;

        while (index < count)
        {
            //Dequeue
            RecordedAction action = allRecordedActions.Dequeue();
            delay = Mathf.Abs(previousTimeSinceFired - action.timeSinceFired);
            player.InitializeBullet(action.playerFirePosition, action.playerAimRotation);
            index++;
            yield return new WaitForSeconds(Mathf.Min(delay, 5f));
            previousTimeSinceFired = action.timeSinceFired;
            allRecordedActions.Enqueue(action);
            Debug.Log($"Current Delay: {delay}");

        }
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


            Debug.Log($"Time of firing: {timeSinceFired}");

        }
    }
}
