using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class FPSSceneBuilder
{
    public const string ScenePath = "Assets/Scenes/BodycamTraining.unity";
    private static Material concrete, darkConcrete, metal, blueMetal, wood, rubber, skin, glass, orange, white, black;

    public static void CreateScene()
    {
        DirectoryEnsure();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateMaterials();
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(.28f,.34f,.42f);
        RenderSettings.ambientEquatorColor = new Color(.20f,.22f,.23f);
        RenderSettings.ambientGroundColor = new Color(.075f,.08f,.09f);
        RenderSettings.fog = true; RenderSettings.fogColor = new Color(.30f,.34f,.38f); RenderSettings.fogDensity = .0035f;

        BuildEnvironment();
        FPSPlayerController player = BuildPlayer();
        BuildEnemy(player.transform);
        BuildLighting();
        BuildHUD(player);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    }

    private static void DirectoryEnsure(){if(!System.IO.Directory.Exists("Assets/Scenes"))System.IO.Directory.CreateDirectory("Assets/Scenes");}

    private static void CreateMaterials()
    {
        concrete = Mat("Weathered concrete",new Color(.34f,.36f,.36f),0,.08f);
        darkConcrete = Mat("Dark concrete",new Color(.16f,.18f,.19f),0,.12f);
        metal = Mat("Gunmetal",new Color(.075f,.085f,.09f),.78f,.72f);
        blueMetal = Mat("Industrial blue",new Color(.075f,.17f,.22f),.55f,.38f);
        wood = Mat("Crate wood",new Color(.30f,.22f,.14f),0,.15f);
        rubber = Mat("Black polymer",new Color(.035f,.04f,.042f),.1f,.25f);
        skin = Mat("Tactical gloves",new Color(.16f,.19f,.18f),0,.2f);
        glass = Mat("Smoked glass",new Color(.055f,.11f,.14f),.5f,.75f);
        orange = Mat("Safety orange",new Color(.75f,.24f,.055f),.15f,.35f);
        white = Mat("HUD white",Color.white,0,0);
        black = Mat("HUD black",new Color(.012f,.016f,.02f),0,0);
    }

    private static Material Mat(string name,Color color,float metallicValue,float smooth)
    {
        Shader shader=Shader.Find("Standard");if(shader==null)shader=Shader.Find("Universal Render Pipeline/Lit");if(shader==null)shader=Shader.Find("Unlit/Color");
        Material m=new Material(shader);m.name=name;m.color=color;
        if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallicValue);
        if(m.HasProperty("_Glossiness"))m.SetFloat("_Glossiness",smooth);
        return m;
    }

    private static GameObject Primitive(PrimitiveType type,string name,Transform parent,Vector3 pos,Vector3 scale,Material mat,bool collider=false)
    {
        GameObject g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=scale;
        Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;
        if(!collider){Collider c=g.GetComponent<Collider>();if(c!=null)Object.DestroyImmediate(c);}
        return g;
    }

    private static GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat)
    {
        GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.position=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=mat;return g;
    }

    private static void BuildEnvironment()
    {
        GameObject warehousePrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Prefabs/Imported/Abandoned_Warehouse.prefab");
        if(warehousePrefab==null)throw new System.Exception("Imported Abandoned Warehouse prefab is missing. Run Bodycam > Prepare Imported Assets.");
        GameObject warehouse=PrefabUtility.InstantiatePrefab(warehousePrefab) as GameObject;
        if(warehouse==null)throw new System.Exception("Could not instantiate imported warehouse.");
        warehouse.name="Abandoned Warehouse Environment";
        warehouse.transform.position=new Vector3(-7.08f,0f,10.5f);
        warehouse.transform.rotation=Quaternion.identity;
        warehouse.isStatic=true;
        int colliderCount=0;
        foreach(MeshFilter filter in warehouse.GetComponentsInChildren<MeshFilter>(true))
        {
            if(filter.sharedMesh==null)continue;
            MeshCollider meshCollider=filter.GetComponent<MeshCollider>();
            if(meshCollider==null)meshCollider=filter.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh=filter.sharedMesh;
            colliderCount++;
        }
        if(colliderCount<10)throw new System.Exception("Warehouse collision generation failed: only "+colliderCount+" mesh colliders.");

        // The player starts inside the imported warehouse; preserve a small outdoor apron beyond its south entrance.
        GameObject apron=new GameObject("Exterior asphalt apron");
        MeshFilter apronFilter=apron.AddComponent<MeshFilter>();
        Mesh apronMesh=new Mesh();apronMesh.name="Exterior asphalt ground mesh";
        apronMesh.vertices=new Vector3[]{new Vector3(-24,0,-12),new Vector3(24,0,-12),new Vector3(-24,0,0),new Vector3(24,0,0)};
        apronMesh.triangles=new int[]{0,2,1,2,3,1};apronMesh.RecalculateNormals();apronFilter.sharedMesh=apronMesh;
        MeshRenderer apronRenderer=apron.AddComponent<MeshRenderer>();apronRenderer.sharedMaterial=darkConcrete;
        MeshCollider apronCollider=apron.AddComponent<MeshCollider>();apronCollider.sharedMesh=apronMesh;

        Debug.Log("Imported warehouse integrated: "+colliderCount+" mesh colliders; model offset (-7.08, 0, 10.5).");
    }

    private static FPSPlayerController BuildPlayer()
    {
        GameObject root=new GameObject("FPS Player");root.tag="Player";root.transform.position=new Vector3(0,0,-1.2f);
        CharacterController cc=root.AddComponent<CharacterController>();cc.height=1.72f;cc.radius=.34f;cc.center=Vector3.up*.86f;cc.slopeLimit=48;cc.stepOffset=.28f;
        FPSPlayerController player=root.AddComponent<FPSPlayerController>();
        GameObject pivotObject=new GameObject("Bodycam View Pivot");pivotObject.transform.SetParent(root.transform,false);pivotObject.transform.localPosition=new Vector3(0,1.48f,0);
        player.viewPivot=pivotObject.transform;
        GameObject cameraObject=new GameObject("Bodycam Camera");cameraObject.tag="MainCamera";cameraObject.transform.SetParent(pivotObject.transform,false);
        Camera cam=cameraObject.AddComponent<Camera>();cam.fieldOfView=84;cam.nearClipPlane=.045f;cam.farClipPlane=180;cam.allowHDR=false;cam.allowMSAA=true;
        cameraObject.AddComponent<AudioListener>();

        GameObject weaponMount=new GameObject("First Person Viewmodel Mount");weaponMount.transform.SetParent(cameraObject.transform,false);weaponMount.transform.localPosition=Vector3.zero;
        const string pistolPath="Assets/Art/Prefabs/Imported/FPS_Pistol_Arms.prefab";
        const string riflePath="Assets/Art/Prefabs/Imported/FPS_AK74M_Arms.prefab";
        GameObject pistolSource=AssetDatabase.LoadAssetAtPath<GameObject>(pistolPath);
        GameObject rifleSource=AssetDatabase.LoadAssetAtPath<GameObject>(riflePath);
        if(pistolSource==null||rifleSource==null)throw new System.Exception("Imported first-person weapon prefabs are missing. Run Bodycam > Prepare Imported Assets first.");
        GameObject pistol=PrefabUtility.InstantiatePrefab(pistolSource) as GameObject;
        GameObject akm=PrefabUtility.InstantiatePrefab(rifleSource) as GameObject;
        if(pistol==null||akm==null)throw new System.Exception("Could not instantiate imported FPS weapon prefabs.");
        pistol.name="Pistol View Model";pistol.transform.SetParent(weaponMount.transform,false);pistol.transform.localPosition=new Vector3(.12f,-.27f,.45f);pistol.transform.localRotation=Quaternion.identity;
        akm.name="AKM View Model";akm.transform.SetParent(weaponMount.transform,false);akm.transform.localPosition=new Vector3(0,-.27f,.45f);akm.transform.localRotation=Quaternion.identity;
        Animator rifleAnimator=akm.GetComponentInChildren<Animator>(true);
        if(rifleAnimator==null)throw new System.Exception("Imported AK74M prefab has no Animator component.");

        FPSBodycamEffects effects=cameraObject.AddComponent<FPSBodycamEffects>();effects.SetWeaponRoot(weaponMount.transform);
        FPSWeapon weapon=root.AddComponent<FPSWeapon>();weapon.aimCamera=cam;weapon.player=player;weapon.cameraEffects=effects;weapon.pistolModel=pistol;weapon.akmModel=akm;
        weapon.pistolMuzzleFlash=CreateMuzzleFlash(pistol.transform,new Vector3(.06f,.06f,.44f),"Pistol Muzzle Flash");
        weapon.akmMuzzleFlash=CreateMuzzleFlash(akm.transform,new Vector3(.05f,.015f,1.02f),"AKM Muzzle Flash");
        weapon.audioSource=cameraObject.AddComponent<AudioSource>();weapon.audioSource.playOnAwake=false;weapon.audioSource.spatialBlend=0;
        weapon.armsAnimator=rifleAnimator;
        return player;
    }

    private static GameObject CreateMuzzleFlash(Transform parent,Vector3 localPosition,string objectName)
    {
        GameObject flash=new GameObject(objectName);flash.transform.SetParent(parent,false);flash.transform.localPosition=localPosition;
        Light light=flash.AddComponent<Light>();light.type=LightType.Point;light.color=new Color(1f,.48f,.12f);light.range=2.8f;light.intensity=3.5f;
        flash.SetActive(false);return flash;
    }

    private static void BuildGun(string name,Transform parent,bool rifle)
    {
        GameObject root=new GameObject(name);root.transform.SetParent(parent,false);root.transform.localPosition=rifle?new Vector3(0,0,0):new Vector3(0,0,0);
        if(rifle)
        {
            Primitive(PrimitiveType.Cube,"Receiver",root.transform,new Vector3(0,0,0),new Vector3(.12f,.14f,.42f),metal);
            Primitive(PrimitiveType.Cube,"Handguard",root.transform,new Vector3(0,.015f,.32f),new Vector3(.105f,.12f,.32f),rubber);
            Primitive(PrimitiveType.Cylinder,"Barrel",root.transform,new Vector3(0,.015f,.62f),new Vector3(.025f,.025f,.19f),metal).transform.localRotation=Quaternion.Euler(90,0,0);
            Primitive(PrimitiveType.Cylinder,"Muzzle brake",root.transform,new Vector3(0,.015f,.80f),new Vector3(.04f,.035f,.055f),metal).transform.localRotation=Quaternion.Euler(90,0,0);
            Primitive(PrimitiveType.Cube,"AKM stock",root.transform,new Vector3(0,-.015f,-.30f),new Vector3(.10f,.13f,.30f),wood);
            Primitive(PrimitiveType.Cube,"AKM magazine",root.transform,new Vector3(0,-.17f,.02f),new Vector3(.09f,.24f,.15f),rubber).transform.localRotation=Quaternion.Euler(-9,0,0);
            Primitive(PrimitiveType.Cube,"Pistol grip",root.transform,new Vector3(0,-.17f,-.08f),new Vector3(.085f,.23f,.10f),wood).transform.localRotation=Quaternion.Euler(-12,0,0);
            Primitive(PrimitiveType.Cube,"Rear sight",root.transform,new Vector3(0,.11f,-.05f),new Vector3(.08f,.06f,.08f),metal);
            Primitive(PrimitiveType.Cube,"Front sight",root.transform,new Vector3(0,.105f,.58f),new Vector3(.035f,.09f,.04f),metal);
            for(int i=0;i<5;i++)Primitive(PrimitiveType.Cube,"Handguard groove "+i,root.transform,new Vector3(0,.082f,.18f+i*.055f),new Vector3(.11f,.012f,.018f),metal);
        }
        else
        {
            Primitive(PrimitiveType.Cube,"Pistol slide",root.transform,new Vector3(0,.055f,.12f),new Vector3(.105f,.085f,.30f),metal);
            Primitive(PrimitiveType.Cube,"Pistol frame",root.transform,new Vector3(0,-.015f,.08f),new Vector3(.09f,.07f,.27f),rubber);
            Primitive(PrimitiveType.Cube,"Pistol grip",root.transform,new Vector3(0,-.13f,-.015f),new Vector3(.085f,.21f,.10f),rubber).transform.localRotation=Quaternion.Euler(-12,0,0);
            Primitive(PrimitiveType.Cylinder,"Pistol barrel",root.transform,new Vector3(0,.06f,.29f),new Vector3(.022f,.022f,.11f),metal).transform.localRotation=Quaternion.Euler(90,0,0);
            Primitive(PrimitiveType.Cube,"Rear sight",root.transform,new Vector3(0,.115f,-.005f),new Vector3(.065f,.035f,.035f),metal);
            Primitive(PrimitiveType.Cube,"Front sight",root.transform,new Vector3(0,.115f,.24f),new Vector3(.025f,.035f,.025f),metal);
        }
        GameObject flash=Primitive(PrimitiveType.Sphere,"Muzzle Flash",root.transform,
            rifle?new Vector3(0,.015f,.88f):new Vector3(0,.06f,.40f),
            rifle?new Vector3(.10f,.10f,.20f):new Vector3(.075f,.075f,.14f),orange);
        Light flashLight=flash.AddComponent<Light>();flashLight.type=LightType.Point;flashLight.color=new Color(1f,.48f,.12f);flashLight.range=2.8f;flashLight.intensity=3.5f;
        flash.SetActive(false);
    }

    private static void BuildArms(Transform camera,Transform mount)
    {
        Transform rightGrip=NewPoint("Right hand IK target",mount,new Vector3(0,-.13f,.04f));
        Transform leftGrip=NewPoint("Left hand IK target",mount,new Vector3(-.13f,-.07f,.34f));
        CreateArmIK(camera,"Right arm IK",new Vector3(.25f,-.20f,.30f),rightGrip,new Vector3(.10f,-.18f,.05f));
        CreateArmIK(camera,"Left arm IK",new Vector3(-.25f,-.20f,.30f),leftGrip,new Vector3(-.10f,-.16f,.05f));
    }
    private static Transform NewPoint(string name,Transform parent,Vector3 pos){GameObject g=new GameObject(name);g.transform.SetParent(parent,false);g.transform.localPosition=pos;return g.transform;}
    private static void CreateArmIK(Transform camera,string name,Vector3 shoulder,Transform target,Vector3 hint)
    {
        Transform upper=NewPoint(name+" Upper",camera,shoulder);Transform lower=NewPoint(name+" Forearm",camera,shoulder+new Vector3(0,-.15f,.15f));Transform hand=NewPoint(name+" Hand",camera,shoulder);
        GameObject upperMesh=Primitive(PrimitiveType.Capsule,name+" Sleeve",upper,new Vector3(0,0,.15f),new Vector3(.115f,.17f,.115f),rubber);upperMesh.transform.localRotation=Quaternion.Euler(90,0,0);
        GameObject lowerMesh=Primitive(PrimitiveType.Capsule,name+" Glove Arm",lower,new Vector3(0,0,.14f),new Vector3(.10f,.16f,.10f),skin);lowerMesh.transform.localRotation=Quaternion.Euler(90,0,0);
        Primitive(PrimitiveType.Sphere,name+" Hand Mesh",hand,Vector3.zero,new Vector3(.12f,.085f,.15f),skin);
        FPSProceduralArmIK ik=camera.gameObject.AddComponent<FPSProceduralArmIK>();ik.upperArm=upper;ik.forearm=lower;ik.hand=hand;ik.gripTarget=target;ik.elbowHint=hint;ik.upperLength=.32f;ik.lowerLength=.30f;
    }

    private static void BuildEnemy(Transform player)
    {
        GameObject enemy=new GameObject("Training Enemy");enemy.transform.position=new Vector3(0,0,13);
        CapsuleCollider hitbox=enemy.AddComponent<CapsuleCollider>();hitbox.center=new Vector3(0,.9f,0);hitbox.height=1.8f;hitbox.radius=.34f;
        GameObject operatorPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Prefabs/Imported/FSB_Operator.prefab");
        if(operatorPrefab==null)throw new System.Exception("Imported FSB Operator prefab is missing.");
        GameObject visual=PrefabUtility.InstantiatePrefab(operatorPrefab) as GameObject;
        if(visual==null)throw new System.Exception("Could not instantiate FSB Operator model.");
        visual.name="FSB Operator Character";visual.transform.SetParent(enemy.transform,false);
        visual.transform.localPosition=Vector3.zero;visual.transform.localRotation=Quaternion.identity;visual.transform.localScale=Vector3.one;
        Animator animator=visual.GetComponentInChildren<Animator>(true);
        if(animator==null)throw new System.Exception("FSB Operator imported model has no Animator.");
        animator.applyRootMotion=false;animator.cullingMode=AnimatorCullingMode.CullUpdateTransforms;
        foreach(SkinnedMeshRenderer renderer in visual.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            renderer.updateWhenOffscreen=false;
            renderer.quality=SkinQuality.Bone4;
        }

        // Temporary articulated proxy is kept separate from the visible imported character until a bone-mapped ragdoll is calibrated.
        GameObject ragdoll=new GameObject("Ragdoll Physics Proxies");ragdoll.transform.SetParent(enemy.transform,false);
        Rigidbody torso=RagdollPart(ragdoll.transform,"Ragdoll Torso",new Vector3(0,1.02f,0),new Vector3(.48f,.55f,.28f),rubber,5f,null);
        RagdollPart(ragdoll.transform,"Ragdoll Head",new Vector3(0,1.68f,0),new Vector3(.27f,.29f,.25f),skin,1.2f,torso);
        RagdollPart(ragdoll.transform,"Ragdoll Left Arm",new Vector3(-.34f,1.05f,.02f),new Vector3(.16f,.39f,.16f),rubber,1.4f,torso);
        RagdollPart(ragdoll.transform,"Ragdoll Right Arm",new Vector3(.34f,1.05f,.02f),new Vector3(.16f,.39f,.16f),rubber,1.4f,torso);
        RagdollPart(ragdoll.transform,"Ragdoll Left Leg",new Vector3(-.14f,.38f,0),new Vector3(.19f,.43f,.19f),darkConcrete,2f,torso);
        RagdollPart(ragdoll.transform,"Ragdoll Right Leg",new Vector3(.14f,.38f,0),new Vector3(.19f,.43f,.19f),darkConcrete,2f,torso);
        ragdoll.SetActive(false);
        FPSDamageable damage=enemy.AddComponent<FPSDamageable>();damage.ragdollRoot=ragdoll;damage.maxHealth=100;damage.respawnDelay=4;
        FPSTargetDummy ai=enemy.AddComponent<FPSTargetDummy>();ai.player=player;ai.wanderRadius=2.2f;ai.wanderSpeed=.35f;
    }
    private static Rigidbody RagdollPart(Transform parent,string name,Vector3 pos,Vector3 scale,Material mat,float mass,Rigidbody connected)
    {
        GameObject part=Primitive(PrimitiveType.Capsule,name,parent,pos,scale,mat,true);Rigidbody rb=part.AddComponent<Rigidbody>();rb.mass=mass;rb.isKinematic=true;rb.interpolation=RigidbodyInterpolation.Interpolate;
        if(connected!=null)
        {
            CharacterJoint joint=part.AddComponent<CharacterJoint>();joint.connectedBody=connected;joint.enableProjection=true;joint.projectionDistance=.12f;
            SoftJointLimit low=new SoftJointLimit();low.limit=-25;joint.lowTwistLimit=low;SoftJointLimit high=new SoftJointLimit();high.limit=25;joint.highTwistLimit=high;
            SoftJointLimit swing=new SoftJointLimit();swing.limit=35;joint.swing1Limit=swing;joint.swing2Limit=swing;
        }
        return rb;
    }

    private static void BuildLighting()
    {
        GameObject sunObject=new GameObject("Industrial skylight");Light sun=sunObject.AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.05f;sun.color=new Color(.78f,.84f,1f);sun.shadows=LightShadows.Soft;sunObject.transform.rotation=Quaternion.Euler(52,-28,0);
        for(int i=0;i<5;i++)
        {
            GameObject lightObject=new GameObject("Warehouse overhead light "+i);lightObject.transform.position=new Vector3((i%2==0?-8:8),7.1f,1+i*5.2f);
            Light l=lightObject.AddComponent<Light>();l.type=LightType.Point;l.range=13;l.intensity=2.1f;l.color=new Color(.78f,.86f,1f);
        }
        GameObject warm=new GameObject("Loading bay sodium light");warm.transform.position=new Vector3(0,5,-3);Light w=warm.AddComponent<Light>();w.type=LightType.Point;w.range=20;w.intensity=1.6f;w.color=new Color(1f,.68f,.38f);
    }


    public static void ValidateScene()
    {
        if(GameObject.Find("FPS Player")==null && System.IO.File.Exists(ScenePath))
            EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        GameObject playerObject=GameObject.Find("FPS Player");
        if(playerObject==null)throw new System.Exception("Validation failed: FPS Player missing.");
        FPSPlayerController player=playerObject.GetComponent<FPSPlayerController>();
        FPSWeapon weapon=playerObject.GetComponent<FPSWeapon>();
        if(player==null||weapon==null)throw new System.Exception("Validation failed: player controller or weapon component missing.");
        if(player.viewPivot==null||player.moveStick==null)throw new System.Exception("Validation failed: view pivot or mobile joystick is not assigned.");
        if(weapon.aimCamera==null||weapon.player==null||weapon.pistolModel==null||weapon.akmModel==null)throw new System.Exception("Validation failed: weapon references are incomplete.");
        if(weapon.pistolMuzzleFlash==null||weapon.akmMuzzleFlash==null||weapon.ammoText==null||weapon.weaponText==null)throw new System.Exception("Validation failed: muzzle flash or weapon HUD reference missing.");
        if(Camera.main==null||Camera.main.GetComponent<FPSBodycamEffects>()==null)throw new System.Exception("Validation failed: Bodycam camera/effects missing.");
        if(weapon.pistolModel.GetComponentInChildren<SkinnedMeshRenderer>(true)==null||weapon.akmModel.GetComponentInChildren<SkinnedMeshRenderer>(true)==null)
            throw new System.Exception("Validation failed: imported skinned first-person weapon/arms models are missing.");
        if(weapon.pistolModel.GetComponentInChildren<Animator>(true)==null||weapon.akmModel.GetComponentInChildren<Animator>(true)==null)
            throw new System.Exception("Validation failed: imported first-person animation controllers are missing.");
        GameObject warehouse=GameObject.Find("Abandoned Warehouse Environment");
        if(warehouse==null||warehouse.GetComponentsInChildren<MeshCollider>(true).Length<10)
            throw new System.Exception("Validation failed: imported warehouse or its collision meshes are missing.");
        GameObject enemy=GameObject.Find("Training Enemy");
        if(enemy==null)throw new System.Exception("Validation failed: training enemy missing.");
        GameObject operatorModel=enemy.transform.Find("FSB Operator Character")?.gameObject;
        if(operatorModel==null||operatorModel.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length<5||operatorModel.GetComponentInChildren<Animator>(true)==null)
            throw new System.Exception("Validation failed: imported FSB Operator character meshes or Animator are missing.");
        FPSDamageable damage=enemy.GetComponent<FPSDamageable>();
        if(damage==null||damage.ragdollRoot==null||damage.ragdollRoot.GetComponentsInChildren<Rigidbody>(true).Length<5)throw new System.Exception("Validation failed: enemy ragdoll setup incomplete.");
        if(GameObject.Find("EventSystem")==null||GameObject.Find("FIRE Button")==null||GameObject.Find("AIM Button")==null||GameObject.Find("RELOAD Button")==null||GameObject.Find("SWAP Button")==null)
            throw new System.Exception("Validation failed: required mobile touch UI or EventSystem missing.");
        Debug.Log("FPS SCENE VALIDATION PASSED: player, mobile controls, Bodycam camera, imported weapon models and animations, enemy and ragdoll references are present.");
    }

    private static void BuildHUD(FPSPlayerController player)
    {
        GameObject canvasObject=new GameObject("Mobile FPS HUD");Canvas canvas=canvasObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler=canvasObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;
        canvasObject.AddComponent<GraphicRaycaster>();
        GameObject es=new GameObject("EventSystem");es.AddComponent<EventSystem>();es.AddComponent<StandaloneInputModule>();

        GameObject joystick=Panel("Movement Joystick",canvas.transform,new Vector2(.13f,.20f),new Vector2(220,220),new Color(.08f,.10f,.12f,.42f));
        FPSMobileJoystick stick=joystick.AddComponent<FPSMobileJoystick>();player.moveStick=stick;
        GameObject handle=Panel("Joystick Handle",joystick.transform,new Vector2(.5f,.5f),new Vector2(90,90),new Color(.8f,.86f,.9f,.60f));
        handle.GetComponent<Image>().raycastTarget=false;
        SerializedObject so=new SerializedObject(stick);so.FindProperty("handle").objectReferenceValue=handle.GetComponent<RectTransform>();so.ApplyModifiedProperties();

        Text title=Label("BODYCAM // TRAINING",canvas.transform,new Vector2(.5f,.94f),new Vector2(650,50),24,TextAnchor.MiddleCenter,new Color(.88f,.92f,.95f));
        Text ammo=Label("15 / 60",canvas.transform,new Vector2(.91f,.91f),new Vector2(260,58),34,TextAnchor.MiddleRight,Color.white);
        Text weaponName=Label("PISTOL",canvas.transform,new Vector2(.91f,.86f),new Vector2(260,34),20,TextAnchor.MiddleRight,new Color(.75f,.82f,.86f));
        FPSWeapon weapon=player.GetComponent<FPSWeapon>();weapon.ammoText=ammo;weapon.weaponText=weaponName;

        Text crosshair=Label("+",canvas.transform,new Vector2(.5f,.5f),new Vector2(35,35),30,TextAnchor.MiddleCenter,new Color(1,1,1,.9f));
        Text hint=Label("DRAG RIGHT SIDE TO LOOK",canvas.transform,new Vector2(.5f,.07f),new Vector2(500,34),17,TextAnchor.MiddleCenter,new Color(1,1,1,.55f));

        Button fire=ButtonUI("FIRE",canvas.transform,new Vector2(.88f,.22f),new Vector2(154,154),new Color(.55f,.12f,.08f,.82f),32);
        AddHold(fire,()=>weapon.SetFire(true),()=>weapon.SetFire(false));
        Button aim=ButtonUI("AIM",canvas.transform,new Vector2(.72f,.20f),new Vector2(112,112),new Color(.12f,.20f,.25f,.78f),23);
        AddHold(aim,()=>weapon.SetAim(true),()=>weapon.SetAim(false));
        Button reload=ButtonUI("RELOAD",canvas.transform,new Vector2(.86f,.44f),new Vector2(118,66),new Color(.10f,.15f,.18f,.82f),19);reload.onClick.AddListener(weapon.Reload);
        Button swap=ButtonUI("SWAP",canvas.transform,new Vector2(.72f,.44f),new Vector2(118,66),new Color(.10f,.15f,.18f,.82f),19);swap.onClick.AddListener(weapon.SwitchWeapon);
        Button crouch=ButtonUI("CROUCH",canvas.transform,new Vector2(.53f,.18f),new Vector2(120,66),new Color(.10f,.15f,.18f,.78f),18);crouch.onClick.AddListener(player.ToggleCrouch);
        Button prone=ButtonUI("PRONE",canvas.transform,new Vector2(.53f,.095f),new Vector2(120,66),new Color(.10f,.15f,.18f,.78f),18);prone.onClick.AddListener(player.ToggleProne);
        Button peekL=ButtonUI("LEAN L",canvas.transform,new Vector2(.62f,.57f),new Vector2(104,58),new Color(.10f,.15f,.18f,.72f),16);peekL.onClick.AddListener(()=>player.TogglePeek(-1));
        Button peekR=ButtonUI("LEAN R",canvas.transform,new Vector2(.73f,.57f),new Vector2(104,58),new Color(.10f,.15f,.18f,.72f),16);peekR.onClick.AddListener(()=>player.TogglePeek(1));
        Button sprint=ButtonUI("SPRINT",canvas.transform,new Vector2(.29f,.12f),new Vector2(110,62),new Color(.10f,.15f,.18f,.65f),17);
        AddHold(sprint,()=>player.SetSprint(true),()=>player.SetSprint(false));
        Text status=Label("AKM + PISTOL  |  TRAINING TARGET",canvas.transform,new Vector2(.5f,.89f),new Vector2(620,30),16,TextAnchor.MiddleCenter,new Color(.7f,.78f,.82f));
    }

    private static GameObject Panel(string name,Transform parent,Vector2 anchor,Vector2 size,Color color)
    {
        GameObject go=new GameObject(name);go.transform.SetParent(parent,false);RectTransform rt=go.AddComponent<RectTransform>();rt.anchorMin=anchor;rt.anchorMax=anchor;rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=Vector2.zero;rt.sizeDelta=size;
        Image image=go.AddComponent<Image>();image.color=color;return go;
    }
    private static Text Label(string value,Transform parent,Vector2 anchor,Vector2 size,int fontSize,TextAnchor align,Color color)
    {
        GameObject go=new GameObject(value+" Label");go.transform.SetParent(parent,false);RectTransform rt=go.AddComponent<RectTransform>();rt.anchorMin=anchor;rt.anchorMax=anchor;rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=Vector2.zero;rt.sizeDelta=size;
        Text t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=fontSize;t.alignment=align;t.color=color;t.raycastTarget=false;return t;
    }
    private static Button ButtonUI(string text,Transform parent,Vector2 anchor,Vector2 size,Color color,int fontSize)
    {
        GameObject go=Panel(text+" Button",parent,anchor,size,color);Image image=go.GetComponent<Image>();Button button=go.AddComponent<Button>();button.targetGraphic=image;button.transition=Selectable.Transition.ColorTint;
        ColorBlock cb=button.colors;cb.normalColor=Color.white;cb.highlightedColor=new Color(1,1,1,1);cb.pressedColor=new Color(.65f,.72f,.78f,1);button.colors=cb;
        Label(text,go.transform,new Vector2(.5f,.5f),size,fontSize,TextAnchor.MiddleCenter,Color.white);return button;
    }
    private static void AddHold(Button button,UnityEngine.Events.UnityAction down,UnityEngine.Events.UnityAction up)
    {
        EventTrigger trigger=button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry d=new EventTrigger.Entry();d.eventID=EventTriggerType.PointerDown;d.callback.AddListener((data)=>down());trigger.triggers.Add(d);
        EventTrigger.Entry u=new EventTrigger.Entry();u.eventID=EventTriggerType.PointerUp;u.callback.AddListener((data)=>up());trigger.triggers.Add(u);
        EventTrigger.Entry x=new EventTrigger.Entry();x.eventID=EventTriggerType.PointerExit;x.callback.AddListener((data)=>up());trigger.triggers.Add(x);
    }
}