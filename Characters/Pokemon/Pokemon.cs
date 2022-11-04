using Godot;
using System;

public class Pokemon : Node
{
    //Pokemon Data structures.
    public struct PokemonStats
    {
        public int HP;
        public int Attack;
        public int Defense;
        public int SpAttack;
        public int SpDefense;
        public int Speed;
    }

    private struct PokemonAttack
    {
        string Name;
        string Description;
        int PP;
        int Power;
        int Accuracy;
        Enums.PokemonTypes Type;
        Enums.PokemonAttackCategory Category;
    }

    private struct PokemonAbility
    {
        string Name;
        string Description;
    }

    private struct PokemonItem
    {
        string Name;
        string Description;
    }

    public struct PokemonInfo
    {
        public string Name;
        public int Level;

        public Enums.PokemonTypes FirstType;
        public Enums.PokemonTypes SecondType;

        public PokemonStats IVs;
        public PokemonStats EVs;
        public PokemonStats BaseStats;
        public PokemonStats CurrentStats;

        public Enums.PokemonNatures Nature;
        Godot.Collections.Array<PokemonAttack> Attacks;
        PokemonAbility Ability;
        PokemonItem HeldItem;

        public float Weight;
        public float Height;
        public int Happiness;
        public Enums.PokemonGenders Gender;
    }

    //Member variables.
    private Sprite              _partySprite = new Sprite();
    private PokemonInfo         _info;

    //Utility functions.
    private void SearchAndAssignSprite(string path)
    {
        string filename = "icon" + path + ".png";
        Texture img = (Texture)GD.Load("res://Assets/Tiles/Menus/Pokemon/" + filename);
        _partySprite.Texture = img;
    }

    private void RandomizeInfo()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();

        _info.Level = rng.RandiRange(1, 10);

        //We will have to retrieve this information from JSON / DB.
        _info.FirstType = (Enums.PokemonTypes)rng.RandiRange(1, 17);
        _info.SecondType = (Enums.PokemonTypes)rng.RandiRange(0, 17);

        _info.IVs.HP = rng.RandiRange(0, 31);
        _info.IVs.Attack = rng.RandiRange(0, 31);
        _info.IVs.Defense = rng.RandiRange(0, 31);
        _info.IVs.SpAttack = rng.RandiRange(0, 31);
        _info.IVs.SpDefense = rng.RandiRange(0, 31);
        _info.IVs.Speed = rng.RandiRange(0, 31);

        _info.EVs.HP = 0;
        _info.EVs.Attack = 0;
        _info.EVs.Defense = 0;
        _info.EVs.SpAttack = 0;
        _info.EVs.SpDefense = 0;
        _info.EVs.Speed = 0;

        //We will have to retrieve this information from JSON / DB.
        _info.BaseStats.HP = rng.RandiRange(1, 30);
        _info.BaseStats.Attack = rng.RandiRange(1, 30);
        _info.BaseStats.Defense = rng.RandiRange(1, 30);
        _info.BaseStats.SpAttack = rng.RandiRange(1, 30);
        _info.BaseStats.SpDefense = rng.RandiRange(1, 30);
        _info.BaseStats.Speed = rng.RandiRange(1, 30);

        //We will have to calculate this information based on the base stats, level and IVs.
        _info.CurrentStats.HP = rng.RandiRange(1, 30);
        _info.CurrentStats.Attack = rng.RandiRange(1, 30);
        _info.CurrentStats.Defense = rng.RandiRange(1, 30);
        _info.CurrentStats.SpAttack = rng.RandiRange(1, 30);
        _info.CurrentStats.SpDefense = rng.RandiRange(1, 30);
        _info.CurrentStats.Speed = rng.RandiRange(1, 30);

        _info.Nature = (Enums.PokemonNatures)rng.RandiRange(0, 24);

        _info.Weight = rng.RandfRange(1.0f, 20.0f);
        _info.Height = rng.RandfRange(0.5f, 2.0f);

        _info.Happiness = 0;

        _info.Gender = (Enums.PokemonGenders)rng.RandiRange(1, 2);
    }

    //Custom constructors
    //NOTE: maybe is there a better way to create multiple parameter based constructor with almost same functionality.
    public Pokemon()
    {

    }

    public Pokemon(string pokemonNumber)
    {
        SearchAndAssignSprite(pokemonNumber);
        RandomizeInfo();
    }

    public Pokemon(int pokemonNumber)
    {
        string s = pokemonNumber.ToString();

        while(s.Length < 3)
        {
            s = "0" + s;
        }

        SearchAndAssignSprite(s);
        RandomizeInfo();
    }

    public Pokemon(string pokemonNumber, Pokemon.PokemonInfo info)
    {
        SearchAndAssignSprite(pokemonNumber);
        _info = info;
    }

    public Pokemon(int pokemonNumber, Pokemon.PokemonInfo info)
    {
        string s = pokemonNumber.ToString();

        while (s.Length < 3)
        {
            s = "0" + s;
        }

        SearchAndAssignSprite(s);
        _info = info;
    }

    //Getters.
    public PokemonInfo GetPokemonInfo()
    {
        return _info;
    }

    public Sprite GetPartySprite()
    {
        return _partySprite;
    }
}
