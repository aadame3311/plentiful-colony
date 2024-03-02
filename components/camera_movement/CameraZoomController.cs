using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class CameraZoomController : Node
{
	[Required]
	private Camera2D _camera;

	[Export]
	public float _zoomSpeed = (float)0.16;

	[Export]
	public float _zoomOutLimit = (float)1.5;

	[Export]
	public float _zoomInLimit = (float)4;

	[Export]
	public float _initialZoom = (float)2;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetParent<Camera2D>();
		_camera.Zoom = new Vector2(_initialZoom, _initialZoom);

		if (_camera == null)
		{
			GD.PushError("Camera 2D is required as a parent node");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 newZoom = _camera.Zoom;

		if (Input.IsActionJustPressed("camera_zoom_in"))
		{
			newZoom += new Vector2(_zoomSpeed, _zoomSpeed);
			if (newZoom < new Vector2(_zoomInLimit, _zoomInLimit))
			{
				_camera.Zoom = newZoom;
			}
		}

		if (Input.IsActionJustPressed("camera_zoom_out"))
		{

			newZoom -= new Vector2(_zoomSpeed, _zoomSpeed);

			if (newZoom > new Vector2(_zoomOutLimit, _zoomOutLimit))
			{
				_camera.Zoom = newZoom;
			}
		}
	}
}
