using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [Serializable]
    private struct WaveData
    {
        public float spawnCount;
        public float spawnPeriodDuration;
    }
    
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private NavMeshSurface navMesh;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask losRayMask;

    [SerializeField] private List<WaveData> waves;

    private ObjectPool<Enemy> enemies;

    private void OnEnable()
    {
        enemies = new ObjectPool<Enemy>(
            () =>
            {
                Enemy enemy = Instantiate(enemyPrefab);
                enemy.gameObject.SetActive(false);
                return enemy;
            },
            enemy =>
            {
                Vector3 navMeshMin = navMesh.navMeshData.sourceBounds.min;
                Vector3 navMeshMax = navMesh.navMeshData.sourceBounds.max;
                float halfHeight = 0.5f * enemy.GetComponent<CapsuleCollider>().height;

                if (((1 << navMesh.gameObject.layer) & losRayMask) <= 0)
                    throw new Exception("NavMesh isn't on the right layer.");
                
                while (true)
                {
                    Vector3 position = new Vector3(
                        Random.Range(navMeshMin.x, navMeshMax.x),
                        Random.Range(navMeshMin.y, navMeshMax.y),
                        Random.Range(navMeshMin.z, navMeshMax.z)
                    );

                    if (NavMesh.SamplePosition(position, out NavMeshHit hit, 4f * halfHeight, NavMesh.AllAreas))
                    {
                        if (!Physics.Linecast(player.position, hit.position + halfHeight * Vector3.up, losRayMask))
                            continue;

                        enemy.transform.position = hit.position;
                        enemy.gameObject.SetActive(true);
                        enemy.Target = player;
                        return;
                    }
                }
            },
            enemy =>
            {
                enemy.gameObject.SetActive(false);
            },
            Destroy
        );

        Enemy test = enemies.Get();
        test = enemies.Get();
        test = enemies.Get();
    }
}
