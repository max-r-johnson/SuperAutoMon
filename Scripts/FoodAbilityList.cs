using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class TinyAppleAbility : FoodAbility
{
	public TinyAppleAbility() : base()
    {
		name = "Tiny Apple";
		attack = 1;
    	health = 1;
		tier = 1;
    }

	public override string AbilityMessage()
	{
		return "Gives +1/+1 to a pet who eats this.";
	}
}

public partial class AppleAbility : FoodAbility
{
	public AppleAbility() : base()
    {
		name = "Apple";
		attack = 2;
    	health = 2;
		tier = 1;
    }

	public override string AbilityMessage()
	{
		return "Gives +2/+2 to a pet who eats this.";
	}
}

public partial class PerfectAppleAbility : FoodAbility
{
	public PerfectAppleAbility() : base()
    {
		name = "Perfect Apple";
		attack = 3;
    	health = 3;
		tier = 1;
    }

	public override string AbilityMessage()
	{
		return "Gives +3/+3 to a pet who eats this.";
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

    public override async Task OnEaten(Pet pet)
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

	public override async Task OnEaten(Pet pet)
	{
		await pet.Faint(null);
	}
}

public partial class FreshWaterAbility : FoodAbility
{
	public FreshWaterAbility() : base()
    {
		name = "Fresh Water";
		attack = 1;
		health = 1;
		tier = 3;
		numTargets = 2;
    }

	public override string AbilityMessage()
	{
		return "Gives +1/+1 to 2 random pets on your team.";
	}

	public override async Task OnEaten(Pet pet)
	{
		await baseFood.feedTargets(baseFood.getTargets(pet));
	}
}

public partial class LeekAbility : FoodAbility
{
	public LeekAbility() : base()
    {
		name = "Leek";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Leek, which allows them to deal 5 extra damage.";
	}

	public override async Task OnEaten(Pet pet)
	{
		pet.GiveItem(new Leek());
	}
}

public partial class BerryJuiceAbility : FoodAbility
{
	public BerryJuiceAbility() : base()
    {
		name = "Berry Juice";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
		pet.GiveItem(new BerryJuice());
	}
}

public partial class EvolutionStoneAbility : FoodAbility
{
	public EvolutionStoneAbility() : base()
    {
		name = "Evolution Stone";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
	}
}

public partial class SitrusBerryAbility : FoodAbility
{
	public SitrusBerryAbility() : base()
    {
		name = "Sitrus Berry";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
		pet.GiveItem(new SitrusBerry());
	}
}

public partial class LumBerryAbility : FoodAbility
{
	public LumBerryAbility() : base()
    {
		name = "Lum Berry";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
		pet.GiveItem(new LumBerry());
	}
}

public partial class LemonadeAbility : FoodAbility
{
	public LemonadeAbility() : base()
    {
		name = "Lemonade";
		tier = 3;
		attack = 1;
		health = 1;
		numTargets = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives +1/+1 to 3 random pets on your team..";
	}

	public override async Task OnEaten(Pet pet)
	{
		await baseFood.feedTargets(baseFood.getTargets(pet));
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

    public override async Task OnEaten(Pet pet)
    {
        await pet.gainExperience(1);
    }
}


public partial class EjectButtonAbility : FoodAbility
{
	public EjectButtonAbility() : base()
    {
		name = "Eject Button";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
		pet.GiveItem(new EjectButton());
	}
}

public partial class ShellBellAbility : FoodAbility
{
	public ShellBellAbility() : base()
    {
		name = "Shell Bell";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
		pet.GiveItem(new ShellBell());
	}
}

public partial class LeftoversAbility : FoodAbility
{
	public LeftoversAbility() : base()
    {
		name = "Leftovers";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
		pet.GiveItem(new Leftovers());
	}
}

public partial class EnergyRootAbility : FoodAbility
{
	public EnergyRootAbility() : base()
    {
		name = "Energy Root";
		tier = 3;
    }

	public override string AbilityMessage()
	{
		return "Gives a pet a Berry Juice, which gives them 1 extra health when eating food.";
	}

	public override async Task OnEaten(Pet pet)
	{
	}
}

