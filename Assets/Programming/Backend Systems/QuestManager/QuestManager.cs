using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    public delegate void OnComplete(Quest quest);
    public static event OnComplete OnQuestComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        foreach(Quest q in activeQuests)
        {
            q.BeginQuest();
        }
    }

    private void Update()
    {
        UpdateQuestStatus();
    }

    public void Reset()
    {
        foreach (Quest q in activeQuests)
        {
            q.Reset();
        }

        foreach(Quest q in completedQuests)
        {
            q.Reset();
        }

        activeQuests.Clear();
        completedQuests.Clear();
    }

    private void UpdateQuestStatus()
    {
        List<Quest> completedThisFrame = new List<Quest>();
        if (activeQuests.Count > 0)
        {
            foreach (Quest q in activeQuests)
            {
                bool completed = q.EvaluateQuest();
                if (completed)
                {
                    completedThisFrame.Add(q);
                    OnQuestComplete?.Invoke(q);
                }
            }
        }

        if (completedThisFrame.Count == 0) return;

        foreach(Quest q in completedThisFrame)
        {
            if (!q.display) continue;
            NotificationManager.Instance.RequestNotification(new Notification(q.questName, "Quest", 1), 0);
            NotificationManager.Instance.RequestNotification(new Notification("Completed", "Quest", 1), 0);
        }

        activeQuests = activeQuests.Except(completedThisFrame).ToList();
        completedQuests.AddRange(completedThisFrame);
    }

    public bool GetQuestStatus(Quest quest)
    {
        if (completedQuests.Contains(quest)) return true;
        else return false;
    }

    public bool GetQuestStatus(string questName)
    {
        if (completedQuests.Where(q => q.questName == questName).Any()) return true;
        else return false;
    }

    public void AssignQuest(Quest quest)
    {
        if (completedQuests.Where(q => q == quest).Any() || activeQuests.Where(q => q == quest).Any()) return;

        if (quest.display) NotificationManager.Instance.RequestNotification(new Notification(quest.questName, "Quest"), 0);
        quest.BeginQuest();
        activeQuests.Add(quest);
    }
}
