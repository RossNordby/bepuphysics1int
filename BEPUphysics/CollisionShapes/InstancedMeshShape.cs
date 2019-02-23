
using BEPUphysics.DataStructures;
using BEPUutilities;


namespace BEPUphysics.CollisionShapes
{
    ///<summary>
    /// Local space data associated with an instanced mesh.
    /// This contains a hierarchy and all the other heavy data needed
    /// by an InstancedMesh.
    ///</summary>
    public class InstancedMeshShape : CollisionShape
    {
        TriangleMesh triangleMesh;
        ///<summary>
        /// Gets or sets the TriangleMesh data structure used by this shape.
        ///</summary>
        public TriangleMesh TriangleMesh
        {
            get
            {
                return triangleMesh;
            }
            set
            {
                triangleMesh = value;
                OnShapeChanged();
            }
        }



        ///<summary>
        /// Constructs a new instanced mesh shape.
        ///</summary>
        ///<param name="vertices">Vertices of the mesh.</param>
        ///<param name="indices">Indices of the mesh.</param>
        public InstancedMeshShape(Vector3[] vertices, int[] indices)
        {
            TriangleMesh = new TriangleMesh(new StaticMeshData(vertices, indices));
        }



        ///<summary>
        /// Computes the bounding box of the transformed mesh shape.
        ///</summary>
        ///<param name="transform">Transform to apply to the shape during the bounding box calculation.</param>
        ///<param name="boundingBox">Bounding box containing the transformed mesh shape.</param>
        public void ComputeBoundingBox(ref AffineTransform transform, out BoundingBox boundingBox)
        {
#if !WINDOWS
            boundingBox = new BoundingBox();
#endif
            Fix minX = Fix.MaxValue;
            Fix minY = Fix.MaxValue;
            Fix minZ = Fix.MaxValue;

            Fix maxX = Fix.MaxValue.Neg();
            Fix maxY = Fix.MaxValue.Neg();
            Fix maxZ = Fix.MaxValue.Neg();
            for (int i = 0; i < triangleMesh.Data.vertices.Length; i++)
            {
                Vector3 vertex;
                triangleMesh.Data.GetVertexPosition(i, out vertex);
                Matrix3x3.Transform(ref vertex, ref transform.LinearTransform, out vertex);
                if (vertex.X < minX)
                    minX = vertex.X;
                if (vertex.X > maxX)
                    maxX = vertex.X;

                if (vertex.Y < minY)
                    minY = vertex.Y;
                if (vertex.Y > maxY)
                    maxY = vertex.Y;

                if (vertex.Z < minZ)
                    minZ = vertex.Z;
                if (vertex.Z > maxZ)
                    maxZ = vertex.Z;
            }
            boundingBox.Min.X = transform.Translation.X.Add(minX);
            boundingBox.Min.Y = transform.Translation.Y.Add(minY);
            boundingBox.Min.Z = transform.Translation.Z.Add(minZ);
            
            boundingBox.Max.X = transform.Translation.X.Add(maxX);
            boundingBox.Max.Y = transform.Translation.Y.Add(maxY);
            boundingBox.Max.Z = transform.Translation.Z.Add(maxZ);
        }
    }
}
