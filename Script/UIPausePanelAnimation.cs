using UnityEngine;

public class UIPausePanelAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject imageBackgroundOverlay;
    [SerializeField]
    private Animator animator;

    public void OnAppear()
    {
        imageBackgroundOverlay.SetActive(true);
        gameObject.SetActive(true);

        animator.SetTrigger("onAppear");
    }

    public void OnDisappear()
    {
        animator.SetTrigger("onDisappear");
    }

    //퇴장 애니메이션 재생 후 실행
    public void EndOfDisappear()
    {
        imageBackgroundOverlay.SetActive(false);
        gameObject.SetActive(false);
    }
}
