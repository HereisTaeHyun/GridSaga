using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UtilityManager : MonoBehaviour
{
    private AudioSource audioSource;
    public static UtilityManager utility = null;
    void Awake()
    {
        if (utility == null)
        {
            utility = this;
        }
        else if (utility != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

     public Vector2 DirSet(Vector2 move)
    {
        Vector2 moveDir = new Vector2(0, 0);
        if (Mathf.Approximately(move.x, 0) == false || Mathf.Approximately(0, move.y) == false)
        {
            moveDir.Set(move.x, move.y);
            moveDir.Normalize();
        }
        return moveDir;
    }

    // 알파 변경자   
    public IEnumerator ChangeAlpha(Image changeTarget, float targetAlpah, float changeTime)
    {
        Color currentColor = changeTarget.color;
        float startAlpha = currentColor.a;
        float time = 0.0f;

        // 현재 알파가 목표 알파보다 작은 동안 점진적으로 알파 값 변경
        while (time < 1.0f)
        {
            time += Time.deltaTime / changeTime;
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpah, time);
            changeTarget.color = currentColor;
            yield return null;
        }
    }

    public IEnumerator ChangeAlpha(SpriteRenderer changeTarget, float targetAlpah, float changeTime)
    {
        Color currentColor = changeTarget.color;
        float startAlpha = currentColor.a;
        float time = 0.0f;

        // 현재 알파가 목표 알파보다 작은 동안 점진적으로 알파 값 변경
        while (time < 1.0f)
        {
            time += Time.deltaTime / changeTime;
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpah, time);
            changeTarget.color = currentColor;
            yield return null;
        }
    }

    // 깜빡거리기 코루틴
    public IEnumerator Blink(SpriteRenderer spriteRenderer, float blinkTime)
    {
        bool isBlink = false;
        Color color = spriteRenderer.color;

        float maxBlinkTime = 1.0f; // 깜빡이는 총 시간
        float currentBlinkTIme = 0.0f;

        // 데미지를 입으면 깜빡임
        while(currentBlinkTIme < maxBlinkTime)
        {
            // 이전 상태 깜빡이면 되돌리기, 일반이면 깜빡임 반복시켜서 효과 적용
            if(isBlink == true)
            {
                color.a = 0.0f;
                spriteRenderer.color = color;
                isBlink = false;
            }
            else if(isBlink == false)
            {
                color.a = 1.0f;
                spriteRenderer.color = color;
                isBlink = true;
            }
            currentBlinkTIme += blinkTime;
            yield return new WaitForSeconds(blinkTime);
        }
        // 기본 상태로 초기화
        color.a = 1.0f;
        spriteRenderer.color = color;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
