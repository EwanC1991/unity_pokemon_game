using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeReference] 
    [SerializeField] List<CutsceneAction> actions;

    public void AddAction(CutsceneAction action)
    {
        actions.Add(action);
    }
}
