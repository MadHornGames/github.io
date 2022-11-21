using System.Collections;
using UnityEngine;

public class DragBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem;
    [SerializeField]
    private Transform[] blockSpawnPoints;
    [SerializeField]
    private GameObject[] blockPrefabs;
    [SerializeField]
    private Vector3 spawnGapAmount = new Vector3(10, 0, 0);

    public Transform[] BlockSpawnPoints => blockSpawnPoints;

    //private void Awake()
    public void SpawnBlocks()
    {
        StartCoroutine("OnSpawnBlocks");
    }

    private IEnumerator OnSpawnBlocks()
    {
        for(int i = 0; i < blockSpawnPoints.Length; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            int index = Random.Range(0, blockPrefabs.Length);

            Vector3 spawnPosition = blockSpawnPoints[i].position + spawnGapAmount;
            GameObject clone = Instantiate(blockPrefabs[index], spawnPosition, Quaternion.identity, blockSpawnPoints[i]);

            clone.GetComponent<DragBlock>().Setup(blockArrangeSystem,blockSpawnPoints[i].position);
        }
    }
}
