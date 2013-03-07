using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Dungeon
{
	public List<Map> MapList {get;set;}
	public Map CurrentMap {get;set;}
	public Map TransitionMap {get;set;}
	public int MapWidth {get;set;}
	public int MapHeight {get;set;}
	
	private int _maxRooms;
	private int _currentAmtOfRooms;
	private int _currentRoomIndex;
	
	//*** NUMBER OF ROOM TEMPLATES
	private int _maxRoomTemplates = 3;

	
	public Dungeon (int maxRooms)
	{
		MapList = new List<Map>();
		
		_maxRooms = maxRooms;
		_currentAmtOfRooms = 0;
		_currentRoomIndex = 0;
		Generate();
	}
	
	private void Generate()
	{
		// get random map
		int mapNumber = XeldaGame.rand.Next(1, _maxRoomTemplates + 1);
		
		// create initial map point
		Map startMap = new Map("Maps/xelda_map"+mapNumber);
		startMap.DebugMapPosition = new Vector2(0,0);
		MapList.Add(startMap);
		_currentAmtOfRooms++;
		
		// set some properties
		// tile size is 24x24.  We need to adjust map width and height so we don't get the extra space around the map that holds
		// the collisions for the passages.
		int tileSize = 32;
		MapWidth = startMap.GetMapWidth() - (2 * tileSize);
		MapHeight = startMap.GetMapHeight() - (2 * tileSize);
		
		while (true)
		{
			CurrentMap = MapList[_currentRoomIndex];
			// randomly choose amount of directions to branch depending on which
			// is less: max rooms left to place or connections possible for map
			int maxPossibleConnections = Math.Min(_maxRooms - _currentAmtOfRooms, CurrentMap.GetPossibleConnectionCount());
			
			// generated max amount of rooms
			if (maxPossibleConnections == 0) break;
			
			int todoConnections = XeldaGame.rand.Next(1, maxPossibleConnections+1);
			// for each needed room, generate and connect the maps
			for (int i = 0; i < todoConnections; i++)
			{
				Direction dir = CurrentMap.GetRandomDirectionForConnection();
				mapNumber = XeldaGame.rand.Next(1, _maxRoomTemplates + 1);
				Map map = new Map("Maps/xelda_map"+mapNumber, dir, MapList.IndexOf(CurrentMap));
				SetDebugMapPosition(CurrentMap, map, dir);
				MapList.Add(map);
				_currentAmtOfRooms++;
				CurrentMap.ConnectThisToNewMap(dir, MapList.IndexOf(map));
			}
			
			// treat list like queue and go to next node and build off of that.
			_currentRoomIndex++;
		}
		
		// set CurrentMap back to 0 so player starts at first node.
		CurrentMap = MapList[0];
		
		// tell each map to add walls to block passages that have no adjacent map to connect
		foreach(Map map in MapList) map.AddPassageWalls();
		
		// make sure everything is connected here via debug
		int a = 0;
		foreach(Map map in MapList)
		{
			Debug.Log("pos: " + a + " n:" +map.connected_N + " s:" + map.connected_S + " w:" + map.connected_W + " e:" + map.connected_E);
			a++;
		}
	}
	
	// debug helping method that sets x,y point of map
	private void SetDebugMapPosition(Map parentMap, Map newMap, Direction dir)
	{
		switch(dir)
		{
		case Direction.N:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x, parentMap.DebugMapPosition.y + 1);
			break;
		case Direction.S:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x, parentMap.DebugMapPosition.y - 1);
			break;
		case Direction.W:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x - 1, parentMap.DebugMapPosition.y);
			break;
		case Direction.E:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x + 1, parentMap.DebugMapPosition.y);
			break;
		}
	}
	
	public void PassageToConnectedMap(string direction)
	{
		switch(direction)
		{
		case "NORTH":
			if (CurrentMap.connected_N != -1) TransitionMap = MapList[CurrentMap.connected_N];
			break;
		case "SOUTH":
			if (CurrentMap.connected_S != -1) TransitionMap = MapList[CurrentMap.connected_S];
			break;
		case "WEST":
			if (CurrentMap.connected_W != -1) TransitionMap = MapList[CurrentMap.connected_W];
			break;
		case "EAST":
			if (CurrentMap.connected_E != -1) TransitionMap = MapList[CurrentMap.connected_E];
			break;
		}
	}
	
	public void ChangeTransitionToCurrentMap()
	{
		CurrentMap = TransitionMap;
		// ret position since transitioning changed these values
		CurrentMap.x = 0;
		CurrentMap.y = 0;
	}
}