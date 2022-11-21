using System.Collections;
using UnityEngine;

public enum BlockState { Empty = 0, Fill = 1 }

public class BackgroundBlock : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public BlockState BlockState { private set; get; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BlockState = BlockState.Empty;
    }

    /// <summary>
    /// 드래그 블록을 배경 블록에 배치했을 때 배경 블록의 색상을 드래그 블록과 동일하게 설정
    /// </summary>
    public void FillBlock(Color color)
    {
        BlockState = BlockState.Fill;
        spriteRenderer.color = color;
    }

    ///<summary>
    ///줄 완성시 클리어 애니메이션 재생 및 배경 블록 정보 초기화
    /// </summary>
    public void EmptyBlock()
    {
        BlockState = BlockState.Empty;
        StartCoroutine("ScaleTo", Vector3.zero);
    }

    /// <summary>
    /// 0.15초동안 블록 사이즈를 1에서 0으로 축소하도록 재생 및 배경 블록 클리어
    /// </summary>
    private IEnumerator ScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0;
        float percent = 0;
        float time = 0.15f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.localScale = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        spriteRenderer.color = Color.white;
        transform.localScale = Vector3.one;
    }
}
