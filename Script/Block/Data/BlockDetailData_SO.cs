using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BlockDetailData_SO", menuName = "Block/BlockDetail/BlockDetailData_SO", order = 0)]
public class BlockDetailData_SO : ScriptableObject
{
    public List<BlockDetail> blockDetailList = new List<BlockDetail>();
}