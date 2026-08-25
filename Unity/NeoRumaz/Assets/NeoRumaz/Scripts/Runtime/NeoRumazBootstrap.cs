// Cairo Night Runner style reminder: premium third-person neon highway, English command UI, cyan infrastructure, amber rewards, magenta hazards.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace NeoRumaz.Runtime
{
    public sealed class NeoRumazBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (SceneManager.GetActiveScene().name != "NeoRumazGame" || FindObjectOfType<NeoRumazBootstrap>() != null) return;
            var runtime = new GameObject("NeoRumaz Runtime");
            runtime.AddComponent<NeoRumazBootstrap>();
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            gameObject.AddComponent<NightRunnerGame>().Configure();
        }
    }

    public enum RunnerItemType { Barrier, Drone, Credit, Shield, NileRush }

    internal sealed class RunnerItem
    {
        internal readonly GameObject Root;
        internal readonly RunnerItemType Type;
        internal bool Active;

        internal RunnerItem(GameObject root, RunnerItemType type)
        {
            Root = root;
            Type = type;
            Root.SetActive(false);
        }
    }

    public sealed class NightRunnerGame : MonoBehaviour
    {
        private const float LaneWidth = 2.8f;
        private const int RoadSegmentCount = 8;
        private const float RoadSegmentLength = 18f;
        private const int ContractTarget = 6;

        private readonly List<Transform> roadSegments = new List<Transform>();
        private readonly List<RunnerItem> items = new List<RunnerItem>();
        private readonly List<Transform> runnerLimbs = new List<Transform>();
        private Transform player;
        private CharacterController playerController;
        private Transform cameraRig;
        private Transform shieldVisual;
        private NeoRumazHud hud;
        private int lane = 1;
        private float verticalVelocity;
        private float targetLaneX;
        private float runDistance;
        private float runSpeed = 14f;
        private float spawnTimer;
        private float shieldSeconds;
        private float nileRushSeconds;
        private int credits = 248;
        private int contractCredits;
        private bool isRunning = true;
        private Vector2 touchStart;
        private bool touchTracking;

        public int Score { get { return Mathf.FloorToInt(runDistance * 10f); } }
        public int Credits { get { return credits; } }
        public int ContractCredits { get { return contractCredits; } }
        public int ContractGoal { get { return ContractTarget; } }
        public float ShieldSeconds { get { return shieldSeconds; } }
        public float NileRushSeconds { get { return nileRushSeconds; } }
        public bool IsRunning { get { return isRunning; } }

        public void Configure()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.035f, 0.07f, 0.16f);
            RenderSettings.ambientEquatorColor = new Color(0.015f, 0.03f, 0.08f);
            RenderSettings.ambientGroundColor = new Color(0.006f, 0.01f, 0.025f);
            BuildCameraAndLights();
            BuildRoadAndCity();
            BuildPlayer();
            BuildItemPool();
            hud = gameObject.AddComponent<NeoRumazHud>();
            hud.Configure(this);
            SpawnOpeningWave();
        }

        private void Update()
        {
            HandleInput();
            if (!isRunning)
            {
                hud.Refresh();
                return;
            }

            float dt = Mathf.Min(Time.deltaTime, 0.05f);
            nileRushSeconds = Mathf.Max(0f, nileRushSeconds - dt);
            shieldSeconds = Mathf.Max(0f, shieldSeconds - dt);
            if (shieldVisual != null) shieldVisual.gameObject.SetActive(shieldSeconds > 0f);
            runSpeed = Mathf.Min(28f, 14f + runDistance * 0.014f) * (nileRushSeconds > 0f ? 1.4f : 1f);
            runDistance += runSpeed * dt;
            MovePlayer(dt);
            MoveRoad(dt);
            MoveItems(dt);
            AnimateRunner(dt);
            UpdateCamera(dt);
            spawnTimer -= dt;
            if (spawnTimer <= 0f)
            {
                SpawnWave();
                spawnTimer = Mathf.Max(0.84f, 1.54f - runDistance * 0.002f);
            }
            hud.Refresh();
        }

        public void RequestJump()
        {
            if (isRunning && playerController.isGrounded) verticalVelocity = 8.2f;
        }

        public void RequestLane(int direction)
        {
            if (!isRunning) return;
            lane = Mathf.Clamp(lane + direction, 0, 2);
            targetLaneX = (lane - 1) * LaneWidth;
        }

        public void RestartRun()
        {
            foreach (RunnerItem item in items) DisableItem(item);
            lane = 1;
            targetLaneX = 0f;
            player.position = new Vector3(0f, 0.05f, 0f);
            verticalVelocity = 0f;
            runDistance = 0f;
            runSpeed = 14f;
            shieldSeconds = 0f;
            nileRushSeconds = 0f;
            contractCredits = 0;
            isRunning = true;
            SpawnOpeningWave();
            hud.HideGameOver();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) RequestLane(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) RequestLane(1);
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) RequestJump();

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) { touchStart = touch.position; touchTracking = true; }
                if (touchTracking && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                {
                    HandleSwipe(touch.position - touchStart);
                    touchTracking = false;
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                touchStart = Input.mousePosition;
                touchTracking = true;
            }
            else if (touchTracking && Input.GetMouseButtonUp(0))
            {
                HandleSwipe((Vector2)Input.mousePosition - touchStart);
                touchTracking = false;
            }
        }

        private void HandleSwipe(Vector2 delta)
        {
            if (delta.magnitude < 42f) return;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) RequestLane(delta.x > 0f ? 1 : -1);
            else if (delta.y > 0f) RequestJump();
        }

        private void BuildCameraAndLights()
        {
            var cameraObject = new GameObject("Runner Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 64f;
            camera.nearClipPlane = 0.05f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.006f, 0.012f, 0.04f);
            cameraRig = cameraObject.transform;
            cameraRig.position = new Vector3(0f, 5.15f, -9.2f);
            cameraRig.LookAt(new Vector3(0f, 1.15f, 11f));

            var key = new GameObject("Moon Key").AddComponent<Light>();
            key.type = LightType.Directional;
            key.color = new Color(0.33f, 0.64f, 1f);
            key.intensity = 0.72f;
            key.transform.rotation = Quaternion.Euler(43f, -22f, 0f);
            var cyan = NewLight("Transit Cyan", new Vector3(0f, 5.5f, 7f), new Color(0.15f, 0.92f, 1f), 5.2f, 25f);
            cyan.type = LightType.Point;
            var horizon = NewLight("Skyline Glow", new Vector3(0f, 9f, 72f), new Color(0.92f, 0.13f, 0.78f), 3f, 90f);
            horizon.type = LightType.Point;
        }

        private void BuildRoadAndCity()
        {
            Material asphalt = MakeMaterial("Asphalt", "#111A2B", "#060B14");
            Material cyan = MakeMaterial("Transit Cyan", "#0B6784", "#42E8FF");
            Material rail = MakeMaterial("Guardrail", "#1A3146", "#123B5B");
            Material building = MakeMaterial("City Glass", "#102039", "#071729");
            Material window = MakeMaterial("Lit Windows", "#0A3850", "#2D9AC7");
            Material magenta = MakeMaterial("Hazard Magenta", "#3A103A", "#FF4FD8");
            for (int index = 0; index < RoadSegmentCount; index++)
            {
                Transform segment = new GameObject("Road Segment " + index).transform;
                segment.position = new Vector3(0f, 0f, -RoadSegmentLength + index * RoadSegmentLength);
                CreatePrimitive(PrimitiveType.Cube, "Road", segment, new Vector3(0f, -0.22f, 0f), new Vector3(10.3f, 0.28f, RoadSegmentLength), asphalt);
                for (int divider = -1; divider <= 1; divider += 2)
                    CreatePrimitive(PrimitiveType.Cube, "Lane Light", segment, new Vector3(divider * LaneWidth * 0.5f, -0.045f, 0f), new Vector3(0.055f, 0.035f, RoadSegmentLength), cyan);
                for (int side = -1; side <= 1; side += 2)
                {
                    CreatePrimitive(PrimitiveType.Cube, "Guardrail", segment, new Vector3(side * 5.02f, 0.35f, 0f), new Vector3(0.25f, 0.75f, RoadSegmentLength), rail);
                    CreatePrimitive(PrimitiveType.Cube, "Rail Strip", segment, new Vector3(side * 5.06f, 0.72f, 0f), new Vector3(0.28f, 0.06f, RoadSegmentLength * 0.96f), cyan);
                    for (int tower = 0; tower < 4; tower++)
                    {
                        float depth = -7f + tower * 4.6f;
                        float height = 5.5f + ((index * 5 + tower * 3) % 7);
                        Transform block = CreatePrimitive(PrimitiveType.Cube, "Cairo Block", segment, new Vector3(side * (8.1f + (tower % 2) * 1.7f), height * 0.5f - 0.2f, depth), new Vector3(2.3f + (tower % 2), height, 2.5f), building);
                        CreatePrimitive(PrimitiveType.Cube, "Window Grid", block, new Vector3(-side * 1.18f, 0.2f, 0f), new Vector3(0.04f, height * 0.72f, 1.8f), tower % 3 == 0 ? magenta : window);
                    }
                }
                roadSegments.Add(segment);
            }
        }

        private void BuildPlayer()
        {
            player = new GameObject("NeoRumaz Courier").transform;
            player.position = new Vector3(0f, 0.05f, 0f);
            playerController = player.gameObject.AddComponent<CharacterController>();
            playerController.height = 2.45f;
            playerController.radius = 0.38f;
            playerController.center = new Vector3(0f, 1.22f, 0f);
            Material suit = MakeMaterial("Courier Suit", "#182536", "#0D2338");
            Material cyan = MakeMaterial("Courier Glow", "#0A6A82", "#42E8FF");
            Material visor = MakeMaterial("Courier Visor", "#123142", "#6DF6FF");
            Transform torso = CreatePrimitive(PrimitiveType.Capsule, "Courier Torso", player, new Vector3(0f, 1.15f, 0f), new Vector3(0.72f, 1.18f, 0.58f), suit);
            CreatePrimitive(PrimitiveType.Sphere, "Courier Head", torso, new Vector3(0f, 0.94f, 0.01f), new Vector3(0.54f, 0.54f, 0.54f), suit);
            CreatePrimitive(PrimitiveType.Cube, "Courier Visor", torso, new Vector3(0f, 0.94f, -0.28f), new Vector3(0.4f, 0.11f, 0.06f), visor);
            CreatePrimitive(PrimitiveType.Cube, "Back Mark", torso, new Vector3(0f, 0.18f, 0.31f), new Vector3(0.38f, 0.26f, 0.06f), cyan);
            runnerLimbs.Add(CreatePrimitive(PrimitiveType.Capsule, "Left Arm", torso, new Vector3(-0.48f, 0.35f, 0f), new Vector3(0.18f, 0.55f, 0.18f), suit));
            runnerLimbs.Add(CreatePrimitive(PrimitiveType.Capsule, "Right Arm", torso, new Vector3(0.48f, 0.35f, 0f), new Vector3(0.18f, 0.55f, 0.18f), suit));
            runnerLimbs.Add(CreatePrimitive(PrimitiveType.Capsule, "Left Leg", player, new Vector3(-0.22f, 0.52f, 0f), new Vector3(0.22f, 0.66f, 0.22f), suit));
            runnerLimbs.Add(CreatePrimitive(PrimitiveType.Capsule, "Right Leg", player, new Vector3(0.22f, 0.52f, 0f), new Vector3(0.22f, 0.66f, 0.22f), suit));
            shieldVisual = CreatePrimitive(PrimitiveType.Cylinder, "Scarab Shield", player, new Vector3(0f, 1.05f, 0f), new Vector3(1.35f, 0.03f, 1.35f), MakeMaterial("Shield Glow", "#8D6511", "#FFC857"));
            shieldVisual.gameObject.SetActive(false);
        }

        private void BuildItemPool()
        {
            for (int index = 0; index < 12; index++) items.Add(CreateItem(RunnerItemType.Barrier, index));
            for (int index = 0; index < 8; index++) items.Add(CreateItem(RunnerItemType.Drone, index));
            for (int index = 0; index < 42; index++) items.Add(CreateItem(RunnerItemType.Credit, index));
            for (int index = 0; index < 4; index++) items.Add(CreateItem(RunnerItemType.Shield, index));
            for (int index = 0; index < 5; index++) items.Add(CreateItem(RunnerItemType.NileRush, index));
        }

        private RunnerItem CreateItem(RunnerItemType type, int index)
        {
            var root = new GameObject(type + " " + index);
            Material cyan = MakeMaterial("Pickup Cyan " + index, "#08748D", "#42E8FF");
            Material amber = MakeMaterial("Reward Amber " + index, "#8C5D11", "#FFC857");
            Material magenta = MakeMaterial("Drone Magenta " + index, "#4A123F", "#FF4FD8");
            Material hazard = MakeMaterial("Barrier Amber " + index, "#A94D12", "#FF7A26");
            if (type == RunnerItemType.Barrier)
            {
                CreatePrimitive(PrimitiveType.Cube, "Barrier Beam", root.transform, new Vector3(0f, 0.74f, 0f), new Vector3(2.25f, 0.55f, 0.32f), hazard);
                CreatePrimitive(PrimitiveType.Cube, "Barrier Foot L", root.transform, new Vector3(-0.85f, 0.35f, 0f), new Vector3(0.28f, 0.8f, 0.42f), hazard);
                CreatePrimitive(PrimitiveType.Cube, "Barrier Foot R", root.transform, new Vector3(0.85f, 0.35f, 0f), new Vector3(0.28f, 0.8f, 0.42f), hazard);
            }
            else if (type == RunnerItemType.Drone)
            {
                CreatePrimitive(PrimitiveType.Sphere, "Drone Core", root.transform, new Vector3(0f, 1.08f, 0f), new Vector3(0.8f, 0.26f, 0.6f), magenta);
                CreatePrimitive(PrimitiveType.Cylinder, "Drone Rotor L", root.transform, new Vector3(-0.66f, 1.08f, 0f), new Vector3(0.5f, 0.05f, 0.5f), magenta);
                CreatePrimitive(PrimitiveType.Cylinder, "Drone Rotor R", root.transform, new Vector3(0.66f, 1.08f, 0f), new Vector3(0.5f, 0.05f, 0.5f), magenta);
            }
            else if (type == RunnerItemType.Credit)
            {
                CreatePrimitive(PrimitiveType.Sphere, "Credit", root.transform, new Vector3(0f, 1.05f, 0f), new Vector3(0.42f, 0.42f, 0.42f), cyan);
                CreatePrimitive(PrimitiveType.Cylinder, "Credit Ring", root.transform, new Vector3(0f, 1.05f, 0f), new Vector3(0.54f, 0.04f, 0.54f), cyan);
            }
            else if (type == RunnerItemType.Shield)
            {
                CreatePrimitive(PrimitiveType.Cylinder, "Scarab Shield", root.transform, new Vector3(0f, 0.08f, 0f), new Vector3(0.95f, 0.07f, 0.95f), amber);
                CreatePrimitive(PrimitiveType.Sphere, "Shield Core", root.transform, new Vector3(0f, 1f, 0f), new Vector3(0.28f, 0.42f, 0.28f), amber);
            }
            else
            {
                for (int stripe = 0; stripe < 4; stripe++)
                    CreatePrimitive(PrimitiveType.Cube, "Nile Rush Stripe", root.transform, new Vector3(0f, 0.03f, stripe * 0.5f), new Vector3(2.12f, 0.05f, 0.32f), cyan);
                CreatePrimitive(PrimitiveType.Cylinder, "Nile Crest", root.transform, new Vector3(0f, 0.12f, 1.5f), new Vector3(0.34f, 0.08f, 0.34f), amber);
            }
            return new RunnerItem(root, type);
        }

        private void MovePlayer(float dt)
        {
            float horizontal = Mathf.Clamp((targetLaneX - player.position.x) * 11f, -10f, 10f);
            if (playerController.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += Physics.gravity.y * dt;
            playerController.Move(new Vector3(horizontal, verticalVelocity, 0f) * dt);
            player.rotation = Quaternion.Euler(0f, 0f, Mathf.Clamp(-horizontal * 2.4f, -14f, 14f));
        }

        private void MoveRoad(float dt)
        {
            float distance = runSpeed * dt;
            foreach (Transform segment in roadSegments)
            {
                segment.position += Vector3.back * distance;
                if (segment.position.z < -RoadSegmentLength * 1.5f) segment.position += Vector3.forward * RoadSegmentLength * RoadSegmentCount;
            }
        }

        private void MoveItems(float dt)
        {
            float distance = runSpeed * dt;
            foreach (RunnerItem item in items)
            {
                if (!item.Active) continue;
                item.Root.transform.position += Vector3.back * distance;
                if (item.Type == RunnerItemType.Credit || item.Type == RunnerItemType.Shield) item.Root.transform.Rotate(0f, 160f * dt, 0f);
                if (item.Type == RunnerItemType.Drone) item.Root.transform.position += Vector3.up * Mathf.Sin((runDistance + item.Root.transform.position.z) * 0.25f) * dt * 0.4f;
                if (item.Root.transform.position.z < -9f) { DisableItem(item); continue; }
                if (Mathf.Abs(item.Root.transform.position.z) < 1.15f && Mathf.Abs(item.Root.transform.position.x - player.position.x) < 0.85f)
                    ResolveItem(item);
            }
        }

        private void ResolveItem(RunnerItem item)
        {
            if (item.Type == RunnerItemType.Barrier || item.Type == RunnerItemType.Drone)
            {
                if (playerController.isGrounded)
                {
                    if (shieldSeconds > 0f) { shieldSeconds = 0f; DisableItem(item); hud.ShowToast("SCARAB SHIELD ABSORBED THE HIT"); }
                    else Crash();
                }
                return;
            }
            if (item.Type == RunnerItemType.Credit)
            {
                credits += 1;
                contractCredits += 1;
                if (contractCredits >= ContractTarget)
                {
                    credits += 3;
                    contractCredits = 0;
                    hud.ShowToast("CAIRO CONTRACT COMPLETE  +3 CREDITS");
                }
            }
            else if (item.Type == RunnerItemType.Shield)
            {
                shieldSeconds = 6f;
                hud.ShowToast("SCARAB SHIELD ONLINE");
            }
            else if (item.Type == RunnerItemType.NileRush)
            {
                nileRushSeconds = 5f;
                hud.ShowToast("NILE RUSH  //  FAST LANE");
            }
            DisableItem(item);
        }

        private void SpawnOpeningWave()
        {
            Spawn(RunnerItemType.Credit, 1, 21f);
            Spawn(RunnerItemType.Credit, 1, 23.2f);
            Spawn(RunnerItemType.Credit, 1, 25.4f);
            Spawn(RunnerItemType.NileRush, 2, 30f);
            Spawn(RunnerItemType.Barrier, 0, 36f);
            spawnTimer = 1.1f;
        }

        private void SpawnWave()
        {
            int pattern = Mathf.FloorToInt(runDistance / 6f) % 5;
            int safeLane = Mathf.FloorToInt(runDistance / 9f) % 3;
            float z = 52f;
            if (pattern == 0)
            {
                Spawn(RunnerItemType.Barrier, (safeLane + 1) % 3, z);
                SpawnCredits(safeLane, z + 2f, 5);
            }
            else if (pattern == 1)
            {
                Spawn(RunnerItemType.Drone, safeLane, z);
                SpawnCredits((safeLane + 2) % 3, z + 1f, 5);
            }
            else if (pattern == 2)
            {
                Spawn(RunnerItemType.Barrier, (safeLane + 1) % 3, z);
                Spawn(RunnerItemType.Drone, (safeLane + 2) % 3, z + 8f);
                SpawnCredits(safeLane, z + 2f, 4);
            }
            else if (pattern == 3)
            {
                SpawnCredits(safeLane, z, 6);
                Spawn(RunnerItemType.Shield, (safeLane + 1) % 3, z + 4f);
            }
            else
            {
                Spawn(RunnerItemType.NileRush, safeLane, z);
                Spawn(RunnerItemType.Drone, (safeLane + 1) % 3, z + 6f);
                SpawnCredits((safeLane + 2) % 3, z + 8f, 4);
            }
        }

        private void SpawnCredits(int laneIndex, float startZ, int count)
        {
            for (int index = 0; index < count; index++) Spawn(RunnerItemType.Credit, laneIndex, startZ + index * 2.1f);
        }

        private void Spawn(RunnerItemType type, int laneIndex, float z)
        {
            RunnerItem item = items.Find(candidate => !candidate.Active && candidate.Type == type);
            if (item == null) item = items.Find(candidate => candidate.Type == type);
            if (item == null) return;
            item.Root.transform.position = new Vector3((laneIndex - 1) * LaneWidth, 0f, z);
            item.Root.transform.rotation = Quaternion.identity;
            item.Active = true;
            item.Root.SetActive(true);
        }

        private void DisableItem(RunnerItem item)
        {
            item.Active = false;
            item.Root.SetActive(false);
        }

        private void Crash()
        {
            isRunning = false;
            hud.ShowGameOver(Score, credits);
        }

        private void AnimateRunner(float dt)
        {
            float swing = Mathf.Sin(runDistance * 1.25f) * 42f;
            for (int index = 0; index < runnerLimbs.Count; index++)
            {
                float signedSwing = index % 2 == 0 ? swing : -swing;
                runnerLimbs[index].localRotation = Quaternion.Euler(signedSwing, 0f, index < 2 ? (index == 0 ? 12f : -12f) : 0f);
            }
            if (shieldVisual != null && shieldVisual.gameObject.activeSelf) shieldVisual.Rotate(0f, 130f * dt, 0f);
        }

        private void UpdateCamera(float dt)
        {
            Vector3 desired = new Vector3(player.position.x * 0.22f, 5.15f + player.position.y * 0.1f, -9.2f);
            cameraRig.position = Vector3.Lerp(cameraRig.position, desired, dt * 4f);
            cameraRig.LookAt(player.position + new Vector3(0f, 1.05f, 12f));
        }

        private Light NewLight(string title, Vector3 position, Color color, float intensity, float range)
        {
            Light light = new GameObject(title).AddComponent<Light>();
            light.transform.position = position;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            return light;
        }

        private Transform CreatePrimitive(PrimitiveType primitive, string title, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject node = GameObject.CreatePrimitive(primitive);
            node.name = title;
            node.transform.SetParent(parent, false);
            node.transform.localPosition = localPosition;
            node.transform.localScale = localScale;
            node.GetComponent<Renderer>().sharedMaterial = material;
            return node.transform;
        }

        private Material MakeMaterial(string title, string colorHex, string emissionHex)
        {
            Color baseColor;
            Color emission;
            ColorUtility.TryParseHtmlString(colorHex, out baseColor);
            ColorUtility.TryParseHtmlString(emissionHex, out emission);
            Material material = new Material(Shader.Find("Standard"));
            material.name = title;
            material.color = baseColor;
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", emission);
            material.SetFloat("_Glossiness", 0.68f);
            return material;
        }
    }
}
