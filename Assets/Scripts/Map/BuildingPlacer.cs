using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public List<Section> sections;
    public enum Direction { Left, Forward, Up, Back }

    public void Fit(Vector3[][] vertices)
    {
        for (int i = 0; i < sections.Count; i++)
            Fit(sections[i], vertices[i]);
    }

    private void Fit(Section section, Vector3[] vertices)
    {
        section.armature.parent.position = (vertices[0] + vertices[1] + vertices[2] + vertices[3]) / 4f;

        for (int i = 0; i < 4; i++)
            AlignBone(section.armature.GetChild(i), vertices[i]);
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

    [System.Serializable]
    public class Section
    {
        public Transform armature;
        public List<Direction> directions;
    }
}
