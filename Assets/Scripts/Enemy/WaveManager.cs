using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    private class Wave
    {
        private HashSet<Enemy> enemies = new();

        public event Action Cleared;

        public bool Add(Enemy enemy) => enemies.Add(enemy);

        public void Remove(Enemy enemy)
        {
            enemies.Remove(enemy);
            
            if (enemies.Count > 0)
                return;
            
            Cleared?.Invoke();
        }
    }
    
    [Serializable]
    private struct WaveInfo
    {
        public float prelude;
        public int spawnCount;
        public float spawnPeriodDuration;
    }
    
    [SerializeField] private Enemy enemyPrefab;
    [FormerlySerializedAs("navMesh")] [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask losRayMask;
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private InputActionAsset inputs;

    [SerializeField] private List<WaveInfo> waves;

    private GameObject enemyParent;
    private ObjectPool<Enemy> enemyPool;
    private Wave currentWave = new();

    private void OnEnable()
    {
        enemyParent = new GameObject("Enemies");
        enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, Destroy);

        StartNextWave();
        currentWave.Cleared += StartNextWave;
    }

    private void OnDisable()
    {
        currentWave.Cleared -= StartNextWave;
    }

    private Enemy CreateEnemy()
    {
        Enemy output = Instantiate(enemyPrefab, enemyParent.transform);
        output.gameObject.SetActive(false);
        return output;
    }

    private void OnGetEnemy(Enemy enemy)
    {
        Vector3 offset = spawnArea.transform.position;
        Vector3 navMeshMin = offset + spawnArea.bounds.min;
        Vector3 navMeshMax = offset + spawnArea.bounds.max;
        float halfHeight = 0.5f * enemy.GetComponent<NavMeshAgent>().height;

        StartCoroutine(SpawnInRandomLocation());

        IEnumerator SpawnInRandomLocation()
        {
            while (true)
            {
                yield return null;
                
                Vector3 position = new Vector3(
                    Random.Range(navMeshMin.x, navMeshMax.x),
                    Random.Range(navMeshMin.y, navMeshMax.y),
                    Random.Range(navMeshMin.z, navMeshMax.z)
                );

                if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 4f * halfHeight, NavMesh.AllAreas))
                    continue;
                
                if (!Physics.Linecast(player.position, hit.position + halfHeight * Vector3.up, losRayMask))
                    continue;

                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                agent.enabled = true;
                agent.Warp(hit.position);
                enemy.transform.position = hit.position;
                enemy.gameObject.SetActive(true);
                enemy.Target = player;
                break;
            }
        }
    }

    private void OnReleaseEnemy(Enemy enemy)
    {
        enemy.GetComponent<Rigidbody>().isKinematic = true;
        enemy.StopAllCoroutines();
        enemy.gameObject.SetActive(false);
    }

    private void StartNextWave()
    {
        if (waves.Count <= 0)
        {
            EndGame();
            return;
        }

        WaveInfo wave = waves[0];
        waves.RemoveAt(0);

        StartCoroutine(SpawningRoutine(wave));
    }

    private void EndGame()
    {
        inputs.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        victoryUI.SetActive(true);
    }

    private IEnumerator SpawningRoutine(WaveInfo wave)
    {
        YieldInstruction waitForNextSpawn = new WaitForSeconds(wave.spawnPeriodDuration / wave.spawnCount);

        yield return new WaitForSeconds(wave.prelude);
        
        int spawnCount = wave.spawnCount;
        while (spawnCount-- > 0)
        {
            Enemy enemy = enemyPool.Get();
            Damageable damageController = enemy.GetComponent<Damageable>();
            damageController.Died += OnDeath;
            currentWave.Add(enemy);

            void OnDeath()
            {
                enemyPool.Release(enemy);
                currentWave.Remove(enemy);
                damageController.Died -= OnDeath;
            }

            yield return waitForNextSpawn;
        }
    }
}
