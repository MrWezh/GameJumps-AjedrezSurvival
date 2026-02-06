using Godot;

[Tool]
public partial class SpawnInfo : Resource
{
    [Export] public int _turn { get; set; } = 1;
    [Export] public Vector2 Position { get; set; } = Vector2.Zero;
	[Export] public Resource _enemy;
	[Export] public int _enemy_num {get; set;}= 0;
    
}