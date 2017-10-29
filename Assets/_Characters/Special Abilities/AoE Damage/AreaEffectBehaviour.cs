using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : AbilityBehaviour
    {
        AudioSource audioSource = null;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
            PlayAudio();
        }

        private void PlayAudio()
        {
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            // Static Sphere Cast for targets
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, (config as AreaEffectConfig).GetRadius(),
                Vector3.up, (config as AreaEffectConfig).GetRadius());
            foreach (RaycastHit hit in hits)
            {
                var damagable = hit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damagable != null && !hitPlayer)
                {
                    float damageToDeal = useParams.baseDamage + (config as AreaEffectConfig).GetDamageToEachTarget();
                    damagable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
