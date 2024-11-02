using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using Godot;

public abstract class PoseObject {
    public enum ObjectType : Int32 {
        None = -1,
        StopSign,
        Warning,
        Regulatory
    }

    public static readonly int SerializedSize = sizeof(ObjectType) + sizeof(float) * 3;

    public Vector3 Position { get; protected set; }
    public ObjectType Type { get; protected set; }

    public PoseObject(Vector3 position, ObjectType type) {
        Position = position;
        Type = type;
    }

    public static int FromBytes(byte[] bytes, out PoseObject? obj) {
        if (bytes.Length < sizeof(ObjectType)) {
            obj = null;
            return 0;
        }
        
        ObjectType type  = (ObjectType)BitConverter.ToInt16(bytes[0..sizeof(ObjectType)]);
        GD.Print($"Print2: {type}");

        switch (type)
        {
            case ObjectType.StopSign:
                StopSign.FromBytes(bytes, out obj);

                return StopSign.SerializedSize;
            case ObjectType.Regulatory:
                SpeedLimitSign.FromBytes(bytes, out obj);

                return SpeedLimitSign.SerializedSize;
            case ObjectType.Warning:
                WarningSign.FromBytes(bytes, out obj);

                return WarningSign.SerializedSize;
            default:
                obj = null;
                return 0;
        };
    }

    public static int PoseObjectsFromBytes(byte[] bytes, out PoseObject[] objs) {
        int HandleUnknownType() {
            GD.PushWarning("An object is being deserialized form the network with unknown type!");
            return SerializedSize;
        }

        GD.Print($"bytesLen: {bytes.Length}");

        List<PoseObject> objects = new List<PoseObject>();

        int arrayReadPosition = 0;
        // While the amount of bytes left is greater than the minimum amount of bytes of a serialized object
        while (arrayReadPosition < bytes.Length) {
            // Get this object's type
            int typeInt = BitConverter.ToInt32(bytes[arrayReadPosition..(arrayReadPosition + sizeof(ObjectType))]);
            GD.Print($"Type {typeInt}");
            ObjectType type = (ObjectType)typeInt;
            int objectSize = type switch {
                ObjectType.StopSign => StopSign.SerializedSize,
                ObjectType.Regulatory => SpeedLimitSign.SerializedSize,
                ObjectType.Warning => WarningSign.SerializedSize,
                // Otherwise set to default serialized size and hope for the best!
                _ => HandleUnknownType()
            };

            GD.Print($"ObjSize: {objectSize}");

            arrayReadPosition += FromBytes(bytes[arrayReadPosition..(arrayReadPosition + objectSize)], out PoseObject obj);
            objects.Add(obj);
        }

        objs = objects.ToArray();
        return arrayReadPosition;
    }
}
