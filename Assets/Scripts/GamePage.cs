using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct Player
{
	public Rect box;
	public string name;
}

public class GamePage : FContainer
{
	private FSprite _manSprite;
	private FSprite _floorSprite;
	
	private bool _keyUp = false;
	private bool _keyDown = false;
	private bool _keyLeft = false;
	private bool _keyRight = false;
	
	private bool _collideUp = false;
	private bool _collideDown = false;
	private bool _collideLeft = false;
	private bool _collideRight = false;
	
	private Player _player;
	
	private float _moveSpeed = 2f;
	
	//private FNode _cameraTarget = new FNode();
	
	private Dungeon _dungeon;
	
	public GamePage ()
	{
		
		_dungeon = new Dungeon(2);
		
		// images!
		_floorSprite = new FSprite("xelda_map2.png");
		//_floorSprite.anchorX = 0;
		//_floorSprite.anchorY = 0;
		AddChild(_floorSprite);
		
		_manSprite = new FSprite("man.png");
		_manSprite.x = -(_dungeon.CurrentMap.GetMapWidth() / 2);
		_manSprite.y = -(_dungeon.CurrentMap.GetMapHeight() / 2);
		_manSprite.anchorX = 0;
		_manSprite.anchorY = 0;
		AddChild(_manSprite);
		
		//_cameraTarget.x = -200;
		//_cameraTarget.y = -100;
		//AddChild(_cameraTarget);
		
		// *** Stay focused on map
		//Futile.stage.Follow(_cameraTarget,true,false);
		
		_player = new Player();
		_player.box.x = _manSprite.x;
		_player.box.y = _manSprite.y;
		_player.box.width = 24;
		_player.box.height = 24;
		
		// *** debug to find collision boxes
		// showCollisionsWithMen();
	}
	
	private void showCollisionsWithMen()
	{
		foreach(collisionBox box in _dungeon.CurrentMap.collisionBoxList)
		{
			FSprite cb = new FSprite("man.png");
			cb.x = box.box.x;
			cb.y = box.box.y;
			cb.anchorX = 0;
			cb.anchorY = 0;
			AddChild(cb);
		}
	}
	
	override public void HandleAddedToStage()
	{
		Futile.instance.SignalUpdate += HandleUpdate;
		base.HandleAddedToStage();
	}
	
	override public void HandleRemovedFromStage()
	{
		Futile.instance.SignalUpdate -= HandleUpdate;
		base.HandleRemovedFromStage();
	}
	
	void HandleUpdate()
	{
		HandleInputs();
		TestForCollisions();
		HandleMovement();
	}
	
	void HandleInputs()
	{
		if (Input.GetKeyUp(KeyCode.W))
		{
			_keyUp = false;
		}
		if (Input.GetKeyUp(KeyCode.S))
		{
			_keyDown = false;
		}
		if (Input.GetKeyUp(KeyCode.A))
		{
			_keyLeft = false;
		}
		if (Input.GetKeyUp(KeyCode.D))
		{
			_keyRight = false;
		}
	
		if (Input.GetKeyDown(KeyCode.W))
		{
			_keyUp = true;
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			_keyDown = true;
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			_keyLeft = true;
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			_keyRight = true;
		}	
	}
	
	void HandleMovement()
	{
		if (_keyUp && !_collideUp)
		{
			_manSprite.y += _moveSpeed;
		}
		if (_keyDown && !_collideDown)
		{
			_manSprite.y -= _moveSpeed;
		}
		if (_keyLeft && !_collideLeft)
		{
			_manSprite.x -= _moveSpeed;
		}
		if (_keyRight && !_collideRight)
		{
			_manSprite.x += _moveSpeed;
		}
		
		_player.box.x = _manSprite.x;
		_player.box.y = _manSprite.y;
		
		//Debug.Log(_cameraTarget.x + " ct " + _cameraTarget.y);
	}
	
	void TestForCollisions()
	{
		_collideUp = false;
		_collideDown = false;
		_collideLeft = false;
		_collideRight = false;
		
		if (_keyUp)
		{
			Rect collisionRect = _player.box;
			collisionRect.y = collisionRect.y + _moveSpeed;
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					break;
				}
			}
		}
		if (_keyDown)
		{
			Rect collisionRect = _player.box;
			collisionRect.y = collisionRect.y - _moveSpeed;
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideDown = true;
					break;
				}
			}
		}
		if (_keyLeft)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x - _moveSpeed;
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideLeft = true;
					break;
				}
			}
		}
		if (_keyRight)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x + _moveSpeed;
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideRight = true;
					break;
				}
			}
		}
	}
}


