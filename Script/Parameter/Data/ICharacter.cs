using UnityEngine;
public interface ICharacter
{
    public SerializableVector3 GetParameterPosition();
    public string GetParameterID();
    public float GetParameterHealth();
}