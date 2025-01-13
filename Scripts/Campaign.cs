using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection;


public partial class Campaign
{
    public MainNode mainNode {get {return game.mainNode;}}
    public static Game game {get {return MainNode.game;}}
	private Shop shop {get {return game.shop;}}
	private Player player {get {return game.player;}}
	private Pack pack {get {return game.pack;}}
	private Team team {get {return Game.team;}}
    private Team enemyTeam {get {return Game.enemyTeam;}}
    private Random random = new Random();


    //BE CAREFUL WITH THIS, DON'T USE CLONE

    public void generateRound(int round)
    {
        enemyTeam.Clear();
        if(round == 1)
        {
            //((FoodAbility)Activator.CreateInstance(availableFood[i][j])).name + ", ";
            //this needs to be list of types
            List<Type> roundOnePets = new List<Type>{typeof(EkansAbility)};
            // List<Type> roundOnePets = new List<Type>{typeof(WeedleAbility), typeof(RattataAbility),typeof(EkansAbility), typeof(NidoranfAbility), typeof(NidoranmAbility)};
            List<Type> starters = new List<Type>{typeof(BulbasaurAbility), typeof(SquirtleAbility), typeof(CharmanderAbility)};
            Pet beginnerPet = new Pet((PetAbility)Activator.CreateInstance(roundOnePets[random.Next(0,roundOnePets.Count)]));
            Pet starter = new Pet((PetAbility)Activator.CreateInstance(starters[random.Next(0,starters.Count)]),1,1,1);
            if(beginnerPet.name == "Rattata")
            {
                beginnerPet.attack += 1;
            }
            enemyTeam.AddPet(beginnerPet,0);
            enemyTeam.AddPet(starter,1);
        }
        else if(round == 2)
        {
            List<Pet> bugCatcher = new List<Pet>{new Pet(new WeedleAbility(),1,1),new Pet(new CaterpieAbility(),3,3,0,new OranBerry())};
            List<Pet> birdCatcher = new List<Pet>{new Pet(new SpearowAbility()), new Pet(new SpearowAbility()), new Pet(new PidgeyAbility()), new Pet(new PidgeyAbility()), new Pet(new PidgeyAbility())};
            List<Pet> punk = new List<Pet>{new Pet(new NidoranfAbility(),2,2), new Pet(new NidoranmAbility(),2,2), new Pet(new EkansAbility())};

            List<List<Pet>> possibleTrainers = new List<List<Pet>>{bugCatcher,birdCatcher,punk};
            var chosenTrainer = possibleTrainers[random.Next(0,possibleTrainers.Count)];
            foreach(int i in GD.Range(chosenTrainer.Count))
            {
                enemyTeam.AddPet(chosenTrainer[i],i);
            }
        }
        else if(round == 3)
        {
            //added a pidgey as well for some reason, probably didn't clear enemy team correctly
            List<Pet> rocketTeam = new List<Pet>();
            List<Pet> rocketLeads = new List<Pet>{new Pet(new ZubatAbility())};
            // List<Pet> rocketLeads = new List<Pet>{new Pet(new ZubatAbility()),new Pet(new MankeyAbility()),new Pet(new GastlyAbility()),new Pet(new MeowthAbility(),1,1)};
            Pet rocketLead = rocketLeads[random.Next(0,rocketLeads.Count)];
            List<Pet> supportPets = new List<Pet>{new Pet(new EkansAbility(),1,1),new Pet(new RattataAbility(),3,0), new Pet(new PikachuAbility())};
            rocketTeam.Add(rocketLead);
            rocketTeam.Add(new Pet(new SandshrewAbility()));
            if(rocketLead.name=="Zubat")
            {
                rocketTeam.Add(supportPets[random.Next(0,supportPets.Count)]);
                rocketTeam.Add(new Pet(new SpearowAbility(),1,1));
            }
            else
            {
                rocketTeam.Add(supportPets[random.Next(0,supportPets.Count)]);
                rocketTeam.Add(new Pet(new EkansAbility()));
            }
            foreach(int i in GD.Range(rocketTeam.Count))
            {
                enemyTeam.AddPet(rocketTeam[i],i);
            }
            enemyTeam.AddPet(new Pet(new PikachuAbility()),5);
        }
        else
        {
            enemyTeam.AddPet(new Pet(new WeedleAbility()),2);
            enemyTeam.AddPet(new Pet(new EkansAbility()),3);
            enemyTeam.AddPet(new Pet(new BulbasaurAbility()),4);
            enemyTeam.AddPet(new Pet(new SquirtleAbility()),5);
        }
    }
}