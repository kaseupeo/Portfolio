using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager
{
    private BaseObjectDB itemDB;

    private BaseObjectDB weaponDB;
    private BaseObjectDB consumptionDB;
    private BaseObjectDB etcDB;
    

    private Dictionary<string, SO_Item> itemDic = new();
    private Dictionary<string, Equipment> equipDic = new();
    private Dictionary<string, SO_CountableItem> consumptionDic = new();
    private Dictionary<string, SO_CountableItem> etcItemDic = new();

    public IReadOnlyDictionary<string, SO_Item> ItemDic => itemDic;
    public IReadOnlyDictionary<string, Equipment> EquipDic => equipDic;
    public IReadOnlyDictionary<string, SO_CountableItem> EtcItemDic => etcItemDic;
    public IReadOnlyDictionary<string, SO_CountableItem> ConsumptionDic => consumptionDic;


    private void SetItemData()
    {
        itemDB = Managers.Resource.Load<BaseObjectDB>("SO/DB/SO_ItemDB");
        for (int i = 0; i < itemDB.DataList.Count; i++)
        {
            if (false == itemDic.ContainsKey(itemDB.DataList[i].CodeName))
            {
                itemDic[$"{itemDB.DataList[i].CodeName}"] = itemDB.GetDataByID<SO_Item>(i);
            }
        }
    }
    private void SetWeaponData()
    {
        weaponDB = Managers.Resource.Load<BaseObjectDB>("SO/DB/EquipmentDB");

        for (int i = 0; i < weaponDB.DataList.Count; i++)
        {
            if (false == equipDic.ContainsKey(weaponDB.DataList[i].CodeName))
            {
                equipDic[$"{weaponDB.DataList[i].CodeName}"] = weaponDB.GetDataByID<Equipment>(i);
            }
        }
    }
    private void SetConsumptionData()
    {
        consumptionDB = Managers.Resource.Load<BaseObjectDB>("SO/DB/ConsumptionDB");
        for (int i = 0; i < consumptionDB.DataList.Count; i++)
        {
            if (consumptionDic.ContainsKey(consumptionDB.DataList[i].CodeName))
            {
                consumptionDic[$"{consumptionDB.DataList[i].CodeName}"] = consumptionDB.GetDataByID<SO_CountableItem>(i);
            }
        }
    }
    private void SetEtcData()
    {
        etcDB = Managers.Resource.Load<BaseObjectDB>("SO/DB/EtcItemDB");
        for (int i = 0; i < etcDB.DataList.Count; i++)
        {
            if (etcItemDic.ContainsKey(etcDB.DataList[i].CodeName))
            {
                etcItemDic[$"{etcDB.DataList[i].CodeName}"] = etcDB.GetDataByID<SO_CountableItem>(i);
            }
        }
    }

    public void Init()
    {
        SetItemData();
        SetWeaponData();
        SetConsumptionData();
        SetEtcData();
    }

    public Item SetItemData(string CodeName, int amount = 1)
    {
        var data = itemDic[CodeName]; 
        Item item = null;

        switch (data.Type)
        {
            case Define.ItemType.Equipment:
                return item = new EquipmentItem((Equipment)data);
            case Define.ItemType.Consume:
                return item = new Consumption((SO_CountableItem)data, amount);
            case Define.ItemType.Etc:
                return item = new ETC((SO_CountableItem)data, amount);
            default:
                Debug.Log("SetItemData 오류");
                break;
        }

        return null;
    }
    
    public Item SetItemData(SO_Item data, int amount = 1)
    {
        // if (!itemDic.ContainsValue(data))
        //     return null;

        if (data == null)
            return null;
        
        Item item = null;

        switch (data.Type)
        {
            case Define.ItemType.Equipment:
                return item = new EquipmentItem((Equipment)data);
            case Define.ItemType.Consume:
                return item = new Consumption((SO_CountableItem)data, amount);
            case Define.ItemType.Etc:
                return item = new ETC((SO_CountableItem)data, amount);
            default:
                Debug.Log("SetItemData 오류");
                break;
        }

        return null;
    }
}
