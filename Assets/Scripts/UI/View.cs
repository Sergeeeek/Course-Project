using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Компонент для выполнения функций окна, таких как появление и переход к другому окну
/// </summary>
/// Требует компонент Animator для анимаций появления и исчезания
[RequireComponent(typeof(Animator))]
public class View : MonoBehaviour
{
    // Ссылка к другому окну
    [System.Serializable]
    public class ViewLink
    {
        [Tooltip("Кнопка при нажатии на которую произойдёт переход")]
        public Button button;
        [Tooltip("Окно к которому перейти")]
        public View view;
    }

    // Hash анимаций появления, исчезания, положения на экране и положения за экраном
    static int enterTrigger = Animator.StringToHash("Enter");
    static int leaveTrigger = Animator.StringToHash("Leave");
    static int idleState = Animator.StringToHash("Idle");
    static int leftState = Animator.StringToHash("Left");

    [Tooltip("Менеджер окон")]
    public ViewManager viewManager;
    [Tooltip("Ссылки к другим окнам")]
    public List<ViewLink> links;

    // локальная ссылка на аниматор
    Animator animator;

    // Функция вызывается когда объект активируется
    void Awake()
    {
        // Получаем ссылку на аниматор
        animator = GetComponent<Animator>();

        foreach(var link in links)
        {
            var localView = link.view;
            // Добавляем функции при нажатие на кнопки
            link.button.onClick.AddListener(() => viewManager.ChangeView(localView));
        }
    }

    // Функция которая играет анимацию появления
    public void FadeIn()
    {
        animator.SetTrigger(enterTrigger);
    }

    // Функция играющая анимацию ухода с экрана
    public void FadeOut()
    {
        animator.SetTrigger(leaveTrigger);
    }

    // Функция возвращающая true если окно на экране
    public bool IsIdle()
    {
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idleState;
    }

    // Функция возвращающая true если окно не на экране
    public bool IsLeft()
    {
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == leftState;
    }
}
