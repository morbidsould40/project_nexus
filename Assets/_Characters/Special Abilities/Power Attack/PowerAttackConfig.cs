﻿using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Power Attack"))]
    public class PowerAttackConfig : AbilityConfig
    {
        [Header("Power Attack Specific")]
        [SerializeField] float extraDamageModifier = 2f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<PowerAttackBehaviour>();
        }

        public float GetExtraDamage()
        {
            return extraDamageModifier;
        }
    }
}

