using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour, iDamagable
{
    public float health;

    public virtual void Damage(float amount)
    {
        health -= amount;
    }
}
