using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;
using Util;

public class GameBlocksView : MonoBehaviour , BaseMVP.IView
{
    private GameBlocksPresenter gameBlocksPresenter;
    private readonly Dictionary<Vector2Int, GameObject> blocksObjects = new Dictionary<Vector2Int, GameObject>();

    private void Start()
    {
        gameBlocksPresenter = new GameBlocksPresenter(this);
        gameBlocksPresenter.CreateModel();
        gameBlocksPresenter.LoadData();
    }
    
    public void SetLayout<T>(T data)
    {
        if(!(data is Blocks blocks)) return;

        foreach (var blocksObject in blocksObjects)
        {
            Managers.Resource.Destroy(blocksObject.Value);
        }

        blocksObjects.Clear();
        
        foreach (var block in blocks.BlocksMap.Values)
        {
            if (block == null) continue;
            var blockObj = Managers.Resource.Instantiate("Game/Block", transform);
            blockObj.GetOrAddComponent<Poolable>();
           
            blocksObjects.Add(new Vector2Int(block.BlockPos.x, block.BlockPos.y), blockObj);
            var blockView = blockObj.GetOrAddComponent<BlockView>();
            blockView.SetLayout(block);
        }
    }

    public bool MoveBlock(MovableBlockView movableBlockView, float moveSpeed = 0)
    {
        if (moveSpeed == 0) moveSpeed = Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed;
        if (!blocksObjects.TryGetValue(movableBlockView.BlockPos, out var movableBlockObj)) return false;
       movableBlockObj.transform.DOLocalMove(GameUtil.ConvertVec3ToHexVet3( new Vector3(movableBlockView.TargetPos.x, movableBlockView.TargetPos.y, 0)),
            moveSpeed).SetEase(Ease.Linear);

        return true;
    }
    
    public GameObject GetInstance() => gameObject;
}
