using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureCustomizer : MonoBehaviour
{
    private Costume[] _costumes;
    private Dictionary<Category, List<GameObject>> _costumeDic = new();

    public IReadOnlyList<Costume> CostumeList => _costumes;
    
    private void Awake()
    {
        _costumes = transform.GetComponentsInChildren<Costume>();

        foreach (Costume costume in _costumes)
        {
            _costumeDic[costume.Category] = new List<GameObject>();
            
            foreach (Transform child in costume.transform)
                _costumeDic[costume.Category].Add(child.gameObject);
        }
    }

    private void OnEnable()
    {
        Managers.UI.OnPrevCostume += PrevCostume;
        Managers.UI.OnNextCostume += NextCostume;
    }

    private void OnDisable()
    {
        Managers.UI.OnPrevCostume -= PrevCostume;
        Managers.UI.OnNextCostume -= NextCostume;
    }


    public void LoadCostume(Category category, int currentCostumeNum)
    {
        foreach (GameObject go in _costumeDic[category]) 
            go.SetActive(false);

        _costumeDic[category][currentCostumeNum].SetActive(true);
    }
    
    public int FindCostumeNumber(Category category, out GameObject costume)
    {
        costume = null;

        if (!_costumeDic.ContainsKey(category))
            return -1;
        
        var costumeList = _costumeDic[category];
        
        for (int i = 0; i < costumeList.Count; i++)
        {
            if (!costumeList[i].activeSelf) 
                continue;
            
            costume = costumeList[i];
            
            return i;
        }

        return costumeList.Count;
    }

    public void NextCostume(Category category)
    {
        var costumeList = _costumeDic[category];
        int currentCostumeNum = FindCostumeNumber(category, out var currentCostume);

        if (currentCostumeNum == -1)
            return;

        currentCostume?.SetActive(false);

        if (_costumes.First(x => x.Category == category).IsNone && currentCostumeNum + 1 == costumeList.Count)
            return;

        int nextCostumeNum = currentCostumeNum != costumeList.Count ? (currentCostumeNum + 1) % costumeList.Count : 0;
        costumeList[nextCostumeNum].SetActive(true);
    }

    public void PrevCostume(Category category)
    {
        var costumeList = _costumeDic[category];
        int currentCostumeNum = FindCostumeNumber(category, out var currentCostume);

        if (currentCostumeNum == -1)
            return;
        
        currentCostume?.SetActive(false);
        
        if (_costumes.First(x => x.Category == category).IsNone && currentCostumeNum == 0)
            return;
        
        int prevCostumeNum =
            (currentCostumeNum < 1 ? currentCostumeNum - 1 + costumeList.Count : currentCostumeNum - 1) %
            costumeList.Count;
        costumeList[prevCostumeNum].SetActive(true);
    }
}