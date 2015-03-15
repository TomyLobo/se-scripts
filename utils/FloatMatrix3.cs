using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opMatrix {
    class FloatMatrix3 {
        private float[] data = new float[9];

        public FloatMatrix3() {
            for (int i = 0; i < 9; ++i) data[i] = 0;
        }

        public FloatMatrix3(FloatVector3 a0, FloatVector3 a1, FloatVector3 a2) {
            data[0] = a0.x; data[1] = a1.x; data[2] = a2.x;
            data[3] = a0.y; data[4] = a1.y; data[5] = a2.y;
            data[6] = a0.z; data[7] = a1.z; data[8] = a2.z;
        }

        public FloatMatrix3(
            float a00, float a01, float a02,
            float a10, float a11, float a12,
            float a20, float a21, float a22
        ) {
            data[0] = a00; data[1] = a01; data[2] = a02;
            data[3] = a10; data[4] = a11; data[5] = a12;
            data[6] = a20; data[7] = a21; data[8] = a22;
        }

        public static FloatMatrix3 CreateFromForwardUp(FloatVector3 forward, FloatVector3 up) {
            forward = forward.normalized();
            up = up.normalized();
            return new FloatMatrix3(forward, up, forward.cross(up));
        }

        #region accessors
        public float this[int row, int col] {
            get {
                //TODO: row column check einbauen
                return data[row * 3 + col];
            }

            set {
                //TODO: row column check einbauen
                data[row * 3 + col] = value;
            }
        }
        #endregion accessors

        #region utils
        public FloatMatrix3 transpose() {
            FloatMatrix3 ret = new FloatMatrix3();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    ret[i, j] = data[i * 3 + j];
            return ret;
        }
        #endregion utils

        #region MatrixOpsMatrix
        public static FloatMatrix3 operator *(FloatMatrix3 v1, FloatMatrix3 v2) {
            FloatMatrix3 A = v1;
            for (int i = 0; i<3; ++i)
                for (int j = 0; j<3; ++j) {
                    v1[i,j] = 0;
                    for (int r = 0; r < 3; ++r) {
                        v1[i, j] += A[i, r] * v2[r, j];
                    }
                }
            return v1;
        }
        #endregion MatrixOpsMatrix

        #region MatrixOpsVector
        public FloatVector3 operator *(FloatMatrix3 m, FloatVector3 v) {
            FloatVector3 ret = new FloatVector3();
            for (int i = 0; i < 3; ++i) {
                ret[i] = 0;
                for (int r = 0; r < 3; ++r) {
                    ret[i] += m[i,r] * v[r];
                }
            }
            return ret;
        }
        #endregion MatrixOpsVector

        public FloatVector3 GetEulerAnglesXYZ() {
            float m00 = this[0, 0];
            float m01 = this[0, 1];
            float m02 = this[0, 2];
            float m10 = this[1, 0];
            float m11 = this[1, 1];
            float m12 = this[1, 2];
            float m20 = this[2, 0];
            float m21 = this[2, 1];
            float m22 = this[2, 2];

            if (m02 < 1.0f) {
                if (m02 > -1.0f) {
                    return new FloatVector3(
                    (float) Math.Atan2(-m12, m22),
                    (float) Math.Asin(m02),
                    (float) Math.Atan2(-m01, m00)
                    );
                }
                else {
                    return new FloatVector3(
                    -(float) Math.Atan2(m10, m11),
                        -1.570796f,
                        0.0f
                    );
                }
            }
            else {
                return new FloatVector3(
                (float) Math.Atan2(m10, m11),
                    -1.570796f,
                    0.0f
                );
            }
        }
    }
}
