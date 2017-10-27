using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Abilities
{
    public class Fireball : MonoBehaviour
    {

        public Rigidbody rgbd;
        public GameObject fieryParticle;
        public GameObject smokeParticle;
        public GameObject explosionParticle;
        public float projectileSpeed;

        float damageCaused;

        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        void Update()
        {

        }

        void OnTriggerEnter(Collider collider)
        {
            Component damageableComponent = collider.gameObject.GetComponent(typeof(IDamageable));
            if (damageableComponent)
            {
                (damageableComponent as IDamageable).TakeDamage(damageCaused);
            }

            fieryParticle.SetActive(false);
            smokeParticle.SetActive(false);
            explosionParticle.SetActive(true);
            rgbd.constraints = RigidbodyConstraints.FreezeAll;
            Destroy(gameObject.GetComponent(typeof(Collider)));
            Destroy(gameObject, 2);
        }
    }
}

