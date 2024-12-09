using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Assertions;


public class ChangePostProceessing : MonoBehaviour
{
    [SerializeField] private Volume volume;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if (volume.profile.TryGet(out ColorAdjustments colorAdjustments))




        }

    }
}