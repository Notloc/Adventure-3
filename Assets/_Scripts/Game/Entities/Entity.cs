using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour, iDamagable
{
    [SerializeField] [Range(0,5)] float noah;

    void iDamagable.Damage(float amount)
    {
        Debug.Log(this.name+" took " + amount + " damage.");
    }
}
