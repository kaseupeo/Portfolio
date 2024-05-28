using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Creature
{
    [SerializeField]
    private int dropExp;
    [SerializeField]
    private List<Item> dropItems;
    
    public int DropExp { get => dropExp; set => dropExp = value; }
    public List<Item> DropItems { get => dropItems; set => dropItems = value; }

    public override void Init()
    {
        base.Init();
    }

    protected override IEnumerator Dead()
    {
        yield return StartCoroutine(base.Dead());
        yield return new WaitForSeconds(0.5f);
        Managers.Game.Monsters.Remove(this);
        currentTile.IsBlocked = false;
        Drop();
        Destroy(gameObject);
    }

    private void Drop()
    {
        Item item = RandomItem();
        Item dropItem = Instantiate(item.gameObject, transform.position + new Vector3(0, 0, -0.25f), Quaternion.identity).GetComponent<Item>();
        dropItem.CurrentTile = currentTile;
        dropItem.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        Managers.Game.FieldItems.Add(dropItem);
        foreach (PlayerCharacter playerCharacter in Managers.Game.PlayerCharacters)
        {
            playerCharacter.Exp += dropExp;
        }
    }

    private Item RandomItem()
    {
        return dropItems[Random.Range(0, dropItems.Count)];
    }
}