using UnityEngine;
using System.Collections.Generic;

public class RotateArrow : MonoBehaviour
{
    public Transform target;
    private void FixedUpdate() => transform.LookAt(target.position, Vector3.up);
}
