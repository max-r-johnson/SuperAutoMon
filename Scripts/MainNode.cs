using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public partial class MainNode : Node
{
	public static Game game {get;set;}
	private Shop shop {get {return game.shop;}}
	private Player player {get {return game.player;}}
	private Pack pack {get {return game.pack;}}
	private Team team {get {return Game.team;}}
	private List<Node2D> shopSlots {get {return shop.shopSlots;}}
	private List<Node2D> teamSlots {get {return team.teamSlots;}}
	private List<Node2D> foodSlots {get {return shop.foodSlots;}}
	private int round {get;set;}
	private int tier {get;set;}
	public Label moneyLabel {get {return (Label)GetNode("Money");}}
	public const int buttonYVal = 550;
	public const int rollXVal = 49;
	public const int sellXVal = 625;
	public const int storeXVal = 325;
	public const int endXVal = 901;
	public const int panelBorderWidth = 4;
	public const int panelCornerRadius = 7;
	public const string panelColor = "6d3945";
	public const string panelBorderColor = "000000";
	public const int buttonWidth = 200;
	public const int buttonHeight = 80;
	public const int buttonFontSize =  40;
	public string instanceBuilder()
	{
		return "";
	}

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		//doesn't work on second round because indexing is fricked up i think
		game = MenuNode.game;
		game.mainNode = this;
		game.battleQueue.Clear();
		shopSlots.Clear();
		teamSlots.Clear();
		foodSlots.Clear();
		await game.WaitForTasks(game.StartTurn());
		createButton(RollButton,rollXVal, "Roll");
		createButton(SellButton,sellXVal, "Sell");
		createButton(StoreButton,storeXVal, "Store");
		createButton(EndButton,endXVal,"End Turn");
		updateMoneyLabel();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// sets first pet on team to the first pet in the shop
		// if (Input.IsActionJustPressed("R_click"))
		// {
		// 	team.AddPet(shop.shopPets[0], 0);
		// }
	}

	public void createButton(Action function, int buttonX, string text)
	{
		Panel panel = new Panel();
		panel.Position = new Godot.Vector2(buttonX,buttonYVal);
		panel.Size = new Godot.Vector2(buttonWidth,buttonHeight);
		StyleBoxFlat styleBox = new StyleBoxFlat();

		styleBox.SetBorderWidthAll(panelBorderWidth);
		styleBox.SetCornerRadiusAll(panelCornerRadius);
		styleBox.BorderColor = new Godot.Color(panelBorderColor);
		styleBox.BgColor = new Godot.Color(panelColor);
		// styleBox.ResourcePath =
		panel.Name = text;

		panel.AddThemeStyleboxOverride("panel", styleBox);
		
		Button button = new Button();
		button.Name = text;
		button.Text = text;
		button.Flat = true;
		button.AddThemeFontSizeOverride("font_size", buttonFontSize);
		button.Size = new Godot.Vector2(buttonWidth,buttonHeight);
		var font = (Font)GD.Load("res://Font//LapsusPro-Bold.otf");
		button.AddThemeFontOverride("font",font);
		button.Pressed += () =>
		{
			if (!game.mouseDisabled)
			{
				function();
			}
		};
		if(text=="Sell" || text=="Store")
		{
			panel.Hide();
		}
		AddChild(panel);
		panel.AddChild(button);
	}

	private void RollButton()
	{
		GetNode<Panel>("Store").Hide();
		shop.roll();
	}

	private async void SellButton()
	{
		await shop.sellPet(team.selectedPet.index);
		GetNode<Panel>("Sell").Hide();
		team.selectedPet = null;
	}

	private void StoreButton()
	{
		//need to change icon/text of button when stored for unstore
		if(shop.selectedPet != null)
		{
			if(shop.selectedPet.stored)
			{
				shop.selectedPet.stored = false;
				game.changeStorePetTexture(shop.shopSlots[shop.selectedPetIndex], "destored");
			}
			else
			{
				shop.selectedPet.stored = true;
				game.changeStorePetTexture(shop.shopSlots[shop.selectedPetIndex], "stored");
			}
			shop.selectedPet = null;
			GetNode<Panel>("Store").Hide();
		}
		if(shop.selectedFood != null)
		{
			if(shop.selectedFood.stored)
			{
				shop.selectedFood.stored = false;
				game.changeStoreFoodTexture(shop.foodSlots[shop.selectedFoodIndex], "destored");
			}
			else
			{
				shop.selectedFood.stored = true;
				game.changeStoreFoodTexture(shop.foodSlots[shop.selectedFoodIndex], "stored");
			}
			shop.selectedFood = null;
			GetNode<Panel>("Store").Hide();
		}
	}

	private async void EndButton()
	{
		await game.WaitForTasks(game.EndTurn());
		GetTree().ChangeSceneToFile("res://Battle.tscn");
	}

	public void updateMoneyLabel()
	{
		moneyLabel.Text = "Money: " + player.money;
	}
}
