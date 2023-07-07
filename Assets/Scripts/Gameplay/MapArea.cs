using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonsInWater;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    [HideInInspector]
    [SerializeField] int totalChanceInWater = 0;

    private void OnValidate()
    {
        CalculatePercentage();
    }

    private void Start() 
    {
        CalculatePercentage();
    }

    void CalculatePercentage()
    {
        totalChance = -1;
        totalChanceInWater = -1;

        if (wildPokemons.Count > 0)
        {
            totalChance = 0;
            foreach (var record in wildPokemons)
            {
                record.changeLower = totalChance;
                record.chanceUpper = totalChance + record.chancePercentage;

                totalChance = totalChance + record.chancePercentage;
            }

        }

        if (wildPokemonsInWater.Count > 0)
        {
            totalChanceInWater = 0;
            foreach (var record in wildPokemonsInWater)
            {
                record.changeLower = totalChanceInWater;
                record.chanceUpper = totalChanceInWater + record.chancePercentage;

                totalChanceInWater = totalChanceInWater + record.chancePercentage;
            }
        }
    }

    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        var pokemonList = (trigger == BattleTrigger.LongGrass) ? wildPokemons : wildPokemonsInWater; 
        int randVal = Random.Range(1, 101);
        var pokemonRecord = pokemonList.First(p => randVal >= p.changeLower && randVal <= p.chanceUpper);

        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y+1);

        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        // var wildPokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        wildPokemon.Init();
        return wildPokemon;
    }
}

[System.Serializable]
public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int changeLower { get; set; }
    public int chanceUpper { get; set; }
}
