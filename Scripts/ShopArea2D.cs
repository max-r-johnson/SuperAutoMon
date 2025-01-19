using Godot;
using System;

public partial class ShopArea2D : Area2D
{
	[Export]
	public bool isInside;
	public Team team {get {return Game.team;}}
	public Game game;
	public Shop shop {get {return game.shop;}}
	[Export]
	public int index;
	public Player player;
	public bool isFood;

	public override void _Input(InputEvent @event)
    {
    	if (@event is InputEventMouseButton inputEventMouse)
    	{
    		if (inputEventMouse.Pressed && inputEventMouse.ButtonIndex == MouseButton.Left && isInside == true && !game.mouseDisabled)
			{
				team.selectedPet = null;
				//DOESNT FULLY WORK BECAUSE DOESn'T UNSELECT FOOD AND PET WHEN CLICKING THE OPPOSITE ONE
				GetParent().GetNode<Panel>("Sell").Hide();
				if(isFood == false)
				{
					shop.selectedFood = null;
					//if you click a slot when nothing is selected
					if(shop.selectedPet == null)
					{
						shop.selectedPet = shop.shopPets[index];
						//if you click a slot with a pet in it when nothing is selected
						if(shop.selectedPet != null)
						{
							GetParent().GetNode<Panel>("Store").Show();
							//GD.Print("Pet " + index + " selected, which is a " + shop.shopPets[index].name);
						}
						shop.selectedPetIndex = index;
					}
					//if you click a slot with a selected pet
					else if(shop.selectedPetIndex == index)
					{
						shop.selectedPet = null;
						GetParent().GetNode<Panel>("Store").Hide();
						//GD.Print("Pet " + index + " unselected, which is a " + shop.shopPets[index].name);
					}
					//if you click an empty slot when have a selected pet
					else
					{
						shop.selectedPet = shop.shopPets[index];
						//if you click a slot with a different pet when one is selected
						if(shop.selectedPet != null)
						{
							GetParent().GetNode<Panel>("Store").Show();
							//GD.Print("Pet " + index + " selected, which is a " + shop.shopPets[index].name);
						}
						else
						{
							GetParent().GetNode<Panel>("Store").Hide();
						}
						shop.selectedPetIndex = index;
					}
				}
				//if food
				else
				{
					shop.selectedPet = null;
					//if you click a slot with nothing selected
					if(shop.selectedFood == null)
					{
						shop.selectedFood = shop.shopFood[index];
						//if you click a slot with food in it when nothing is selected
						if(shop.selectedFood != null)
						{
							GetParent().GetNode<Panel>("Store").Show();
							//GD.Print("Food " + index + " selected, which is a " + shop.shopFood[index].name);
						}
						shop.selectedFoodIndex = index;
					}
					//if you click a food slot when that food is already selected
					else if(shop.selectedFoodIndex == index)
					{
						shop.selectedFood = null;
						GetParent().GetNode<Panel>("Store").Hide();
						//GD.Print("Food " + index + " unselected, which is a " + shop.shopFood[index].name);
					}
					//if you click an empty slot when have a selected food
					else
					{
						shop.selectedFood = shop.shopFood[index];
						//if you click a slot with a different food when one is selected
						if(shop.selectedFood != null)
						{
							GetParent().GetNode<Panel>("Store").Show();
							//GD.Print("Food " + index + " selected, which is a " + shop.shopFood[index].name);
						}
						else
						{
							GetParent().GetNode<Panel>("Store").Hide();
						}
						shop.selectedFoodIndex = index;
					}
				}
			}
			else if (inputEventMouse.Pressed && inputEventMouse.ButtonIndex == MouseButton.Right && isInside == true && !game.mouseDisabled)
			{
				shop.selectedPet = null;
				shop.selectedFood = null;
				if(isFood == false)
				{
					shop.selectedPet = shop.shopPets[index];
					shop.selectedPetIndex = index;
				}
				else
				{
					shop.selectedFood = shop.shopFood[index];
					shop.selectedFoodIndex = index;
				}
				game.mainNode.Store();
			}
		}
	}

    public override void _MouseEnter()
    {
		isInside = true;
		if(isFood == false && shop.shopPets[index]!=null)
		{
			VBoxContainer Description = (VBoxContainer)GetChildren()[4];
			Description.Show();
		}
		else if(isFood == true && shop.shopFood[index]!=null)
		{
			VBoxContainer Description = (VBoxContainer)GetChildren()[4];
			Description.Show();
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
}
