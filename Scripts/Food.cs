using Godot;
using System;

public partial class Food
{
	readonly Pack foodPack;
	public FoodAbility foodAbility;
	Game game;
	public string name;

	public Food(FoodAbility ability)
	{
		this.foodAbility = ability;
		this.health = ability.health;
		this.attack = ability.attack;
		this.name = ability.name;
		this.foodAbility.food = this;
	}

	public int index {get; set;}
	public bool stored {get; set;}
	public int cost {get {return foodAbility.cost;}}
	public int health {get; set;}
	public int attack {get; set;}
	public int tier {get {return foodAbility.tier;}}
	public override string ToString()
	{
		return "Food: " + attack + "/" + health + " " + name + " - " + foodAbility.AbilityMessage();
	}
}
