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
	public List<Type> tierOneFood {get; set;}
	public List<Type> tierTwoFood {get; set;}
	public List<Type> tierThreeFood {get; set;}
	public List<Type> tierFourFood {get; set;}
	public List<Type> tierFiveFood {get; set;}
	public List<Type> tierSixFood {get; set;}
	public List<Type> tierSevenFood {get; set;}
	Pack currentPack;

	public FoodList()
	{

	}

	public void generateFoods()
	{
		Dictionary<int, List<Type>> Food = new Dictionary<int, List<Type>>();
		Food[1] = tierOneFood;
		Food[2] = tierTwoFood;
		Food[3] = tierThreeFood;
		Food[4] = tierFourFood;
		Food[5] = tierFiveFood;
		Food[6] = tierSixFood;
		Food[7] = tierSevenFood;
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
