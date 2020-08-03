using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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







    public void RecordAction(PlayerController player)
    {
        allRecordedActions.Enqueue(new RecordedAction(player.transform));
    }

    public void PlayRecordedActions()
    {
        for (int i = 0; i < allRecordedActions.Count; i++)
        {
            RecordedAction action = allRecordedActions.Dequeue();

            GameObject playerClone = Object.Instantiate(Resources.Load<GameObject>("Model/Componentless_Player"));
            playerClone.transform.position = action.transform.position;
            playerClone.transform.rotation = action.transform.rotation;
        }
    }


    /// <summary>
    /// Contains information for a recorded action.
    /// </summary>
    struct RecordedAction {
        public Transform transform;

        public RecordedAction(Transform transform)
        {
            this.transform = transform;
        }
    }
}
