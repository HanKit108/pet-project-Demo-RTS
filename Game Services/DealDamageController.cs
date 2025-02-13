public class DealDamageController
{
    public void DealDamage(IDamagable target, float damageAmount)
    {
        target.TakeDamage(damageAmount);
    }
}
