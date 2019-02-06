using System;
using System.Collections.Generic;
using BEPUphysics.CollisionTests.Manifolds;

using BEPUutilities;
using BEPUutilities.DataStructures;


namespace BEPUphysics.CollisionShapes
{
    ///<summary>
    /// The local space data needed by a Terrain collidable.
    /// Contains the Heightmap and other information.
    ///</summary>
    public class TerrainShape : CollisionShape
    {
        private Fix32[,] heights;
        //note: changing heights in array does not fire OnShapeChanged automatically.
        //Need to notify parent manually if you do it.
        ///<summary>
        /// Gets or sets the height field of the terrain shape.
        ///</summary>
        public Fix32[,] Heights
        {
            get
            {
                return heights;
            }
            set
            {
                heights = value;
                OnShapeChanged();
            }
        }



        QuadTriangleOrganization quadTriangleOrganization;
        ///<summary>
        /// Gets or sets the quad triangle organization.
        ///</summary>
        public QuadTriangleOrganization QuadTriangleOrganization
        {
            get
            {
                return quadTriangleOrganization;
            }
            set
            {
                quadTriangleOrganization = value;
                OnShapeChanged();
            }
        }

        ///<summary>
        /// Constructs a TerrainShape.
        ///</summary>
        ///<param name="heights">Heights array used for the shape.</param>
        ///<param name="triangleOrganization">Triangle organization of each quad.</param>
        ///<exception cref="ArgumentException">Thrown if the heights array has less than 2x2 vertices.</exception>
        public TerrainShape(Fix32[,] heights, QuadTriangleOrganization triangleOrganization)
        {
            if (heights.GetLength(0) <= 1 || heights.GetLength(1) <= 1)
            {
                throw new ArgumentException("Terrains must have a least 2x2 vertices (one quad).");
            }
            this.heights = heights;
            quadTriangleOrganization = triangleOrganization;
        }

        ///<summary>
        /// Constructs a TerrainShape.
        ///</summary>
        ///<param name="heights">Heights array used for the shape.</param>
        public TerrainShape(Fix32[,] heights)
            : this(heights, QuadTriangleOrganization.BottomLeftUpperRight)
        {
        }



        ///<summary>
        /// Constructs the bounding box of the terrain given a transform.
        ///</summary>
        ///<param name="transform">Transform to apply to the terrain during the bounding box calculation.</param>
        ///<param name="boundingBox">Bounding box of the terrain shape when transformed.</param>
        public void GetBoundingBox(ref AffineTransform transform, out BoundingBox boundingBox)
        {
#if !WINDOWS
            boundingBox = new BoundingBox();
#endif
            Fix32 minX = Fix32.MaxValue, maxX = Fix32.MaxValue.Neg(),
                  minY = Fix32.MaxValue, maxY = Fix32.MaxValue.Neg(),
                  minZ = Fix32.MaxValue, maxZ = Fix32.MaxValue.Neg();
            Vector3 minXvertex = new Vector3(),
                    maxXvertex = new Vector3(),
                    minYvertex = new Vector3(),
                    maxYvertex = new Vector3(),
                    minZvertex = new Vector3(),
                    maxZvertex = new Vector3();

            //Find the extreme locations.
            for (int i = 0; i < heights.GetLength(0); i++)
            {
                for (int j = 0; j < heights.GetLength(1); j++)
                {
                    var vertex = new Vector3(i.ToFixFast(), heights[i, j], j.ToFixFast());
                    Matrix3x3.Transform(ref vertex, ref transform.LinearTransform, out vertex);
                    if (vertex.X < minX)
                    {
                        minX = vertex.X;
                        minXvertex = vertex;
                    }
                    else if (vertex.X > maxX)
                    {
                        maxX = vertex.X;
                        maxXvertex = vertex;
                    }

                    if (vertex.Y < minY)
                    {
                        minY = vertex.Y;
                        minYvertex = vertex;
                    }
                    else if (vertex.Y > maxY)
                    {
                        maxY = vertex.Y;
                        maxYvertex = vertex;
                    }

                    if (vertex.Z < minZ)
                    {
                        minZ = vertex.Z;
                        minZvertex = vertex;
                    }
                    else if (vertex.Z > maxZ)
                    {
                        maxZ = vertex.Z;
                        maxZvertex = vertex;
                    }
                }
            }

            //Shift the bounding box.
            boundingBox.Min.X = minXvertex.X.Add(transform.Translation.X);
            boundingBox.Min.Y = minYvertex.Y.Add(transform.Translation.Y);
            boundingBox.Min.Z = minZvertex.Z.Add(transform.Translation.Z);
            boundingBox.Max.X = maxXvertex.X.Add(transform.Translation.X);
            boundingBox.Max.Y = maxYvertex.Y.Add(transform.Translation.Y);
            boundingBox.Max.Z = maxZvertex.Z.Add(transform.Translation.Z);
        }

        ///<summary>
        /// Tests a ray against the terrain shape.
        ///</summary>
        ///<param name="ray">Ray to test against the shape.</param>
        ///<param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
        ///<param name="transform">Transform to apply to the terrain shape during the test.</param>
        ///<param name="hit">Hit data of the ray cast, if any.</param>
        ///<returns>Whether or not the ray hit the transformed terrain shape.</returns>
        public bool RayCast(ref Ray ray, Fix32 maximumLength, ref AffineTransform transform, out RayHit hit)
        {
            return RayCast(ref ray, maximumLength, ref transform, TriangleSidedness.Counterclockwise, out hit);
        }
        ///<summary>
        /// Tests a ray against the terrain shape.
        ///</summary>
        ///<param name="ray">Ray to test against the shape.</param>
        ///<param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
        ///<param name="transform">Transform to apply to the terrain shape during the test.</param>
        ///<param name="sidedness">Sidedness of the triangles to use when raycasting.</param>
        ///<param name="hit">Hit data of the ray cast, if any.</param>
        ///<returns>Whether or not the ray hit the transformed terrain shape.</returns>
        public bool RayCast(ref Ray ray, Fix32 maximumLength, ref AffineTransform transform, TriangleSidedness sidedness, out RayHit hit)
        {
            hit = new RayHit();
            //Put the ray into local space.
            Ray localRay;
            AffineTransform inverse;
            AffineTransform.Invert(ref transform, out inverse);
            Matrix3x3.Transform(ref ray.Direction, ref inverse.LinearTransform, out localRay.Direction);
            AffineTransform.Transform(ref ray.Position, ref inverse, out localRay.Position);

            //Use rasterizey traversal.
            //The origin is at 0,0,0 and the map goes +X, +Y, +Z.
            //if it's before the origin and facing away, or outside the max and facing out, early out.
            Fix32 maxX = (heights.GetLength(0) - 1).ToFix();
            Fix32 maxZ = (heights.GetLength(1) - 1).ToFix();

            Vector3 progressingOrigin = localRay.Position;
            Fix32 distance = F64.C0;
            //Check the outside cases first.
            if (progressingOrigin.X < F64.C0)
            {
                if (localRay.Direction.X > F64.C0)
                {
                    //Off the left side.
                    Fix32 timeToMinX = (progressingOrigin.X.Neg()).Div(localRay.Direction.X);
					distance = distance.Add(timeToMinX);
                    Vector3 increment;
                    Vector3.Multiply(ref localRay.Direction, timeToMinX, out increment);
                    Vector3.Add(ref increment, ref progressingOrigin, out progressingOrigin);
                }
                else
                    return false; //Outside and pointing away from the terrain.
            }
            else if (progressingOrigin.X > maxX)
            {
                if (localRay.Direction.X < F64.C0)
                {
                    //Off the left side.
                    Fix32 timeToMinX = (progressingOrigin.X.Sub(maxX).Neg()).Div(localRay.Direction.X);
					distance = distance.Add(timeToMinX);
                    Vector3 increment;
                    Vector3.Multiply(ref localRay.Direction, timeToMinX, out increment);
                    Vector3.Add(ref increment, ref progressingOrigin, out progressingOrigin);
                }
                else
                    return false; //Outside and pointing away from the terrain.
            }

            if (progressingOrigin.Z < F64.C0)
            {
                if (localRay.Direction.Z > F64.C0)
                {
                    Fix32 timeToMinZ = (progressingOrigin.Z.Neg()).Div(localRay.Direction.Z);
					distance = distance.Add(timeToMinZ);
                    Vector3 increment;
                    Vector3.Multiply(ref localRay.Direction, timeToMinZ, out increment);
                    Vector3.Add(ref increment, ref progressingOrigin, out progressingOrigin);
                }
                else
                    return false;
            }
            else if (progressingOrigin.Z > maxZ)
            {
                if (localRay.Direction.Z < F64.C0)
                {
                    Fix32 timeToMinZ = (progressingOrigin.Z.Sub(maxZ).Neg()).Div(localRay.Direction.Z);
					distance = distance.Add(timeToMinZ);
                    Vector3 increment;
                    Vector3.Multiply(ref localRay.Direction, timeToMinZ, out increment);
                    Vector3.Add(ref increment, ref progressingOrigin, out progressingOrigin);
                }
                else
                    return false;
            }

            if (distance > maximumLength)
                return false;



            //By now, we should be entering the main body of the terrain.

            int xCell = progressingOrigin.X.ToInt();
            int zCell = progressingOrigin.Z.ToInt();
            //If it's hitting the border and going in, then correct the index
            //so that it will initially target a valid quad.
            //Without this, a quad beyond the border would be tried and failed.
            if (xCell == heights.GetLength(0) - 1 && localRay.Direction.X < F64.C0)
                xCell = heights.GetLength(0) - 2;
            if (zCell == heights.GetLength(1) - 1 && localRay.Direction.Z < F64.C0)
                zCell = heights.GetLength(1) - 2;

            while (true)
            {
                //Check for a miss.
                if (xCell < 0 ||
                    zCell < 0 ||
                    xCell >= heights.GetLength(0) - 1 ||
                    zCell >= heights.GetLength(1) - 1)
                    return false;

                //Test the triangles of this cell.
                Vector3 v1, v2, v3, v4;
                // v3 v4
                // v1 v2
                GetLocalPosition(xCell, zCell, out v1);
                GetLocalPosition(xCell + 1, zCell, out v2);
                GetLocalPosition(xCell, zCell + 1, out v3);
                GetLocalPosition(xCell + 1, zCell + 1, out v4);
                RayHit hit1, hit2;
                bool didHit1;
                bool didHit2;

                //Don't bother doing ray intersection tests if the ray can't intersect it.

                Fix32 highest = v1.Y;
                Fix32 lowest = v1.Y;
                if (v2.Y > highest)
                    highest = v2.Y;
                else if (v2.Y < lowest)
                    lowest = v2.Y;
                if (v3.Y > highest)
                    highest = v3.Y;
                else if (v3.Y < lowest)
                    lowest = v3.Y;
                if (v4.Y > highest)
                    highest = v4.Y;
                else if (v4.Y < lowest)
                    lowest = v4.Y;


                if (!(progressingOrigin.Y > highest && localRay.Direction.Y > F64.C0 ||
                    progressingOrigin.Y < lowest && localRay.Direction.Y < F64.C0))
                {


                    if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
                    {
                        //Always perform the raycast as if Y+ in local space is the way the triangles are facing.
                        didHit1 = Toolbox.FindRayTriangleIntersection(ref localRay, maximumLength, sidedness, ref v1, ref v2, ref v3, out hit1);
                        didHit2 = Toolbox.FindRayTriangleIntersection(ref localRay, maximumLength, sidedness, ref v2, ref v4, ref v3, out hit2);
                    }
                    else //if (quadTriangleOrganization == CollisionShapes.QuadTriangleOrganization.BottomRightUpperLeft)
                    {
                        didHit1 = Toolbox.FindRayTriangleIntersection(ref localRay, maximumLength, sidedness, ref v1, ref v2, ref v4, out hit1);
                        didHit2 = Toolbox.FindRayTriangleIntersection(ref localRay, maximumLength, sidedness, ref v1, ref v4, ref v3, out hit2);
                    }
                    if (didHit1 && didHit2)
                    {
                        if (hit1.T < hit2.T)
                        {
                            Vector3.Multiply(ref ray.Direction, hit1.T, out hit.Location);
                            Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
                            Matrix3x3.TransformTranspose(ref hit1.Normal, ref inverse.LinearTransform, out hit.Normal);
                            hit.T = hit1.T;
                            return true;
                        }
                        Vector3.Multiply(ref ray.Direction, hit2.T, out hit.Location);
                        Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
                        Matrix3x3.TransformTranspose(ref hit2.Normal, ref inverse.LinearTransform, out hit.Normal);
                        hit.T = hit2.T;
                        return true;
                    }
                    else if (didHit1)
                    {
                        Vector3.Multiply(ref ray.Direction, hit1.T, out hit.Location);
                        Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
                        Matrix3x3.TransformTranspose(ref hit1.Normal, ref inverse.LinearTransform, out hit.Normal);
                        hit.T = hit1.T;
                        return true;
                    }
                    else if (didHit2)
                    {
                        Vector3.Multiply(ref ray.Direction, hit2.T, out hit.Location);
                        Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
                        Matrix3x3.TransformTranspose(ref hit2.Normal, ref inverse.LinearTransform, out hit.Normal);
                        hit.T = hit2.T;
                        return true;
                    }
                }

                //Move to the next cell.

                Fix32 timeToX;
                if (localRay.Direction.X < F64.C0)
                    timeToX = (progressingOrigin.X.Sub(xCell.ToFixFast()).Neg()).Div(localRay.Direction.X);
                else if (localRay.Direction.X > F64.C0)
                    timeToX = ((xCell + 1).ToFixFast().Sub(progressingOrigin.X)).Div(localRay.Direction.X);
                else
                    timeToX = Fix32.MaxValue;

                Fix32 timeToZ;
                if (localRay.Direction.Z < F64.C0)
                    timeToZ = (progressingOrigin.Z.Sub(zCell.ToFixFast()).Neg()).Div(localRay.Direction.Z);
                else if (localRay.Direction.Z > F64.C0)
                    timeToZ = ((zCell + 1).ToFixFast().Sub(progressingOrigin.Z)).Div(localRay.Direction.Z);
                else
                    timeToZ = Fix32.MaxValue;

                //Move to the next cell.
                if (timeToX < timeToZ)
                {
                    if (localRay.Direction.X < F64.C0)
                        xCell--;
                    else
                        xCell++;

					distance =
distance.Add(timeToX);
                    if (distance > maximumLength)
                        return false;

                    Vector3 increment;
                    Vector3.Multiply(ref localRay.Direction, timeToX, out increment);
                    Vector3.Add(ref increment, ref progressingOrigin, out progressingOrigin);
                }
                else
                {
                    if (localRay.Direction.Z < F64.C0)
                        zCell--;
                    else
                        zCell++;

					distance =
distance.Add(timeToZ);
                    if (distance > maximumLength)
                        return false;

                    Vector3 increment;
                    Vector3.Multiply(ref localRay.Direction, timeToZ, out increment);
                    Vector3.Add(ref increment, ref progressingOrigin, out progressingOrigin);
                }

            }


        }

        ///<summary>
        /// Gets the position of a vertex at the given indices in local space.
        ///</summary>
        ///<param name="columnIndex">Index in the first dimension.</param>
        ///<param name="rowIndex">Index in the second dimension.</param>
        ///<param name="v">Local space position at the given vertice.s</param>
        public void GetLocalPosition(int columnIndex, int rowIndex, out Vector3 v)
        {
#if !WINDOWS
            v = new Vector3();
#endif
            v.X = columnIndex.ToFix();
            v.Y = heights[columnIndex, rowIndex];
            v.Z = rowIndex.ToFix();
        }

        /// <summary>
        /// Gets the world space position of a vertex in the terrain at the given indices.
        /// </summary>
        ///<param name="columnIndex">Index in the first dimension.</param>
        ///<param name="rowIndex">Index in the second dimension.</param>
        /// <param name="transform">Transform to apply to the vertex.</param>
        /// <param name="position">Transformed position of the vertex at the given indices.</param>
        public void GetPosition(int columnIndex, int rowIndex, ref AffineTransform transform, out Vector3 position)
        {
            if (columnIndex <= 0)
                columnIndex = 0;
            else if (columnIndex >= heights.GetLength(0))
                columnIndex = heights.GetLength(0) - 1;
            if (rowIndex <= 0)
                rowIndex = 0;
            else if (rowIndex >= heights.GetLength(1))
                rowIndex = heights.GetLength(1) - 1;
#if !WINDOWS
            position = new Vector3();
#endif
            position.X = columnIndex.ToFix();
            position.Y = heights[columnIndex, rowIndex];
            position.Z = rowIndex.ToFix();
            AffineTransform.Transform(ref position, ref transform, out position);


        }


        /// <summary>
        /// Gets the non-normalized local space vertex normal at the given indices.
        /// </summary>
        ///<param name="columnIndex">Vertex index in the first dimension.</param>
        ///<param name="rowIndex">Vertex index in the second dimension.</param>
        /// <param name="normal">Non-normalized local space normal at the given indices.</param>
        public void GetLocalNormal(int columnIndex, int rowIndex, out Vector3 normal)
        {

            Fix32 topHeight = heights[columnIndex, Math.Min(rowIndex + 1, heights.GetLength(1) - 1)];
            Fix32 bottomHeight = heights[columnIndex, Math.Max(rowIndex - 1, 0)];
            Fix32 rightHeight = heights[Math.Min(columnIndex + 1, heights.GetLength(0) - 1), rowIndex];
            Fix32 leftHeight = heights[Math.Max(columnIndex - 1, 0), rowIndex];

            //Since the horizontal offsets are known to be 1 in local space, we can omit quite a few operations compared to a full Vector3 and cross product.

            //The implicit vectors are:
            //var leftToRight = new Vector3(2, rightHeight - leftHeight, 0);
            //var bottomToTop = new Vector3(0, topHeight - bottomHeight, 2);
            //the result is then:
            //Vector3.Cross(bottomToTop, leftToRight);
            //Which is:
            //Fix32 resultX = bottomToTop.Y * leftToRight.Z - bottomToTop.Z * leftToRight.Y;
            //Fix32 resultY = bottomToTop.Z * leftToRight.X - bottomToTop.X * leftToRight.Z;
            //Fix32 resultZ = bottomToTop.X * leftToRight.Y - bottomToTop.Y * leftToRight.X;
            //Which becomes:
            //Fix32 resultX = bottomToTop.Y * 0 - 2 * leftToRight.Y;
            //Fix32 resultY = 2 * 2 - 0 * 0;
            //Fix32 resultZ = 0 * leftToRight.Y - bottomToTop.Y * 2;
            //Which becomes:
            normal.X = rightHeight.Sub(leftHeight);
            normal.Y = F64.C2;
            normal.Z = topHeight.Sub(bottomHeight);

        }


        ///<summary>
        /// Gets overlapped triangles with the terrain shape with a bounding box in the local space of the shape.
        ///</summary>
        ///<param name="localBoundingBox">Bounding box in the local space of the terrain shape.</param>
        ///<param name="overlappedElements">Indices of triangles whose bounding boxes overlap the input bounding box. Encoded as 2 * (quadRowIndex * terrainWidthInQuads + quadColumnIndex) + isFirstTriangleOfQuad ? 0 : 1, where isFirstTriangleOfQuad refers to which of the two triangles in a quad is being requested. Matches the input of the TerrainShape.GetTriangle function.</param>
        ///<typeparam name="T">Type of the list to fill with overlaps.</typeparam>
        public bool GetOverlaps<T>(BoundingBox localBoundingBox, ref T overlappedElements) where T : IList<int> //Designed to work with value type ILists, hence anti-boxing interface constraint and ref.
        {
            int width = heights.GetLength(0);
            int minX = Math.Max(localBoundingBox.Min.X.ToInt(), 0);
            int minY = Math.Max(localBoundingBox.Min.Z.ToInt(), 0);
            int maxX = Math.Min(localBoundingBox.Max.X.ToInt(), width - 2);
            int maxY = Math.Min(localBoundingBox.Max.Z.ToInt(), heights.GetLength(1) - 2);
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    //Before adding a triangle to the list, make sure the object isn't too high or low from the quad.
                    Fix32 highest, lowest;
                    Fix32 y1 = heights[i, j];
                    Fix32 y2 = heights[i + 1, j];
                    Fix32 y3 = heights[i, j + 1];
                    Fix32 y4 = heights[i + 1, j + 1];

                    highest = y1;
                    lowest = y1;
                    if (y2 > highest)
                        highest = y2;
                    else if (y2 < lowest)
                        lowest = y2;
                    if (y3 > highest)
                        highest = y3;
                    else if (y3 < lowest)
                        lowest = y3;
                    if (y4 > highest)
                        highest = y4;
                    else if (y4 < lowest)
                        lowest = y4;


                    if (localBoundingBox.Max.Y < lowest ||
                        localBoundingBox.Min.Y > highest)
                        continue;

                    //Now the local bounding box is very likely intersecting those of the triangles.
                    //Add the triangles to the list.
                    int quadIndex = (i + j * width) << 1;
                    overlappedElements.Add(quadIndex);
                    overlappedElements.Add(quadIndex | 1);


                }
            }
            return overlappedElements.Count > 0;
        }


        ///<summary>
        /// Gets the first triangle of the quad at the given indices in world space.
        ///</summary>
        ///<param name="columnIndex">Index of the triangle's quad in the first dimension.</param>
        ///<param name="rowIndex">Index of the triangle's quad in the second dimension.</param>
        ///<param name="transform">Transform to apply to the triangle vertices.</param>
        ///<param name="a">First vertex of the triangle.</param>
        ///<param name="b">Second vertex of the triangle.</param>
        ///<param name="c">Third vertex of the triangle.</param>
        public void GetFirstTriangle(int columnIndex, int rowIndex, ref AffineTransform transform, out Vector3 a, out Vector3 b, out Vector3 c)
        {
            if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
            {
                GetPosition(columnIndex, rowIndex, ref transform, out a);
                GetPosition(columnIndex + 1, rowIndex, ref transform, out b);
                GetPosition(columnIndex, rowIndex + 1, ref transform, out c);
            }
            else
            {
                GetPosition(columnIndex, rowIndex, ref transform, out a);
                GetPosition(columnIndex + 1, rowIndex, ref transform, out b);
                GetPosition(columnIndex + 1, rowIndex + 1, ref transform, out c);
            }
        }

        ///<summary>
        /// Gets the second triangle of the quad at the given indices in world space.
        ///</summary>
        ///<param name="columnIndex">Index of the triangle's quad in the first dimension.</param>
        ///<param name="rowIndex">Index of the triangle's quad in the second dimension.</param>
        ///<param name="transform">Transform to apply to the triangle vertices.</param>
        ///<param name="a">First vertex of the triangle.</param>
        ///<param name="b">Second vertex of the triangle.</param>
        ///<param name="c">Third vertex of the triangle.</param>
        public void GetSecondTriangle(int columnIndex, int rowIndex, ref AffineTransform transform, out Vector3 a, out Vector3 b, out Vector3 c)
        {
            if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
            {
                GetPosition(columnIndex, rowIndex + 1, ref transform, out a);
                GetPosition(columnIndex + 1, rowIndex + 1, ref transform, out b);
                GetPosition(columnIndex + 1, rowIndex, ref transform, out c);
            }
            else
            {
                GetPosition(columnIndex, rowIndex, ref transform, out a);
                GetPosition(columnIndex, rowIndex + 1, ref transform, out b);
                GetPosition(columnIndex + 1, rowIndex + 1, ref transform, out c);
            }
        }


        ///<summary>
        /// Gets a world space triangle in the terrain at the given triangle index.
        ///</summary>
        ///<param name="index">Index of the triangle. Encoded as 2 * (quadRowIndex * terrainWidthInQuads + quadColumnIndex) + isFirstTriangleOfQuad ? 0 : 1, where isFirstTriangleOfQuad refers to which of the two triangles in a quad is being requested. Matches the output of the TerrainShape.GetOverlaps function.</param>
        ///<param name="transform">Transform to apply to the triangle vertices.</param>
        ///<param name="a">First vertex of the triangle.</param>
        ///<param name="b">Second vertex of the triangle.</param>
        ///<param name="c">Third vertex of the triangle.</param>
        public void GetTriangle(int index, ref AffineTransform transform, out Vector3 a, out Vector3 b, out Vector3 c)
        {
            //Find the quad.
            int quadIndex = index / 2;
            //TODO: This division could be avoided if you're willing to get tricky or impose some size requirements.
            int rowIndex = quadIndex / heights.GetLength(0);
            int columnIndex = quadIndex - rowIndex * heights.GetLength(0);
            if ((index & 1) == 0) //Check if this is the first or second triangle.
            {
                GetFirstTriangle(columnIndex, rowIndex, ref transform, out a, out b, out c);
            }
            else
            {
                GetSecondTriangle(columnIndex, rowIndex, ref transform, out a, out b, out c);
            }
        }



        internal void GetLocalIndices(int i, out TerrainVertexIndices a, out TerrainVertexIndices b, out TerrainVertexIndices c)
        {
            int quadIndex = i / 2;
            //TODO: This division could be avoided if you're willing to get tricky or impose some size requirements.
            int rowIndex = quadIndex / heights.GetLength(0);
            int columnIndex = quadIndex - rowIndex * heights.GetLength(0);
            if ((i & 1) == 0) //Check if this is the first or second triangle.
            {
                if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
                {
                    a = new TerrainVertexIndices { ColumnIndex = columnIndex, RowIndex = rowIndex };
                    b = new TerrainVertexIndices { ColumnIndex = columnIndex + 1, RowIndex = rowIndex };
                    c = new TerrainVertexIndices { ColumnIndex = columnIndex, RowIndex = rowIndex + 1 };
                }
                else
                {
                    a = new TerrainVertexIndices { ColumnIndex = columnIndex, RowIndex = rowIndex };
                    b = new TerrainVertexIndices { ColumnIndex = columnIndex + 1, RowIndex = rowIndex };
                    c = new TerrainVertexIndices { ColumnIndex = columnIndex + 1, RowIndex = rowIndex + 1 };
                }
            }
            else
            {
                if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
                {
                    a = new TerrainVertexIndices { ColumnIndex = columnIndex, RowIndex = rowIndex + 1 };
                    c = new TerrainVertexIndices { ColumnIndex = columnIndex + 1, RowIndex = rowIndex + 1 };
                    b = new TerrainVertexIndices { ColumnIndex = columnIndex + 1, RowIndex = rowIndex };
                }
                else
                {
                    a = new TerrainVertexIndices { ColumnIndex = columnIndex, RowIndex = rowIndex };
                    b = new TerrainVertexIndices { ColumnIndex = columnIndex + 1, RowIndex = rowIndex + 1 };
                    c = new TerrainVertexIndices { ColumnIndex = columnIndex, RowIndex = rowIndex + 1 };
                }
            }
        }
    }

    /// <summary>
    /// Defines how a Terrain organizes triangles in its quads.
    /// </summary>
    public enum QuadTriangleOrganization
    {
        /// <summary>
        /// Triangle with a right angle at the (-i,-j) position and another at the (+i,+j) position.
        /// </summary>
        BottomLeftUpperRight,
        /// <summary>
        /// Triangle with a right angle at the (+i,-j) position and another at the high (-i,+j) position.
        /// </summary>
        BottomRightUpperLeft
    }
}
