
using System;
using Godot;

public class SpeedLimitSign : PoseObject {
    public new static readonly int SerializedSize = sizeof(ObjectType) + sizeof(float) * 3 + sizeof(int);

    public readonly int Speed;

    public SpeedLimitSign(Vector3 position, int speed) : base(position, ObjectType.Regulatory) {
        Speed = speed;
    }

    public SpeedLimitSign(float x, float y, float z, int speed) : this(new Vector3(x, y, z), speed) {}

    public static new int FromBytes(byte[] bytes, out PoseObject? obj) {
        if (bytes.Length < SerializedSize) {
            obj = null;
            return 0;
        }
        

        int arrayReadIndex = 0;
        // First 4 bytes should be object's type
        ObjectType type  = (ObjectType)BitConverter.ToInt16(bytes[arrayReadIndex..(arrayReadIndex + sizeof(ObjectType))]);
        arrayReadIndex += sizeof(ObjectType);

        if (type != ObjectType.Regulatory) {
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

        int speed = BitConverter.ToInt32(bytes[arrayReadIndex..(arrayReadIndex + sizeof(int))]);
        arrayReadIndex += sizeof(int);

        obj = new SpeedLimitSign(x , y, z, speed); 
        return SerializedSize;
    }
}
