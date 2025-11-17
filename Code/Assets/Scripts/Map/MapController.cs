using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;

    public LayerMask terrainMask;

    public GameObject currentChunk;

    public List<GameObject> spawnedChunks;
    public GameObject latestChunk;
    public float maxDist;
    float Dist;

    float Cooldown;
    public float MaxCooldown;

    Vector3 playerLastPosition;
    // Start is called before the first frame update
    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {   
        if(!currentChunk)
        {
            return;
        }
        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;
        string directionName = GetDirectionString(moveDir);

        CheckAndSpawnChunk(directionName);

        if(directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if (directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }

    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionString(Vector3 dir)
    {
        dir = dir.normalized;
        if(Mathf.Abs(dir.x)>Mathf.Abs(dir.y))
        {
            if(dir.y>0.5f)
            {
                return dir.x > 0 ? "Right Up" : "Left Up";
            }
            else if(dir.y<-0.5f)
            {
                return dir.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                return dir.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            if (dir.x > 0.5f)
            {
                return dir.y > 0 ? "Right Up" : "Right Down";
            }
            else if (dir.x < -0.5f)
            {
                return dir.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                return dir.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 position)
    {
        
            int rand = UnityEngine.Random.Range(0, terrainChunks.Count);
            latestChunk = Instantiate(terrainChunks[rand], position, Quaternion.identity);
            spawnedChunks.Add(latestChunk);
        
    }
    void ChunkOptimizer()
    {
        Cooldown -= Time.deltaTime;
        
        if(Cooldown<0f)
        {
            Cooldown = MaxCooldown;
        }
        else
        {
            return;
        }
        
        foreach (GameObject chunk in spawnedChunks)
        {
            Dist = Vector3.Distance(chunk.transform.position, player.transform.position);
            if(Dist>maxDist)
            {
                chunk.SetActive(false);

            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
