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
        [SerializeField] float waypointTolerance = 2.0f;
        [SerializeField] float waypointWaitTime = 3.0f;
        [SerializeField] float patrolSpeed = 0.4f;
        [SerializeField] float animateSpeed = 0.8f;
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
            defaultMoveSpeedMultiplier = character.moveSpeedMultiplier;
            defaultAnimationSpeedMultiplier = character.animationSpeedMultiplier;
            weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        private void Update()
        {
            // Attack player if in attack range
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);            
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

            if (distanceToPlayer > chaseRadius && state != EnemyState.patrolling)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(Patrol());
            }
            if (distanceToPlayer <= chaseRadius && state != EnemyState.chasing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(ChasePlayer());
            }
            if (distanceToPlayer <= currentWeaponRange && state != EnemyState.attacking)
            {
                StopAllCoroutines();
                weaponSystem.AttackTarget(player.gameObject);
            }            
        }

        IEnumerator Patrol()
        {
            state = EnemyState.patrolling;
            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);                
                character.moveSpeedMultiplier = patrolSpeed;
                character.animationSpeedMultiplier = animateSpeed;
                CycleWaypointWhenClose(nextWaypointPos);                               
                yield return new WaitForSeconds(waypointWaitTime);
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
            character.moveSpeedMultiplier = defaultMoveSpeedMultiplier;
            character.animationSpeedMultiplier = defaultAnimationSpeedMultiplier;
            while (distanceToPlayer >= currentWeaponRange)
            {
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

