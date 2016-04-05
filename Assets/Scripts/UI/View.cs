using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class View : MonoBehaviour
{
    [System.Serializable]
    public class ViewLink
    {
        public Button button;
        public View view;
    }

    static int enterTrigger = Animator.StringToHash("Enter");
    static int leaveTrigger = Animator.StringToHash("Leave");
    static int idleState = Animator.StringToHash("Idle");
    static int leftState = Animator.StringToHash("Left");

    public ViewManager viewManager;
    public List<ViewLink> links;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        foreach(var link in links)
        {
            var localView = link.view;
            link.button.onClick.AddListener(() => viewManager.ChangeView(localView));
        }
    }

    public void FadeIn()
    {
        animator.SetTrigger(enterTrigger);
    }

    public void FadeOut()
    {
        animator.SetTrigger(leaveTrigger);
    }

    public bool IsIdle()
    {
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idleState;
    }

    public bool IsLeft()
    {
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == leftState;
    }
}
