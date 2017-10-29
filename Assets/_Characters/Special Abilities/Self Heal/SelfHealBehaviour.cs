using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player = null;
        AudioSource audioSource = null;

        void Start()
        {
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            player.Heal((config as SelfHealConfig).GetAmountToHeal());
            PlayParticleEffect();
            PlayAudio();
        }

        private void PlayAudio()
        {
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }  
    }
}
