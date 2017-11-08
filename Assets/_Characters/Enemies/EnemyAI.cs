using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 3f;

        //[SerializeField] float damagePerShot = 8f;
        //[SerializeField] float firingPeriodInSeconds = 0.5f;
        //[SerializeField] float firingPeriodVariation = 0.2f;
        //[SerializeField] GameObject projectileToUse;
        //[SerializeField] GameObject projectileSocket;
        //[SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);


        float currentWeaponRange = 5f;
        float distanceToPlayer;
        PlayerMovement player;
        Character character;

        enum EnemyState { idle, attacking, patrolling, chasing}
        EnemyState state = EnemyState.idle;

        private void Start()
        {
            character = GetComponent<Character>();

            player = FindObjectOfType<PlayerMovement>();
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
                state = EnemyState.patrolling;
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

        IEnumerator ChasePlayer()
        {
            state = EnemyState.chasing;
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

