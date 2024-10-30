using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public partial class Controller : Node
{
	private IPEndPoint m_IpEndpoint = new IPEndPoint(IPAddress.Loopback, 5006);
	private Socket m_Socket;
	private Task m_ConnectTask;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		m_Socket = new Socket(
			m_IpEndpoint.AddressFamily,
			SocketType.Stream,
			ProtocolType.Tcp
		);

		// TODO: Make this async better
		GD.Print("Waiting for connection...");
		m_ConnectTask = m_Socket.ConnectAsync(m_IpEndpoint);
		GD.Print("Connected.");

		var test = new NetworkFrame();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if(m_ConnectTask.IsCompleted) {
			byte[] buff = new byte[2];
			await m_Socket.ReceiveAsync(buff, SocketFlags.None);
			int i = BitConverter.ToInt16(buff);
			GD.Print($"Result: {i}");
		}
	}
}
