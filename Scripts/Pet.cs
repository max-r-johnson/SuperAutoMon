using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

public partial class Pet
{
	readonly Pack petPack;
	//health, attack are different from currentHealth, currentAttack because some stats are temporary
	public int currentHealth;
	public int currentAttack;
	public static Game game {get {return MainNode.game;}}
	public Team team {get; set;}
	public Team enemyTeam {get;set;}
	public int index {get; set;}
	public bool stored {get; set;}
	public int cost {get; set;}
	public int health {get; set;}
	public int attack {get; set;}
	public string name {get;set;}
	public PetAbility petAbility {get; set;}
	public int experience {get; set;}
	public int tier {get {return petAbility.tier;}}
	public Item item {get; set;}
	public Item currentItem {get;set;}
	public int eatenFood;
	public int sellValue;

	//initialize with its ability, default xp to 0, and get ability and stats from Ability class
	public Pet(PetAbility ability, int extraAttack = 0, int extraHealth = 0, int experience = 0, Item item = null)
	{
		//petPack = pack;
		this.experience = experience;
		//can be overridden
		cost = 3;
		sellValue = 1;
		petAbility = ability;
		health = ability.health + extraHealth;
		attack = ability.attack + extraAttack;
		currentAttack = attack;
		currentHealth = health;
		name = ability.name;
		petAbility.basePet = this;
		this.item = item;
		if(item!=null)
		{
			this.item.basePet = this;
		}
	}

	public async Task takeDamage(int damage, Pet source)
	{
		if (game.inBattle == true)
		{
			currentHealth -= damage;
			MethodInfo methodInfo = petAbility.GetType().GetMethod("Hurt");
			if(methodInfo.DeclaringType != typeof(PetAbility))
			{
				Func<Pet, Task> task = petAbility.Hurt;
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,null));
			}
			if(currentItem!=null)
			{
				methodInfo = currentItem.GetType().GetMethod("Hurt");
				if(methodInfo.DeclaringType != typeof(Item))
				{
					Func<Pet, Task> task = currentItem.Hurt;
					game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,null));
				}
			}
		}
		else
		{
			health -= damage;
			await petAbility.Hurt(source);
			if(item!=null)
			{
				await item.Hurt(null);
			}
		}
		//this is commented out for now so I remember to adjust it to work with the queue system
		// foreach (int i in GD.Range(team.team.Count))
		// {
		// 	//if there is a pet on the team with a friend hurt ability that isn't this one, activate it
		// 	if(team.GetPetAt(i)!=null&&i!=index)
		// 	{
		// 		team.GetPetAt(i).petAbility.FriendHurt();
		// 	}
		// }
		if(game.inBattle==true)
		{
			if(currentHealth<1)
			{
				//game.battleQueue.Enqueue(Faint);
				await Faint(source);
			}
			else
			{
				game.changeLabel(team.teamSlots[index],this,"team");
			}
		}
		else
		{
			if(health<1)
			{
				await Faint(source);
			}
			game.changeLabel(team.teamSlots[index],this,"team");
		}
	}

	public async Task takeDamageNoFaint(int damage, Pet source)
	{
		if (game.inBattle == true)
		{
			currentHealth -= damage;
			MethodInfo methodInfo = petAbility.GetType().GetMethod("Hurt");
			if(methodInfo.DeclaringType != typeof(PetAbility))
			{
				Func<Pet, Task> task = petAbility.Hurt;
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,null));
			}
			if(currentItem!=null)
			{
				methodInfo = currentItem.GetType().GetMethod("Hurt");
				if(methodInfo.DeclaringType != typeof(Item))
				{
					Func<Pet, Task> task = currentItem.Hurt;
					game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,null));
				}
			}
		}
		else
		{
			health -= damage;
			await petAbility.Hurt(source);
			if(item!=null)
			{
				await item.Hurt(null);
			}
		}
		//this is commented out for now so I remember to adjust it to work with the queue system
		// foreach (int i in GD.Range(team.team.Count))
		// {
		// 	//if there is a pet on the team with a friend hurt ability that isn't this one, activate it
		// 	if(team.GetPetAt(i)!=null&&i!=index)
		// 	{
		// 		team.GetPetAt(i).petAbility.FriendHurt();
		// 	}
		// }
		if(game.inBattle==true)
		{
			if(currentHealth>=1)
			{
				game.changeLabel(team.teamSlots[index],this,"team");
			}
		}
		else
		{
			if(health>=1)
			{
				game.changeLabel(team.teamSlots[index],this,"team");
			}
		}
	}

	public async Task Eat(Food food)
	{
		Task task1 = null;
		Task task2 = null;
		eatenFood += 1;
		if(food.health>0)
		{
			task1 = GainHealth(food.health);
		}
		else if(food.health<0)
		{
			task1 = ReduceHealth(Math.Abs(food.health));
		}
		else
		{
			task1 = Task.CompletedTask;
		}
		if(food.attack>0)
		{
			task2 = GainAttack(food.attack);
		}
		else if(food.attack<0)
		{
			task2 = ReduceAttack(Math.Abs(food.attack));
		}
		else
		{
			task2 = Task.CompletedTask;
		}
		await game.WaitForTasks(task1, task2);
		await petAbility.AteFood(null);
		food.foodAbility.OnEaten(this);
	}

	//test giving exp at max evo, test giving enough exp to evolve from 1 to 3 evo, test on pets without evos
	public async Task gainExperience(int amount)
	{
		//should still be able to gain exp when at full. But have conditions in teamSlot so that you can't stack em.

		//also if stone evo or friend evo
		if(experience < 2 && (experience += amount) >= 2 && petAbility.evolution!=null && petAbility.isStoneEvo == false)
		{
			await evolve();
		}
		if(experience < Game.maxExp && (experience += amount) >= Game.maxExp && petAbility.isStoneEvo == false && petAbility.evolution != null)
		{
			await evolve();
		}
		experience += amount;
		//this also needs to be changed for gaining exp when at max evo	
		game.updateExpTexture(team.teamSlots[index], this);
		await game.WaitForTasks(GainHealth(amount),GainAttack(amount));
	}

	public async Task evolve()
	{
		GD.Print("evolving");
		//stats, exp should be same. Should create new pet using the petAbility corresponding to the "evolution" of a pet. The check for gaining exp if at max/can't evolve
		//should be moved to gainExperience. Add check for stoneEvo. If friendship evo, evolve at start of next turn?
		//update texture
		Pet evolvedPet = new Pet(petAbility.evolution);
		evolvedPet.health = health;
		evolvedPet.attack = attack;
		evolvedPet.item = item;
		evolvedPet.sellValue = sellValue;
		await Task.CompletedTask;
		// another method to give next tier pets in shop
	}

	//need to fix remove at for this. I didn't want it to remove right away cuz the animation needs to play	
	public async Task Faint(Pet source)
	{
		if(game.inBattle)
		{
			MethodInfo methodInfo = petAbility.GetType().GetMethod("Faint");
			if(methodInfo.DeclaringType != typeof(PetAbility))
			{
				Func<Pet, Task> task = parameter => petAbility.Faint(parameter);
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,source));
			}
			game.justFainted = true;
			methodInfo = source.petAbility.GetType().GetMethod("Knockout");
			if(methodInfo.DeclaringType != typeof(PetAbility))
			{
				Func<Pet, Task> task = source.petAbility.Knockout;
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,null));
			}
		}
		else
		{
			await petAbility.Faint(source);
		}
		foreach (int i in GD.Range(team.team.Count))
		{
			//untested
			if(team.GetPetAt(i)!=null&&i!=index)
			{
				MethodInfo methodInfo = team.GetPetAt(i).petAbility.GetType().GetMethod("FriendFainted");
				if(methodInfo.DeclaringType != typeof(PetAbility))
				{
					Func<Pet, Task> task = parameter => team.GetPetAt(i).petAbility.Faint(parameter);
					game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,source));
				}
			}
			Pet enemyPet = enemyTeam.GetPetAt(i);
			if(enemyPet!=null)
			{
				MethodInfo methodInfo = enemyPet.petAbility.GetType().GetMethod("EnemyFainted");
				if(methodInfo.DeclaringType != typeof(PetAbility))
				{
					Func<Pet, Task> task = parameter => enemyPet.petAbility.EnemyFainted(parameter);
					game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,this));
				}
			}
		}
		team.RemoveAt(index);
	}

	public async Task GainAttack(int attack)
	{
		GD.Print("gain attack started");
		var projectile = GD.Load<PackedScene>("res://Projectile.tscn");
		Node2D instance = (Node2D)projectile.Instantiate();
		if(game.inBattle == true)
		{
			game.battleNode.AddChild(instance);
		}
		else
		{
			game.mainNode.AddChild(instance);
		}
		instance.Position = team.teamSlots[index].Position + new Vector2(-20, 0);
		((Sprite2D)instance.GetChild(0)).Texture = (Texture2D)GD.Load(Game.pngURLBuilder("AttackBuff"));
		await ((ProjectileAnimation)instance.GetChild(1)).FireProjectile(team.teamSlots[index]);

		if(game.inBattle==true)
		{
			currentAttack += attack;
			game.changeLabel(team.teamSlots[index],this,"team");
		}
		else
		{
			GainPermanentAttack(attack);
		}
	}

	public async Task GainHealth(int health)
	{
		GD.Print("gain health started");
		var projectile = GD.Load<PackedScene>("res://Projectile.tscn");
		Node2D instance = (Node2D)projectile.Instantiate();
		if(game.inBattle == true)
		{
			game.battleNode.AddChild(instance);
		}
		else
		{
			game.mainNode.AddChild(instance);
		}
		instance.Position = team.teamSlots[index].Position + new Vector2(20, 0);
		((Sprite2D)instance.GetChild(0)).Texture = (Texture2D)GD.Load(Game.pngURLBuilder("HealthBuff"));
		await ((ProjectileAnimation)instance.GetChild(1)).FireProjectile(team.teamSlots[index]);

		if(game.inBattle==true)
		{
			currentHealth += health;
			game.changeLabel(team.teamSlots[index],this,"team");
		}
		else
		{
			GainPermanentHealth(health);
		}
	}

	//for pets like axolotl
	public void GainPermanentAttack(int attack)
	{
		this.attack += attack;
		GainTemporaryAttack(attack);
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public void GainPermanentHealth(int health)
	{
		this.health += health;
		GainTemporaryHealth(health);
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public void GainTemporaryAttack(int attack)
	{
		this.currentAttack += attack;
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public void GainTemporaryHealth(int health)
	{
		this.currentHealth += health;
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public async Task ReduceAttack(int attack)
	{
		if(game.inBattle==true)
		{
			this.currentAttack -= attack;
			if(this.currentAttack<=0)
			{
				this.currentAttack = 1;
			}
		}
		else
		{
			this.attack -= attack;
			this.currentAttack -= attack;
			if(this.attack<=0)
			{
				this.attack = 1;
			}
			if(this.currentAttack<=0)
			{
				this.currentAttack = 1;
			}
		}
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public async Task ReduceHealth(int health)
	{
		if(game.inBattle==true)
		{
			this.currentHealth -= health;
			if(this.currentHealth<=0)
			{
				this.currentHealth = 1;
			}
		}
		else
		{
			this.health -= health;
			this.currentHealth -= health;
			if(this.currentHealth<=0)
			{
				this.currentHealth = 1;
			}
			if(this.health<=0)
			{
				this.health = 1;
			}
		}
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public void SetAttack(int attack)
	{		
		if(game.inBattle==true)
		{
			this.currentAttack = attack;
		}
		else
		{
			this.attack = attack;
		}
		//I DON'T KNOW WHY THIS ISN'T WORKING
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public void SetHealth(int health)
	{		
		if(game.inBattle==true)
		{
			this.currentHealth = health;
		}
		else
		{
			this.health = health;
		}
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public async Task Snipe(int damage, Pet target)
	{
		GD.Print("snipe started");
		var projectile = GD.Load<PackedScene>("res://Projectile.tscn");
		Node2D instance = (Node2D)projectile.Instantiate();
		if(game.inBattle == true)
		{
			game.battleNode.AddChild(instance);
		}
		else
		{
			game.mainNode.AddChild(instance);
		}
		instance.Position = team.teamSlots[index].Position;
		((Sprite2D)instance.GetChild(0)).Texture = (Texture2D)GD.Load(Game.pngURLBuilder("Snipe"));
		await ((ProjectileAnimation)instance.GetChild(1)).FireProjectile(target.team.teamSlots[target.index]);
		await target.takeDamage(damage, this);
		GD.Print("snipe finished");
	}

	//should have perm + temp attack/health too
	public enum BuffType
	{
		SetAttack,
		GainAttack,
		SetHealth,
		GainHealth
	}

	public async Task GiveBuff(int amount, Pet target, BuffType buffType)
	{
		GD.Print("Give buff triggered");
		var projectile = GD.Load<PackedScene>("res://Projectile.tscn");
		Node2D instance = (Node2D)projectile.Instantiate();
		if(game.inBattle == true)
		{
			game.battleNode.AddChild(instance);
		}
		else
		{
			game.mainNode.AddChild(instance);
		}
		instance.Position = team.teamSlots[index].Position;
		((Sprite2D)instance.GetChild(0)).Texture = (Texture2D)GD.Load(Game.pngURLBuilder("Buff"));
		await ((ProjectileAnimation)instance.GetChild(1)).FireProjectile(target.team.teamSlots[target.index]);

		switch (buffType)
		{
			case BuffType.SetAttack:
				target.SetAttack(amount);
				break;
			case BuffType.GainAttack:
				await target.GainAttack(amount);
				break;
			case BuffType.SetHealth:
				target.SetHealth(amount);
				break;
			case BuffType.GainHealth:
				await target.GainHealth(amount);
				break;
			default:
				throw new ArgumentException("Invalid BuffType.");
		}
	}

	public enum DebuffType
	{
		SetAttack,
		ReduceAttack,
		SetHealth,
		ReduceHealth
	}

	public async Task GiveDebuff(int amount, Pet target, DebuffType debuffType)
	{
		var projectile = GD.Load<PackedScene>("res://Projectile.tscn");
		Node2D instance = (Node2D)projectile.Instantiate();
		if(game.inBattle == true)
		{
			game.battleNode.AddChild(instance);
		}
		else
		{
			game.mainNode.AddChild(instance);
		}
		instance.Position = team.teamSlots[index].Position;
		((Sprite2D)instance.GetChild(0)).Texture = (Texture2D)GD.Load(Game.pngURLBuilder("Debuff"));
		await ((ProjectileAnimation)instance.GetChild(1)).FireProjectile(target.team.teamSlots[target.index]);

		switch (debuffType)
		{
			case DebuffType.SetAttack:
				target.SetAttack(amount);
				break;
			case DebuffType.ReduceAttack:
				await target.ReduceAttack(amount);
				break;
			case DebuffType.SetHealth:
				target.SetAttack(amount);
				break;
			case DebuffType.ReduceHealth:
				await target.ReduceAttack(amount);
				break;
			default:
				throw new ArgumentException("Invalid DebuffType.");
		}
	}

	public void GiveItem(Item item)
	{
		//needs a thing for lum berry that checks if pet is holding a lum berry and if the new item is an ailment. then uses perk
		if(game.inBattle!=true)
		{
			this.item = item;
			this.item.basePet = this;
			currentItem = this.item;
		}
		else
		{
			currentItem = item;
			currentItem.basePet = this;
		}
		game.changeTexture(team.teamSlots[index],this,"team");
	}

	public void RemoveItem()
	{
		if(game.inBattle!=true)
		{
			this.item = null;
		}
		else
		{
			this.currentItem = null;
		}
		game.changeTexture(team.teamSlots[index],this,"team");
	}

	public Pet Clone()
	{
		return (Pet)MemberwiseClone();
	}
	public override string ToString()
	{
		return "Pet: " + attack + "/" + health + " (" + currentAttack + "/" + currentHealth + ") " + name + " - " + petAbility.AbilityMessage();
	}
}
