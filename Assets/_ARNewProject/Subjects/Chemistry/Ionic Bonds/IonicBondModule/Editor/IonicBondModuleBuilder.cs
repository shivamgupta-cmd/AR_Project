#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class IonicBondModuleBuilder
{
    const string ModelPath = "Assets/IonicBondModule/Models/Iconic_Bond.fbx";

    [MenuItem("Tools/Ionic Bond/Create Complete Module Rig")]
    public static void Build()
    {
        GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (!modelAsset)
        {
            EditorUtility.DisplayDialog("Ionic Bond", "Iconic_Bond.fbx was not found at:\n" + ModelPath, "OK");
            return;
        }

        GameObject root = new GameObject("IONIC_BOND_MODULE");
        Undo.RegisterCreatedObjectUndo(root, "Create Ionic Bond Module");
        IonicBondModuleController controller = root.AddComponent<IonicBondModuleController>();

        GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
        model.name = "Iconic_Bond_Model";
        model.transform.SetParent(root.transform, false);

        controller.sodiumAtom = FindGO(model.transform, "Sodium_atom");
        controller.chlorineAtom = FindGO(model.transform, "Chlorine_atom");
        controller.sodiumCation = FindGO(model.transform, "Sodium_cation");
        controller.chlorideAnion = FindGO(model.transform, "Chloride_anion");
        controller.crystalLattice = FindGO(model.transform, "Crystal_Lattice");

        if (controller.sodiumAtom)
            controller.sodiumElectron = FindDirectOrRecursive(controller.sodiumAtom.transform, "Electron");

        // Electron target marker near chlorine atom center; move it to the desired outer shell position.
        GameObject electronTarget = new GameObject("Electron_Target_MOVE_TO_CHLORINE_OUTER_SHELL");
        electronTarget.transform.SetParent(root.transform, false);
        if (controller.chlorineAtom)
            electronTarget.transform.position = controller.chlorineAtom.transform.position + controller.chlorineAtom.transform.right * BoundsSize(controller.chlorineAtom) * 0.55f;
        controller.electronTarget = electronTarget.transform;

        // Attraction target markers.
        GameObject catT = new GameObject("NaPlus_Attraction_Target");
        GameObject anT = new GameObject("ClMinus_Attraction_Target");
        catT.transform.SetParent(root.transform, false);
        anT.transform.SetParent(root.transform, false);
        if (controller.sodiumCation && controller.chlorideAnion)
        {
            Vector3 mid = (controller.sodiumCation.transform.position + controller.chlorideAnion.transform.position) * 0.5f;
            Vector3 dir = (controller.chlorideAnion.transform.position - controller.sodiumCation.transform.position).normalized;
            catT.transform.position = mid - dir * 0.12f;
            anT.transform.position = mid + dir * 0.12f;
        }
        controller.cationAttractionTarget = catT.transform;
        controller.anionAttractionTarget = anT.transform;

        BuildElectronFX(controller, root.transform);
        BuildChargeIcons(controller, root.transform);
        BuildAttractionLine(controller, root.transform);
        BuildParticles(controller, root.transform);

        if (controller.crystalLattice)
            controller.crystalLattice.AddComponent<SlowRotate>().speed = 10f;

        Selection.activeGameObject = root;
        EditorUtility.DisplayDialog("Ionic Bond Module", "Complete rig created.\n\nImportant: move Electron_Target_MOVE_TO_CHLORINE_OUTER_SHELL to the exact receiving position on chlorine.\n\nUse PlayFullSequence() or Timeline Signal methods.", "OK");
    }

    static void BuildElectronFX(IonicBondModuleController c, Transform parent)
    {
        if (!c.sodiumElectron) return;

        // Trail on the actual FBX electron.
        TrailRenderer tr = c.sodiumElectron.gameObject.GetComponent<TrailRenderer>();
        if (!tr) tr = c.sodiumElectron.gameObject.AddComponent<TrailRenderer>();
        tr.time = 0.35f;
        tr.startWidth = 0.035f;
        tr.endWidth = 0f;
        tr.minVertexDistance = 0.01f;
        tr.emitting = false;
        tr.material = NewURPUnlit(new Color(1f,0.75f,0.05f,0.9f));
        Gradient g = new Gradient();
        g.SetKeys(
            new [] { new GradientColorKey(new Color(1f,0.9f,0.1f),0), new GradientColorKey(new Color(1f,0.25f,0.02f),1) },
            new [] { new GradientAlphaKey(1,0), new GradientAlphaKey(0,1) }
        );
        tr.colorGradient = g;
        c.electronTrail = tr;

        GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glow.name = "Electron_Highlight_Glow";
        Object.DestroyImmediate(glow.GetComponent<Collider>());
        glow.transform.SetParent(parent, false);
        glow.transform.localScale = Vector3.one * 0.16f;
        MeshRenderer mr = glow.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("IonicBond/Glow"));
        mat.SetColor("_BaseColor", new Color(1f,0.65f,0.02f,0.15f));
        mat.SetColor("_EmissionColor", new Color(1f,0.35f,0.02f,1));
        mat.SetFloat("_EmissionStrength", 5f);
        mat.SetFloat("_Alpha", 0.45f);
        mr.sharedMaterial = mat;
        glow.SetActive(false);
        c.electronHighlightGlow = glow;
    }

    static void BuildChargeIcons(IonicBondModuleController c, Transform parent)
    {
        c.positiveChargeIcon = MakeCharge("Na_Positive_Charge_Icon", true, parent, new Color(0.55f,0.1f,1f));
        c.negativeChargeIcon = MakeCharge("Cl_Negative_Charge_Icon", false, parent, new Color(0.1f,0.9f,0.15f));
        if (c.sodiumCation) c.positiveChargeIcon.transform.position = c.sodiumCation.transform.position + Vector3.up * BoundsSize(c.sodiumCation) * 0.7f;
        if (c.chlorideAnion) c.negativeChargeIcon.transform.position = c.chlorideAnion.transform.position + Vector3.up * BoundsSize(c.chlorideAnion) * 0.7f;
        c.positiveChargeIcon.SetActive(false);
        c.negativeChargeIcon.SetActive(false);
    }

    static GameObject MakeCharge(string name, bool plus, Transform parent, Color color)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        Material mat = NewURPUnlit(color);

        GameObject h = GameObject.CreatePrimitive(PrimitiveType.Cube);
        h.name = plus ? "Plus_H" : "Minus";
        Object.DestroyImmediate(h.GetComponent<Collider>());
        h.transform.SetParent(root.transform, false);
        h.transform.localScale = new Vector3(0.16f, 0.035f, 0.035f);
        h.GetComponent<MeshRenderer>().sharedMaterial = mat;

        if (plus)
        {
            GameObject v = GameObject.CreatePrimitive(PrimitiveType.Cube);
            v.name = "Plus_V";
            Object.DestroyImmediate(v.GetComponent<Collider>());
            v.transform.SetParent(root.transform, false);
            v.transform.localScale = new Vector3(0.035f, 0.16f, 0.035f);
            v.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
        root.transform.localScale = Vector3.one * 1.25f;
        return root;
    }

    static void BuildAttractionLine(IonicBondModuleController c, Transform parent)
    {
        GameObject go = new GameObject("Electrostatic_Attraction_Line");
        go.transform.SetParent(parent, false);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.material = NewURPUnlit(new Color(1f,0.35f,0.05f,0.9f));
        lr.textureMode = LineTextureMode.Tile;
        lr.enabled = false;
        c.attractionLine = lr;
    }

    static void BuildParticles(IonicBondModuleController c, Transform parent)
    {
        c.transferImpactFX = MakeBurst("Electron_Transfer_Impact", parent, new Color(1f,0.55f,0.05f), 22, 0.15f);
        c.ionFormationFX = MakeBurst("Ion_Formation_Flash", parent, new Color(0.5f,0.2f,1f), 35, 0.25f);
        c.latticeCompleteFX = MakeBurst("Lattice_Complete_FX", parent, new Color(0.1f,0.7f,1f), 50, 0.35f);
    }

    static ParticleSystem MakeBurst(string name, Transform parent, Color color, int count, float size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.6f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.25f,0.6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f,0.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(size*0.4f,size);
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new [] { new ParticleSystem.Burst(0f, (short)count) });
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.08f;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = NewURPUnlit(color);
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        return ps;
    }

    static Material NewURPUnlit(Color color)
    {
        Shader s = Shader.Find("Universal Render Pipeline/Unlit");
        if (!s) s = Shader.Find("Sprites/Default");
        Material m = new Material(s);
        m.color = color;
        return m;
    }

    static GameObject FindGO(Transform root, string name)
    {
        Transform t = FindDirectOrRecursive(root, name);
        return t ? t.gameObject : null;
    }

    static Transform FindDirectOrRecursive(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static float BoundsSize(GameObject go)
    {
        Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
        if (rs.Length == 0) return 0.5f;
        Bounds b = rs[0].bounds;
        for (int i=1;i<rs.Length;i++) b.Encapsulate(rs[i].bounds);
        return Mathf.Max(b.size.x, b.size.y, b.size.z);
    }
}
#endif
