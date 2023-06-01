using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Source.Shop
{
    public class PlaneSelection : MonoBehaviour
    {
        [Header("Navigation")] [SerializeField]
        private Button previousButton;

        [SerializeField] 
        private Button nextButton;

        [Header("Play/Buy")] [SerializeField] 
        private Button play;
        
        [SerializeField] 
        private Button buy;

        [Header("Attributes")] [SerializeField]
        private TMP_Text priceText;

        [FormerlySerializedAs("PlanePrices")] [SerializeField] 
        private int[] planePrices;

        [SerializeField] [Header("Объект текста для монет")]
        private TMP_Text moneyText;
        
        [SerializeField] 
        private bool[] planeUnlock = { true, false, false };

        private int _currentPlane;
        private int _equipedPlane;

        private int _money;
        private void Awake()
        {
            for (var i = 1; i <= 2; i++)
            {
                if (PlayerPrefs.GetInt($"skin{i}", 1) == 1)
                {
                    planeUnlock[i] = true;
                    if (PlayerPrefs.GetInt("equipedPlane", i) == 1)
                    {
                        _equipedPlane = i;
                        PlayerPrefs.SetInt("equipedPlane", i);
                    }
                }
            }
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

        private void SelectPlane(int index)
        {
            previousButton.interactable = index != 0;
            nextButton.interactable = index != transform.childCount - 1;
            for (var i = 0; i < transform.childCount; i++) 
                transform.GetChild(i).gameObject.SetActive(i == index);

            _currentPlane = index;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (planeUnlock[_currentPlane])
            {
                buy.gameObject.SetActive(true);
                if (_equipedPlane == _currentPlane)
                    priceText.text = "выбран";
                else
                    priceText.text = "куплен";
            }
            else
            {
                buy.gameObject.SetActive(true);
                priceText.text = planePrices[_currentPlane] + "$";
                buy.interactable = _money >= planePrices[_currentPlane];
            }
        }

        public void ChangePlane(int change)
        {
            _currentPlane += change;
            SelectPlane(_currentPlane);
        }

        public void BuyPlane()
        {
            if (planeUnlock[_currentPlane])
                return;

            _money -= planePrices[_currentPlane];
            UpdateText();
            planeUnlock[_currentPlane] = true;
            UpdateUI();
            SavePlanes();
        }

        public void EquipPlane()
        {
            if (planeUnlock[_currentPlane] && _equipedPlane != _currentPlane)
            {
                _equipedPlane = _currentPlane;
                PlayerPrefs.SetInt("equipedPlane", _currentPlane);
                priceText.text = "выбран";
            }
        }

        private void SavePlanes()
        {
            for (var i = 0; i <= 2; i++)
                PlayerPrefs.SetInt($"skin{i}", planeUnlock[i] ? 1 : 0);

            if (planeUnlock[_currentPlane]) 
                PlayerPrefs.SetInt("equipedPlane", _currentPlane);
        }
    }
}