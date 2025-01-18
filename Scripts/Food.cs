using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Food
{
	readonly Pack foodPack;
	public FoodAbility foodAbility;
	Game game {get {return MainNode.game;}}
	public string name;
	Random random = new Random();

	public Food(FoodAbility ability)
	{
		this.foodAbility = ability;
		this.health = ability.health;
		this.attack = ability.attack;
		this.name = ability.name;
		this.foodAbility.baseFood = this;
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
		GD.Print("----");
		foreach (Pet target in targets)
		{
			GD.Print(target);
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
