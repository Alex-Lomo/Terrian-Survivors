using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Accessory Data", menuName = "Data Object/Accessory Data")]
public class AccessoryData : ItemData
{
    public Accessory.Modifier baseStats;
    public Accessory.Modifier[] growth;

    public Accessory.Modifier GetLevelData(int level)
    {
        if(level - 2<growth.Length)
        {
            return growth[level - 2];
        }

        Debug.LogWarning("Accessory does not have a next level");
        return new Accessory.Modifier();
    }
}
