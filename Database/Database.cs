using Godot;
using SQLite;
using System;
using System.Collections.Generic;
using PokeApiNet;
using System.Net.Http;

public class Database : Node
{
    private SQLiteConnection _db;
    private PokeApiClient _pokeClient;

    public override void _Ready()
    {
        GD.Print("Database ready.");
        _db = new SQLiteConnection("Database/pokemon.sqlite");

        _db.CreateTables<Ability, Item, PokemonDB, PokemonAbility>();

        // _db.InsertOrReplace(new Ability() { Id = 1, Name = "Overgrow", Description = "Grass type Pokemon get a boost in power." });
        // _db.InsertOrReplace(new Ability() { Id = 2, Name = "Chlorophyll", Description = "Grass type Pokemon get a boost in speed." });
        // _db.InsertOrReplace(new PokemonDB() { Id = 1, Name = "Bulbasaur", HP = 45, Attack = 49, Defense = 49, SpAttack = 65, SpDefense = 65, Speed = 45, FirstType = Enums.PokemonTypes.kPokemonTypes_Grass, SecondType = Enums.PokemonTypes.kPokemonTypes_Poison });
        // _db.Insert(new PokemonAbility() { PokemonId = 1, AbilityId = 1 });
        // _db.Insert(new PokemonAbility() { PokemonId = 1, AbilityId = 2 });




        // GD.Print("Pokemon: ", bulbasaur.Name);
        getPokemon();
    }

    private async void getPokemon() {
        GD.Print("Initialize client.");
        _pokeClient = new PokeApiClient();

        GD.Print("Get pokemon.");
        try {
            PokeApiNet.Pokemon bulbasaur = await _pokeClient.GetResourceAsync<PokeApiNet.Pokemon>("bulbasaur");
        } catch (Exception e) {
            GD.Print("Error: ", e);
        }
    }


    private void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {

        if (responseCode != 200)
        {
            GD.Print("Error: ", responseCode);
            return;
        }

        GD.Print("Request completed.");

        var response = JSON.Parse(body.GetStringFromUTF8());

        GD.Print("Pokemon: ");
        foreach (var key in response.GetPropertyList())
        {
            GD.Print(key);
        }

        // PokemonDB pokemon = _db.Table<PokemonDB>().First(p => p.Id == (int)response.Get("id"));

        // List<PokemonAbility> pokemonAbilities = _db.Table<PokemonAbility>().Where(pokemonAbility => pokemonAbility.PokemonId == pokemon.Id).ToList();
        // pokemon.Ability = pokemonAbilities.ConvertAll(x => _db.Table<Ability>().Where(y => y.Id == x.AbilityId).First());

        // GD.Print("Pokemon: ", pokemon.Name);
        // GD.Print("Abilities:");
        // foreach (Ability a in pokemon.Ability)
        // {
        //     GD.Print(a.Name);
        // }

    }





    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

