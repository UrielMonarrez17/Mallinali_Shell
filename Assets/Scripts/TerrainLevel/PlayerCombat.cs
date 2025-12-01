using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float comboRangeMultiplier = 2.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 10;

    [Header("Cooldown Settings")]
    [Tooltip("Time in seconds between attacks")]
    public float attackCooldown = 0.5f; // <--- NEW VARIABLE
    private float nextAttackTime = 0f;  // <--- NEW VARIABLE
    
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PerformAttack()
    {
        // 1. CHECK COOLDOWN
        // If the current time is less than the allowed time, we exit immediately.
        if (Time.time < nextAttackTime) return;

        // 2. SET NEXT ATTACK TIME
        nextAttackTime = Time.time + attackCooldown;

        // animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        bool landedHit = false; 

        foreach (Collider2D enemy in hitEnemies)
        {
            var damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                landedHit = true; 

                // Standard damage logic
                Vector2 hitPoint = attackPoint.position;
                Vector2 hitNormal = new Vector2(spriteRenderer.flipX ? -1 : 1, 0f);
                damageable.TakeDamage(attackDamage, hitPoint, hitNormal);
            }
        }

        // --- REPORT TO MANAGER ---
        if (landedHit)
        {
            if (PlayerManagerDual.Instance != null)
            {
                 PlayerManagerDual.Instance.RegisterHit();
            }
        }
    }

    // This function is called by the Manager (Combo Assist)
    // Note: We usually DON'T check cooldown here because this is a special event triggered by the Manager.
    public void PerformComboAttack()
    {
        Debug.Log(gameObject.name + " performs Combo Attack!");
        // animator.SetTrigger("ComboAttack");

        float comboRange = attackRange * comboRangeMultiplier; 

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, comboRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            var damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector2 hitNormal = new Vector2(spriteRenderer.flipX ? -1 : 1, 0f);
                // Double damage for the combo assist
                damageable.TakeDamage(attackDamage * 2, attackPoint.position, hitNormal);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}