using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI selectionUI;

    BattleSystem bs;

    public static ActionSelectionState i { get; private set; }


    private void Awake()
    {
        i = this;
    }
    public override void Enter(BattleSystem owner)
    {
        bs = owner; 

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnActionSelected;

        bs.DialogBox.TypeDialog("Choose an action");
    }

    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnActionSelected;
    }

    private void OnActionSelected(int selection)
    {
        if (selection == 0)
        {
            // FIGHT
            MoveSelectionState.i.Moves = bs.PlayerUnit.Pokemon.Moves;
            bs.StateMachine.ChangeState(MoveSelectionState.i);
        }
    }
}
