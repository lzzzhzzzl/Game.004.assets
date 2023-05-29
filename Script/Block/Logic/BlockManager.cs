using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace Strategy.Map
{
    public class BlockManager : Singleton<BlockManager>
    {
        [Header("区块信息对照表")]
        public BlockDetailData_SO _blockDetailData_SO;
        private MapComponent[,] _mapComponents;
        private Dictionary<string, List<Block>> _sceneBlockDict = new Dictionary<string, List<Block>>();

        /// <summary>
        /// 为该场景构建区块列表
        /// </summary>
        public bool buildBlock(Dictionary<MapComponent, float[,]> mapComponentList, string sceneName, Vector2Int size)
        {
            List<Block> blockList = new List<Block>();
            _mapComponents = new MapComponent[size.x, size.y];
            foreach (var mapComponent in mapComponentList)
            {
                //       Debug.Log(mapComponent.Key);
                // 分配地图组件数组
                float[,] value = mapComponent.Value;
                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        if (value[x, y] == 1)
                            _mapComponents[x, y] = mapComponent.Key;
                    }

                // 区块初始化与赋值
                string blockID = GridMapManager.Instance.GetBlockID(mapComponent.Key);
                BlockDetail blockDetail = _blockDetailData_SO.blockDetailList.Find(i => i.blockId == blockID);
                Block newBlock = new Block(blockDetail, mapComponent.Value);
                blockList.Add(newBlock);
            }
            if (blockList.Count != 0)
                _sceneBlockDict.Add(sceneName, blockList);
            return true;
        }

        /// <summary>
        /// 构建区块信息，也就是生成全部物品
        /// </summary>
        private void BuildBlock(Block block)
        {
            BlockDetail blockDetail = block.blockDetail;
            float[,] blockRange = block.blockRange;

            int quantity = block.quantity;
            if (blockDetail.itemsID.Length == 0)
                return;

            int IDnum = 0;
            //BLOCKTAG: 区块加载时使用，如果有额外的区块类型，则需要在这里添加
            switch (blockDetail.blockType)
            {
                case BlockType.Floor:
                    float[,] landspaceRange = CommandMethod.ErosionForVector2Array(blockRange, 1);

                    List<Vector3> regionList = new List<Vector3>();
                    for (int x = 0; x < blockRange.GetLength(0); x++)
                        for (int y = 0; y < blockRange.GetLength(1); y++)
                            if (landspaceRange[x, y] == 1)
                                regionList.Add(new Vector3(x, y, 0));

                    for (int i = 0; i < quantity; i++)
                    {
                        IDnum = Random.Range(0, blockDetail.itemsID.Length);
                        int region = Random.Range(0, regionList.Count - 1);
                        EventHandler.CallGenerateInventoryItem(blockDetail.itemsID[IDnum], regionList[region], blockDetail.blockId);
                        regionList.RemoveAt(region);
                    }
                    break;
                case BlockType.Spawn:
                    IDnum = Random.Range(0, blockDetail.itemsID.Length);
                    Vector3 pos = TransitionManager.Instance.GetSpawnPoint();

                    EventHandler.CallGenerateInventoryItem(blockDetail.itemsID[IDnum], pos, blockDetail.blockId);
                    break;
            }
        }

        /// <summary>
        /// 刷新全部区块的携程,生成地图时使用，这个刷新会按照比例与最小间距在区块内随机生成物体，小心使用
        /// </summary>
        public IEnumerator OnBuildAllBlock(string sceneName)
        {
            List<Block> blockList = new List<Block>();
            if (_sceneBlockDict.TryGetValue(sceneName, out blockList))
                foreach (var block in blockList)
                {
                    BuildBlock(block);
                    yield return null;
                }
        }

        /// <summary>
        /// 为小地图生成物品
        /// </summary>
        public IEnumerator BuildBuildingInsideInventory(string sceneName)
        {
            BuildingInsideDetail buildingInsideDetail = InventoryManager.Instance.GetBuildingInsideDetail(sceneName.Split("+")[0]);
            EventHandler.CallGenerateInventoryItem(buildingInsideDetail.buildingID, Vector3.zero, "null");

            string[] inventoryList = buildingInsideDetail.inventoryList;
            List<SerializableVector3> inventoryPositionList = buildingInsideDetail.inventoryPositionList;
            for (int i = 0; i < inventoryPositionList.Count; i++)
            {
                int IDnum = Random.Range(0, inventoryList.Length);
                EventHandler.CallGenerateInventoryItem(inventoryList[IDnum], inventoryPositionList[i].ToVector3(), "null");
                yield return null;
            }
        }

        /// <summary>
        /// 返回地图层级与元素的对照数组给AnimationManager
        /// </summary>
        public MapComponent[,] GetMapComponents()
        {
            return _mapComponents;
        }
    }
}