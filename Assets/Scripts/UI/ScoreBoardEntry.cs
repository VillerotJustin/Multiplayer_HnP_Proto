using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreBoardEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText, killsText, deathsText;

        public void SetData(string playerName, int kills, int deaths)
        {
            nameText.text = playerName;
            killsText.text = kills.ToString();
            deathsText.text = deaths.ToString();
        }
    }
}
