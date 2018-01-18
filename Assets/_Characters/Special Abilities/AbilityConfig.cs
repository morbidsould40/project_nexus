using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public abstract class AbilityConfig : ScriptableObject
    {

        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] float coolDown = 0.25f;
        [SerializeField] GameObject particlePrefab;
        [SerializeField] Image backgroundIcon;
        [SerializeField] Image coolDownIcon;
        [SerializeField] AnimationClip abilityAnimation;
        [SerializeField] AudioClip[] audioClips;

        protected AbilityBehaviour behaviour;

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

        public void AttachAbilityTo(GameObject objectToattachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToattachTo);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public void Use(GameObject target)
        {
            behaviour.Use();
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public float GetCoolDown()
        {
            Debug.Log("Ability Config got cooldown");
            return coolDown;
        }

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AnimationClip GetAbilityAnimation()
        {
            return abilityAnimation;
        }

        public AudioClip GetRandomAbilitySounds()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }

        public Image GetBackgroundIcon()
        {
            return backgroundIcon;
        }

        public Image GetCoolDownIcon()
        {
            return coolDownIcon;
        }
    }
}

