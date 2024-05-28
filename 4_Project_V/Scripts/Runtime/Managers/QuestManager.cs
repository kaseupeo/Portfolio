using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class QuestManager
{
    private const string SaveRootPath = "questManager";
    private const string ActiveQuestsSavePath = "_activeQuests";
    private const string CompletedQuestsSavePath = "_completedQuests";
    private const string ActiveAchievementsSavePath = "_activeAchievements";
    private const string CompletedAchievementsSavePath = "_completedAchievements";
    
    #region 이벤트

    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);

    public event QuestRegisteredHandler OnQuestRegistered;
    public event QuestCompletedHandler OnQuestCompleted;
    public event QuestCanceledHandler OnQuestCanceled;

    public event QuestRegisteredHandler OnAchievementRegistered;
    public event QuestCompletedHandler OnAchievementCompleted;

    #endregion
    
    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();

    private List<Quest> _activeAchievements = new List<Quest>();
    private List<Quest> _completedAchievements = new List<Quest>();

    private BaseObjectDB _questDB;
    private BaseObjectDB _achievementDB;
    
    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuests;
    
    public IReadOnlyList<Quest> ActiveAchievements => _activeAchievements;
    public IReadOnlyList<Quest> CompletedAchievements => _completedAchievements;

    public void Init()
    {
        _questDB = Managers.Resource.Load<BaseObjectDB>("SO/DB/QuestDB");
        _achievementDB = Managers.Resource.Load<BaseObjectDB>("SO/DB/AchievementDB");

        if (!Load())
        {
            // foreach (var obj in _achievementDB.DataList)
            // {
            //     var achievement = (Quest)obj;
            //     Register(achievement);
            // }

            foreach (var obj in _questDB.DataList)
            {
                var quest = (Quest)obj;
                if (quest.IsAcceptable && !ContainsInCompleteQuests(quest)) 
                    Register(quest);
            }
        }
    }

    public bool Check(Quest quest)
    {
        return quest.IsAcceptable && quest.State == Define.QuestState.Inactive &&
               _activeQuests.All(x => x.CodeName != quest.CodeName) &&
               _completedQuests.All(x => x.CodeName != quest.CodeName);
    }
    
    public Quest Register(Quest quest)
    {
        // TODO
        var newQuest = quest.Clone() as Quest;

        if (newQuest is Achievement)
        {
            newQuest.OnCompleted += AchievementCompleted;
            
            _activeAchievements.Add(newQuest);
            
            newQuest.OnRegister();
            OnAchievementRegistered?.Invoke(newQuest);
        }
        else
        {
            newQuest.OnCompleted += QuestCompleted;
            newQuest.OnCanceled += QuestCanceled;
            
            _activeQuests.Add(newQuest);
            
            newQuest.OnRegister();
            OnQuestRegistered?.Invoke(newQuest);
        }

        return newQuest;
    }

    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        foreach (Quest quest in quests.ToArray()) 
            quest.ReceiveReport(category, target, successCount);
    }
    
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(_activeQuests, category, target, successCount);
        ReceiveReport(_activeAchievements, category, target, successCount);
    }

    public void ReceiveReport(Category category, TaskTarget target, int successCount)
    {
        ReceiveReport(category.CodeName, target.Value, successCount);
    }

    public void CompleteWaitingQuests()
    {
        foreach (Quest quest in _activeQuests.ToList())
        {
            if (quest.IsCompletable) 
                quest.Complete();
        }
    }
    
    public bool ContainsInActiveQuests(Quest quest)
    {
        return _activeQuests.Any(x => x.ID == quest.ID);
    }
    
    public bool ContainsInCompleteQuests(Quest quest)
    {
        return _completedQuests.Any(x => x.ID == quest.ID);
    }
    
    public bool ContainsInActiveAchievements(Quest quest)
    {
        return _activeAchievements.Any(x => x.ID == quest.ID);
    }
    
    public bool ContainsInCompleteAchievements(Quest quest)
    {
        return _completedAchievements.Any(x => x.ID == quest.ID);
    }

    #region 저장 & 불러오기

    // 게임의 세이브 방식에 따라 세이브 함수를 사용하는 위치 변경
    public void Save()
    {
        var root = new JObject();
        root.Add(ActiveQuestsSavePath, CreateSaveData(_activeQuests));
        root.Add(CompletedQuestsSavePath, CreateSaveData(_completedQuests));
        root.Add(ActiveAchievementsSavePath, CreateSaveData(_activeAchievements));
        root.Add(CompletedAchievementsSavePath, CreateSaveData(_completedAchievements));
        
        PlayerPrefs.SetString(SaveRootPath, root.ToString());
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        if (PlayerPrefs.HasKey(SaveRootPath))
        {
            var root = JObject.Parse(PlayerPrefs.GetString(SaveRootPath));

            LoadSaveData(root[ActiveQuestsSavePath], _questDB, LoadActiveQuest);
            LoadSaveData(root[CompletedQuestsSavePath], _questDB, LoadCompletedQuest);
            
            LoadSaveData(root[ActiveAchievementsSavePath], _achievementDB, LoadActiveQuest);
            LoadSaveData(root[CompletedAchievementsSavePath], _achievementDB, LoadCompletedQuest);

            return true;
        }

        return false;
    }
    
    private JArray CreateSaveData(IReadOnlyList<Quest> quests)
    {
        var saveData = new JArray();
        
        foreach (Quest quest in quests)
        {
            if (quest.IsSavable) 
                saveData.Add(JObject.FromObject(quest.ToSaveData()));
        }

        return saveData;
    }

    private void LoadSaveData(JToken dataToken, BaseObjectDB db, Action<QuestSaveData, Quest> onSuccess)
    {
        var data = dataToken as JArray;

        foreach (JToken token in data)
        {
            var saveData = token.ToObject<QuestSaveData>();
            Quest quest = db.GetDataCodeName(saveData.CodeName) as Quest;

            onSuccess.Invoke(saveData, quest);
        }
    }

    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone() as Quest;
        newQuest.LoadFrom(saveData);

        if (newQuest is Achievement)
            _completedAchievements.Add(newQuest);
        else
            _completedQuests.Add(newQuest);
    }

    #endregion

    #region 콜백

    private void QuestCompleted(Quest quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);

        OnQuestCompleted?.Invoke(quest);
    }

    private void QuestCanceled(Quest quest)
    {
        _activeQuests.Remove(quest);
        
        OnQuestCanceled?.Invoke(quest);

        Object.Destroy(quest, Time.deltaTime);
    }

    private void AchievementCompleted(Quest achievement)
    {
        _activeAchievements.Remove(achievement);
        _completedAchievements.Add(achievement);

        OnAchievementCompleted?.Invoke(achievement);
    }

    #endregion
}