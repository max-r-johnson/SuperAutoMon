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
		return "Faint => BLEEEHHHH.";
	}

    // public override async async Task Faint(Pet source)
    // {
	// 	//I want this to trigger before it gets yeeted offscreen. However, I also want the other pet to do the recover animation at the same time as this.
	// 	//if there is a faint ability or any friend faint ability, the other recovers. The faint abilities gets enqueued. The faint ability triggers. The pet gets yeeted.
	// 	await game.WaitForTasks(Game.GetAnimator(team.teamSlots[basePet.index]).AnimateAttack());
    // }
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
		return "Faint => BLEEEHHHH.";
	}

    // public override async async Task Faint(Pet source)
    // {
	// 	//I want this to trigger before it gets yeeted offscreen. However, I also want the other pet to do the recover animation at the same time as this.
	// 	//if there is a faint ability or any friend faint ability, the other recovers. The faint abilities gets enqueued. The faint ability triggers. The pet gets yeeted.
	// 	await game.WaitForTasks(Game.GetAnimator(team.teamSlots[basePet.index]).AnimateAttack());
    // }
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
		return "Faint => BLEEEHHHH.";
	}

    // public override async async Task Faint(Pet source)
    // {
	// 	//I want this to trigger before it gets yeeted offscreen. However, I also want the other pet to do the recover animation at the same time as this.
	// 	//if there is a faint ability or any friend faint ability, the other recovers. The faint abilities gets enqueued. The faint ability triggers. The pet gets yeeted.
	// 	await game.WaitForTasks(Game.GetAnimator(team.teamSlots[basePet.index]).AnimateAttack());
    // }
}

public partial class CharmanderAbility : PetAbility
{
	public CharmanderAbility() : base()
    {
		name = "Charmander";
		attack = 3;
    	health = 2;
		tier = 1;
    }

    // public override async Task StartOfBattle(Pet target)
    // {
    //     Pet randomPet = team.GetRandomPet();
    // 	randomPet.GainAttack(1);
    // 	randomPet.GainHealth(1);
    // }
}

public partial class SquirtleAbility : PetAbility
{
	public SquirtleAbility() : base()
    {
		name = "Squirtle";
		attack = 2;
    	health = 3;
		tier = 1;
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
    }

	public override string AbilityMessage()
	{
		return "Sell => Put a free apple in the shop.";
	}

    public override async Task Sell(Pet target)
    {
		Food addedFood = new Food(new TinyAppleAbility());
		addedFood.foodAbility.cost = 0;
		await shop.replaceShop(addedFood);
		GD.Print("Replaced shop with a free apple!");
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
		foreach (int i in GD.Range(1))
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
			GD.Print(enemyTeam.lastIndex);
			GD.Print(this.name + " moved " + enemyTeam.lastIndex.name + " 1 space forward!");
        	await enemyTeam.Move(enemyTeam.lastIndex,-1);
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
    }

	public override string AbilityMessage()
	{
		return "End of Turn => Gain 1 attack.";
	}

    public override async Task EndOfTurn(Pet target)
    {
		GD.Print("rat ability triggered");
		await game.WaitForTasks(basePet.GainAttack(1));
		GD.Print(basePet.name + " gained 1 attack!");
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
			Game.enableBorder(enemyTeam.teamSlots[target.index]);
			await game.WaitForTasks(basePet.Snipe(1,target));
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
    }

    public override string AbilityMessage()
    {
        return "Start of Battle => Swap attack and health.";
    }

    public override async Task StartOfBattle(Pet target)
    {
		await base.StartOfBattle(null);
		//must do pet.attack instead of attack cuz petAbility attack and health are just the base
		if(basePet.currentHealth>0)
		{
			int tempAttack = basePet.attack;
			basePet.SetAttack(basePet.health);
			basePet.SetHealth(tempAttack);
			GD.Print(basePet.name + " swapped its attack and health!");
		}
		else
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
				GD.Print("Pikachu ability against " + randomPet.name);
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

public partial class SandshrewAbility : PetAbility
{
	public SandshrewAbility() : base()
	{
		name = "Sandshrew";
		attack = 1;
		health = 3;
		tier = 2;
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

public partial class ClefairyAbility : PetAbility
{
	public ClefairyAbility() : base()
	{
		name = "Clefairy";
		attack = 2;
		health = 4;
		tier = 2;
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

public partial class JigglypuffAbility : PetAbility
{
	public JigglypuffAbility() : base()
	{
		name = "Jigglypuff";
		attack = 1;
		health = 2;
		tier = 2;
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

public partial class OddishAbility : PetAbility
{
	public OddishAbility() : base()
	{
		name = "Oddish";
		attack = 1;
		health = 1;
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Food Purchased => Temporarily gain +1/1 this turn.";
    }

    public override async Task FoodPurchased(Pet target)
    {
        basePet.GainTemporaryAttack(1);
		basePet.GainTemporaryHealth(1);
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

public partial class MankeyAbility : PetAbility
{
	public MankeyAbility() : base()
	{
		name = "Mankey";
		attack = 1;
		health = 4;
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Hurt => Gain 4 attack.";
    }

    public override async Task Hurt(Pet source)
    {
		//probably should dequeue if mankey is dead
        await basePet.GainAttack(4);
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
        Pet randomPet = team.GetRandomPet(basePet);
		if(randomPet != null)
		{
			Task task1 = basePet.GiveBuff(1, randomPet, Pet.BuffType.GainAttack);
			Task task2 = basePet.GiveBuff(1, randomPet, Pet.BuffType.GainHealth);
			await game.WaitForTasks(task1, task2);
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

public partial class BellsproutAbility : PetAbility
{
	public BellsproutAbility() : base()
	{
		name = "Bellsprout";
		attack = 1;
		health = 2;
		tier = 2;
	}

    public override string AbilityMessage()
    {
        return "Friend Used Perk => Give it 1 attack and 2 health.";
    }

	//doesn't work when it uses the perk
    public override async Task FriendUsedPerk(Pet target)
    {
		Task task1 = basePet.GiveBuff(1, target, Pet.BuffType.GainAttack);
		Task task2 = basePet.GiveBuff(2, target, Pet.BuffType.GainHealth);
		await game.WaitForTasks(task1, task2);
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

public partial class VulpixAbility : PetAbility
{
	public VulpixAbility() : base()
	{

	}
}

public partial class ParasAbility : PetAbility
{
	public ParasAbility() : base()
	{

	}
}

public partial class VenonatAbility : PetAbility
{
	public VenonatAbility() : base()
	{

	}
}

public partial class DiglettAbility : PetAbility
{
	public DiglettAbility() : base()
	{

	}
}

public partial class PsyduckAbility : PetAbility
{
	public PsyduckAbility() : base()
	{

	}
}

public partial class GrowlitheAbility : PetAbility
{
	public GrowlitheAbility() : base()
	{

	}
}

public partial class MachopAbility : PetAbility
{
	public MachopAbility() : base()
	{

	}
}

public partial class GeodudeAbility : PetAbility
{
	public GeodudeAbility() : base()
	{

	}
}

public partial class OnixAbility : PetAbility
{
	public OnixAbility() : base()
	{

	}
}

public partial class DrowzeeAbility : PetAbility
{
	public DrowzeeAbility() : base()
	{

	}
}

public partial class GoldeenAbility : PetAbility
{
	public GoldeenAbility() : base()
	{

	}
}

public partial class StaryuAbility : PetAbility
{
	public StaryuAbility() : base()
	{
		name = "Staryu";
	}
}

//next time just use text replacement lol 

// public partial class PokemonAbility : PetAbility
// {
// 	public PokemonAbility() : base()
// 	{
//		name = "Pokemon"
// 	}
// }