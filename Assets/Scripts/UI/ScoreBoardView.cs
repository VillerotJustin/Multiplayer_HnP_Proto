using System.Collections.Generic;
using PurrNet;
using UnityEngine;

namespace UI
{
    public class ScoreBoardView : GameView
    {
        [SerializeField] private Transform scoreboardEntriesParent;
        [SerializeField] private ScoreBoardEntry scoreBoardEntryPrefab;
    
        private GameViewManager _gameViewManager;

        private void Awake()
        {
            InstanceHandler.RegisterInstance(this);
        }

        private void Start()
        {
            _gameViewManager = InstanceHandler.GetInstance<GameViewManager>();
        }

        protected void OnDestroy()
        {
            InstanceHandler.UnregisterInstance<ScoreBoardView>();
        }

        public void setData(Dictionary<PlayerID, ScoreManager.ScoreData> data)
        {
            foreach (Transform child in scoreboardEntriesParent.transform)
            {
                Destroy(child.gameObject);
            }
        
            foreach (var playerScore in data)
            {
                var entry = Instantiate(scoreBoardEntryPrefab, scoreboardEntriesParent);
                entry.SetData(playerScore.Key.id.ToString(), playerScore.Value.kills, playerScore.Value.deaths);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                _gameViewManager.ShowView<ScoreBoardView>(false);
            if (Input.GetKeyUp(KeyCode.Tab))
                _gameViewManager.HideView<ScoreBoardView>();
        }

        public override void OnShow()
        {
        
        }

        public override void OnHide()
        {
        
        }
    }
}
