using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 3f;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float waypointWaitTime = 3.0f;
        [SerializeField] float patrolMovementSpeed = 0.5f;
        [SerializeField] WaypointContainer patrolPath;

        float currentWeaponRange;
        float distanceToPlayer;
        float defaultMoveSpeedMultiplier;
        float defaultAnimationSpeedMultiplier;
        int nextWaypointIndex;
        PlayerControl player;
        Character character;
        WeaponSystem weaponSystem;
        enum EnemyState { idle, attacking, patrolling, chasing}

        EnemyState state = EnemyState.idle;

        private void Start()
        {
            character = GetComponent<Character>();

            player = FindObjectOfType<PlayerControl>();
            weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        private void Update()
        {
            // Attack player if in attack range
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);            
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

            bool inWeaponRadius = distanceToPlayer <= currentWeaponRange;
            bool inChaseRadius = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
            bool outsideChaseRadius = distanceToPlayer > chaseRadius;

            if (outsideChaseRadius)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(Patrol());
            }
            if (inChaseRadius)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(ChasePlayer());
            }
            if (inWeaponRadius)
            {
                StopAllCoroutines();
                state = EnemyState.attacking;
                weaponSystem.AttackTarget(player.gameObject);
            }            
        }

        IEnumerator Patrol()
        {
            state = EnemyState.patrolling;
            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.AnimatorMaxPatrol = patrolMovementSpeed;
                character.SetDestination(nextWaypointPos);                
                CycleWaypointWhenClose(nextWaypointPos);                               
                yield return new WaitForSecondsRealtime(waypointWaitTime);
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = EnemyState.chasing;
            //var minRangedDistance = weaponSystem.GetCurrentWeapon().GetMinAttackRange();
            //float rangedDistance = Vector3.Distance(player.transform.position, character.transform.position) - minRangedDistance;
            //Vector3 offset = -character.transform.forward;
            //offset *= rangedDistance;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.AnimatorMaxPatrol = character.GetAnimatorMaxForward();
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        void OnDrawGizmos()
        {
            // Draw Attack Range Gizmo
            Gizmos.color = new Color(255f, 0f, 0f, .5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            // Draw Chase Range Gizmo
            Gizmos.color = new Color(0f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}

