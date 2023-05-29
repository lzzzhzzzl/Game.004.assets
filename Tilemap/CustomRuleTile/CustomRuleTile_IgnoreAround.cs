using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CustomRuleTile_IgnoreAround", menuName = "CustomRuleTile/IgnoreAround", order = 1)]
public class CustomRuleTile_IgnoreAround : RuleTile<CustomRuleTile_IgnoreAround.Neighbor>
{
    public bool m_CheckAnyTile = false;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {

    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case TilingRule.Neighbor.This: return m_CheckAnyTile ? tile != null : tile == this;
            case TilingRule.Neighbor.NotThis: return m_CheckAnyTile ? tile == null : tile != this;
        }

        return base.RuleMatch(neighbor, tile);
    }
}