using UnityEngine;

public static class Vector3Extension
{
    public static bool AreFieldsTouching(this Vector3 sourceVec3, Vector3 otherVec3)
    {
        if (sourceVec3.x == otherVec3.x)
        {
            if (Mathf.Abs(sourceVec3.z - otherVec3.z) <= 1.1f)
            {
                return true;
            }
        }
        else if(sourceVec3.z == otherVec3.z)
        {
            if (Mathf.Abs(sourceVec3.x - otherVec3.x) <= 1.1f)
            {
                return true;
            }
        }
        return false;
    }
    public static Vector3 CopyAndCreateNewVector(this Vector3 sourceVec3, float xOffset, float yOffset, float zOffset)
    {
        return new Vector3(sourceVec3.x + xOffset, sourceVec3.y + yOffset, sourceVec3.z + zOffset);
    }
    public static Vector3 MirrorPositionAlong(this Vector3 sourceVec3,Vector3 originPoint, bool xAxis, bool zAxis)
    {
        if (xAxis && zAxis) return sourceVec3;
        float x = originPoint.x - sourceVec3.x;
        float z = originPoint.z - sourceVec3.z;
        if (xAxis && !zAxis)
        {
            return new Vector3(originPoint.x - x, originPoint.y, originPoint.z + z); 
        }
        else if(!xAxis && zAxis)
        {
            return new Vector3(originPoint.x + x, originPoint.y, originPoint.z - z);
        }
        else
        {
            return new Vector3(originPoint.x - x, originPoint.y, originPoint.z - z);
        }
    }
}
