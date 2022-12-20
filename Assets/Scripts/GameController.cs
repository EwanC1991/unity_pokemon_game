using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
   GameState state;

   public static GameController Instance;

   private void Awake() 
   {
    Instance = this;
    ConditionsDB.Init();
   }

   private void Start() 
   {
        
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
          state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
          if (state == GameState.Dialog)
               state = GameState.FreeRoam;
        };
   }

   public void StartBattle() 
   {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty, wildPokemonCopy);
   }

   TrainerController trainer;

   public void StartTrainerBattle(TrainerController trainer) 
   {
     state = GameState.Battle;
     battleSystem.gameObject.SetActive(true);
     worldCamera.gameObject.SetActive(false);

     this.trainer = trainer;
     var playerParty = playerController.GetComponent<PokemonParty>();
     var trainerParty = trainer.GetComponent<PokemonParty>();


     battleSystem.StartTrainerBattle(playerParty, trainerParty);
   }


   public void OnEnterTrainersView(TrainerController trainer)
   {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
   }

   void EndBattle(bool won)
   {
     if (trainer!= null && won == true)
     {
          trainer.BattleLost();
          trainer = null;
     }
        
     state = GameState.FreeRoam;
     battleSystem.gameObject.SetActive(false);
     worldCamera.gameObject.SetActive(true);
   }

   private void Update() 
   {
        if (state == GameState.FreeRoam)
        {
          playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
          battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
          DialogManager.Instance.HandleUpdate();
        }

   }
}
