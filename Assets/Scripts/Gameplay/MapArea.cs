using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;

    private void Start() 
    {
        int totalChance = 0;
        foreach (var record in wildPokemons)
        {
            record.changeLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }
    }

    public Pokemon GetRandomWildPokemon()
    {
        int randVal = Random.Range(1, 101);
        var pokemonRecord = wildPokemons.First(p => randVal >= p.changeLower && randVal <= p.chanceUpper);

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
