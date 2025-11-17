using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterData characterData;
    public GameObject confirmButton;

     void Awake()
    {
       if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        confirmButton.SetActive(false);
    }
    public static CharacterData GetData()
    {
        if(instance && instance.characterData)
            return instance.characterData;
        else
        {
            CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
            return characters[Random.Range(0, characters.Length)];
        }

        

    }

    public void SelectCharacter(CharacterData character)
    {
        characterData = character;
        confirmButton.SetActive(true);
    }

    public void DestroyInstance()
    {
        instance = null;
        Destroy(gameObject);
    }
}
