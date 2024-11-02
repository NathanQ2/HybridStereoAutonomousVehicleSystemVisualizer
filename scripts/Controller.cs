using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public partial class Controller : Node
{
    private IPEndPoint m_IpEndpoint = new IPEndPoint(IPAddress.Loopback, 5006);
    private Socket m_Socket;
    private Task m_ConnectTask;

    public Label m_DebugLabel;

    private PackedScene m_NodeStop;
    private PackedScene m_NodeSpeedLimit;
    private PackedScene m_NodeWarning;

    private List<Node3D> m_StopSigns;
    private List<Node3D> m_SpeedLimitSigns;
    private List<Node3D> m_WarningSigns;

    private Task<NetworkFrame?> m_NetworkFrameTask = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Load sign scenes
        m_NodeStop = GD.Load<PackedScene>("res://scenes/stopSign.tscn");
        m_NodeSpeedLimit = GD.Load<PackedScene>("res://scenes/speedLimitSign.tscn");
        m_NodeWarning = GD.Load<PackedScene>("res://scenes/warningSign.tscn");

        m_StopSigns = new List<Node3D>();
        m_SpeedLimitSigns = new List<Node3D>();
        m_WarningSigns = new List<Node3D>();

        m_DebugLabel = GetNode<Label>("DebugLabel");

        m_Socket = new Socket(
            m_IpEndpoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        // TODO: Make this async better
        GD.Print("Waiting for connection...");
        // m_DebugLabel.Text = "Status: Waiting for connection\n";
        m_Socket.Connect(m_IpEndpoint);
        // m_DebugLabel.Text += "Connected!\n";
        GD.Print("Connected!");
        
        m_NetworkFrameTask = NetworkFrame.FromSocketStream(m_Socket);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // GD.Print($"Status: {NetworkFrame.IsRunningFromSocketStream}");

        if (m_NetworkFrameTask.IsCompleted) {
            NetworkFrame? frame = m_NetworkFrameTask.Result;
            if (frame.HasValue) {
                int i = frame.Value.PoseObjects.Length;
                m_DebugLabel.Text = $"PoseObjects Len: {i}\n";

                foreach (PoseObject obj in frame.Value.PoseObjects) {
                    GD.Print($"OBJ {obj.Type}:\n  X: {obj.Position.X}\n  Y: {obj.Position.Y}\n  Z: {obj.Position.Z}");
                    if (obj.Type == PoseObject.ObjectType.Regulatory) {
                        GD.Print($"  Speed: {((SpeedLimitSign)obj).Speed}");
                    }
                }

                InstantiateNodes(frame.Value.PoseObjects);
            }
            // else {
            //     InstantiateNodes
            // }

            m_NetworkFrameTask = NetworkFrame.FromSocketStream(m_Socket);
        }

        
    }

    private void InstantiateNodes(PoseObject[] poseObjects) {
        PoseObject[] stopSigns = poseObjects.Where(obj => obj.Type == PoseObject.ObjectType.StopSign).ToArray();
        PoseObject[] speedLimitSigns = poseObjects.Where(obj => obj.Type == PoseObject.ObjectType.Regulatory).ToArray();
        PoseObject[] warningSigns = poseObjects.Where(obj => obj.Type == PoseObject.ObjectType.Warning).ToArray();
        // Get amount of each sign in this array
        int stopSignCount = stopSigns.Length;
        int speedLimitSignCount = speedLimitSigns.Length;
        int warningSignCount = warningSigns.Length;

        if (stopSignCount > m_StopSigns.Capacity)
            m_StopSigns.EnsureCapacity(stopSignCount);
        if (speedLimitSignCount > m_SpeedLimitSigns.Capacity)
            m_SpeedLimitSigns.EnsureCapacity(speedLimitSignCount);
        if (warningSignCount > m_WarningSigns.Count)
            m_WarningSigns.EnsureCapacity(warningSignCount);


        for (int i = 0; i < stopSignCount; i++) {
            if (i >= m_StopSigns.Count) {
                m_StopSigns.Add(m_NodeStop.Instantiate<Node3D>());
                AddChild(m_StopSigns[^1]);
            }

            m_StopSigns[i].Position = stopSigns[i].Position;
        }

        for (int i = 0; i < speedLimitSignCount; i++) {
            if (i >= m_SpeedLimitSigns.Count) {
                m_SpeedLimitSigns.Add(m_NodeSpeedLimit.Instantiate<Node3D>());
                AddChild(m_SpeedLimitSigns[^1]);
            }

            m_SpeedLimitSigns[i].Position = speedLimitSigns[i].Position; 
        }

        for (int i = 0; i < warningSignCount; i++) {
            if (i >= m_WarningSigns.Count) {
                m_WarningSigns.Add(m_NodeWarning.Instantiate<Node3D>());
                AddChild(m_WarningSigns[^1]);
            }

            m_WarningSigns[i].Position = warningSigns[i].Position;
        }
    }
}
