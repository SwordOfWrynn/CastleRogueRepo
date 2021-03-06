﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CharacterInfo : MonoBehaviour {

    public Button purchaseButton;
    public Button selectButton;
    public int characterInfoArrayNumber;
    public int price = 0;
    public CurrencyManager currencyManager;
    public bool[] isBought;

    private int money;
    private bool FemmeFatale = false;

	// Use this for initialization
	void Start () {
        if (File.Exists(Application.persistentDataPath + "/Purchases.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Purchases.dat", FileMode.Open);
            PurchaseInfo myLoadedInfo = (PurchaseInfo)bf.Deserialize(file);
            isBought = myLoadedInfo.purchase;
        }
        else
            isBought = new bool [100];
        selectButton.gameObject.SetActive(false);
        GameObject currencyManagerObject = GameObject.Find("CurrencyManager");
        currencyManager = currencyManagerObject.GetComponent<CurrencyManager>();
        money = currencyManager.money;
        Debug.Log("Money = " + money);
        Debug.Log("Array Number = " + characterInfoArrayNumber);
		if (isBought[characterInfoArrayNumber] == true)
        {
            purchaseButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Purchase()
    {
        money = currencyManager.money;
        if (price <= money)
        {
            isBought[characterInfoArrayNumber] = true;
            purchaseButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
            money = money - price;
            currencyManager.money = money;
            //saving the purchase
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Purchases.dat", FileMode.OpenOrCreate);
            PurchaseInfo myInfo = new PurchaseInfo();
            myInfo.purchase = isBought;
            bf.Serialize(file, myInfo);
            file.Close();
        }
    }
    public void Select()
    {

        //saving the selection
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/SelectedCharacter.dat", FileMode.OpenOrCreate);
        SelectInfo myInfo = new SelectInfo();
        myInfo.character = characterInfoArrayNumber;
        bf.Serialize(file, myInfo);
        file.Close();
    }
}
[System.Serializable]
public class PurchaseInfo
{
    public bool [] purchase;
}
