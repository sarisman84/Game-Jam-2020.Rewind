using UnityEngine;
using System.Collections;
using Assets.Enemy;
using System;

public class WaveManager {

    int currentWave = 0;


    public WaveManager()
    {

    }



    public void StartWave<E>(int amountOfEnemies, params E[] enemyTypes) where E : Enemy
    {
        //foreach (var enemy in enemyTypes)
        //{

        //    //Create an enemy of type enemy
        //    //LevelManager.GetInstance.SpawnEnemy(1,1, enemy);
        //}
    }
}
