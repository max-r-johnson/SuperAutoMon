using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Shop
{
    public Game game {get {return MainNode.game;}}
    public int tier {get {return game.tier;}}
    public List<Pet> shopPets {get; set;}
    public List<Food> shopFood {get; set;}
    //if I were to ever clean this up, these could be gotten rid of
    public int petSlots;
    public int foodSlotCount;
    //for if there should be a constant added to shop pets (i.e. Can/drom/chicken, etc.)
    public int shopBuffHealth;
    public int shopBuffAttack;
    public Pack Pack {get {return game.pack;}}
    public Player Player {get {return game.player;}}
    public Team team {get {return Game.team;}}
    public Pet selectedPet;
    public int selectedPetIndex;
    public Food selectedFood;
    public int selectedFoodIndex;
    public List<Node2D> shopSlots = new List<Node2D>();
    public List<Node2D> foodSlots = new List<Node2D>();


    Random random = new Random();

    public Shop()
    {
        // Pack = Game.pack;
        // Player = Game.player;
        //game = Game;
    }

    //TODO: have a weighted choosing system for current tier, not completely random
    public List<Pet> addPets()
    {
        List<Pet> newPets = new List<Pet>();
        {
            for (int i=0; i<petSlots;i++)
            {
                if(shopPets[i] != null)
                {
                    if (shopPets[i].stored)
                    {
                        newPets.Add(shopPets[i]);
                    }
                }
            }
            while (newPets.Count < petSlots)
            {
                int randomNumber = random.Next(0,Pack.petList.availablePets[tier].Count);
                newPets.Add(new Pet((PetAbility)Activator.CreateInstance(Pack.petList.availablePets[tier][randomNumber])));
            }      
        }
        return newPets;
    }

    public List<Food> addFood()
    {
        List<Food> newFood = new List<Food>();
        {
            for (int i=0; i<foodSlotCount;i++)
            {
                if(shopFood[i] != null)
                {
                    if (shopFood[i].stored)
                    {
                        newFood.Add(shopFood[i]);
                    }
                }
            }
            while (newFood.Count < foodSlotCount)
            {
                //have Foods/food be a dictionary, access from dictionary corresponding to tier
                //have a weighted choosing system, not completely random
                int randomNumber = random.Next(0,Pack.foodList.availableFood[tier].Count);
                //it's not adding a new instance, when i store something it changes the one in the dictionary so they are all stored
                newFood.Add(new Food((FoodAbility)Activator.CreateInstance(Pack.foodList.availableFood[tier][randomNumber])));
            }      
        }
        foreach (Food p in newFood)
        {
        }
        return newFood;
    }

    public void generateShop()
    {
        shopPets = addPets();
        selectedPet = null;
        foreach(int i in GD.Range(shopSlots.Count))
			{
                game.changeTexture(shopSlots[i],shopPets[i], "shop");
                if(shopPets[i] != null)
                {
                    game.createDescription(shopSlots[i],shopPets[i], "shop");
                    if (shopPets[i].stored)
                    {
                        game.changeStorePetTexture(shopSlots[i], "stored");
                    }
                    else
                    {
                        game.changeStorePetTexture(shopSlots[i], "destored");
                    }
                    game.changeLabel(shopSlots[i],shopPets[i],"shop");
                }
			}
        shopFood = addFood();
        selectedFood = null;
        foreach(int i in GD.Range(foodSlots.Count))
			{
                game.changeFoodTexture(foodSlots[i],shopFood[i]);
                if(shopFood[i] != null)
                {
                    game.createDescription(foodSlots[i],shopFood[i], "shop");
                    if (shopFood[i].stored)
                    {
                        game.changeStoreFoodTexture(foodSlots[i], "stored");
                    }
                    else
                    {
                        game.changeStoreFoodTexture(foodSlots[i], "destored");
                    }
                }
			}
    }

    public void roll()
    {
        generateShop();
        decMoney(Game.rollCost);
    }

    public void decMoney(int money)
    {
        Player.money -= money;
        game.mainNode.updateMoneyLabel();
    }

    public void incMoney(int money)
    {
        Player.money += money;
        game.mainNode.updateMoneyLabel();
    }

    public void setMoney(int money)
    {
        Player.money = money;
        game.mainNode.updateMoneyLabel();
    }

    public async Task buyPet(int shopIndex, int teamIndex)
    {
        decMoney(selectedPet.cost);
        shopPets[shopIndex] = null;
        game.changeTexture(shopSlots[shopIndex],shopPets[shopIndex], "shop");
        if(team.GetPetAt(teamIndex)!=null)
        {
            //NEEDS TO CHECK IF STATS ARE HIGHER/HAS ITEM IN CASE SHOP IS SCALED OR HAS ITEMS ATTACHED
            if(team.GetPetAt(teamIndex).experience < Game.maxExp)
            {
                await team.GetPetAt(teamIndex).gainExperience(selectedPet.experience + 1);
            }
            else
            {
                selectedPet = null;
                return;
            }
            selectedPet = null;
        }
        //if empty slot
        else
        {
            team.AddPet(selectedPet, teamIndex);
        }
        await team.GetPetAt(teamIndex).petAbility.Buy(null);
    }

    //for refactoring - make buyFood and buyPet have the same parameters
    public async Task buyFood(Food food, int teamIndex)
    {
        if(food.foodAbility.canBeEaten(team.GetPetAt(teamIndex)))
        {
            decMoney(food.cost);
            shopFood[selectedFoodIndex] = null;
            game.changeFoodTexture(foodSlots[selectedFoodIndex],null);
            await team.GetPetAt(teamIndex).Eat(food);
            foreach(int i in GD.Range(Game.teamSize))
            {
                if(team.GetPetAt(i)!=null)
                {
                    await team.GetPetAt(i).petAbility.FoodPurchased(null);
                }
            }
        }
        selectedFood = null;
    }

    public async Task sellPet(int teamIndex)
    {
        incMoney(team.GetPetAt(teamIndex).sellValue);
        team.RemoveAt(teamIndex);
        game.changeTexture(team.teamSlots[teamIndex],team.team[teamIndex], "team");
        await team.selectedPet.petAbility.Sell(null);
    }

    public async Task replaceShop(Food food)
    {
        foreach(int i in GD.Range(shopFood.Count))
        {
            shopFood[i] = null;
            game.changeFoodTexture(foodSlots[i],shopFood[i]);
        }
        shopFood[0] = food;
        game.changeFoodTexture(foodSlots[0],shopFood[0]);
        game.createDescription(foodSlots[0],food,"shop");
        //This would change if I added an anim to replace the shop food
        await Task.CompletedTask;
    }

    public void replaceShop(Pet pet)
    {

    }

    //maybe should trigger when pets with no evo reach their max exp too?
    public void evolveReward()
    {
        List<Node2D> shopList = new List<Node2D>();
        foreach(Node2D slot in shopSlots)
        {
            shopList.Add(slot);
        }
        if(shopList.Count < Game.maxSlots - 1)
        {
            //add temp slots - define new method - to get to maxSlots
        }
        else
        {
            //replace food slots
        }
        if(tier < Game.numTiers)
        {
            List<Pet> newPets = new List<Pet>();
            int randomNumber = random.Next(0,Pack.petList.tiers[tier+1].Count);
            newPets.Add(new Pet((PetAbility)Activator.CreateInstance(Pack.petList.tiers[tier+1][randomNumber])));
            //TODO need to make sure they are different pets
            randomNumber = random.Next(0,Pack.petList.tiers[tier+1].Count);
            newPets.Add(new Pet((PetAbility)Activator.CreateInstance(Pack.petList.tiers[tier+1][randomNumber])));
            foreach(Pet pet in newPets)
            {
                GD.Print(pet);
            }
        }
        else
        {
            List<Pet> newPets = new List<Pet>();
            int randomNumber = random.Next(0,Pack.petList.tiers[tier].Count);
            newPets.Add(new Pet((PetAbility)Activator.CreateInstance(Pack.petList.tiers[tier][randomNumber])));
            //TODO need to make sure they are different pets
            randomNumber = random.Next(0,Pack.petList.tiers[tier].Count);
            newPets.Add(new Pet((PetAbility)Activator.CreateInstance(Pack.petList.tiers[tier][randomNumber])));
            foreach(Pet pet in newPets)
            {
                GD.Print(pet);
            }
        }
        //add two pets of next tier
        //add slots back with the two new ones at the end - but change what's in em...?
    }

    //commented out because it doesn't properly add a slot and i don't wanna deal with that rn
    // public void addToShop(Food food)
    // {
    //     shopFood[foodSlots + 1] = food;
    // }

    // public void addToShop(Pet pet)
    // {
    //     shopPets[petSlots + 1] = pet;
    // }
}
