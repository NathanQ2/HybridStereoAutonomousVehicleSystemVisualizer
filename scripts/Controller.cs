using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public partial class Controller : Node
{
    // Size of a packet sent over network
    private static readonly int PacketSizeBytes = 512;

    private IPEndPoint m_IpEndpoint = new IPEndPoint(IPAddress.Loopback, 5006);
    private Socket m_Socket;
    private Task m_ConnectTask;

    public Label m_DebugLabel;

    private PackedScene m_NodeStop;
    private PackedScene m_NodeSpeedLimit;
    private PackedScene m_NodeWarning;

    private List<Node3D> m_StopSigns;
    private List<Node3D> m_SpeedLimitSings;
    private List<Node3D> m_WarningSigns;

    private Task<NetworkFrame?> m_NetworkFrameTask = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Load sign scenes
        // m_NodeStop = GD.Load<PackedScene>("res://scenes/stopSign.tscn");
        // m_NodeSpeedLimit = GD.Load<PackedScene>("res://scenes/speedLimitSign.tscn");
        // m_NodeWarning = GD.Load<PackedScene>("res://scenes/warningSign.tscn");

        // m_StopSigns = new List<Node3D>();
        // m_SpeedLimitSings = new List<Node3D>();
        // m_WarningSigns = new List<Node3D>();

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
            }

            m_NetworkFrameTask = NetworkFrame.FromSocketStream(m_Socket);
        }

        // InstantiateNodes(frame.PoseObjects);
    }

    private void InstantiateNodes(PoseObject[] poseObjects) {
        // Get amount of each sign in this array
        int stopSignCount = poseObjects.Where(obj => obj.Type == PoseObject.ObjectType.StopSign).Count();
        int speedLimitSignCount = poseObjects.Where(obj => obj.Type == PoseObject.ObjectType.Regulatory).Count();
        int warningSignCount = poseObjects.Where(obj => obj.Type == PoseObject.ObjectType.Warning).Count();

        m_StopSigns.EnsureCapacity(stopSignCount);
        m_SpeedLimitSings.EnsureCapacity(speedLimitSignCount);
        m_WarningSigns.EnsureCapacity(warningSignCount);
    }
}
