using Revert_2.Script.Hostile;
using Spyro.Optimisation.ObjectManagement;
using TMPro;
using UnityEngine;

namespace Revert_2.Script.Player
{
    public class WeaponHandler
    {
        private GameObject owner;

        public WeaponHandler(string prefabPath, GameObject obj)
        {
            owner = obj;
            ObjectManager.PoolGameObject(Resources.Load<BulletBehaviour>(prefabPath).gameObject, 200);
        }


        private float localTimer = 0;

        public void FireWeapon(float delay, Transform barrel)
        {
            localTimer += Time.deltaTime;
            localTimer = Mathf.Clamp(localTimer, 0, delay);

            if (localTimer.Equals(delay))
            {
                BulletBehaviour bullet = ObjectManager.DynamicInstantiate<BulletBehaviour>();

                bullet.Setup(new BulletBehaviour.BulletInfo(5f, 10f, 1, owner.GetInstanceID()), barrel.GetChild(0).position,
                    barrel.rotation);
                bullet.gameObject.SetActive(true);
                localTimer = 0;
            }

            Debug.Log("Attempted to spawn bullet");
        }
    }
}