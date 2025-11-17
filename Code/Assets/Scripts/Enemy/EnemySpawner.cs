using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int waveQuota;
        public float spawnInterval;
        public int spawnCount;

        public List<EnemyType> enemies;
    }
    [System.Serializable]
    public class EnemyType
    {
        public string enemyName;
        public int enemyCount;
        public int spawnedCount;
        public GameObject enemyPrefab;
    }
    public List<Wave> waves;
    public int currentWave;
    Transform player;

    [Header("Spawner Properties")]
    float spawnTimer;
    public float waveInterval;
    public int enemiesAlive;
    public int maxEnemies;
    public bool isAtMax;
    public bool isWaveActive = false;


    [Header("Spawn Points")]
    public List<Transform> spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        CalculateWaveQuota();
        player = FindObjectOfType<PlayerStats>().transform;
        
    }

    // Update is called once per frame
    void Update()
    {   

        if(currentWave<waves.Count && waves[currentWave].spawnCount==0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if(spawnTimer >= waves[currentWave].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;
        yield return new WaitForSeconds(waveInterval);
        if(currentWave<waves.Count-1)
        {
            isWaveActive = false;
            currentWave++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentQuota = 0;
        foreach (EnemyType enemy in waves[currentWave].enemies)
        {
            currentQuota += enemy.enemyCount;
        }
        waves[currentWave].waveQuota = currentQuota;
    }

    void SpawnEnemies()
    {
        if (waves[currentWave].spawnCount < waves[currentWave].waveQuota && !isAtMax)
        {
            foreach (EnemyType enemy in waves[currentWave].enemies)
            {   
                if(enemy.spawnedCount<enemy.enemyCount)
                {
                    Instantiate(enemy.enemyPrefab, player.position + spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
                    enemy.spawnedCount++;
                    waves[currentWave].spawnCount++;
                    enemiesAlive++;

                    if (enemiesAlive >= maxEnemies)
                    {
                        isAtMax = true;
                        return;
                    }
                }
            }
        }

        
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        GameManager.instance.AddKillCount();

        if (enemiesAlive < maxEnemies)
        {
            isAtMax = false;
        }

    }
}
