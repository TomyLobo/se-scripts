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
        var targetRotation = new FloatMatrix3(
            1,0,0,
            0,1,0,
            0,0,1
        );
        var targetAngularVelocity = targetRotation * invertOrthogonalMatrix(currentRotation);
        // T*x=V*(C*x)
        // T*x=(V*C)*x
        // T=V*C
        // T*invC=(V*C)*invC
        // T*invC=V*(C*invC)
        // T*invC=V*I
        // T*invC=V
        //gyro.SetCustomName(currentRotation.ToString());
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

FloatMatrix3 getRotation(IMyGyro gyro) {
/*    return getRotation(gyro.CubeGrid);
}

FloatMatrix3 getRotation(IMyCubeGrid cubeGrid) {*/
    IMyCubeGrid cubeGrid = gyro.CubeGrid;
    /*
    Matrix matrix;
    gyro.Orientation.GetMatrix(out matrix);
    */
    var backBlock = GridTerminalSystem.GetBlockWithName("Back");
    var bottomBlock = GridTerminalSystem.GetBlockWithName("Bottom");

    FloatVector3 forward = -(v(backBlock.GetPosition()) - v(gyro.GetPosition()));
    FloatVector3 up = -(v(bottomBlock.GetPosition()) - v(gyro.GetPosition()));
    //Vector3 forward = cubeGrid.GetCubeBlock(gyro.Position + matrix.Forward).GetPosition();
    //Vector3 up = -cubeGrid.GetCubeBlock(gyro.Position - matrix.Up).GetPosition();
    //Vector3 forward = transform(gyro, new Vector3(1, 0, 0));
    //Vector3 up = -transform(gyro, new Vector3(0, 0, -1));
    /*
    forward.Normalize();
    up.Normalize();
    */

    return FloatMatrix3.CreateFromForwardUp(forward, up);
}

/*
FloatMatrix3 getAngularVelocity(IMyGyro gyro) {
    return FloatMatrix3.CreateFromYawPitchRoll(gyro.Yaw, gyro.Pitch, gyro.Roll);
}
*/

void setAngularVelocity(IMyGyro gyro, ref FloatMatrix3 matrix) {
    FloatVector3 xyz = matrix.GetEulerAnglesXYZ();
    /*
    rollen verändert pitch(y)
    yaw verändert roll(z)
    pitch verändert yaw(x)
    */

    /*
    rollen verändert yaw(x)
    yaw verändert pitch(y)
    pitch verändert roll(z)
    */
    setAngularVelocity(
        gyro,
        -xyz.y,
        -xyz.z,
        -xyz.x
    );
}

void setAngularVelocity(IMyGyro gyro, float targetYaw, float targetPitch, float targetRoll) {
    gyro.SetCustomName("targetYaw="+targetYaw+", targetPitch="+targetPitch+", targetRoll="+targetRoll/*+", gyro.Yaw="+gyro.Yaw+", gyro.Pitch="+gyro.Pitch+", gyro.Roll="+ gyro.Roll*/);
    setAngularVelocity(gyro, "Yaw", gyro.Yaw, targetYaw);
    setAngularVelocity(gyro, "Pitch", gyro.Pitch, targetPitch);
    setAngularVelocity(gyro, "Roll", gyro.Roll, targetRoll);
}

void setAngularVelocity(IMyGyro gyro, string angleComponentName, float currentAngle, float targetAngle) {
    //targetAngle = -targetAngle;
    targetAngle *= 0.25f;
    //gyro.SetCustomName("acm="+angleComponentName+", cur="+currentAngle+", target="+targetAngle );
    /*if (targetAngle < currentAngle) {
        gyro.ApplyAction("Decrease" + angleComponentName);
    }
    else if (targetAngle > currentAngle) {
        gyro.ApplyAction("Increase" + angleComponentName);
    }*/
    gyro.SetValueFloat(angleComponentName, targetAngle);
}

FloatVector3 v(Vector3 vector) {
    return new FloatVector3(vector.GetDim(0), vector.GetDim(1), vector.GetDim(2));
}

FloatMatrix3 invertOrthogonalMatrix(FloatMatrix3 matrix) {
    return matrix.transpose();
}
