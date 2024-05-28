using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager
{
    private GameObject _tilePrefab;

    private List<GameObject> _loadMaps;
    
    private Dictionary<Vector2Int, Tile> _mapTiles = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, Tile> _startMapTiles = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, Tile> _updateMapTiles = new Dictionary<Vector2Int, Tile>();
    
    public List<GameObject> LoadMaps => _loadMaps;
    public Dictionary<Vector2Int, Tile> MapTiles => _mapTiles;
    public Dictionary<Vector2Int, Tile> UpdateMapTiles
    {
        get
        {
            _updateMapTiles.Clear();
            
            foreach (var tile in _mapTiles.Values.Where(tile => !tile.IsBlocked))
            {
                _updateMapTiles.Add(tile.Grid2DLocation, tile);
            }
            
            return _updateMapTiles;
        }
    }

    public void Init()
    {
        LoadMapPrefabs();
    }

    private void LoadMapPrefabs()
    {
        _loadMaps = new List<GameObject>();

        foreach (GameObject map in Resources.LoadAll("Prefabs/Map"))
        {
            _loadMaps.Add(map);
        }
    }

    public void GenerateOverlayTile(GameObject go)
    {
        Tilemap[] tilemaps = GameObject.Instantiate(go).GetComponentsInChildren<Tilemap>();
        
        GenerateOverlayTile(tilemaps);
    }
    
    private void GenerateOverlayTile(Tilemap[] tileMaps)
    {
        _tilePrefab = Resources.Load("Prefabs/UI/Tile") as GameObject;
        
        foreach (Tilemap tile in tileMaps)
        {
            GenerateOverlayTile(tile);
        }
    }
    
    // 움직일수 있는 타일 생성
    private void GenerateOverlayTile(Tilemap tileMap)
    {
        BoundsInt bounds = tileMap.cellBounds;
        
        // 모든 타일 맵 읽기 + 오버레이 타일 그리기
        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    Vector3Int tileLocation = new Vector3Int(x, y, z);
                    Vector2Int tileKey = new Vector2Int(x, y);
                    
                    if (tileMap.HasTile(tileLocation) && !_mapTiles.ContainsKey(tileKey))
                    {
                        GameObject overlayTile = GameObject.Instantiate(_tilePrefab, tileMap.transform);
                        Vector3 cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder =
                            tileMap.GetComponent<TilemapRenderer>().sortingOrder/* + 1*/;
                        overlayTile.transform.position += new Vector3(0, 0, 2);
                        overlayTile.GetComponent<Tile>().GridLocation = new Vector3Int(x, y, z);
                        overlayTile.name = $"{overlayTile.GetComponent<Tile>().Grid2DLocation}";

                        _mapTiles.Add(tileKey, overlayTile.GetComponent<Tile>());
                    }
                }
            }
        }
    }

    public void ShowTile(List<Tile> rangeTile, Color color)
    {
        if (rangeTile == null)
            return;
        
        foreach (Tile tile in rangeTile)
            tile.ShowTile(color);
    }
    
    public void Clear()
    {
        _mapTiles.Clear();
    }
}
