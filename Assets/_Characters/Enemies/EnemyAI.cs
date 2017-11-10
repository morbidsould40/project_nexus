using System;
using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 3f;
        [SerializeField] float waypointTolerance = 2.0f;        
        [SerializeField] float patrolSpeed = 0.2f;
        [SerializeField] float animateSpeed = 0.4f;
        [SerializeField] WaypointContainer patrolPath;

        //[SerializeField] float damagePerShot = 8f;
        //[SerializeField] float firingPeriodInSeconds = 0.5f;
        //[SerializeField] float firingPeriodVariation = 0.2f;
        //[SerializeField] GameObject projectileToUse;
        //[SerializeField] GameObject projectileSocket;
        //[SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);


        float currentWeaponRange = 5f;
        float distanceToPlayer;
        float defaultMoveSpeedMultiplier;
        float defaultAnimationSpeedMultiplier;
        int nextWaypointIndex;
        PlayerMovement player;
        Character character;

        enum EnemyState { idle, attacking, patrolling, chasing}
        EnemyState state = EnemyState.idle;

        private void Start()
        {
            character = GetComponent<Character>();

            player = FindObjectOfType<PlayerMovement>();
            defaultMoveSpeedMultiplier = character.moveSpeedMultiplier;
            defaultAnimationSpeedMultiplier = character.animationSpeedMultiplier;

        }

        private void Update()
        {
            // Attack player if in attack range
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

            if (distanceToPlayer > chaseRadius && state != EnemyState.patrolling)
            {
                StopAllCoroutines();
                StartCoroutine(Patrol());
            }
            if (distanceToPlayer <= chaseRadius && state != EnemyState.chasing)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }
            if (distanceToPlayer <= currentWeaponRange && state != EnemyState.attacking)
            {
                StopAllCoroutines();
                state = EnemyState.attacking;
            }            
        }

        IEnumerator Patrol()
        {
            state = EnemyState.patrolling;
            while (true)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);                
                character.moveSpeedMultiplier = patrolSpeed;
                character.animationSpeedMultiplier = animateSpeed;
                CycleWaypointWhenClose(nextWaypointPos);                               
                yield return new WaitForSeconds(3.0f);
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

        // TODO seperate out Character firing logic
        //void FireProjectile()
        //{
        //    GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
        //    Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
        //    projectileComponent.SetDamage(damagePerShot);
        //    projectileComponent.SetShooter(gameObject);

        //    Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
        //    float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
        //    newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        //}

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

