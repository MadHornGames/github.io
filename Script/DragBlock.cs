using System.Collections;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curveMovement;  //이동 모션
    [SerializeField]
    private AnimationCurve curveScale;   //크기 모션

    private BlockArrangeSystem blockArrangeSystem;

    private float appearTime = 0.5f;  //등장 시간
    private float returnTime = 0.1f;  //원 위치 돌아가는 시간

    [field: SerializeField]
    public Vector2Int BlockCount { private set; get; }

    public Color Color { private set; get; }
    public Vector3[] ChildBlocks { private set; get; }



    public void Setup(BlockArrangeSystem blockArrangeSystem,Vector3 parentPosition)   //우측 밖에서 블럭이 생성되어 날아오는 부분
    {
        this.blockArrangeSystem = blockArrangeSystem;

        Color = GetComponentInChildren<SpriteRenderer>().color;

        ChildBlocks = new Vector3[transform.childCount];
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i] = transform.GetChild(i).localPosition;
        }

        StartCoroutine(OnMoveTo(parentPosition, appearTime));
    }



    ///<summary>
    /// 오브젝트 클릭시 1회 호출
    /// </summary>
    private void OnMouseDown()
    {
        StopCoroutine("OnScaleTo");
        StartCoroutine("OnScaleTo", Vector3.one);
    }



    /// <summary>
    /// 오브젝트 드레그시 프레임 호출
    /// </summary>
    private void OnMouseDrag()
    {
        Vector3 gap = new Vector3(0, BlockCount.y * 0.5f + 1, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }



    /// <summary>
    /// 오브젝트의 클릭 종료시 1회 호출
    /// </summary>
    private void OnMouseUp()
    {
        float x = Mathf.RoundToInt(transform.position.x - BlockCount.x % 2 * 0.5f) + BlockCount.x % 2 * 0.5f;
        float y = Mathf.RoundToInt(transform.position.y - BlockCount.y % 2 * 0.5f) + BlockCount.y % 2 * 0.5f;

        transform.position = new Vector3(x, y, 0);

        bool isSuccess = blockArrangeSystem.TryArrangementBlock(this);

        if (isSuccess == false)
        {
            StopCoroutine("OnScaleTo");
            StartCoroutine("OnScaleTo", Vector3.one * 0.5f);
            StartCoroutine(OnMoveTo(transform.parent.position, returnTime));
        }
        
        
    }



    /// <summary>
    /// 현 위치에서 end 위치까지 time 시간동안 이동
    /// </summary>
    private IEnumerator OnMoveTo(Vector3 end, float time)
    {
        Vector3 start = transform.position;
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;
            transform.position = Vector3.Lerp(start, end, curveMovement.Evaluate(percent));   //부드러운 움직임

            yield return null;
        }
    }


    /// <summary>
    /// 현 크기에서 end 크기까지 scaleTime 시간동안 확대or축소
    /// </summary>
    private IEnumerator OnScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / returnTime;

            transform.localScale = Vector3.Lerp(start, end, curveScale.Evaluate(percent));

            yield return null;
        }
    }
}
