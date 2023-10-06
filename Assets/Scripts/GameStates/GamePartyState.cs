using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public static GamePartyState i { get; private set; }

    private void Awake()
    {
        i = this; 
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;

    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;

    }

    void OnPokemonSelected(int selection)
    {
        if (gc.StateMachine.GetPrevState() == InventoryState.i)
        {
            // Use Item
            StartCoroutine(GoToUseItemState());
        }
        else
        {
            // TODO: Open Summary Screen
            Debug.Log($"Selected Pokemon at index {selection}");
        }
        
    }

    IEnumerator GoToUseItemState()
    {
        yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }

    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}