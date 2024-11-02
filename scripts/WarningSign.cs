using System;
using Godot;

public class WarningSign : PoseObject {
    public static new readonly int SerializedSize = sizeof(ObjectType) + sizeof(float) * 3;

    public WarningSign(Vector3 position) : base(position, ObjectType.Warning) {}

    public WarningSign(float x, float y, float z) : this(new Vector3(x, y, z)) {}

    public static new int FromBytes(byte[] bytes, out PoseObject? obj) {
        if (bytes.Length < SerializedSize) {
            obj = null;
            return 0;
        }

        int arrayReadIndex = 0;
        // First 4 bytes should be object's type
        ObjectType type  = (ObjectType)BitConverter.ToInt16(bytes[arrayReadIndex..(arrayReadIndex + sizeof(ObjectType))]);
        arrayReadIndex += sizeof(ObjectType);

        if (type != ObjectType.Warning) {
            obj = null;
            return 0;
        }

        // Get object's position
        float x = BitConverter.ToSingle(bytes[arrayReadIndex..(arrayReadIndex + sizeof(float))]);
        arrayReadIndex += sizeof(float);
        float y = BitConverter.ToSingle(bytes[arrayReadIndex..(arrayReadIndex + sizeof(float))]);
        arrayReadIndex += sizeof(float);
        float z = BitConverter.ToSingle(bytes[arrayReadIndex..(arrayReadIndex + sizeof(float))]);
        arrayReadIndex += sizeof(float);

        obj = new WarningSign(x , y, z); 
        return SerializedSize;
    }
}
