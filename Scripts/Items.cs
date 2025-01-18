using Godot;
using System;
using System.Reflection;
using System.Threading.Tasks;

public partial class Item
{
    public Game game {get {return MainNode.game;}}
    public Pet basePet;
    public string name {get;set;}
    public bool isAilment;

    public Team team {get {return basePet.team;}}
    public Team enemyTeam {get {return basePet.enemyTeam;}}
    public Pet enemyPet;
    public Shop shop {get {return game.shop;}}
    public Item()
    {

    }

    public virtual string itemMessage()
    {
        return "No effect.";
    }

    public virtual async Task Hurt(Pet pet)
    {
        await Task.CompletedTask;
    }

    public virtual async Task GainedAilment(Pet pet)
    {
        await Task.CompletedTask;
    }

    public async Task PerkUsed()
    {
        basePet.RemoveItem();
        if(game.inBattle == true)
        {
            if(basePet!=null)
            {
                MethodInfo methodInfo = basePet.petAbility.GetType().GetMethod("UsedPerk");
                if(methodInfo.DeclaringType != typeof(PetAbility))
                {
                    Func<Pet, Task> task = parameter => basePet.petAbility.UsedPerk(parameter);
                    game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,null,basePet));
                }
            }
            foreach(int i in GD.Range(Game.teamSize))
            {
                //friend moved and enemy moved may need a parameter corresponding to the pet that is moved
                //for each pet in the team, queue its friendmoved ability if its not the pet moved
                Pet teamPet = team.GetPetAt(i);
                if(teamPet!=null&&i!=basePet.index)
                {
                    MethodInfo methodInfo = teamPet.petAbility.GetType().GetMethod("FriendUsedPerk");
                    if(methodInfo.DeclaringType != typeof(PetAbility))
                    {
                        Func<Pet, Task> task = parameter => teamPet.petAbility.FriendUsedPerk(parameter);
                        game.battleQueue.Enqueue(new Tuple<Func<Pet, Task>, Pet, Pet>(task,basePet, teamPet));
                    }
                }
                // //for each pet in the opposing team, queue its enemyMoved ability
                // Pet enemyPet = pet.enemyTeam.GetPetAt(i);
                // if(enemyPet!=null)
                // {
                //     MethodInfo methodInfo = enemyPet.petAbility.GetType().GetMethod("EnemyUsedPerk");
                //     if(methodInfo.DeclaringType != typeof(PetAbility))
                //     {
                //         Func<Pet, Task> task = parameter => enemyPet.petAbility.EnemyUsedPerk(parameter);
                //         game.battleQueue.Enqueue(new Tuple<task<Pet>, Pet>(task,pet));
                //     }
                // }
            }
        }
        else
        {
            await basePet.petAbility.UsedPerk(null);
            foreach(int i in GD.Range(Game.teamSize))
            {
                await team.GetPetAt(i).petAbility.FriendUsedPerk(this.basePet);
            }
        }
    }
}
public partial class OranBerry : Item
{
    public OranBerry() : base()
    {
        name = "Oran Berry";
        isAilment = false;
    }

    public override async Task Hurt(Pet pet)
    {
        if(basePet.currentHealth>0&&basePet.currentItem != null)
        {
            await basePet.GainHealth(2);
            await PerkUsed();
        }
        else
        {
            game.battleNode.BattleDequeue();
        }
    }

    public override string itemMessage()
    {
        return "Gives 2 health when hurt. One use.";
    }
}

public partial class LumBerry : Item
{
    public LumBerry() : base()
    {
        name = "Lum Berry";
        isAilment = false;
    }

    public override async Task GainedAilment(Pet pet)
    {
        //removes the new ailment
        basePet.RemoveItem();
        await PerkUsed();
    }

    public override string itemMessage()
    {
        return "Removes an ailment. One use.";
    }
}

public partial class Poison : Item
{
    public Poison() : base()
    {
        name = "Poison";
        isAilment = true;
    }

    public override string itemMessage()
    {
        return "Take 3 extra damage.";
    }
}

public partial class Leek : Item
{
    public Leek() : base()
    {
        name = "Leek";
        isAilment = false;
    }

    public override string itemMessage()
    {
        return "Removes an ailment. One use.";
    }
}

public partial class BerryJuice : Item
{
    public BerryJuice() : base()
    {
        name = "Berry Juice";
        isAilment = false;
    }

    public override string itemMessage()
    {
        return "Removes an ailment. One use.";
    }
}