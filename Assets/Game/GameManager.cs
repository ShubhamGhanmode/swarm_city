//using UnityEngine;
//using UnityEngine.SceneManagement;

//public enum GameState
//{
//    Playing,
//    Paused,
//    Won,
//    Lost
//}

//public class GameManager : MonoBehaviour
//{
//    public static GameManager Instance { get; private set; }

//    public GameUI ui;
//    public string levelSceneName = "Demo_City";  // change to your actual scene name

//    public GameState State { get; private set; } = GameState.Playing;

//    void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    void Start()
//    {
//        if (!ui)
//            ui = FindObjectOfType<GameUI>();

//        if (ui)
//            ui.SetObjective("Reach the radio at the crash site");
//    }

//    void Update()
//    {
//        if (State == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
//            PauseToggle();
//    }

//    public void PauseToggle()
//    {
//        if (State == GameState.Paused)
//        {
//            State = GameState.Playing;
//            Time.timeScale = 1f;
//            if (ui) ui.HideCenterMessage();
//        }
//        else if (State == GameState.Playing)
//        {
//            State = GameState.Paused;
//            Time.timeScale = 0f;
//            if (ui) ui.ShowCenterMessage("Paused\nPress ESC to resume");
//        }
//    }

//    public void Win()
//    {
//        if (State == GameState.Won || State == GameState.Lost) return;
//        State = GameState.Won;
//        Time.timeScale = 0f;
//        if (ui) ui.ShowCenterMessage("You escaped!\nPress R to restart");
//    }

//    public void Lose()
//    {
//        if (State == GameState.Won || State == GameState.Lost) return;
//        State = GameState.Lost;
//        Time.timeScale = 0f;
//        if (ui) ui.ShowCenterMessage("You were caught\nPress R to restart");
//    }

//    void LateUpdate()
//    {
//        if ((State == GameState.Won || State == GameState.Lost) && Input.GetKeyDown(KeyCode.R))
//        {
//            Time.timeScale = 1f;
//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//            State = GameState.Playing;
//        }
//    }
//}
