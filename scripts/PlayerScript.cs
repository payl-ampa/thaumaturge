using Godot;
using System;

public partial class PlayerScript : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	[Export]
	public float HorizontalMouseSensitivity = 0.1f;
	[Export]
	public float VerticalMouseSensitivity = 0.1f;

	private Node3D _camera_mount;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera_mount = GetNode<Node3D>("CameraMount");
    }

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseMotion mouseMotion)
		{
			RotateY(-Mathf.DegToRad(mouseMotion.Relative.X) * HorizontalMouseSensitivity);
			_camera_mount.RotateX(-Mathf.DegToRad(mouseMotion.Relative.Y) * VerticalMouseSensitivity);
		}

		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				GetTree().Quit();
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
