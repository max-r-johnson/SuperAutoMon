using Godot;
using System;

public partial class FoodAbility
{
    public int health {get;set;}
    public int attack {get;set;}
    public string name {get;set;}
    public int tier {get;set;}
    public int cost = 3;
    public Food food {get;set;}
	public Team team;
    public Team enemyTeam;
    public Pet enemyPet;
    public Shop shop;
    public virtual string AbilityMessage()
    {
        return "No Ability";
    }

    public virtual void OnEaten(Pet pet)
    {
        
    }
}
