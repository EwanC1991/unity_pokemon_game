using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTriggerable
{
    bool isJumpingToWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {

        var animator = initiator.GetComponent<CharacterAnimator>();

        if (animator.IsSurfing || isJumpingToWater)
            yield break;

        yield return DialogManager.Instance.ShowDialogText("The water is calm and blue...");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Surf"));

        if (pokemonWithSurf != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {pokemonWithSurf.Base.Name} use SURF?",
                choices: new List<string>() { "Yes", "No"},
                onChoiceSelected: (selection) => selectedChoice = selection );

            if (selectedChoice == 0)
            {
                // Yes
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithSurf.Base.Name} used SURF!");

                
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                isJumpingToWater = false;

                animator.IsSurfing = true;
            }
        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) <= 5)
        {
            GameController.Instance.StartBattle(BattleTrigger.Water);
        }
    }
}
