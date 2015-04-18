using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Logo : MonoBehaviour
{

    [SerializeField]
    private Image FadeImg;

    [SerializeField]
    private float Speed;

    private bool IsFadeIn;

    private void Start ()
    {
        GameManager.Instance.ChangeState(GameManager.States.Logo);
        FadeImg.color = new Color(0,0,0,0);
        IsFadeIn = true;
    }

    private void Update ()
    {
        if(IsFadeIn) {
            FadeIn();
            IsFadeIn = FadeImg.color.a < 0.95f ? true : false;
        }
        else {
            FadeOut();
            if(FadeImg.color.a < 0.2f)
                GameManager.Instance.LoadLevel("Menu");
        }
    }

    private void FadeIn()
    {
        FadeImg.color = Color.Lerp(FadeImg.color, Color.white, Speed * Time.deltaTime);
    }

    private void FadeOut()
    {
        FadeImg.color = Color.Lerp(FadeImg.color, Color.clear, Speed * Time.deltaTime);
    }
}
