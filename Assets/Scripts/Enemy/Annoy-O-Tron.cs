using UnityEngine;

public class Annoy_O_Tron : Enemy
{
    public Annoy_O_Tron(string modelPath = "Annoy-O-Tron") : base("Annoy-O-Tron")
    {
        
    }

    public override void StartEvent(EnemyBehaviour obj)
    {        
        obj.overrideUpdate = true;
        obj.enemySpeed = 10;
        obj.accelerationRate = 15;
    }

    public override void UpdateEvent(EnemyBehaviour obj)
    {
        int distance = 10;

        Vector3 player = obj.foundPlayer.transform.position;
        Vector3 enemy = obj.transform.position;
        Vector3 targetLocation = new Vector3(player.x + distance, 0, player.z + distance);
        if (player.x > enemy.x)
            targetLocation.x = player.x - distance;
        if (player.z > enemy.z)
            targetLocation.z = player.z - distance;
             
        obj.agent.SetDestination(targetLocation);


    }
}


