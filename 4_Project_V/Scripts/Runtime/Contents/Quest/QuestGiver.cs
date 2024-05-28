using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private Quest[] quests;

    private void Update()
    {
        foreach (Quest quest in quests)
        {
            if (Managers.Quest.Check(quest))
            {
                Managers.Quest.Register(quest);
            }
        }
    }
}