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
	public int maxExp;
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
		if(petAbility.evolution == null)
		{
			maxExp = 2;
		}
		else
		{
			maxExp = Game.maxExp;
		}
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
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,null,this));
			}
			if(currentItem!=null)
			{
				methodInfo = currentItem.GetType().GetMethod("Hurt");
				if(methodInfo.DeclaringType != typeof(Item))
				{
					Func<Pet, Task> task = currentItem.Hurt;
					game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,null,this));
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
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,null,this));
			}
			if(currentItem!=null)
			{
				methodInfo = currentItem.GetType().GetMethod("Hurt");
				if(methodInfo.DeclaringType != typeof(Item))
				{
					Func<Pet, Task> task = currentItem.Hurt;
					game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,null,this));
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
		Func<Task> task1 = async () => await Task.CompletedTask;
		Func<Task> task2 = async () => await Task.CompletedTask;
		eatenFood += 1;
		if(food.foodAbility.numTargets == 0)
		{
			if(food.health>0)
			{
				task1 = async () => await GainHealth(food.health);
			}
			else if(food.health<0)
			{
				task1 = async () => await ReduceHealth(Math.Abs(food.health));
			}
			if(food.attack>0)
			{
				task2 = async () => await GainAttack(food.attack);
			}
			else if(food.attack<0)
			{
				task2 = async () => await ReduceAttack(Math.Abs(food.attack));
			}
		}
		await game.WaitForFuncTasks(task1, task2);
		await petAbility.AteFood(null);
		await food.foodAbility.OnEaten(this);
	}

	//test giving exp at max evo, test giving enough exp to evolve from 1 to 3 evo, test on pets without evos
	public async Task gainExperience(int amount)
	{
		Func<Task> task1 = async () => await Task.CompletedTask;
		Func<Task> task2 = async () => await Task.CompletedTask;
		if(experience == maxExp)
		{
			await game.WaitForTasks(GainHealth(amount),GainAttack(amount));
			return;
		}
		//should still be able to gain exp when at full. But have conditions in teamSlot so that you can't stack em.

		//also if stone evo or friend evo
		if(experience < 2 && (experience + amount) >= 2 && petAbility.evolution!=null && petAbility.isStoneEvo == false)
		{
			task1 = async () => await evolve();
		}
		if(experience < Game.maxExp && (experience + amount) >= Game.maxExp && petAbility.isStoneEvo == false && petAbility.evolution != null)
		{
			task2 = async () => await evolve();
		}
		if(experience + amount > maxExp)
		{
			experience = maxExp;
		}
		else
		{
			experience += amount;
		}
		game.updateExpTexture(team.teamSlots[index], this);
		await Task.WhenAll(GainHealth(amount),GainAttack(amount));
		await task1();
		await task2();
	}

	public async Task evolve()
	{
		PetAbility previousAbility = petAbility;
		petAbility = petAbility.evolution;
		name = petAbility.name;
		petAbility.basePet = this;
		sellValue += 1;
		game.changeTexture(team.teamSlots[index], this, "team");
		if(game.inBattle != true)
		{
			game.createDescription(team.teamSlots[index], this, "team");
			game.shop.evolveReward();
			// another method to give next tier pets in shop
			// it needs to have a check for combining two level 2s
			await previousAbility.Evolve(this);
			foreach (int i in GD.Range(team.team.Count))
			{
				await petAbility.FriendEvolved(this);
			}
		}
		else
		{
			game.createDescription(team.teamSlots[index], this, "battle");
			//untested
			MethodInfo methodInfo = previousAbility.GetType().GetMethod("Evolve");
			if(methodInfo.DeclaringType != typeof(PetAbility))
			{
				Func<Pet, Task> task = previousAbility.Evolve;
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,null,this));
			}
			foreach (int i in GD.Range(team.team.Count))
			{
				if(team.GetPetAt(i)!=null&&i!=index)
				{
					methodInfo = team.GetPetAt(i).petAbility.GetType().GetMethod("FriendEvolved");
					if(methodInfo.DeclaringType != typeof(PetAbility))
					{
						Func<Pet, Task> task = parameter => team.GetPetAt(i).petAbility.FriendEvolved(parameter);
						game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,this,this));
					}
				}
			}
		}
		await Task.CompletedTask;
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
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,source,this));
			}
			methodInfo = source.petAbility.GetType().GetMethod("Knockout");
			if(methodInfo.DeclaringType != typeof(PetAbility))
			{
				Func<Pet, Task> task = source.petAbility.Knockout;
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,null,this));
			}
			foreach (int i in GD.Range(team.team.Count))
			{
				//untested
				if(team.GetPetAt(i)!=null&&i!=index)
				{
					methodInfo = team.GetPetAt(i).petAbility.GetType().GetMethod("FriendFainted");
					if(methodInfo.DeclaringType != typeof(PetAbility))
					{
						Func<Pet, Task> task = parameter => team.GetPetAt(i).petAbility.Faint(parameter);
						game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,source,this));
					}
				}
				Pet enemyPet = enemyTeam.GetPetAt(i);
				if(enemyPet!=null)
				{
					methodInfo = enemyPet.petAbility.GetType().GetMethod("EnemyFainted");
					if(methodInfo.DeclaringType != typeof(PetAbility))
					{
						Func<Pet, Task> task = parameter => enemyPet.petAbility.EnemyFainted(parameter);
						game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,source,this));
					}
				}
			}
			game.justFainted = true;
		}
		else
		{
			await petAbility.Faint(source);
			foreach (int i in GD.Range(team.team.Count))
			{
				await petAbility.FriendFainted(source);
			}
		}
		team.RemoveAt(index);
	}

	public async Task GainAttack(int attack)
	{
		if(game.inBattle==true)
		{
			await attackAnimation(attack);
			currentAttack += attack;
			game.changeLabel(team.teamSlots[index],this,"team");
		}
		else
		{
			await GainPermanentAttack(attack);
		}
	}

	public async Task GainHealth(int health)
	{
		if(game.inBattle==true)
		{
			await healthAnimation(health);
			currentHealth += health;
			game.changeLabel(team.teamSlots[index],this,"team");
		}
		else
		{
			await GainPermanentHealth(health);
		}
	}

	//for pets like axolotl
	public async Task GainPermanentAttack(int attack)
	{
		this.attack += attack;
		await GainTemporaryAttack(attack);
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public async Task GainPermanentHealth(int health)
	{
		this.health += health;
		await GainTemporaryHealth(health);
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public async Task GainTemporaryAttack(int attack)
	{
		await attackAnimation(attack);
		this.currentAttack += attack;
		game.changeLabel(team.teamSlots[index],this,"team");
	}

	public async Task GainTemporaryHealth(int health)
	{
		await healthAnimation(health);
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

	public async Task healthAnimation(int amount)
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
		instance.Position = team.teamSlots[index].Position + new Vector2(20, 0);
		Sprite2D sprite = (Sprite2D)instance.GetChild(0);
		sprite.Texture = (Texture2D)GD.Load(Game.pngURLBuilder("HealthBuff"));
		((Label)sprite.GetChild(0)).Text = "+" + amount;
		await ((ProjectileAnimation)instance.GetChild(1)).Buff();
	}

	public async Task attackAnimation(int amount)
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
		instance.Position = team.teamSlots[index].Position + new Vector2(-20, 0);
		Sprite2D sprite = (Sprite2D)instance.GetChild(0);
		sprite.Texture = (Texture2D)GD.Load(Game.pngURLBuilder("AttackBuff"));
		((Label)sprite.GetChild(0)).Text = "+" + amount;
		await ((ProjectileAnimation)instance.GetChild(1)).Buff();

	}

	public async Task Snipe(int damage, Pet target)
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
		((Sprite2D)instance.GetChild(0)).Texture = (Texture2D)GD.Load(Game.pngURLBuilder("Snipe"));
		await ((ProjectileAnimation)instance.GetChild(1)).FireProjectile(target.team.teamSlots[target.index]);
		await target.takeDamage(damage, this);
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
			game.createDescription(team.teamSlots[index],this,"team");
		}
		else
		{
			currentItem = item;
			currentItem.basePet = this;
			game.createDescription(team.teamSlots[index],this,"battle");
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

	public List<Pet> getAdjacentPets()
	{
		List<Pet> adjacentPets = new List<Pet>();
		if(index > 0)
		{
			foreach(int i in GD.Range(index - 1, -1, -1))
			{
				if (team.GetPetAt(i)!=null)
				{
					adjacentPets.Add(team.GetPetAt(i));
					break;
				}
			}
		}
		if(index < Game.teamSize)
		{
			foreach(int i in GD.Range(index + 1, Game.teamSize))
			{
				if (team.GetPetAt(i)!=null)
				{
					adjacentPets.Add(team.GetPetAt(i));
					break;
				}
			}
		}
		return adjacentPets;
	}

	
	public Pet getNearestFriendAhead()
	{
		if(index > 0)
		{
			foreach(int i in GD.Range(index - 1, -1, -1))
			{
				if (team.GetPetAt(i)!=null)
				{
					return team.GetPetAt(i);
				}
			}
		}
		return null;
	}

	public Pet getNearestFriendBehind()
	{
		if(index < Game.teamSize)
		{
			foreach(int i in GD.Range(index + 1, Game.teamSize))
			{
				if (team.GetPetAt(i)!=null)
				{
					return team.GetPetAt(i);
				}
			}
		}
		return null;
	}

	public Pet getRandomFriend()
	{
		Pet randomPet = null;
		Random random = new Random();
		List<Pet> newList = new List<Pet>();
		foreach(int i in GD.Range(team.team.Count))
		{
			if(team.GetPetAt(i)!=null && team.GetPetAt(i)!=this)
			{
				newList.Add(team.GetPetAt(i));
			}
		}
		if(newList.Count>=1)
		{
			randomPet = newList[random.Next(0,newList.Count)];
		}
		return randomPet;
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
