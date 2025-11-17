using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Character Data", menuName = "Data Object/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    string characterName;
    public string CharacterName { get => characterName; private set => characterName = value; }

    [SerializeField]
    WeaponData startingWeapon;
    public WeaponData StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery,  armor;
        public float moveSpeed, magnet;
        [Range(-1,10)]public float damageMultiplier, projectileSpeed, area, duration;
        [Range(-1, 10)] public int amount;
        [Range(-1, 1)] public float cooldown;
        [Min(-1)]public float growth, greed, curse;
        public int revival;

        

        public static Stats operator +(Stats s1,Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.damageMultiplier += s2.damageMultiplier;
            s1.projectileSpeed += s2.projectileSpeed;
            s1.magnet += s2.magnet;
            s1.armor += s2.armor;
            s1.area += s2.area;
            s1.duration += s2.duration;
            s1.amount += s2.amount;
            s1.cooldown += s2.cooldown;
            s1.growth += s2.growth;
            s1.greed += s2.greed;
            s1.curse += s2.curse;
            s1.revival += s2.revival;
           
            return s1;
        }
    }

    public Stats stats;
    public Stats growth;
    
    

}
