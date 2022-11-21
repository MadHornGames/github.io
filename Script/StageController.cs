using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private BackgroundBlockSpawner backgroundBlockSpawner;
    [SerializeField]
    private BackgroundBlockSpawner foregroundBlockSpawner;
    [SerializeField]
    private DragBlockSpawner dragBlockSpawner;
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem;
    [SerializeField]
    private UIController uiController;

    public int CurrentScore { private set; get; }  //현 점수
    public int HighScore { private set; get; }  //최고 점수

    private BackgroundBlock[] backgroundBlocks;
    private int currentDragBlockCount;

    private readonly Vector2Int blockCount = new Vector2Int(10, 10);
    private readonly Vector2 blockHalf = new Vector2(0.5f, 0.5f);
    private readonly int maxDragBlockCount = 3;

    private List<BackgroundBlock> filledBlockList;

    private void Awake()
    {
        //현점수 초기화
        CurrentScore = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");

        filledBlockList = new List<BackgroundBlock>();
        
        backgroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        backgroundBlocks = new BackgroundBlock[blockCount.x * blockCount.y];
        backgroundBlocks = foregroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        blockArrangeSystem.Setup(blockCount, blockHalf, backgroundBlocks, this);

        //SpawnDragBlocks();
        StartCoroutine(SpawnDragBlocks());
    }

    private IEnumerator SpawnDragBlocks()
    {
        currentDragBlockCount = maxDragBlockCount;
        dragBlockSpawner.SpawnBlocks();

        yield return new WaitUntil(() => IsCompleteSpawnBlocks());
    }


    /// <summary>
    /// 블록 배치 후처리 코루틴 매소드 호출
    /// </summary>
    public void AfterBlockArrangement(DragBlock block)
    {
        StartCoroutine("OnAfterBlockArrangement", block);
    }
    
    //드래그 블록 생성 및 다른 블록들의 애니메이션이 종료되었는지 검사
    private bool IsCompleteSpawnBlocks()
    {
        int count = 0;
        for(int i = 0; i < dragBlockSpawner.BlockSpawnPoints.Length; ++i)
        {
            if(dragBlockSpawner.BlockSpawnPoints[i].childCount != 0 &&
                   dragBlockSpawner.BlockSpawnPoints[i].GetChild(0).localPosition == Vector3.zero)
            {
                count++;
            }
        }

        return count == dragBlockSpawner.BlockSpawnPoints.Length;
    }


    private IEnumerator OnAfterBlockArrangement(DragBlock block)
    {
        Destroy(block.gameObject);

        int filledLineCount = CheckFilledLine();

        int lineScore = filledLineCount == 0 ? 0 : (int)Mathf.Pow(2, filledLineCount - 1) * 10;
        CurrentScore += block.ChildBlocks.Length + lineScore;

        yield return StartCoroutine(DestroyFilledBlocks(block));  //마지막에 배치한 블록을 기준으로 퍼지듯 삭제

        currentDragBlockCount--;
        if(currentDragBlockCount == 0)
        {
            yield return StartCoroutine(SpawnDragBlocks());
        }

        yield return new WaitForEndOfFrame();

        if (IsGameOver())
        {
            Debug.Log("GameOver");

            if(CurrentScore > HighScore)
            {
                PlayerPrefs.SetInt("HighScore", CurrentScore);
                Debug.Log("NewHighScore");
            }

            uiController.GameOver();
        }
    }

    private int CheckFilledLine()
    {
        int filledLineCount = 0;

        filledBlockList.Clear();

        //가로 줄 검사
        for(int y = 0; y < blockCount.y; ++y)
        {
            int fillBlockCount = 0;
            for(int x = 0; x < blockCount.x; ++x)
            {
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill)
                    fillBlockCount++;
            }

            if(fillBlockCount == blockCount.x)
            {
                for(int x = 0; x < blockCount.x; ++x)
                {
                    filledBlockList.Add(backgroundBlocks[y * blockCount.x + x]);
                }
                filledLineCount++;
            }
        }

        //세로 줄 검사
        for(int x = 0; x < blockCount.x; ++x)
        {
            int fillBlockCount = 0;
            for (int y = 0; y < blockCount.y; ++y)
            {
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill)
                    fillBlockCount++;
            }

            if (fillBlockCount == blockCount.y)
            {
                for (int y = 0; y < blockCount.x; ++y)
                {
                    filledBlockList.Add(backgroundBlocks[y * blockCount.x + x]);
                }
                filledLineCount++;
            }
        }

        return filledLineCount;
    }

    private IEnumerator DestroyFilledBlocks(DragBlock block)
    {
        filledBlockList.Sort((a, b) =>
            (a.transform.position - block.transform.position).sqrMagnitude.
            CompareTo((b.transform.position - block.transform.position).sqrMagnitude));

        for(int i = 0; i < filledBlockList.Count; ++i)
        {
            filledBlockList[i].EmptyBlock();

            yield return new WaitForSeconds(0.01f);
        }

        filledBlockList.Clear();
    }


    private bool IsGameOver()
    {
        int dragBlockCount = 0;

        //배치 가능한 드래그 블록이 남은 경우 검사
        for(int i = 0; i < dragBlockSpawner.BlockSpawnPoints.Length; ++i)
        {
            if(dragBlockSpawner.BlockSpawnPoints[i].childCount != 0)
            {
                dragBlockCount++;

                if(blockArrangeSystem.IsPossibleArrangement
                    (dragBlockSpawner.BlockSpawnPoints[i].GetComponentInChildren<DragBlock>()))
                {
                    return false;  //배치가 가능한 경우 false반환
                }
            }
        }

        return dragBlockCount != 0;
    }
}
