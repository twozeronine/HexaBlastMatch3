using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Logic;
using UniRx;
using UnityEngine;
using Util;

public class GameManager
{
    // Model
    public GameTilesModel GameTilesModel { get; set; } = new GameTilesModel();
    public GameBlocksModel GameBlocksModel { get; set; } = new GameBlocksModel();
    
    // Presenter
    public GamePresenter GamePresenter { get; set; } = new GamePresenter();

    // SubSystem
    public GameViewSubSystem GameViewSubSystem { get; set; } = new GameViewSubSystem();
    public GameModelSubSystem GameModelSubSystem { get; set; } = new GameModelSubSystem();
    
    // StateMachine
    private StateMachine<EGameState> stateProcessObject = new StateMachine<EGameState>();
    public bool Pause { get; set; } = false;

    public void Init()
    {
        GamePresenter.Init();   
        GameViewSubSystem.Init();
        CreateGameState();
        ChangeGameState(EGameState.WaitUserInput);
    }
    public EGameState CurrentGameState() => stateProcessObject.CurrentState;

    private void CreateGameState()
    {
        stateProcessObject.AddState(EGameState.WaitUserInput, new WaitUserInputState());
        stateProcessObject.AddState(EGameState.Match3State, new Match3State());
        stateProcessObject.AddState(EGameState.WaitUserTapOff, new WaitUserTapOff());
        stateProcessObject.AddState(EGameState.ScanBlankTileState, new ScanBlankTileState());
        stateProcessObject.AddState(EGameState.ScanAllBlocksMatch3, new ScanAllBlocksMatch3State());
        stateProcessObject.AddState(EGameState.ShuffleBlocks, new ShuffleBlocksState());
    }

    public void ChangeGameState(EGameState state)
    {
        stateProcessObject.ChangeState(state);
    }

    public void OnUpdate()
    {
        if (!Pause)
        {
        }

        stateProcessObject.Process();
    }
}
public class GamePresenter
{
    // View
    private GameTilesView gameTilesView;
    private GameBlocksView gameBlocksView;
    
    public void Init()
    {
        var gameBoardLayout = Managers.Resource.Instantiate("Game/GameBoardLayout");
        gameTilesView = gameBoardLayout.GetComponentInChildren<GameTilesView>();
        gameBlocksView = gameBoardLayout.GetComponentInChildren<GameBlocksView>();
        SubscribeUserDragBlocks();
    }
    
    // User Drag 구독
    private void SubscribeUserDragBlocks()
    {
        var blockTryMatchBufferStream = GameUtil.GetGameViewSubsystem().DragBlockSubject
            // Stream Hot 변환
            .AsObservable()
            .Publish().RefCount();


        blockTryMatchBufferStream
            // 다른 두 개의 블럭을 Buffer로 모음
            .DistinctUntilChanged()
            // 두 개를 드래그 했을시
            .Buffer(2)
            // 바로 옆칸일때 ..
            .Where(selectBlocks => Math.Abs(selectBlocks[0].x - selectBlocks[1].x) + Math.Abs(selectBlocks[0].y - selectBlocks[1].y) == 2)
            // 손가락을 때면 스트림 Complete함.
            .TakeUntil(GameUtil.GetGameViewSubsystem().TapOffSubject.AsObservable())
            // 이후 다시 스트림을 구독함. ( DistinctUntilChanged 의 스트림 중복 검사를 초기화 하기 위함 )
            .Repeat()
            // 두 개의 블록 스왑 !
            .Subscribe(BlockSwap)
            .AddTo(gameBlocksView);
    }
    
    // 블럭 옮기는 로직
    private async void BlockSwap(IList<Vector2Int> selectBlocks)
    {
        var swapBlockPos = selectBlocks.Reverse().ToList();

        // View 갱신
        // View 갱신 하는 동안 User State는 입력을 받지 않는 대기 상태로 변환
        Managers.Game.ChangeGameState(EGameState.Match3State);
        GameUtil.GetGameViewSubsystem().IsTapOff = false;

        var movableBlocks = new List<MovableBlockView>();
        foreach (var selectBlock in selectBlocks)
        {
            movableBlocks.Add(new MovableBlockView() { BlockPos = selectBlock, TargetPos = swapBlockPos.First(), });
            swapBlockPos.Reverse();
        }

        foreach (var movableBlock in movableBlocks)
        {
            gameBlocksView.MoveBlock(movableBlock);
        }

        // Match3 검사 로직 
        var scanBlocks = new Blocks()
        {
            BlocksMap = new Dictionary<Vector2Int, Block>(Managers.Game.GameBlocksModel.BlocksProperty.Value
                .BlocksMap)
        };

        // 블럭 교체
        (scanBlocks.BlocksMap[movableBlocks[0].BlockPos].BlockPos,
            scanBlocks.BlocksMap[movableBlocks[0].TargetPos].BlockPos,
            Managers.Game.GameTilesModel.TilesProperty.Value.TilesMap[movableBlocks[0].BlockPos].ChildBlock,
            Managers.Game.GameTilesModel.TilesProperty.Value.TilesMap[movableBlocks[0].TargetPos].ChildBlock) = (
            movableBlocks[0].TargetPos
            , movableBlocks[0].BlockPos,
            scanBlocks.BlocksMap[movableBlocks[0].TargetPos],
            scanBlocks.BlocksMap[movableBlocks[0].BlockPos]);

        (scanBlocks.BlocksMap[movableBlocks[0].BlockPos], scanBlocks.BlocksMap[movableBlocks[0].TargetPos]) = (
            scanBlocks.BlocksMap[movableBlocks[0].TargetPos], scanBlocks.BlocksMap[movableBlocks[0].BlockPos]);

        var swapDirection = GameUtil.GetSwapDirection(movableBlocks[0].BlockPos, movableBlocks[0].TargetPos);

        var matchedBlocks = movableBlocks.Select(movableBlock =>
            HexaBlastEngine.ScanMatch3(scanBlocks.BlocksMap[movableBlock.BlockPos], scanBlocks,movableBlock.TargetPos)).ToList();

        var removableBlocks = new List<MovableBlockView>();

        foreach (var matchedBlock in matchedBlocks)
        {
            removableBlocks.AddRange(HexaBlastEngineUtil.GetMatchedBlocks(scanBlocks, matchedBlock,false,swapDirection));
            if(matchedBlock.IsMatched())
                GameUtil.GetGameModelSubsystem().CurrentSwapMatchedBlocks.Add(matchedBlock.BlockPos);
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed));
        
        // 제거 할 블록이 없을 시에 로직
        if (!removableBlocks.Any())
        {
            // View 갱신
            // 다시 블럭을 제자리로 갖다놓음.
            movableBlocks.AddRange(selectBlocks.Select(selectBlock =>
                new MovableBlockView() { BlockPos = selectBlock, TargetPos = selectBlock, }));

            foreach (var movableBlock in movableBlocks)
            {
                gameBlocksView.MoveBlock(movableBlock);
            }

            // 다시 블럭 제자리
            (scanBlocks.BlocksMap[movableBlocks[0].BlockPos].BlockPos,
                    scanBlocks.BlocksMap[movableBlocks[0].TargetPos].BlockPos) =
                (movableBlocks[0].TargetPos, movableBlocks[0].BlockPos);
            (scanBlocks.BlocksMap[movableBlocks[0].BlockPos], scanBlocks.BlocksMap[movableBlocks[0].TargetPos]) = (
                scanBlocks.BlocksMap[movableBlocks[0].TargetPos], scanBlocks.BlocksMap[movableBlocks[0].BlockPos]);


            await UniTask.Delay(TimeSpan.FromSeconds(Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed));
            Managers.Game.GameBlocksModel.BlocksProperty.Value = new Blocks()
            {
                BlocksMap = new Dictionary<Vector2Int, Block>(Managers.Game.GameBlocksModel.BlocksProperty.Value
                    .BlocksMap)
            };
            Managers.Game.ChangeGameState(EGameState.WaitUserInput);
            return;
        }

        Managers.Game.ChangeGameState(EGameState.WaitUserTapOff);
        // User State를 다시 입력 받을 수 있는 상태로 변경.
        Managers.Game.GameBlocksModel.BlocksProperty.Value = scanBlocks;
        Debug.Log("제거할 블록 있음");
        Managers.Game.ChangeGameState(EGameState.ScanAllBlocksMatch3);
    }
}