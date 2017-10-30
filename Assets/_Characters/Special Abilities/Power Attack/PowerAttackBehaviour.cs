using System;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        public override void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
            PlayAbilitySound();
        }

        private void DealDamage(AbilityUseParams useParams)
        {
            float damageToDeal = ((useParams.baseDamage + useParams.weaponDamage) * (config as PowerAttackConfig).GetExtraDamage());
            useParams.target.TakeDamage(damageToDeal);            
        }
    }
}

