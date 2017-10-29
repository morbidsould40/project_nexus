﻿using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : AbilityBehaviour
    {
        AreaEffectConfig config;

        public void SetConfig(AreaEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        public override void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            var particlePrefab = config.GetParticlePrefab();
            var prefab = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
            // TODO Decide if particle system attaches to player
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration);
        }

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            // Static Sphere Cast for targets
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());
            foreach (RaycastHit hit in hits)
            {
                var damagable = hit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damagable != null && !hitPlayer)
                {
                    float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();
                    damagable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
