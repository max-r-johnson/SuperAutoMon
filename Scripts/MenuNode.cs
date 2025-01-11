using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection;

public partial class MenuNode : Node
{
	public static Game game {get;set;}
	// private Shop shop {get {return game.shop;}}
	// private Player player {get {return game.player;}}
	// private Pack pack {get {return game.pack;}}
	// private Team team {get {return game.team;}}
	// private List<Node2D> shopSlots {get {return game.shopSlots;}}
	// private List<Node2D> team.teamSlots {get {return game.team.teamSlots;}}
	// private List<Node2D> foodSlots {get {return game.foodSlots;}}
	private Button newGame {get {return (Button)GetChildren()[0].GetChildren()[0];}}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		newGame.Pressed += NewGameButton;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void NewGameButton()
	{
		game = new Game();
		GetTree().ChangeSceneToFile("res://Main Node.tscn");
	}
}
