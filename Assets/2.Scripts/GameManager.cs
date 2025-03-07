using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public UIDocument UIDoc;
    public GameObject KeyPanel;
    public TurnManager TurnManager { get; private set; }
    public int Level {
        get { return m_CurrentLevel; }
        private set {}
    }

    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    private Label m_FoodLabel;
    private int m_FoodAmount = 10;
    private int m_CurrentLevel = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        KeyPanel.SetActive(true);
#else
        KeyPanel.SetActive(false);
#endif

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;
        
        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        StartNewGame();
    }

    public void NewLevel()
    {
        m_CurrentLevel++;
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }

    public void StartNewGame()
    {
        m_CurrentLevel = 0;
        m_FoodAmount = 100;
        m_GameOverPanel.style.visibility = Visibility.Hidden;
        m_FoodLabel.text = $"Food : {m_FoodAmount:000}";
        
        PlayerController.Init();
        NewLevel();
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = $"Food : {m_FoodAmount:000}";

        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = $"Game Over!\n\nYou traveled through \n\n{m_CurrentLevel} levels";
        }
    }
}
