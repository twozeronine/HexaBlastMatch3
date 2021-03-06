using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Logic;
using Match3OriginDataStructure;
using UniRx;
using UnityEngine;
using Util;

public class GameManager
{
    // Model
    public GameTilesModel GameTilesModel { get; set; } = new GameTilesModel();
    public GameBlocksModel GameBlocksModel { get; set; } = new GameBlocksModel();
    public GameTopUIModel GameTopUIModel { get; set; } = new GameTopUIModel();
    
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
    private GameTopUIView gameTopUIView;
    
    public void Init()
    {
        var gameBoardLayout = Managers.Resource.Instantiate("Game/GameBoardLayout");
        gameTilesView = gameBoardLayout.GetComponentInChildren<GameTilesView>();
        gameBlocksView = gameBoardLayout.GetComponentInChildren<GameBlocksView>();
        SubscribeUserDragBlocks();

        gameTopUIView = Managers.UI.ShowSceneUI<GameTopUIView>("GameTopUiCanvas");
    }
    
    // User Drag ??????
    private void SubscribeUserDragBlocks()
    {
        var blockTryMatchBufferStream = GameUtil.GetGameViewSubsystem().DragBlockSubject
            // Stream Hot ??????
            .AsObservable()
            .Publish().RefCount();


        blockTryMatchBufferStream
            // ?????? ??? ?????? ????????? Buffer??? ??????
            .DistinctUntilChanged()
            // ??? ?????? ????????? ?????????
            .Buffer(2)
            // ?????? ???????????? ..
            .Where(selectBlocks => Math.Abs(selectBlocks[0].x - selectBlocks[1].x) + Math.Abs(selectBlocks[0].y - selectBlocks[1].y) == 2)
            // ???????????? ?????? ????????? Complete???.
            .TakeUntil(GameUtil.GetGameViewSubsystem().TapOffSubject.AsObservable())
            // ?????? ?????? ???????????? ?????????. ( DistinctUntilChanged ??? ????????? ?????? ????????? ????????? ?????? ?????? )
            .Repeat()
            // ??? ?????? ?????? ?????? !
            .Subscribe(BlockSwap)
            .AddTo(gameBlocksView);
    }
    
    // ?????? ????????? ??????
    private async void BlockSwap(IList<Vector2Int> selectBlocks)
    {
        var swapBlockPos = selectBlocks.Reverse().ToList();

        
        Managers.Game.ChangeGameState(EGameState.Match3State);
        GameUtil.GetGameViewSubsystem().IsTapOff = false;

        var movableBlocks = new List<MovableBlockView>();
        foreach (var selectBlock in selectBlocks)
        {
            movableBlocks.Add(new MovableBlockView() { BlockPos = selectBlock, TargetPos = swapBlockPos.First(), });
            swapBlockPos.Reverse();
        }

        // View ??????
        foreach (var movableBlock in movableBlocks)
        {
            gameBlocksView.MoveBlock(movableBlock);
        }

        // Match3 ?????? ?????? 
        var scanBlocks = new Blocks()
        {
            BlocksMap = new Dictionary<Vector2Int, Block>(Managers.Game.GameBlocksModel.BlocksProperty.Value
                .BlocksMap)
        };

        // ?????? ??????
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
        
        // ?????? ??? ????????? ?????? ?????? ??????
        if (!removableBlocks.Any())
        {
            // View ??????
            // ?????? ????????? ???????????? ????????????.
            movableBlocks.AddRange(selectBlocks.Select(selectBlock =>
                new MovableBlockView() { BlockPos = selectBlock, TargetPos = selectBlock, }));

            foreach (var movableBlock in movableBlocks)
            {
                gameBlocksView.MoveBlock(movableBlock);
            }

            // ?????? ?????? ?????????
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
        // User State??? ?????? ?????? ?????? ??? ?????? ????????? ??????.
        Managers.Game.GameBlocksModel.BlocksProperty.Value = scanBlocks;
        Debug.Log("????????? ?????? ??????");
        Managers.Game.GameTopUIModel.MoveCount.Value -= 1;
        Managers.Game.ChangeGameState(EGameState.ScanAllBlocksMatch3);
    }
    
     public async UniTaskVoid RequestScanAllBlocksMatch3()
    {
        Managers.Game.ChangeGameState(EGameState.Match3State);

        // Match3 ?????? ?????? 
        var scanBlocks = new Blocks()
        {
            BlocksMap = new Dictionary<Vector2Int, Block>(Managers.Game.GameBlocksModel.BlocksProperty.Value
                .BlocksMap)
        };


        List<MatchedBlock> matchedBlocks;

        // ???????????? ?????? ?????? ????????????
        var isSwap = GameUtil.GetGameModelSubsystem().CurrentSwapMatchedBlocks.Any();
        
        if (isSwap)
        {
            matchedBlocks = GameUtil.GetGameModelSubsystem().CurrentSwapMatchedBlocks.Select(pos =>
                HexaBlastEngine.ScanMatch3(scanBlocks.BlocksMap[pos], scanBlocks)).ToList();
            GameUtil.GetGameModelSubsystem().CurrentSwapMatchedBlocks.Clear();
        }
            
        // ???????????? ????????? ????????? ????????? ???????????????
        else
        {
            matchedBlocks = scanBlocks.BlocksMap.Values.Select(block =>
                HexaBlastEngine.ScanMatch3(scanBlocks.BlocksMap[block.BlockPos], scanBlocks)).ToList();
        }

        var removableBlocks = new List<MovableBlockView>();

        // ????????? ????????? ?????? ?????? ?????? ?????? ????????????.
        /*  Ex ) 
         *                                                            1 <-- ??? ?????? ???????????? ?????? ?????? 5???.
         *                                                            1 <-- ??? ?????? ???????????? ?????? ?????? 4.
         * ??? ?????? ???????????? ?????? ?????? 3 ??? ?????? 5???. ( ?????? ?????? 1?????? ) ---> 1 1 1 <-- ??? ?????? ???????????? ?????? ?????? 3.
         *                                                            1 <-- ??? ?????? ??????????????? ?????? ?????? 4.
         *                                                            1 <-- ??? ?????? ???????????? ?????? ?????? 5???.
         *
         * ??????????????? ?????? ???????????? ?????? ?????? ??? ????????? ?????? ?????? ??????????????? ????????? ???????????? ????????? ??? ????????? ???????????? ????????? ????????? ?????? !!!
         */
        var priorityMatchedBlockQueue = new PriorityQueue<MatchedBlock>();

        foreach (var matchedBlock in matchedBlocks)
        {
            priorityMatchedBlockQueue.Push(matchedBlock);
        }

        while (priorityMatchedBlockQueue.Count > 0)
        {
            // ?????? ????????? ?????? ?????? ??????
            var node = priorityMatchedBlockQueue.Pop();

            // ?????? ????????? ?????? ????????? ????????? ????????? ????????? ?????? node ????????? ????????? ??????
            var isRemoved = (from removeBlock in removableBlocks
                where node.BlockPos == removeBlock.BlockPos
                select removeBlock).Any();

            if (isRemoved) continue;

            removableBlocks.AddRange(HexaBlastEngineUtil.GetMatchedBlocks(scanBlocks, node, isSwap));
        }

        // ?????? ?????? ??? ?????? ?????? ??????
        foreach (var removableBlock in removableBlocks)
        {
            scanBlocks.BlocksMap[removableBlock.BlockPos] = null;
            Managers.Game.GameTilesModel.TilesProperty.Value.TilesMap[removableBlock.BlockPos].ChildBlock = null;
           
            // TopUIView ??????
            Managers.Game.GameTopUIModel.Score.Value += 100;
        }
        
        Debug.Log("????????? ?????? ?????? " + removableBlocks.Count);
        Managers.Game.GameBlocksModel.BlocksProperty.Value = scanBlocks;
        
        // ????????? ????????? ?????????
        if (!removableBlocks.Any())
        {
            // ????????? ????????? ???????????? ????????? ??? ?????? ????????? ?????? ???????????? ??????.
            var doShuffle = HexaBlastEngine.TryCheckCanRemoveBlocks(scanBlocks, out var hintBlock);
            if (!doShuffle)
            {
                
                Managers.Game.ChangeGameState(EGameState.ShuffleBlocks);
                return;
            }
            else
            {
                Managers.Game.ChangeGameState(EGameState.WaitUserInput);
                return;    
            }
        }

        
        await UniTask.Delay(TimeSpan.FromSeconds(Managers.Data.ConstantsTableData.BlankToFullBlockTime));
        Managers.Game.ChangeGameState(EGameState.ScanBlankTileState);
    }
     
     public async UniTaskVoid RequestScanBlankTile()
    {
        Managers.Game.ChangeGameState(EGameState.Match3State);

        var blankTiles = new List<Tile>();
        var tiles = Managers.Game.GameTilesModel.TilesProperty.Value;
        blankTiles = (from tile in tiles.TilesMap.Values
            where tile.IsValid && tile.ChildBlock == null
            select tile).ToList();

        var spawnNewBlocksQueue = new Queue<Block>();
        var spawnNewBlocksViewQueue = new Queue<Block>();
        
        var prevBlocks = new Blocks()
        {
            BlocksMap = new Dictionary<Vector2Int, Block>(Managers.Game.GameBlocksModel.BlocksProperty.Value
                .BlocksMap)
        };
        
        var scanBlocks = new Blocks()
        {
            BlocksMap = new Dictionary<Vector2Int, Block>(prevBlocks.BlocksMap),
        };
        
        var scanTiles = new Tiles()
        {
            TilesMap = new Dictionary<Vector2Int, Tile>(Managers.Game.GameTilesModel.TilesProperty.Value.TilesMap),
        };
        
        foreach (var blankTile in blankTiles)
        {
            spawnNewBlocksQueue.Enqueue(HexaBlastEngineUtil.GetRandomSpawnBlock(blankTile.TilePos));
        }
        
        var movableViewBlocks = new List<MovableBlockView>();

        do
        {
            blankTiles = (from tile in scanTiles.TilesMap.Values
                where tile.IsValid && tile.ChildBlock == null
                select tile).ToList();

            if(!blankTiles.Any()) break;
            
            
            // ????????? ??????
            List<Block> canMoveToBottomBlocks;
            do
            {
                movableViewBlocks.Clear();
                canMoveToBottomBlocks = (from block in scanBlocks.BlocksMap.Values
                    where block != null
                    where HexaBlastEngineUtil.ScanEmptyTile(block, scanTiles, Direction.Bottom)
                    select block).ToList();
                
                if(!canMoveToBottomBlocks.Any()) break;

                foreach (var canMoveToBottomBlock in canMoveToBottomBlocks)
                {
                    var bottomPosition =
                        HexaBlastEngineUtil.GetPosition(Direction.Bottom, canMoveToBottomBlock.BlockPos);
                    if (scanBlocks.BlocksMap.ContainsKey(canMoveToBottomBlock.BlockPos))
                        scanBlocks.BlocksMap[canMoveToBottomBlock.BlockPos] = null;
                    scanBlocks.BlocksMap[bottomPosition] = canMoveToBottomBlock;
                    if (scanTiles.TilesMap.ContainsKey(canMoveToBottomBlock.BlockPos))
                        scanTiles.TilesMap[canMoveToBottomBlock.BlockPos].ChildBlock = null;
                    scanTiles.TilesMap[bottomPosition]
                            .ChildBlock
                        = canMoveToBottomBlock;
                    movableViewBlocks.Add(new MovableBlockView()
                    {
                        BlockPos = canMoveToBottomBlock.BlockPos,
                        BlockColor = canMoveToBottomBlock.Color,
                        BlockType = canMoveToBottomBlock.BlockType,
                        TargetPos = bottomPosition,
                        PrevBlockType = canMoveToBottomBlock.BlockType,
                    });
                    scanBlocks.BlocksMap[bottomPosition].BlockPos = bottomPosition;
                }

                foreach (var movableViewBlock in movableViewBlocks)
                {
                    gameBlocksView.TrySpawnBlock(movableViewBlock,
                        Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed);
                }
                await UniTask.Delay(TimeSpan.FromSeconds(Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed));
                Managers.Game.GameBlocksModel.BlocksProperty.Value = new Blocks()
                {
                    BlocksMap = new Dictionary<Vector2Int, Block>(scanBlocks.BlocksMap),
                };
            } while (canMoveToBottomBlocks.Any());

            
            // ???????????? ??????
            List<Block> canMoveToBottomLeftBlocks;
            do
            {
                movableViewBlocks.Clear();
                canMoveToBottomLeftBlocks = (from block in scanBlocks.BlocksMap.Values
                    where block != null
                    where HexaBlastEngineUtil.ScanEmptyTile(block, scanTiles, Direction.BottomLeft)
                    select block).ToList();
                
                if(!canMoveToBottomLeftBlocks.Any()) break;

                foreach (var canMoveToBottomLeftBlock in canMoveToBottomLeftBlocks)
                {
                    var bottomLeftPosition =
                        HexaBlastEngineUtil.GetPosition(Direction.BottomLeft, canMoveToBottomLeftBlock.BlockPos);
                    if (scanBlocks.BlocksMap.ContainsKey(canMoveToBottomLeftBlock.BlockPos))
                        scanBlocks.BlocksMap[canMoveToBottomLeftBlock.BlockPos] = null;
                    scanBlocks.BlocksMap[bottomLeftPosition]
                        = canMoveToBottomLeftBlock;
                    if (scanTiles.TilesMap.ContainsKey(canMoveToBottomLeftBlock.BlockPos))
                        scanTiles.TilesMap[canMoveToBottomLeftBlock.BlockPos].ChildBlock = null;
                    scanTiles.TilesMap[bottomLeftPosition].ChildBlock
                        = canMoveToBottomLeftBlock;
                    movableViewBlocks.Add(new MovableBlockView()
                    {
                        BlockPos = canMoveToBottomLeftBlock.BlockPos,
                        BlockColor = canMoveToBottomLeftBlock.Color,
                        BlockType = canMoveToBottomLeftBlock.BlockType,
                        TargetPos = bottomLeftPosition,
                        PrevBlockType = canMoveToBottomLeftBlock.BlockType,
                    });
                    scanBlocks.BlocksMap[bottomLeftPosition].BlockPos = bottomLeftPosition;
                }

                foreach (var movableViewBlock in movableViewBlocks)
                {
                    gameBlocksView.TrySpawnBlock(movableViewBlock,
                        Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed);
                }
                await UniTask.Delay(TimeSpan.FromSeconds(Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed));
                Managers.Game.GameBlocksModel.BlocksProperty.Value = new Blocks()
                {
                    BlocksMap = new Dictionary<Vector2Int, Block>(scanBlocks.BlocksMap),
                };
            } while (canMoveToBottomLeftBlocks.Any());
            
            // ??????????????? ??????
            List<Block> canMoveToBottomRightBlocks;
            do
            {
                movableViewBlocks.Clear();
                canMoveToBottomRightBlocks = (from block in scanBlocks.BlocksMap.Values
                    where block != null
                    where HexaBlastEngineUtil.ScanEmptyTile(block, scanTiles, Direction.BottomRight)
                    select block).ToList();
                
                if(!canMoveToBottomRightBlocks.Any()) break;

                foreach (var canMoveToBottomRightBlock in canMoveToBottomRightBlocks)
                {
                    var bottomRightPosition =
                        HexaBlastEngineUtil.GetPosition(Direction.BottomRight, canMoveToBottomRightBlock.BlockPos); 
                    if (scanBlocks.BlocksMap.ContainsKey(canMoveToBottomRightBlock.BlockPos))
                        scanBlocks.BlocksMap[canMoveToBottomRightBlock.BlockPos] = null;
                    scanBlocks.BlocksMap[bottomRightPosition] = canMoveToBottomRightBlock;
                    if (scanTiles.TilesMap.ContainsKey(canMoveToBottomRightBlock.BlockPos))
                        scanTiles.TilesMap[canMoveToBottomRightBlock.BlockPos].ChildBlock = null;
                    scanTiles.TilesMap[bottomRightPosition].ChildBlock = canMoveToBottomRightBlock;
                    movableViewBlocks.Add(new MovableBlockView()
                    {
                        BlockPos = canMoveToBottomRightBlock.BlockPos,
                        BlockColor = canMoveToBottomRightBlock.Color,
                        BlockType = canMoveToBottomRightBlock.BlockType,
                        TargetPos = bottomRightPosition,
                        PrevBlockType = canMoveToBottomRightBlock.BlockType,
                    });
                    scanBlocks.BlocksMap[bottomRightPosition].BlockPos = bottomRightPosition;
                }

                foreach (var movableViewBlock in movableViewBlocks)
                {
                    gameBlocksView.TrySpawnBlock(movableViewBlock,
                        Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed);
                }
                await UniTask.Delay(TimeSpan.FromSeconds(Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed));
                Managers.Game.GameBlocksModel.BlocksProperty.Value = new Blocks()
                {
                    BlocksMap = new Dictionary<Vector2Int, Block>(scanBlocks.BlocksMap),
                };
            } while (canMoveToBottomRightBlocks.Any());

            var canRespawnBlockTiles = (from tile in scanTiles.TilesMap.Values
                where tile.CanSpawnBlockTile
                select tile).ToList();

            

            foreach (var canRespawnBlockTile in canRespawnBlockTiles)
            {
                var newSpawnBlock = spawnNewBlocksQueue.Dequeue();
                newSpawnBlock.BlockPos = canRespawnBlockTile.TilePos;
                movableViewBlocks.Add(new MovableBlockView()
                {
                    BlockPos = HexaBlastEngineUtil.GetPosition(Direction.Top, canRespawnBlockTile.TilePos),
                    BlockType = newSpawnBlock.BlockType,
                    BlockColor = newSpawnBlock.Color,
                    TargetPos = newSpawnBlock.BlockPos,
                    PrevBlockType = newSpawnBlock.BlockType,
                });
                
                scanBlocks.BlocksMap[canRespawnBlockTile.TilePos] = newSpawnBlock;
                scanTiles.TilesMap[canRespawnBlockTile.TilePos].ChildBlock = newSpawnBlock;
            }
            
            foreach (var movableViewBlock in movableViewBlocks)
            {
                gameBlocksView.TrySpawnBlock(movableViewBlock,
                    Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(Managers.Data.ConstantsTableData.BlockDefaultMoveSpeed));
            Managers.Game.GameBlocksModel.BlocksProperty.Value = new Blocks()
            {
                BlocksMap = new Dictionary<Vector2Int, Block>(scanBlocks.BlocksMap),
            };
            
        } while (blankTiles.Any());

        Managers.Game.GameBlocksModel.BlocksProperty.Value = scanBlocks;
        Managers.Game.ChangeGameState(EGameState.ScanAllBlocksMatch3);
    }
     
      public async UniTaskVoid RequestShuffleBlocks()
    {
        var completeShuffleBlocks = false;

        var scanBlocks = new Blocks()
        {
            BlocksMap = new Dictionary<Vector2Int, Block>(Managers.Game.GameBlocksModel.BlocksProperty.Value
                .BlocksMap)
        };

        var prevCopyBlocks = scanBlocks.BlocksMap.ToDictionary(block => new Vector2Int(block.Key.x, block.Key.y), block => new Block()
        {
            BlockPos = block.Value.BlockPos,
            BlockType = block.Value.BlockType,
            Color = block.Value.Color,
            IsValid = block.Value.IsValid,
            ParentTile = block.Value.ParentTile,
        });

        var scanTiles = new Tiles()
        {
            TilesMap = new Dictionary<Vector2Int, Tile>(Managers.Game.GameTilesModel.TilesProperty.Value.TilesMap),
        };

        var movableBlocks = new List<MovableBlockView>();
        
        while (!completeShuffleBlocks)
        {
            movableBlocks = HexaBlastEngine.ShuffleAllBlocks(scanBlocks);

            foreach (var movableBlock in movableBlocks)
            {
                
            
                // Model ??????
                scanBlocks.BlocksMap[movableBlock.TargetPos] = new Block()
                {
                    IsValid = prevCopyBlocks[movableBlock.BlockPos].IsValid,
                    Color = prevCopyBlocks[movableBlock.BlockPos].Color,
                    BlockPos = prevCopyBlocks[movableBlock.TargetPos].BlockPos,
                    ParentTile = prevCopyBlocks[movableBlock.BlockPos].ParentTile,
                    BlockType = prevCopyBlocks[movableBlock.BlockPos].BlockType,
                };

                scanTiles.TilesMap[movableBlock.TargetPos].ChildBlock = new Block()
                {
                    IsValid = prevCopyBlocks[movableBlock.BlockPos].IsValid,
                    Color = prevCopyBlocks[movableBlock.BlockPos].Color,
                    BlockPos = prevCopyBlocks[movableBlock.TargetPos].BlockPos,
                    ParentTile = prevCopyBlocks[movableBlock.BlockPos].ParentTile,
                    BlockType = prevCopyBlocks[movableBlock.BlockPos].BlockType,
                };
            }

            var match3BlockList = HexaBlastEngine.ScanInitMatched3Blocks(scanBlocks);
            completeShuffleBlocks = !match3BlockList.Any() && HexaBlastEngine.TryCheckCanRemoveBlocks(scanBlocks,out var hintBlock);
        }

        var defaultBlocksShuffleSpeed = Managers.Data.ConstantsTableData.DefaultBlockShuffleSpeed;
        
        foreach (var movableBlock in movableBlocks)
        {
            // View ??????
            gameBlocksView.MoveBlock(movableBlock,defaultBlocksShuffleSpeed);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(defaultBlocksShuffleSpeed));
        Managers.Game.GameTilesModel.TilesProperty.Value = scanTiles;
        Managers.Game.GameBlocksModel.BlocksProperty.Value = scanBlocks;
        Managers.Game.ChangeGameState(EGameState.WaitUserInput);
    }
}