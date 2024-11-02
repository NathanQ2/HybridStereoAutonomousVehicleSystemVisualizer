using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;

public struct NetworkFrame 
{
    public PoseObject[] PoseObjects { get; }

    public NetworkFrame(PoseObject[] poseObjects) {
        PoseObjects = poseObjects;
    }

    public static async Task<NetworkFrame?> FromSocketStream(Socket socket) {
        // GD.Print("BEGIN");
        byte[] sizeBuff = new byte[4];
        int bytesRead = await socket.ReceiveAsync(sizeBuff, SocketFlags.None);
        // GD.Print("1");
        if (bytesRead != 4){
            // GD.Print($"It did not read 4 bytes... {bytesRead}");

            return null;
        }

        uint buffSize = BitConverter.ToUInt32(sizeBuff);
        if (buffSize == 0)
            return null;

        GD.Print($"buffSize: {buffSize}");

        byte[] buff = new byte[buffSize];
        bytesRead = await socket.ReceiveAsync(buff, SocketFlags.None);
        GD.Print("2");
        if (bytesRead != buffSize) {
            // GD.Print($"It did not read the right bytes! 222 {bytesRead}");

            // return null;
        }

        // int type = BitConverter.ToInt32(buff[0..4]);
        // GD.Print($"Type: {type}");

        bytesRead = PoseObject.PoseObjectsFromBytes(buff, out PoseObject[] objects);
        if (bytesRead != buffSize) {
            // GD.Print($"It did not read the right bytes! 333 {bytesRead}");
            return null;
        }
        
        GD.Print("END");
        return new NetworkFrame(objects);
    }
}
