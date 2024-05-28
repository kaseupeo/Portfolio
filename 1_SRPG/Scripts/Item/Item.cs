using System;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected string name;
    [SerializeField] protected Sprite sprite;
    [SerializeField] protected string description;
    protected Tile currentTile;
    
    public string Name { get => name; set => name = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public Tile CurrentTile { get => currentTile; set => currentTile = value; }
    public string Description { get => description; set => description = value; }
    
    public void Init()
    {
        
    }
}