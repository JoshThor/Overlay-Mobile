using UnityEngine;
using System.Collections;

public class SetMeshBounds : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public Vector3 extents = Vector3.zero;

    private Mesh m_Mesh;
    private Vector3 m_OldCenter;
    private Vector3 m_OldExtents;
    void Start()
    {
        MeshFilter MF = GetComponent<MeshFilter>();
        if (MF != null)
            m_Mesh = MF.mesh;
        SkinnedMeshRenderer SMR = GetComponent<SkinnedMeshRenderer>();
        if (SMR != null)
            m_Mesh = SMR.sharedMesh;
        m_OldCenter = m_Mesh.bounds.center;
        m_OldExtents = m_Mesh.bounds.extents;

        if (center == Vector3.zero && extents == Vector3.zero)
        {
            center = m_OldCenter;
            extents = m_OldExtents;
        }
    }

    void Update()
    {
        if (m_OldExtents != extents || m_OldCenter != center)
        {
            Bounds B = new Bounds(center, extents * 2f);
            m_OldCenter = center;
            m_OldExtents = extents;
            m_Mesh.bounds = B;
        }
    }
}