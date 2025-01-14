using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class Game
{
	public const int numTiers = 7;
	public const int rollCost = 1;
	public const int shopMoney = 11;
	public const int startSlots = 4;
	public const int foodCount = 2;
	public const int slotSpacing = 100;
	public const int battleSpacing = 90;
	public const int teamSize = 6;
	public const int sellCost = 1;
	public const int petNameSize = 20;
	public const int descriptionSize = 14;
	public const int maxExp = 5;
	public List<int> slotRounds = new List<int>(){5,9};
	public int round {get;set;}
	public int tier {get;set;}
	public bool inBattle {get; set;}
	public Pack pack;
	public Shop shop;
	public Player player;
	public static Team team;
	public static Team enemyTeam;
	public Queue<Tuple<Func<Pet, Task>,Pet,Pet>> battleQueue = new Queue<Tuple<Func<Pet, Task>,Pet,Pet>>();
	public MainNode mainNode;
	public BattleNode battleNode;
	public bool justFainted;
	public int foodThisTurn;
	public bool mouseDisabled;

	//need to have this initialize the start slots and stuff, then mainNode creates slots based on how many current slots the game is at.
	public Game()
	{
		pack = Pack.generateKanto();
		player = new Player();
		shop = new Shop();
		team = new Team();
		//mainNode = node;
		round = 0;
		tier = 0;
		enemyTeam = new Team();
		//enemyTeam.team = new List<Pet>{null,new Pet(new WeedleAbility()),new Pet(new WeedleAbility()),null,null,new Pet(new RattataAbility())};
	}

	public static string pngURLBuilder(string String)
	{
		return "res://Images/" + String + ".png";
	}

	// public void changeTexture(int index, string type)
	// {
	// 	if(type == "team")
	// 	{
	// 		Sprite2D pet = (Sprite2D)team.teamSlots[index].GetChildren()[1];
	// 		Sprite2D stats = (Sprite2D)team.teamSlots[index].GetChildren()[1].GetChildren()[0];
	// 		if(team.GetPetAt(index) == null)
	// 		{
	// 			pet.Texture = null;
	// 			stats.Hide();
	// 		}
	// 		else
	// 		{
	// 			pet.Texture = (Texture2D)GD.Load(pngURLBuilder(team.GetPetAt(index).name));
	// 			stats.Show();
	// 		}
	// 	}
	// 	if(type == "shop")
	// 	{
	// 		Sprite2D pet = (Sprite2D)shop.shopSlots[index].GetChildren()[1];
	// 		Sprite2D stats = (Sprite2D)shop.shopSlots[index].GetChildren()[1].GetChildren()[0];
	// 		if(shop.shopPets[index] == null)
	// 		{
	// 			pet.Texture = null;
	// 			stats.Hide();
	// 			changeTexture(index,"destored");
	// 		}
	// 		else
	// 		{
	// 			pet.Texture = (Texture2D)GD.Load(pngURLBuilder(shop.shopPets[index].name));
	// 			stats.Show();
	// 		}
	// 	}
	// 	if(type == "battle")
	// 	{
	// 		Sprite2D pet;
	// 		Sprite2D stats = (Sprite2D)battleSlots[index].GetChildren()[1].GetChildren()[0];
	// 		Sprite2D ground = (Sprite2D)battleSlots[index].GetChildren()[0];
	// 		if(index<Game.teamSize)
	// 		{
	// 			pet = (Sprite2D)battleSlots[index].GetChildren()[1];
	// 			if(team.GetPetAt(index) == null)
	// 			{
	// 				pet.Texture = null;
	// 				stats.Hide();
	// 			}
	// 			else
	// 			{
	// 				pet.Texture = (Texture2D)GD.Load(pngURLBuilder(team.GetPetAt(index).name));
	// 				stats.Show();
	// 			}
	// 		}
	// 		else
	// 		{
	// 			pet = (Sprite2D)battleSlots[index].GetChildren()[1];
	// 			if(enemyTeam.GetPetAt(index-teamSize) == null)
	// 			{
	// 				pet.Texture = null;
	// 				stats.Hide();
	// 			}
	// 			else
	// 			{
	// 				pet.Texture = (Texture2D)GD.Load(pngURLBuilder(enemyTeam.GetPetAt(index-teamSize).name));
	// 				stats.Show();
	// 			}
	// 		}
	// 		//ground.Hide();
	// 	}
	// 	if(type == "stored")
	// 	{
	// 		Sprite2D stored = (Sprite2D)shop.shopSlots[index].GetChildren()[2];
	// 		stored.Texture = (Texture2D)GD.Load(pngURLBuilder("Stored"));
	// 	}
	// 	if(type == "destored")
	// 	{
	// 		Sprite2D stored = (Sprite2D)shop.shopSlots[index].GetChildren()[2];
	// 		stored.Texture = null;
	// 	}
	// 	if(type == "food")
	// 	{
	// 		Sprite2D stats = (Sprite2D)shop.foodSlots[index].GetChildren()[1].GetChildren()[0];
	// 		Sprite2D food = (Sprite2D)shop.foodSlots[index].GetChildren()[1];
	// 		stats.Hide();
	// 		if(shop.shopFood[index] == null)
	// 		{
	// 			food.Texture = null;
	// 			changeTexture(index,"food destored");
	// 		}
	// 		else
	// 		{
	// 			food.Texture = (Texture2D)GD.Load(pngURLBuilder(shop.shopFood[index].name.Replace(" ", string.Empty)));
	// 		}
	// 	}
	// 	if(type == "food stored")
	// 	{
	// 		Sprite2D stored = (Sprite2D)shop.foodSlots[index].GetChildren()[2];
	// 		stored.Texture = (Texture2D)GD.Load(pngURLBuilder("Stored"));
	// 	}
	// 	if(type == "food destored")
	// 	{
	// 		Sprite2D stored = (Sprite2D)shop.foodSlots[index].GetChildren()[2];
	// 		stored.Texture = null;
	// 	}
	// }
	public void updateExpTexture(Node slot, Pet pet)
	{
		AnimatedSprite2D expSprite = (AnimatedSprite2D)slot.GetChildren()[1].GetChildren()[2];
		if(pet != null)
		{
			expSprite.Show();
			if(pet.maxExp == 2 && pet.experience == pet.maxExp)
			{
				GD.Print("frame 6");
				expSprite.Frame = 6;
			}
			else
			{
				expSprite.Frame = pet.experience;
			}
		}
		else
		{
			expSprite.Hide();
		}
	}

	public void changeTexture(Node slot, Pet pet, string type)
	{
		if(type == "team")
		{
			updateExpTexture(slot, pet);
			Sprite2D petSprite = (Sprite2D)slot.GetChildren()[1];
			Sprite2D statSprite = (Sprite2D)slot.GetChildren()[1].GetChildren()[0];
			Sprite2D itemSprite = (Sprite2D)slot.GetChildren()[1].GetChildren()[1];
			if(pet == null)
			{
				petSprite.Texture = null;
				statSprite.Hide();
				itemSprite.Texture = null;
			}
			else
			{
				petSprite.Texture = (Texture2D)GD.Load(pngURLBuilder(pet.name));
				if(pet.currentItem!=null)
				{
					itemSprite.Texture = (Texture2D)GD.Load(pngURLBuilder(pet.item.name.Replace(" ", string.Empty)));
				}
				else
				{
					itemSprite.Texture = null;
				}
				itemSprite.Show();
				statSprite.Show();
			}
			petSprite.Position = new Godot.Vector2(0,0);
		}
		if(type == "shop")
		{
			Sprite2D petSprite = (Sprite2D)slot.GetChildren()[1];
			Sprite2D statSprite = (Sprite2D)slot.GetChildren()[1].GetChildren()[0];
			Sprite2D itemSprite = (Sprite2D)slot.GetChildren()[1].GetChildren()[1];
			if(pet == null)
			{
				petSprite.Texture = null;
				statSprite.Hide();
				itemSprite.Texture = null;
				changeStorePetTexture(slot, "destored");
			}
			else
			{
				petSprite.Texture = (Texture2D)GD.Load(pngURLBuilder(pet.name));
				if(pet.currentItem!=null)
				{
					itemSprite.Texture = (Texture2D)GD.Load(pngURLBuilder(pet.item.name.Replace(" ", string.Empty)));
				}
				else
				{
					itemSprite.Texture = null;
				}
				itemSprite.Show();
				statSprite.Show();
			}
		}
	}

	public void changeFoodTexture(Node slot, Food food)
	{
		Sprite2D statSprite = (Sprite2D)slot.GetChildren()[1].GetChildren()[0];
		Sprite2D foodSprite = (Sprite2D)slot.GetChildren()[1];
		statSprite.Hide();
		if(food == null)
		{
			foodSprite.Texture = null;
			changeStoreFoodTexture(slot, "destored");
		}
		else
		{
			foodSprite.Texture = (Texture2D)GD.Load(pngURLBuilder(food.name.Replace(" ", string.Empty)));
		}
	}

	//split this into different methods instead of having "type" string
	public void changeStorePetTexture(Node slot, string type)
	{
		Sprite2D sprite = (Sprite2D)slot.GetChildren()[1];
		if(sprite.Texture == null)
		{
			return;
		}
		string spritePath = sprite.Texture.ResourcePath.Split('/').Last();
		string pokemonName = spritePath.Substring(spritePath.LastIndexOf('/') + 1, spritePath.LastIndexOf('.') - spritePath.LastIndexOf('/') - 1);
		if(type == "stored")
		{
			sprite.Texture = (Texture2D)GD.Load(pngURLBuilder("Stored/" + pokemonName));
		}
		else if (type == "destored")
		{
			sprite.Texture = (Texture2D)GD.Load(pngURLBuilder(pokemonName));
		}
	}

	public void changeStoreFoodTexture(Node slot, string type)
	{
		Sprite2D sprite = (Sprite2D)slot.GetChildren()[2];
		if(type == "stored")
		{
			sprite.Texture = (Texture2D)GD.Load(pngURLBuilder("Stored"));
		}
		else if (type == "destored")
		{
			sprite.Texture = null;
		}
	}

	public void changeLabel(Node slot, Pet pet, string type)
	{
		if(pet!=null)
		{
			if(type=="shop")
			{
				Label health = (Label)slot.GetChildren()[1].GetChildren()[0].GetChildren()[0];
				health.Text = pet.health.ToString();
				Label attack = (Label)slot.GetChildren()[1].GetChildren()[0].GetChildren()[1];
				attack.Text = pet.attack.ToString();
			}
			if(type=="team")
			{
				// if(inBattle == true)
				{
					Label health = (Label)slot.GetChildren()[1].GetChildren()[0].GetChildren()[0];
					health.Text = pet.currentHealth.ToString();
					Label attack = (Label)slot.GetChildren()[1].GetChildren()[0].GetChildren()[1];
					attack.Text = pet.currentAttack.ToString();
				}
				// else
				// {
				// 	Label health = (Label)slot.GetChildren()[1].GetChildren()[0].GetChildren()[0];
				// 	health.Text = pet.health.ToString();
				// 	Label attack = (Label)slot.GetChildren()[1].GetChildren()[0].GetChildren()[1];
				// 	attack.Text = pet.attack.ToString();
				// }
			}
		}
	}

	public void createDescription(Node slot, Pet pet, string type)
	{
		if(pet!=null)
		{
			VBoxContainer Window = (VBoxContainer)slot.GetChildren()[4];
			Window.ZIndex = 1;
			Window.SetPosition(new Godot.Vector2(-80,-140));
			Window.SetSize(new Godot.Vector2(160,80));
			Window.MouseFilter = Control.MouseFilterEnum.Ignore;
			Panel panel = (Panel)Window.GetChild(0);
			panel.MouseFilter = Control.MouseFilterEnum.Ignore;
			Label Name = (Label)Window.GetChildren()[0].GetChildren()[0];
			Name.Text = pet.name;
			while(Name.GetMinimumSize().X>105)
			{
				Name.LabelSettings.FontSize -=1;
			}
			Label Cost = (Label)Window.GetChildren()[0].GetChildren()[1].GetChildren()[0];
			Cost.Text = pet.cost.ToString();
			Label Tier = (Label)Window.GetChildren()[0].GetChildren()[2];
			Tier.Text = "Tier: " + pet.tier;
			Label Description = (Label)Window.GetChildren()[0].GetChildren()[3];
			createLabelSettings(Description,descriptionSize);
			Description.Text = pet.petAbility.AbilityMessage();
			// Label ItemDescription = (Label)Window.GetChildren()[0].GetChildren()[4];
			// createLabelSettings(ItemDescription,descriptionSize);
			// if(pet.item!= null)
			// {
			// 	ItemDescription.Text = pet.item.itemMessage();
			// 	Window.SetSize(new Godot.Vector2(Window.Size.X,Window.Size.Y + 100));
			// 	Window.SetPosition(new Godot.Vector2(Window.Position.X,Window.Position.Y - 100));
			// }
			Godot.Vector2 size = AdjustedDescriptionSize(Description, 150);
			Description.Size = size;
			Window.SetSize(new Godot.Vector2(160, size.Y + Tier.Size.Y + Name.Size.Y));
			float xPos = Window.Position.X;
			//global position due to adjusted canvaslayer position
			if(Window.GlobalPosition.X <= -30)
			{
				xPos = Window.Position.X + 50;
			}
			else if(Window.GlobalPosition.X >= 1022)
			{
				xPos = Window.Position.X - 50;
			}
			Window.SetPosition(new Godot.Vector2(xPos,Window.Position.Y - size.Y + 25));
			if(type == "team")
			{
				//should show up directly to the right of the slot
				Window.SetPosition(new Godot.Vector2(Window.Position.X + 130, Window.Position.Y + 100));
			}
		}
	}

	public void createDescription(Node slot, Food food, string type)
	{
		VBoxContainer Window = (VBoxContainer)slot.GetChildren()[4];
		Window.SetPosition(new Godot.Vector2(-80,-140));
		Window.SetSize(new Godot.Vector2(160,80));
		Window.MouseFilter = Control.MouseFilterEnum.Ignore;
		Panel panel = (Panel)Window.GetChild(0);
		panel.MouseFilter = Control.MouseFilterEnum.Ignore;
		Label Name = (Label)Window.GetChildren()[0].GetChildren()[0];
		createLabelSettings(Name,petNameSize);
		Name.Text = food.name;
		while(Name.GetMinimumSize().X>105)
		{
			Name.LabelSettings.FontSize -= 1;
		}
		Label Cost = (Label)Window.GetChildren()[0].GetChildren()[1].GetChildren()[0];
		Cost.Text = food.cost.ToString();
		Label Tier = (Label)Window.GetChildren()[0].GetChildren()[2];
		Tier.Text = "Tier: " + food.tier;
		Label Description = (Label)Window.GetChildren()[0].GetChildren()[3];
		createLabelSettings(Description,descriptionSize);
		Description.Text = food.foodAbility.AbilityMessage();
		//CANNOT FOR THE LIFE OF ME FIGURE OUT HOW TO SHOW ACCURATE SIZE (EVEN THO MIN SIZE WORKED WITH NAME)
		Godot.Vector2 size = AdjustedDescriptionSize(Description, 150);
		Description.Size = size;
		Window.SetSize(new Godot.Vector2(160, size.Y + Tier.Size.Y + Name.Size.Y));
		Window.SetPosition(new Godot.Vector2(Window.Position.X,Window.Position.Y - size.Y + 36));
		// if(Window.Position.X)
	}

	public static Godot.Vector2 AdjustedDescriptionSize(Label description, float maxWidth)
    {
        // Ensure you are using the correct font
        var font = description.LabelSettings.Font;
        var text = description.Text;
        var lines = new List<string>();
        var currentLine = "";
        var words = text.Split(' ');

        foreach (var word in words)
        {
            var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
            var testSize = font.GetStringSize(testLine).X;

            if (testSize > maxWidth)
            {
                // If adding the word exceeds maxWidth, finalize the current line
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }

        // Append the final line
        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine);
        }

        // Set the description text to wrapped lines
        description.Text = string.Join("\n", lines);

        // Adjust the size based on the number of lines
        var lineHeight = font.GetHeight();
        return new Godot.Vector2(maxWidth, lineHeight * lines.Count);
    }

	public void createLabelSettings(Label label, int size)
	{
        label.LabelSettings = new LabelSettings
        {
            Font = (Font)GD.Load("res://Font//LapsusPro-Bold.otf"),
            FontColor = new Color("#000000"),
            FontSize = size
        };
    }


	public async Task EndTurn()
	{
		foreach(int i in GD.Range(teamSize))
		{
			if(team.GetPetAt(i)!=null)
			{
				await team.GetPetAt(i).petAbility.EndOfTurn(null);
			}
		}

		await Task.Delay(100);
	}

	public async Task StartTurn()
	{
		foodThisTurn = 0;
		incRound();
		if(round==1)
		{
			shop.petSlots += startSlots;
			shop.foodSlotCount += foodCount;
		}
		else if(slotRounds.Contains(round))
		{
			shop.petSlots += 1;
		}
		foreach(int i in GD.Range(shop.petSlots))
		{
			addSlot(mainNode,i, "shop");
		}
		foreach(int i in GD.Range(Game.teamSize))
		{
			if(team.GetPetAt(i)!=null)
			{
				Pet pet = team.GetPetAt(i);
				pet.index = i;
				pet.eatenFood = 0;
				pet.currentAttack = pet.attack;
				pet.currentHealth = pet.health;
				pet.currentItem = pet.item;
			}
			addSlot(mainNode,i, "team");
		}
		foreach(int i in GD.Range(shop.foodSlotCount))
		{
			addSlot(mainNode,i,"food");
		}
		shop.generateShop();
		foreach (int i in GD.Range(team.team.Count))
		{
			if(team.GetPetAt(i)!=null)
			{
				await team.GetPetAt(i).petAbility.StartOfTurn(null);
			}
		}
	}
	//should maybe have selectPet method that also updates buttons because it's kinda sloppy rn

	//rn doesn't work cuz it's called multiple times (each time it goes back to main node scene) and petSlot is incremented each time
	public void addSlot(Node node, int index, string type)
	{
		if(type == "shop")
		{
			var shopSlot = GD.Load<PackedScene>("res://ShopSlot.tscn");
			ShopArea2D instance = (ShopArea2D)shopSlot.Instantiate();
			instance.Name = "ShopSlot" + index; 
			node.AddChild(instance);
			shop.shopSlots.Add(instance);
			instance.Position = new Godot.Vector2((index + 1) * Game.slotSpacing,400);
			instance.index = index;
			instance.game = this;
			if(shop.shopPets == null)
			{
				shop.shopPets = new List<Pet>();
			}
			//if not a round where an extra slot should be added
			while(shop.shopPets.Count < shop.petSlots)
			{
				shop.shopPets.Add(null);
			}
		}
		//team is different since there is no generateTeam method(maybe should add), so labels,textures,descriptions must be generated again.
		if(type == "team")
		{
			var teamSlot = GD.Load<PackedScene>("res://TeamSlot.tscn");
			TeamArea2D instance = (TeamArea2D)teamSlot.Instantiate();
			instance.Name = "TeamSlot" + index; 
			node.AddChild(instance);
			team.teamSlots.Add(instance);
			instance.Position = new Godot.Vector2(Game.slotSpacing + ((Game.teamSize - 1) * Game.slotSpacing) - ((index) * Game.slotSpacing),200);
			instance.slotIndex = index;
			instance.game = this;
			changeTexture(instance,team.team[index],"team");
			changeLabel(instance,team.team[index],"team");
			createDescription(instance,team.team[index],"team");
		}
		if(type == "food")
		{
			var shopSlot = GD.Load<PackedScene>("res://ShopSlot.tscn");
			ShopArea2D instance = (ShopArea2D)shopSlot.Instantiate();
			instance.Name = "FoodSlot" + index; 
			node.AddChild(instance);
			shop.foodSlots.Add(instance);
			instance.Position = new Godot.Vector2((index + 1) * Game.slotSpacing + (Game.startSlots + 2) * Game.slotSpacing,400);
			instance.index = index;
			instance.game = this;
			instance.isFood = true;
			if(shop.shopFood == null)
			{
				shop.shopFood = new List<Food>();
			}
			while(shop.shopFood.Count < shop.foodSlotCount)
			{
				shop.shopFood.Add(null);
			}
		}
		if(type == "battle")
		{
			var teamSlot1 = GD.Load<PackedScene>("res://TeamSlot.tscn");
			TeamArea2D instance = (TeamArea2D)teamSlot1.Instantiate();
			instance.Name = "TeamSlot" + index; 
			node.AddChild(instance);
			team.teamSlots.Add(instance);
			instance.Position = new Godot.Vector2(50+((Game.teamSize - 1) * Game.battleSpacing) - ((index) * Game.battleSpacing),350);
			instance.slotIndex = index;
			instance.game = this;
		}
		if(type == "battle2")
		{
			var teamSlot = GD.Load<PackedScene>("res://TeamSlot.tscn");
			TeamArea2D instance = (TeamArea2D)teamSlot.Instantiate();
			instance.Name = "EnemyTeamSlot" + index; 
			node.AddChild(instance);
			enemyTeam.teamSlots.Add(instance);
			instance.Position = new Godot.Vector2(1102 - ((Game.teamSize-1) * Game.battleSpacing) + (index * Game.battleSpacing),350);
			instance.slotIndex = index;
			instance.game = this;
		}
	}

	public int incRound()
	{
		round += 1;
		if (round%2==1)
		{
			tier +=1;
			// if(slotRounds.Contains(round))
			// {
			// 	addSlot(mainNode,shop.petSlots, "shop");
			// }
		}
		//shop.generateShop();
		shop.setMoney(Game.shopMoney);
		//if tier 2 or tier 4, adds another shop slot instance to the mainNode.
		return round;
	}

	public void FindTargets()
	{
		team.highestAttack.Clear();
		team.lowestAttack.Clear();
		team.highestHealth.Clear();
		team.lowestHealth.Clear();
		team.lastIndex = null;
		enemyTeam.highestAttack.Clear();
		enemyTeam.lowestAttack.Clear();
		enemyTeam.highestHealth.Clear();
		enemyTeam.lowestHealth.Clear();
		enemyTeam.lastIndex = null;
		foreach(int i in GD.Range(Game.teamSize))
		{
			Pet teamPet = team.GetPetAt(i);
			if(teamPet != null)
			{
				// if the list is empty or if the attack is equal
				if(team.highestAttack.Count==0 || teamPet.currentAttack==team.highestAttack[0].currentAttack)
				{
					team.highestAttack.Add(teamPet);
				}
				else if(teamPet.currentAttack>team.highestAttack[0].currentAttack)
				{
					team.highestAttack.Clear();
					team.highestAttack.Add(teamPet);
				}
				if(team.highestHealth.Count==0 || teamPet.currentHealth==team.highestHealth[0].currentHealth)
				{
					team.highestHealth.Add(teamPet);
				}
				else if(teamPet.currentHealth>team.highestHealth[0].currentHealth)
				{
					team.highestHealth.Clear();
					team.highestHealth.Add(teamPet);
				}
				if(team.lowestAttack.Count==0 || teamPet.currentAttack==team.lowestAttack[0].currentAttack)
				{
					team.lowestAttack.Add(teamPet);
				}
				else if(teamPet.currentAttack<team.lowestAttack[0].currentAttack)
				{
					team.lowestAttack.Clear();
					team.lowestAttack.Add(teamPet);
				}
				if(team.lowestHealth.Count==0 || teamPet.currentHealth==team.lowestHealth[0].currentHealth)
				{
					team.lowestHealth.Add(teamPet);
				}
				else if(teamPet.currentHealth<team.lowestHealth[0].currentHealth)
				{
					team.lowestHealth.Clear();
					team.lowestHealth.Add(teamPet);
				}
				if(team.lastIndex == null || teamPet.index>team.lastIndex.index)
				{
					team.lastIndex = teamPet;
				}
			}
			Pet enemyPet = enemyTeam.GetPetAt(i);
			if(enemyPet != null)
			{
				if(enemyTeam.highestAttack.Count==0 || enemyPet.currentAttack==enemyTeam.highestAttack[0].currentAttack)
				{
					enemyTeam.highestAttack.Add(enemyPet);
				}
				else if(enemyPet.currentAttack>enemyTeam.highestAttack[0].currentAttack)
				{
					enemyTeam.highestAttack.Clear();
					enemyTeam.highestAttack.Add(enemyPet);
				}
				if(enemyTeam.highestHealth.Count==0 || enemyPet.currentHealth==enemyTeam.highestHealth[0].currentHealth)
				{
					enemyTeam.highestHealth.Add(enemyPet);
				}
				else if(enemyPet.currentHealth>enemyTeam.highestHealth[0].currentHealth)
				{
					enemyTeam.highestHealth.Clear();
					enemyTeam.highestHealth.Add(enemyPet);
				}
				if(enemyTeam.lowestAttack.Count==0 || enemyPet.currentAttack==enemyTeam.lowestAttack[0].currentAttack)
				{
					enemyTeam.lowestAttack.Add(enemyPet);
				}
				else if(enemyPet.currentAttack<enemyTeam.lowestAttack[0].currentAttack)
				{
					enemyTeam.lowestAttack.Clear();
					enemyTeam.lowestAttack.Add(enemyPet);
				}
				if(enemyTeam.lowestHealth.Count==0 || enemyPet.currentHealth==enemyTeam.lowestHealth[0].currentHealth)
				{
					enemyTeam.lowestHealth.Add(enemyPet);
				}
				else if(enemyPet.currentHealth<enemyTeam.lowestHealth[0].currentHealth)
				{
					enemyTeam.lowestHealth.Clear();
					enemyTeam.lowestHealth.Add(enemyPet);
				}
				if(enemyTeam.lastIndex == null || enemyPet.index>enemyTeam.lastIndex.index)
				{
					enemyTeam.lastIndex = enemyPet;
				}
			}
		}
		foreach(int i in GD.Range(Game.teamSize))
		{
			Pet teamPet = team.GetPetAt(i);
			if(teamPet != null)
			{
				teamPet.enemyTeam = enemyTeam;
				teamPet.team = team;
			}
			Pet enemyPet = enemyTeam.GetPetAt(i);
			if(enemyPet != null)
			{
				enemyPet.enemyTeam = team;
				enemyPet.team = enemyTeam;
			}
		}
	}

	public static PetAnimation GetPetAnimator(Node2D slot)
	{
		return (PetAnimation)slot.GetChild(5);
	}

	public async Task WaitForTasks(params Task[] tasks)
    {
		mouseDisabled = true;

		await Task.WhenAll(tasks);

		mouseDisabled = false;
    }

	public async Task WaitForFuncTasks(params Func<Task>[] taskFuncs)
    {
		mouseDisabled = true;

		var tasks = taskFuncs.Select(func => func()).ToArray();
		await Task.WhenAll(tasks);

		mouseDisabled = false;
    }

	public static bool isSamePet(Pet pet1, Pet pet2)
	{
		GD.Print(pet1.name);
		GD.Print(pet2.name);
		GD.Print(pet1.petAbility);
		GD.Print(pet1.petAbility?.evolution);
		GD.Print(pet1.petAbility?.evolution?.name);
		GD.Print(pet1.petAbility?.evolution?.evolution?.name);

		bool secondIsEvo = pet1.name == pet2.name || 
           pet1.petAbility?.evolution?.name == pet2.name || 
           pet1.petAbility?.evolution?.evolution?.name == pet2.name;

		bool firstIsEvo = pet2.name == pet1.name || 
           pet2.petAbility?.evolution?.name == pet1.name || 
           pet2.petAbility?.evolution?.evolution?.name == pet1.name;

		GD.Print("pet 1 " + pet1.name);
		GD.Print("pet 2 " + pet2.name);
		GD.Print("second is evo: " + secondIsEvo);
		GD.Print("first is evo: " + firstIsEvo);
    	return firstIsEvo || secondIsEvo;
	}

	public static bool isSamePetNidorans(Pet pet1, Pet pet2)
	{
		GD.Print("nidorans");
		GD.Print(isSamePet(new Pet(new NidoranfAbility()), pet1));
		GD.Print(isSamePet(pet2, new Pet(new NidoranmAbility())));
		GD.Print(isSamePet(new Pet(new NidoranfAbility()), pet2));
		GD.Print(isSamePet(pet1, new Pet(new NidoranmAbility())));
		return (isSamePet(new Pet(new NidoranfAbility()), pet1) && isSamePet(pet2, new Pet(new NidoranmAbility()))) || (isSamePet(new Pet(new NidoranfAbility()), pet2) && isSamePet(pet1, new Pet(new NidoranmAbility())));

	}
}
