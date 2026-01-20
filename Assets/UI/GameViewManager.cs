using System;
using PurrNet;
using UnityEngine;

public class GameViewManager : MonoBehaviour
{
    [SerializeField] private GameView[] gameViews;
    [SerializeField] private GameView defaultView;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        
        foreach (var view in gameViews)
        {
            HideViewInternal(view);
        }

        ShowViewInternal(defaultView);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<GameViewManager>();
    }

    public void ShowView<T>(bool hideOthers = true) where T : GameView
    {
        foreach (var view in gameViews)
        {
            if (view is T)
            {
                ShowViewInternal(view);
            }
            else
            {
                if (hideOthers) {
                    HideViewInternal(view);
                }
            }
        }
    }

    public void HideView<T>() where T : GameView
    {
        foreach (var view in gameViews)
        {
            if (view is T)
            {
                HideViewInternal(view);
            }
        }
    }

    private void ShowViewInternal(GameView view)
    {
        view.canvasGroup.alpha = 1f;
        view.OnShow();
    }

    private void HideViewInternal(GameView view)
    {
        view.canvasGroup.alpha = 0f;
        view.OnHide();
    }
}

public abstract class GameView : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public abstract void OnShow();
    public abstract void OnHide();
}