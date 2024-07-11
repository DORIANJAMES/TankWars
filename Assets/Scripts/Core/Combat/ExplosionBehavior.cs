using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    public void DestroyGameObject()
    {
        Destroy(gameObject.GetComponentInParent<Rigidbody2D>().gameObject);
    }
}
