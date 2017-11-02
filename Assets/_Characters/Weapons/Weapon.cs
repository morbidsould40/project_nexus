using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class Weapon : ScriptableObject
    {
        public Transform gripTransform;

        public enum WeaponHands
        {
            OneHanded,
            TwoHanded
        }

        public enum DominantGripHand
        {
            RightHand,
            LeftHand
        }

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] AnimationClip idleAnimation;
        [SerializeField] AnimationClip runAnimation;
        [SerializeField] float minDamagePerHit = 2f;
        [SerializeField] float maxDamagePerHit = 5f;
        [SerializeField] float minTimeBetweenHits = 1f;
        [SerializeField] float maxMeleeRange = 2f;
        [SerializeField] WeaponHands weaponHands;
        [SerializeField] DominantGripHand dominantGripHand;        

        public float GetMinTimeBetweenHits()
        {
            // TODO consider whether we take animation time into account
            return minTimeBetweenHits;
        }

        public float GetAttackRange()
        {
            return maxMeleeRange;
        }

        public float GetMinDamagePerHit()
        {
            return minDamagePerHit;
        }

        public float GetMaxDamagePerHit()
        {
            return maxDamagePerHit;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        public AnimationClip GetIdleAnimClip()
        {
            RemoveAnimationEvents();
            return idleAnimation;
        }

        public AnimationClip GetRunAnimClip()
        {
            RemoveAnimationEvents();
            return runAnimation;
        }
        
        public WeaponHands GetWeaponHand()
        {
            return weaponHands;
        }

        public DominantGripHand GetDominantGrip()
        {
            return dominantGripHand;
        }

        // Remove animation events so that asset packs cannot cause crashes
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
            idleAnimation.events = new AnimationEvent[0];
            runAnimation.events = new AnimationEvent[0];
        }
    }
}

