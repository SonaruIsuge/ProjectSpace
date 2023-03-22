using System;
using UnityEngine;


public class Billboard : MonoBehaviour
{
    private Camera MainCamera => Camera.main;


    private void Update()
    {
        transform.LookAt(transform.position + MainCamera.transform.forward);
    }
}
