using Godot;
using System;

public partial class GameOver : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_reset_pressed(){
		// Reiniciar la escena actual
		 GetTree().ChangeSceneToFile("res://scenes/board.tscn"); 
	}

	public void _on_exit_pressed(){
		// Salir del juego
		GetTree().Quit();
	}
}
