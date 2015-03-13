using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opMatrix
{
    class FloatVector3
    {
        float x =0;
        float y =0;
        float z =0;


        public FloatVector3(float X, float Y, float Z)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
        }
        #region operator
        public static FloatVector3 operator /(FloatVector3 v1, FloatVector3 v2)
        {
            return new FloatVector3(
                v1.x / v2.x,
                v1.y / v2.y,
                v1.z / v2.z
                );
        }
        public static FloatVector3 operator /(FloatVector3 v1, float f)
        {
            return new FloatVector3
            (
                v1.x / f,
                v1.y / f,
                v1.z / f
            );
        }
        public static FloatVector3 operator /(float f, FloatVector3 v1)
        {
            return new FloatVector3
            (
                v1.x / f,
                v1.y / f,
                v1.z / f
            );
        }


        public static FloatVector3 operator -(FloatVector3 v1, FloatVector3 v2)
        {
            return new FloatVector3
            (
                v1.x - v2.x,
                v1.y - v2.y,
                v1.z - v2.z
            );
        }
        public static FloatVector3 operator -(FloatVector3 v1, float f)
        {
            return new FloatVector3
            (
                v1.x - f,
                v1.y - f,
                v1.z - f
            );
        }
        public static FloatVector3 operator -(float f, FloatVector3 v1)
        {
            return new FloatVector3
            (
                v1.x - f,
                v1.y - f,
                v1.z - f
            );
        }
        public static FloatVector3 operator *(FloatVector3 v1, FloatVector3 v2)
        {
            return new FloatVector3
            (
                v1.x * v2.x,
                v1.y * v2.y,
                v1.z * v2.z
            );
        }
        public static FloatVector3 operator *(FloatVector3 v1, float f)
        {
            return new FloatVector3
            (
                v1.x * f,
                v1.y * f,
                v1.z * f
            );
        }
        public static FloatVector3 operator *(float f, FloatVector3 v1)
        {
            return new FloatVector3
            (
                v1.x * f,
                v1.y * f,
                v1.z * f
            );
        }
        public static FloatVector3 operator /(FloatVector3 v1, FloatVector3 v2)
        {
            return new FloatVector3
            (
                v1.x / v2.x,
                v1.y / v2.y,
                v1.z / v2.z
            );
        }
        public static FloatVector3 operator /(FloatVector3 v1, float f)
        {
            return new FloatVector3
            (
                v1.x / f,
                v1.y / f,
                v1.z / f
            );
        }
        public static FloatVector3 operator -(FloatVector3 v)
        {
            return v * -1;
        }
       

        public float dot( FloatVector3 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public FloatVector3 cross(FloatVector3 other)
        {
            return new FloatVector3
            (
                y * other.z - z * other.y,
                z * other.x - x * other.z,
                x * other.y - y * other.x
            );
        }
        #endregion operator
        public float length2()
        {
            return x * x + y * y + z * z;
        }

        public float length()
        {
            return (float)Math.Sqrt(length2());
        }
        public void normalize()
        {
            float len = length();
            this.x = x / len;
            this.y = y / len;
            this.z = z / len;
        }
        public FloatVector3 normalized(FloatVector3 v)
        {
            v.normalize();
            return v;
        }
    }
}
