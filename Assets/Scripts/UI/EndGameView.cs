using System;
using System.Collections;
using PurrNet;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EndGameView : GameView
    {
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private CanvasGroup canvasGroupRef;
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

        private IEnumerator FadeScreen(bool fadeIn)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                canvasGroupRef.alpha = fadeIn ? t / fadeDuration : 1f - t / fadeDuration;
                
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        public override void OnShow()
        {
            
        }

        public override void OnHide()
        {
            
        }
    }
}
