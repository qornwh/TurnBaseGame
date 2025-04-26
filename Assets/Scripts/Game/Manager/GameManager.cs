using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    // 테스트용 코드
    public List<PlayerState> PlayerList { get; set; } //플레이어 키, 캐릭터 코드
    public Dictionary<int, List<PlayerState>> TeamDic { get; set; } // 팀, 캐릭터 코드
    public Dictionary<string, GameObject> PrefabDict { get; set; } // 경로, 프리팹

    public int MapCode = 101;
    // 이제 사망한 캐릭터들은 빼야 되는데...

    public int PlayerId { get; set; }
    public TileManager TileManager { get; set; }
    public UiManager UiManager { get; set; }
    public TurnSystem TurnSystem { get; set; }
    public GameDataManager GameDataManager { get; set; }
    public Camera GameCamera { get; set; }
    public ObjectPoolManager ObjectPoolManager { get; set; }

    [FormerlySerializedAs("BlockBase")][SerializeField] private BlockBase blockBase;
    [FormerlySerializedAs("MouseHover")][SerializeField] private MouseHover mouseHover;
    [FormerlySerializedAs("PlayerUi")][SerializeField] private PlayerUi playerUi;
    [FormerlySerializedAs("TurnUi")][SerializeField] private TurnUi turnUi;
    private BlockState<BlockBase> _selectedBlock; // 이동할 위치
    private Dictionary<string, Sprite> SpriteMap { get; set; }

    private void TestInit()
    {
        // 테스트용 코드
        PlayerId = 1;
        PlayerList = new List<PlayerState>();
        TeamDic = new Dictionary<int, List<PlayerState>>();
        PlayerList.Add(new PlayerState(1, new Vector2Int(0, 2),1, false, GameDataManager.GetCharacter(100)));
        PlayerList.Add(new PlayerState(2, new Vector2Int(7, 3), 2, true, GameDataManager.GetCharacter(300)));
        TeamDic.Add(1, PlayerList.Where(p => p.Team == 1).ToList());
        TeamDic.Add(2, PlayerList.Where(p => p.Team == 2).ToList());

        var gameObject1 = SpawnPrefab("Prefabs/Default_Player");
        if (gameObject1 != null && gameObject1.TryGetComponent<Player>(out var player))
        {
            var playerState = PlayerList.Find(p => p.PlayerID == 1);
            if (playerState != null)
            {
                var tilePosition = TileManager.GetTilePosition(playerState.Position);
                tilePosition.y = 0;
                TileManager.RootBlockStates[playerState.Position.y][playerState.Position.x].Type = BlockType.Player;
                player.Init(playerState, tilePosition);

                playerUi.Init(playerState);
            }
        }

        var gameObject2 = SpawnPrefab("Prefabs/Default_Player");
        if (gameObject2 != null && gameObject2.TryGetComponent<Player>(out var player2))
        {
            var playerState = PlayerList.Find(p => p.PlayerID == 2);
            if (playerState != null)
            {
                var tilePosition = TileManager.GetTilePosition(playerState.Position);
                tilePosition.y = 0;
                TileManager.RootBlockStates[playerState.Position.y][playerState.Position.x].Type = BlockType.Player;
                player2.Init(playerState, tilePosition);
            }
        }
        //
    }

    private void Awake()
    {
        GameInstance.GetInstance().GameManager = this;
        GameDataManager = new GameDataManager();
        UiManager = new UiManager();
        TileManager = new TileManager();
        TurnSystem = new TurnSystem();
        SpriteMap = new Dictionary<string, Sprite>();
        PrefabDict = new Dictionary<string, GameObject>();
        ObjectPoolManager = gameObject.AddComponent<ObjectPoolManager>();
    }

    void Start()
    {
        SetResolution();
        GameCamera = Camera.main;
        TileManager.CreateGameObject += CreateEmptyObject;
        TileManager.CreateBlock += CreateTile;
        TileManager.Init();
        UiManager.Init();

        TestInit();
        TurnSystem.Init(PlayerList.Select(p => p.PlayerID).ToList());

        StartGame();
    }

    GameObject CreateEmptyObject(Vector3 position, Quaternion rotation)
    {
        GameObject emptyObject = new GameObject();
        emptyObject.transform.localPosition = position;
        emptyObject.transform.localRotation = rotation;
        return emptyObject;
    }

    GameObject CreateTile(GameObject parent, Vector3 position, Quaternion rotation)
    {
        var block = Instantiate(blockBase, parent.transform);
        block.transform.localPosition = position;
        block.transform.localRotation = rotation;
        return block.gameObject;
    }

    public T CreateObject<T>(T prefab, GameObject parent) where T : Component
    {
        if (parent == null)
            return Instantiate(prefab);
        return Instantiate(prefab, parent.transform);
    }

    public GameObject CreateObject<T>(T prefab, GameObject parent, Vector3 position, Quaternion rotation) where T : Component
    {
        T instance = CreateObject(prefab, parent);
        instance.transform.localPosition = position;
        instance.transform.localRotation = rotation;
        return instance.gameObject;
    }

    public GameObject CreateObject<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        T instance = CreateObject(prefab);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        return instance.gameObject;
    }

    public T CreateObject<T>(T prefab) where T : Component
    {
        return CreateObject(prefab, null);
    }

    void StartGame()
    {
        TurnSystem.OnTurnStart += () =>
        {
            turnUi.OnTurnUpdate();
            turnUi.OnSubTurnUpdate();
        };
        TurnSystem.OnMovePhaseStart += (playerId) =>
        {
            turnUi.OnSubTurnUpdate();
            var playerState = PlayerList.Find(p => p.PlayerID == playerId);
            // 해당 플레이어를 가져온다 (통신은 일단 제외)
            if (TurnSystem.GetCurrentPlayerId() == PlayerId)
            {
                // 내 플레이어인 경우
                MoveTurn(playerState.Move);
                UiManager.CreateDoneUi();
            }
            else
            {
                // ai 플레이어인 경우
                // astar 알고리즘으로 최단 경로 찾기
                TrunDone();
            }
        };
        TurnSystem.OnMovePhaseEnd += () =>
        {
            // 플레이어 이동완료
        };
        TurnSystem.OnSkillPhaseStart += (playerId) =>
        {
            turnUi.OnSubTurnUpdate();
            if (TurnSystem.GetCurrentPlayerId() == PlayerId)
            {
                // 내 플레이어인 경우
                // 스킬 셋팅
                AttackTurn();
                UiManager.CreateDoneUi();
            }
            else
            {
                TrunDone();
            }
        };
        TurnSystem.OnSkillExecute += (playerId) =>
        {
            turnUi.OnSubTurnUpdate();
            TrunDone();
        };
        TurnSystem.OnSkillPhaseEnd += () =>
        {
            turnUi.OnSubTurnUpdate();
            // 버프 1턴 당기기
            foreach (var playerState in PlayerList)
            {
                playerState.TurnUpdate();
                playerState.ManaRegen();
            }
        };
        TurnSystem.OnTurnEnd += () =>
        {
            // 게임 종료
            turnUi.OnSubTurnUpdate();
        };
        TurnSystem.IsSkillEnd += () =>
        {
            foreach (var playerState in PlayerList)
            {
                if (playerState.SkillCodes.Count > 0) 
                    return false;
            }
            return true;
        };
        
        TurnSystem.StartTurn();
        CreateObject(mouseHover);
        mouseHover.enabled = false;
    }

    public void TrunDone()
    {
        var playerState = PlayerList.Find(p => p.PlayerID == TurnSystem.GetCurrentPlayerId());
        if (playerState.IsDead()) 
            EndTurn();
        
        if (TurnSystem.GetCurrentPhase() == TurnPhase.Move)
        {
            if (playerState != null)
            {
                if (playerState.PlayerID == PlayerId)
                {
                    playerState.OnMove(_selectedBlock.Position);
                    TileManager.HideMovePointer();
                }
                else if (playerState.IsAi)
                {
                    playerState.OnAutoMove();
                }
                _selectedBlock = null;
            }
        }
        else if (TurnSystem.GetCurrentPhase() == TurnPhase.Skill)
        {
            if (playerState != null)
            {
                if (playerState.PlayerID == PlayerId)
                {
                    AttackTurnEnd();
                    EndTurn();
                }
                else if (playerState.IsAi)
                {
                    // ai 스킬 정하기
                    playerState.OnAutoSkill();
                    EndTurn();
                }
            }
        }
        else if (TurnSystem.GetCurrentPhase() == TurnPhase.SkillExecute)
        {
            if (playerState != null)
            {
                if (playerState.SkillCodes.Count > 0)
                {
                    int attackCode = playerState.SkillCodes.First();
                    playerState.SkillCodes.RemoveFirst();
                    playerState.OnSkill(attackCode);
                    playerState.Mp -= GameDataManager.GetSkill(attackCode).manaCost;
                }
                else
                {
                    EndTurn();
                }
                
                // 게임 종료 체크한다.
                int team = playerState.Team;
                foreach (var state in PlayerList)
                {
                    if (state.Team != team && !state.IsDead())
                    {
                        return;
                    }
                }

                TurnSystem.TurnEndPhase();
            }
        }
    }

    public void EndTurn()
    {
        TurnSystem.EndTurn();
    }

    void MoveTurn(int distance)
    {
        // 움직이기 전, 어떤 위치로 갈수 있는지 보여주기 시작
        var playerState = PlayerList.Find(p => p.PlayerID == PlayerId);
        TileManager.ViewMovePinter(playerState.Position.x, playerState.Position.y, distance);
        mouseHover.enabled = true;
    }
    public void MoveTurnEnd()
    {
        // 캐릭터 이동시키기
        mouseHover.enabled = false;
    }

    void AttackTurn()
    {
        // 공격, 스킬 셋팅 시작
        ShowUi("AttackUi");
    }
    void AttackTurnEnd()
    {
        // 공격, 스킬 셋팅 종료
        HideUi("AttackUi");
    }

    public void SelectBlock(BlockState<BlockBase> state)
    {
        if (_selectedBlock != null)
        {
            _selectedBlock.Type = BlockType.MoveDst;
        }
        state.Type = BlockType.MoveSelectDst;
        _selectedBlock = state;
    }

    public void SetResolution()
    {
        const int setWidth = 1920;
        const int setHight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)((float)deviceHeight / deviceWidth * setWidth), false);

        if ((float)setWidth / setHight < (float)deviceWidth / deviceHeight)
        {
            // 기기의 너비 비율이 더 클떄
            float newWidth = ((float)setWidth / setHight) / ((float)deviceWidth / deviceHeight); // 새로운 비율
            if (Camera.main != null) Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            // 기기의 높이 비율이 더 클때
            float newHight = ((float)setHight / setWidth) / ((float)deviceHeight / deviceWidth); // 새로운 비율
            if (Camera.main != null) Camera.main.rect = new Rect(0f, (1f - newHight) / 2f, 1f, newHight);
        }
    }

    public void PushSkill(int skillCode)
    {
        // 플레이할 스킬을 넣는다.
        // 마나를 깐다.
    }

    public PlayerState GetPlayerState(int playerId)
    {
        var state = PlayerList.Find(p => p.PlayerID == playerId);
        if (state != null)
        {
            return state;
        }
        return null;
    }
    
    public PlayerState GetPlayerState(Vector2Int position)
    {
        var state = PlayerList.Find(p => p.Position == position);
        if (state != null)
        {
            return state;
        }
        return null;
    }

    public void ShowUi(String uiName)
    {
        UiManager.GetUi(uiName).SetActive(true);
    }

    public void HideUi(String uiName)
    {
        UiManager.GetUi(uiName).SetActive(false);
    }

    public GameObject SpawnPrefab(String path, GameObject parent = null)
    {
        GameObject prefab = LoadPrefab(path);
        if (prefab == null) 
            return null;
        if (parent == null)
            return Instantiate(prefab);
        return Instantiate(prefab, parent.transform);
    }

    public Sprite LoadSpriteFromResources(string path)
    {
        if (!SpriteMap.ContainsKey(path))
        {
            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError($"Failed to load sprite from path : {path}");
                return null;
            }
            SpriteMap.Add(path, sprite);
        }
        return SpriteMap[path];
    }

    public GameObject LoadPrefab(String path)
    {
        if (!PrefabDict.ContainsKey(path))
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab from path : {path}");
                return null;
            }
            PrefabDict.Add(path, prefab);
        }
        return PrefabDict[path];    
    }

    public bool IsWin()
    {
        if (GetPlayerState(PlayerId).IsDead())
            return false;
        return true;
    }
}
