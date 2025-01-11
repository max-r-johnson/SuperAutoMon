using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

public partial class BattleNode : Node
{
    public MainNode mainNode {get {return game.mainNode;}}
    public static Game game {get {return MainNode.game;}}
	private Shop shop {get {return game.shop;}}
	private Player player {get {return game.player;}}
	private Pack pack {get {return game.pack;}}
	private Team team {get {return Game.team;}}
    private Team enemyTeam {get {return Game.enemyTeam;}}
	private List<Pet> permTeam = new List<Pet>();
    private List<Pet> permEnemyTeam = new List<Pet>();
	private List<Node2D> teamSlots {get {return team.teamSlots;}}
	private List<Node2D> enemyTeamSlots {get {return enemyTeam.teamSlots;}}
	private Queue<Tuple<Func<Pet, Task>,Pet>> battleQueue {get {return game.battleQueue;}}
	public Button playButton {get {return (Button)GetNode("Play");}}
	public Button pauseButton {get {return (Button)GetNode("Pause");}}
	private Campaign campaign = new Campaign();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		game.battleNode = this;
		playButton.Pressed += PlayButton;
		pauseButton.Pressed += PauseButton;
		teamSlots.Clear();
		enemyTeamSlots.Clear();
		game.inBattle = true;
        foreach(int i in GD.Range(Game.teamSize))
		{
			game.addSlot(this,i,"battle");
		}
		foreach(int i in GD.Range(Game.teamSize))
		{
			//generate enemy slots too, hide base
			game.addSlot(this,i,"battle2");
		}
		campaign.generateRound(game.round);
		foreach(int i in GD.Range(Game.teamSize))
		{
			Pet teamPet = team.GetPetAt(i);
			if(teamPet != null)
			{
				teamPet.enemyTeam = enemyTeam;
				teamPet.currentItem = teamPet.item;
			}
			permTeam.Add(teamPet);
			Pet enemyTeamPet = enemyTeam.GetPetAt(i);
			if(enemyTeamPet != null)
			{
				enemyTeamPet.enemyTeam = team;
				enemyTeamPet.currentItem = enemyTeamPet.item;
			}
			permEnemyTeam.Add(enemyTeamPet);
			game.changeTexture(teamSlots[i],team.team[i],"team");
			game.changeTexture(enemyTeamSlots[i],enemyTeam.team[i],"team");
			game.changeLabel(teamSlots[i],team.team[i],"team");
			game.changeLabel(enemyTeamSlots[i],enemyTeam.team[i],"team");
			game.createDescription(teamSlots[i],team.team[i],"team");
			game.createDescription(enemyTeamSlots[i],enemyTeam.team[i],"team");
		}
		battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(OrganizeTeam,null));
		battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(StartOfBattleAbilities,null));
		//Start of battle abilities to queue, queue checks ifPaused and stops if so.
		//front attackers to queue
		//also make sure fainting works in battle correctly
		//game.inBattle = false;
        //GetTree().ChangeSceneToFile("res://Main Node.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	//puts all pets at the front of each team, can be made more efficient but I don't think it needs to be.
	//Has pet as an argument so that it can be enqueued into the battleQueue
	public async Task OrganizeTeam(Pet pet)
	{
		bool teamIsEmpty = true;
		bool enemyTeamIsEmpty = true;
		foreach(int i in GD.Range(Game.teamSize))
		{
			if(team.GetPetAt(i)!=null)
			{
				teamIsEmpty = false;
				// GD.Print(team.GetPetAt(i).name + " index " + team.GetPetAt(i).index);
			}
			if(enemyTeam.GetPetAt(i)!=null)
			{
				enemyTeamIsEmpty = false;
				// GD.Print(enemyTeam.GetPetAt(i).name + " index " + enemyTeam.GetPetAt(i).index);
			}
		}
		if(teamIsEmpty == true)
		{
			game.inBattle = false;
			team.team = permTeam;
			GetTree().ChangeSceneToFile("res://Main Node.tscn");
			return;
		}
		else if(enemyTeamIsEmpty == true)
		{
			game.inBattle = false;
			team.team = permTeam;
			GetTree().ChangeSceneToFile("res://Main Node.tscn");
			return;
		}
		foreach(int j in GD.Range(Game.teamSize))
		{
			foreach(int i in GD.Range(Game.teamSize))
			{
				if(team.GetPetAt(i)==null && i!=Game.teamSize-1)
				{
					await team.Swap(i,i+1);
				}
				if(enemyTeam.GetPetAt(i)==null && i!=Game.teamSize-1)
				{
					await enemyTeam.Swap(i,i+1);
				}
			}
		}
	}

	//doesn't work if a pet in the middle of the team faints
	public async Task PetsBattle(Pet pet)
	{
		Pet teamPet = team.GetPetAt(0);
		Pet enemyPet = enemyTeam.GetPetAt(0);
		//doesn't completely work cuz if it's the back pet and there's no reason to organize
		//if something died in the middle of the team, organize first instead of battling.
		//not enqueueing organizeteam cuz it just dequeued petsbattle
		if(game.justFainted == true)
		{
			game.justFainted = false;
			await OrganizeTeam(null);
		}
		else
		{
			//if want to prioritize by attack instead of index, this would compare attack and whichever has higher attack takes damage first
			//if there isn't a front ability, attack. If there is, dequeue and trigger it/them.

			if(CheckFrontAbility() == false)
			{
				Task task1 = Game.GetPetAnimator(teamSlots[teamPet.index]).AnimateAttack();
				Task task2 = Game.GetPetAnimator(enemyTeamSlots[enemyPet.index]).AnimateAttack();
				await game.WaitForTasks(task1, task2);

				//if there is a faint ability or similar, it would get queued.
				//The AttackRecover animation would play, but the AttackFaint animation would not play until the new abilities are dequeued.
				//therefore, this needs to be changed
				await teamPet.takeDamageNoFaint(enemyPet.currentAttack, enemyPet);
				await enemyPet.takeDamageNoFaint(teamPet.currentAttack, teamPet);
				if(teamPet.currentHealth > 0)
				{
					task1 = Game.GetPetAnimator(teamSlots[teamPet.index]).AnimateAttackRecover();
				}
				else
				{
					MethodInfo methodInfo = teamPet.petAbility.GetType().GetMethod("Faint");
					if(methodInfo.DeclaringType != typeof(PetAbility))
					{
						//I think I should just split this up into both faint, neither faint, teamPet faints, enemyPet faints
						//if any of these three options - if enemy did not faint, trigger its recover. queue these. if enemy did faint, queue those. Then, queue faint anim?
					}
							foreach (int i in GD.Range(team.team.Count))
							{
								if(team.GetPetAt(i)!=null&&i!=teamPet.index)
								{
									methodInfo = team.GetPetAt(i).petAbility.GetType().GetMethod("FriendFainted");
									if(methodInfo.DeclaringType != typeof(PetAbility))
									{
										//if any of these three options - if enemy did not faint, trigger its recover. queue these. if enemy did faint, queue those. Then, queue faint anim?
									}
								}
								if(enemyTeam.GetPetAt(i)!=null)
								{
									methodInfo = enemyTeam.GetPetAt(i).petAbility.GetType().GetMethod("EnemyFainted");
									if(methodInfo.DeclaringType != typeof(PetAbility))
									{
										//if any of these three options - if enemy did not faint, trigger its recover. queue these. if enemy did faint, queue those. Then, queue faint anim?
									}
								}
							}
					task1 = Game.GetPetAnimator(teamSlots[teamPet.index]).AnimateAttackFaint();
				}
				if(enemyPet.currentHealth > 0)
				{
					task2 = Game.GetPetAnimator(enemyTeamSlots[enemyPet.index]).AnimateAttackRecover();
				}
				else
				{
					MethodInfo methodInfo = enemyPet.petAbility.GetType().GetMethod("Faint");
					if(methodInfo.DeclaringType != typeof(PetAbility))
					{
						//repeat above
					}
					task2 = Game.GetPetAnimator(enemyTeamSlots[enemyPet.index]).AnimateAttackFaint();
				}
				await game.WaitForTasks(task1, task2);
				GD.Print("animations done");
				if(teamPet.currentHealth <= 0)
				{
					await teamPet.Faint(enemyPet);
				}
				if(enemyPet.currentHealth <= 0)
				{
					await enemyPet.Faint(teamPet);
				}
			}
			else
			{
				BattleDequeue();
			}
		}
	}

	public async Task StartOfBattleAbilities(Pet pet)
	{
		//adds start of battle abilities to queue, tho not by attack like SAP. rn just does by index.
		//should not add petsbattle at the end
		foreach(int i in GD.Range(Game.teamSize))
		{
			//must declare teamPet before hand because cannot have the action call team.getpetat, since it may have changed since originally being queued
			Pet teamPet = team.GetPetAt(i);
			if(teamPet!=null)
			{
				PetAbility ability = teamPet.petAbility;
				MethodInfo methodInfo = ability.GetType().GetMethod("StartOfBattle");
				if(methodInfo.DeclaringType != typeof(PetAbility))
				{
					Func<Pet, Task> task = teamPet.petAbility.StartOfBattle;
					battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,null));
				}
			}
			Pet enemyPet = enemyTeam.GetPetAt(i);
			if(enemyPet!=null)
			{
				PetAbility ability = enemyPet.petAbility;
				MethodInfo methodInfo = ability.GetType().GetMethod("StartOfBattle");
				if(methodInfo.DeclaringType != typeof(PetAbility))
				{
					Func<Pet, Task> task = enemyPet.petAbility.StartOfBattle;
					battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(task,null));
				}
			}
		}
		//if there are SoB abilities, this invokes the first one so that a click isn't wasted on getting them
		//if there aren't, organizePets adds a petsBattle to the queue
		//battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(PetsBattle,null));
		BattleDequeue();
		await Task.CompletedTask;
	}

	//This code kinda sucks lol
	public bool CheckFrontAbility()
	{
		Pet teamPet = team.GetPetAt(0);
		Pet enemyPet = enemyTeam.GetPetAt(0);
		bool frontAbility = false;

		//If the full team is just abras, then the ability doesn't get queued
		int abraCount = 0;
		int nullCount = 0;
		foreach(int i in GD.Range(Game.teamSize))
		{
			if(team.GetPetAt(i)!= null)
			{
				if(team.GetPetAt(i).name == "Abra")
				{
					abraCount += 1;
				}
			}
			else
			{
				nullCount += 1;
			}
		}
		if(abraCount + nullCount == Game.teamSize)
		{
			return false;
		}
		else
		{
			//checks if a pet in front has an in front ability and enqueues it
			if(teamPet!= null)
			{
				PetAbility ability = teamPet.petAbility;
				MethodInfo methodInfo = ability.GetType().GetMethod("InFront");
				if(methodInfo.DeclaringType != typeof(PetAbility))
				{
					battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(teamPet.petAbility.InFront,null));
					frontAbility = true;
				}
			}
			if(enemyPet!=null)
			{
				PetAbility ability = enemyPet.petAbility;
				MethodInfo methodInfo = ability.GetType().GetMethod("InFront");
				if(methodInfo.DeclaringType != typeof(PetAbility))
				{
					battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(enemyPet.petAbility.InFront,null));
					frontAbility = true;
				}
			}
			if(frontAbility == true)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public void BattleDequeue()
	{
		game.FindTargets();
		if(battleQueue.Count<=0)
		{
			battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet>(PetsBattle,null));
		}
		Tuple<Func<Pet, Task>,Pet> tuple = battleQueue.Dequeue();
		Func<Pet, Task> action = tuple.Item1;
		foreach(int i in GD.Range(Game.teamSize))
		{
			if(team.teamSlots[i] != null)
			{
				Game.disableBorder(team.teamSlots[i]);
			}
		}
		foreach(int i in GD.Range(Game.teamSize))
		{
			if(enemyTeam.teamSlots[i] != null)
			{
				Game.disableBorder(enemyTeam.teamSlots[i]);
			}
		}
		action(tuple.Item2);
	}

	public void PlayButton()
	{
		if(!game.mouseDisabled)
		{
			BattleDequeue();
		}
	}

	public void PauseButton()
	{

	}
}