using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour, iDamagable
{
    public float health;

    public void Damage(float amount)
    {

    }
}
