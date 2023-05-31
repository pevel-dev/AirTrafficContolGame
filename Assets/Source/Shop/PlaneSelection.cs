using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaneSelection : MonoBehaviour
{
    [Header ("Navigation")]
    [SerializeField]private Button previousButton;
    [SerializeField]private Button nextButton;
    
    [Header ("Play/Buy")]
[SerializeField] private Button play;
[SerializeField] private Button buy;

[Header ("Attributes")]
[SerializeField] private TMP_Text priceText;
[SerializeField] private int[] PlanePrices;

[SerializeField] [Header("Объект текста для монет")]
private TMP_Text moneyText;
private int currentPlane;

private int _money;
[SerializeField] private bool[] planeUnlock = new bool[3] {true, false, false};

private void Awake()
{
    if (PlayerPrefs.GetInt("skin1") == 1)
        planeUnlock[1] = true;
    if (PlayerPrefs.GetInt("skin2") == 1)
        planeUnlock[2] = true;
    SelectPlane(0);
    _money = PlayerPrefs.GetInt("money");
    UpdateText();
}

private void UpdateText()
{
    moneyText.text = _money.ToString();
    PlayerPrefs.SetInt("money", _money);
    PlayerPrefs.Save();
}
public void SelectPlane(int _index)
{
    previousButton.interactable = (_index != 0);
    nextButton.interactable = (_index != transform.childCount-1);
    for (var i = 0; i < transform.childCount; i++)
    {
        transform.GetChild(i).gameObject.SetActive(i == _index);
    }
    currentPlane = _index;
    UpdateUI();
}

private void UpdateUI()
{
    if (planeUnlock[currentPlane])
    {
        buy.gameObject.SetActive(true);
        priceText.text = "куплен";
    }
    else
    {
        buy.gameObject.SetActive(true);
        priceText.text = PlanePrices[currentPlane] + "$";

        buy.interactable = (_money >= PlanePrices[currentPlane]);
    }
}

public void ChangePlane(int _change)
{
    currentPlane += _change;
    SelectPlane(currentPlane);
}

public void BuyPlane()
{
    if (planeUnlock[currentPlane]) return;

    _money -= PlanePrices[currentPlane];
    UpdateText();
    planeUnlock[currentPlane] = true;
    UpdateUI();
    SavePlanes();
}

private void SavePlanes()
{
    for (var i = 0; i <= 2; i++)
    {
        if (planeUnlock[i])
        {
            PlayerPrefs.SetInt("skin" + i, 1);
        }
        else
        {
            PlayerPrefs.SetInt("skin" + i, 0);
        }
    }

    if (planeUnlock[currentPlane] == true)
    {
        PlayerPrefs.SetInt("equip", currentPlane);
    }
}
}