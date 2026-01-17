using System.Collections.Generic;
using UnityEngine;

namespace DONT_TOUCH.Scripts
{
    public static class PrimitiveMeshGetter
    {
        private static readonly Dictionary<PrimitiveType, Mesh> PrimitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

        public static Mesh GetPrimitiveMesh(PrimitiveType type)
        {
            if (!PrimitiveMeshes.ContainsKey(type))
            {
                CreatePrimitiveMesh(type);
            }
            return PrimitiveMeshes[type];
        }

        private static Mesh CreatePrimitiveMesh(PrimitiveType type)
        {
            GameObject tempGameObject = GameObject.CreatePrimitive(type);
            Mesh mesh = tempGameObject.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(tempGameObject);

            PrimitiveMeshes[type] = mesh;
            return mesh;
        }
    }
}