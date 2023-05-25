using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SShop : MonoBehaviour
{
    int comprato;//controlla se l'oggetto è comprato se è comprato ,comprato uguale 1         check if the item is bought if it is bought, bought equal 1
    public Image show;//controllo oggetto selezionato     selected object control
    public GestioneShop Gs;
  public void Comprare()
    {
        string indexconvert = gameObject.transform.GetChild(0).name;
        int indice = Convert.ToInt32(indexconvert);//converte il nome dell'oggetto in numero     converts the name of the object into a number
        if (PlayerPrefs.GetInt(gameObject.name) == 0)//controllo se oggetto  è comprato se non è comprato viene attivato  check if item is bought if it's not bought is activated
        {

            string Daconvertire = gameObject.GetComponentInChildren<Text>().text;
            int convertito = Convert.ToInt32(Daconvertire);//converte il testo del componente text in numero    converts the text of the text component to a number
            Gs.coin = Gs.coin - convertito;
            if (Gs.coin < 0)//controllo se le monete sono meno di 0   check if the coins are less than 0
            {
                Gs.coin = Gs.coin + convertito;
                Debug.Log("non puoi comprare");
            }
           else if (Gs.coin >= 0)
            {
                comprato = 1;
                Debug.Log("comprato " + comprato);
                GetComponentInChildren<Text>().text = "0";//imposta il prezzo a 0  set the price to 0
                PlayerPrefs.SetInt(gameObject.name, comprato);//salva oggetto comprato       save purchased item
                show.sprite = Gs.palle[indice];//imposta oggetto selezionato    set selected item
                Debug.Log("indice " + indexconvert);
            }
        }
        else if (PlayerPrefs.GetInt(gameObject.name) == 1)//se l'oggetto è comprato   if the item is bought
            show.sprite = Gs.palle[indice];// cambia oggetto selezionato    change selected object
    }
    
}
