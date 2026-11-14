using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class CodexTutorialSetup
{
    private const string Work = @"C:\Users\arthu\Documents\Codex\2026-11-14\doc\work\tutorial-setup";
    private const string Folder = "Assets/Scenes/LEVELS_FINAL/";
    static CodexTutorialSetup() { EditorApplication.delayCall += RunRequested; }

    private static T[] Components<T>(Scene scene) where T : Component =>
        scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<T>(true)).ToArray();

    private static GameObject Marker(Scene scene) => Components<Transform>(scene)
        .Single(t => t.name == "SHOOT-PLACE").gameObject;

    private static GameObject[] Targets(Scene scene) => Components<Transform>(scene)
        .Where(t => t.name.StartsWith("Target-", StringComparison.OrdinalIgnoreCase)).Select(t => t.gameObject).ToArray();

    private static Bounds BoundsOf(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) throw new Exception("No visible geometry on " + go.name);
        var bounds = renderers[0].bounds;
        foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
        return bounds;
    }

    private static void MapHierarchy(GameObject source, GameObject clone, Dictionary<Object, Object> map)
    {
        map[source] = clone;
        var a = source.GetComponents<Component>();
        var b = clone.GetComponents<Component>();
        if (a.Length != b.Length) throw new Exception("Component copy mismatch: " + source.name);
        for (int i = 0; i < a.Length; i++) if (a[i] != null) map[a[i]] = b[i];
        if (source.transform.childCount != clone.transform.childCount) throw new Exception("Hierarchy mismatch");
        for (int i = 0; i < source.transform.childCount; i++)
            MapHierarchy(source.transform.GetChild(i).gameObject, clone.transform.GetChild(i).gameObject, map);
    }

    private static void Rewire(Dictionary<Object, Object> map, Scene source)
    {
        foreach (var component in map.Values.OfType<Component>())
        {
            var serialized = new SerializedObject(component);
            var property = serialized.GetIterator();
            while (property.Next(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                var reference = property.objectReferenceValue;
                if (reference == null) continue;
                if (map.TryGetValue(reference, out var replacement)) property.objectReferenceValue = replacement;
                else if ((reference is GameObject go && go.scene == source) ||
                         (reference is Component c && c.gameObject.scene == source))
                    throw new Exception("Uncopied Tut1 scene dependency: " + component.name + "." + property.propertyPath);
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static void RemoveOldSetup(Scene scene)
    {
        var remove = new HashSet<GameObject>();
        foreach (var gm in Components<GameManager>(scene))
        {
            if (gm.player) remove.Add(gm.player);
            if (gm.shootPosition) remove.Add(gm.shootPosition);
            if (gm.finishUI) remove.Add(gm.finishUI.transform.root.gameObject);
            remove.Add(gm.gameObject);
        }
        foreach (var p in Components<Player>(scene)) remove.Add(p.gameObject);
        foreach (var e in Components<EventSystem>(scene)) remove.Add(e.gameObject);
        foreach (var c in Components<Canvas>(scene)) remove.Add(c.transform.root.gameObject);
        foreach (var t in Components<Transform>(scene))
        {
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject);
            if (path == "Assets/Prefabs/TUT_PSpwan.prefab") remove.Add(t.gameObject);
        }
        foreach (var camera in Components<Camera>(scene)) remove.Add(camera.gameObject);
        foreach (var go in remove.Where(g => g != null).ToArray())
            if (go != null && !remove.Any(other => other != null && other != go && go.transform.IsChildOf(other.transform)))
                Object.DestroyImmediate(go);
    }

    private static void ConfigureTarget(GameObject target)
    {
        target.tag = "Finish";
        foreach (var meshFilter in target.GetComponentsInChildren<MeshFilter>(true))
        {
            if (!meshFilter.sharedMesh) continue;
            var collider = meshFilter.GetComponent<MeshCollider>();
            if (!collider) collider = meshFilter.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.sharedMesh;
            collider.enabled = true;
            collider.isTrigger = false;
            meshFilter.gameObject.tag = "Finish";
        }
        foreach (var collider in target.GetComponentsInChildren<Collider>(true))
        {
            collider.gameObject.tag = "Finish";
            collider.enabled = true;
            collider.isTrigger = false;
        }
        if (!target.GetComponentsInChildren<Collider>(true).Any()) throw new Exception("No target collider: " + target.name);
    }

    private static void Validate(Scene scene)
    {
        var gm = Components<GameManager>(scene).Single();
        var player = Components<Player>(scene).Single();
        if (Components<EventSystem>(scene).Length != 1) throw new Exception("EventSystem count");
        if (Components<Camera>(scene).Count(c => c.enabled && c.gameObject.activeInHierarchy) != 1) throw new Exception("Camera count");
        if (Components<AudioListener>(scene).Count(c => c.enabled && c.gameObject.activeInHierarchy) != 1) throw new Exception("Listener count");
        if (gm.player != player.gameObject || !gm.shootPosition || !gm.finishUI || !gm.ballPrefab)
            throw new Exception("GameManager references missing");
        if (gm.player.scene != scene || gm.shootPosition.scene != scene || gm.finishUI.scene != scene)
            throw new Exception("Cross-scene gameplay reference");
        var bounds = BoundsOf(Marker(scene));
        var position = player.transform.position;
        if (Mathf.Abs(position.x - bounds.center.x) > 0.001f || Mathf.Abs(position.z - bounds.center.z) > 0.001f || position.y <= bounds.max.y)
            throw new Exception("Player is not above spawn marker");
        if (Vector3.Distance(position, gm.shootPosition.transform.position) > 0.001f) throw new Exception("Respawn differs from initial position");
        foreach (var target in Targets(scene))
            if (!target.CompareTag("Finish") || !target.GetComponentsInChildren<Collider>(true).Any(c => c.enabled && !c.isTrigger && c.CompareTag("Finish")))
                throw new Exception("Finish target wiring");
        foreach (var component in Components<Component>(scene).Where(c => c != null))
        {
            var serialized = new SerializedObject(component);
            var property = serialized.GetIterator();
            while (property.Next(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                var reference = property.objectReferenceValue;
                if (reference is GameObject go && go.scene.IsValid() && go.scene != scene) throw new Exception("Cross-scene reference " + component.name);
                if (reference is Component c && c.gameObject.scene.IsValid() && c.gameObject.scene != scene) throw new Exception("Cross-scene component reference " + component.name);
            }
        }
    }

    private static void RunRequested()
    {
        string request = Path.Combine(Work, "run.request");
        if (!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode) return;
        File.Move(request, Path.Combine(Work, "run.started"));
        var originalActive = SceneManager.GetActiveScene();
        var originallyLoaded = Enumerable.Range(0, SceneManager.sceneCount).Select(SceneManager.GetSceneAt).Select(s => s.path).ToHashSet();
        var report = new List<string>();
        Scene source = default;
        try
        {
            Directory.CreateDirectory(Path.Combine(Work, "backup"));
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var open = SceneManager.GetSceneAt(i);
                if (open.isDirty) EditorSceneManager.SaveScene(open, Path.Combine(Work, "backup", open.name + "-unsaved.unity"), true);
            }
            source = SceneManager.GetSceneByPath(Folder + "Tut1.unity");
            if (!source.isLoaded) source = EditorSceneManager.OpenScene(Folder + "Tut1.unity", OpenSceneMode.Additive);
            var sourceManager = Components<GameManager>(source).Single();
            var templateRoots = new HashSet<GameObject>
            {
                sourceManager.gameObject, sourceManager.player, sourceManager.shootPosition,
                sourceManager.finishUI.transform.root.gameObject
            };
            foreach (var canvas in Components<Canvas>(source)) templateRoots.Add(canvas.transform.root.gameObject);
            foreach (var e in Components<EventSystem>(source)) templateRoots.Add(e.gameObject);
            templateRoots.RemoveWhere(go => templateRoots.Any(other => other != go && go.transform.IsChildOf(other.transform)));
            var sourceMarker = Marker(source);
            var relativeRotation = Quaternion.Inverse(sourceMarker.transform.rotation) * sourceManager.player.transform.rotation;
            for (int level = 2; level <= 7; level++)
            {
                string path = Folder + "Tut" + level + ".unity";
                File.Copy(path, Path.Combine(Work, "backup", "Tut" + level + ".unity"), true);
                var scene = SceneManager.GetSceneByPath(path);
                if (!scene.isLoaded) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                SceneManager.SetActiveScene(scene);
                var marker = Marker(scene);
                var targets = Targets(scene);
                if (targets.Length != 1) throw new Exception(scene.name + " target count: " + targets.Length);
                var mapObjects = Components<Renderer>(scene).Where(r => !r.GetComponentInParent<Canvas>()).Select(r => r.gameObject).ToHashSet();
                RemoveOldSetup(scene);
                var map = new Dictionary<Object, Object>();
                foreach (var root in templateRoots)
                {
                    var clone = Object.Instantiate(root);
                    clone.name = root.name;
                    SceneManager.MoveGameObjectToScene(clone, scene);
                    MapHierarchy(root, clone, map);
                }
                Rewire(map, source);
                var manager = (GameManager)map[sourceManager];
                var player = manager.player.GetComponent<Player>();
                var bounds = BoundsOf(marker);
                var spawn = new Vector3(bounds.center.x, bounds.max.y + 0.75f, bounds.center.z);
                var rotation = marker.transform.rotation * relativeRotation;
                player.gameObject.name = "Player";
                player.transform.SetPositionAndRotation(spawn, rotation);
                player.canShoot = true;
                player.isBuilding = false;
                manager.shootPosition.name = "Spawn";
                manager.shootPosition.transform.SetPositionAndRotation(spawn, rotation);
                manager.finishUI.SetActive(false);
                foreach (var target in targets) ConfigureTarget(target);
                foreach (var original in mapObjects) if (!original) throw new Exception("Map renderer removed unexpectedly");
                Validate(scene);
                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene)) throw new Exception("Save failed " + scene.name);
                report.Add(scene.name + ": PASS; player/spawn=" + spawn.ToString("F3") + "; marker top=" + bounds.max.y.ToString("F3") + "; target=" + targets[0].name + "; target colliders=" + targets[0].GetComponentsInChildren<Collider>(true).Length);
                if (!originallyLoaded.Contains(path)) EditorSceneManager.CloseScene(scene, true);
            }
            // Include every configured tutorial after MainMenu while preserving unrelated entries.
            var build = EditorBuildSettings.scenes.ToList();
            for (int level = 1; level <= 7; level++)
            {
                string path = Folder + "Tut" + level + ".unity";
                var existing = build.FirstOrDefault(s => s.path == path);
                if (existing != null) existing.enabled = true;
                else build.Add(new EditorBuildSettingsScene(path, true));
            }
            EditorBuildSettings.scenes = build.ToArray();
            report.Add("COMPLETE: Tut2-Tut7 saved and build settings include Tut1-Tut7.");
            File.WriteAllLines(Path.Combine(Work, "result.txt"), report);
        }
        catch (Exception ex)
        {
            report.Add(ex.ToString());
            File.WriteAllLines(Path.Combine(Work, "error.txt"), report);
            Debug.LogException(ex);
        }
        finally
        {
            if (source.IsValid() && source.isLoaded && !originallyLoaded.Contains(source.path)) EditorSceneManager.CloseScene(source, true);
            if (originalActive.IsValid() && originalActive.isLoaded) SceneManager.SetActiveScene(originalActive);
        }
    }
}
