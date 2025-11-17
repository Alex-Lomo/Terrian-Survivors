using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D col)
    {
        PlayerInventory p = col.GetComponent<PlayerInventory>();
        if(p)
        {
            bool randomBool = Random.Range(0, 2) == 0;

            OpenTreasureChest(p, randomBool);

            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {
        foreach (PlayerInventory.Slot slot in inventory.weaponSlots)
        {
            Weapon w = slot.item as Weapon;

            if (!w || w.weaponData.evolutionData == null) continue;

            foreach (ItemData.Evolution e in w.weaponData.evolutionData)
            {
                if(e.condition == ItemData.Evolution.Condition.treasureChest)
                {
                    bool attempt = w.AttemptEvolution(e,0);
                    if (attempt) return;
                }
            }
        }

        GameManager.instance.StartLevelUp();
    }
}
