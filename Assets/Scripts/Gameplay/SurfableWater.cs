using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class SurfableWater : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
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

                var animator = initiator.GetComponent<CharacterAnimator>();
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                animator.IsSurfing = true;
            }
        }
    }
}
