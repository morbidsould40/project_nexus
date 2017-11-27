using System;
using System.Collections;
using System.Collections.Generic;
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
        int nextWaypointIndex;
        PlayerControl player;
        Character character;

        enum EnemyState { idle, attacking, patrolling, chasing}

        EnemyState state;

        private void Start()
        {
            state = EnemyState.idle;
            character = GetComponent<Character>();
            player = FindObjectOfType<PlayerControl>();
        }

        void Update()
        {            
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();            

            bool inWeaponCircle = distanceToPlayer <= currentWeaponRange;
            bool inChaseRing = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
            bool outsideChaseRing = distanceToPlayer > chaseRadius && distanceToPlayer > currentWeaponRange;

            if (outsideChaseRing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(Patrol());
            }
            if (inChaseRing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(ChasePlayer());
            }
            if (inWeaponCircle)
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
                character.SetDestination(nextWaypointPos);                
                yield return new WaitForSeconds(waypointWaitTime);
                CycleWaypointWhenClose(nextWaypointPos);
            }
        }

        void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = EnemyState.chasing;
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

