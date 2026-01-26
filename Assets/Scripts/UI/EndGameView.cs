using System;
using System.Collections;
using PurrNet;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EndGameView : GameView
    {
        [SerializeField] private TMP_Text winnerText;

        private void Awake()
        {
            InstanceHandler.RegisterInstance(this);
        }

        protected void OnDestroy()
        {
            InstanceHandler.UnregisterInstance<EndGameView>();
        }

        public void SetWinner(PlayerID winner)
        {
            winnerText.text = $"Player {winner.id} wins!";
        }

        public override void OnShow()
        {
            
        }

        public override void OnHide()
        {
            
        }
    }
}
