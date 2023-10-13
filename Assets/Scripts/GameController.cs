using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GDEUtils.StateMachine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused, Evolution, Shop }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    GameState state;

    GameState prevState;
    GameState stateBeforeEvolution;

    public StateMachine<GameController> StateMachine { get; private set; }

   public SceneDetails CurrentScene { get; private set; }

   public SceneDetails PrevScene { get; private set; }

   public static GameController Instance;

   private void Awake() 
   {
      Instance = this;

      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;

      PokemonDB.Init();
      MoveDB.Init();
      ConditionsDB.Init();
      ItemDB.Init();
      QuestDB.Init();
   }

   private void Start() 
   {
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.i);
        
        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () =>
        {
          prevState = state;
          state = GameState.Dialog;
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
          if (state == GameState.Dialog)
               state = prevState;
        };

        EvolutionManager.i.OnStartEvolution += () => 
        {
          stateBeforeEvolution = state;
          state = GameState.Evolution;
        };
        EvolutionManager.i.OnCompleteEvolution += () => 
        {
          partyScreen.SetPartyData();
          state = stateBeforeEvolution;

          AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);
        };

        ShopController.i.OnStart += () => state = GameState.Shop;
        ShopController.i.OnFinish += () => state = GameState.FreeRoam;
   }

   public void PauseGame(bool pause)
   {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else 
        {
            state = prevState;
        }
   }

    public void StartCutsceneState()
    {
        state = GameState.Cutscene;
    }

    public void StartFreeRoamState()
    {
        state = GameState.FreeRoam;
    }

   public void StartBattle(BattleTrigger trigger) 
   {
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
   }

   TrainerController trainer;

   public void StartTrainerBattle(TrainerController trainer) 
   {
        BattleState.i.trainer = trainer;
        StateMachine.Push(BattleState.i);
   }


   public void OnEnterTrainersView(TrainerController trainer)
   {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
   }

   void EndBattle(bool won)
   {
     

     partyScreen.SetPartyData();
        
     state = GameState.FreeRoam;
     battleSystem.gameObject.SetActive(false);
     worldCamera.gameObject.SetActive(true);

     var playerParty = playerController.GetComponent<PokemonParty>();
     bool hasEvolutions = playerParty.CheckForEvolutions();

    if (hasEvolutions)
        StartCoroutine(playerParty.RunEvolutions());
    else 
        AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);

   }

   private void Update() 
   {
        StateMachine.Execute();

        if (state == GameState.Cutscene)
        {
            playerController.Character.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
          DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Shop)
        {
            ShopController.i.HandleUpdate();
        }

        

   }

   public void SetCurrentScene(SceneDetails currScene)
   {
      PrevScene = CurrentScene;
      CurrentScene = currScene;
   }

   void OnMenuSelected(int selectedItem)
   {
      if (selectedItem == 0)
      {
          // Pokemon
          partyScreen.gameObject.SetActive(true);
          state = GameState.PartyScreen;
      }
      else if (selectedItem == 1)
      {
          // Bag
          inventoryUI.gameObject.SetActive(true);
          state = GameState.Bag;
      }
      else if (selectedItem == 2)
      {
          // Save 1
          SavingSystem.i.Save("saveSlot1");
          state = GameState.FreeRoam;
      }
      else if (selectedItem == 3)
      {
          // Load 1
          SavingSystem.i.Load("saveSlot1");
          partyScreen.SetPartyData();
          state = GameState.FreeRoam;
      }
    
      
      
   }

   public IEnumerator MoveCamera(Vector2 moveOffset, bool WaitForFadeOut=false)
   {
      yield return Fader.i.FadeIn(0.5f);

      worldCamera.transform.position += new Vector3(moveOffset.x, moveOffset.y);

      if (WaitForFadeOut)
        yield return Fader.i.FadeOut(0.5f);
      else
        StartCoroutine(Fader.i.FadeOut(0.5f));
   }

    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 24;
        GUILayout.Label("STATE STACK", style);

        foreach (var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style);
        }
    }

    public GameState State => state;

    public PlayerController PlayerController => playerController;
    public Camera WorldCamera => worldCamera;
}
