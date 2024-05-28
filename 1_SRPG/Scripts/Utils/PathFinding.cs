using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO : 길찾기 제네릭으로 할지 아니면 그냥 쓸지 고민하고 수정하기
public class PathFinding
{
    private static Dictionary<Vector2Int, Tile> _searchableTiles;
    
    // 길찾기
    public static List<Tile> FindPath(Tile start, Tile end, List<Tile> inRangeTiles)
    {
         _searchableTiles = new Dictionary<Vector2Int, Tile>();
        
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        if (inRangeTiles.Count > 0)
        {
            foreach (Tile tile in inRangeTiles)
                _searchableTiles.Add(tile.Grid2DLocation, Managers.Map.MapTiles[tile.Grid2DLocation]);
        }
        else
        {
            _searchableTiles = Managers.Map.MapTiles;
        }
        
        openList.Add(start);

        while (openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
                return GetFinishedList(start, end);
            
            foreach (Tile tile in GetSurroundingTiles(currentTile.Grid2DLocation, _searchableTiles))
            {
                if (closedList.Contains(tile) || IsCheckToPassTile(currentTile, tile, Define.Height))
                    continue;
                
                tile.G = GetManhattanDistance(start, tile);
                tile.H = GetManhattanDistance(end, tile);

                tile.Previous = currentTile;
                
                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }
            }
        }

        return new List<Tile>();
    }

    // 시작 타일에서 도착 타일을 갈 수 있는지 확인하는 
    public static bool IsCheckToPassTile(Tile startTile, Tile endTile, int height = 1)
    {
        if (Managers.Game.SelectedCharacter != null && Managers.Game.SelectedCharacter.State == Define.State.Attack)
            return false;

        if (Managers.Game.Monster != null && Managers.Game.Monster.State == Define.State.Attack)
            return false;

        return Mathf.Abs(startTile.transform.position.z - endTile.transform.position.z) > height ||
               endTile.IsBlocked;
    }
    
    // 범위 내 타일들 찾기
    // public static List<Tile> GetTilesInRange(Vector2Int location, int range)
    // {
    //     Tile startTile = Managers.Map.MapTiles[location];
    //     List<Tile> inRangeTile = new List<Tile>();
    //     List<Tile> tilesForPreviousStep = new List<Tile>();
    //     int stepCount = 0;
    //
    //     // if (range != 0) 
    //     //     inRangeTile.Add(startTile);
    //     
    //     tilesForPreviousStep.Add(startTile);
    //
    //     while (stepCount < range)
    //     {
    //         List<Tile> surroundingTiles = new List<Tile>();
    //         
    //         foreach (Tile tile in tilesForPreviousStep)
    //             surroundingTiles.AddRange(GetSurroundingTiles(new Vector2Int(tile.GridLocation.x, tile.GridLocation.y), Managers.Map.MapTiles));
    //
    //         inRangeTile.AddRange(surroundingTiles);
    //         tilesForPreviousStep = surroundingTiles.Distinct().ToList();
    //         stepCount++;
    //     }
    //
    //     return inRangeTile.Distinct().ToList();
    // }
    
    // 해당 타일에서 갈 수 있는 타일 리스트
    public static List<Tile> GetSurroundingTiles(Vector2Int originTile, Dictionary<Vector2Int, Tile> mapTiles)
    {
        List<Tile> surroundingTiles = new List<Tile>();
        
        Vector2Int tileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
        if (mapTiles.ContainsKey(tileToCheck) && !IsCheckToPassTile(mapTiles[originTile], mapTiles[tileToCheck], Define.Height))
            surroundingTiles.Add(mapTiles[tileToCheck]);

        tileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
        if (mapTiles.ContainsKey(tileToCheck) && !IsCheckToPassTile(mapTiles[originTile], mapTiles[tileToCheck], Define.Height))
            surroundingTiles.Add(mapTiles[tileToCheck]);

        tileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
        if (mapTiles.ContainsKey(tileToCheck) && !IsCheckToPassTile(mapTiles[originTile], mapTiles[tileToCheck], Define.Height))
            surroundingTiles.Add(mapTiles[tileToCheck]);

        tileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
        if (mapTiles.ContainsKey(tileToCheck) && !IsCheckToPassTile(mapTiles[originTile], mapTiles[tileToCheck], Define.Height))
            surroundingTiles.Add(mapTiles[tileToCheck]);
        
        return surroundingTiles;
    }

    public static List<Tile> GetAttackSurroundingTiles(Vector2Int originTile, Dictionary<Vector2Int, Tile> mapTiles, int range)
    {
        List<Tile> surroundingTiles = new List<Tile>();

        for (int y = -range; y <= range; y++)
        {
            for (int x = -range; x <= range; x++)
            {
                Vector2Int tileToCheck = new Vector2Int(originTile.x + x, originTile.y + y);
                if (mapTiles.TryGetValue(tileToCheck, out var tile))
                    surroundingTiles.Add(tile);
            }
        }
        
        // Vector2Int tileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
        // if (mapTiles.TryGetValue(tileToCheck, out var tile))
        //     surroundingTiles.Add(tile);
        //
        // tileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
        // if (mapTiles.TryGetValue(tileToCheck, out tile))
        //     surroundingTiles.Add(tile);
        //
        // tileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
        // if (mapTiles.TryGetValue(tileToCheck, out tile))
        //     surroundingTiles.Add(mapTiles[tileToCheck]);
        //
        // tileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
        // if (mapTiles.TryGetValue(tileToCheck, out tile))
        //     surroundingTiles.Add(mapTiles[tileToCheck]);
        
        return surroundingTiles;
    }
    
    private static List<Tile> GetFinishedList(Tile start, Tile end)
    {
        List<Tile> finishedList = new List<Tile>();
        Tile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.Previous;
        }

        finishedList.Reverse();

        return finishedList;
    }

    // ASTAR 방법 (맨해튼거리법)
    private static int GetManhattanDistance(Tile start, Tile tile)
    {
        return Mathf.Abs(start.GridLocation.x - tile.GridLocation.x) +
               Mathf.Abs(start.GridLocation.y - tile.GridLocation.y);
    }
}