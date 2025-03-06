using Godot;
using System;

[GlobalClass]
public partial class SpawnData : Resource
{
    [Export]
    public Vector2 spawnPosition;

    [Export]
    public bool faceLeft;
}
