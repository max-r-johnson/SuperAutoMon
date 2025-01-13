using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

public partial class PetAbility
{
    public Game game {get {return MainNode.game;}}
    public int health {get;set;}
    public int attack {get;set;}
    public string name {get;set;}
    public int tier {get;set;}
    public int cost {get;set;}
    public Pet basePet {get;set;}
	public Team team {get {return basePet.team;}}
    public Team enemyTeam {get {return basePet.enemyTeam;}}
    public Pet enemyPet {get;set;}
    public Shop shop {get {return game.shop;}}
    public bool isStoneEvo {get;set;}
    public PetAbility evolution {get;set;}
    public virtual string AbilityMessage()
    {
        return "No Ability";
    }

    //Pet target is not actually referring to the pet that invokes this action, but for actions like "friendfainted" or "enemymoved," as they have
    //a target pet. This is the best way I could find to implement the action queue.
    public virtual async Task StartOfTurn(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task EndOfTurn(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task StartOfBattle(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FoodPurchased(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task AteFood(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendAteFood(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task Hurt(Pet source)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendHurt(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task Faint(Pet source)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendFainted(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task EnemyFainted(Pet target)
    {
        await Task.CompletedTask;
    }
    
    public virtual async Task Move(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendMoved(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task EnemyMoved(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task Buy(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendBought(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task Sell(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendSold(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task BeforeAttack(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task AfterAttack(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task UsedPerk(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendUsedPerk(Pet target)
    {
        await Task.CompletedTask;
    }

    //since knockout triggers before the team is organized again, it should be noted that a knockout ability
    //should never target the pet at index 0 on the enemy team.
    public virtual async Task Knockout(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task InFront(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task Evolve(Pet target)
    {
        await Task.CompletedTask;
    }

    public virtual async Task FriendEvolved(Pet target)
    {
        await Task.CompletedTask;
    }
}
