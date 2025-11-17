using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerStats : MonoBehaviour
{

    public CharacterData characterData;
    public CharacterData.Stats baseStats;
    public CharacterData.Stats growthStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }


    float health;
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if(health!=value)
            {
                health = value;
                
            }
        }
    }

    

    [Header("Level")]
    public float experience = 0;
    public int level=1;
    public float xpCap;

    [Header("IFrames")]
    public float invulDur;
    float invulTimer;
    bool isInvul;

    [Header("UI")]
    public Image healthBar;
    public Image xpBar;
    public TMP_Text levelText;


    

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public float xpCapGrowth;
    }

    public List<LevelRange> levelRanges;

    PlayerInventory inventory;
    PlayerCollector collector;
    

    Animator am;
    SpriteRenderer sr;
    Color damageFlashColor = new Color(255,0,0,60);
    Color blockFlashColor = new Color(0, 0, 255, 60);
    Color regenFlashColor = new Color(0, 255, 0, 60);
    float FlashDuration = 0.2f;
    Color originalColor;
    void Awake()
    {
        characterData = CharacterSelector.GetData();
        if(CharacterSelector.instance)CharacterSelector.instance.DestroyInstance();
        am = GetComponent<Animator>();
        am.SetBool(characterData.CharacterName, true);
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
       

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        baseStats = actualStats = characterData.stats;
        growthStats = characterData.growth;
        collector.SetOwner(this);
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
    }

    private void Start()
    {
        inventory.Add(characterData.StartingWeapon);

        xpCap = levelRanges[0].xpCapGrowth;
        
        GameManager.instance.AssignCharacterUI(characterData);
        UpdateHealthBar();
        UpdateXpBar();
        UpdateLevelText();
    }

    

    private void Update()
    {
        if(invulTimer>0)
        {
            invulTimer -= Time.deltaTime;
        }
        else if(isInvul)
        {
            isInvul = false;
        }
        RegenHealth();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.accessorySlots)
        {
            Accessory a = s.item as Accessory;
            if(a)
            {
                actualStats += a.GetBoosts();
            }
        }
        for (int i = 0; i < level; i++)
        {
            actualStats += growthStats;
        }
        collector.SetRadius(actualStats.magnet);
    }

    public void GainXp(float amount)
    {
        experience += amount;
        LevelUpCheck();
        UpdateXpBar();
    }

    public void LevelUpCheck()
    {
        if(experience>=xpCap)
        {
            level += 1;
            experience -= xpCap;
            float xpCapIncrease=0;
            foreach (LevelRange range in levelRanges)
            {
                if(level>=range.startLevel && level<=range.endLevel)
                {
                    xpCapIncrease = range.xpCapGrowth;
                }
            }
            xpCap += xpCapIncrease;

            UpdateLevelText();

            RecalculateStats();

            GameManager.instance.StartLevelUp();
        }
    }

    public void TakeDamage(float amount)
    {
       


        if(!isInvul)
        {
            amount -= actualStats.armor;
            invulTimer = invulDur;
            isInvul = true;
            if (amount > 0)
            {
                CurrentHealth -= amount;
                StartCoroutine(DamageFlash());
                if (CurrentHealth <= 0)
                {
                    Kill();
                }

                UpdateHealthBar();
            }
            else StartCoroutine(BlockedFlash());
            
        }
        
    }
    IEnumerator DamageFlash()
    {
        sr.color = damageFlashColor;
        yield return new WaitForSeconds(FlashDuration);
        sr.color = originalColor;
    }

    IEnumerator BlockedFlash()
    {
        sr.color = blockFlashColor;
        yield return new WaitForSeconds(FlashDuration);
        sr.color = originalColor;
    }

    IEnumerator HealFlash()
    {
        sr.color = regenFlashColor;
        yield return new WaitForSeconds(FlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        AssignInventoryUI();
        if(!GameManager.instance.isGameOver)
        {   
            
            GameManager.instance.GameOver();
        }
    }

    public void AssignInventoryUI()
    {
        GameManager.instance.AssignWeaponsUI(inventory.weaponSlots);
        GameManager.instance.AssignAccessoriesUI(inventory.accessorySlots);
        GameManager.instance.AssignEnemiesKilled();
        GameManager.instance.AssignLevelReached(level);
        GameManager.instance.AssignTimeSurvived();
    }

    public void RecoverHealth(float hp)
    {
        if(CurrentHealth<actualStats.maxHealth)
        {
            CurrentHealth += hp;
            StartCoroutine(HealFlash());
            if(CurrentHealth>actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            UpdateHealthBar();
            
        }
    }

    void RegenHealth()
    {
        if(CurrentHealth< actualStats.maxHealth)
        {
            CurrentHealth += actualStats.recovery * Time.deltaTime;

            if(CurrentHealth> actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    void UpdateXpBar()
    {
        xpBar.fillAmount = experience / xpCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "LV. " + level.ToString();
    }
}
