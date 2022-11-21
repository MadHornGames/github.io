using UnityEngine;

public class BlockArrangeSystem : MonoBehaviour
{
	private Vector2Int blockCount;
	private Vector2 blockHalf;
	private BackgroundBlock[] backgroundBlocks;
	private StageController stageController;

	public void Setup(Vector2Int blockCount, Vector2 blockHalf,
		BackgroundBlock[] backgroundBlocks, StageController stageController)
	{
		this.blockCount = blockCount;
		this.blockHalf = blockHalf;
		this.backgroundBlocks = backgroundBlocks;
		this.stageController = stageController;
	}


	/// <summary>
	/// block을 배치할 수 있는지 검사하고, 배치가 가능하면 블록 배치 및 줄 완성과 점수 계산 처리
	/// </summary>
	public bool TryArrangementBlock(DragBlock block)
	{
		for (int i = 0; i < block.ChildBlocks.Length; ++i)
		{
			Vector3 position = block.transform.position + block.ChildBlocks[i];
			if (!IsBlockInsideMap(position))
				return false;
			if (!IsOtherBlockInThisBlock(position))
				return false;
		}

		//블록배치
		for (int i = 0; i < block.ChildBlocks.Length; ++i)
		{
			Vector3 position = block.transform.position + block.ChildBlocks[i];
			backgroundBlocks[PositionToIndex(position)].FillBlock(block.Color);
		}

		//블록배치 후처리
		stageController.AfterBlockArrangement(block);

		return true;
	}



	/// <summary>
	/// position이 배경 블록판의 바깥인지 검사, 밖이면 flase, 안이면 true
	/// </summary>
	private bool IsBlockInsideMap(Vector2 position)
	{
		if (position.x < -blockCount.x * 0.5f + blockHalf.x || position.x > blockCount.x * 0.5f - blockHalf.x ||
			position.y < -blockCount.y * 0.5f + blockHalf.y || position.y > blockCount.y * 0.5f - blockHalf.y)
		{
			return false;
		}
		return true;
	}



	/// <summary>
	/// position 정보를 바탕으로 맵에 배치된 블록의 index를 계산하여 반환
	/// </summary>
	private int PositionToIndex(Vector2 position)
	{
		float x = blockCount.x * 0.5f - blockHalf.x + position.x;
		float y = blockCount.y * 0.5f - blockHalf.y - position.y;

		return (int)(y * blockCount.x + x);
	}



	/// <summary>
	/// 현재 position에 있는 블록이 비어있는지 검사 후 결과 반환
	/// </summary>
	private bool IsOtherBlockInThisBlock(Vector2 position)
	{
		int index = PositionToIndex(position);

		if (backgroundBlocks[index].BlockState == BlockState.Fill)
		{
			return false;
		}

		return true;
	}

	//자식 블록을 포함하여 블록 판에 배치가 가능한지 검사
	public bool IsPossibleArrangement(DragBlock block)
    {
		for (int y = 0; y < blockCount.y; ++y)
        {
			for(int x = 0; x < blockCount.x; ++x)
            {
				int count = 0;
				Vector3 position = new Vector3(-blockCount.x * 0.5f + blockHalf.x + x, blockCount.y * 0.5f - blockHalf.y - y, 0);

				position.x = block.BlockCount.x % 2 == 0 ? position.x + 0.5f : position.x;
				position.y = block.BlockCount.y % 2 == 0 ? position.y + 0.5f : position.y;

				for (int i = 0; i < block.ChildBlocks.Length; ++i)
                {
					Vector3 blockPosition = block.ChildBlocks[i] + position;

					if (!IsBlockInsideMap(blockPosition))
						break;
					if (!IsOtherBlockInThisBlock(blockPosition))
						break;

					count++;
                }

				if(count == block.ChildBlocks.Length)
                {
					return true;
                }
            }
        }

		return false;  //게임 오버
    }
}