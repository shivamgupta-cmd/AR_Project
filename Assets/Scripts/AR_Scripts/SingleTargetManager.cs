using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class SingleTargetManager : MonoBehaviour
{
    public GameObject[] prefabs; // Assign your 20 prefabs here in the Inspector
    private GameObject currentPrefab;
    private bool isTracking = false;

    void Start()
    {
        foreach (var target in FindObjectsOfType<ImageTargetBehaviour>())
        {
            target.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour target, TargetStatus status)
    {
        if (status.Status == Status.TRACKED && !isTracking)
        {
            isTracking = true;
            SpawnPrefab(target);
        }
        else if (status.Status != Status.TRACKED && currentPrefab != null)
        {
            Destroy(currentPrefab);
            isTracking = false;
        }
    }

    private void SpawnPrefab(ObserverBehaviour target)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (target.TargetName == prefabs[i].name)
            {
                currentPrefab = Instantiate(prefabs[i], target.transform.position, target.transform.rotation);
                currentPrefab.transform.parent = target.transform; // Optional: Parent the prefab to the target
                break;
            }
        }
    }
}
