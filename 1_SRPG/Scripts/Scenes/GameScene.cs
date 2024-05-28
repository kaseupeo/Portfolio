using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameScene : MonoBehaviour
{
    private Tilemap _startTile;
    
    private void Awake()
    {
        Managers.Game.Init();
        Managers.Map.Init();
    }
}
