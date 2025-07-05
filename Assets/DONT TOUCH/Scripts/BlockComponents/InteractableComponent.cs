using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, SelectionBase, DisallowMultipleComponent]
public class InteractableComponent : SchematicBlock
{
	public override BlockType BlockType { get; } = BlockType.Interactable;
	public ColliderShape ColliderShape;
	[Min(0)] public float InteractionDuration = 1;
	public bool IsLocked;

	private Collider _currentCollider;
	private ColliderShape _lastShape;

	private void Reset()
	{
		Init();
	}

	public void Init()
	{
		EditorApplication.delayCall += () =>
		{
			if (_currentCollider != null)
			{
				DestroyImmediate(_currentCollider);
				_currentCollider = null;
			}
			switch (ColliderShape)
			{
				case ColliderShape.Box:
					_currentCollider = gameObject.AddComponent<BoxCollider>();
					break;
				case ColliderShape.Sphere:
					_currentCollider = gameObject.AddComponent<SphereCollider>();
					break;
				case ColliderShape.Capsule:
					var capsule = gameObject.AddComponent<CapsuleCollider>();
					capsule.height = 2f;
					capsule.radius = 0.5f;
					_currentCollider = capsule;
					break;
			}
			_currentCollider.hideFlags = HideFlags.HideInInspector;
		};
	}

	private void OnValidate()
	{
		if (_lastShape == ColliderShape) return;
		_lastShape = ColliderShape;
		
		Init();
	}

	public override bool Compile(SchematicBlockData block, Schematic _)
	{
		block.BlockType = BlockType;

		block.Properties = new Dictionary<string, object>
		{
			{ "Shape", ColliderShape },
			{ "InteractionDuration", InteractionDuration },
			{ "IsLocked", IsLocked },
		};

		return true;
	}
}