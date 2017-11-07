using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; // used for raycaster

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Weapon currentWeaponConfig;                              
        [SerializeField] float baseDamage = 10;
        [Range(.0f, 1.0f)] [SerializeField] float criticalHitChance = .1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle;
                
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string HUMANOID_IDLE = "HumanoidIdle";
        const string HUMANOID_RUN = "HumanoidRun";

        Character character;
        Enemy enemy;        
        Animator animator;        
        CameraRaycaster cameraRaycaster;        
        GameObject weaponObject;
        SpecialAbilities abilities;
        float lastHitTime = 0f;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();

            RegisterForMouseEvents();
            PutWeaponInHand(currentWeaponConfig);
            SetupWeaponAnimations();                        
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

        void Update()
        {
            ScanForAbilityKeyDown();            
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }

        void RegisterForMouseEvents()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
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
                abilities.AttemptSpecialAbility(0);
            }
        }

        void SetupWeaponAnimations()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
            animatorOverrideController[HUMANOID_IDLE] = currentWeaponConfig.GetIdleAnimClip();
            animatorOverrideController[HUMANOID_RUN] = currentWeaponConfig.GetRunAnimClip();
        }

        GameObject RequestDominantHand()
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

        float WeaponDamageRange()
        {            
            float damageRange = UnityEngine.Random.Range(currentWeaponConfig.GetMinDamagePerHit(), currentWeaponConfig.GetMaxDamagePerHit());           
            return Mathf.Round(damageRange);
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetAttackRange();
        }
    }
}

