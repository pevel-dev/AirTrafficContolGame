using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Controllers
{
    public class LeaderboardController : MonoBehaviour
    {
        [SerializeField] [Header("Тексты для отображения")]
        private TMP_Text[] textFields;

        [FormerlySerializedAs("_textPlayerName")] [SerializeField] [Header("Текстовое поле для ника")]
        private TMP_InputField textPlayerName;

        [SerializeField] [Header("Текстово поле для отображения результата")]
        private TMP_Text textResult;

        private int[] _results;
        private string[] _playerNames;
        private int _lastResult;
        private bool _saved;

        void Awake()
        {
            _lastResult = PlayerPrefs.GetInt("lastResult", 0);
            textResult.text = _lastResult.ToString();
            
            _results = new int[textFields.Length];
            _playerNames = new string[textFields.Length];
            for (var i = 0; i < textFields.Length; i++)
            {
                _results[i] = PlayerPrefs.GetInt($"result_{i}", -1);
                _playerNames[i] = PlayerPrefs.GetString($"player_{i}", "NoName");
                if (_results[i] != -1)
                    textFields[i].text = $"{_playerNames[i]} - {_results[i]}";
            }
        }

        public void SaveRecord()
        {
            if (_saved)
                return;
            for (var i = 0; i < textFields.Length; i++)
            {
                if (_lastResult > _results[i])
                {
                    var saveResults = new int[textFields.Length];
                    var saveNames = new string[textFields.Length];
                    _results.CopyTo(saveResults, 0);
                    _playerNames.CopyTo(saveNames, 0);
                    // _results[i] = _lastResult;
                    // _playerNames[i] = textPlayerName.text;
                    // SaveResult(i);
                    for (var j = i; j < textFields.Length; j++)
                    {
                        if (i == j)
                        {
                            _results[j] = _lastResult;
                            _playerNames[j] = textPlayerName.text;
                        }
                        else
                        {
                            _results[j] = saveResults[j - 1];
                            _playerNames[j] = saveNames[j - 1];
                        }

                        SaveResult(j);
                    }
                    _saved = true;
                    PlayerPrefs.Save();
                    break;

                }
            }
        }

        private void SaveResult(int i)
        {
            PlayerPrefs.SetInt($"result_{i}", _results[i]);
            PlayerPrefs.SetString($"player_{i}", _playerNames[i]);
            if (_results[i] > -1)
                textFields[i].text = $"{_playerNames[i]} - {_results[i]}";
        }
    }
}