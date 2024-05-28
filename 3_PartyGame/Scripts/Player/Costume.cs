using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Costume : MonoBehaviour
{
    //CharacterCustomizer
    [SerializeField] private CostumeType type;
    [SerializeField] private List<GameObject> costumeList;
    [SerializeField] private bool isNone;

    public int CurrentCostume { get; set; }
    public CostumeType Type => type;

    private void Awake()
    {
        CurrentCostume = isNone ? -1 : 0;
    }

    public void LoadCostume(int currentCostume)
    {
        foreach (GameObject go in costumeList) 
            go.SetActive(false);

        if (currentCostume == -1)
            return;

        costumeList[currentCostume].SetActive(true);
    }
    
    public void NextCostume()
    {
        int currentNum = -1;
        for (int i = 0; i < costumeList.Count; i++)
        {
            if (costumeList[i].activeSelf) 
                currentNum = i;

            costumeList[i].SetActive(false);
        }
        
        if (isNone && (currentNum + 1) % (costumeList.Count + 1) == costumeList.Count)
        {
            CurrentCostume = -1;
            return;
        }

        CurrentCostume = (currentNum + 1) % costumeList.Count;
        costumeList[CurrentCostume].SetActive(true);
    }

    public void PrevCostume()
    {
        int currentNum = costumeList.Count;
        for (int i = 0; i < costumeList.Count; i++)
        {
            if (costumeList[i].activeSelf)
                currentNum = i;

            costumeList[i].SetActive(false);
        }

        if (isNone && (currentNum - 1) % (costumeList.Count + 1) == costumeList.Count)
        {
            CurrentCostume = -1;
            return;
        }

        CurrentCostume = (currentNum - 1 < 0 ? currentNum - 1 + costumeList.Count : currentNum - 1) %
                          costumeList.Count;
        costumeList[CurrentCostume].SetActive(true);
    }
    
    [ContextMenu("Update Costume List")]
    private void UpdateCostume()
    {
        costumeList = new List<GameObject>();
        foreach (Transform child in transform)
        {
            costumeList.Add(child.gameObject);
        }
    }
}

