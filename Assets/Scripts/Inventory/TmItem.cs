using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Create new TM or HM")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;

    public override bool Use(Pokemon pokemon)
    {
        // Learning Move is handling rom Inventory UI, If it was learned then return true
        return pokemon.HasMove(move);
    }

    public MoveBase Move => move;
}
