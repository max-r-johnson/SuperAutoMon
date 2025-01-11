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
	public List<Type> tierOnePets {get; set;}
	public List<Type> tierTwoPets {get; set;}
	public List<Type> tierThreePets {get; set;}
	public List<Type> tierFourPets {get; set;}
	public List<Type> tierFivePets {get; set;}
	public List<Type> tierSixPets {get; set;}
	public List<Type> tierSevenPets {get; set;}
	Pack currentPack;

	public PetList()
	{
	}

	public void generatePets()
	{
		Dictionary<int, List<Type>> Pets = new Dictionary<int, List<Type>>();
		Pets[1] = tierOnePets;
		Pets[2] = tierTwoPets;
		Pets[3] = tierThreePets;
		Pets[4] = tierFourPets;
		Pets[5] = tierFivePets;
		Pets[6] = tierSixPets;
		Pets[7] = tierSevenPets;
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
