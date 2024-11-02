using System;
using Godot;

public class StopSign : PoseObject
{
    public new static readonly int SerializedSize = sizeof(ObjectType) + sizeof(float) * 3;

    public StopSign(Vector3 position) : base(position, ObjectType.StopSign) {}

    public StopSign(float x, float y, float z) : this(new Vector3(x, y, z)) {}
    
    
    public static new int FromBytes(byte[] bytes, out PoseObject? obj)
    {
        if (bytes.Length < SerializedSize) {
            obj = null;
            return 0;
        }

        int arrayReadIndex = 0;
        // First 4 bytes should be object's type
        ObjectType type  = (ObjectType)BitConverter.ToInt16(bytes[arrayReadIndex..(arrayReadIndex + sizeof(ObjectType))]);
        arrayReadIndex += sizeof(ObjectType);

        if (type != ObjectType.StopSign) {
            obj = null;
            return 0;
        }

        // Get object's position
        float x = BitConverter.ToSingle(bytes[arrayReadIndex..(arrayReadIndex + sizeof(float))]);
        // GD.Print($"X: {x}");
        arrayReadIndex += sizeof(float);
        float y = BitConverter.ToSingle(bytes[arrayReadIndex..(arrayReadIndex + sizeof(float))]);
        // GD.Print($"Y: {y}");
        arrayReadIndex += sizeof(float);
        float z = BitConverter.ToSingle(bytes[arrayReadIndex..(arrayReadIndex + sizeof(float))]);
        // GD.Print($"Z: {z}");
        arrayReadIndex += sizeof(float);

        obj = new StopSign(x , y, z); 
        return arrayReadIndex;
    }
}
