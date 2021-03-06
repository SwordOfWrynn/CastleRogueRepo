﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int finalRoomsMultiplier = 10;
    public int finalScoreMultiplier = 10;

    public float levelStartDelay = 2f;
    public float turnDelay = .2f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerStaminaPoints = 100;
    public Text endScoreText;
    public Text totalScoreScore;
    public Text endRoomsText;
    public Text totalRoomScore;
    public Text totalScoreText;
    public Text totalScoreNumber;

    [HideInInspector] public int playerScorePoints = 0;
    [HideInInspector] public bool playersTurn = true;
    [HideInInspector] public GameObject MainMenuLossButton;

    private Text levelText;
    private GameObject levelImage;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private bool firstRoom = true;

    public int level = 1;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();

        boardScript = GetComponent<BoardManager>();

        //InitGame();
    }
    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
        //Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    //This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Add one to our level number.
        if (firstRoom == true)
        {
            firstRoom = false;
            InitGame();
            return;
        }
        level++;
        Debug.Log("Level " + level);
        //Call InitGame to initialize our level.
        InitGame();
    }


    void InitGame() {
        doingSetup = true;
        MainMenuLossButton = GameObject.Find("MainMenuLossButton");
        MainMenuLossButton.SetActive(false);
        endScoreText = GameObject.Find("endScoreText").GetComponent<Text>();
        endScoreText.text = "";
        totalScoreScore = GameObject.Find("totalScoreScore").GetComponent<Text>();
        totalScoreScore.text = "";
        endRoomsText = GameObject.Find("endRoomsText").GetComponent<Text>();
        endRoomsText.text = "";
        totalRoomScore = GameObject.Find("totalRoomScore").GetComponent<Text>();
        totalRoomScore.text = "";
        totalScoreText = GameObject.Find("totalScoreText").GetComponent<Text>();
        totalScoreText.text = "";
        totalScoreNumber = GameObject.Find("totalScoreNumber").GetComponent<Text>();
        totalScoreNumber.text = "";
        levelImage = GameObject.Find("levelImage");
        levelText = GameObject.Find("levelText").GetComponent<Text>();
        levelText.text = ("Room " + level);
        levelImage.SetActive(true);
        enemies.Clear();
        boardScript.SetupScene(level);
        Invoke("HideLevelImage", levelStartDelay);
	}

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        endScoreText.text = "Score " + playerScorePoints + " * " + finalScoreMultiplier + " =";
        int score = playerScorePoints * finalScoreMultiplier;
        totalScoreScore.text = score.ToString();
        endRoomsText.text = "Rooms " + level + " * " + finalRoomsMultiplier + " =";
        int roomScore = level * finalRoomsMultiplier;
        totalRoomScore.text = roomScore.ToString();
        totalScoreText.text = "Total Score =";
        int totalScore = score + roomScore;
        totalScoreNumber.text = totalScore.ToString();

        levelText.text = ("You robbed " + level + " rooms before \n you had to make your escape");
        levelImage.SetActive(true);
        MainMenuLossButton.SetActive(true);
        enabled = false;
    }
    private void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        StartCoroutine(MoveEnemies());
    }
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
    public void RemoveEnemiesFromList(Enemy script)
    {
        enemies.Remove(script);
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(0.3f);
        }
        playersTurn = true;
        enemiesMoving = false;
    }

    public void MenuButton()
    {
        GameObject gameManager = GameObject.Find("GameManager(Clone)");
        Destroy(gameManager);
        SceneManager.LoadScene("MainMenu");
    }
    
}
