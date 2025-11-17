using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerObject;
    

    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    public GameState previousState;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    

    [Header("Results Screen")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public List<Image> chosenWeaponImages = new List<Image>(6);
    public List<TMP_Text> chosenWeaponNames =new List<TMP_Text>(6);
    public List<TMP_Text> chosenWeaponLevels = new List<TMP_Text>(6);
    public List<Image> chosenAccessoryImages= new List<Image>(6);
    public List<TMP_Text> chosenAccessoryNames = new List<TMP_Text>(6);
    public List<TMP_Text> chosenAccessoryLevels = new List<TMP_Text>(6);

    public TMP_Text timeSurvivedDisplay;
    public TMP_Text levelReachedDisplay;
    public TMP_Text enemiesKilledDisplay;


    [Header("Bools")]
    public bool isGameOver=false;
    public bool isChoosingUpgrade = false;

    [Header("Timer")]
    public float timeLimit;
    float elapsedTime=0;
    public TMP_Text timerDisplay;
    public int enemiesKilled=0;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize=10;
    public TMP_FontAsset textFont;
    public Color damageTextColor;
    public Camera referenceCamera;


    void Update()
    {
        switch(currentState)
        {
            case GameState.Gameplay:
                CheckForPause();
                UpdateTimer();
                break;
            case GameState.Paused:
                CheckForPause();
                break;
            case GameState.GameOver:
                if(!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    DisplayResults();
                }
                break;
            case GameState.LevelUp:
                if(!isChoosingUpgrade)
                {
                    isChoosingUpgrade = true;
                    Time.timeScale = 0f;
                    levelUpScreen.SetActive(true);
                }
                break;
            default:
                Debug.LogError("You shouldn't be here.");
                break;
        }
    }

    void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DisableScreens();
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {   

        if(currentState!=GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
        }
        
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
        }
    }

    void CheckForPause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState==GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
        for (int i=0;i<6;i++)
        {
            chosenAccessoryImages[i].enabled = false;
            chosenAccessoryNames[i].enabled = false;
            chosenAccessoryLevels[i].enabled = false;
            chosenWeaponImages[i].enabled = false;
            chosenWeaponNames[i].enabled = false;
            chosenWeaponLevels[i].enabled = false;
        }
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = "Time Survived: " + timerDisplay.text;
        ChangeState(GameState.GameOver);
        
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignCharacterUI(CharacterData characterData)
    {
        chosenCharacterImage.sprite = characterData.Icon;
        chosenCharacterName.text = characterData.CharacterName;
        
    }


    public void AssignWeaponsUI(List<PlayerInventory.Slot> weaponSlots)
    {
        int i = 0;
        foreach (PlayerInventory.Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if(w)
            {
                chosenWeaponImages[i].sprite = s.image.sprite;
                chosenWeaponImages[i].enabled = true;

                chosenWeaponNames[i].text = w.name;
                chosenWeaponNames[i].enabled = true;

                chosenWeaponLevels[i].text = w.currentLevel.ToString();
                chosenWeaponLevels[i].enabled = true;
                i++;
            }
            
        }
    }

    public void AssignAccessoriesUI(List<PlayerInventory.Slot> accessorySlots)
    {
        int i = 0;
        foreach (PlayerInventory.Slot s in accessorySlots)
        {
            Accessory a = s.item as Accessory;
            if (a)
            {
                chosenAccessoryImages[i].sprite = s.image.sprite;
                chosenAccessoryImages[i].enabled = true;

                chosenAccessoryNames[i].text = a.name;
                chosenAccessoryNames[i].enabled = true;

                chosenAccessoryLevels[i].text = a.currentLevel.ToString();
                chosenAccessoryLevels[i].enabled = true;
                i++;
            }

        }
    }


    public void AssignEnemiesKilled()
    {
        enemiesKilledDisplay.text = "Enemies killed: " + enemiesKilled.ToString();
    }

    public void AddKillCount()
    {
        enemiesKilled++;
    }

    public void AssignLevelReached(int levelReached)
    {
        levelReachedDisplay.text = "Level Reached: " + levelReached.ToString();
    }

    public void AssignTimeSurvived()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timeSurvivedDisplay.text = "Time Survived: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimerDisplay();
        if(elapsedTime>=timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime/60);
        int seconds = Mathf.FloorToInt(elapsedTime%60);

        timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        isChoosingUpgrade = false;
        Time.timeScale = 1;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 3f)
    {
        GameObject textObject = new GameObject("Damage Floating Text");
        RectTransform rect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObject.AddComponent<TextMeshProUGUI>();
        
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        tmPro.color = damageTextColor;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        

        textObject.transform.SetParent(instance.damageTextCanvas.transform);
        textObject.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastPosition = target.position;
        while(t<duration)
        {
            

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            if (target)
                lastPosition = target.position;

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastPosition + new Vector3(0, yOffset));

            yield return w;
            t += Time.deltaTime;
        }

        Destroy(textObject);
    }

    public static void GenerateFloatingText(string text, Transform target, float duration=0.25f, float speed =1f)
    {
        if (!instance.damageTextCanvas) return;

        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }
}
