using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName ="Pokemon/Create new Pokemon")]

public class PokemonBase  : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;

    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // Base stats

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;


    [SerializeField] int catchRate = 255;

    [SerializeField] List<LearnableMove> learnableMoves;

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return (6 / 5 * (level * level * level)) - (15 * (level * level)) + (100 * level) - 140;
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return (5 * (level * level * level)) / 4;
        }
        else if (growthRate == GrowthRate.Fluctuating)
        {
            if (level < 15)
            {
                return (level * level * level) * (((level + 1) / 3) + 24) / 50;
            }
            else if (15 <= level && level < 36)
            {
                return (level * level * level) * (level + 14) / 50;
            }
            else if (36 <= level && level < 100)
            {
                return (level * level * level) * ((level / 2) + 32) / 50; 
            }
        }
        else if (growthRate == GrowthRate.Erratic)
        {
            if (level < 50)
            {
                return (level * level * level) * (100 - level) / 50;
            }
            else if (50 <= level && level < 68)
            {
                return (level * level * level) * (150 - level) / 100;
            }
            else if (68 <= level && level < 98)
            {
                return (level * level * level) * ((1911 - (10 * level)) / 3) / 500;
            }
            else if (98 <= level && level < 100)
            {
                return (level * level * level) * (160 - level) / 100;
            }
        }
        return -1;
    }

    public string Name{
        get { return name; }
    }

    public string Description{
        get { return description; }
    }

    public Sprite FrontSprite{
        get { return frontSprite; }
    }

    public Sprite BackSprite{
        get { return backSprite; }
    }

    public PokemonType Type1{
        get { return type1; }
    }

    public PokemonType Type2{
        get { return type2; }
    }

    public int MaxHp{
        get { return maxHp; }
    }

    public int Attack{
        get { return attack; }
    }

    public int Defense{
        get { return defense; }
    }

    public int SpAttack {
        get { return spAttack; }
    }

    public int SpDefense {
        get { return spDefense; }
    }

    public int Speed {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
    }

    public int CatchRate => catchRate;

    public int ExpYield => expYield;

    public GrowthRate GrowthRate => growthRate;

}

[System.Serializable]
public class LearnableMove{
    
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base {
        get { return moveBase; }
    }

    public int Level {
        get { return level; }
    }
}



public enum PokemonType{
    None,
    Normal,
    Fighting,
    Flying,
    Poison,
    Ground,
    Rock,
    Bug,
    Ghost,
    Steel,
    Fire,
    Water,
    Grass,
    Electric,
    Psychic,
    Ice,
    Dragon,
    Dark,
    Fairy
}

public enum GrowthRate
{
    Erratic, Fast, MediumFast, MediumSlow, Slow, Fluctuating 
}

public enum Stat
{
    Attack,
    Defense, 
    SpAttack, 
    SpDefense, 
    Speed,

// These 2 are not actual stats, they're used to boost moveAccuracy
    Accuracy,
    Evasion
}

// TYPE INDEXES
// NON - 0
// NOR - 1
// FIG - 2 
// FLY - 3
// POI - 4 
// GRO - 5
// ROC - 6 
// BUG - 7 
// GHO - 8 
// STE - 9 
// FIR - 10 
// WAT - 11 
// GRA - 12 
// ELE - 13 
// PSY - 14 
// ICE - 15
// DRA - 16
// DAR - 17
// FAI - 18


public class TypeChart
{
    static float [][] chart =
    {
        //                     NOR   FIG   FLY     POI   GRO   ROC   BUG   GHO   STE   FIR   WAT   GRA   ELE   PSY   ICE   DRA   DAR   FAI
        /* NOR */ new float[] {1f,   1f,   1f,     1f,   1f,   0.5f,  1f,  0f,   0.5f,  1f,  1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f },
        /* FIG */ new float[] {2f,   1f,  0.5f,    0.5f, 1f,   2f,   0.5f, 0f,   2f,    1f,  1f,   1f,   1f,   0.5f, 2f,   1f,   2f,   0.5f },
        /* FLY */ new float[] {1f,   2f,   1f,     1f,   1f,   0.5f,  2f,  1f,   0.5f,  1f,  1f,   2f,   0.5f, 1f,   1f,   1f,   1f,   1f },
        /* POI */ new float[] {1f,   1f,   1f,     0.5f, 0.5f, 0.5f,  1f,  0.5f, 0f,    1f,  1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f },
        /* GRO */ new float[] {1f,   1f,   0f,     2f,   1f,   2f,   0.5f, 1f,   2f,    2f,  1f,  0.5f,  2f,   1f,   1f,   1f,   1f,   1f },
        /* ROC */ new float[] {1f,  0.5f,  2f,     1f,   0.5f, 1f,    2f,  1f,   0.5f,  2f,  1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f },
        /* BUG */ new float[] {1f,  0.5f,  0.5f,   0.5f, 1f,   1f,    1f,  0.5f, 0.5f,  0.5f, 1f,  2f,   1f,   2f,   1f,   1f,   2f,   0.5f },
        /* GHO */ new float[] {0f,   1f,   1f,     1f,   1f,   1f,    1f,  2f,   1f,    1f,   1f,  1f,   1f,   2f,   1f,   1f,   0.5f, 1f },
        /* STE */ new float[] {1f,   1f,   1f,     1f,   1f,   2f,    1f,  1f,   0.5f,  0.5f, 0.5f,1f,   0.5f,  1f,  2f,   1f,    1f,  2f },
        /* FIR */ new float[] {1f,   1f,   1f,     1f,   1f,   0.5f,  2f,  1f,   2f,    0.5f, 0.5f,2f,   1f,    1f,  1f,   0.5f,  1f,  1f },
        /* WAT */ new float[] {1f,   1f,   1f,     1f,   2f,   2f,    1f,   1f,  1f,    2f,   0.5f,0.5f, 1f,    1f,  1f,   0.5f,  1f,  1f },
        /* GRA */ new float[] {1f,   1f,   0.5f,   0.5f, 2f,   2f,    0.5f, 1f,  0.5f,  0.5f, 2f,  0.5f, 1f,    1f,  1f,   0.5f,  1f,  1f },
        /* ELE */ new float[] {1f,   1f,   2f,     1f,   0f,   1f,    1f,   1f,  1f,    1f,   2f,  0.5f, 0.5f,  1f,  1f,   0.5f,  1f,  1f },
        /* PSY */ new float[] {1f,   2f,   1f,     2f,   1f,   1f,    1f,   1f,  0.5f,  1f,   1f,  1f,   1f,   0.5f, 1f,   1f,    0f,  1f },
        /* ICE */ new float[] {1f,   1f,   2f,     1f,   2f,   1f,    1f,   1f,  0.5f,  0.5f, 0.5f,2f,   1f,    1f,  0.5f, 2f,    1f,  1f },
        /* DRA */ new float[] {1f,   1f,   1f,     1f,   1f,   1f,    1f,   1f,  0.5f,  1f,   1f,   1f,  1f,    1f,  1f,   2f,    1f,  0f },
        /* DAR */ new float[] {1f,  0.5f,  1f,     1f,   1f,   1f,    1f,   2f,  1f,    1f,   1f,   1f,  1f,    2f,  1f,   1f,   0.5f, 0.5f },
        /* FAI */ new float[] {1f,  2f,    1f,     0.5f, 1f,   1f,    1f,   1f,  0.5f,  0.5f, 1f,   1f,  1f,    1f,  1f,   2f,   2f,   1f },

        
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        int row = (int) attackType - 1;
        int col = (int) defenseType - 1;

        return chart[row][col];
    }
}
