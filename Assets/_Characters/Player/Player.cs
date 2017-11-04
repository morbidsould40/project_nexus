using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.CameraUI; // TODO consider rewiring
using RPG.Core;

namespace RPG.Characters
{
    public class Player : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] Weapon currentWeaponConfig = null;        
        [SerializeField] AbilityConfig[] abilities;                      
        [SerializeField] float baseDamage = 10;
        [Range(.0f, 1.0f)] [SerializeField] float criticalHitChance = .1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle = null;
                
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string HUMANOID_IDLE = "HumanoidIdle";
        const string HUMANOID_RUN = "HumanoidRun";
                
        Enemy enemy = null;        
        Animator animator = null;        
        CameraRaycaster cameraRaycaster = null;
        float lastHitTime = 0f;
        float weaponDamage = 0f;        
        GameObject weaponObject;

        void Start()
        {
            RegisterForMouseClick();
            PutWeaponInHand(currentWeaponConfig);
            SetupWeaponAnimations();
            AttachInitialAbilities();            
        }

        public void PutWeaponInHand(Weapon weaponToUse)
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

        private void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }            
        }

        void Update()
        {
            var healthPercentage = GetComponent<HealthSystem>().healthAsPercentage;
            if (healthPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.Length; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    AttemptSpecialAbility(keyIndex);
                }
            }
        }              

        private void SetupWeaponAnimations()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
            animatorOverrideController[HUMANOID_IDLE] = currentWeaponConfig.GetIdleAnimClip();
            animatorOverrideController[HUMANOID_RUN] = currentWeaponConfig.GetRunAnimClip();
        }

        private GameObject RequestDominantHand()
        {
            var handed = currentWeaponConfig.GetDominantGrip();
            if (handed == Weapon.DominantGripHand.RightHand)
            {
                var dominantHands = GetComponentsInChildren<DominantHandRight>();
                return dominantHands[0].gameObject;
            }
            if (handed == Weapon.DominantGripHand.LeftHand)
            {
                var dominantHands = GetComponentsInChildren<DominantHandLeft>();
                return dominantHands[0].gameObject;
            }
            return null;             
        }
        
        private void RegisterForMouseClick()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0);
            }
        }

        private void AttemptSpecialAbility(int abilityIndex)
        {
            var energyComponent = GetComponent<Energy>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            weaponDamage = currentWeaponConfig.GetMinDamagePerHit();
            if (energyComponent.IsEnergyAvailable(energyCost))
            {
                energyComponent.ConsumeEnergy(energyCost);
                var abilityParams = new AbilityUseParams(enemy, baseDamage, weaponDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
        }

        private void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetupWeaponAnimations();
                animator.SetTrigger(ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + WeaponDamageRange();
            if (isCriticalHit)
            {
                criticalHitParticle.Play();
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }

        private float WeaponDamageRange()
        {            
            float damageRange = UnityEngine.Random.Range(currentWeaponConfig.GetMinDamagePerHit(), currentWeaponConfig.GetMaxDamagePerHit());           
            return Mathf.Round(damageRange);
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetAttackRange();
        }
    }
}

