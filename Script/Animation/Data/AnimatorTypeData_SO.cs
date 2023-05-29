using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimatorTypeData_SO", menuName = "Animation/Animator/AnimatorTypeData_SO", order = 0)]
public class AnimatorTypeData_SO : ScriptableObject
{
    public List<AnimatorTypeGroup> animatorTypeGroupList;
    public List<AnimatorType> GetAnimatorTypeList(string ID)
    {
        if (animatorTypeGroupList.Find(i => i.ID == ID) != null)
            return animatorTypeGroupList.Find(i => i.ID == ID).animatorTypes;
        else
            return null;
    }
}