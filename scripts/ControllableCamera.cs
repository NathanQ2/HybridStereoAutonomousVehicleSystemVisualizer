using Godot;
using System;

public partial class ControllableCamera : Node3D
{
    private Camera3D m_Cam;

    private float m_OrbitRadius = 1;
    private float m_Sensitivity = 0.005f;
    private Vector3 m_OrbitPoint;
    private Vector3 m_OrbitRotation;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        m_Cam = GetChild<Camera3D>(0);
        m_OrbitPoint = new Vector3();
        m_OrbitRotation = new Vector3();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        UpdateCamera();
    }

    private bool m_MouseIsDragging = false;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion && m_MouseIsDragging) {
            m_OrbitRotation += m_Sensitivity * new Vector3(eventMouseMotion.Relative.Y, eventMouseMotion.Relative.X, 0);
        }

        if (@event is InputEventMouseButton eventMouseButton) {
            if (!m_MouseIsDragging && eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed) {
                m_MouseIsDragging = true;
            }

            if (m_MouseIsDragging && eventMouseButton.ButtonIndex == MouseButton.Left && !eventMouseButton.Pressed) {
                m_MouseIsDragging = false;
            }
        }
    }

    private void UpdateCamera() {
        m_OrbitRotation.X = Math.Clamp(m_OrbitRotation.X, -MathF.PI / 2 + MathF.PI / 12, MathF.PI / 2 - MathF.PI / 12);
        
        float newX = m_OrbitPoint.X + m_OrbitRadius * MathF.Sin(m_OrbitRotation.Y) * MathF.Cos(m_OrbitRotation.X);
        float newY = m_OrbitPoint.Y + m_OrbitRadius * MathF.Sin(m_OrbitRotation.X);
        float newZ = m_OrbitPoint.Z + m_OrbitRadius * -MathF.Cos(m_OrbitRotation.Y) * MathF.Cos(m_OrbitRotation.X);
        
        m_Cam.Position = new Vector3(newX, newY, newZ);
        m_Cam.LookAt(m_OrbitPoint, Vector3.Up);

        // GD.Print($"CamPos: ({m_Cam.Position.X}, {m_Cam.Position.Y}, {m_Cam.Position.Z}) CamRot: ({m_Cam.Rotation.X}, {m_Cam.Rotation.Y}, {m_Cam.Rotation.Z})");
        // GD.Print($"OrbitPos: ({m_OrbitPoint.X}, {m_OrbitPoint.Y}, {m_OrbitPoint.Z}) OrbitRotation: ({m_OrbitRotation.X}, {m_OrbitRotation.Y}, {m_OrbitRotation.Z})");
    }
}
