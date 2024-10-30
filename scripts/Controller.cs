using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public partial class Controller : Node
{
	private IPEndPoint m_IpEndpoint = new IPEndPoint(IPAddress.Loopback, 5006);
	private Socket m_Socket;
	private Task m_ConnectTask;

	public Label m_DebugLabel;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		m_DebugLabel = GetNode<Label>("DebugLabel");

		m_Socket = new Socket(
			m_IpEndpoint.AddressFamily,
			SocketType.Stream,
			ProtocolType.Tcp
		);

		// TODO: Make this async better
		GD.Print("Waiting for connection...");
		m_DebugLabel.Text = "Status: Waiting for connection\n";
		await m_Socket.ConnectAsync(m_IpEndpoint);
		m_DebugLabel.Text += "Connected!\n";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		byte[] buff = new byte[4];
		await m_Socket.ReceiveAsync(buff, SocketFlags.None);
		int i = BitConverter.ToUInt16(buff);

		StringBuilder hex = new StringBuilder(buff.Length * 2);
		foreach (byte b in buff)
			hex.AppendFormat("{0:x2}", b);

		m_DebugLabel.Text = $"Result: {i}\nHex: {hex}";
		GD.Print($"Result: {i} Hex: {hex}");
	}
}
