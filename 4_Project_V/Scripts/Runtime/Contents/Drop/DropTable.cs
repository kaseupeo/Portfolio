using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropTable : MonoBehaviour
{
    [SerializeField, Tooltip("최소, 최대 드랍")] private Vector2 gold;
    [SerializeField] private DropData[] items;

    private Creature _creature;

    private void Awake()
    {
        _creature = GetComponent<Creature>();
    }
    
    private void OnEnable()
    {
        _creature.OnDead += Drop;
    }

    private void OnDisable()
    {
        _creature.OnDead -= Drop;
    }

    public void Drop(Creature creature)
    {
        // Item Drop
        foreach (DropData dropData in items)
        {
            var rand = Random.Range(0.0f, 1.0f);

            switch (dropData.IsUseRarityRate)
            {
                case true when rand <= dropData.Item.Rarity.Rate:
                    Managers.Game.Inventory.AddItem(Managers.Data.SetItemData(dropData.Item));
                    break;
                case false when rand <= dropData.Rate:
                    Managers.Game.Inventory.AddItem(Managers.Data.SetItemData(dropData.Item));
                    break;
            }
        }

        // Gold Drop
        {
            var rand = Random.Range(gold.x, gold.y);

            Managers.Game.Player.GainGold((int)rand);
        }
    }
}