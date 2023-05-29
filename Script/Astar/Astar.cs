using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Strategy.Map;

namespace Strategy.Astar
{
    public class Astar : MonoBehaviour
    {
        private GridNodes gridNodes;
        private Node starNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;
        private bool isFindObstacle;

        private List<Node> openNodeList;        //当前选中的Node周围的8个点
        private HashSet<Node> closedNodeList;    //所有被选中的点
        private void OnEnable()
        {
            EventHandler.SetMapObstacleToCharacter += OnSetMapObstacleToCharacter;
        }
        private void OnDisable()
        {
            EventHandler.SetMapObstacleToCharacter -= OnSetMapObstacleToCharacter;
        }
        private bool pathFound;
        /// <summary>
        /// 构建更新Stack的每一步
        /// </summary>
        public void BuildPath(Vector2Int starPos, Vector2Int endPos, Stack<MovementStep> npcMovementStep)
        {
            pathFound = false;
            if (isFindObstacle && SetNodesPosition(starPos, endPos))
            {
                if (FindShortestPath())
                {
                    UpdatePathOnMovementStepSrack(npcMovementStep);
                }
            }
        }

        /// <summary>
        /// 寻找最短路径
        /// </summary>
        private bool FindShortestPath()
        {
            //添加起点
            openNodeList.Add(starNode);
            while (openNodeList.Count > 0)
            {//节点排序，Node内涵比较函数
                openNodeList.Sort();

                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                EvaluateNeighbourNodes(closeNode);
            }

            return pathFound;
        }

        /// <summary>
        /// 评估周围8个点，并生成对应消耗值
        /// </summary>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);

                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            //链接父节点
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 找到有效的Node,非障碍，非已选择
        /// </summary>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
                return null;
            else
                return neighbourNode;
        }


        /// <summary>
        /// 返回两点距离值
        /// </summary>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
        public void OnSetMapObstacleToCharacter()
        {
            if (GridMapManager.Instance.GetGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                //根据瓦片地图范围构建网格移动节点范围数组
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                isFindObstacle = true;
            }
            else
            {
                isFindObstacle = false;
                return;
            }

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    bool obstacle = GridMapManager.Instance.GetMpaObstacle(new Vector2Int(x, y));

                    Node node = gridNodes.GetGridNode(x, y);
                    if (obstacle)
                    {
                        node.isObstacle = true;
                    }
                }
            }
            return;
        }
        private bool SetNodesPosition(Vector2Int startPos, Vector2Int endPos)
        {
            openNodeList = new List<Node>();
            closedNodeList = new HashSet<Node>();
            gridNodes.ClearGridNodes();

            starNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            if (starNode.isObstacle || targetNode.isObstacle)
                return false;
            return true;
        }
        /// <summary>
        /// 构建节点信息，初始化两个列表
        /// </summary>
        private bool GenerateGridNodes(Vector2Int startPos, Vector2Int endPos)
        {
            if (GridMapManager.Instance.GetGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                //根据瓦片地图范围构建网格移动节点范围数组
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();

                closedNodeList = new HashSet<Node>();
            }
            else
                return false;

            //gridNodes的范围是从0,0开始所以需要减去原点坐标得到实际位置
            starNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    bool obstacle = GridMapManager.Instance.GetMpaObstacle(new Vector2Int(x, y));

                    Node node = gridNodes.GetGridNode(x, y);
                    if (obstacle)
                    {
                        node.isObstacle = true;
                    }
                }
            }
            if (starNode.isObstacle || targetNode.isObstacle)
                return false;
            return true;
        }

        /// <summary>
        /// 更新坐标
        /// </summary>
        private void UpdatePathOnMovementStepSrack(Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;
            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                //压入堆栈
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }

    }
}