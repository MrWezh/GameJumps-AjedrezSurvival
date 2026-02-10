using Godot;
using Godot.Collections;

public partial class Enemies : Node2D
{
    private PackedScene peon = GD.Load<PackedScene>("res://Scenes/Enemies/peon.tscn");
    private PackedScene alfil = GD.Load<PackedScene>("res://Scenes/Enemies/alfil.tscn");
    private PackedScene caballo = GD.Load<PackedScene>("res://Scenes/Enemies/caballo.tscn");
    private PackedScene torre = GD.Load<PackedScene>("res://Scenes/Enemies/torre.tscn");
    private PackedScene reyNegro = GD.Load<PackedScene>("res://Scenes/Enemies/rey.tscn");
    private PackedScene reinaNegra = GD.Load<PackedScene>("res://Scenes/Enemies/reina.tscn");

    public PackedScene _peon => peon;
    public PackedScene _alfil => alfil;
    public PackedScene _caballo => caballo;
    public PackedScene _torre => torre;
    public PackedScene _reynegro => reyNegro;
    public PackedScene _reinaNegra => reinaNegra;


}