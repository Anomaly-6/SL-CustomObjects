using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Decompiler
{
    public class SchematicBuilder : MonoBehaviour
    {
        public bool TryGetBlockFromType(BlockType blockType, out SchematicBlock schematicBlock) => Dict.TryGetValue(blockType, out schematicBlock);

        private readonly Dictionary<BlockType, SchematicBlock> Dict = new();

        public SchematicBuilder Init()
        {
            Dict.Add(BlockType.Empty, gameObject.AddComponent<EmptyComponent>());
            Dict.Add(BlockType.Primitive, gameObject.AddComponent<PrimitiveComponent>());
            Dict.Add(BlockType.Light, gameObject.AddComponent<LightComponent>());
            Dict.Add(BlockType.Pickup, gameObject.AddComponent<PickupComponent>());
            Dict.Add(BlockType.Workstation, gameObject.AddComponent<WorkstationComponent>());
            Dict.Add(BlockType.Teleport, gameObject.AddComponent<TeleportComponent>());
            Dict.Add(BlockType.Locker, gameObject.AddComponent<LockerComponent>());
            Dict.Add(BlockType.Text, gameObject.AddComponent<TextComponent>());
            Dict.Add(BlockType.Interactable, gameObject.AddComponent<InteractableComponent>());
            Dict.Add(BlockType.Waypoint, gameObject.AddComponent<WaypointComponent>());
            Dict.Add(BlockType.Door, gameObject.AddComponent<DoorComponent>());
            return this;
		}
	}
    
    private static SchematicBuilder _schematicBuilder;

    [MenuItem("SchematicManager/Import Schematic/JSON")]
    private static void ImportSchematicJson()
    {
        string importPath = SchematicManager.Config.ExportPath;
        if (!Directory.Exists(importPath))
            Directory.CreateDirectory(importPath);

        string jsonFilePath = EditorUtility.OpenFilePanelWithFilters("Select json with the schemaitc", importPath, new string[] { "Schematic", "json" });
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Debug.LogError("Invalid schematic file. Path is empty.");
            return;
        }

        _schematicDirectoryPath = null;
        _schematicName = Path.GetFileNameWithoutExtension(jsonFilePath);
        _schematicData = JsonConvert.DeserializeObject<SchematicObjectDataList>(File.ReadAllText(jsonFilePath));

        PortBack();
    }

    [MenuItem("SchematicManager/Import Schematic/Folder")]
    private static void ImportSchematicFolder()
    {
        string importPath = SchematicManager.Config.ExportPath;
        if (!Directory.Exists(importPath))
            Directory.CreateDirectory(importPath);

        _schematicDirectoryPath = EditorUtility.OpenFolderPanel("Select folder with the schematic", importPath, "");
        if (string.IsNullOrEmpty(_schematicDirectoryPath))
        {
            Debug.LogError("Invalid schematic directory. Path is empty.");
            return;
        }

        _schematicName = Path.GetFileNameWithoutExtension(_schematicDirectoryPath);
        string jsonFilePath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}.json");
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("No json file found in the schematic directory!");
            return;
        }

        _schematicData = JsonConvert.DeserializeObject<SchematicObjectDataList>(File.ReadAllText(jsonFilePath));

        PortBack();
    }

    private static void PortBack()
    {
        _rootTransform = new GameObject(_schematicName).AddComponent<Schematic>().transform;
        _objectFromId = new Dictionary<int, Transform>(_schematicData.Blocks.Count + 1)
        {
            { _schematicData.RootObjectId, _rootTransform },
        };

        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Debug.Log("<color=#FFFF00>Importing schematic...</color>");

        _schematicBuilder = new GameObject("SchematicBuilder").AddComponent<SchematicBuilder>().Init();

        CreateRecursiveFromID(_schematicData.RootObjectId, _schematicData.Blocks, _rootTransform);

        if (_schematicDirectoryPath != null)
        {
            // CreateTeleporters();
            AddRigidbodies();
        }

        Debug.Log($"<color=#00FF00>Successfully imported <b>{_schematicName}</b> schematic in {stopwatch.ElapsedMilliseconds} ms!</color>");
        NullifyFields();

		Object.DestroyImmediate(_schematicBuilder.gameObject);
    }

		Debug.Log(
			$"<color=#00FF00>Successfully imported <b>{_schematicName}</b> schematic in {stopwatch.ElapsedMilliseconds} ms!</color>");
		NullifyFields();
	}

	private static void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
	{
		Transform childGameObjectTransform =
			CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ??
			_rootTransform; // Create the object first before creating children.
		int[] parentSchematics =
			blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

		// Gets all the ObjectIds of all the schematic blocks inside "blocks" argument.
		foreach (SchematicBlockData block in blocks.FindAll(c => c.ParentId == id))
		{
			if (parentSchematics.Contains(block
				    .ParentId)) // The block is a child of some schematic inside "parentSchematics" array, therefore it will be skipped to avoid spawning it and its children twice.
				continue;

			CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
		}
	}

        GameObject gameObject = null;

		if (_schematicBuilder.TryGetBlockFromType(block.BlockType, out SchematicBlock schematicBlock))
		{
			schematicBlock.Decompile(ref gameObject, block, rootObject);
			_objectFromId.Add(block.ObjectId, gameObject.transform);
		}

		if (_schematicDirectoryPath != null && TryGetAnimatorController(block.AnimatorName, out RuntimeAnimatorController animatorController))
            gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;

				return gameObject.transform;
			}
			case BlockType.Door:
			{
				foreach (GameObject blockPrefab in _blockPrefabs)
				{
					if (!blockPrefab.TryGetComponent(out DoorComponent doorComponent)) continue;
					if (doorComponent.DoorType != (DoorType)Convert.ToInt32(block.Properties["DoorType"])) continue;
					var door = Object.Instantiate(doorComponent, rootObject);
					gameObject = door.gameObject;
					door.name = block.Name;
					door.transform.localPosition = block.Position;
					door.transform.localEulerAngles = block.Rotation;
					door.transform.localScale = block.Scale;
					door.IsOpen = (bool)block.Properties["IsOpen"];
					door.IsLocked = (bool)block.Properties["IsLocked"];
					door.RequiredPermissions =
						(DoorPermissionFlags)Convert.ToUInt16(block.Properties["RequiredPermissions"]);
					door.RequireAll = (bool)block.Properties["RequireAll"];
					return gameObject.transform;
				}

				break;
			}
			case BlockType.Teleport:
			{
				foreach (GameObject blockPrefab in _blockPrefabs)
				{
					if (!blockPrefab.TryGetComponent(out TeleportComponent teleportComponent)) continue;
					var teleport = Object.Instantiate(teleportComponent, rootObject);
					gameObject = teleport.gameObject;
					teleport.name = block.Name;
					teleport.transform.localPosition = block.Position;
					teleport.transform.localEulerAngles = block.Rotation;
					teleport.transform.localScale = block.Scale;
					teleport.Cooldown = Convert.ToSingle(block.Properties["Cooldown"]);
				}

				break;
			}
			case BlockType.Interactable:
			{
				foreach (GameObject blockPrefab in _blockPrefabs)
				{
					if (!blockPrefab.TryGetComponent(out InteractableComponent interactableComponent)) continue;
					var interactable = Object.Instantiate(interactableComponent, rootObject);
					gameObject = interactable.gameObject;
					interactable.name = block.Name;
					interactable.transform.localPosition = block.Position;
					interactable.transform.localEulerAngles = block.Rotation;
					interactable.transform.localScale = block.Scale;
					interactable.ColliderShape =
						(ColliderShape)Enum.Parse(typeof(ColliderShape), block.Properties["Shape"].ToString());
					interactable.InteractionDuration = Convert.ToSingle(block.Properties["InteractionDuration"]);
					interactable.IsLocked = (bool)block.Properties["IsLocked"];
					interactable.Init();
				}

				break;
			}
			case BlockType.Text:
			{
				foreach (GameObject blockPrefab in _blockPrefabs)
				{
					if (!blockPrefab.TryGetComponent(out TextComponent textComponent)) continue;
					var instantiate = Object.Instantiate(textComponent, rootObject);
					gameObject = instantiate.gameObject;
					instantiate.name = block.Name;
					instantiate.transform.localPosition = block.Position;
					instantiate.transform.localEulerAngles = block.Rotation;
					instantiate.transform.localScale = block.Scale;
					instantiate.Text = Convert.ToString(block.Properties["Text"]);
				}

				break;
			}
			case BlockType.Camera:
			{
				object cameraType = Enum.Parse(typeof(CameraType), block.Properties["CameraType"].ToString());
				GameObject cameraBase = _blockPrefabs.FirstOrDefault(s => s.name.Contains(cameraType.ToString()));
				gameObject = Object.Instantiate(cameraBase, rootObject);
				gameObject.name = block.Name;
				gameObject.transform.localPosition = block.Position;
				gameObject.transform.localEulerAngles = block.Rotation;
				gameObject.transform.localScale = block.Scale;

				if (gameObject.TryGetComponent(out Scp079CameraComponent cameraComponent) && block.Properties != null)
				{
					cameraComponent.Label = Convert.ToString(block.Properties["Label"]);
				}

				return gameObject.transform;
			}
			case BlockType.ShootingTarget:
			{
				foreach (GameObject blockPrefab in _blockPrefabs)
				{
					if (!blockPrefab.TryGetComponent(out ShootingTargetComponent targetComponent)) continue;
					if (targetComponent.TargetType != (TargetType)Convert.ToInt32(block.Properties["TargetType"])) continue;
					var instantiate = Object.Instantiate(targetComponent, rootObject);
					gameObject = instantiate.gameObject;
					instantiate.name = block.Name;
					instantiate.transform.localPosition = block.Position;
					instantiate.transform.localEulerAngles = block.Rotation;
					instantiate.transform.localScale = block.Scale;
				}

				break;
			}
			case BlockType.PlayerSpawnPoint:
			{
				foreach (GameObject blockPrefab in _blockPrefabs)
				{
					if (!blockPrefab.TryGetComponent(out PlayerSpawnPointComponent playerSpawnPointComponent)) continue;
					var instantiate = Object.Instantiate(playerSpawnPointComponent, rootObject);
					gameObject = instantiate.gameObject;
					instantiate.name = block.Name;
					instantiate.transform.localPosition = block.Position;
					instantiate.transform.localEulerAngles = block.Rotation;
					instantiate.transform.localScale = block.Scale;
					foreach (var role in ((JArray)block.Properties["Roles"]).ToObject<List<RoleTypeId>>())
					{
						instantiate.Roles.Add(role);
					}
				}

    /*
    private static void CreateTeleporters()
    {
        string teleportPath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}-Teleports.json");
        if (!File.Exists(teleportPath))
            return;

				break;
			}
		}

		if (TryGetAnimatorController(block.AnimatorName, out animatorController))
			gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;

		return gameObject.transform;
	}

	private static bool TryGetAnimatorController(string animatorName, out RuntimeAnimatorController animatorController)
	{
		animatorController = null;

		if (!string.IsNullOrEmpty(animatorName))
		{
			Object animatorObject = AssetBundle.GetAllLoadedAssetBundles()
				.FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets()
				.First(x => x is RuntimeAnimatorController);

        foreach (TeleportComponent teleport in _rootTransform.GetComponentsInChildren<TeleportComponent>())
        {
            foreach (TargetTeleporter targetTeleporter in teleport.TargetTeleporters)
            {
                targetTeleporter.Teleporter = _objectFromId[targetTeleporter.Id].GetComponent<TeleportComponent>();
            }
        }
    }
    */

				if (!File.Exists(path))
					return false;

				animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets()
					.First(x => x is RuntimeAnimatorController);
			}

			animatorController = animatorObject as RuntimeAnimatorController;
			return true;
		}

    private static void NullifyFields()
    {
        _rootTransform = null;
        _schematicName = null;
        _schematicDirectoryPath = null;
        _schematicData = null;
        _objectFromId = null;
        AssetBundle.UnloadAllAssetBundles(false);
    }

    private static Transform _rootTransform;
    private static string _schematicName;
    private static string _schematicDirectoryPath;
    private static SchematicObjectDataList _schematicData;
    private static Dictionary<int, Transform> _objectFromId;
}

			if (source == null) continue;
			foreach (var target in ((JArray)block.Properties["Targets"]).ToObject<List<string>>())
			{
				foreach (var teleportComponent in teleports)
				{
					if (teleportComponent.name == (string)target)
					{
						source.TargetTeleporters.Add(teleportComponent);
					}
				}
			}
		}

		// string teleportPath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}-Teleports.json");
		// if (!File.Exists(teleportPath))
		//     return;
		//
		// foreach (SerializableTeleport teleport in JsonConvert.DeserializeObject<List<SerializableTeleport>>(File.ReadAllText(teleportPath)))
		// {
		//     GameObject gameObject = Object.Instantiate(_blockPrefabs.FirstOrDefault(x => x.name == "Teleporter"));
		//     gameObject.name = teleport.Name;
		//     gameObject.transform.parent = _objectFromId[teleport.ParentId];
		//     gameObject.transform.localPosition = teleport.Position;
		//     gameObject.transform.localEulerAngles = teleport.Rotation;
		//     gameObject.transform.localScale = teleport.Scale;
		//
		//     if (gameObject.TryGetComponent(out TeleportComponent teleportComponent))
		//     {
		//         // teleportComponent.TargetTeleporters = teleport.TargetTeleporters.ToArray();
		//         // teleportComponent.RoomType = teleport.RoomType;
		//         // teleportComponent.AllowedRoleTypes = teleport.AllowedRoles.ToArray();
		//         teleportComponent.Cooldown = teleport.Cooldown;
		//         // teleportComponent.TeleportFlags = teleport.TeleportFlags;
		//         // teleportComponent.LockOnEvent = teleport.LockOnEvent;
		//         // teleportComponent.SoundOnTeleport = teleport.TeleportSoundId;
		//
		//         // if (teleport.PlayerRotationX.HasValue)
		//         // {
		//         //     teleportComponent.OverridePlayerXRotation = true;
		//         //     teleportComponent.PlayerRotationX = teleport.PlayerRotationX.Value;
		//         // }
		//         //
		//         // if (teleport.PlayerRotationY.HasValue)
		//         // {
		//         //     teleportComponent.OverridePlayerYRotation = true;
		//         //     teleportComponent.PlayerRotationY = teleport.PlayerRotationY.Value;
		//         // }
		//     }
		//
		//     _objectFromId.Add(teleport.ObjectId, gameObject.transform);
		// }

		// foreach (TeleportComponent teleport in _rootTransform.GetComponentsInChildren<TeleportComponent>())
		// {
		//     foreach (TargetTeleporter targetTeleporter in teleport.TargetTeleporters)
		//     {
		//         targetTeleporter.Teleporter = _objectFromId[targetTeleporter.Id].GetComponent<TeleportComponent>();
		//     }
		// }
	}

	private static void AddRigidbodies()
	{
		string rigidbodyPath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}-Rigidbodies.json");
		if (!File.Exists(rigidbodyPath))
			return;

		foreach (KeyValuePair<int, SerializableRigidbody> dict in JsonConvert
			         .DeserializeObject<Dictionary<int, SerializableRigidbody>>(File.ReadAllText(rigidbodyPath)))
		{
			if (!_objectFromId[dict.Key].gameObject.TryGetComponent(out Rigidbody rigidbody))
				rigidbody = _objectFromId[dict.Key].gameObject.AddComponent<Rigidbody>();

			rigidbody.isKinematic = dict.Value.IsKinematic;
			rigidbody.useGravity = dict.Value.UseGravity;
			rigidbody.constraints = dict.Value.Constraints;
			rigidbody.mass = dict.Value.Mass;
		}
	}

	private static void NullifyFields()
	{
		_blockPrefabs = null;
		_rootTransform = null;
		_schematicName = null;
		_schematicDirectoryPath = null;
		_schematicData = null;
		_objectFromId = null;
		AssetBundle.UnloadAllAssetBundles(false);
	}

	private static List<GameObject> _blockPrefabs;
	private static Transform _rootTransform;
	private static string _schematicName;
	private static string _schematicDirectoryPath;
	private static SchematicObjectDataList _schematicData;
	private static Dictionary<int, Transform> _objectFromId;
}