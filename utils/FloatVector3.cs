public class FloatVector3 {
    private float[] data = new float[3];

    public float x {
        get { return data[0]; }
        set { data[0] = value; }
    }

    public float y {
        get { return data[1]; }
        set { data[1] = value; }
    }

    public float z {
        get { return data[2]; }
        set { data[2] = value; }
    }

    public FloatVector3() { }

    public FloatVector3(float x, float y, float z) {
        data[0] = x;
        data[1] = y;
        data[2] = z;
    }

    public float this[int index] {
        get {
            // TODO: index check einbauen
            return data[index];
        }

        set {
            data[index] = value;
        }
    }

    #region operator
    public static FloatVector3 operator /(FloatVector3 v1, FloatVector3 v2) {
        return new FloatVector3(
            v1.x / v2.x,
            v1.y / v2.y,
            v1.z / v2.z
        );
    }

    public static FloatVector3 operator /(FloatVector3 v1, float f) {
        return new FloatVector3(
            v1.x / f,
            v1.y / f,
            v1.z / f
        );
    }

    public static FloatVector3 operator /(float f, FloatVector3 v1) {
        return new FloatVector3(
            v1.x / f,
            v1.y / f,
            v1.z / f
        );
    }

    public static FloatVector3 operator -(FloatVector3 v1, FloatVector3 v2) {
        return new FloatVector3(
            v1.x - v2.x,
            v1.y - v2.y,
            v1.z - v2.z
        );
    }

    public static FloatVector3 operator -(FloatVector3 v1, float f) {
        return new FloatVector3(
            v1.x - f,
            v1.y - f,
            v1.z - f
        );
    }

    public static FloatVector3 operator -(float f, FloatVector3 v1) {
        return new FloatVector3(
            v1.x - f,
            v1.y - f,
            v1.z - f
        );
    }

    public static FloatVector3 operator *(FloatVector3 v1, FloatVector3 v2) {
        return new FloatVector3(
            v1.x * v2.x,
            v1.y * v2.y,
            v1.z * v2.z
        );
    }

    public static FloatVector3 operator *(FloatVector3 v1, float f) {
        return new FloatVector3(
            v1.x * f,
            v1.y * f,
            v1.z * f
        );
    }

    public static FloatVector3 operator *(float f, FloatVector3 v1) {
        return new FloatVector3(
            v1.x * f,
            v1.y * f,
            v1.z * f
        );
    }

    public static FloatVector3 operator -(FloatVector3 v) {
        return v * -1;
    }

    public float dot(FloatVector3 other) {
        return x * other.x + y * other.y + z * other.z;
    }

    public FloatVector3 cross(FloatVector3 other) {
        return new FloatVector3(
            y * other.z - z * other.y,
            z * other.x - x * other.z,
            x * other.y - y * other.x
        );
    }
    #endregion operator

    public float length2() {
        return x * x + y * y + z * z;
    }

    public float length() {
        return (float) Math.Sqrt(length2());
    }

    public void normalize() {
        float len = length();
        x /= len;
        y /= len;
        z /= len;
    }

    public FloatVector3 normalized() {
        this.normalize();
        return this;
    }
}
