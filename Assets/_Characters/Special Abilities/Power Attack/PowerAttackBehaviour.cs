using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {

        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }
        
        // Use this for initialization
        void Start()
        {
            print("Power Attack behaviour attached to " + gameObject.name);
        }

        public void Use(AbilityUseParams useParams)
        {
            print("base damage: " + useParams.baseDamage + ". weapon damage: " + useParams.weaponDamage + ".");
            float damageToDeal = ((useParams.baseDamage + useParams.weaponDamage) * config.GetExtraDamage());
            print("Power Attack. Damage Dealt: " + damageToDeal);
            useParams.target.TakeDamage(damageToDeal);
        }
    }
}

