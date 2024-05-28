using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseObject DB")]
public class BaseObjectDB : ScriptableObject
{
    [SerializeField] private List<BaseObject> dataList = new List<BaseObject>();

    public IReadOnlyList<BaseObject> DataList => dataList;
    public int Count => dataList.Count;

    public BaseObject this[int index] => dataList[index];

    private void SetID(BaseObject target, int id)
    {
        var field = typeof(BaseObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        field.SetValue(target, id);

#if UNITY_EDITOR
        EditorUtility.SetDirty(target);
#endif
    }

    private void ReorderDataList()
    {
        var field = typeof(BaseObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < dataList.Count; i++)
        {
            field.SetValue(dataList[i], i);

#if UNITY_EDITOR
            EditorUtility.SetDirty(dataList[i]);
#endif
        }
    }

    public void Add(BaseObject newData)
    {
        dataList.Add(newData);
        SetID(newData, dataList.Count - 1);
    }

    public void Remove(BaseObject data)
    {
        dataList.Remove(data);
        ReorderDataList();
    }

    public BaseObject GetDataByID(int id) 
        => dataList[id];
    
    public T GetDataByID<T>(int id) where T : BaseObject 
        => GetDataByID(id) as T;

    public BaseObject GetDataCodeName(string codeName) 
        => dataList.Find(x => x.CodeName == codeName);

    public T GetDataCodeName<T>(string codeName) where T : BaseObject
        => GetDataCodeName(codeName) as T;

    public bool Contains(BaseObject obj)
        => dataList.Contains(obj);

    public void SortByCodeName()
    {
        dataList.Sort((x, y) => String.Compare(x.CodeName, y.CodeName, StringComparison.Ordinal));
        ReorderDataList();
    }
}