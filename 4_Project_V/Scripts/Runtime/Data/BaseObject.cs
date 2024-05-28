using System;
using System.Linq;
using UnityEngine;

public class BaseObject : ScriptableObject, ICloneable
{
    [SerializeField] private Category[] categories;

    [SerializeField] private int id = -1;
    [SerializeField] private Sprite icon;
    [SerializeField] private string codeName;
    [SerializeField] private string displayName;
    [SerializeField] private string description;

    public Category[] Categories => categories;
    public int ID => id;
    public Sprite Icon => icon;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    public virtual string Description => description;

    public virtual object Clone() => Instantiate(this);

    public bool HasCategory(Category category) 
        => categories.Any(x => x.ID == category.ID);

    public bool HasCategory(string category) 
        => categories.Any(x => x == category);

    public bool HasCategory(Category[] categories)
    {
        bool hasCategory = false;
        foreach (Category category in categories)
            hasCategory = hasCategory || HasCategory(category);

        return hasCategory;
    }
}