using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager
{
    private Define.GameMode _gameMode;
    private Define.GameSpeedMode _gameSpeedMode = Define.GameSpeedMode.Normal;
    private float _gameSpeed;
    private Define.CameraMode _cameraMode = Define.CameraMode.Both;
    private bool _pause;
    private bool _isFullScreenMode = true;
    private GameObject _map;
    
    private List<PlayerCharacter> _loadPlayerCharacters;
    private List<PlayerCharacter> _playerCharacters;
    private PlayerCharacter _selectedCharacter;
    private int _maxPlayerCharacter;
    
    private List<Monster> _loadMonsters;
    private List<Monster> _monsters;
    private Monster _monster;
    private int _monsterRate;
    
    private List<Item> _fieldItems;
    private List<Item> _items;
    
    public Define.GameMode GameMode { get => _gameMode; set => _gameMode = value; }
    public Define.GameSpeedMode GameSpeedMode { get => _gameSpeedMode; set => _gameSpeedMode = value; }
    public float GameSpeed
    {
        get
        {
            switch (_gameSpeedMode)
            {
                case Define.GameSpeedMode.Slow:
                    _gameSpeed = 1f;
                    break;
                case Define.GameSpeedMode.Normal:
                    _gameSpeed = 5f;
                    break;
                case Define.GameSpeedMode.Fast:
                    _gameSpeed = 10f;
                    break;
            }
            
            return _gameSpeed;
        }
    }
    public Define.CameraMode CameraMode { get => _cameraMode; set => _cameraMode = value; }
    public bool Pause
    {
        get => _pause;
        set
        {
            _pause = value;
            Time.timeScale = _pause ? 0 : 1;
        }
    }
    public bool IsFullScreenMode
    {
        get => _isFullScreenMode;
        set
        { 
            _isFullScreenMode = value;
            Screen.fullScreen = _isFullScreenMode;
        }
    }
    public GameObject Map { get => _map; set => _map = value; }
    
    public List<PlayerCharacter> LoadPlayerCharacters { get => _loadPlayerCharacters; set => _loadPlayerCharacters = value; }
    public List<PlayerCharacter> PlayerCharacters { get => _playerCharacters; set => _playerCharacters = value; }
    public int MaxPlayerCharacter { get => _maxPlayerCharacter; set => _maxPlayerCharacter = value; }
    
    public PlayerCharacter SelectedCharacter { get => _selectedCharacter; set => _selectedCharacter = value; }
    public List<Monster> LoadMonsters { get => _loadMonsters; set => _loadMonsters = value; }
    public List<Monster> Monsters { get => _monsters; set => _monsters = value; }
    public Monster Monster { get => _monster; set => _monster = value; }
    public int MonsterRate { get => _monsterRate; set => _monsterRate = value; }
    
    public List<Item> FieldItems { get => _fieldItems; set => _fieldItems = value; }
    
    public void Init()
    {
        _gameMode = Define.GameMode.Preparation;
        Pause = false;
        LoadCharacters();
        _playerCharacters = new List<PlayerCharacter>();
        _monsters = new List<Monster>();
        _fieldItems = new List<Item>();
        
        _maxPlayerCharacter = 3;
        _monsterRate = 1;
    }
    
    // 플레이어캐릭터 & 몬스터 프리팹 로드 메소드
    private void LoadCharacters()
    {
        _loadPlayerCharacters = new List<PlayerCharacter>();
        _loadMonsters = new List<Monster>();
        foreach (GameObject pc in Resources.LoadAll("Prefabs/Creatures/PlayerCharacter"))
        {
            _loadPlayerCharacters.Add(pc.GetComponent<PlayerCharacter>());
        }

        foreach (GameObject monster in Resources.LoadAll("Prefabs/Creatures/Monster"))
        {
            _loadMonsters.Add(monster.GetComponent<Monster>());
        }
    }
    
    // 캐릭터 생성 메소드
    public void GeneratePlayerCharacter(Tile tile)
    {
        if (!_loadPlayerCharacters.Contains(_selectedCharacter) || tile == null || tile.IsBlocked)
            return;
        
        PlayerCharacter find = _playerCharacters.Find(x => x.Name == _selectedCharacter.Name);
        if (find != null)
        {
            var gameObject = GameObject.Find(find.Name);
            _playerCharacters.Remove(gameObject.GetComponent<PlayerCharacter>());
            GameObject.Destroy(gameObject);
        }
        
        if (_maxPlayerCharacter <= _playerCharacters.Count)
            return;

        PlayerCharacter pc = GameObject.Instantiate(_selectedCharacter.gameObject).GetComponent<PlayerCharacter>();
        pc.Init();
        pc.CharacterPositionOnTile(tile);
        pc.transform.position += new Vector3(0, 0, 1);
        _playerCharacters.Add(pc);
    }

    // 몬스터 생성 * 몬스터 생성 배율
    public void GenerateMonster()
    {
        for (int i = 0; i < _monsterRate; i++)
        {
            GenerateRandomMonster();
        }
    }
    
    // 몬스터 랜덤 위치에 생성
    private void GenerateRandomMonster()
    {
        List<Tile> tiles =
            new List<Tile>(Managers.Map.UpdateMapTiles.Values.Where(tile =>
                tile.transform.parent.CompareTag("MonsterGround")));
        
        foreach (Monster loadMonster in _loadMonsters)
        {
            Monster monster = GameObject.Instantiate(loadMonster.gameObject).GetComponent<Monster>();
            monster.Init();
            monster.CharacterPositionOnTile(tiles[Random.Range(0, tiles.Count)]);
            _monsters.Add(monster);
        }
    }

    public void ShowStartTile()
    {
        GameObject startGround = GameObject.FindWithTag("StartGround");
        if (startGround == null)
            return;

        foreach (SpriteRenderer spriteRenderer in startGround.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = new Color(0, 0, 1, 0.5f);
        }

        GameObject monsterGround = GameObject.FindWithTag("MonsterGround");
        if (monsterGround == null)
            return;

        foreach (SpriteRenderer spriteRenderer in monsterGround.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
        }
    }
    
    // 최소 ~ 최대 범위 타일 리스트
    public List<Tile> GetRangeTiles(Tile startTile, int maxRange, int minRange = 0)
    {
        if (startTile == null)
            return null;

        List<Tile> rangeFindingTiles = GetTilesInRange(startTile.Grid2DLocation, maxRange);
        List<Tile> dontRangeFindingTiles = GetTilesInRange(startTile.Grid2DLocation, minRange);

        if (rangeFindingTiles == null || dontRangeFindingTiles == null)
            return null;

        foreach (Tile tile in dontRangeFindingTiles)
            rangeFindingTiles.Remove(tile);
        
        rangeFindingTiles.Add(startTile);

        return rangeFindingTiles;
    }

    private List<Tile> GetTilesInRange(Vector2Int location, int range)
    {
        if (!Managers.Map.MapTiles.ContainsKey(location))
            return null;
        
        Tile startTile = Managers.Map.MapTiles[location];
        List<Tile> inRangeTile = new List<Tile>();
        List<Tile> tilesForPreviousStep = new List<Tile>();
        int stepCount = 0;
        
        tilesForPreviousStep.Add(startTile);

        while (stepCount < range)
        {
            List<Tile> surroundingTiles = new List<Tile>();
            
            foreach (Tile tile in tilesForPreviousStep)
                surroundingTiles.AddRange(PathFinding.GetSurroundingTiles(new Vector2Int(tile.GridLocation.x, tile.GridLocation.y), Managers.Map.MapTiles));
            
            inRangeTile.AddRange(surroundingTiles);
            tilesForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }

        return inRangeTile.Distinct().ToList();
    }

    // 1. 몬스터의 공격 범위 만큼 플레이어 중심으로 타일 찾기
    // 찾은 타일들에서 몬스터까지 길찾기해서 가장 잛은 경로 저장
    // -> 몬스터까지 거리가 된다면 그곳으로
    // -> 되지 않는다면 다시 길찾기해서 가장 가까운곳으로
    private List<Tile> MonsterMovement(Creature monster, List<PlayerCharacter> targets)
    {
        List<Tile> shortestTiles = PathFinding.FindPath(monster.CurrentTile,
            targets[Random.Range(0, targets.Count)].CurrentTile, new List<Tile>());
        List<Tile> attackRangeTiles = new List<Tile>();
        
        foreach (PlayerCharacter playerCharacter in targets)
        {
            attackRangeTiles = GetRangeTiles(playerCharacter.CurrentTile, monster.CurrentStat.MaxAttackRange, monster.CurrentStat.MinAttackRange);
            
            foreach (Tile attackRangeTile in attackRangeTiles)
            {
                
                List<Tile> path = PathFinding.FindPath(monster.CurrentTile, attackRangeTile, new List<Tile>());

                if (attackRangeTile == playerCharacter.CurrentTile)
                    continue;

                if (shortestTiles.Count == 0)
                {
                    shortestTiles = path;
                }

                if (shortestTiles.Count > path.Count && path.Count <= monster.CurrentStat.TurnCost)
                {
                    shortestTiles = path;
                }
            }
        }
        
        return shortestTiles;
    }
    
    public IEnumerator CoMovePath(List<Monster> monsters, List<PlayerCharacter> targets)
    {
        if (monsters == null)
        {
            Managers.Game.GameMode = Define.GameMode.PlayerTurn;
            yield break;
        }
        
        Debug.Log("CoMovePath");
        foreach (Monster monster in monsters)
        {
            Debug.Log($"{monster.name} move");
            _monster = monster;
            monster.State = Define.State.Move;
            List<Tile> path = MonsterMovement(monster, targets);
            int cost = monster.CurrentStat.TurnCost;
            while (true)
            {
                yield return null;
                
                if (path.Count <= 0 || cost == 0)
                {
                    List<Tile> attackRangeTiles = PathFinding.GetAttackSurroundingTiles(monster.CurrentTile.Grid2DLocation, Managers.Map.MapTiles, monster.CurrentStat.MaxAttackRange);
                    
                    monster.State = Define.State.Idle;
                    foreach (Tile tile in attackRangeTiles)
                    {
                        foreach (PlayerCharacter target in _playerCharacters)
                        {
                            if (tile == target.CurrentTile)
                            {
                                Debug.Log($"{monster.name} attack");
                                monster.State = Define.State.Attack;
                                monster.Attack(target);
                                break;
                            }
                        }

                        if (monster.State == Define.State.Attack)
                            break;
                    }
                    
                    Debug.Log($"{monster.name} idle");
                    
                    monster.State = Define.State.Idle;
                    Managers.Game.ResetTurn();
                    break;
                }

                monster.Move(path[0]);
                
                if (Vector2.Distance(monster.transform.position, path[0].transform.position) < 0.00001f)
                {
                    monster.CharacterPositionOnTile(path[0]);
                    path.RemoveAt(0);
                    cost--;
                }

                if (cost == 0 || path.Count == 0)
                {
                    monster.GetComponent<Animator>().SetBool("Walk", false);
                }
            }

            _monster = null;
        }
        
        Managers.Game.GameMode = Define.GameMode.PlayerTurn;
    }

    private void ResetTurn()
    {
        foreach (PlayerCharacter playerCharacter in _playerCharacters)
        {
            playerCharacter.ResetTurnCost();
        }
    }
    
    public void Clear()
    {
        _loadPlayerCharacters?.Clear();
    }
    
    public void GameQuit()
    {
        Managers.Clear();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}