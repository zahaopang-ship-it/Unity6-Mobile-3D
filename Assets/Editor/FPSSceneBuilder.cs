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
        Box("Main asphalt slab",new Vector3(0,-.55f,8),new Vector3(76,1,82),darkConcrete);
        Box("Indoor concrete floor",new Vector3(0,-.08f,12),new Vector3(38,.18f,34),concrete);
        Box("Warehouse roof",new Vector3(0,8.2f,12),new Vector3(38,.5f,34),darkConcrete);
        Box("North wall",new Vector3(0,4,29),new Vector3(38,8,1),concrete);
        Box("West wall",new Vector3(-19,4,12),new Vector3(1,8,34),concrete);
        Box("East wall",new Vector3(19,4,12),new Vector3(1,8,34),concrete);
        Box("South west wall",new Vector3(-13,3.6f,-5),new Vector3(12,7,1),concrete);
        Box("South east wall",new Vector3(13,3.6f,-5),new Vector3(12,7,1),concrete);
        Box("Loading bay lintel",new Vector3(0,7.2f,-5),new Vector3(14,1.6f,1),concrete);
        Box("Interior partition A",new Vector3(-7,2.1f,17),new Vector3(1,4.2f,12),darkConcrete);
        Box("Interior partition B",new Vector3(7,2.1f,17),new Vector3(1,4.2f,12),darkConcrete);
        Box("Rear divider",new Vector3(0,1.7f,23),new Vector3(14,3.4f,.5f),concrete);
        Box("Divider doorway left",new Vector3(-4.2f,1.7f,8),new Vector3(5,3.4f,.5f),concrete);
        Box("Divider doorway right",new Vector3(4.2f,1.7f,8),new Vector3(5,3.4f,.5f),concrete);
        for(int i=0;i<7;i++)
        {
            float z=-1+i*4.5f;
            Box("Roof beam "+i,new Vector3(0,7.8f,z),new Vector3(38,.22f,.28f),metal);
            Box("Ceiling light housing "+i,new Vector3((i%2==0?-1:1)*8,7.55f,z),new Vector3(2.1f,.12f,.42f),rubber);
        }
        for(int side=-1;side<=1;side+=2)
        {
            for(int i=0;i<4;i++)
            {
                float z=1+i*7.2f;
                Box("Wall support "+side+" "+i,new Vector3(side*18.2f,4,z),new Vector3(.5f,8,.5f),darkConcrete);
                Box("Window recess "+side+" "+i,new Vector3(side*18.42f,4.7f,z+2.1f),new Vector3(.08f,1.6f,2.6f),glass);
            }
        }

        // Stacked industrial shipping containers and cover positions.
        for(int i=0;i<3;i++)
        {
            float z=2+i*8.5f;
            Box("Blue shipping container "+i,new Vector3(-13,1.35f,z),new Vector3(5.2f,2.7f,7.2f),blueMetal);
            for(int rib=0;rib<9;rib++)Box("Container rib "+i+" "+rib,new Vector3(-15.62f+rib*.65f,1.35f,z),new Vector3(.07f,2.55f,7.15f),metal);
            Box("Container top seam "+i,new Vector3(-13,2.75f,z),new Vector3(5.25f,.08f,7.25f),metal);
        }
        for(int i=0;i<3;i++)
        {
            float z=1+i*8.5f;
            Box("Wooden crate stack "+i,new Vector3(13, .65f,z),new Vector3(2.3f,1.3f,2.3f),wood);
            Box("Crate upper "+i,new Vector3(13,1.95f,z),new Vector3(2.1f,1.3f,2.1f),wood);
            for(int x=-1;x<=1;x++)Box("Crate band "+i+" "+x,new Vector3(13+x*.65f,1.95f,z-1.07f),new Vector3(.06f,1.2f,.06f),metal);
        }
        for(int i=0;i<7;i++)
        {
            float x=-9+i*3;
            Box("Concrete waist cover "+i,new Vector3(x,.65f,10+(i%2)*5),new Vector3(1.8f,1.3f,1.1f),concrete);
        }
        for(int i=0;i<5;i++)
        {
            GameObject barrel=Primitive(PrimitiveType.Cylinder,"Oil drum "+i,null,new Vector3(-10+i*5,.65f,25),new Vector3(.52f, .65f,.52f),i%2==0?blueMetal:orange,true);
            barrel.transform.rotation=Quaternion.identity;
            Box("Drum rim "+i,new Vector3(-10+i*5,1.28f,25),new Vector3(1.05f,.07f,1.05f),metal);
        }
        for(int i=0;i<8;i++)
        {
            float x=-15+i*4.2f;
            Box("Floor hazard stripe "+i,new Vector3(x,-.445f,-1),new Vector3(1.6f,.015f,.12f),orange);
        }
        // Exterior yard details, pipes and bollards.
        for(int i=0;i<6;i++)
        {
            Primitive(PrimitiveType.Cylinder,"Safety bollard "+i,null,new Vector3(-16+i*6,.65f,-22),new Vector3(.18f,.65f,.18f),orange);
        }
        Box("Outdoor service road",new Vector3(0,-.47f,-27),new Vector3(44,.08f,14),darkConcrete);
        for(int i=0;i<8;i++)Box("Road dash "+i,new Vector3(-16+i*4.5f,-.42f,-27),new Vector3(2,.02f,.12f),white);
        Box("Low concrete barrier",new Vector3(0,.45f,-18),new Vector3(16,.9f,.8f),concrete);
        Box("Pipe run",new Vector3(10,3.6f,13),new Vector3(.25f,.25f,25),metal);
        Box("Pipe support",new Vector3(10,2.6f,13),new Vector3(.5f,2,.5f),darkConcrete);
        Box("Pipe support 2",new Vector3(10,2.6f,23),new Vector3(.5f,2,.5f),darkConcrete);
        Box("Training target backstop",new Vector3(0,2.2f,27.8f),new Vector3(9,4.4f,.35f),darkConcrete);
        for(int i=0;i<3;i++)
        {
            Box("Target plate "+i,new Vector3(-3+i*3,1.6f,27.45f),new Vector3(.75f,1.15f,.12f),orange);
            Box("Target stand "+i,new Vector3(-3+i*3,.55f,27.45f),new Vector3(.12f,1.1f,.12f),metal);
        }
    }

    private static FPSPlayerController BuildPlayer()
    {
        GameObject root=new GameObject("FPS Player");root.tag="Player";root.transform.position=new Vector3(0,0,-12);
        CharacterController cc=root.AddComponent<CharacterController>();cc.height=1.72f;cc.radius=.34f;cc.center=Vector3.up*.86f;cc.slopeLimit=48;cc.stepOffset=.28f;
        FPSPlayerController player=root.AddComponent<FPSPlayerController>();
        GameObject pivotObject=new GameObject("Bodycam View Pivot");pivotObject.transform.SetParent(root.transform,false);pivotObject.transform.localPosition=new Vector3(0,1.48f,0);
        player.viewPivot=pivotObject.transform;
        GameObject cameraObject=new GameObject("Bodycam Camera");cameraObject.tag="MainCamera";cameraObject.transform.SetParent(pivotObject.transform,false);
        Camera cam=cameraObject.AddComponent<Camera>();cam.fieldOfView=82;cam.nearClipPlane=.045f;cam.farClipPlane=180;cam.allowHDR=false;cam.allowMSAA=true;
        cameraObject.AddComponent<AudioListener>();
        GameObject weaponMount=new GameObject("First Person Weapon Mount");weaponMount.transform.SetParent(cameraObject.transform,false);weaponMount.transform.localPosition=new Vector3(.22f,-.20f,.48f);
        FPSBodycamEffects effects=cameraObject.AddComponent<FPSBodycamEffects>();effects.SetWeaponRoot(weaponMount.transform);
        BuildGun("Pistol View Model",weaponMount.transform,false);
        GameObject pistol=weaponMount.transform.Find("Pistol View Model").gameObject;
        BuildGun("AKM View Model",weaponMount.transform,true);
        GameObject akm=weaponMount.transform.Find("AKM View Model").gameObject;
        FPSWeapon weapon=root.AddComponent<FPSWeapon>();weapon.aimCamera=cam;weapon.player=player;weapon.cameraEffects=effects;weapon.pistolModel=pistol;weapon.akmModel=akm;
        weapon.audioSource=cameraObject.AddComponent<AudioSource>();weapon.audioSource.playOnAwake=false;weapon.audioSource.spatialBlend=0;
        effects.SetWeaponRoot(weaponMount.transform);
        BuildArms(cameraObject.transform,weaponMount.transform);
        return player;
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
        GameObject visual=new GameObject("Animated target body");visual.transform.SetParent(enemy.transform,false);
        Primitive(PrimitiveType.Capsule,"Torso",visual.transform,new Vector3(0,1.02f,0),new Vector3(.48f,.55f,.28f),rubber);
        Primitive(PrimitiveType.Sphere,"Head",visual.transform,new Vector3(0,1.68f,0),new Vector3(.27f,.29f,.25f),skin);
        Primitive(PrimitiveType.Capsule,"Left arm",visual.transform,new Vector3(-.34f,1.05f,.02f),new Vector3(.16f,.39f,.16f),rubber).transform.localRotation=Quaternion.Euler(0,0,-12);
        Primitive(PrimitiveType.Capsule,"Right arm",visual.transform,new Vector3(.34f,1.05f,.02f),new Vector3(.16f,.39f,.16f),rubber).transform.localRotation=Quaternion.Euler(0,0,12);
        Primitive(PrimitiveType.Capsule,"Left leg",visual.transform,new Vector3(-.14f,.38f,0),new Vector3(.19f,.43f,.19f),darkConcrete);
        Primitive(PrimitiveType.Capsule,"Right leg",visual.transform,new Vector3(.14f,.38f,0),new Vector3(.19f,.43f,.19f),darkConcrete);
        Primitive(PrimitiveType.Cube,"Chest rig",visual.transform,new Vector3(0,1.02f,.15f),new Vector3(.39f,.38f,.08f),blueMetal);
        Primitive(PrimitiveType.Cube,"Chest patch",visual.transform,new Vector3(0,1.1f,.20f),new Vector3(.12f,.08f,.015f),orange);

        GameObject ragdoll=new GameObject("Ragdoll");ragdoll.transform.SetParent(enemy.transform,false);
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