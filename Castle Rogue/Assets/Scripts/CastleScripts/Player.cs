﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class Player : MovingObject {

    public int Damage = 1;
    public int pointsPerCoin = 10;
    public int pointsPerDiamond = 20;
    public int staminaPerPotion = 25;
    public int horizontal;
    public int vertical;
    public GameObject adCanvas;
    public GameObject menuButton;
    
    private Button rightArrow;
    private Button upArrow;
    private Button leftArrow;
    private Button downArrow;
    private Button yesAd;
    private Button noAd;

    [SerializeField]
    private float restartDelay = 1f;
    [SerializeField]
    private Text staminaText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private int staminaPerAd = 50;

    private bool hasWatchedAd = false;
    private Animator animator;
    private int stamina;
    private int score;

    // Use this for initialization
    protected override void Start()
    {
        staminaText = GameObject.Find("staminaText").GetComponent<Text>();
        scoreText = GameObject.Find("scoreText").GetComponent<Text>();
        adCanvas = GameObject.Find("adCanvas");
        menuButton = GameObject.Find("MenuButton");
        yesAd = GameObject.Find("YesAd").GetComponent<Button>();

#if UNITY_ANDROID
        yesAd.onClick.AddListener(ShowRewardedAd);
#endif

        rightArrow = GameObject.Find("RightArrow").GetComponent<Button>();
        rightArrow.onClick.AddListener(RightButton);
        upArrow = GameObject.Find("UpArrow").GetComponent<Button>();
        upArrow.onClick.AddListener(UpButton);
        leftArrow = GameObject.Find("LeftArrow").GetComponent<Button>();
        leftArrow.onClick.AddListener(LeftButton);
        downArrow = GameObject.Find("DownArrow").GetComponent<Button>();
        downArrow.onClick.AddListener(DownButton);
        noAd = GameObject.Find("NoAd").GetComponent<Button>();

        #if UNITY_ANDROID
        noAd.onClick.AddListener(NoToAds);
#endif

        adCanvas.SetActive(false);
        animator = GetComponent<Animator>();
        stamina = GameManager.instance.playerStaminaPoints;
        staminaText.text = ("Stamina: " + stamina);
        score = GameManager.instance.playerScorePoints;
        scoreText.text = ("Score: " + score);
        base.Start();

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        rightArrow.gameObject.SetActive(false);
        upArrow.gameObject.SetActive(false);
        leftArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
#endif
    }

    private void OnDisable()
    {
        GameManager.instance.playerStaminaPoints = stamina;
        GameManager.instance.playerScorePoints = score;
    }

    // Update is called once per frame
    private void Update () {
        if (!GameManager.instance.playersTurn) return;

        //int horizontal = 0;
        //int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        if (horizontal != 0)
        {
            vertical = 0;
        }
        
#else
        //Do nothing
#endif
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<InnerWalls>(horizontal, vertical);
            horizontal = 0;
            vertical = 0;
        }
        horizontal = 0;
        vertical = 0;
	}

    public void RightButton()
    {
        horizontal = 1;
    }
    public void UpButton()
    {
        vertical = 1;
    }
    public void LeftButton()
    {
        horizontal = -1;
    }
    public void DownButton()
    {
        vertical = -1;
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        stamina--;
        Debug.Log("Stamina Move Loss");
        staminaText.text = ("Stamina: " + stamina);
        scoreText.text = ("Score: " + score);
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;

        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        if (component is InnerWalls)
        {
            InnerWalls hitWall = component as InnerWalls;
            hitWall.DamageWall(Damage);
            animator.SetTrigger("rogueAttack");
        }
    }   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartDelay);
            enabled = false;
        }
        else if (other.tag == "Coin")
        {
            score += pointsPerCoin;
            scoreText.text = ("+" + pointsPerCoin + " Score: " + score);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Diamond")
        {
            score += pointsPerDiamond;
            scoreText.text = ("+" + pointsPerDiamond + " Score: " + score);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Potion")
        {
            stamina += staminaPerPotion;
            staminaText.text = ("+" + staminaPerPotion + " Stamina: " + stamina);
            other.gameObject.SetActive(false);
        }
    }
    
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseStamina (int loss)
    {
        animator.SetTrigger("rogueHit");
        stamina -= loss;
        staminaText.text = ("-" + loss + " Stamina: " + stamina);
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
#if UNITY_ANDROID
        if (stamina <= 0)
            if (hasWatchedAd == false)
            {
                adCanvas.SetActive(true);
                Time.timeScale = 0;
                hasWatchedAd = true;
            }
            else
                GameManager.instance.GameOver();
#else
        if(stamina <= 0)
            GameManager.instance.GameOver();
#endif
    }

    //Ad Code
#region Advertisments

#if UNITY_ANDROID
    public void NoToAds()
    {
        Time.timeScale = 1;
        adCanvas.SetActive(false);
        GameManager.instance.GameOver();
    }
    public void ShowDefaultAd()
    {
        if (!Advertisement.IsReady())
        {
            Debug.Log("Ads not ready for default placement");
            if (adCanvas != null)
                adCanvas.SetActive(false);
            menuButton.SetActive(true);
            return;
        }

        Advertisement.Show();
    }

    public void ShowRewardedAd()
    {
        const string RewardedPlacementId = "rewardedVideo";
        Time.timeScale = 1;
        menuButton.SetActive(false);
        if (!Advertisement.IsReady(RewardedPlacementId))
        {
            Debug.Log(string.Format("Ads not ready for placement '{0}'", RewardedPlacementId));
            if (adCanvas != null)
                adCanvas.SetActive(false);
            menuButton.SetActive(true);
            return;
        }

        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show(RewardedPlacementId, options);
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                if(adCanvas != null)
                    adCanvas.SetActive(false);
                menuButton.SetActive(true);
                stamina = stamina + staminaPerAd;
                staminaText.text = ("+" + staminaPerAd + " Stamina: " + stamina);
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
#endif

#endregion
}
