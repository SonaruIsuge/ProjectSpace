using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityRangeChecker : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
