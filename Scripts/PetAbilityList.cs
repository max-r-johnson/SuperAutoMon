using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public partial class NoAbility : PetAbility
{
    public NoAbility() : base()
    {

    }
}

public partial class BulbasaurAbility : PetAbility
{
	public BulbasaurAbility() : base()
    {
		name = "Bulbasaur";
		attack = 2;
    	health = 3;
		tier = 1;
		evolution = new IvysaurAbility();
    }

	public override string AbilityMessage()
	{
		return "Faint => Give a random friend on your team +1 health permanently.";
	}

    public override async Task Faint(Pet source)
    {
		//I want this to trigger before it gets yeeted offscreen. However, I also want the other pet to do the recover animation at the same time as this.
		//if there is a faint ability or any friend faint ability, the other recovers. The faint abilities gets enqueued. The faint ability triggers. The pet gets yeeted.
		await game.WaitForTasks(basePet.getRandomFriend().GainPermanentHealth(1));
    }
}

public partial class IvysaurAbility : PetAbility
{
	public IvysaurAbility() : base()
    {
		name = "Ivysaur";
		attack = 2;
    	health = 3;
		tier = 1;
		evolution = new VenusaurAbility();
    }

	public override string AbilityMessage()
	{
		return "Faint => Give a random friend on your team +2 health permanently.";
	}

    public override async Task Faint(Pet source)
    {
		await game.WaitForTasks(basePet.getRandomFriend().GainPermanentHealth(2));
    }
}

public partial class VenusaurAbility : PetAbility
{
	public VenusaurAbility() : base()
    {
		name = "Venusaur";
		attack = 2;
    	health = 3;
		tier = 1;
    }

	public override string AbilityMessage()
	{
		return "Faint => Give a random friend on your team +3 health permanently.";
	}

    public override async Task Faint(Pet source)
    {
		await game.WaitForTasks(basePet.getRandomFriend().GainPermanentHealth(3));
    }
}

public partial class CharmanderAbility : PetAbility
{
	public CharmanderAbility() : base()
    {
		name = "Charmander";
		attack = 3;
    	health = 2;
		tier = 1;
		evolution = new CharmeleonAbility();
    }

	public override string AbilityMessage()
	{
		return "Evolved => Give adjacent friends +1 attack.";
	}

    public override async Task Evolve(Pet target)
    {
		List<Task> taskList = new List<Task>(); 
		foreach(Pet pet in basePet.getAdjacentPets())
		{
			GD.Print("Charmander gave " + pet.name + " 1 attack upon evolving!");
			taskList.Add(basePet.GiveBuff(1, pet, Pet.BuffType.GainAttack));
		}
		await game.WaitForTasks(taskList.ToArray());
    }
}

public partial class CharmeleonAbility : PetAbility
{
	public CharmeleonAbility() : base()
    {
		name = "Charmeleon";
		evolution = new CharizardAbility();
    }

	public override string AbilityMessage()
	{
		return "Evolved => Give adjacent friends +2 attack.";
	}

    public override async Task Evolve(Pet target)
    {
		List<Task> taskList = new List<Task>(); 
		foreach(Pet pet in basePet.getAdjacentPets())
		{
			GD.Print("Charmeleon gave " + pet.name + " 2 attack upon evolving!");
			taskList.Add(basePet.GiveBuff(2, pet, Pet.BuffType.GainAttack));
		}
		await game.WaitForTasks(taskList.ToArray());
    }
}

public partial class CharizardAbility : PetAbility
{
	public CharizardAbility() : base()
    {
		name = "Charizard";
    }

	public override string AbilityMessage()
	{
		return "No ability.";
	}
}

public partial class SquirtleAbility : PetAbility
{
	public SquirtleAbility() : base()
    {
		name = "Squirtle";
		attack = 2;
    	health = 3;
		tier = 1;
		evolution = new WartortleAbility();
    }

	public override string AbilityMessage()
	{
		return base.AbilityMessage();
	}
}

public partial class WartortleAbility : PetAbility
{
	public WartortleAbility() : base()
    {
		name = "Wartortle";
		tier = 1;
		evolution = new BlastoiseAbility();
    }

	public override string AbilityMessage()
	{
		return base.AbilityMessage();
	}
}

public partial class BlastoiseAbility : PetAbility
{
	public BlastoiseAbility() : base()
    {
		name = "Blastoise";
		tier = 1;
    }

	public override string AbilityMessage()
	{
		return base.AbilityMessage();
	}
}


public partial class CaterpieAbility : PetAbility
{
	public CaterpieAbility() : base()
    {
		name = "Caterpie";
		attack = 1;
    	health = 1;
		tier = 1;
		evolution = new MetapodAbility();
    }

	public override string AbilityMessage()
	{
		return "Sell => Put a free tiny apple in the shop.";
	}

    public override async Task Sell(Pet target)
    {
		Food addedFood = new Food(new TinyAppleAbility());
		addedFood.foodAbility.cost = 0;
		await shop.replaceShop(addedFood);
		GD.Print("Replaced shop with a free tiny apple!");
    }
}

public partial class MetapodAbility : PetAbility
{
	public MetapodAbility() : base()
    {
		name = "Metapod";
		tier = 1;
		evolution = new ButterfreeAbility();
    }

	public override string AbilityMessage()
	{
		return "Sell => Put a free apple in the shop.";
	}

    public override async Task Sell(Pet target)
    {
		Food addedFood = new Food(new AppleAbility());
		addedFood.foodAbility.cost = 0;
		await shop.replaceShop(addedFood);
		GD.Print("Replaced shop with a free apple!");
    }
}

public partial class ButterfreeAbility : PetAbility
{
	public ButterfreeAbility() : base()
    {
		name = "Butterfree";
		tier = 1;
    }

	public override string AbilityMessage()
	{
		return "Sell => Put a free perfect apple in the shop.";
	}

    public override async Task Sell(Pet target)
    {
		Food addedFood = new Food(new PerfectAppleAbility());
		addedFood.foodAbility.cost = 0;
		await shop.replaceShop(addedFood);
		GD.Print("Replaced shop with a free perfect apple!");
    }
}

public partial class WeedleAbility : PetAbility
{
	public WeedleAbility() : base()
    {
		name = "Weedle";
		attack = 3;
    	health = 1;
		tier = 1;
		evolution = new KakunaAbility();
    }

	public override string AbilityMessage()
	{
		return "Start of Battle => Deal 1 damage to the pet in front of this.";
	}

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		int currentIndex = basePet.index;
		List<Task> taskList = new List<Task>();
		foreach (int i in GD.Range(1))
		{
			if(currentIndex - i - 1 < 0)
			{
				if(enemyTeam.GetPetAt(Math.Abs(currentIndex - i))!= null)
				{
					GD.Print(enemyTeam.GetPetAt(Math.Abs(currentIndex - i)).name + " took 1 damage from the weedle on their opponent's team!");
					taskList.Add(basePet.Snipe(1, enemyTeam.GetPetAt(Math.Abs(currentIndex - i))));
				}
			}
			else
			{
				if(team.GetPetAt(currentIndex - i - 1)!= null)
				{
					GD.Print(team.GetPetAt(currentIndex - i - 1).name + " took 1 damage from the weedle on their team!");
					taskList.Add(basePet.Snipe(1, team.GetPetAt(currentIndex - i - 1)));
				}
			}
		}
		if(taskList.Count <= 0)
		{
			game.battleNode.BattleDequeue();
		}
		else
		{
			await game.WaitForTasks(taskList.ToArray());
		}
    }
}

public partial class KakunaAbility : PetAbility
{
	public KakunaAbility() : base()
    {
		name = "Kakuna";
		evolution = new BeedrillAbility();
    }

	public override string AbilityMessage()
	{
		return "Start of Battle => Deal 1 damage to the two pets in front of this.";
	}

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		int currentIndex = basePet.index;
		List<Task> taskList = new List<Task>();
		foreach (int i in GD.Range(2))
		{
			if(currentIndex - i - 1 < 0)
			{
				if(enemyTeam.GetPetAt(Math.Abs(currentIndex - i))!= null)
				{
					GD.Print(enemyTeam.GetPetAt(Math.Abs(currentIndex - i)).name + " took 1 damage from the kakuna on their opponent's team!");
					taskList.Add(basePet.Snipe(1, enemyTeam.GetPetAt(Math.Abs(currentIndex - i))));
				}
			}
			else
			{
				if(team.GetPetAt(currentIndex - i - 1)!= null)
				{
					GD.Print(team.GetPetAt(currentIndex - i - 1).name + " took 1 damage from the kakuna on their team!");
					taskList.Add(basePet.Snipe(1, team.GetPetAt(currentIndex - i - 1)));
				}
			}
		}
		if(taskList.Count <= 0)
		{
			game.battleNode.BattleDequeue();
		}
		else
		{
			await game.WaitForTasks(taskList.ToArray());
		}
    }
}

public partial class BeedrillAbility : PetAbility
{
	public BeedrillAbility() : base()
    {
		name = "Beedrill";
    }

	public override string AbilityMessage()
	{
		return "Start of Battle => Deal 1 damage to the three pets in front of this.";
	}

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		int currentIndex = basePet.index;
		List<Task> taskList = new List<Task>();
		foreach (int i in GD.Range(3))
		{
			if(currentIndex - i - 1 < 0)
			{
				if(enemyTeam.GetPetAt(Math.Abs(currentIndex - i))!= null)
				{
					GD.Print(enemyTeam.GetPetAt(Math.Abs(currentIndex - i)).name + " took 1 damage from the beedrill on their opponent's team!");
					taskList.Add(basePet.Snipe(1, enemyTeam.GetPetAt(Math.Abs(currentIndex - i))));
				}
			}
			else
			{
				if(team.GetPetAt(currentIndex - i - 1)!= null)
				{
					GD.Print(team.GetPetAt(currentIndex - i - 1).name + " took 1 damage from the beedrill on their team!");
					taskList.Add(basePet.Snipe(1, team.GetPetAt(currentIndex - i - 1)));
				}
			}
		}
		if(taskList.Count <= 0)
		{
			game.battleNode.BattleDequeue();
		}
		else
		{
			await game.WaitForTasks(taskList.ToArray());
		}
    }
}

public partial class PidgeyAbility : PetAbility
{
	public PidgeyAbility() : base()
    {
		name = "Pidgey";
		attack = 1;
    	health = 2;
		tier = 1;
		evolution = new PidgeottoAbility();
    }

    public override string AbilityMessage()
    {
        return "Start of Battle => Move the enemy in the last spot forward 1 space.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		//this needs to find the last pet on their team that isn't null and move it forward
		if(enemyTeam.lastIndex!=null)
		{
			GD.Print(name + " moved " + enemyTeam.lastIndex.name + " 1 space forward!");
        	await enemyTeam.Move(enemyTeam.lastIndex,-1);
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class PidgeottoAbility : PetAbility
{
	public PidgeottoAbility() : base()
    {
		name = "Pidgeotto";
		evolution = new PidgeotAbility();
    }

    public override string AbilityMessage()
    {
        return "Start of Battle => Move the enemy in the last spot forward 1 space 2 times.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		//this needs to find the last pet on their team that isn't null and move it forward
		if(enemyTeam.lastIndex!=null)
		{
			foreach(int i in GD.Range(2))
			{
				GD.Print(name + " moved " + enemyTeam.lastIndex.name + " 1 space forward!");
        		await enemyTeam.Move(enemyTeam.lastIndex,-1);
			}
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class PidgeotAbility : PetAbility
{
	public PidgeotAbility() : base()
    {
		name = "Pidgeot";
    }

    public override string AbilityMessage()
    {
        return "Start of Battle => Move the enemy in the last spot forward 1 space 3 times.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		//this needs to find the last pet on their team that isn't null and move it forward
		if(enemyTeam.lastIndex!=null)
		{
			foreach(int i in GD.Range(3))
			{
				GD.Print(name + " moved " + enemyTeam.lastIndex.name + " 1 space forward!");
        		await enemyTeam.Move(enemyTeam.lastIndex,-1);
			}
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class RattataAbility : PetAbility
{
	public RattataAbility() : base()
    {
		name = "Rattata";
		attack = 2;
    	health = 2;
		tier = 1;
		evolution = new RaticateAbility();
    }

	public override string AbilityMessage()
	{
		return "End of Turn => Gain 1 attack.";
	}

    public override async Task EndOfTurn(Pet target)
    {
		await game.WaitForTasks(basePet.GainAttack(1));
		GD.Print(basePet.name + " gained 1 attack!");
    }
}

public partial class RaticateAbility : PetAbility
{
	public RaticateAbility() : base()
    {
		name = "Raticate";
		tier = 1;
    }

	public override string AbilityMessage()
	{
		return "End of Turn => Gain 2 attack.";
	}

    public override async Task EndOfTurn(Pet target)
    {
		await game.WaitForTasks(basePet.GainAttack(2));
		GD.Print(basePet.name + " gained 2 attack!");
    }
}

public partial class SpearowAbility : PetAbility
{
	public SpearowAbility() : base()
    {
		name = "Spearow";
		attack = 2;
    	health = 1;
		tier = 1;
		evolution = new FearowAbility();
    }

    public override string AbilityMessage()
    {
        return "Enemy Moved => deal 1 damage to it.";
    }

    public override async Task EnemyMoved(Pet target)
    {
		if(target.currentHealth>0)
		{
			GD.Print(target.name + " took 1 damage from spearow after moving!");
			await game.WaitForTasks(basePet.Snipe(1,target));
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class FearowAbility : PetAbility
{
	public FearowAbility() : base()
    {
		name = "Fearow";
    }

    public override string AbilityMessage()
    {
        return "Enemy Moved => deal 2 damage to it.";
    }

    public override async Task EnemyMoved(Pet target)
    {
		if(target.currentHealth>0)
		{
			GD.Print(target.name + " took 2 damage from fearow after moving!");
			await game.WaitForTasks(basePet.Snipe(2,target));
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class EkansAbility : PetAbility
{
	public EkansAbility() : base()
    {
		name = "Ekans";
		attack = 3;
    	health = 2;
		tier = 1;
		evolution = new ArbokAbility();
    }

    public override string AbilityMessage()
    {
        return "Start of Battle => Swap attack and health.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		if(basePet.currentHealth>0)
		{
			int tempAttack = basePet.currentAttack;
			basePet.SetAttack(basePet.currentHealth);
			basePet.SetHealth(tempAttack);
			GD.Print(basePet.name + " swapped its attack and health!");
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class ArbokAbility : PetAbility
{
	public ArbokAbility() : base()
    {
		name = "Arbok";
    }

    public override string AbilityMessage()
    {
        return "Start of Battle => Swap own attack and health as well as the pet across from this.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		if(basePet.currentHealth>0)
		{
			int tempAttack = basePet.currentAttack;
			basePet.SetAttack(basePet.currentHealth);
			basePet.SetHealth(tempAttack);
			GD.Print(basePet.name + " swapped its attack and health!");
		}
		enemyPet = enemyTeam.GetPetAt(basePet.index);
		if(enemyPet!=null)
		{
			int tempAttack = enemyPet.currentAttack;
			enemyPet.SetAttack(enemyPet.currentHealth);
			enemyPet.SetHealth(tempAttack);
			GD.Print(enemyPet.name + " swapped its attack and health!");
		}
		if(basePet.currentHealth <= 0 && enemyPet == null)
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class NidoranmAbility : PetAbility
{
	public NidoranmAbility() : base()
    {
		name = "Nidoranm";
		attack = 1;
    	health = 2;
		tier = 1;
		evolution = new NidorinoAbility();
    }

	public override string AbilityMessage()
    {
        return "Can be combined with female Nidorans and their evolutions.";
    }
}

public partial class NidorinoAbility : PetAbility
{
	public NidorinoAbility() : base()
    {
		name = "Nidorino";
		evolution = new NidokingAbility();
		isStoneEvo = true;
    }

	public override string AbilityMessage()
    {
        return "Can be combined with female Nidorans and their evolutions.";
    }
}

public partial class NidokingAbility : PetAbility
{
	public NidokingAbility() : base()
    {
		name = "Nidoking";
    }

	public override string AbilityMessage()
    {
        return "Can be combined with female Nidorans and their evolutions.";
    }
}

public partial class NidoranfAbility : PetAbility
{
	public NidoranfAbility() : base()
    {
		name = "Nidoranf";
		attack = 2;
    	health = 1;
		tier = 1;
		evolution = new NidorinaAbility();
    }

	public override string AbilityMessage()
    {
        return "Can be combined with male Nidorans and their evolutions.";
    }
}

public partial class NidorinaAbility : PetAbility
{
	public NidorinaAbility() : base()
    {
		name = "Nidorina";
		evolution = new NidoqueenAbility();
		isStoneEvo = true;
    }

	public override string AbilityMessage()
    {
        return "Can be combined with female Nidorans and their evolutions.";
    }
}

public partial class NidoqueenAbility : PetAbility
{
	public NidoqueenAbility() : base()
    {
		name = "Nidoqueen";
    }

	public override string AbilityMessage()
    {
        return "Can be combined with female Nidorans and their evolutions.";
    }
}

public partial class PikachuAbility : PetAbility
{
	public PikachuAbility() : base()
	{
		name = "Pikachu";
		attack = 3;
		health = 2;
		tier = 2;
		isStoneEvo = true;
		evolution = new RaichuAbility();
	}

    public override string AbilityMessage()
    {
        return "Start of Battle => Deal 3 damage to the enemy with the highest health.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		Random random = new Random();
		if(enemyTeam.highestHealth.Count>0)
		{
			Pet randomPet = enemyTeam.highestHealth[random.Next(0,enemyTeam.highestHealth.Count)];
			if(randomPet.currentHealth>0)
			{
				GD.Print(randomPet.name + " took 3 damage from Pikachu!");
				await game.WaitForTasks(basePet.Snipe(3,randomPet));
			}
			else
			{
				game.battleNode.BattleDequeue();
			}
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class RaichuAbility : PetAbility
{
	public RaichuAbility() : base()
	{
		name = "Raichu";
	}

    public override string AbilityMessage()
    {
        return "Start of Battle => Deal 6 damage to the enemy with the highest health.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		Random random = new Random();
		if(enemyTeam.highestHealth.Count>0)
		{
			Pet randomPet = enemyTeam.highestHealth[random.Next(0,enemyTeam.highestHealth.Count)];
			if(randomPet.currentHealth>0)
			{
				GD.Print(randomPet.name + " took 6 damage from Pikachu!");
				await game.WaitForTasks(basePet.Snipe(6,randomPet));
			}
			else
			{
				game.battleNode.BattleDequeue();
			}
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class SandshrewAbility : PetAbility
{
	public SandshrewAbility() : base()
	{
		name = "Sandshrew";
		attack = 1;
		health = 3;
		tier = 2;
		evolution = new SandslashAbility();
	}

    public override string AbilityMessage()
    {
        return "Start of Battle => Give the friend ahead 3 health.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
        if(basePet.index!=0)
		{
			if(team.GetPetAt(basePet.index-1)!=null)
			{
				await game.WaitForTasks(basePet.GiveBuff(3,team.GetPetAt(basePet.index-1),Pet.BuffType.GainHealth));
			}
			else
			{
				game.battleNode.BattleDequeue();
			}
		}
    }
}

public partial class SandslashAbility : PetAbility
{
	public SandslashAbility() : base()
	{
		name = "Sandslash";
	}

    public override string AbilityMessage()
    {
        return "Start of Battle => Give the friend ahead 6 health.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
        if(basePet.index!=0)
		{
			if(team.GetPetAt(basePet.index-1)!=null)
			{
				await game.WaitForTasks(basePet.GiveBuff(6,team.GetPetAt(basePet.index-1),Pet.BuffType.GainHealth));
			}
			else
			{
				game.battleNode.BattleDequeue();
			}
		}
    }
}

public partial class ClefairyAbility : PetAbility
{
	public ClefairyAbility() : base()
	{
		name = "Clefairy";
		attack = 2;
		health = 4;
		tier = 2;
		evolution = new ClefableAbility();
		isStoneEvo = true;
	}

    public override string AbilityMessage()
    {
        return "Start of Battle => Reduce the attack of the opposite pet by 2.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		if(enemyTeam.GetPetAt(basePet.index)!=null)
		{
			await game.WaitForTasks(basePet.GiveDebuff(2,enemyTeam.GetPetAt(basePet.index),Pet.DebuffType.ReduceAttack));
		}
    }
}

public partial class ClefableAbility : PetAbility
{
	public ClefableAbility() : base()
	{
		name = "Clefable";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Start of Battle => Reduce the attack of the opposite pet by 4.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		if(enemyTeam.GetPetAt(basePet.index)!=null)
		{
			await game.WaitForTasks(basePet.GiveDebuff(4,enemyTeam.GetPetAt(basePet.index),Pet.DebuffType.ReduceAttack));
		}
    }
}

public partial class JigglypuffAbility : PetAbility
{
	public JigglypuffAbility() : base()
	{
		name = "Jigglypuff";
		attack = 1;
		health = 2;
		tier = 2;
		evolution = new WigglytuffAbility();
		isStoneEvo = true;
	}

	public override string AbilityMessage()
    {
        return "Start of Turn => Reduce the cost of apples by 1. Minimum 0 cost.";
    }

	public override async Task StartOfTurn(Pet target)
	{
		foreach(Food food in shop.shopFood)
		{
			if((food.name == "Tiny Apple" || food.name == "Apple" || food.name == "Perfect Apple") && food.cost > 0)
			{
				await food.changeCost(-1);
			}
		}
	}
}

public partial class WigglytuffAbility : PetAbility
{
	public WigglytuffAbility() : base()
	{
		name = "Wigglytuff";
		tier = 2;
	}

	public override string AbilityMessage()
    {
        return "Start of Turn => Reduce the cost of apples by 2. Minimum 0 cost.";
    }

	public override async Task StartOfTurn(Pet target)
	{
		foreach(Food food in shop.shopFood)
		{
			if((food.name == "Tiny Apple" || food.name == "Apple" || food.name == "Perfect Apple") && food.cost > 0)
			{
				await food.changeCost(-2);
			}
		}
	}
}

public partial class ZubatAbility : PetAbility
{
	public ZubatAbility() : base()
	{
		name = "Zubat";
		attack = 4;
		health = 3;
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Knockout => Move front enemy back 1 space.";
    }

    public override async Task Knockout(Pet target)
    {
        if(enemyTeam.GetPetAt(1)!=null)
		{
			await enemyTeam.Move(enemyTeam.GetPetAt(1),1);
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class GolbatAbility : PetAbility
{
	public GolbatAbility() : base()
	{
		name = "Golbat";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Knockout => Move front enemy back 2 spaces.";
    }

    public override async Task Knockout(Pet target)
    {
        if(enemyTeam.GetPetAt(1)!=null)
		{
			await enemyTeam.Move(enemyTeam.GetPetAt(1),2);
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class OddishAbility : PetAbility
{
	public OddishAbility() : base()
	{
		name = "Oddish";
		attack = 1;
		health = 1;
		tier = 2;
		evolution = new GloomAbility();
	}

    public override string AbilityMessage()
    {
        return "Food Purchased => Temporarily gain +1/1 this turn.";
    }

    public override async Task FoodPurchased(Pet target)
    {
        await game.WaitForTasks(basePet.GainTemporaryAttack(1), basePet.GainTemporaryHealth(1));
    }
}

public partial class GloomAbility : PetAbility
{
	public GloomAbility() : base()
	{
		name = "Gloom";
		tier = 2;
		evolution = new VileplumeAbility();
		isStoneEvo = true;
	}

    public override string AbilityMessage()
    {
        return "Food Purchased => Temporarily gain +2/2 this turn.";
    }

    public override async Task FoodPurchased(Pet target)
    {
        await game.WaitForTasks(basePet.GainTemporaryAttack(2), basePet.GainTemporaryHealth(2));
    }
}

public partial class VileplumeAbility : PetAbility
{
	public VileplumeAbility() : base()
	{
		name = "Vileplume";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Food Purchased => Temporarily gain +3/3 this turn.";
    }

    public override async Task FoodPurchased(Pet target)
    {
        await game.WaitForTasks(basePet.GainTemporaryAttack(3), basePet.GainTemporaryHealth(3));
    }
}


public partial class MeowthAbility : PetAbility
{
	public MeowthAbility() : base()
	{
		name = "Meowth";
		attack = 3;
		health = 2;
		tier = 2;
		evolution = new PersianAbility();
	}

    public override string AbilityMessage()
    {
        return "Eats Food => Gain 2 gold. (Once per turn)";
   
    }

    public override async Task AteFood(Pet target)
    {
        if(basePet.eatenFood<=1)
		{
			shop.incMoney(2);
		}
    }
}

public partial class PersianAbility : PetAbility
{
	public PersianAbility() : base()
	{
		name = "Persian";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Eats Food => Gain 4 gold. (Once per turn)";
   
    }

    public override async Task AteFood(Pet target)
    {
        if(basePet.eatenFood<=1)
		{
			shop.incMoney(4);
		}
    }
}

public partial class MankeyAbility : PetAbility
{
	public MankeyAbility() : base()
	{
		name = "Mankey";
		attack = 1;
		health = 4;
		tier = 2;
		evolution = new PrimeapeAbility();
	}

    public override string AbilityMessage()
    {
        return "Hurt => Gain 4 attack.";
    }

    public override async Task Hurt(Pet source)
    {
		if(basePet.currentHealth>0)
		{
        	await basePet.GainAttack(4);
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class PrimeapeAbility : PetAbility
{
	public PrimeapeAbility() : base()
	{
		name = "Primeape";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Hurt => Gain 8 attack.";
    }

    public override async Task Hurt(Pet source)
    {
		if(basePet.currentHealth>0)
		{
        	await basePet.GainAttack(8);
		}
		else
		{
			game.battleNode.BattleDequeue();
		}
    }
}

public partial class PoliwagAbility : PetAbility
{
	public PoliwagAbility() : base()
	{
		name = "Poliwag";
		attack = 2;
		health = 2;
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Buy => Give a random friend 1 attack and 1 health.";
    }

    public override async Task Buy(Pet target)
    {
        Pet randomPet = basePet.getRandomFriend();
		if(randomPet != null)
		{
			Func<Task> task1 = async () => await basePet.GiveBuff(1, randomPet, Pet.BuffType.GainAttack);
			Func<Task> task2 = async () => await basePet.GiveBuff(1, randomPet, Pet.BuffType.GainHealth);
			await game.WaitForFuncTasks(task1, task2);
		}
    }
}

public partial class AbraAbility : PetAbility
{
	public AbraAbility() : base()
	{
		name = "Abra";
		attack = 4;
		health = 2;
		tier = 2;
		evolution = new KadabraAbility();
	}

    public override string AbilityMessage()
    {
        return "In Front => Move to the back and gain 2 attack. (Doesn't work with others in the Abra evolution family)";
    }

    public override async Task InFront(Pet target)
    {
		game.mouseDisabled = true;
		await team.Move(basePet,5);
		await basePet.GainAttack(2);
		game.mouseDisabled = false;
		GD.Print(name + " at index + " + basePet.index + " moved to the back and gained 2 attack!");
    }
}

public partial class KadabraAbility : PetAbility
{
	public KadabraAbility() : base()
	{
		name = "Kadabra";
		tier = 2;
		evolution = new AlakazamAbility();
	}

    public override string AbilityMessage()
    {
        return "In Front => Move to the back and gain 4 attack. (Doesn't work with others in the Kadabra evolution family)";
    }

    public override async Task InFront(Pet target)
    {
		game.mouseDisabled = true;
		await team.Move(basePet,5);
		await basePet.GainAttack(4);
		game.mouseDisabled = false;
		GD.Print(name + " at index + " + basePet.index + " moved to the back and gained 4 attack!");
    }
}

public partial class AlakazamAbility : PetAbility
{
	public AlakazamAbility() : base()
	{
		name = "Alakazam";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "In Front => Move to the back and gain 6 attack. (Doesn't work with others in the Alakazam evolution family)";
    }

    public override async Task InFront(Pet target)
    {
		game.mouseDisabled = true;
		await team.Move(basePet,5);
		await basePet.GainAttack(6);
		game.mouseDisabled = false;
		GD.Print(name + " at index + " + basePet.index + " moved to the back and gained 6 attack!");
    }
}

public partial class BellsproutAbility : PetAbility
{
	public BellsproutAbility() : base()
	{
		name = "Bellsprout";
		attack = 1;
		health = 2;
		tier = 2;
		evolution = new WeepinbellAbility();
	}

    public override string AbilityMessage()
    {
        return "Friend Used Perk => Give it 1 attack and 2 health.";
    }

    public override async Task FriendUsedPerk(Pet target)
    {
		Func<Task> task1 = async () => await basePet.GiveBuff(1, target, Pet.BuffType.GainAttack);
		Func<Task> task2 = async () => await basePet.GiveBuff(2, target, Pet.BuffType.GainHealth);
		await game.WaitForFuncTasks(task1, task2);
    }
}

public partial class WeepinbellAbility : PetAbility
{
	public WeepinbellAbility() : base()
	{
		name = "Weepinbell";
		tier = 2;
		evolution = new VictreebelAbility();
		isStoneEvo = true;
	}

    public override string AbilityMessage()
    {
        return "Friend Used Perk => Give it 2 attack and 4 health.";
    }

    public override async Task FriendUsedPerk(Pet target)
    {
		Func<Task> task1 = async () => await basePet.GiveBuff(2, target, Pet.BuffType.GainAttack);
		Func<Task> task2 = async () => await basePet.GiveBuff(4, target, Pet.BuffType.GainHealth);
		await game.WaitForFuncTasks(task1, task2);
    }
}

public partial class VictreebelAbility : PetAbility
{
	public VictreebelAbility() : base()
	{
		name = "Victreebel";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Friend Used Perk => Give it 3 attack and 6 health.";
    }

    public override async Task FriendUsedPerk(Pet target)
    {
		Func<Task> task1 = async () => await basePet.GiveBuff(3, target, Pet.BuffType.GainAttack);
		Func<Task> task2 = async () => await basePet.GiveBuff(6, target, Pet.BuffType.GainHealth);
		await game.WaitForFuncTasks(task1, task2);
    }
}

public partial class GastlyAbility : PetAbility
{
	public GastlyAbility() : base()
	{
		name = "Gastly";
		attack = 4;
		health = 1;
		tier = 2;
		evolution = new HaunterAbility();
	}

    public override string AbilityMessage()
    {
        return "Faint => Deal 5 damage to the pet that knocked this out.";
    }

    public override async Task Faint(Pet source)
    {
        if(source.currentHealth>0)
		{
			await game.WaitForTasks(basePet.Snipe(5, source));
    	}
		else
		{
			game.battleNode.BattleDequeue();
		}
	}
}

public partial class HaunterAbility : PetAbility
{
	public HaunterAbility() : base()
	{
		name = "Haunter";
		tier = 2;
		evolution = new GengarAbility();
	}

    public override string AbilityMessage()
    {
        return "Faint => Deal 10 damage to the pet that knocked this out.";
    }

    public override async Task Faint(Pet source)
    {
        if(source.currentHealth>0)
		{
			await game.WaitForTasks(basePet.Snipe(10, source));
    	}
		else
		{
			game.battleNode.BattleDequeue();
		}
	}
}

public partial class GengarAbility : PetAbility
{
	public GengarAbility() : base()
	{
		name = "Gengar";
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Faint => Deal 15 damage to the pet that knocked this out.";
    }

    public override async Task Faint(Pet source)
    {
        if(source.currentHealth>0)
		{
			await game.WaitForTasks(basePet.Snipe(15, source));
    	}
		else
		{
			game.battleNode.BattleDequeue();
		}
	}
}

public partial class VulpixAbility : PetAbility
{
	public VulpixAbility() : base()
	{
		name = "Vulpix";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class ParasAbility : PetAbility
{
	public ParasAbility() : base()
	{
		name = "Paras";
		attack = 1;
		health = 1;
		tier = 3;
		evolution = new ParasectAbility();
	}

	public override string AbilityMessage()
    {
        return "End Turn => Give a lum berry to the pet behind this.";
    }

    public override async Task EndOfTurn(Pet target)
    {
        if(basePet.getNearestFriendBehind()!=null)
		{
			basePet.getNearestFriendBehind().GiveItem(new LumBerry());
		}
    }
}

public partial class ParasectAbility : PetAbility
{
	public ParasectAbility() : base()
	{
		name = "Parasect";
		tier = 3;
	}

	public override string AbilityMessage()
    {
        return "End Turn => Give a lum berry to the two pets behind this.";
    }

    public override async Task EndOfTurn(Pet target)
    {
        if(basePet.getNearestFriendBehind()!=null)
		{
			basePet.getNearestFriendBehind().GiveItem(new LumBerry());
			if(basePet.getNearestFriendBehind().getNearestFriendBehind()!=null)
			{
				basePet.getNearestFriendBehind().getNearestFriendBehind().GiveItem(new LumBerry());
			}
		}
    }
}

public partial class VenonatAbility : PetAbility
{
	public VenonatAbility() : base()
	{
		name = "Venonat";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class DiglettAbility : PetAbility
{
	public DiglettAbility() : base()
	{
		name = "Diglett";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class PsyduckAbility : PetAbility
{
	public PsyduckAbility() : base()
	{
		name = "Psyduck";
		attack = 1;
		health = 1;
		tier = 3;
	}

	public override string AbilityMessage()
    {
        return "Sell => Give this pet's perk to the pet behind this.";
    }

    public override async Task Sell(Pet target)
    {
        if(basePet.getNearestFriendBehind()!=null)
		{
			basePet.getNearestFriendBehind().GiveItem(basePet.item);
		}
    }
}

public partial class GolduckAbility : PetAbility
{
	public GolduckAbility() : base()
	{
		name = "Golduck";
		attack = 1;
		health = 1;
		tier = 3;
	}

	public override string AbilityMessage()
    {
        return "Sell => Give this pet's perk to the two pets behind this.";
    }

    public override async Task Sell(Pet target)
    {
        if(basePet.getNearestFriendBehind()!=null)
		{
			basePet.getNearestFriendBehind().GiveItem(basePet.item);
			if(basePet.getNearestFriendBehind().getNearestFriendBehind()!=null)
			{
				basePet.getNearestFriendBehind().getNearestFriendBehind().GiveItem(basePet.item);
			}
		}
    }
}

public partial class GrowlitheAbility : PetAbility
{
	public GrowlitheAbility() : base()
	{
		name = "Growlithe";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class MachopAbility : PetAbility
{
	public MachopAbility() : base()
	{
		name = "Machop";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class GeodudeAbility : PetAbility
{
	public GeodudeAbility() : base()
	{
		name = "Geodude";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class OnixAbility : PetAbility
{
	public OnixAbility() : base()
	{
		name = "Onix";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class DrowzeeAbility : PetAbility
{
	public DrowzeeAbility() : base()
	{
		name = "Drowzee";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class GoldeenAbility : PetAbility
{
	public GoldeenAbility() : base()
	{
		name = "Goldeen";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class StaryuAbility : PetAbility
{
	public StaryuAbility() : base()
	{
		name = "Staryu";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class MagikarpAbility : PetAbility
{
	public MagikarpAbility() : base()
	{
		name = "Magikarp";
		attack = 1;
		health = 1;
		tier = 3;
	}
}

public partial class TentacoolAbility : PetAbility
{
	public TentacoolAbility() : base()
	{
		name = "Tentacool";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class PonytaAbility : PetAbility
{
	public PonytaAbility() : base()
	{
		name = "Ponyta";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class SlowpokeAbility : PetAbility
{
	public SlowpokeAbility() : base()
	{
		name = "Slowpoke";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class MagnemiteAbility : PetAbility
{
	public MagnemiteAbility() : base()
	{
		name = "Magnemite";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class DoduoAbility : PetAbility
{
	public DoduoAbility() : base()
	{
		name = "Doduo";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class SeelAbility : PetAbility
{
	public SeelAbility() : base()
	{
		name = "Seel";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class GrimerAbility : PetAbility
{
	public GrimerAbility() : base()
	{
		name = "Grimer";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class KoffingAbility : PetAbility
{
	public KoffingAbility() : base()
	{
		name = "Koffing";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class HorseaAbility : PetAbility
{
	public HorseaAbility() : base()
	{
		name = "Horsea";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class KrabbyAbility : PetAbility
{
	public KrabbyAbility() : base()
	{
		name = "Krabby";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class CuboneAbility : PetAbility
{
	public CuboneAbility() : base()
	{
		name = "Cubone";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class ExeggcuteAbility : PetAbility
{
	public ExeggcuteAbility() : base()
	{
		name = "Exeggcute";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class VoltorbAbility : PetAbility
{
	public VoltorbAbility() : base()
	{
		name = "Voltorb";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class ShellderAbility : PetAbility
{
	public ShellderAbility() : base()
	{
		name = "Shellder";
		attack = 1;
		health = 1;
		tier = 4;
	}
}

public partial class TaurosAbility : PetAbility
{
	public TaurosAbility() : base()
	{
		name = "Tauros";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class PinsirAbility : PetAbility
{
	public PinsirAbility() : base()
	{
		name = "Pinsir";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class MagmarAbility : PetAbility
{
	public MagmarAbility() : base()
	{
		name = "Magmar";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class ElectabuzzAbility : PetAbility
{
	public ElectabuzzAbility() : base()
	{
		name = "Electabuzz";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class JynxAbility : PetAbility
{
	public JynxAbility() : base()
	{
		name = "Jynx";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class ScytherAbility : PetAbility
{
	public ScytherAbility() : base()
	{
		name = "Scyther";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class MrMimeAbility : PetAbility
{
	public MrMimeAbility() : base()
	{
		name = "Mr. Mime";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class FarfetchdAbility : PetAbility
{
	public FarfetchdAbility() : base()
	{
		name = "Farfetch'd";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class LickitungAbility : PetAbility
{
	public LickitungAbility() : base()
	{
		name = "Lickitung";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class RhyhornAbility : PetAbility
{
	public RhyhornAbility() : base()
	{
		name = "Rhyhorn";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class TangelaAbility : PetAbility
{
	public TangelaAbility() : base()
	{
		name = "Tangela";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class EeveeAbility : PetAbility
{
	public EeveeAbility() : base()
	{
		name = "Eevee";
		attack = 1;
		health = 1;
		tier = 5;
	}
}

public partial class DratiniAbility : PetAbility
{
	public DratiniAbility() : base()
	{
		name = "Dratini";
		attack = 1;
		health = 1;
		tier = 6;
	}
}

public partial class SnorlaxAbility : PetAbility
{
	public SnorlaxAbility() : base()
	{
		name = "Snorlax";
		attack = 1;
		health = 1;
		tier = 6;
	}
}

public partial class AerodactylAbility : PetAbility
{
	public AerodactylAbility() : base()
	{
		name = "Aerodactyl";
		attack = 1;
		health = 1;
		tier = 6;
	}
}

public partial class OmanyteAbility : PetAbility
{
	public OmanyteAbility() : base()
	{
		name = "Omanyte";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class KabutoAbility : PetAbility
{
	public KabutoAbility() : base()
	{
		name = "Kabuto";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class LaprasAbility : PetAbility
{
	public LaprasAbility() : base()
	{
		name = "Lapras";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class KangaskhanAbility : PetAbility
{
	public KangaskhanAbility() : base()
	{
		name = "Kangaskhan";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class HitmonleeAbility : PetAbility
{
	public HitmonleeAbility() : base()
	{
		name = "Hitmonlee";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class HitmonchanAbility : PetAbility
{
	public HitmonchanAbility() : base()
	{
		name = "Hitmonchan";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class ChanseyAbility : PetAbility
{
	public ChanseyAbility() : base()
	{
		name = "Chansey";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class DittoAbility : PetAbility
{
	public DittoAbility() : base()
	{
		name = "Ditto";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class PorygonAbility : PetAbility
{
	public PorygonAbility() : base()
	{
		name = "Porygon";
		attack = 1;
		health = 1;
		tier = 6;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class ArticunoAbility : PetAbility
{
	public ArticunoAbility() : base()
	{
		name = "Articuno";
		attack = 1;
		health = 1;
		tier = 7;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class ZapdosAbility : PetAbility
{
	public ZapdosAbility() : base()
	{
		name = "Zapdos";
		attack = 1;
		health = 1;
		tier = 7;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class MoltresAbility : PetAbility
{
	public MoltresAbility() : base()
	{
		name = "Moltres";
		attack = 1;
		health = 1;
		tier = 7;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class MewAbility : PetAbility
{
	public MewAbility() : base()
	{
		name = "Mew";
		attack = 1;
		health = 1;
		tier = 7;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}

public partial class MewtwoAbility : PetAbility
{
	public MewtwoAbility() : base()
	{
		name = "Mewtwo";
		attack = 1;
		health = 1;
		tier = 7;
	}

	public override string AbilityMessage()
    {
        return base.AbilityMessage();
    }
}


//next time just use text replacement lol 

// public partial class PokemonAbility : PetAbility
// {
// 	public PokemonAbility() : base()
// 	{
// 		name = "Pokemon";
// 	}
// }