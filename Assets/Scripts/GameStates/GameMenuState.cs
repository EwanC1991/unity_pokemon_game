using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : State<GameController>
{
    [SerializeField] MenuController menuController;


    public static GameMenuState i { get; private set; }

    public void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;
        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnBack;

    }

    public override void Execute()
    {
        menuController.HandleUpdate();
    }

    public override void Exit()
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnBack;
    }

    void OnMenuItemSelected(int selection)
    {
        if (selection == 0) //Pokemon
            gc.StateMachine.Push(GamePartyState.i);
    }

    void OnBack()
    {
            gc.StateMachine.Pop();
    }
}
