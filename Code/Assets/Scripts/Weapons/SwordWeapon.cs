using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : ProjectileWeapon
{
    int currentSpawnCount;
    float currentSpawnYOffset;

    protected override bool Attack(int attackCount = 1)
    {
        if(!currentStats.projectilePrefab)
        {
            Debug.LogWarning("No projectile assigned");
            ActivateCooldown();
            return false;
        }

        if (!CanAttack()) return false;

        if(currentCooldown<=0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0;
        }

        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax), currentSpawnYOffset);

        Projectile prefab = Instantiate(currentStats.projectilePrefab, player.transform.position + (Vector3)spawnOffset, Quaternion.identity);
        prefab.player = player;

        if(spawnDir<0)
        {
            prefab.transform.localScale = new Vector3(-Mathf.Abs(prefab.transform.localScale.x), prefab.transform.localScale.y, prefab.transform.localScale.z);
        }

        prefab.weapon = this;

        ActivateCooldown();
        attackCount--;

        currentSpawnCount++;
        if(currentSpawnCount>1 && currentSpawnCount%2==0)
        {
            currentSpawnYOffset += 1;
        }

        if(attackCount>0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = weaponData.baseStats.projectileInterval;
        }

        return true;
    }
}
