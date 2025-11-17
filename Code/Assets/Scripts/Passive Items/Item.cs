using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    protected ItemData.Evolution[] evolutionData;
    protected PlayerInventory inventory;
    protected PlayerStats player;

    public PlayerStats Owner { get { return player; } }

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;

        evolutionData = data.evolutionData;

        inventory = FindObjectOfType<PlayerInventory>();
        player = FindObjectOfType<PlayerStats>();
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel < maxLevel;
    }

    public virtual bool DoLevelUp()
    {
       if(evolutionData ==null) return true;

        foreach (ItemData.Evolution e in evolutionData)
        {
            if (e.condition == ItemData.Evolution.Condition.auto)
                AttemptEvolution(e);
        }
        return true;
    }

    public virtual void OnEquip()
    {

    }

    public virtual void OnUnequip()
    {

    }

    public virtual ItemData.Evolution[] CanEvolve()
    {
        List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();

        foreach (ItemData.Evolution e in evolutionData)
        {
            if (CanEvolve(e)) possibleEvolutions.Add(e);
        }

        return possibleEvolutions.ToArray();
    }

    public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount = 1)
    {
        if(evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.LogWarning("Evolution failed");
            return false;
        }

        foreach (ItemData.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if(!item||item.currentLevel<c.level)
            {
                Debug.LogWarning("Evolution failed.");
                return false;
            }
        }

        return true;
    }

    public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelAmount =1)
    {
        if (!CanEvolve(evolutionData, levelAmount))
            return false;

        bool consumeAccessories = (evolutionData.consumes & ItemData.Evolution.Consumption.accessories) > 0;
        bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;

        foreach (ItemData.Evolution.Config c in evolutionData.catalysts)
        {
            if (c.itemType is AccessoryData && consumeAccessories) inventory.Remove(c.itemType, true);
            if (c.itemType is WeaponData && consumeWeapons) inventory.Remove(c.itemType, true);
        }

        if (this is Accessory && consumeAccessories) inventory.Remove((this as Accessory).data, true);
        else if (this is Weapon && consumeWeapons) inventory.Remove((this as Weapon).weaponData, true);

        inventory.Add(evolutionData.outcome.itemType);

        return true;
    }
}
