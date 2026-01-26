using PurrNet;
using UnityEngine;

namespace UI
{
    public class MainGameView : GameView
    {
        [SerializeField] private TMPro.TMP_Text healthText;

        private void Awake()
        {
            InstanceHandler.RegisterInstance(this);
        }

        private void OnDestroy()
        {
            InstanceHandler.UnregisterInstance<MainGameView>();
        }

        public override void OnShow()
        {
            // Logic to execute when the main game view is shown
            Debug.Log("Main Game View Shown");
        }

        public override void OnHide()
        {
            // Logic to execute when the main game view is hidden
            Debug.Log("Main Game View Hidden");
        }

        public void UpdateHealthDisplay(int health)
        {
            healthText.text = $"{health}";
        }
    }
}
