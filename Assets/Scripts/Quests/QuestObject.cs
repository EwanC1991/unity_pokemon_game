using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] QuestBase questToCheck;
    [SerializeField] ObjectActions onStart;
    [SerializeField] ObjectActions onComplete;

    QuestList questList;

    private void Start()
    {
        questList = QuestList.GetQuestList();
        questList.OnUpdated += UpdateObjectStatus;

        UpdateObjectStatus();
    }

    private void OnDestroy() 
    {
        questList.OnUpdated -= UpdateObjectStatus;
    }

    public void UpdateObjectStatus()
    {
        if (onStart != ObjectActions.DoNothing && questList.IsStarted(questToCheck.Name))
        {
            foreach(Transform child in transform)
            {
                if (onStart == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);

                    var savable = child.GetComponent<SavableEntity>();
                    if (savable != null)
                        SavingSystem.i.RestoreEntity(savable);

                }
                else if (onStart == ObjectActions.Disable)
                    child.gameObject.SetActive(false);
            }
        }

        if (onComplete != ObjectActions.DoNothing && questList.IsCompleted(questToCheck.Name))
        {
            foreach(Transform child in transform)
            {
                if (onComplete == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);
                    
                    var savable = child.GetComponent<SavableEntity>();
                    if (savable != null)
                        SavingSystem.i.RestoreEntity(savable);
                }
                else if (onComplete == ObjectActions.Disable)
                    child.gameObject.SetActive(false);
            }
        }
    }
}

public enum ObjectActions { DoNothing, Enable, Disable }
