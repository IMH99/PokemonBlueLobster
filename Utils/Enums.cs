using Godot;
using System;

public class Enums : Node
{
    public enum PokemonTypes
    {
        kPokemonTypes_None,
        kPokemonTypes_Grass,
        kPokemonTypes_Fire,
        kPokemonTypes_Water,
        kPokemonTypes_Normal,
        kPokemonTypes_Bug,
        kPokemonTypes_Poison,
        kPokemonTypes_Flying,
        kPokemonTypes_Electric,
        kPokemonTypes_Rock,
        kPokemonTypes_Ground,
        kPokemonTypes_Steel,
        kPokemonTypes_Ice,
        kPokemonTypes_Psychic,
        kPokemonTypes_Dark,
        kPokemonTypes_Dragon,
        kPokemonTypes_Ghost,
        kPokemonTypes_Fairy,
        kPokemonTypes_Fighting
    };

    public enum PokemonNatures
    {
        kPokemonNatures_Hardy,
        kPokemonNatures_Lonely,
        kPokemonNatures_Adamant,
        kPokemonNatures_Naughty,
        kPokemonNatures_Brave,
        kPokemonNatures_Bold,
        kPokemonNatures_Docile,
        kPokemonNatures_Impish,
        kPokemonNatures_Lax,
        kPokemonNatures_Relaxed,
        kPokemonNatures_Modest,
        kPokemonNatures_Mild,
        kPokemonNatures_Bashful,
        kPokemonNatures_Rash,
        kPokemonNatures_Quiet,
        kPokemonNatures_Calm,
        kPokemonNatures_Gentle,
        kPokemonNatures_Careful,
        kPokemonNatures_Quirky,
        kPokemonNatures_Sassy,
        kPokemonNatures_Timid,
        kPokemonNatures_Hasty,
        kPokemonNatures_Jolly,
        kPokemonNatures_Naive,
        kPokemonNatures_Serious
    };

    public enum PokemonAttackCategory
    {
        kPokemonAttackCategory_Physical,
        kPokemonAttackCategory_Special,
        kPokemonAttackCategory_Status
    }

    public enum PokemonGenders
    {
        kPokemonGenders_None,
        kPokemonGenders_Male,
        kPokemonGenders_Female
    }

    public enum FacingDirection
    {
        kFacingDirection_None = -1,
        kFacingDirection_Left,
        kFacingDirection_Right,
        kFacingDirection_Up,
        kFacingDirection_Down
    };
}
