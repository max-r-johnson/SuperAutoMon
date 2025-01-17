using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

public partial class FoodList
{
	Game game {get {return MainNode.game;}}
	public int currentTier {get {return game.tier;}}
	public Dictionary<int, List<Type>> availableFood;
	public List<List<Type>> tiers {get; set;}
	Pack currentPack;

	public FoodList()
	{
		tiers = new List<List<Type>>();
	}

	public void generateFoods()
	{
		Dictionary<int, List<Type>> Food = new Dictionary<int, List<Type>>();
		Food[1] = tiers[1];
		foreach(int i in GD.Range(2,Game.numTiers + 1))
		{
			Food[i] = tiers[i].Concat(Food[i-1]).ToList();
		}
		this.availableFood = Food;
	}

	public override string ToString()
    {
		string newString = "";
        foreach(int i in GD.Range(1,8))
		{
			if (availableFood[i] != null)
			{
				string FoodInTier = "";
				foreach(int j in GD.Range(0,availableFood[i].Count()-1))
				{
					FoodInTier += ((FoodAbility)Activator.CreateInstance(availableFood[i][j])).name + ", ";
				}
				FoodInTier += ((FoodAbility)Activator.CreateInstance(availableFood[i][availableFood[i].Count()-1])).name;
			newString = newString + "Tier " + i + ": " + FoodInTier + "\n";
			}
			else
			{
				newString = newString + "Tier " + i + ": None\n";
			}
		}
		return newString;
    }
}
