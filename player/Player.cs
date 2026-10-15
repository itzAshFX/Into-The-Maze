using Godot;

namespace Intothemaze.player;

public partial class Player : CharacterBody3D
{
	private bool _mouseGrab = true;
	public const float Speed = 5.0f;
	public static float currentSpeed = Speed;
	public const float JumpVelocity = 4f;
	public const float MouseSense = 0.003f;
	public const float Sprint = 2.5f;
	public const float stamina = 100;
	[Export] public Node3D Head;
	[Export] public Camera3D Camera;

	public override void _Process(double delta)
	{
		Input.SetMouseMode(_mouseGrab ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible);
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			_mouseGrab = !_mouseGrab;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		if (Input.IsActionPressed("Sprint"))
		{
			currentSpeed += Sprint;
		}
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
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

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (!_mouseGrab) return;
			var relative = mouseMotion.Relative;
			Head.RotateY(-relative.X * MouseSense);
			Camera.RotateX(-relative.Y * MouseSense);
			Camera.Rotation = Camera.Rotation with
			{
				X = Mathf.Clamp(Camera.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89))
			};
		}
	}
}
