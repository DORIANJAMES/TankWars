using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private static List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    

    private void OnEnable()
    {
        _spawnPoints.Add(this);
    }

    private void OnDisable()
    {
        _spawnPoints.Remove(this);
    }
    
    public static Vector3 GetRandomSpawnPos()
    {
        if (_spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }
        return _spawnPoints[Random.Range(0, _spawnPoints.Count - 1)].transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
