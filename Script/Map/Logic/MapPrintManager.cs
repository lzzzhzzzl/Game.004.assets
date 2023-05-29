using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace Strategy.Map
{
    public class MapPrintManager : Singleton<MapPrintManager>
    {
        [Header("地图碰撞体的填充瓦片，这个不会显示，只是标定碰撞体")]
        public Tile _collisionTile;

        /// <summary>
        /// 生成地图的父级 
        /// </summary>
        private Transform _tilemapGrid;         //瓦片地图的父级
        private Tilemap[] _mapTilemaps;   //瓦片地图的父级中的大地图
        private Tilemap[] _gridProperties;   //瓦片地图的中的组件

        /// <summary>
        /// 随机数字 
        /// </summary>
        private System.Random _prng;
        private bool _isRandom;
        private int _blockHeight;
        private Vector2Int _mapSize;

        private Dictionary<MapComponent, float[,]> _mapComponentList = new Dictionary<MapComponent, float[,]>();
        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }
        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }
        private void OnAfterSceneLoadEvent()
        {
            // 获取地图组建的父级坐标
            _tilemapGrid = GameObject.FindWithTag("Tilemap").transform;

            SetTilemapInMainMap();
        }
        private void SetTilemapInMainMap()
        {
            //获取大地图的Tilemap组件
            _mapTilemaps = new Tilemap[_tilemapGrid.GetChild(0).childCount];
            for (int i = 0; i < _tilemapGrid.GetChild(0).childCount; i++)
                _mapTilemaps[i] = _tilemapGrid.GetChild(0).GetChild(i).GetComponent<Tilemap>();

            //获取地图功能的Tilemap组件
            _gridProperties = new Tilemap[_tilemapGrid.GetChild(1).childCount];
            for (int i = 0; i < _tilemapGrid.GetChild(1).childCount; i++)
                _gridProperties[i] = _tilemapGrid.GetChild(1).GetChild(i).GetComponent<Tilemap>();
        }
        #region Random
        /// <summary>
        /// 地图随机数字初始化
        /// </summary>
        private void CreatRandom(int seed)
        {
            _prng = new System.Random(seed);
            _isRandom = true;
        }

        /// <summary>
        /// 返回一个随机数，需要初始化
        /// </summary>
        private float GetRandom(int start, int end)
        {
            if (_isRandom)
                return _prng.Next(start, end);
            return 0;
        }
        #endregion
        /// <summary>
        /// 绘制随机地图
        /// </summary>
        public IEnumerator PtintRandomMap(Dictionary<MapComponent, float[,]> mapComponentList/*, Dictionary<MapComponent, float[,]> minimapCompentonList*/, MapDetailsData_SO mapDetailsData_SO)
        {
            _mapSize = mapDetailsData_SO.mapDetail._mapSize;
            _blockHeight = mapDetailsData_SO.mapDetail.blockHeight;
            //绘制小地图
            yield return new WaitForFixedUpdate();
            yield return StartCoroutine(PrintSubjectmap(mapComponentList));
            _mapComponentList = SetMapComponentList(mapComponentList);
            SetCollision(_mapComponentList);
        }
        private Dictionary<MapComponent, float[,]> SetMapComponentList(Dictionary<MapComponent, float[,]> mapComponentList)
        {
            Dictionary<MapComponent, float[,]> newComponentList = new Dictionary<MapComponent, float[,]>();
            foreach (var mapComponent in mapComponentList)
            {
                float[,] value = mapComponent.Value;
                foreach (var next in mapComponentList)
                {
                    if (mapComponent.Key == next.Key)
                        continue;

                    float[,] nextValue = next.Value;
                    Vector2Int size = new Vector2Int(nextValue.GetLength(0), nextValue.GetLength(1));
                    for (int x = 0; x < size.x; x++)
                        for (int y = 0; y < size.y; y++)
                        {
                            if (nextValue[x, y] == 1)
                                value[x, y] = 0;
                        }
                }
                newComponentList.Add(mapComponent.Key, value);
            }
            return newComponentList;
        }

        /// <summary>
        /// 绘制大地图，同时会将地图信息输出blockID与地图数组
        /// </summary>
        private IEnumerator PrintSubjectmap(Dictionary<MapComponent, float[,]> mapComponentList)
        {
            yield return null;
            foreach (MapLevel mapLevel in Enum.GetValues(typeof(MapLevel)))
            {
                if (!mapComponentList.ContainsKey((MapComponent)mapLevel))
                    continue;

                string blockID = GridMapManager.Instance.GetBlockID((MapComponent)mapLevel);
                float[,] value = mapComponentList[(MapComponent)mapLevel];
                Vector2Int size = new Vector2Int(value.GetLength(0), value.GetLength(1));

                TileDetail subjectmapTile = GridMapManager.Instance.GetSubjectmapTile(blockID);
                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        if (value[x, y] == 1)
                        {
                            _mapTilemaps[(int)mapLevel].SetTile(new Vector3Int(x, y, 0), subjectmapTile.tile);
                        }
                    }

                float[,] erosionArray = CommandMethod.ErosionForVector2Array(value, 1);
                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        if (erosionArray[x, y] == 1)
                            for (int num = (int)mapLevel - 1; num >= 0; num--)
                                _mapTilemaps[num].SetTile(new Vector3Int(x, y, 0), null);
                    }
            }

            //绘制小地图的元素部分
            foreach (MapElement mapElement in Enum.GetValues(typeof(MapElement)))
            {
                if (!mapComponentList.ContainsKey((MapComponent)mapElement + 2))
                    continue;

                float[,] value = mapComponentList[(MapComponent)mapElement + 2];
                string blockID = GridMapManager.Instance.GetBlockID((MapComponent)mapElement + 2);
                Vector2Int size = new Vector2Int(value.GetLength(0), value.GetLength(1));

                TileDetail subjectmapTile = GridMapManager.Instance.GetSubjectmapTile(blockID);
                if (subjectmapTile != null)
                {
                    for (int x = 0; x < size.x; x++)
                        for (int y = 0; y < size.y; y++)
                        {
                            if (value[x, y] == 1)
                            {
                                _mapTilemaps[2].SetTile(new Vector3Int(x, y, 0), subjectmapTile.tile);
                            }
                        }
                    float[,] mapErisor = CommandMethod.ErosionForVector2Array(value, 1);
                    for (int x = 0; x < size.x; x++)
                        for (int y = 0; y < size.y; y++)
                        {
                            if (mapErisor[x, y] == 1)
                                for (int num = 1; num >= 0; num--)
                                    _mapTilemaps[num].SetTile(new Vector3Int(x, y, 0), null);
                        }
                }
            }
        }
        private void SetCollision(Dictionary<MapComponent, float[,]> mapComponentList)
        {
            foreach (var mapComponent in mapComponentList)
            {
                float[,] value = mapComponent.Value;
                Vector2Int size = new Vector2Int(value.GetLength(0), value.GetLength(1));
                switch (mapComponent.Key)
                {
                    case MapComponent.Water:
                        for (int x = 0; x < size.x; x++)
                            for (int y = 0; y < size.y; y++)
                            {
                                if (value[x, y] == 1)
                                    _gridProperties[0].SetTile(new Vector3Int(x, y, 0), _collisionTile);
                            }
                        break;
                    case MapComponent.Ground:
                        break;
                    case MapComponent.Forset:
                        break;
                    case MapComponent.Town:
                        break;
                    case MapComponent.Lake:
                        for (int x = 0; x < size.x; x++)
                            for (int y = 0; y < size.y; y++)
                            {
                                if (value[x, y] == 1)
                                    _gridProperties[0].SetTile(new Vector3Int(x, y, 0), _collisionTile);
                            }
                        break;
                }
            }
        }

        /// <summary>
        /// 获取当前地图的边界
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetCurrentSceneBounds()
        {
            _mapTilemaps[0].CompressBounds();
            Vector3Int minPos = _mapTilemaps[0].cellBounds.min;
            Vector3Int maxPos = _mapTilemaps[0].cellBounds.max;
            for (int i = 0; i < _tilemapGrid.GetChild(0).childCount; i++)
            {
                _mapTilemaps[i].CompressBounds();
                if (minPos.x > _mapTilemaps[i].cellBounds.min.x || minPos.y > _mapTilemaps[i].cellBounds.min.y)
                    minPos = _mapTilemaps[i].cellBounds.min;
                if (maxPos.x < _mapTilemaps[i].cellBounds.max.x || maxPos.y < _mapTilemaps[i].cellBounds.max.y)
                    maxPos = _mapTilemaps[i].cellBounds.max;
            }

            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(minPos.x, minPos.y);
            points[1] = new Vector2(minPos.x, maxPos.y);
            points[2] = new Vector2(maxPos.x, maxPos.y);
            points[3] = new Vector2(maxPos.x, minPos.y);

            return points;
        }
        /// <summary>
        /// 获取地图组件列表
        /// </summary>
        public Dictionary<MapComponent, float[,]> GetMapDetailDict()
        {
            return _mapComponentList;
        }

        /// <summary>
        /// 绘制建筑内部地图
        /// </summary>
        public void GenerateBuildingMap(List<TilePropertyList> tilepropertiesList)
        {
            int num = 0;
            foreach (var tileProperties in tilepropertiesList)
            {
                if (tileProperties.tileProperties.Count != 0)
                    foreach (TileProperty tileProperty in tileProperties.tileProperties)
                    {
                        TileBase tile = GridMapManager.Instance.GetTile(tileProperty.tileName);
                        if (num < _mapTilemaps.Length)
                            _mapTilemaps[num].SetTile(new Vector3Int((int)tileProperty.position.x, (int)tileProperty.position.y, 0), tile);
                        else
                            _gridProperties[num - _mapTilemaps.Length].SetTile(new Vector3Int((int)tileProperty.position.x, (int)tileProperty.position.y, 0), tile);
                    }
                num++;
            }

        }
    }
}