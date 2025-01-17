using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

public partial class PetList
{
	Game game {get {return MainNode.game;}}
	public int currentTier {get {return game.tier;}}

	public Dictionary<int, List<Type>> availablePets {get;set;}
	public List<List<Type>> tiers {get; set;}
	Pack currentPack;

	public PetList()
	{
		tiers = new List<List<Type>>();
	}

	public void generatePets()
	{
		Dictionary<int, List<Type>> Pets = new Dictionary<int, List<Type>>();
		Pets[1] = tiers[1];
		foreach(int i in GD.Range(2,Game.numTiers + 1))
		{
			Pets[i] = tiers[i].Concat(Pets[i-1]).ToList();
		}
		this.availablePets = Pets;
	}

    public override string ToString()
    {
		string newString = "";
        foreach(int i in GD.Range(1,8))
		{
			if (availablePets[i] != null)
			{
				string petsInTier = "";
				foreach(int j in GD.Range(0,availablePets[i].Count()-1))
				{
					petsInTier += ((PetAbility)Activator.CreateInstance(availablePets[i][j])).name + ", ";
				}
				petsInTier += ((PetAbility)Activator.CreateInstance(availablePets[i][availablePets[i].Count()-1])).name;
			newString = newString + "Tier " + i + ": " + petsInTier + "\n";
			}
			else
			{
				newString = newString + "Tier " + i + ": None\n";
			}
		}
		return newString;
    }
}
