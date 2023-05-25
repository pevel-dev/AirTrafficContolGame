using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestioneShop : MonoBehaviour
{
    public GameObject[] prodotto;//inserire gli oggetti dello shop     insert the objects of the shop
    public Sprite[] palle;//inserire tutti gli sprite nello stesso ordine degli oggetti dello shop   insert all the sprites in the same order as the shop objects
    public int NOggetti, coin=60;//in NOggetti va il numero degli oggetti comprati e non, coin numero soldi totali      in NOggetti goes the number of items purchased and not, 
    //coin number of total money
    public Text TCoin;

    public void Start()
    {
        
        for (int o = 0; o < NOggetti; o++)//controlla se l'oggetto è stato comprato    check if the item was bought
            if (PlayerPrefs.GetInt(prodotto[o].gameObject.name) == 1)
                prodotto[o].gameObject.GetComponentInChildren<Text>().text = "0";
    }
    public void Update()
    {

        TCoin.text = "COIN " + coin;

        if (Input.GetKey(KeyCode.T))//cancella oggetti comprati    delete purchased items
            for (int o = 0; o < NOggetti; o++)
                PlayerPrefs.DeleteKey(prodotto[o].gameObject.name);
       
    }
}
