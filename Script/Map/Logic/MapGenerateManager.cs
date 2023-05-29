using System.Linq;
using System.Data;
using System;
using UnityEngine;
using System.Collections.Generic;
using Strategy.Astar;

namespace Strategy.Map
{

    /// <summary>
    /// 地图构建管理器
    /// </summary>
    public class MapGenerateManager : Singleton<MapGenerateManager>
    {
        /// <summary>
        /// 地图的总信息 
        /// </summary>
        private List<MapBlock> _mapBlockList = new List<MapBlock>();
        /// <summary>
        /// 随机数字 
        /// </summary>
        private System.Random _prng;
        /// <summary>
        /// 是否可以进行随机数获取，没有初始化，不能获取，会报错
        /// </summary>
        private bool _isRandom;
        /// <summary>
        /// 保存一个区域，用于获取岛屿，在遍历时输出
        /// </summary>
        private List<Vector2Int> _areaBase = new List<Vector2Int>();
        private Stack<Vector2Int> dfsPositionList = new Stack<Vector2Int>();
        /// <summary>
        /// 地图的数据表
        /// </summary>
        private float[,] _noiseMap;
        /// <summary>
        /// 地图的尺寸
        /// </summary>
        private Vector2Int _mapSize;
        /// <summary>
        ///  构建地图区块-随机
        /// </summary>
        public Dictionary<MapComponent, float[,]> BuildRandomMap(MapDetailsData_SO mapDetailsData_SO, int seed)
        {
            // 初始化数据
            _mapBlockList.Clear();
            MapDetail mapDetail = mapDetailsData_SO.mapDetail;
            List<MapComponentDetail> mapLevelList = mapDetailsData_SO.mapLevelList;
            _mapSize = mapDetail._mapSize * mapDetail.blockHeight; // 生成地图尺寸

            // 随机数，用于地图种子用于地图生成，根据地图种子生成一个确定的数
            CreatRandom(seed);
            // 偏移量的存储变量
            Vector2 offset = new Vector2();
            // 获取和分配偏移量
            float offsetX = GetRandom(-Settings.seedSize, Settings.seedSize);
            float offsetY = GetRandom(-Settings.seedSize, Settings.seedSize);
            offset = new Vector2(offsetX, offsetY);

            _noiseMap = BuildNoiseMap(mapDetail, offset); // 生成噪音图
            Dictionary<MapComponent, float[,]> noiseMapList = SetNoiseMap(_noiseMap, mapLevelList); // 根据噪音图和地图层级信息表绘制基础地图

            // 放置地图层级
            Dictionary<MapComponent, float[,]> mapComponentList = new Dictionary<MapComponent, float[,]>();
            foreach (MapComponentDetail mapLevel in mapLevelList)
            {
                float[,] mapBase = noiseMapList[mapLevel.mapComponent];
                float[,] map = IdentifyIsland(mapBase, mapLevel.minisize);
                mapComponentList.Add(mapLevel.mapComponent, map);
            }

            Dictionary<MapComponent, float[,]> erosiorComponentList = new Dictionary<MapComponent, float[,]>();
            foreach (var mapComponent in mapComponentList)
            {
                if (mapComponent.Key != MapComponent.Water)
                    erosiorComponentList.Add(mapComponent.Key, CommandMethod.ErosionForVector2Array(mapComponent.Value, 5));
            }

            // 放置地图元素
            List<MapComponentDetail> mapElementList = mapDetailsData_SO.mapElementList;
            foreach (MapComponentDetail mapElement in mapElementList)
            {
                float[,] map = BuildMapGrid(erosiorComponentList, mapElement, offset);
                mapComponentList.Add(mapElement.mapComponent, IdentifyIsland(map, mapElement.minisize));
            }

            return mapComponentList;
        }

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

        /// <summary>
        /// 识别岛屿算法，识别一个连续的岛屿，并返回消除该岛屿后的信息表
        /// </summary>
        private float[,] dfs(int x, int y, float[,] map)
        {
            if (map[x, y] == 0)
                return map;
            dfsPositionList.Push(new Vector2Int(x, y + 1));
            dfsPositionList.Push(new Vector2Int(x, y - 1));
            dfsPositionList.Push(new Vector2Int(x + 1, y));
            dfsPositionList.Push(new Vector2Int(x - 1, y));
            while (true)
            {
                if (dfsPositionList.Count == 0)
                    break;
                Vector2Int pos = dfsPositionList.Pop();
                if (pos.x < 0 || pos.x >= map.GetLength(0) || pos.y < 0 || pos.y >= map.GetLength(1))
                    continue;
                if (map[pos.x, pos.y] == 0)
                    continue;
                if (map[pos.x, pos.y] == 1)
                {
                    map[pos.x, pos.y] = 0;
                    _areaBase.Add(pos);
                }
                dfsPositionList.Push(new Vector2Int(pos.x, pos.y + 1));
                dfsPositionList.Push(new Vector2Int(pos.x, pos.y - 1));
                dfsPositionList.Push(new Vector2Int(pos.x + 1, pos.y));
                dfsPositionList.Push(new Vector2Int(pos.x - 1, pos.y));
            }
            return map;
        }

        /// <summary>
        /// 剔除大小过小的岛屿
        /// </summary> 
        public float[,] IdentifyIsland(float[,] mapBase, int minimunSize = 5)
        {
            List<List<Vector2Int>> areaBaseList = new List<List<Vector2Int>>();
            float[,] map = new float[_mapSize.x, _mapSize.y];
            for (int x = 0; x < _mapSize.x; x++)
                for (int y = 0; y < _mapSize.y; y++)
                    map[x, y] = mapBase[x, y];

            for (int x = 0; x < _mapSize.x; x++)
                for (int y = 0; y < _mapSize.y; y++)
                {
                    if (map[x, y] == 1)
                    {
                        _areaBase = new List<Vector2Int>(); // 这里初始化一个新的实例，该实例会在dfs中被添加岛屿信息
                        map = dfs(x, y, map);
                        if (_areaBase.Count > minimunSize)
                        {
                            areaBaseList.Add(_areaBase);
                        }
                    }
                }

            float[,] outArray = new float[_mapSize.x, _mapSize.y];
            foreach (var areaBase in areaBaseList)
            {
                foreach (Vector2Int pos in areaBase)
                {
                    outArray[pos.x, pos.y] = 1;
                }
            }
            return outArray;
        }

        /// <summary>
        ///  设置基础噪音图, 也是会初始化后续的数据，需要写在开头
        /// </summary>
        private float[,] BuildNoiseMap(MapDetail mapDetail, Vector2 offset)
        {
            //创建噪音数组，保存地图全部坐标的权重值
            float[,] noiseMap = new float[_mapSize.x, _mapSize.y];

            //生成噪音图-基础层
            switch (mapDetail.mapGenerationmode)
            {
                case MapGmodeType.PerlinNoiseIntensify:
                    for (int y = 0; y < _mapSize.y; y++)
                        for (int x = 0; x < _mapSize.x; x++)
                        {
                            float amplitude = 1;
                            float frequency = 1;
                            float noiseHeight = 0;
                            for (int i = 0; i < mapDetail.octaves; i++)
                            {
                                //根据坐标获取噪音值
                                float sampleX = (float)x / mapDetail.scale * frequency + offset.x;
                                float sampleY = (float)y / mapDetail.scale * frequency + offset.y;
                                float perlinValue = (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1f) * mapDetail.weight;
                                noiseHeight += perlinValue * amplitude;

                                amplitude *= mapDetail.persistance;
                                frequency *= mapDetail.lacunarity;
                            }
                            //调整噪音值
                            if (noiseHeight > 1)
                                noiseMap[x, y] = 1.0f;
                            else if (noiseHeight < 0)
                                noiseMap[x, y] = 0.1f;
                            else
                                noiseMap[x, y] = noiseHeight;
                        }
                    break;
                case MapGmodeType.PerlinNoiseStandard:
                    for (int y = 0; y < _mapSize.y; y++)
                        for (int x = 0; x < _mapSize.x; x++)
                        {
                            //根据坐标获取噪音值
                            float sampleX = (float)x / (float)_mapSize.x * mapDetail.scale + offset.x;
                            float sampleY = (float)y / (float)_mapSize.y * mapDetail.scale + offset.y;
                            float noiseHeight = Mathf.PerlinNoise(sampleX, sampleY) * mapDetail.weight;

                            //调整噪音值
                            if (noiseHeight > 1)
                                noiseMap[x, y] = 1.0f;
                            else if (noiseHeight < 0)
                                noiseMap[x, y] = 0.1f;
                            else
                                noiseMap[x, y] = noiseHeight;
                        }
                    break;
                default:
                    break;
            }

            //根据地形信息再地图上生成岛屿
            foreach (Landform landform in mapDetail.landformList)
            {
                switch (landform.shape)
                {
                    case ShapeType.Roundness:
                        for (int x = -landform.length; x < landform.length; x++)
                            for (int y = -landform.length; y < landform.length; y++)
                            {
                                if (Mathf.Sqrt(x * x + y * y) <= landform.length &&
                                   (x + landform.pos.x >= 0 &&
                                    x + landform.pos.x < _mapSize.x) &&
                                   (y + landform.pos.y >= 0 &&
                                    y + landform.pos.y < _mapSize.y))
                                {
                                    if (landform.incersionValue)
                                        noiseMap[x + landform.pos.x, y + landform.pos.y] -=
                                            ((float)(landform.length - Mathf.Sqrt(x * x + y * y)) / (float)landform.length) * landform.weight;
                                    else
                                        noiseMap[x + landform.pos.x, y + landform.pos.y] +=
                                            ((float)(landform.length - Mathf.Sqrt(x * x + y * y)) / (float)landform.length) * landform.weight;
                                }
                            }
                        break;
                    case ShapeType.Square:
                        for (int x = -landform.length; x < landform.length; x++)
                            for (int y = -landform.length; y < landform.length; y++)
                            {
                                if ((x + landform.pos.x >= 0 && x + landform.pos.x < _mapSize.x) && (y + landform.pos.y >= 0 && y + landform.pos.y < _mapSize.y))
                                {
                                    if (landform.incersionValue)
                                        noiseMap[x + landform.pos.x, y + landform.pos.y] -= Mathf.Abs(x) >= Mathf.Abs(y) ? (float)(landform.length - Mathf.Abs(x)) / (float)landform.length * landform.weight : (float)(landform.length - Mathf.Abs(y)) / (float)landform.length * landform.weight;
                                    else
                                        noiseMap[x + landform.pos.x, y + landform.pos.y] += Mathf.Abs(x) >= Mathf.Abs(y) ? (float)(landform.length - Mathf.Abs(x)) / (float)landform.length * landform.weight : (float)(landform.length - Mathf.Abs(y)) / (float)landform.length * landform.weight;
                                }
                            }
                        break;
                    default:
                        break;
                }
            }

            //规整地图地图参数，将噪音图的权重值规范到正确范围
            for (int x = 0; x < _mapSize.x; x++)
                for (int y = 0; y < _mapSize.y; y++)
                {
                    {
                        if (noiseMap[x, y] > 1 && noiseMap[x, y] < 2)
                            noiseMap[x, y] = 1.0f;
                        else if (noiseMap[x, y] < 0)
                            noiseMap[x, y] = .1f;
                    }
                }

            return noiseMap;
        }

        /// <summary>
        /// 根据noiseMap（噪音图）和 网格信息 构建地图，并返回不同层级的列表（分离和整合）
        /// </summary>
        private Dictionary<MapComponent, float[,]> SetNoiseMap(float[,] noiseMap, List<MapComponentDetail> mapLevelGridList)
        {
            Dictionary<MapComponent, float[,]> mapComponentList = new Dictionary<MapComponent, float[,]>();
            // 循环所有的地图瓦片信息，并将 MapElementType.None => 无地图元素，也就是基础的地图层存储到地图信息中。
            foreach (MapComponentDetail mapLevelGrid in mapLevelGridList)
            {
                float[,] map = new float[_mapSize.x, _mapSize.y];   //  地图层级信息-分离表
                for (int y = 0; y < _mapSize.y; y++)
                    for (int x = 0; x < _mapSize.x; x++)
                    {
                        if (mapLevelGrid.range.x <= noiseMap[x, y])
                        {
                            map[x, y] = 1;
                        }
                    }

                float[,] mapDilate = CommandMethod.DilateForVector2Array(map, 3);    // 地图信息表
                for (int y = 0; y < _mapSize.y; y++)
                    for (int x = 0; x < _mapSize.x; x++)
                    {
                        //分配地图层级信息-分离表
                        if (mapDilate[x, y] == 1 && noiseMap[x, y] > mapLevelGrid.range.y)
                        {
                            mapDilate[x, y] = 0;
                        }
                    }

                mapComponentList[mapLevelGrid.mapComponent] = mapDilate;
            }
            return mapComponentList;
        }

        /// <summary>
        /// 生成地图瓦片信息
        /// </summary>
        private float[,] BuildMapGrid(Dictionary<MapComponent, float[,]> erosiorComponentList, MapComponentDetail mapElementDetail, Vector2 offset)
        {
            float[,] map = new float[_mapSize.x, _mapSize.y];
            foreach (var component in erosiorComponentList)
            {
                float[,] value = component.Value;
                for (int x = 0; x < _mapSize.x; x++)
                    for (int y = 0; y < _mapSize.y; y++)
                    {
                        //根据坐标获取噪音值
                        float sampleX = (float)x / (float)_mapSize.x * mapElementDetail.scale + offset.x;
                        float sampleY = (float)y / (float)_mapSize.y * mapElementDetail.scale + offset.y;
                        float noiseHeight = Mathf.PerlinNoise(sampleX, sampleY);

                        if (mapElementDetail.range.x < noiseHeight && noiseHeight < mapElementDetail.range.y && value[x, y] == 1)
                        {
                            map[x, y] = 1;
                        }
                    }
            }
            return map;
        }
    }
}