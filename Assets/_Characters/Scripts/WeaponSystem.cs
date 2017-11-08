
using UnityEngine;

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

        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            print("Attacking " + targetToAttack);
            // TODO use a repeat attack Co-routine
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
            animator = GetComponent<Animator>();
            var animatorOverrideController = character.GetAnimatorOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
            animatorOverrideController[HUMANOID_IDLE] = currentWeaponConfig.GetIdleAnimClip();
            animatorOverrideController[HUMANOID_RUN] = currentWeaponConfig.GetRunAnimClip();
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

        void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetupWeaponAnimations();
                animator.SetTrigger(ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
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
