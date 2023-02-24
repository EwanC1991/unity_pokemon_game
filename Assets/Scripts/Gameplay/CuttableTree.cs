using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CuttableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("This tree looks like it can be cut!");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Cut"));

        if (pokemonWithCut != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {pokemonWithCut.Base.Name} use CUT?",
                choices: new List<string>() { "Yes", "No"},
                onChoiceSelected: (selection) => selectedChoice = selection );

            if (selectedChoice == 0)
            {
                // Yes
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} used CUT!");
                gameObject.SetActive(false);
            }
        }
    }
}
