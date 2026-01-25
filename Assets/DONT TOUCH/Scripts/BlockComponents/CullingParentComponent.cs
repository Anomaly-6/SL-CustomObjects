using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CullingParentComponent : SchematicBlock
{
    public override BlockType BlockType => BlockType.CullingParent;
    internal MeshFilter _filter;
    
    public override void Compile(SchematicBlockData block)
    {
        base.Compile(block);
    }

    public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
    {
        CullingParentComponent cullingParent =
            Instantiate(AssetDatabase.LoadAssetAtPath<CullingParentComponent>("Assets/Resources/Blocks/CullingParent.prefab"));
        gameObject = cullingParent.gameObject;
        base.Decompile(ref gameObject, block, parent);
    }
    
    private void Start()
    {
        TryGetComponent(out _filter);
    }

    private void Update()
    {
        _filter.hideFlags = HideFlags.HideInInspector;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0.48f, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
