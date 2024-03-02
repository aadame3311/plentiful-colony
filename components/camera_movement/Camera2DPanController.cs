using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class Camera2DPanController : Node
{
	[Required]
	private Camera _camera;

	[Export]
	public float cameraPanSpeed = (float)1;

	private Viewport _viewport;

	private Vector2 _mousePos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetParent<Camera>();
		_viewport = GetViewport();

		if (_camera == null)
		{
			GD.PushError("Camera 2D is required as a parent node");
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.IsActionPressed("camera_pan"))
		{
			Vector2 newPositionDelta = (@event as InputEventMouseMotion).Relative * cameraPanSpeed / _camera.Zoom;

			Vector2 newPosition = _camera.Position - newPositionDelta;

			// Camera2D limits don't clamp camera position, must manually clamp...
			_camera.Position = new Vector2(
				Mathf.Clamp(newPosition.X, _camera.LimitLeft + _camera.GetViewportRect().Size.X / (2 * _camera.Zoom.X), _camera.LimitRight - _camera.GetViewportRect().Size.X / (2 * _camera.Zoom.X)),
				Mathf.Clamp(newPosition.Y, _camera.LimitTop + _camera.GetViewportRect().Size.Y / (2 * _camera.Zoom.Y), _camera.LimitBottom - _camera.GetViewportRect().Size.Y / (2 * _camera.Zoom.Y))
			);
		}
	}
}
