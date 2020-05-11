using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMesh : MonoBehaviour
{
    public Transform rootArmature;
    public Extension[] extensions;
    public float boneRadius;

    public enum StepDirection { Left, Forward, Right, Back }

    public void Rotate()
    {
        //foreach (Extension extension in extensions)
        //    for (int i = 0; i < extension.steps.Length; i++)
        //        extension.steps[i] = (StepDirection)((int)(extension.steps[i] + 1) % 4);
    }

    public void Fit(Vector3[][] vertices)
    {
        Fit(rootArmature, vertices[0]);
        for (int i = 0; i < extensions.Length; i++)
        {
            Fit(extensions[i].armature, vertices[i + 1]);
        }
    }

    public void Fit(Transform armature, Vector3[] vertices)
    {
        armature.transform.position = (vertices[0] + vertices[1] + vertices[2] + vertices[3]) / 4f;
        AlignBone(armature.GetChild(0), vertices[0]);
        AlignBone(armature.GetChild(1), vertices[1]);
        AlignBone(armature.GetChild(2), vertices[2]);
        AlignBone(armature.GetChild(3), vertices[3]);
    }

    private void AlignBone(Transform bone, Vector3 vertex)
    {
        Vector3 boneDirection = bone.up;
        Vector3 vertexDirection = (vertex - bone.parent.parent.position).normalized;

        float angle = Vector3.SignedAngle(boneDirection, vertexDirection, Vector3.up);

        bone.Rotate(bone.parent.parent.up, angle, Space.World);

        float scaleRatio = Vector3.Distance(bone.position, vertex) / Vector3.Distance(bone.position, bone.GetChild(0).position);

        bone.localScale = new Vector3(bone.localScale.x, bone.localScale.y * scaleRatio, bone.localScale.z);
    }

    private void OnDrawGizmos()
    {
        DrawArmature(rootArmature);
    }

    private void DrawArmature(Transform armature)
    {
        Gizmos.color = Color.white;

        DrawBone(armature.GetChild(0));

        DrawBone(armature.GetChild(1));

        DrawBone(armature.GetChild(2));

        DrawBone(armature.GetChild(3));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(armature.GetChild(0).GetChild(0).position, armature.GetChild(1).GetChild(0).position);
        Gizmos.DrawLine(armature.GetChild(1).GetChild(0).position, armature.GetChild(2).GetChild(0).position);
        Gizmos.DrawLine(armature.GetChild(2).GetChild(0).position, armature.GetChild(3).GetChild(0).position);
        Gizmos.DrawLine(armature.GetChild(3).GetChild(0).position, armature.GetChild(0).GetChild(0).position);
    }

    private void DrawBone(Transform bone)
    {
        Gizmos.DrawSphere(bone.position, boneRadius);
        Gizmos.DrawSphere(bone.GetChild(0).position, boneRadius);
        Gizmos.DrawLine(bone.position, bone.GetChild(0).position);
    }

    [System.Serializable]
    public struct Extension
    {
        public Transform armature;
        public StepDirection[] steps;
    }
}
