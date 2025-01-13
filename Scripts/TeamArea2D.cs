using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

public partial class TeamArea2D : Area2D
{

	[Export]
	public bool isInside;
	//when selectedPet is updated, updates this with its name for easy test/visualization
	public Game game;
	public Shop shop {get {return game.shop;}}
	public Team team {get {return Game.team;}}
	public Team enemyTeam {get {return Game.enemyTeam;}}
	[Export]
	public int slotIndex;
	public Player player;
	
	public override async void _Input(InputEvent @event)
    {
    	if (@event is InputEventMouseButton inputEventMouse)
    	{
    		if (inputEventMouse.Pressed && inputEventMouse.ButtonIndex == MouseButton.Left && isInside == true && !game.mouseDisabled)
			{
				if(game.inBattle!= true)
				{
					//if clicking a slot without a pet in it when you have a shop pet selected
					if(shop.selectedPet != null && team.GetPetAt(slotIndex) == null)
					{
						buyPet();
					}
					//if clicking a slot with a pet in it when you have a shop pet selected
					else if(shop.selectedPet != null && team.GetPetAt(slotIndex) != null)
					{
						//if its the same pet
						if(Game.isSamePet(team.GetPetAt(slotIndex),shop.selectedPet) && team.GetPetAt(slotIndex).experience < team.GetPetAt(slotIndex).maxExp)
						{
							buyPet();
						}
						//if different pet (move the pets)
						else
						{
							team.selectedPet = team.GetPetAt(slotIndex);
							// game.mouseDisabled = true;
							if(await arrangePets())
							{
								shop.selectedPet = shop.shopPets[shop.selectedPetIndex];
								buyPet();
							}
							else
							{
								//if still haven't found space, there is no space, don't buy. For now, having it unselect the pet so it's more clear/smoother
								shop.selectedPet = null;
								GetParent().GetNode<Panel>("Store").Hide();
							}
							// game.mouseDisabled = false;
							team.selectedPet = null;
						}
					}
					//if clicking a slot with a pet in it when you have no shop pets selected
					else if(shop.selectedPet == null && team.GetPetAt(slotIndex)!=null)
					{
						//if a food is selected
						if(shop.selectedFood != null)
						{
							await shop.buyFood(shop.selectedFood,slotIndex);
							GetParent().GetNode<Panel>("Store").Hide();
						}
						//if no team pet selected, selects a team pet
						else if(team.selectedPet == null)
						{
							selectPet();
						}
						//if clicking the slot where a selected team pet is
						else if(team.selectedPet.index == slotIndex)
						{
							unselectPet();
						}
						else if(team.selectedPet != null)
						{
							//if same name (must take stats of higher, experience of higher, held item of not selected pet)

							//need to check if evolution too
							if(Game.isSamePet(team.GetPetAt(slotIndex),team.selectedPet) && team.GetPetAt(slotIndex).experience < team.GetPetAt(slotIndex).maxExp && team.selectedPet.experience < team.selectedPet.maxExp)
							{
								Pet tempPet = new Pet(
									(team.GetPetAt(slotIndex).experience > team.selectedPet.experience)
										? team.GetPetAt(slotIndex).petAbility
										: team.selectedPet.petAbility
								);
								assignTempPetStats(tempPet);
								tempPet.experience = Math.Max(team.selectedPet.experience,team.GetPetAt(slotIndex).experience);
								if(team.GetPetAt(slotIndex).experience == team.GetPetAt(slotIndex).maxExp || tempPet.experience == tempPet.maxExp)
								{
									unselectPet();
									return;
								}
								else
								{
									Pet selectedPet = team.selectedPet;
									Pet combinePet = team.GetPetAt(slotIndex);
									team.RemoveAt(team.selectedPet.index);
									team.RemoveAt(slotIndex);
									team.AddPet(tempPet, slotIndex);
									unselectPet();
									await tempPet.gainExperience(Math.Min(Math.Min(selectedPet.experience,combinePet.experience) + 1, Game.maxExp - tempPet.experience));
								}
							}
							//if different name
							else
							{
								Pet tempPet = team.selectedPet;
								if(Math.Abs(team.selectedPet.index - slotIndex) == 1)
								{
									await team.Swap(slotIndex, team.selectedPet.index);
								}
								else
								{
									team.RemoveAt(team.selectedPet.index);
									await arrangePets();
									team.AddPet(tempPet, slotIndex);
								}
								unselectPet();
							}
						}
					}
					//if clicking a slot without a pet in it when you have no shop pets selected
					else if(shop.selectedPet == null && team.GetPetAt(slotIndex)==null)
					{
						//if there is a team pet selected
						if(team.selectedPet != null)
						{
							team.RemoveAt(team.selectedPet.index);
							team.AddPet(team.selectedPet, slotIndex);
							unselectPet();
						}
					}
					if(team.selectedPet != null){}
					if(shop.selectedPet != null){}
				}
			}
		}		
	}

    public override void _MouseEnter()
    {
		isInside = true;
		if(team.teamSlots.Contains(this))
		{
			if(team.GetPetAt(slotIndex)!=null)
			{
				//maybe add a constant that finds the ID of the description node
				VBoxContainer Description = (VBoxContainer)GetChildren()[4];
				Description.Show();
			}
		}
		else
		{
			if(enemyTeam.GetPetAt(slotIndex)!=null)
			{
				VBoxContainer Description = (VBoxContainer)GetChildren()[4];
				Description.Show();
			}
		}
    }

    public override void _MouseExit()
    {
		isInside = false;
		VBoxContainer Description = (VBoxContainer)GetChildren()[4];
		Description.Hide();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void unselectPet()
	{
		team.selectedPet = null;
		GetParent().GetNode<Panel>("Sell").Hide();
		GetParent().GetNode<Panel>("Store").Hide();
	}
	
	public void selectPet()
	{
		team.selectedPet = team.GetPetAt(slotIndex);
		GetParent().GetNode<Panel>("Sell").Show();
		GetParent().GetNode<Panel>("Store").Hide();
	}

	public async void buyPet()
	{
		await shop.buyPet(shop.selectedPetIndex,slotIndex);
		GetParent().GetNode<Panel>("Store").Hide();
	}

	public async Task<bool> arrangePets()
	{
		foreach(int i in GD.Range(0,Game.teamSize))
		{
			if(slotIndex <= team.selectedPet.index)
			{
				//if space to the left
				if(slotIndex+i < Game.teamSize)
				{
					if(team.GetPetAt(slotIndex + i) == null)
					{
						foreach(int j in GD.Range(slotIndex + i-1,slotIndex-1,-1))
						{
							await team.Swap(j,j+1);
						}
						return true;
					}
				}
				//if space to the right
				if(slotIndex-i >= 0)
				{
					if(team.GetPetAt(slotIndex - i) == null)
					{
						foreach(int j in GD.Range(slotIndex - i,slotIndex))
						{
							await team.Swap(j,j+1);
						}
						return true;
					}
				}
			}
			if(slotIndex > team.selectedPet.index)
			{
				//if space to the right
				if(slotIndex-i >= 0)
				{
					if(team.GetPetAt(slotIndex - i) == null)
					{
						foreach(int j in GD.Range(slotIndex - i,slotIndex))
						{
							await team.Swap(j,j+1);
						}
						return true;
					}
				}
				//if space to the left
				if(slotIndex+i < Game.teamSize)
				{
					if(team.GetPetAt(slotIndex + i) == null)
					{
						foreach(int j in GD.Range(slotIndex + i-1,slotIndex-1,-1))
						{
							await team.Swap(j,j+1);
						}
						return true;
					}
				}
			}
		}
		return false;
	}


	private async void swapToIndex()
	{
		GD.Print(team.selectedPet.index + " to " + slotIndex);
		int lastEmptyIndex=team.selectedPet.index;
		foreach(int i in GD.Range(team.selectedPet.index, slotIndex, Math.Sign(slotIndex-team.selectedPet.index)))
		{
			GD.Print(i);
			if(team.GetPetAt(i)==null)
			{
				lastEmptyIndex=i;
			}
		}
		GD.Print("last empty: " + lastEmptyIndex);
		team.RemoveAt(team.selectedPet.index);
		team.AddPet(team.selectedPet, lastEmptyIndex);
		GD.Print(lastEmptyIndex + " to " + slotIndex);
		foreach(int i in GD.Range(lastEmptyIndex, slotIndex, Math.Sign(slotIndex-lastEmptyIndex)))
		{
			await team.Swap(i,i + Math.Sign(slotIndex-lastEmptyIndex));
		}
	}

	private void assignTempPetStats(Pet tempPet)
	{
		tempPet.index = slotIndex;
		tempPet.team = team;

		tempPet.health = Math.Max(team.GetPetAt(slotIndex).health, team.selectedPet.health);
		tempPet.currentHealth = Math.Max(team.GetPetAt(slotIndex).currentHealth, team.selectedPet.currentHealth);
		tempPet.attack = Math.Max(team.GetPetAt(slotIndex).attack, team.selectedPet.attack);
		tempPet.currentAttack = Math.Max(team.GetPetAt(slotIndex).currentAttack, team.selectedPet.currentAttack);

		tempPet.item = team.GetPetAt(slotIndex).item ?? team.selectedPet.item;
		tempPet.currentItem = team.GetPetAt(slotIndex).currentItem ?? team.selectedPet.currentItem;
	}
}
