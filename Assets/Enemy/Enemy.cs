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
        Vector2Int currentPos;
        public Enemy(Vector2Int index, string resourcePrefabPath)
        {


            GameObject newPill = Object.Object.Instantiate(Resources.Load<GameObject>($"Enemies/{resourcePrefabPath}"));

            EnemyBehaviour enemy = newPill.AddComponent<EnemyBehaviour>();
            enemy.onDamageEvent += DamageEvent;
            enemy.onStartEvent += StartEvent;
            enemy.onUpdateEvent += UpdateEvent;

            newPill.transform.position = LevelManager.GetInstance.PlayArea[index.x, index.y].GetWorldPosition(newPill);
            obj = newPill;
            LevelManager.GetInstance.PlayArea[index.x, index.y].entity = newPill;

            currentPos = index;


            
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
            LevelManager.GetInstance.PlayArea[currentPos.x, currentPos.y].RemoveEntity();

            obj.gameObject.SetActive(false);
            obj.onDamageEvent -= DamageEvent;


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

        public GameObject obj;
    }
}
