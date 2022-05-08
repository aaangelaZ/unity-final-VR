using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackarea : MonoBehaviour
{
    private int damage = 3;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<fuluhealth>() != null)
        {
            fuluhealth health = GetComponent<Collider>().GetComponent<fuluhealth>();
            health.Damage(damage);
        }
    }
}
