using UnityEngine;
using System.Collections.Generic;

public class ViewManager : MonoBehaviour
{
    public List<View> views;

    View currentView, nextView;
    bool isChangingViews;

    void Awake()
    {
        if (views == null || views.Count == 0)
        {
            Debug.LogError("No views are defined for ViewManager!");

            enabled = false;
            return;
        }

        foreach(var view in views)
        {
            view.viewManager = this;
            view.gameObject.SetActive(true);
        }

        currentView = views[0];
        currentView.FadeIn();
    }

    public void ChangeView(View next)
    {
        if (currentView == next || next == null || isChangingViews)
            return;

        currentView.FadeOut();
        nextView = next;

        isChangingViews = true;
    }

    void Update()
    {
        if (!isChangingViews)
            return;

        if (!currentView.IsLeft())
            return;

        nextView.FadeIn();
        currentView = nextView;
        isChangingViews = false;
    }
}
