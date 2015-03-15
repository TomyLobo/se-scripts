void Main() {
    List<IMyTerminalBlock> gyros = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros);

    for (int i = 0; i < gyros.Count; i++) {
        IMyGyro gyro = gyros[i] as IMyGyro;
        //SerializableVector3 targetAngularVelocity = new SerializableVector3(-gyro.Yaw, -gyro.Pitch, -gyro.Roll);
        //gyro.TargetAngularVelocity = targetAngularVelocity;
        IMyCubeGrid cubeGrid = gyro.CubeGrid;
        
        //Matrix localMatrix = cubeGrid.LocalMatrix;
        //MatrixD worldMatrix = cubeGrid.WorldMatrix;
        //var phys = (cubeGrid as Sandbox.ModAPI.IMyEntity).Physics;
        //Matrix matrix = new Matrix();
        //var orientation = gyro.Orientation;
        //orientation.GetMatrix(out matrix);
        //throw new Exception(orientation.ToString());
        //throw new Exception(gyro.Yaw.ToString());
        var currentRotation = getRotation(gyro);
        //gyro.SetCustomName(currentRotation.ToString());
        //var targetRotation = Matrix.Identity; // invalid program -_-
        var targetRotation = new Matrix(
            1,0,0,0,
            0,1,0,0,
            0,0,1,0,
            0,0,0,1
        );
        var targetAngularVelocity = targetRotation * Matrix.Invert(currentRotation);
        // T=V*C
        // T*invC=(V*C)*invC
        // T*invC=V*(C*invC)
        // T*invC=V*I
        // T*invC=V
        setAngularVelocity(gyro, ref targetAngularVelocity);
        
        //Quaternion.CreateFromYawPitchRoll
        //quat.GetAxisAngle

        //gyro.Yaw = .1;
        /*
        GetPosition() on 3 blocks
        => Quaternion.FromForwardUp
        => distance to target quaternion
        => Matrix.GetEulerAnglesXYZ(mat, out xyz)
        => Euler Angles
        => gyro "Increase/DecreaseYaw/Pitch/Roll" actions
        => monitor effective gyro Yaw/Pitch/Roll properties
        */
    }

    //GridTerminalSystem.GetBlockWithName("DS-1 IngotContainer")
}

Matrix getRotation(IMyGyro gyro) {
/*    return getRotation(gyro.CubeGrid);
}

Matrix getRotation(IMyCubeGrid cubeGrid) {*/
    IMyCubeGrid cubeGrid = gyro.CubeGrid;
    Matrix matrix;
    gyro.Orientation.GetMatrix(out matrix);
    var backBlock = GridTerminalSystem.GetBlockWithName("Back");
    var bottomBlock = GridTerminalSystem.GetBlockWithName("Bottom");

    Vector3 forward = -(backBlock.GetPosition() - gyro.GetPosition());
    Vector3 up = -(bottomBlock.GetPosition() - gyro.GetPosition());
    //Vector3 forward = cubeGrid.GetCubeBlock(gyro.Position + matrix.Forward).GetPosition();
    //Vector3 up = -cubeGrid.GetCubeBlock(gyro.Position - matrix.Up).GetPosition();
    //Vector3 forward = transform(gyro, new Vector3(1, 0, 0));
    //Vector3 up = -transform(gyro, new Vector3(0, 0, -1));
    forward.Normalize();
    up.Normalize();

    return Matrix.CreateFromDir(forward, up);
}

Matrix getAngularVelocity(IMyGyro gyro) {
    return Matrix.CreateFromYawPitchRoll(gyro.Yaw, gyro.Pitch, gyro.Roll);
}

void setAngularVelocity(IMyGyro gyro, ref Matrix matrix) {
    Vector3 xyz = new Vector3(0,0,0);
    Matrix.GetEulerAnglesXYZ(ref matrix, out xyz);

    setAngularVelocity(
        gyro,
        xyz.GetDim(0),
        xyz.GetDim(1),
        xyz.GetDim(2)
    );
}

void setAngularVelocity(IMyGyro gyro, float targetYaw, float targetPitch, float targetRoll) {
    gyro.SetCustomName("targetYaw="+targetYaw+", targetPitch="+targetPitch+", targetRoll="+targetRoll+", gyro.Yaw="+gyro.Yaw+", gyro.Pitch="+gyro.Pitch+", gyro.Roll="+ gyro.Roll);
    setAngularVelocity(gyro, "Yaw", gyro.Yaw, targetYaw);
    setAngularVelocity(gyro, "Pitch", gyro.Pitch, targetPitch);
    setAngularVelocity(gyro, "Roll", gyro.Roll, targetRoll);
}

void setAngularVelocity(IMyGyro gyro, string angleComponentName, float currentAngle, float targetAngle) {
    //targetAngle = -targetAngle;
    //targetAngle *= 0.01f;
    //gyro.SetCustomName("acm="+angleComponentName+", cur="+currentAngle+", target="+targetAngle );
    if (targetAngle < currentAngle) {
        gyro.ApplyAction("Decrease" + angleComponentName);
    }
    else if (targetAngle > currentAngle) {
        gyro.ApplyAction("Increase" + angleComponentName);
    }
}

float narf(string x) {
    //throw new Exception(x.ToString());
    return float.Parse(x);
}

bool GetEulerAnglesXYZ(ref Matrix mat, out Vector3 xyz) {
    float num1 = narf(mat.M11.ToString());//GetRow(0).X;
    float num2 = narf(mat.M12.ToString());//GetRow(0).Y;
    float num3 = narf(mat.M13.ToString());//GetRow(0).Z;
    float num4 = narf(mat.M21.ToString());//GetRow(1).X;
    float num5 = narf(mat.M22.ToString());//GetRow(1).Y;
    float num6 = narf(mat.M23.ToString());//GetRow(1).Z;
    double num7 = (double) narf(mat.M31.ToString());//GetRow(2).X;
    double num8 = (double) narf(mat.M32.ToString());//GetRow(2).Y;
    float num9 = narf(mat.M33.ToString());//GetRow(2).Z;
    float num10 = num3;

    if ((double) num10 < 1.0) {
        if ((double) num10 > -1.0) {
            xyz = new Vector3(
                (float) Math.Atan2(-(double) num6, (double) num9),
                (float) Math.Asin((double) num3),
                (float) Math.Atan2(-(double) num2, (double) num1)
            );
            return true;
        }
        else {
            xyz = new Vector3(
                (float) -Math.Atan2((double) num4, (double) num5),
                -1.570796f,
                0.0f
            );
            return false;
        }
    }
    else {
        xyz = new Vector3(
            (float) Math.Atan2((double) num4, (double) num5),
            -1.570796f,
            0.0f
        );
        throw new Exception(xyz.X.ToString()+", "+xyz.Y.ToString()+", "+xyz.Z.ToString());
        return false;
    }
}
