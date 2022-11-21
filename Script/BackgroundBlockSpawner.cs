using UnityEngine;

public class BackgroundBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;         //배경 블록 프리펩
    [SerializeField]
    private int orderInLayer;           //블록 스폰 순서

    /// <summary>
    /// 매개변수로 받은 blockCount.x * blockCount.y 개수만큼 배경 블록을 생성해서 배치하고,
    /// 생성한 모든 블록을 BackgroundBlock 타입으로 반환
    /// </summary>
    public BackgroundBlock[] SpawnBlocks(Vector2Int blockCount, Vector2 blockHalf)
    {
        BackgroundBlock[] blocks = new BackgroundBlock[blockCount.x * blockCount.y];

        for(int y = 0; y < blockCount.y; ++y)
        {
            for(int x = 0; x < blockCount.x; ++x)
            {
                float px = -blockCount.x * 0.5f + blockHalf.x + x;
                float py = blockCount.y * 0.5f - blockHalf.y - y;
                Vector3 position = new Vector3(px, py, 0);
                GameObject clone = Instantiate(blockPrefab, position, Quaternion.identity, transform);
                clone.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
                blocks[y * blockCount.x + x] = clone.GetComponent<BackgroundBlock>();
            }
        }

        return blocks;
    }
}
