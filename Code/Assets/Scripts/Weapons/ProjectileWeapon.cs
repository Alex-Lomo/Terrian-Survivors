using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    protected override void Update()
    {
        base.Update();

        if(currentAttackInterval>0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
       if(!currentStats.projectilePrefab)
        {
            Debug.LogWarning("Projectile prefab not set");
            ActivateCooldown();
            return false;
        }

        if (!CanAttack()) return false;

        float spawnAngle = GetSpawnAngle();

        Projectile prefab = Instantiate(currentStats.projectilePrefab, player.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle));

        prefab.weapon = this;
        prefab.player = player;

        ActivateCooldown();
        attackCount--;

        if(attackCount>0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = weaponData.baseStats.projectileInterval;
        }

        return true;
    }

    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle =0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax), Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));
    }
}
