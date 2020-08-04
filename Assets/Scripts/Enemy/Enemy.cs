using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LevelManager;
using Object = UnityEngine;
using Westwind.Scripting;

namespace Assets.Enemy {
    public class Enemy {
        public Vector2Int spawnIndex;
        
        string modelPath;
        public Enemy(string modelPath)
        {
            this.modelPath = modelPath;
            
        }

        public EnemyBehaviour SpawnEnemy(Vector3 spawnPos, Vector2Int index)
        {
            EnemyBehaviour enemy = ObjectPooler.GetPooledObject(Resources.Load<EnemyBehaviour>($"Enemies/{modelPath}"));

            enemy.parentClass = this;
            enemy.AssignEvents(this);
            enemy.spawnPos = spawnPos;
            enemy.transform.position = spawnPos;
            enemy.gameObject.SetActive(true);
            LevelManager.GetInstance.PlayArea[spawnIndex.x, spawnIndex.y].entity = enemy.gameObject;






        
            spawnIndex = index;
            return enemy;
        }

       

        public virtual void UpdateEvent(EnemyBehaviour obj)
        {

        }

        public virtual void StartEvent(EnemyBehaviour obj)
        {


        }

        public virtual void DamageEvent(EnemyBehaviour obj)
        {

            TimeHandler.GetInstance.ConfirmAllEnemyDeaths();


            obj.gameObject.SetActive(false);
            obj.onDamageEvent -= DamageEvent;
            obj.onStartEvent -= StartEvent;
            obj.onUpdateEvent -= UpdateEvent;



        }


        public void Test()
        {
            var script = new CSharpScriptExecution()
            {
                SaveGeneratedCode = true,
                CompilerMode = ScriptCompilerModes.Roslyn
            };
            script.AddDefaultReferencesAndNamespaces();

            script.AddNamespace("UnityEngine");
            //script.AddAssembly("Westwind.Utilities.dll");
            //script.AddNamespace("Westwind.Utilities");

            var code = $"Debug.Log({"Hello World!"})";

            string result = script.ExecuteCode(code) as string;

            if (script.Error)
                Debug.LogWarning(script.ErrorMessage);
        }


    }
}
