using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if(item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.transform.parent.gameObject.GetComponent<Image>().enabled = true;
                image.sprite = w.weaponData.icon;

            }
            else
            {
                Accessory a = item as Accessory;
                image.enabled = true;
                image.transform.parent.gameObject.GetComponent<Image>().enabled = true;
                image.sprite = a.data.icon;
            }
            Debug.Log(string.Format("Assign {0} to player", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;

        }

        public bool isEmpty() { return item == null; }
    }
    [Header("UI Elements")]
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> accessorySlots = new List<Slot>(6);


    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }


    
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<AccessoryData> availableAccessories = new List<AccessoryData>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    PlayerStats player;

    private void Start()
    {
        player =GetComponent<PlayerStats>();
    }

    public bool Has(ItemData type) { return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is AccessoryData) return Get(type as AccessoryData);
        else return null;
    }

    public Accessory Get(AccessoryData type)
    {
        foreach (Slot s in accessorySlots)
        {
            Accessory a = s.item as Accessory;
            if(a.data == type)
            {
                return a;
            }
        }
        return null;
    }

    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.weaponData == type)
            {
                return w;
            }
        }
        return null;
    }

    public bool Remove(WeaponData data, bool removeUpgradeAvailability=false)
    {
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if(w.weaponData==data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(AccessoryData data, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availableAccessories.Remove(data);

        for (int i = 0; i < accessorySlots.Count; i++)
        {
            Accessory a = accessorySlots[i].item as Accessory;
            if  (a.data== data)
            {
                accessorySlots[i].Clear();
                a.OnUnequip();
                Destroy(a.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is AccessoryData) return Remove(data as AccessoryData, removeUpgradeAvailability);
        else if (data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailability);
        else return false;
    }

    public int Add(WeaponData data)
    {
        int slotNum = -1;

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i].isEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        Type weaponType = Type.GetType(data.behaviour);

        if(weaponType!=null)
        {
            GameObject go = new GameObject(data.baseStats.name);
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            weaponSlots[slotNum].Assign(spawnedWeapon);

            if(GameManager.instance !=null && GameManager.instance.isChoosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }

            return slotNum;
        }
        else
        {
            Debug.LogWarning("Invalid Weapon");
        }

        return -1;

    }

    public int Add(AccessoryData data)
    {
        int slotNum = -1;

        for (int i = 0; i < accessorySlots.Count; i++)
        {
            if (accessorySlots[i].isEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        GameObject go = new GameObject(data.baseStats.name);
        Accessory a = go.AddComponent<Accessory>();
        
        a.transform.SetParent(transform);
        a.transform.localPosition = Vector2.zero;
        a.Initialise(data);

        accessorySlots[slotNum].Assign(a);

        if (GameManager.instance != null && GameManager.instance.isChoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        player.RecalculateStats();

        return slotNum;


    }

    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is AccessoryData) return Add(data as AccessoryData);
        return -1;
    }

    public void LevelUpWeapon(int slotIndex)
    {
        if(weaponSlots.Count>slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            if(!weapon.DoLevelUp())
            {
                Debug.LogWarning("Failed to level up");
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.isChoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpAccessory(int slotIndex)
    {
        if (accessorySlots.Count > slotIndex)
        {
            Accessory accessory = accessorySlots[slotIndex].item as Accessory;

            if (!accessory.DoLevelUp())
            {
                Debug.LogWarning("Failed to level up");
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.isChoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
        List<AccessoryData> availableAccessoryUpgrades = new List<AccessoryData>(availableAccessories);

        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            if (availableWeaponUpgrades.Count == 0 && availableAccessoryUpgrades.Count == 0) return;

            int upgradeType;
            if (availableWeaponUpgrades.Count == 0) upgradeType = 2;
            else if (availableAccessoryUpgrades.Count == 0) upgradeType = 1;
            else upgradeType = UnityEngine.Random.Range(1, 3);

            if(upgradeType ==1)
            {
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);


                if(chosenWeaponUpgrade!=null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool isLevelUp = false;
                    for(int i=0; i< weaponSlots.Count;i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if(w!=null && w.weaponData==chosenWeaponUpgrade)
                        {
                            

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i));
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel +1)
                            {
                                availableWeapons.Remove(w.weaponData);
                                
                            }
                            break;
                        }
                    }

                    if(!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            else if(upgradeType==2)
            {
                AccessoryData chosenAccessoryUpgrade = availableAccessoryUpgrades[UnityEngine.Random.Range(0, availableAccessoryUpgrades.Count)];
                availableAccessoryUpgrades.Remove(chosenAccessoryUpgrade);


                if (chosenAccessoryUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool isLevelUp = false;
                    for (int i = 0; i < accessorySlots.Count; i++)
                    {
                        Accessory a = accessorySlots[i].item as Accessory;
                        if (a != null && a.data == chosenAccessoryUpgrade)
                        {
                            if (chosenAccessoryUpgrade.maxLevel <= a.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpAccessory(i));
                            Accessory.Modifier nextLevel = chosenAccessoryUpgrade.GetLevelData(a.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenAccessoryUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenAccessoryUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenAccessoryUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosenAccessoryUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chosenAccessoryUpgrade.icon;
                    }
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

}
