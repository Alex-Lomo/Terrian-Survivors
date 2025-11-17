using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drop
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }
    public List<Drop> drops;
    
    void OnDestroy()
    {   
        if(!gameObject.scene.isLoaded)
        {
            return;
        }


        float rand = UnityEngine.Random.Range(0f, 100f);
        List<Drop> possible=new List<Drop>();
        foreach (Drop drop in drops)
        {
            if(rand<= drop.dropRate)
            {
                possible.Add(drop);
                
            }
        }
        if(possible.Count>0)
        {
            Drop drop = possible[UnityEngine.Random.Range(0, possible.Count)];
            Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
