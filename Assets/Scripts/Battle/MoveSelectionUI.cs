using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GDE.GenericSelectionUI;
using System.Linq;

public class MoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] List<Text> moveTexts;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i=0; i<currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;

        SetItems(moveTexts.Select(m => m.GetComponent<TextSlot>()).ToList());
    }
}
