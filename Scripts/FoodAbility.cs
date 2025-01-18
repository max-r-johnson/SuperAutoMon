using Godot;
using System;
using System.Threading.Tasks;

public partial class FoodAbility
{
    public int health {get;set;}
    public int attack {get;set;}
    public string name {get;set;}
    public int tier {get;set;}
    public int cost = 3;
    public int numTargets;
    public Food baseFood {get;set;}
	public Team team;
    public Team enemyTeam;
    public Pet enemyPet;
    public Shop shop;
    public virtual string AbilityMessage()
    {
        return "No Ability";
    }

    public virtual bool canBeEaten(Pet pet)
    {
        return true;
    }

    public virtual async Task OnEaten(Pet pet)
    {
        await Task.CompletedTask;
    }
}
