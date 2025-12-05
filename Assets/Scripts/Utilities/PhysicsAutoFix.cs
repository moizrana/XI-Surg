using System.Collections.Generic;
using UnityEngine;

namespace Meducator.Utilities
{
	public class PhysicsAutoFix : MonoBehaviour
	{
		[Header("Fix Options")]
		public bool makeMeshCollidersConvex = true;
		public bool replacePlanesWithBoxCollider = true;
		public bool onlyOnStart = true;
		public bool includeInactive = true;

		[Header("Logging")]
		public bool logEachFix = true;

		private readonly List<Component> _fixedComponents = new List<Component>();

		private void Start()
		{
			RunFixes();
			if (onlyOnStart)
			{
				enabled = false;
			}
		}

		[ContextMenu("Run Physics Auto Fixes")]
		public void RunFixes()
		{
			_fixedComponents.Clear();

			// Find all MeshColliders under dynamic (non-kinematic) rigidbodies
			var rigidbodies = FindObjectsOfType<Rigidbody>(includeInactive);
			foreach (var rb in rigidbodies)
			{
				if (rb.isKinematic) continue; // not dynamic, skip

				var meshColliders = rb.GetComponentsInChildren<MeshCollider>(includeInactive);
				foreach (var mc in meshColliders)
				{
					if (mc == null) continue;

					// Heuristic: Unity "Plane" mesh is concave and invalid for dynamic rigidbodies unless convex
					bool isUnityPlane = mc.sharedMesh != null && mc.sharedMesh.name == "Plane";

					if (replacePlanesWithBoxCollider && isUnityPlane)
					{
						ReplaceWithBox(mc);
						continue;
					}

					if (makeMeshCollidersConvex && !mc.convex)
					{
						mc.convex = true;
						_fixedComponents.Add(mc);
						if (logEachFix)
							Debug.Log($"[PhysicsAutoFix] Set MeshCollider convex on: {GetHierarchyPath(mc.transform)}", mc);
					}
				}
			}

			if (logEachFix && _fixedComponents.Count > 0)
			{
				Debug.Log($"[PhysicsAutoFix] Fixed {_fixedComponents.Count} collider components.");
			}
		}

		private void ReplaceWithBox(MeshCollider meshCollider)
		{
			var t = meshCollider.transform;
			// Attempt to size a BoxCollider to the renderer bounds
			var renderer = t.GetComponent<Renderer>();
			var box = t.gameObject.AddComponent<BoxCollider>();
			if (renderer != null)
			{
				var bounds = renderer.bounds;
				// Convert world bounds to local space size
				Vector3 localCenter = t.InverseTransformPoint(bounds.center);
				Vector3 localSize = t.InverseTransformVector(bounds.size);
				box.center = localCenter;
				box.size = new Vector3(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y), Mathf.Abs(localSize.z));
			}
			else
			{
				// Fallback: reasonable default thin box for a plane
				box.size = new Vector3(1f, 0.02f, 1f);
			}

			if (logEachFix)
				Debug.Log($"[PhysicsAutoFix] Replaced Plane MeshCollider with BoxCollider on: {GetHierarchyPath(t)}", meshCollider);

			Destroy(meshCollider);
			_fixedComponents.Add(box);
		}

		private static string GetHierarchyPath(Transform t)
		{
			var stack = new Stack<string>();
			while (t != null)
			{
				stack.Push(t.name);
				t = t.parent;
			}
			return string.Join("/", stack.ToArray());
		}
	}
}



