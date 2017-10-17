using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/AoE Attack"))]
    public class AreaEffectConfig : SpecialAbility
    {
        [Header("AoE Attack Specific")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15f;

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            var behaviourComponent = gameObjectToAttachTo.AddComponent<AreaEffectBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetRadius()
        {
            return radius;
        }

        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        } 
    }
}

