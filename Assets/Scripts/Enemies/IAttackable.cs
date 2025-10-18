namespace Enemies
{
public interface IAttackable
{
    void TakeDamage(int damageAmount);

    void Die();
}
}