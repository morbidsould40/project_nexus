﻿using UnityEngine;
using System.Collections;
using System;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] WeaponConfig currentWeaponConfig;
        [SerializeField] float baseDamage = 10;
        //[Range(.0f, 1.0f)] [SerializeField] float criticalHitChance = .1f;
        //[SerializeField] float criticalHitMultiplier = 1.25f;
        //[SerializeField] ParticleSystem criticalHitParticle;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string HUMANOID_IDLE = "HumanoidIdle";
        const string HUMANOID_RUN = "HumanoidRun";

        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;
        float lastHitTime = 0f;

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>(); ;

            PutWeaponInHand(currentWeaponConfig);
            SetupWeaponAnimations();
        }

        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;

            if (target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;            
            }
            else
            {
                // check to see if target is dead
                var targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
                targetIsDead = targetHealth <= Mathf.Epsilon;

                // check to see if target is out of range
                var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());            
        }

        IEnumerator AttackTargetRepeatedly()
        {
            // determine if alive (both attacker and defender)
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && targetStillAlive)
            {
                var animationClip = currentWeaponConfig.GetAttackAnimClip();
                float animationClipTime = animationClip.length / character.GetAnimationSpeedMultiplier();
                float timeToWait = animationClipTime + currentWeaponConfig.GetTimeBetweenAnimationCycles();
                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;
                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }            
        }

        void AttackTargetOnce()
        {
            SetupWeaponAnimations();
            transform.LookAt(target.transform); 
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = currentWeaponConfig.GetDamageDelay();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject);
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
            SetupWeaponAnimations();
        }

        void SetupWeaponAnimations()
        {
            // protect against no override controller
            if (!character.GetAnimatorOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide " + gameObject + " with an animation override controller to the player");
            }
            else
            {
                var animatorOverrideController = character.GetAnimatorOverrideController();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
                animatorOverrideController[HUMANOID_IDLE] = currentWeaponConfig.GetIdleAnimClip();
                animatorOverrideController[HUMANOID_RUN] = currentWeaponConfig.GetRunAnimClip();
            }
        }

        GameObject RequestDominantHand()
        {
            var handed = currentWeaponConfig.GetDominantGrip();
            if (handed == WeaponConfig.DominantGripHand.RightHand)
            {
                var dominantHands = GetComponentsInChildren<DominantHandRight>();
                return dominantHands[0].gameObject;
            }
            if (handed == WeaponConfig.DominantGripHand.LeftHand)
            {
                var dominantHands = GetComponentsInChildren<DominantHandLeft>();
                return dominantHands[0].gameObject;
            }
            return null;
        }

        float CalculateDamage()
        {
            // bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            return baseDamage + WeaponDamageRange();
            //if (isCriticalHit)
            //{
            //    criticalHitParticle.Play();
            //    return damageBeforeCritical * criticalHitMultiplier;
            //}
            //else
            //{
            //    return damageBeforeCritical;
            //}
        }

        float WeaponDamageRange()
        {
            float damageRange = UnityEngine.Random.Range(currentWeaponConfig.GetMinDamagePerHit(), currentWeaponConfig.GetMaxDamagePerHit());
            return Mathf.Round(damageRange);
        }
    }
}
