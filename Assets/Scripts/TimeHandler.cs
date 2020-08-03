using Assets.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        RecordedAction action = new RecordedAction(player.transform.position, player.aimGameObject.transform.GetChild(1).position, player.aimGameObject.transform.rotation);

        //Add position and rotation of the player to a list.
        allRecordedActions.Enqueue(action);
        Debug.Log($"Recorded Action. Current Size: {allRecordedActions.Count}");
        CreatePlayerGhost(PlayerReference, action);

    }

    public void ConfirmAllEnemyDeaths()
    {
        Debug.Log($"Checking if all enemies are dead. Currently alive: {currentEnemies.FindAll(e => e.obj.activeSelf == true).Count}");
        if (currentEnemies.FindAll(e => e.obj.activeSelf == true).Count == 1)
        {

            PlayRecordedActions(PlayerReference);


        }
    }

    void CreatePlayerGhost(PlayerController player, RecordedAction action)
    {
        //Visualise said position and rotation with a model.
        GameObject playerClone = UnityEngine.Object.Instantiate(player.gameObject, replayList);
        playerClone.GetComponent<PlayerController>().enabled = false;
        playerClone.GetComponent<Collider>().enabled = false;
        playerClone.transform.position = action.playerPosition;

        playerClone.transform.GetChild(1).rotation = action.playerAimRotation;

    }

    public void PlayRecordedActions(PlayerController player)
    {
        int i = 0;
        while (allRecordedActions.Count > 0)
        {
            Debug.Log($"Playing back action: {i}");
            RecordedAction action = allRecordedActions.Dequeue();
            player.InitializeBullet(action.playerFirePosition, action.playerAimRotation);
            i++;
        }
    }


    /// <summary>
    /// Contains information for a recorded action.
    /// </summary>
    struct RecordedAction {
        public Vector3 playerPosition;
        public Quaternion playerAimRotation;
        public Vector3 playerFirePosition;
        public RecordedAction(Vector3 position, Vector3 firePosition, Quaternion rotation)
        {
            playerPosition = position;
            playerAimRotation = rotation;
            playerFirePosition = firePosition;

        }
    }
}
