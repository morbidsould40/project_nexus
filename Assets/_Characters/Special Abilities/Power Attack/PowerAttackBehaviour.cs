using System;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        AudioSource audioSource = null;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
            PlayAudio();
        }

        private void PlayAudio()
        {
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }

        private void DealDamage(AbilityUseParams useParams)
        {
            float damageToDeal = ((useParams.baseDamage + useParams.weaponDamage) * (config as PowerAttackConfig).GetExtraDamage());
            useParams.target.TakeDamage(damageToDeal);
        }
    }
}

