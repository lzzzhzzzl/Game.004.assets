using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



namespace Strategy.Map
{
    public class GridMapManager : Singleton<GridMapManager>, ISaveable
    {
        [Header("区块ID与大地图的瓦片对照表")]
        public TileDetailsData_SO _subjectmapDetailsData_SO;

        [Header("生成地图时使用的数据")]
        public List<MapDetailsData_SO> _mapDetailsDataList;

        [Header("地图总数据列表")]
        public MapData_SO _mapData_SO;

        [Header("地图数据信息")]
        public RuleTile _mainmapSubject;
        public TileBase _mainmapBase;
        public TileBase _mainmapMark;
        public Vector2Int _islandMapSize;
        [Range(0, 1)]
        public float _islandRange;
        [Header("蓝图数据")]
        public TileBase bluePrintBaseTile;
        public TileBase bulePrintChoiceTile;

        [Header("瓦片信息")]
        public TileBaseListData_SO _tileBaseListData_SO;

        private List<GridSaveDetail> _gridsaveDetailDict = new List<GridSaveDetail>();
        private Dictionary<MapComponent, float[,]> _mapDetailDict = new Dictionary<MapComponent, float[,]>();
        private Dictionary<string, SceneNameSaved> sceneNameDict = new Dictionary<string, SceneNameSaved>();
        private Dictionary<Vector3Int, SceneDetail> _sceneDetailDict = new Dictionary<Vector3Int, SceneDetail>();
        private MapDetailsData_SO _currentMapDetailsData_SO;
        private SceneDetail _currentSceneDetail;

        private Transform _gridBuildMapParent;
        private Transform _islandmapParent;
        private Transform _tilemapParent;
        private Transform _boundsTransform;
        private bool[,] _gridmapObstacle;

        private Tilemap[] _islandmapTilemap;
        private Tilemap[] _gridBuildTilemap;
        private Vector2Int _gridDimensions;
        private Vector2Int _gridOrigin;
        private Vector3Int _currentIslandmapPosition;

        private float[,] gridBuildMap;
        private Vector3Int min;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.startGameEvent += OnstartGameEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.OpenBulePrintEvent += OnOpenBulePrintEvent;
            EventHandler.IslandMapClickEvent += OnIslandMapClickEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.AfterSceneDataLoadEvent += OnAfterSceneDataLoadEvent;
            EventHandler.mouseClickBulePrintEvent += OnmouseClickBulePrintEvent;
            EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
            EventHandler.IslandMapInputEvent += OnIslandMapInputEvent;
            EventHandler.RemoveFurnitureEvent += OnRemoveFurnitureEvent;
        }

        private void OnDisable()
        {
            EventHandler.startGameEvent -= OnstartGameEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.OpenBulePrintEvent -= OnOpenBulePrintEvent;
            EventHandler.IslandMapClickEvent -= OnIslandMapClickEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.AfterSceneDataLoadEvent -= OnAfterSceneDataLoadEvent;
            EventHandler.mouseClickBulePrintEvent -= OnmouseClickBulePrintEvent;
            EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
            EventHandler.IslandMapInputEvent -= OnIslandMapInputEvent;
            EventHandler.RemoveFurnitureEvent -= OnRemoveFurnitureEvent;
        }


        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
            _gridBuildMapParent = GameObject.FindWithTag("GridBuildMapParent").transform;
            _islandmapParent = GameObject.FindWithTag("islandmapParent").transform;
            GetIslandmapTilemap();
            GetBuildMapTilemap();
        }

        private void OnAfterSceneLoadEvent()
        {
            _tilemapParent = GameObject.FindWithTag("Tilemap").transform;
            _boundsTransform = GameObject.FindWithTag("BoundConfiner").transform;
        }

        private void OnBeforeSceneLoadEvent()
        {
            string sceneName = TransitionManager.Instance.GetCurrentSceneName();
            SaveMapGrid(sceneName);
        }
        private void OnRemoveFurnitureEvent(Transform furnitureTransform, BluePrintDetail bluePrintDetail)
        {
            Vector3Int position = new Vector3Int((int)furnitureTransform.position.x, (int)furnitureTransform.position.y, 0);
            for (int x = position.x - min.x; x < position.x + bluePrintDetail.size.x - min.x; x++)
                for (int y = position.y - min.y; y < position.y + bluePrintDetail.size.y - min.y; y++)
                {
                    gridBuildMap[x, y] = 0;
                    _gridBuildTilemap[0].SetTile(new Vector3Int(x + min.x, y + min.y, 0), bluePrintBaseTile);
                }
        }

        private void OnmouseClickBulePrintEvent(BluePrintDetail bluePrintDetail, Vector3Int position)
        {
            for (int x = position.x - min.x; x < position.x + bluePrintDetail.size.x - min.x; x++)
                for (int y = position.y - min.y; y < position.y + bluePrintDetail.size.y - min.y; y++)
                {
                    gridBuildMap[x, y] = 1;
                    _gridBuildTilemap[0].SetTile(new Vector3Int(x + min.x, y + min.y, 0), bulePrintChoiceTile);
                }
        }

        public bool CheckGridBuildMap(Vector3Int position, Vector3Int size)
        {
            if (position.x - 1 - min.x > gridBuildMap.GetLength(0) || position.y - 1 - min.y > gridBuildMap.GetLength(1))
                return false;

            bool canPrint = true;
            _gridBuildTilemap[1].ClearAllTiles();

            for (int x = position.x - min.x; x < position.x + size.x - min.x; x++)
                for (int y = position.y - min.y; y < position.y + size.y - min.y; y++)
                {
                    if (gridBuildMap[x, y] == 0)
                    {
                        _gridBuildTilemap[1].SetTile(new Vector3Int(x + min.x, y + min.y, 0), bulePrintChoiceTile);
                    }
                    else if (gridBuildMap[x, y] == 1)
                        canPrint = false;
                }

            if (!canPrint)
                return false;
            return true;
        }
        private void GetBuildMapTilemap()
        {
            _gridBuildTilemap = new Tilemap[_gridBuildMapParent.GetChild(0).childCount];
            for (int i = 0; i < _gridBuildMapParent.GetChild(0).childCount; i++)
                _gridBuildTilemap[i] = _gridBuildMapParent.GetChild(0).GetChild(i).GetComponent<Tilemap>();
        }
        private void ClearGridBuildMap()
        {
            _gridBuildTilemap[0].ClearAllTiles();
            _gridBuildTilemap[1].ClearAllTiles();
        }
        private void OnOpenBulePrintEvent(bool isOpenBluePrint)
        {
            _gridBuildMapParent.GetChild(0).GetChild(0).gameObject.SetActive(isOpenBluePrint);
            _gridBuildMapParent.GetChild(0).GetChild(1).gameObject.SetActive(isOpenBluePrint);
        }

        private void SetBuildMap()
        {
            _gridBuildTilemap[2].CompressBounds();
            Vector3Int size = _gridBuildTilemap[2].cellBounds.size;
            Vector3Int max = _gridBuildTilemap[2].cellBounds.max;
            min = _gridBuildTilemap[2].cellBounds.min;
            gridBuildMap = new float[size.x, size.y];

            for (int x = min.x; x < max.x; x++)
                for (int y = min.y; y < max.y; y++)
                {
                    TileBase tile = _gridBuildTilemap[2].GetTile(new Vector3Int(x, y, 0));
                    if (tile != null)
                    {
                        gridBuildMap[x - min.x, y - min.y] = 1;
                    }
                }
        }
        private void PrintBuildMap()
        {
            _gridBuildTilemap[2].CompressBounds();
            Vector3Int size = _gridBuildTilemap[2].cellBounds.size;
            Vector3Int max = _gridBuildTilemap[2].cellBounds.max;
            min = _gridBuildTilemap[2].cellBounds.min;

            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {
                    if (gridBuildMap[x, y] == 1)
                        _gridBuildTilemap[0].SetTile(new Vector3Int(x + min.x, y + min.y, 0), bulePrintChoiceTile);
                    else if (gridBuildMap[x, y] == 0)
                        _gridBuildTilemap[0].SetTile(new Vector3Int(x + min.x, y + min.y, 0), bluePrintBaseTile);
                }
        }
        private void OnIslandMapClickEvent(bool isClick)
        {
            _islandmapParent.gameObject.SetActive(isClick);
        }

        private void OnAfterSceneDataLoadEvent()
        {
            SetGridmapObstacle();
            EventHandler.CallSetMapObstacleToCharacter();
        }


        private void OnstartGameEvent()
        {
            if (_currentSceneDetail != null)
                EventHandler.CallTransitionInIslandEvent(_currentSceneDetail);
        }
        public SceneDetail GetSceneDetail(SceneNameSaved sceneNameSaved)
        {
            SceneDetail sceneDetail = new SceneDetail();
            if (_mapData_SO.mapDataList.Find(i => i.mapID == sceneNameSaved.sceneName) != null)
            {
                MapData mapData = _mapData_SO.mapDataList.Find(i => i.mapID == sceneNameSaved.sceneName);
                sceneDetail = new SceneDetail(sceneNameSaved.seed,
                    mapData.mapID,
                    mapData.fromScene,
                    mapData.sprite,
                    mapData.startPos,
                    mapData.isRandomMap);
            }
            return sceneDetail;
        }
        private void GetIslandmapTilemap()
        {
            _islandmapTilemap = new Tilemap[_islandmapParent.GetChild(0).childCount];
            for (int i = 0; i < _islandmapParent.GetChild(0).childCount; i++)
            {
                _islandmapTilemap[i] = _islandmapParent.GetChild(0).GetChild(i).GetComponent<Tilemap>();
            }
        }
        private void ClearIslandmapTilemap()
        {
            for (int i = 1; i < _islandmapParent.GetChild(0).childCount; i++)
            {
                _islandmapTilemap[i].ClearAllTiles();
            }
        }
        private void SetIslandList(Vector2Int islandMapSize, float islandRange)
        {
            List<Vector3Int> regionList = new List<Vector3Int>();
            for (int x = 1; x < islandMapSize.x - 1; x++)
                for (int y = 1; y < islandMapSize.y - 1; y++)
                    regionList.Add(new Vector3Int(x + 1, y + 1, 0));


            int campNum = ((islandMapSize.x - 2) / 2) + ((islandMapSize.y - 2) / 2) * (islandMapSize.x - 2);
            Vector3Int position = regionList[campNum];
            foreach (MapData mapData in _mapData_SO.mapDataList)
            {
                if (mapData.isCamp)
                {
                    SceneNameSaved sceneNameSaved = new SceneNameSaved(0, mapData.mapID);
                    sceneNameDict.Add(position.x + "+" + position.y, sceneNameSaved);
                    regionList.RemoveAt(campNum);
                    break;
                }
            }

            int region;
            int islandNumber;
            int mapIDListLength = _mapData_SO.mapDataList.Count - 1;
            int length = (int)((islandMapSize.x) * (islandMapSize.y) * islandRange);
            int seed = Random.Range(0, 10000);
            System.Random _prng = new System.Random(seed);
            for (int i = 0; i < length; i++)
            {
                islandNumber = Random.Range(0, mapIDListLength);
                region = Random.Range(0, regionList.Count);


                SceneNameSaved sceneNameSaved = new SceneNameSaved(_prng.Next(0, 10000), _mapData_SO.mapDataList[islandNumber].mapID);
                sceneNameDict.Add(regionList[region].x + "+" + regionList[region].y, sceneNameSaved);
                regionList.RemoveAt(region);
            }
        }
        private void SetSceneDetail()
        {
            foreach (var sceneName in sceneNameDict)
            {
                if (_mapData_SO.mapDataList.Find(i => i.mapID == sceneName.Value.sceneName) != null)
                {
                    MapData mapData = _mapData_SO.mapDataList.Find(i => i.mapID == sceneName.Value.sceneName);
                    SceneDetail islandDetail = new SceneDetail(sceneName.Value.seed,
                    mapData.mapID,
                    mapData.fromScene,
                    mapData.sprite,
                    mapData.startPos,
                    mapData.isRandomMap);
                    string[] pos = sceneName.Key.Split("+");
                    _sceneDetailDict.Add(new Vector3Int(int.Parse(pos[0]), int.Parse(pos[1]), 0), islandDetail);
                }
            }
        }
        private void PrintIslandList(Vector2Int islandMapSize, float islandRange)
        {

            for (int x = 0; x < islandMapSize.x + 2; x++)
                for (int y = 0; y < islandMapSize.y + 2; y++)
                {
                    if (x != 0 && x != islandMapSize.x + 1 && y != 0 && y != islandMapSize.y + 1)
                        _islandmapTilemap[1].SetTile(new Vector3Int(x, y, 0), _mainmapSubject);
                }

            foreach (var islandID in _sceneDetailDict)
            {
                if (_mapData_SO.mapDataList.Find(i => i.mapID == islandID.Value.mapID) != null)
                {
                    MapData mapData = _mapData_SO.mapDataList.Find(i => i.mapID == islandID.Value.mapID);
                    _islandmapTilemap[2].SetTile(islandID.Key, mapData.mapTile);
                }
            }
        }
        private void OnIslandMapInputEvent(Vector3Int position)
        {
            if (_sceneDetailDict.ContainsKey(position))
            {
                SceneDetail value = _sceneDetailDict[position];
                if (_mapData_SO.mapDataList.Find(i => i.mapID == value.mapID) != null)
                {
                    _islandmapTilemap[3].SetTile(_currentIslandmapPosition, null);
                    _islandmapTilemap[3].SetTile(position, _mainmapMark);
                    _currentIslandmapPosition = position;

                    MapData mapData = _mapData_SO.mapDataList.Find(i => i.mapID == value.mapID);
                    EventHandler.CallSetMapDetailUIEvent(mapData.sprite, mapData.describe);
                    if (mapData.isRandomMap && _mapDetailsDataList.Find(i => i.mapID == value.mapID) != null)
                    {
                        Debug.Log("已选择地图模板:" + value.mapID + "  地图场景:" + value.targetScene + "  地图种子:" + value.seed);
                        _currentMapDetailsData_SO = _mapDetailsDataList.Find(i => i.mapID == value.mapID);
                        _currentSceneDetail = value;
                    }
                    else
                    {
                        Debug.Log("已选择地图模板:" + value.mapID + "  地图场景:" + value.targetScene + "  地图种子:" + value.seed);
                        _currentSceneDetail = value;
                    }
                }
            }
        }

        /// 返回根据区块返回该区块对应的大地图信息，可能为空
        /// </summary>
        public TileDetail GetSubjectmapTile(string blockID)
        {
            return _subjectmapDetailsData_SO.tileDetails.Find(i => i.blockID == blockID);
        }

        /// <summary>
        /// 获取一个地图层级或元素信息，用于判定区块
        /// </summary>
        public MapComponent GetMapComponent(string blockID)
        {
            if (_currentMapDetailsData_SO.mapLevelList.Find(i => i.blockID == blockID) != null)
            {
                return _currentMapDetailsData_SO.mapLevelList.Find(i => i.blockID == blockID).mapComponent;
            }
            if (_currentMapDetailsData_SO.mapElementList.Find(i => i.blockID == blockID) != null)
            {
                return _currentMapDetailsData_SO.mapElementList.Find(i => i.blockID == blockID).mapComponent;
            }
            return MapComponent.Water;

        }

        public bool GetMpaObstacle(Vector2Int pos)
        {
            if (_gridmapObstacle[pos.x, pos.y])
                return true;
            return false;
        }

        /// <summary>
        /// 获取该地图的信息
        /// </summary>
        public MapDetail GetMapDetail()
        {
            return _currentMapDetailsData_SO.mapDetail;
        }

        /// <summary>
        /// 根据瓦片的名称获取瓦片
        /// </summary>
        public TileBase GetTile(string tileName)
        {
            if (_tileBaseListData_SO.tileBaseDataList.Find(i => i.name == tileName) != null)
                return _tileBaseListData_SO.tileBaseDataList.Find(i => i.name == tileName);
            else
                return null;
        }

        /// <summary>
        ///设置地图相机的边界
        /// </summary>
        public void SetCurrentSceneBounds()
        {
            Vector2[] points = MapPrintManager.Instance.GetCurrentSceneBounds();
            _boundsTransform.GetComponent<PolygonCollider2D>().SetPath(0, points);
        }

        /// <summary>
        /// 保存MainMap的瓦片信息
        /// </summary>
        public void SaveMapGrid(string sceneName)
        {
            GridSaveDetail gridSaveDetail = new GridSaveDetail();
            //获取大地图的Tilemap组件
            for (int i = 0; i < _tilemapParent.GetChild(0).childCount; i++)
            {
                string key = _tilemapParent.GetChild(0).name + "-" + _tilemapParent.GetChild(0).GetChild(i).name;
                TilePropertyList tilePropertyList = SaveTilemap(_tilemapParent.GetChild(0).GetChild(i).GetComponent<Tilemap>());
                gridSaveDetail.tilemaps.Add(key, tilePropertyList);
            }

            //获取大地图的功能组件
            for (int i = 0; i < _tilemapParent.GetChild(1).childCount; i++)
            {
                string key = _tilemapParent.GetChild(1).name + "-" + _tilemapParent.GetChild(1).GetChild(i).name;
                TilePropertyList tilePropertyList = SaveTilemap(_tilemapParent.GetChild(1).GetChild(i).GetComponent<Tilemap>());
                gridSaveDetail.tilemaps.Add(key, tilePropertyList);
            }

            gridSaveDetail.sceneName = sceneName;
            _gridsaveDetailDict.Add(gridSaveDetail);
            Debug.Log("------场景:" + sceneName + "   瓦片保存完毕------");
        }


        /// <summary>
        /// 保存瓦片地图
        /// </summary>
        public TilePropertyList SaveTilemap(Tilemap tilemap)
        {
            TilePropertyList tilePropertyList = new TilePropertyList();
            tilemap.CompressBounds();

            Vector3Int startPos = tilemap.cellBounds.min;
            Vector3Int endPos = tilemap.cellBounds.max;
            for (int x = startPos.x; x < endPos.x; x++)
                for (int y = startPos.y; y < endPos.y; y++)
                {
                    TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                    if (tile != null)
                    {
                        tilePropertyList.tileProperties.Add(new TileProperty(new SerializableVector3(x, y, 0), tile.name));
                    }
                    else
                        tilePropertyList.tileProperties.Add(new TileProperty(new SerializableVector3(x, y, 0), "null"));

                }

            return tilePropertyList;
        }

        /// <summary>
        /// 加载MainMap的瓦片地图的信息
        /// </summary>
        public IEnumerator LoadMainMapGrid(string sceneName)
        {
            TilePropertyList tilePropertyList;
            Dictionary<string, TilePropertyList> tilemaps = _gridsaveDetailDict.Find(i => i.sceneName == sceneName).tilemaps;

            //获取大地图的Tilemap组件
            for (int i = 0; i < _tilemapParent.GetChild(1).childCount; i++)
            {
                string key = _tilemapParent.GetChild(1).name + "-" + _tilemapParent.GetChild(1).GetChild(i).name;
                if (tilemaps.TryGetValue(key, out tilePropertyList))
                    yield return LoadTilemap(_tilemapParent.GetChild(1).GetChild(i).GetComponent<Tilemap>(), tilePropertyList);
            }


            //获取大地图边界的Tilemap组件
            for (int i = 0; i < _tilemapParent.GetChild(0).childCount; i++)
            {
                string key = _tilemapParent.GetChild(0).name + "-" + _tilemapParent.GetChild(0).GetChild(i).name;
                if (tilemaps.TryGetValue(key, out tilePropertyList))
                    yield return LoadTilemap(_tilemapParent.GetChild(0).GetChild(i).GetComponent<Tilemap>(), tilePropertyList);
            }

            Debug.Log("------场景:" + sceneName + "   瓦片加载完毕------");
        }

        /// <summary>
        /// 加载瓦片地图
        /// </summary>
        public IEnumerator LoadTilemap(Tilemap tilemap, TilePropertyList tilePropertyList)
        {
            List<TileProperty> tileProperties = tilePropertyList.tileProperties;
            foreach (TileProperty tilePropertie in tileProperties)
            {
                if (tilePropertie.tileName != "null")
                {
                    Vector3Int pos = tilePropertie.position.ToVector3Int();
                    tilemap.SetTile(pos, GetTile(tilePropertie.tileName));
                }
            }
            yield return null;
        }

        /// <summary>
        /// 生成MainMap地图
        /// </summary>
        public IEnumerator GenerateMainMap(string sceneName, int seed)
        {
            if (_gridsaveDetailDict.Find(i => i.sceneName == sceneName) == null)
            {
                Dictionary<MapComponent, float[,]> mapCompentonList = MapGenerateManager.Instance.BuildRandomMap(_currentMapDetailsData_SO, seed);
                yield return new WaitForFixedUpdate();

                yield return StartCoroutine(MapPrintManager.Instance.PtintRandomMap(mapCompentonList, _currentMapDetailsData_SO));
                yield return _mapDetailDict = MapPrintManager.Instance.GetMapDetailDict();

                Vector2Int size = _currentMapDetailsData_SO.mapDetail._mapSize * _currentMapDetailsData_SO.mapDetail.blockHeight;
                yield return BlockManager.Instance.buildBlock(_mapDetailDict, sceneName, size);
                Debug.Log("------场景:" + sceneName + "   瓦片生成完毕------");
            }
            else
            {
                yield return LoadMainMapGrid(sceneName);
            }
        }

        /// <summary>
        /// 生成Building地图
        /// </summary>
        public IEnumerator GenenrateBuildingInside(string sceneName, string blockID)
        {
            if (_gridsaveDetailDict.Find(i => i.sceneName == sceneName) == null)
            {
                BuildingInsideDetail buildingInsideDetail = InventoryManager.Instance.GetBuildingInsideDetail(blockID);
                MapPrintManager.Instance.GenerateBuildingMap(buildingInsideDetail.tilePropertiesList);
            }
            else
            {
                yield return LoadMainMapGrid(sceneName);
            }
            yield return null;
        }

        private void SetGridmapObstacle()
        {
            Tilemap gridmapObstacleTilemap = _tilemapParent.GetChild(1).GetChild(0).GetComponent<Tilemap>();

            gridmapObstacleTilemap.CompressBounds();
            Vector3Int startPos = gridmapObstacleTilemap.cellBounds.min;
            Vector3Int endPos = gridmapObstacleTilemap.cellBounds.max;

            _gridmapObstacle = new bool[endPos.x - startPos.x + 1, endPos.y - startPos.y + 1];
            _gridDimensions = new Vector2Int(endPos.x - startPos.x + 1, endPos.y - startPos.y + 1);
            _gridOrigin = new Vector2Int(startPos.x, startPos.y);

            for (int x = 0; x < endPos.x - startPos.x + 1; x++)
                for (int y = 0; y < endPos.y - startPos.y + 1; y++)
                {
                    TileBase tile = gridmapObstacleTilemap.GetTile(new Vector3Int(x + startPos.x, y + startPos.y, 0));
                    if (tile != null)
                    {
                        _gridmapObstacle[x, y] = true;
                    }
                }

        }

        /// <summary>
        /// 获取地图信息，返回给Astar算法使用，Astar算法会在初始化时获取这些信息
        /// </summary>
        public bool GetGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            gridDimensions.x = _gridDimensions.x;
            gridDimensions.y = _gridDimensions.y;

            gridOrigin.x = _gridOrigin.x;
            gridOrigin.y = _gridOrigin.y;
            if (gridDimensions == Vector2Int.zero)
                return false;
            if (gridBuildMap == null)
                return false;
            return true;
        }

        public string GetBlockID(MapComponent mapComponent)
        {
            if (_currentMapDetailsData_SO.mapLevelList.Find(i => i.mapComponent == mapComponent) != null)
                return _currentMapDetailsData_SO.mapLevelList.Find(i => i.mapComponent == mapComponent).blockID;
            else if (_currentMapDetailsData_SO.mapElementList.Find(i => i.mapComponent == mapComponent) != null)
                return _currentMapDetailsData_SO.mapElementList.Find(i => i.mapComponent == mapComponent).blockID;
            else
                return "null";
        }


        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.sceneNameDict = this.sceneNameDict;
            saveData.gridsaveDetailDict = this._gridsaveDetailDict;
            saveData.gridBuildMap = this.gridBuildMap;

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            _sceneDetailDict.Clear();
            this.sceneNameDict = saveData.sceneNameDict;
            this._gridsaveDetailDict = saveData.gridsaveDetailDict;
            this.gridBuildMap = saveData.gridBuildMap;
            ClearIslandmapTilemap();
            ClearGridBuildMap();

            SetSceneDetail();
            PrintIslandList(_islandMapSize, _islandRange);
            PrintBuildMap();

            OnOpenBulePrintEvent(false);
            OnIslandMapClickEvent(false);
        }

        private void OnStartNewGameEvent(int index)
        {
            _sceneDetailDict.Clear();
            sceneNameDict.Clear();
            _gridsaveDetailDict.Clear();
            ClearIslandmapTilemap();
            ClearGridBuildMap();

            SetIslandList(_islandMapSize, _islandRange);
            SetSceneDetail();
            PrintIslandList(_islandMapSize, _islandRange);

            SetBuildMap();
            PrintBuildMap();

            OnOpenBulePrintEvent(false);
            OnIslandMapClickEvent(false);
        }
    }
}