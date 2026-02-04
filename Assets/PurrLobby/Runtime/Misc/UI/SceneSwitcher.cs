using PurrNet;
using PurrNet.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PurrLobby
{
    /// <summary>
    /// Switches to the game scene after relay allocation is complete.
    /// Subscribe to LobbyManager.OnAllReady event or call from a button.
    /// </summary>
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [PurrScene, SerializeField] private string nextScene;
        [SerializeField] private bool subscribeToOnAllReady = true;

        private void Start()
        {
            if (subscribeToOnAllReady && lobbyManager != null)
            {
                // Automatically switch scene when all players are ready
                // At this point, SetAllReadyAsync() has already been called by LobbyManager
                lobbyManager.OnAllReady.AddListener(SwitchScene);
            }
        }

        private void OnDestroy()
        {
            if (lobbyManager != null)
            {
                lobbyManager.OnAllReady.RemoveListener(SwitchScene);
            }
        }

        /// <summary>
        /// Call this to switch scenes. Should only be called AFTER relay is allocated.
        /// If subscribeToOnAllReady is true, this is called automatically at the right time.
        /// </summary>
        public void SwitchScene()
        {
            if (string.IsNullOrEmpty(nextScene))
            {
                PurrLogger.LogError("Next scene name is not set!", this);
                return;
            }

            PurrLogger.Log($"Switching to scene: {nextScene}", this);
            
            // Mark lobby as started
            if (lobbyManager != null)
            {
                lobbyManager.SetLobbyStarted();
            }
            
            // Load the game scene - relay should already be allocated at this point
            SceneManager.LoadSceneAsync(nextScene);
        }
    }
}
