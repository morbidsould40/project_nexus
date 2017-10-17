using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : MonoBehaviour, ISpecialAbility
    {

        AreaEffectConfig config;

        public void SetConfig(AreaEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        // Use this for initialization
        void Start()
        {
            print("AoE Attack behaviour attached to " + gameObject.name);
        }

        public void Use(AbilityUseParams useParams)
        {
            print("AoE Attack used by " + gameObject.name);
            // Static Sphere Cast for targets
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());
            foreach (RaycastHit hit in hits)
            {
                var damagable = hit.collider.gameObject.GetComponent<IDamageable>();
                if (damagable != null)
                {
                    float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();
                    damagable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
