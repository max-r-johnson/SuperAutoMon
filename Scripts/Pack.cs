using Godot;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public partial class Pack
{
	public string name;
	public PetList petList;
	public FoodList foodList;
	public PetAbility PetAbility;
	public FoodAbility FoodAbility;
	public Pet pet;

	public Pack(string Name, PetList petlist, FoodList foodlist)
	{
		name = Name;
		this.petList = petlist;
		this.foodList = foodlist;
	}

	public Pack()
	{
	}

	public static Pack generateKanto()
	{
		PetList kantoPetList = new PetList();
		kantoPetList.tierOnePets = new List<Type>{typeof(BulbasaurAbility), typeof(CharmanderAbility), typeof(SquirtleAbility), typeof(CaterpieAbility), typeof(WeedleAbility), typeof(PidgeyAbility), typeof(RattataAbility), typeof(SpearowAbility), typeof(EkansAbility), typeof(NidoranmAbility), typeof(NidoranfAbility)};
		kantoPetList.tierTwoPets = new List<Type>{typeof(PikachuAbility), typeof(OddishAbility), typeof(ClefairyAbility), typeof(JigglypuffAbility), typeof(AbraAbility), typeof(ZubatAbility), typeof(GastlyAbility), typeof(BellsproutAbility), typeof(SandshrewAbility), typeof(MankeyAbility), typeof(PoliwagAbility), typeof(MeowthAbility)}.Concat(kantoPetList.tierOnePets).ToList();
		//kantoPetList.tierTwoPets = (List<Type>)kantoPetList.tierTwoPets.Concat(kantoPetList.tierOnePets);
		//kantoPetList.tierThreePets = new List<Pet> {(new Pet(ability))};
		//kantoPetList.tierFourPets.Add(new Pet(ability));
		//kantoPetList.tierFivePets.Add(new Pet(ability));
		//kantoPetList.tierSixPets.Add(new Pet(ability));
		//kantoPetList.tierSevenPets.Add(new Pet(ability));
		kantoPetList.generatePets();
		
		//for every pet in tier one of kanto, print the type of every pet's ability (weedleability, rattataability, etc.)

		FoodList kantoFoodList = new FoodList();
		kantoFoodList.tierOneFood = new List<Type>{typeof(TinyAppleAbility),typeof(OranBerryAbility),typeof(RareCandyAbility)};
		kantoFoodList.tierTwoFood = new List<Type>{typeof(DoomSeedAbility),typeof(GummiAbility),typeof(EnergyPowderAbility)}.Concat(kantoFoodList.tierOneFood).ToList();
		//kantoFoodList.tierThreeFood.Add(new Food(ability));
		//kantoFoodList.tierFourFood.Add(new Food(ability));
		//kantoFoodList.tierFiveFood.Add(new Food(ability));
		//kantoFoodList.tierSixFood.Add(new Food(ability));
		//kantoFoodList.tierSevenFood.Add(new Food(ability));
		kantoFoodList.generateFoods();
		Pack Kanto = new Pack("Kanto",kantoPetList,kantoFoodList);

		// foreach(Pet i in Kanto.petList.tierOnePets)
		// {
		// 	//GD.Print(i.ToString());
		// }
		// //GD.Print(Kanto.petList.ToString());
		return Kanto;
	}

}