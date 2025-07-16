using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class TextComponent : SchematicBlock
{
	public override BlockType BlockType { get; } = BlockType.Text;
	[TextArea(3,20)]
	public string Text;

	private void Start()
	{
		GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
		GetComponent<TextMeshPro>().hideFlags = HideFlags.HideInInspector;
	}

	public override bool Compile(SchematicBlockData block, Schematic _)
	{
		block.BlockType = BlockType;
		block.Properties = new Dictionary<string, object>()
		{
			{ "Text", Text }
		};
		return true;
	}
	
	public void OnValidate()
	{
		GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
		GetComponent<TextMeshPro>().hideFlags = HideFlags.HideInInspector;
		GetComponent<TextMeshPro>().text = Text;
	}
}