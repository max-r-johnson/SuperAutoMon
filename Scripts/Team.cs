using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

public partial class Team
{
	public List<Pet> team;
	public List<Pet> currentTeam;
	//public List<Pet> enemyTeam {get {return game.enemyTeam.team;}}
	public Game game {get {return MainNode.game;}}

	public Shop shop {get {return game.shop;}}
	public Pet selectedPet;
	//these are used for abilities (rn only in battle, but can be adapted to outside of battle too). They are lists in case multiple fulfill the qualification
	public List<Pet> highestAttack = new List<Pet>();
	public List<Pet> highestHealth = new List<Pet>();
	public List<Pet> lowestAttack = new List<Pet>();
	public List<Pet> lowestHealth = new List<Pet>();
	public Pet lastIndex = new Pet(new PetAbility());
	public List<Node2D> teamSlots = new List<Node2D>();
	public BattleNode battleNode {get {return game.battleNode;}}

	public Team()
	{
		team = new List<Pet>();
		foreach(int i in GD.Range(Game.teamSize))
		{
			team.Add(null);
		}
	}

	public Pet GetPetAt(int index)
	{
		return team[index];
	}

	//rn can get the pet that calls this with their ability
	public Pet GetRandomPet(Pet source)
	{
		Pet randomPet = null;
		Random random = new Random();
		List<Pet> newList = new List<Pet>();
		foreach(int i in GD.Range(team.Count))
		{
			if(GetPetAt(i)!=null && GetPetAt(i)!=source)
			{
				newList.Add(GetPetAt(i));
			}
		}
		if(newList.Count>=1)
		{
			randomPet = newList[random.Next(0,newList.Count)];
		}
		return randomPet;
	}

	public void Clear()
	{
		for(int i=0;i<game.shop.petSlots;i++)
		{
			team[i] = null;
		}
	}

	public void RemoveAt(int index)
	{
		team[index] = null;
		if(game.inBattle == true)
		{
			game.changeTexture(teamSlots[index],team[index],"team");
			VBoxContainer Description = (VBoxContainer)teamSlots[index].GetChildren()[4];
			Description.Hide();
		}
		else if(game.inBattle != true)
		{
			game.changeTexture(teamSlots[index],team[index],"team");
			VBoxContainer Description = (VBoxContainer)teamSlots[index].GetChildren()[4];
			Description.Hide();
		}
		game.changeLabel(teamSlots[index],team[index],"team");
	}

	//this is not tested with in battle addPet (summoning for the future), it may work tho ?
	public void AddPet(Pet pet, int index)
	{
		// if(game.inBattle != true)
		// {
			shop.selectedPet = null;
			team[index] = pet;
			if(pet!=null)
			{
				pet.index = index;
				pet.team = this;
				//pet.petAbility.team = this;
				game.changeLabel(teamSlots[index],pet,"team");
			}
			game.changeTexture(teamSlots[index],pet,"team");
			game.createDescription(teamSlots[index], pet, "team");
			//if a pet is bought from the shop, the description is shown immediately. This is in shop.buyPet
			//this could have issues if the player's mouse leaves the area2D before the pet is finished buying (IE long animations)
	}

	//problems are caused by the user being able to click stuff before the animations have finished
	public async Task Swap(int index1, int index2)
	{
		Pet tempPet;
		if(team[index1]	== null && team[index2] == null)
		{
			return;
		}
		if(game.inBattle != true)
		{
			Task task1 = Game.GetPetAnimator(teamSlots[index2]).AnimateSwap(teamSlots[index2]);
			Task task2 = Game.GetPetAnimator(teamSlots[index2]).AnimateSwap(teamSlots[index1]);

			await game.WaitForTasks(task1, task2);
		}
		else
		{
			Task task1 = Game.GetPetAnimator(teamSlots[index2]).AnimateSwap(teamSlots[index2]);
			Task task2 = Game.GetPetAnimator(teamSlots[index2]).AnimateSwap(teamSlots[index1]);

			await game.WaitForTasks(task1, task2);
		}
		if(team[index1]!=null)
		{
			tempPet = team[index1];
			RemoveAt(index1);
			AddPet(team[index2],index1);
			RemoveAt(index2);
			AddPet(tempPet,index2);
		}
		else
		{
			tempPet = team[index2];
			RemoveAt(index2);
			AddPet(team[index1],index2);
			RemoveAt(index1);
			AddPet(tempPet,index1);
		}
	}

	public async Task Move(Pet pet, int amount)
	{
		if(game.inBattle==true && pet!=null)
		{
			foreach(int i in GD.Range(Math.Abs(amount)))
			{
				if(amount<0)
				{
					if(pet.index!=0)
					{
						Game.enableBorder(teamSlots[pet.index]);
						Game.enableBorder(teamSlots[pet.index-1]);
						await Swap(pet.index, pet.index-1);
					}
					//if can move frontwards
					//move frontwards
				}
				else if(amount>0)
				{
					if(pet.index!=Game.teamSize)
					{
						Game.enableBorder(teamSlots[pet.index]);
						Game.enableBorder(teamSlots[pet.index+1]);
						await Swap(pet.index, pet.index+1);
					}
					//move backwards
				}
			}
			//if the method for "move" was overridden (it's not the base, which is nothing), add it to the queue
			MethodInfo methodInfo = pet.petAbility.GetType().GetMethod("Move");
			if(methodInfo.DeclaringType != typeof(PetAbility))
			{
				Func<Pet, Task> action = pet.petAbility.Move;
				game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(action,null));
			}
			foreach(int i in GD.Range(Game.teamSize))
			{
				//friend moved and enemy moved may need a parameter corresponding to the pet that is moved
				//for each pet in the team, queue its friendmoved ability if its not the pet moved
				Pet teamPet = pet.team.GetPetAt(i);
				if(teamPet!=null&&i!=pet.index)
				{
					methodInfo = teamPet.petAbility.GetType().GetMethod("FriendMoved");
					if(methodInfo.DeclaringType != typeof(PetAbility))
					{
						Func<Pet, Task> action = parameter => teamPet.petAbility.FriendMoved(parameter);
						game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(action,pet));
					}
				}
				//for each pet in the opposing team, queue its enemyMoved ability
				Pet enemyPet = pet.enemyTeam.GetPetAt(i);
				if(enemyPet!=null)
				{
					methodInfo = enemyPet.petAbility.GetType().GetMethod("EnemyMoved");
					if(methodInfo.DeclaringType != typeof(PetAbility))
					{
						Func<Pet, Task> action = parameter => enemyPet.petAbility.EnemyMoved(parameter);
						game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(action,pet));
					}
				}
			}
			//organize for if it gets moved and there is an empty space
			await battleNode.OrganizeTeam(null);
		}
		else
		{
			//for move in shop, still need to check if pet not null
		}
	}
    public override string ToString()
    {
		String newString = "";
		foreach(int index in GD.Range(team.Count))
		{
			newString += "Pet " + index + ": " + team[index] + "\n";
		}
		return newString;
    }
}
