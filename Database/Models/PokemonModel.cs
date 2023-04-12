using SQLite;
using System.Collections.Generic;

[Table("Pokemon")]
public class PokemonDB
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Name { get; set; }
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SpAttack { get; set; }
    public int SpDefense { get; set; }
    public int Speed { get; set; }
    public Enums.PokemonTypes FirstType { get; set; }
    public Enums.PokemonTypes SecondType { get; set; }

    [Ignore]
    public List<Ability> Ability { get; set; } = new List<Ability>();
}

[Table("Abilities")]
public class Ability
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

[Table("Items")]
public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

[Table("PokemonAbilities")]
public class PokemonAbility
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int PokemonId { get; set; }
    [Indexed]
    public int AbilityId { get; set; }
}

[Table("PokemonItems")]
public class PokemonItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int PokemonId { get; set; }
    [Indexed]
    public int ItemId { get; set; }
    public int Rate { get; set; }
}