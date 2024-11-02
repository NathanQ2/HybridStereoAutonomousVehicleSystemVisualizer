@tool

extends Node3D

@onready var positionText: Label3D = $PositionText

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	positionText.text = "(%0.2f, %0.2f, %0.2f)" % [position.x, position.y, position.z]
