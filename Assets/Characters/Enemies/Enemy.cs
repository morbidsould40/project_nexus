using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] float maxHealthPoints = 100;
    [SerializeField] float attackRadius = 5f;
    [SerializeField] float chaseRadius = 3f;
    [SerializeField] float damagePerShot = 8f;
    [SerializeField] float secondsBetweenShots = 0.5f;
    [SerializeField] GameObject projectileToUse;
    [SerializeField] GameObject projectileSocket;
    [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

    bool isAttacking = false;
    float currentHealthPoints;
    AICharacterControl aiCharacterControl = null;
    GameObject player = null;

    public float healthAsPercentage
    {
        get { return currentHealthPoints / maxHealthPoints; }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterControl = GetComponent<AICharacterControl>();
        currentHealthPoints = maxHealthPoints;
    }

    private void Update()
    {
        // Attack player if in attack range
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= attackRadius && !isAttacking)
        {
            isAttacking = true;
            InvokeRepeating("FireProjectile", 0f, secondsBetweenShots); // TODO Switch to coroutines
        }

        // Stop attacking player if outside attack range
        if (distanceToPlayer > attackRadius)
        {
            isAttacking = false;
            CancelInvoke();
        }

        // Chase player if within chase range
        if (distanceToPlayer <= chaseRadius)
        {
            aiCharacterControl.SetTarget(player.transform);
        }
        else
        {
            aiCharacterControl.SetTarget(transform);
        }
    }

    // TODO seperate out Character firing logic
    void FireProjectile()
    {
        GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
        Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
        projectileComponent.SetDamage(damagePerShot);
        projectileComponent.SetShooter(gameObject);

        Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
        float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        if(currentHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        // Draw Attack Range Gizmo
        Gizmos.color = new Color(255f, 0f, 0f, .5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Draw Chase Range Gizmo
        Gizmos.color = new Color(0f, 0f, 255f, .5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
