using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour, iDamagable
{
    void iDamagable.Damage(float amount)
    {
        Debug.Log(this.name+" took " + amount + " damage.");
    }
}
