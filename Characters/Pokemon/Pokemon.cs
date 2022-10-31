using Godot;
using System;

public class Pokemon : Node
{
    private struct PokemonStats
    {
        int HP;
        int Attack;
        int Defense;
        int SpAttack;
        int SpDefense;
        int Speed;
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
        string Name;
        int Level;

        Enums.PokemonTypes FirstType;
        Enums.PokemonTypes SecondType;
        
        PokemonStats IVs;
        PokemonStats EVs;
        PokemonStats BaseStats;
        PokemonStats CurrentStats;

        Enums.PokemonNatures Nature;
        Godot.Collections.Array<PokemonAttack> Attacks;
        PokemonAbility Ability;
        PokemonItem HeldItem;

        int Weight;
        int Height;
        int Happiness;
    }

    private Sprite _partySprite = new Sprite();

    public Pokemon()
    {

    }
    public Pokemon(string pokemonNumber)
    {
        string filename = "icon" + pokemonNumber + ".png";
        Texture img = (Texture)GD.Load("res://Assets/Tiles/Menus/Pokemon/" + filename);
        _partySprite.Texture = img;
    }

    public Pokemon(int pokemonNumber)
    {
        string s = pokemonNumber.ToString();

        while(s.Length < 3)
        {
            s = "0" + s;
        }

        string filename = "icon" + s + ".png";
        Texture img = (Texture)GD.Load("res://Assets/Tiles/Menus/Pokemon/" + filename);
        _partySprite.Texture = img;
    }

    public Sprite GetPartySprite()
    {
        return _partySprite;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
}
