using Godot;
using System;

public partial class TinyAppleAbility : FoodAbility
{
	public TinyAppleAbility() : base()
    {
		name = "Tiny Apple";
		attack = 1;
    	health = 1;
		tier = 1;
		foreach(int i in GD.Range(Game.teamSize))
		{
			//TODO: needs to update if a jigglypuff is bought or sold
			//TODO: needs to access team somehow
			// if(team.GetPetAt(i)!=null)
			// {
			// 	if(team.GetPetAt(i).name == "Igglybuff")
			// 	{
				
			// 	}
			// 	else if(team.GetPetAt(i).name == "Jigglypuff")
			// 	{
			// 		cost = Math.Min(0,cost - 1);
			// 	}
			// 	else if(team.GetPetAt(i).name == "Wigglytuff")
			// 	{
			// 		cost = Math.Min(0,cost - 2);
			// 	}
			// }
		}
    }

	public override string AbilityMessage()
	{
		return "Gives +1/+1 to a pet who eats this.";
	}
}

public partial class OranBerryAbility : FoodAbility
{
	public OranBerryAbility()  : base()
	{
		name = "Oran Berry";
		tier = 1;
	}

    public override string AbilityMessage()
    {
        return "Gives a pet an Oran Berry, which gives 2 health when hurt.";
    }

    public override void OnEaten(Pet pet)
    {
        pet.GiveItem(new OranBerry());
    }
}

public partial class GummiAbility : FoodAbility
{
	public GummiAbility() : base()
	{
		//need to fix the -1s
		name = "Gummi";
		tier = 2;
		attack = 3;
		health = -1;
	}

    public override string AbilityMessage()
    {
        return "Gives +3/-1 to a pet who eats this.";
    }
}

public partial class EnergyPowderAbility : FoodAbility
{
	public EnergyPowderAbility() : base()
	{
		//need to fix the -1s
		name = "Energy Powder";
		tier = 2;
		attack = -1;
		health = 3;
	}

    public override string AbilityMessage()
    {
        return "Gives -1/+3 to a pet who eats this.";
    }
}

public partial class DoomSeedAbility : FoodAbility
{
	public DoomSeedAbility() : base()
    {
		name = "Doom Seed";
		tier = 2;
		cost = 1;
    }

	public override string AbilityMessage()
	{
		return "Makes a pet faint.";
	}

	public override async void OnEaten(Pet pet)
	{
		await pet.Faint(null);
	}
}

public partial class RareCandyAbility : FoodAbility
{
	public RareCandyAbility()  : base()
	{
		name = "Rare Candy";
		tier = 5;
	}

    public override string AbilityMessage()
    {
        return "Gives a pet 1 experience.";
    }

	public override bool canBeEaten(Pet pet)
    {
		return pet.experience<pet.maxExp;
    }

    public override async void OnEaten(Pet pet)
    {
        await pet.gainExperience(1);
    }
}
