using System;
using UnityEngine;

namespace Strategy.Astar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition;//网格的坐标
        public int gCost = 0; //距离起点的距离
        public int hCost = 0; //距离终点的距离
        public int FCost => gCost + hCost;
        public bool isObstacle = false;
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            //比较选出最低的Fcost值，返回-1，0，1
            int result = FCost.CompareTo(other.FCost);

            //如果FCost相同比较hCost
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }

            return result;
        }
    }
}