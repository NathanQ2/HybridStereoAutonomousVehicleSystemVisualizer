using System;
using System.Runtime.InteropServices;
using Godot;

public struct PoseObject {
    public enum ObjectType : Int16 {
        None = -1,
        StopSign,
        Warning,
        Regulatory
    }

    public ObjectType Type { get; private set;}
    public Vector3 Position { get; private set;}

    public PoseObject(ObjectType type, Vector3 position) {
        Type = type;
        Position = position;
    }

    public PoseObject() {
        Type = ObjectType.None;
        Position = new Vector3();
    }

    public static PoseObject? FromBytes(byte[] bytes) {
        PoseObject obj = new();
        if (bytes.Length < Marshal.SizeOf(obj)) 
            return null;

        // First 4 bytes should be object's type
        obj.Type = (ObjectType)BitConverter.ToInt16(bytes[0..sizeof(ObjectType)]);

        return null;
    }
}