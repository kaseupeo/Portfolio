using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerCostumeData : MonoBehaviour
{   
    private Costume[] _costumes;
    
    private void Start()
    {
        _costumes = transform.GetComponentsInChildren<Costume>();

        foreach (Costume costume in _costumes)
        {
            if (Managers.Game.CostumeDic.ContainsKey(costume.Type))
                return;
            
            Managers.Game.CostumeDic[costume.Type] = costume.CurrentCostume;
        }
    }

    public void Save()
    {
        Hashtable playerProperties = new Hashtable();
        
        foreach (Costume costume in _costumes)
        {
            Managers.Game.CostumeDic[costume.Type] = costume.CurrentCostume;

            playerProperties[costume.Type.ToString()] = costume.CurrentCostume;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public void Load(Dictionary<CostumeType, int> dictionary)
    {
        if (dictionary == null)
            return;
        
        _costumes ??= transform.GetComponentsInChildren<Costume>();

        foreach (Costume costume in _costumes)
        {
            costume.LoadCostume(dictionary[costume.Type]);
        }
    }

    public Costume FindByType(CostumeType type)
    {
        return _costumes.First(x => x.Type == type);
    }
}
