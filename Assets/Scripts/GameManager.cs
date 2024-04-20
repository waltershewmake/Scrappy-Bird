using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum GameState
{
    Menu, // -> Playing | quit game
    Playing, // -> GameOver
    GameOver // -> Menu | Playing
}

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;
    
    public GameObject playerPrefab;
    public Text roomCodeText;

    private GameState _gameState;
    
    public GameState State
    {
        get => _gameState;
        set
        {
            switch (value)
            {
                case GameState.Menu:
                    roomCodeText.gameObject.SetActive(true);
                    
                    Time.timeScale = 0f;

                    foreach (PlayerManager playerManager in playerManagers.Values)
                        playerManager.State = PlayerState.Menu;
                    
                    _gameState = GameState.Menu;
                    break;
                case GameState.Playing:
                    
                    roomCodeText.gameObject.SetActive(false);
                    
                    Time.timeScale = 1f;
                    
                    foreach (PlayerManager playerManager in playerManagers.Values)
                        playerManager.State = PlayerState.Alive;
                    
                    _gameState = GameState.Playing;
                    break;
                case GameState.GameOver:
                    roomCodeText.gameObject.SetActive(false);
                    
                    Time.timeScale = 0f;

                    foreach (PlayerManager playerManager in playerManagers.Values)
                        playerManager.State = PlayerState.Hidden;
                    
                    _gameState = GameState.GameOver;
                    break;
            }
        }
    }

    private Dictionary<string /*player name*/, PlayerJoin> playerJoins = new Dictionary<string, PlayerJoin>();
    private Dictionary<string /*player name*/, PlayerManager> playerManagers = new Dictionary<string, PlayerManager>();

    [DllImport("__Internal")]
    private static extern void MessageToPlayer(string userName, string message);
    [DllImport("__Internal")]
    private static extern void MessageToAllPlayers(string message);

    [DllImport("__Internal")]
    private static extern void Exit();

    [DllImport("__Internal")]
    private static extern void UpdateStats(string userName, string stats);

    #region Madderness
    public void RoomCode(string roomCode)
    {
        roomCodeText.text = roomCode;
    }

    public void PlayerJoined(string jsonPlayerJoin)
    {
        PlayerJoin playerJoin = JsonUtility.FromJson<PlayerJoin>(jsonPlayerJoin);
        
        Debug.Log("Player Joined: " + playerJoin.name);

        GameStats expectedStats = new GameStats();
        if (playerJoin.stats == null)
        {
            playerJoin.stats = expectedStats;
        }
        else
        {
            string[] keysToCheck = new []{ "Games Played", "Round Wins", "Game Wins" };
            foreach (string key in keysToCheck)
            {
                if (playerJoin.stats.GetType().GetField(key) == null)
                {
                    playerJoin.stats = expectedStats;
                    break;
                }
            }
        }
        
        playerJoins.Add(playerJoin.name, playerJoin);
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerManager playerManager = newPlayer.GetComponent<PlayerManager>();
        
        newPlayer.name = playerManager.playerNameText.text = playerJoin.name;
        playerManager.gameStats = playerJoin.stats;
        
        playerManagers.Add(playerJoin.name, playerManager);
        
        int playerIndex = 0;
        foreach (PlayerManager manager in playerManagers.Values)
        {
            // re-calculate the viewport rect for each player
            manager.playerCamera.rect = new Rect((1f / playerManagers.Count) * playerIndex, 0f, 1f / playerManagers.Count, 1f);
            playerIndex++;
        }
    }

    public void PlayerLeft(string playerName)
    {
        if (playerJoins.ContainsKey(playerName)) playerJoins.Remove(playerName);
        else return;

        if (playerJoins.Count == 0) HandleExit();
        
        if (playerManagers.ContainsKey(playerName))
        {
            Destroy(playerManagers[playerName].gameObject);
            playerManagers.Remove(playerName);
        }
        
        Debug.Log("Player Left: " + playerName);
    }

    public void PlayerControllerState(string jsonControllerState)
    {
        ControllerState controllerState = JsonUtility.FromJson<ControllerState>(jsonControllerState);
        
        Debug.Log("Player: " + controllerState.name);
        Debug.Log("Joystick: " + controllerState.Joystick.x + ", " + controllerState.Joystick.y);
        Debug.Log("Circle: " + controllerState.circle);
        Debug.Log("Triangle: " + controllerState.triangle);
        Debug.Log("Plus: " + controllerState.plus);
        
        if (playerManagers.ContainsKey(controllerState.name))
        {
            PlayerManager playerManager = playerManagers[controllerState.name];
            playerManager.circleButtonDown = controllerState.circle;
            playerManager.triangleButtonDown = controllerState.triangle;
        }
        
        if (controllerState.plus)
        {
            if (playerJoins.Count > 1) Play();
        }
    }

    public class Message
    {
        public string name;
        public string message;
    }

    [System.Serializable]
    public class GameStats
    {
        public Stat gamesPlayed;
        public Stat gameWins;
        
        public GameStats()
        {
            gamesPlayed = new Stat("Games Played", 0);
            gameWins = new Stat("Game Wins", 0);
        }
        
        public void addGamePlayed()
        {
            gamesPlayed.value++;
        }
        public void addGameWin()
        {
            gameWins.value++;
        }
    }

    [System.Serializable]
    public class Stat
    {
        public string title;
        public int value;

        public Stat(string _title, int _value)
        {
            title = _title;
            value = _value;
        }
    }

    public class PlayerJoin
    {
        public string name;
        public GameStats stats;
    }

    [System.Serializable]
    public class Joystick
    {
        public int x;
        public int y;

        public Joystick(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public class ControllerState
    {
        public string name;
        public Joystick Joystick;
        public bool circle;
        public bool triangle;
        public bool plus;
    }
    #endregion

    #region Unity Functions
    private void Awake()
    {
        Application.targetFrameRate = 60;
        State = GameState.Menu;
        
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Update()
    {
        #if UNITY_EDITOR == true && UNITY_WEBGL == false
        DebugMode();
        #endif
        
        if (State != GameState.Playing) return;
        
        bool allDead = true;
        foreach (PlayerManager playerManager in playerManagers.Values)
            if (playerManager.isAlive)
            {
                allDead = false;
                break;
            }

        if (allDead) State = GameState.GameOver;
        
    }
    #endregion

    private void DebugMode()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RoomCode(Random.Range(1000, 9999).ToString());
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerJoin playerJoin = new PlayerJoin();
            playerJoin.name = "Player " + Random.Range(1000, 9999);
            playerJoin.stats = new GameStats();
            PlayerJoined(JsonUtility.ToJson(playerJoin));
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (playerJoins.Count == 0) return;
            string[] keys = new string[playerJoins.Keys.Count];
            playerJoins.Keys.CopyTo(keys, 0);
            PlayerLeft(keys[Random.Range(0, keys.Length)]);
        }

        if (Input.GetKeyDown(KeyCode.S))
            PlayerControllerState(
                JsonUtility.ToJson(((new ControllerState(){ plus = true })))
                );
    }
    
    public void HandleExit()
    {
        #if UNITY_WEBGL == true && UNITY_EDITOR == false
        Exit();
        #endif
    }
    
    public void Play()
    {
        State = GameState.Playing;

        Pipes[] pipes = FindObjectsOfType<Pipes>();
        foreach (Pipes pipe in pipes) Destroy(pipe.gameObject);
    }
}
