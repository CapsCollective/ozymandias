using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMesh : MonoBehaviour
{
    public Transform boneA;
    public Transform boneB;
    public Transform boneC;
    public Transform boneD;

    public float boneRadius;

    public void Fit(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 vertexD)
    {
        AlignBone(boneA, vertexA);
        AlignBone(boneB, vertexB);
        AlignBone(boneC, vertexC);
        AlignBone(boneD, vertexD);
    }

    private static void AlignBone(Transform bone, Vector3 vertex)
    {
        Vector3 boneDirection = bone.up;
        Vector3 vertexDirection = (vertex - bone.parent.parent.position).normalized;

        float angle = Vector3.SignedAngle(boneDirection, vertexDirection, Vector3.up);

        bone.Rotate(bone.parent.parent.up, angle, Space.World);

        float scaleRatio = Vector3.Distance(bone.position, vertex) / Vector3.Distance(bone.position, bone.GetChild(0).position);

        bone.localScale = new Vector3(bone.localScale.x, bone.localScale.y * scaleRatio, bone.localScale.z);

        //Debug.DrawRay(bone.parent.parent.position, boneDirection, Color.blue);
        //Debug.DrawRay(bone.parent.parent.position, vertexDirection, Color.green);
        //Debug.Log(angle);
        //Debug.Break();

        //Quaternion rotation = Quaternion.FromToRotation(boneDirection, vertexDirection);
        //bone.up = rotation * bone.up;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(boneA.position, boneRadius);
        Gizmos.DrawSphere(boneA.GetChild(0).position, boneRadius);
        Gizmos.DrawLine(boneA.position, boneA.GetChild(0).position);

        Gizmos.DrawSphere(boneB.position, boneRadius);
        Gizmos.DrawSphere(boneB.GetChild(0).position, boneRadius);
        Gizmos.DrawLine(boneB.position, boneB.GetChild(0).position);

        Gizmos.DrawSphere(boneC.position, boneRadius);
        Gizmos.DrawSphere(boneC.GetChild(0).position, boneRadius);
        Gizmos.DrawLine(boneC.position, boneC.GetChild(0).position);

        Gizmos.DrawSphere(boneD.position, boneRadius);
        Gizmos.DrawSphere(boneD.GetChild(0).position, boneRadius);
        Gizmos.DrawLine(boneD.position, boneD.GetChild(0).position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(boneA.GetChild(0).position, boneB.GetChild(0).position);
        Gizmos.DrawLine(boneB.GetChild(0).position, boneC.GetChild(0).position);
        Gizmos.DrawLine(boneC.GetChild(0).position, boneD.GetChild(0).position);
        Gizmos.DrawLine(boneD.GetChild(0).position, boneA.GetChild(0).position);
    }
}
