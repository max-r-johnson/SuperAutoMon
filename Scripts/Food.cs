using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Food
{
	public FoodAbility foodAbility;
	Game game {get {return MainNode.game;}}
	public string name;
	Random random = new Random();
	public int index {get; set;}
	public bool stored {get; set;}
	public int cost {get; set;}
	public int health {get; set;}
	public int attack {get; set;}
	public int tier {get {return foodAbility.tier;}}

	public Food(FoodAbility ability)
	{
		foodAbility = ability;
		health = ability.health;
		attack = ability.attack;
		name = ability.name;
		foodAbility.baseFood = this;
		cost = foodAbility.cost;
	}

	public List<Pet> getTargets(Pet pet)
	{
		List<Pet> targets = new List<Pet>();
		List<Pet> tempList = new List<Pet>();
		foreach(Pet availablePet in pet.team.team)
		{
			if(availablePet != null)
			{
				tempList.Add(availablePet);
			}
		}
		foreach(int i in GD.Range(foodAbility.numTargets))
		{
			if (tempList.Count == 0) // Check if the list is empty to avoid errors
        		break;
			int randomNumber = random.Next(0,tempList.Count);
			targets.Add(tempList[randomNumber]);
			tempList.RemoveAt(randomNumber);
		}
		return targets;
	}

	public async Task feedTargets(List<Pet> targets)
	{
		List<Task> taskList = new List<Task>(); 
		foreach(Pet target in targets)
		{
			taskList.Add(target.GainAttack(foodAbility.attack));
			taskList.Add(target.GainHealth(foodAbility.health));
		}
		await game.WaitForTasks(taskList.ToArray());
	}

	public async Task changeCost(int amount)
	{
		cost += amount;
		game.createDescription(game.shop.foodSlots[index], this);
	}

	public override string ToString()
	{
		return "Food: " + attack + "/" + health + " " + name + " - " + foodAbility.AbilityMessage();
	}
}
