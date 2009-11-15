using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics
{
    public struct OrientedBoundingBox
    {
        public static OrientedBoundingBox Cube
        {
            get
            {
                return new OrientedBoundingBox(Vector3.One * -0.5f, Vector3.One * +0.5f);
            }
        }

        public static OrientedBoundingBox CreateFromBoundingBox(BoundingBox aabb)
        {
            return new OrientedBoundingBox(aabb);
        }

        private Vector3 _min;
        private Vector3 _max;
        private Vector3 _center;
        private Vector3 _extents;
        private Matrix _transforms;

        public Vector3 Min
        {
            get { return _min; }
            set { _min = value; UpdateFromMinMax(); }
        }
        public Vector3 Max
        {
            get { return _max; }
            set { _max = value; UpdateFromMinMax(); }
        }
        public Vector3 Center
        {
            get { return _center; }
        }
        public Vector3 Extents
        {
            get { return _extents; }
        }
        public Matrix Transforms
        {
            get { return _transforms; }
            set { _transforms = value; }
        }

        public OrientedBoundingBox(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;

            _center = Vector3.Zero;
            _extents = Vector3.Zero;
            _transforms = Matrix.Identity;

            UpdateFromMinMax();
        }

        public OrientedBoundingBox(BoundingBox AABB)
            : this(AABB.Min, AABB.Max)
        {

        }

        public void UpdateFromMinMax()
        {
            _center = (_min + _max) * 0.5f;
            _extents = (_max - _min) * 0.5f;
        }

        public Vector3[] ComputeCorners()
        {
            // Del max al min de arriba a abajo y de izquierda a derecha
            Vector3[] corners = new Vector3[8];

            corners[0] = Vector3.Transform(new Vector3(_max.X, _max.Y, _max.Z), _transforms);
            corners[1] = Vector3.Transform(new Vector3(_max.X, _max.Y, _min.Z), _transforms);
            corners[2] = Vector3.Transform(new Vector3(_max.X, _min.Y, _max.Z), _transforms);
            corners[3] = Vector3.Transform(new Vector3(_max.X, _min.Y, _min.Z), _transforms);

            corners[4] = Vector3.Transform(new Vector3(_min.X, _min.Y, _max.Z), _transforms);
            corners[5] = Vector3.Transform(new Vector3(_min.X, _max.Y, _min.Z), _transforms);
            corners[6] = Vector3.Transform(new Vector3(_min.X, _max.Y, _max.Z), _transforms);
            corners[7] = Vector3.Transform(new Vector3(_min.X, _min.Y, _min.Z), _transforms);


            return corners;
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            // Matrix to transform other OBB into my reference to allow me to be treated as an AABB
            Matrix toMe = other.Transforms * Matrix.Invert(_transforms);

            Vector3 centerOther = Vector3.Transform(other.Center, toMe);
            Vector3 extentsOther = other.Extents;
            Vector3 separation = centerOther - _center;

            Matrix3 rotations = new Matrix3(toMe);
            Matrix3 absRotations = Matrix3.Abs(rotations);

            float r, r0, r1, r01;

            //--- Test case 1 - X axis

            r = Math.Abs(separation.X);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(0));
            r01 = _extents.X + r1;

            if (r > r01) return false;

            //--- Test case 1 - Y axis

            r = Math.Abs(separation.Y);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(1));
            r01 = _extents.Y + r1;

            if (r > r01) return false;

            //--- Test case 1 - Z axis

            r = Math.Abs(separation.Z);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(2));
            r01 = _extents.Z + r1;

            if (r > r01) return false;

            //--- Test case 2 - X axis

            r = Math.Abs(Vector3.Dot(rotations.Row(0), separation));
            r0 = Vector3.Dot(_extents, absRotations.Row(0));
            r01 = r0 + extentsOther.X;

            if (r > r01) return false;

            //--- Test case 2 - Y axis

            r = Math.Abs(Vector3.Dot(rotations.Row(1), separation));
            r0 = Vector3.Dot(_extents, absRotations.Row(1));
            r01 = r0 + extentsOther.Y;

            if (r > r01) return false;

            //--- Test case 2 - Z axis

            r = Math.Abs(Vector3.Dot(rotations.Row(2), separation));
            r0 = Vector3.Dot(_extents, absRotations.Row(2));
            r01 = r0 + extentsOther.Z;

            if (r > r01) return false;

            //--- Test case 3 # 1

            r = Math.Abs(separation.Z * rotations[0, 1] - separation.Y * rotations[0, 2]);
            r0 = _extents.Y * absRotations[0, 2] + _extents.Z * absRotations[0, 1];
            r1 = extentsOther.Y * absRotations[2, 0] + extentsOther.Z * absRotations[1, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 2

            r = Math.Abs(separation.Z * rotations[1, 1] - separation.Y * rotations[1, 2]);
            r0 = _extents.Y * absRotations[1, 2] + _extents.Z * absRotations[1, 1];
            r1 = extentsOther.X * absRotations[2, 0] + extentsOther.Z * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 3

            r = Math.Abs(separation.Z * rotations[2, 1] - separation.Y * rotations[2, 2]);
            r0 = _extents.Y * absRotations[2, 2] + _extents.Z * absRotations[2, 1];
            r1 = extentsOther.X * absRotations[1, 0] + extentsOther.Y * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 4

            r = Math.Abs(separation.X * rotations[0, 2] - separation.Z * rotations[0, 0]);
            r0 = _extents.X * absRotations[0, 2] + _extents.Z * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 1] + extentsOther.Z * absRotations[1, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 5

            r = Math.Abs(separation.X * rotations[1, 2] - separation.Z * rotations[1, 0]);
            r0 = _extents.X * absRotations[1, 2] + _extents.Z * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 1] + extentsOther.Z * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 6

            r = Math.Abs(separation.X * rotations[2, 2] - separation.Z * rotations[2, 0]);
            r0 = _extents.X * absRotations[2, 2] + _extents.Z * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 1] + extentsOther.Y * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 7

            r = Math.Abs(separation.Y * rotations[0, 0] - separation.X * rotations[0, 1]);
            r0 = _extents.X * absRotations[0, 1] + _extents.Y * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 2] + extentsOther.Z * absRotations[1, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 8

            r = Math.Abs(separation.Y * rotations[1, 0] - separation.X * rotations[1, 1]);
            r0 = _extents.X * absRotations[1, 1] + _extents.Y * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 2] + extentsOther.Z * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 9

            r = Math.Abs(separation.Y * rotations[2, 0] - separation.X * rotations[2, 1]);
            r0 = _extents.X * absRotations[2, 1] + _extents.Y * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 2] + extentsOther.Y * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            return true;  // No separating axis, then we have intersection
        }

        public bool Intersects(BoundingFrustum frustum)
        {
            Vector3[] corners = ComputeCorners();

            foreach (Vector3 corner in corners)
            {
                if (frustum.Contains(corner) != ContainmentType.Disjoint)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string mask = @"Min:{0} Max:{1} Center:{2} Extents:{3}";

            return string.Format(mask, _min, _max, _center, _extents);
        }
    }
}
