using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using MidnightMart.Core;
using MidnightMart.Player;
using MidnightMart.Interactions;
using MidnightMart.Combat;
using MidnightMart.Customer;
using MidnightMart.Tutorial;
using MidnightMart.Night;
using MidnightMart.Prologue;
using MidnightMart.UI;
using MidnightMart.Environment;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace MidnightMart.EditorTools
{
    /// <summary>
    /// Công cụ Editor tự động thiết lập TOÀN BỘ kịch bản và môi trường game Midnight Mart:
    /// 1. Căn hộ của người chơi (Home Prologue): Giường ngủ, bàn làm việc, giấy nợ tiền nhà $800,
    ///    tin nhắn tuyển dụng gấp (David ca trước mất tích bí ẩn), cửa lên xe đi làm trong mưa bão.
    /// 2. Siêu thị tiện lợi Midnight Mart hoàn chỉnh:
    ///    - Mặt tiền cửa kính, đường nhựa đêm, cột đèn đường hắt sáng ngoài trời.
    ///    - Quầy thu ngân, máy tính tiền, máy quét mã vạch, súng Shotgun & điện thoại David trong ngăn kéo.
    ///    - Kệ thuốc lá sau quầy, 3 dãy kệ hàng siêu thị (Aisles) ngập tràn đồ, 4 tủ lạnh nước ngọt phát sáng.
    ///    - Gương cầu lồi góc tiệm, cửa kho sau Employees Only.
    /// 3. Hệ thống Gameplay: Kho đồ 2 Slot, nhận diện quái vật không bóng đổ, cúp điện 03:30 AM, tổng kết 06:00 AM.
    /// </summary>
    public static class StoreSceneBuilder
    {
        [MenuItem("MidnightMart/Import TMP Essential Resources", false, 1)]
        public static void ImportTMPEssentialResources()
        {
            string[] packagePaths = System.IO.Directory.GetFiles("Library/PackageCache", "TMP Essential Resources.unitypackage", System.IO.SearchOption.AllDirectories);
            if (packagePaths.Length > 0)
            {
                Debug.Log($"[StoreSceneBuilder] Đang tự động import TextMesh Pro Essential Resources từ: {packagePaths[0]}");
                UnityEditor.AssetPackage.Package.Import(packagePaths[0], false);
            }
            else
            {
                EditorUtility.DisplayDialog("Midnight Mart", "Vui lòng vào menu: [Window ➜ TextMesh Pro ➜ Import TMP Essential Resources] để nạp font chữ TMP.", "OK");
            }
        }

        [MenuItem("MidnightMart/🎬 BẮT ĐẦU TỪ NHÀ (FULL KỊCH BẢN: Ở NHÀ ➜ LÊN XE ➜ SIÊU THỊ ➜ CA TRỰC ĐÊM)", false, 10)]
        public static void BuildFullStoryFromHome()
        {
            BuildEnvironmentInternal(startFromHome: true, quickNightMode: false);

            EditorUtility.DisplayDialog("Midnight Mart - Kịch Bản Đầy Đủ", 
                "ĐÃ DỰNG TOÀN BỘ KỊCH BẢN TỪ NHÀ ĐẾN SIÊU THỊ!\n\n" +
                "1. Bắt đầu trong phòng trọ của bạn (có giấy nợ $800 và tin nhắn David mất tích trên bàn).\n" +
                "2. Nhấn E đọc tin nhắn ➜ Mở cửa phòng bước ra sân nhìn thấy chiếc xe máy dựng sẵn.\n" +
                "3. Lại gần xe máy nhấn [E] lên xe ➜ Nổ máy lái xuyên qua con đường rừng âm u đêm tối (Giữ [W], lượn [A][D])!\n" +
                "4. Xe tấp vào lề tắt máy ➜ Đi lại cửa siêu thị, cửa cảm ứng tự động trượt mở sang 2 bên (Chuông LENG KENG!)\n" +
                "5. Dọn sàn ➜ Khách mua hàng ➜ Nhặt súng lục & shotgun ➜ Ca trực đêm kinh hoàng (01:00 - 06:00)!\n\n" +
                "Bấm nút [PLAY] trên Unity Editor để trải nghiệm ngay!", "BẮT ĐẦU CHƠI TỪ NHÀ");
        }

        [MenuItem("MidnightMart/🏪 BẮT ĐẦU TẠI SIÊU THỊ (TUTORIAL & CA TRỰC ĐÊM)", false, 11)]
        [MenuItem("MidnightMart/Setup Tutorial Store Environment", false, 12)]
        public static void BuildStoreStartEnvironment()
        {
            BuildEnvironmentInternal(startFromHome: false, quickNightMode: false);

            EditorUtility.DisplayDialog("Midnight Mart", 
                "ĐÃ DỰNG TOÀN BỘ SIÊU THỊ TIỆN LỢI HOÀN CHỈNH!\n\n" +
                "• Nội thất đầy đủ: 3 dãy kệ hàng siêu thị, 4 tủ mát nước ngọt phát sáng, kệ thuốc lá sau quầy, gương cầu lồi, cửa kính mặt tiền nhìn ra đường đêm.\n" +
                "• Bắt đầu ngay tại quầy thu ngân để làm quen dọn đồ, quét mã, tính tiền và nhặt súng Shotgun.\n\n" +
                "Bấm nút [PLAY] để chơi ngay!", "BẮT ĐẦU");
        }

        [MenuItem("MidnightMart/⚡ TEST NHANH CA ĐÊM 01:00 AM (CÓ SẴN SÚNG SHOTGUN)", false, 20)]
        public static void QuickStartNightShiftMode()
        {
            BuildEnvironmentInternal(startFromHome: false, quickNightMode: true);

            EditorUtility.DisplayDialog("Midnight Mart", 
                "CHẾ ĐỘ TEST NHANH CA TRỰC ĐÊM ĐÃ SẴN SÀNG!\n\n" +
                "- Bỏ qua đoạn dọn rác và hướng dẫn.\n" +
                "- Súng Shotgun đã mở sẵn ở Slot [2].\n" +
                "- Vào thẳng 01:00 AM để test bắn quái vật không bóng đổ và cúp điện 03:30 AM!\n\n" +
                "Bấm nút [PLAY] để test ngay!", "OK");
        }

        [MenuItem("MidnightMart/🧹 Dọn dẹp & Reset Scene", false, 30)]
        public static void CleanSceneOnly()
        {
            CleanupOldSetup();
            EditorUtility.DisplayDialog("Midnight Mart", "Đã dọn dẹp sạch sẽ toàn bộ bản dựng cũ trong Scene.", "OK");
        }

        private static void BuildEnvironmentInternal(bool startFromHome, bool quickNightMode)
        {
            if (!System.IO.File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset"))
            {
                ImportTMPEssentialResources();
            }

            // Đảm bảo toàn bộ tài nguyên âm thanh vui tai, tiếng súng và tiếng động cơ xe máy sẵn sàng
            if (!System.IO.File.Exists("Assets/Audio/SFX/Barcode_Scan_Beep.wav") || 
                !System.IO.File.Exists("Assets/Audio/SFX/Pistol_Fire.wav") ||
                !System.IO.File.Exists("Assets/Audio/SFX/Motorbike_Engine_Loop.wav"))
            {
                ProceduralAudioGenerator.GenerateAllSFXFiles();
            }

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Setup Midnight Mart Full");
            int groupIndex = Undo.GetCurrentGroup();

            CleanupOldSetup();

            // 0. Tạo Hệ thống Quản Lý Đa Ngôn Ngữ (Localization) & Âm Thanh Toàn Cục (Audio Manager)
            GameObject managersObj = new GameObject("--- GAME MANAGERS ---");
            managersObj.AddComponent<LocalizationManager>();
            managersObj.AddComponent<AudioManager>();
            Undo.RegisterCreatedObjectUndo(managersObj, "Create Game Managers");

            // 1. Root Container
            GameObject environmentRoot = new GameObject("--- STORE ENVIRONMENT ---");
            Undo.RegisterCreatedObjectUndo(environmentRoot, "Create Store Environment");

            // 2. Dựng Căn hộ phòng trọ đặt ở đầu con đường, có sân trước và xe máy dựng sẵn
            GameObject homeApt = CreatePlayerApartment(environmentRoot.transform, out Transform homePlayerSpawn, out Transform aptDoorHinge, out GameObject homeBike);

            // 3. Dựng Sàn, Tường, Cửa kính tự động cảm ứng 2 cánh trượt sang 2 bên, Đèn trần & Đèn khẩn cấp đỏ
            CreateStoreRoom(environmentRoot.transform, out GameObject mainLights, out Light emergencyLight, out StoreAutomaticDoors autoDoors);

            // 4. Dựng Con đường rừng âm u bao quanh bởi cây cối đêm tối & Xe máy đỗ vỉa hè
            CreateGloomyForestRoad(environmentRoot.transform, out Transform roadStartPoint, out GameObject parkedBike);

            // 5. Dựng Quầy thu ngân, Máy tính tiền & Kệ thuốc lá sau quầy
            GameObject counter = CreateCheckoutCounter(environmentRoot.transform, out CashRegister cashRegister, out SecretDrawer secretDrawer);

            // 6. Dựng 3 Dãy kệ hàng siêu thị (Aisles) & 3 Món đồ bừa bộn rơi trên sàn
            CreateSupermarketAislesAndMessItems(environmentRoot.transform);

            // 7. Dựng Dãy 4 Tủ lạnh nước ngọt phát sáng
            CreateDrinkCoolers(environmentRoot.transform);

            // 8. Dựng Gương cầu lồi góc tiệm & Biển Employees Only
            CreateStoreProps(environmentRoot.transform);

            // 9. Tạo Khách hàng & Điểm đứng chờ không xuyên bàn
            CustomerNPC customerNPC = CreateTutorialCustomer(environmentRoot.transform, counter.transform, out Transform doorPoint, out Transform waitingPoint);

            // 10. Tạo Nhân vật Người chơi (Player First-Person Rig) với Camera chính & Ghi-đông lái xe máy
            Vector3 spawnPos = startFromHome ? homePlayerSpawn.position : new Vector3(-3.4f, 1.0f, -3.2f);
            Quaternion spawnRot = startFromHome ? homePlayerSpawn.rotation : Quaternion.Euler(0f, 90f, 0f);
            GameObject player = CreatePlayerRig(spawnPos, spawnRot, parkedBike);

            // 11. Tạo Giao diện UI HUD hoàn chỉnh & Menu Chính
            CreateTutorialCanvas(player, startFromHome, homePlayerSpawn);

            // 12. Tạo Managers
            CreateTutorialManager(customerNPC, environmentRoot, out TutorialManager tutMgr);
            CreateNightShiftManager(mainLights, emergencyLight, customerNPC, doorPoint, waitingPoint, parkedBike, player.GetComponent<MotorbikeRideController>(), out NightShiftManager nightMgr);
            CreatePlayerHomeManager(player.transform, homeApt, aptDoorHinge, homeBike, roadStartPoint, player.GetComponent<MotorbikeRideController>(), tutMgr, startFromHome);

            // Gán TutorialManager cho Cửa kính trượt tự động cảm ứng & Bản ghi nhớ của Quản lý
            if (autoDoors != null)
            {
                SerializedObject doorSO = new SerializedObject(autoDoors);
                SafeSetObject(doorSO, "_tutorialManager", tutMgr);
                doorSO.ApplyModifiedProperties();
            }

            BossDoorMemo bossMemo = Object.FindAnyObjectByType<BossDoorMemo>();
            if (bossMemo != null)
            {
                SerializedObject memoSO = new SerializedObject(bossMemo);
                SafeSetObject(memoSO, "_tutorialManager", tutMgr);
                memoSO.ApplyModifiedProperties();
            }

            // Gán tương tác xe máy trở về nhà sau ca trực
            if (parkedBike != null)
            {
                parkedBike.AddComponent<MidnightMart.Interactions.ReturnMotorbikeMountInteractable>();
                parkedBike.SetActive(!startFromHome);
            }

            if (quickNightMode)
            {
                if (homeApt != null) homeApt.SetActive(false);
                if (tutMgr != null) tutMgr.gameObject.SetActive(false);
                if (nightMgr != null)
                {
                    SerializedObject nightSO = new SerializedObject(nightMgr);
                    SafeSetBool(nightSO, "_autoStartOnAwake", true);
                    nightSO.ApplyModifiedProperties();
                }
                PlayerInventory inv = player.GetComponent<PlayerInventory>();
                if (inv != null)
                {
                    inv.TryAcquireItem(MidnightMart.Player.InventoryItemType.Gun, "SÚNG 9MM", 5, 15);
                }
            }
            else if (!startFromHome)
            {
                if (homeApt != null) homeApt.SetActive(false);
            }
            else
            {
                // Khi bắt đầu từ nhà: Tạm thời tắt TutorialManager để không chèn mục tiêu dọn dẹp lên màn hình
                if (tutMgr != null) tutMgr.gameObject.SetActive(false);
            }

            // Thiết lập bầu không khí đêm tối om kì bí, sương mù dày đặc rùng rợn
            SetupMysteriousPitchBlackNight(player.GetComponentInChildren<Camera>());

            Undo.CollapseUndoOperations(groupIndex);

            // Đánh dấu Scene đã thay đổi và lưu lại ngay lập tức để người chơi nhấn Play là chơi được bản mới nhất
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

            Debug.Log("[StoreSceneBuilder] ĐÃ DỰNG THÀNH CÔNG TOÀN BỘ MÔI TRƯỜNG & LƯU SCENE!");
        }

        private static void CleanupOldSetup()
        {
            string[] oldObjectNames = { "--- STORE ENVIRONMENT ---", "--- TUTORIAL MANAGER ---", "--- NIGHT SHIFT MANAGER ---", "--- HOME PROLOGUE MANAGER ---", "--- GAME MANAGERS ---", "Tutorial_HUD_Canvas", "Player_FPS_Character" };
            foreach (string objName in oldObjectNames)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj != null) Undo.DestroyObjectImmediate(obj);
            }

            // Xóa sạch toàn bộ Canvas cũ trên Scene để giao diện không bao giờ bị đè hoặc lệch
            Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include);
            foreach (Canvas c in allCanvases)
            {
                Undo.DestroyObjectImmediate(c.gameObject);
            }

            // Xóa sạch mọi Camera cũ còn sót lại để Camera chính của Player_FPS_Character là duy nhất
            Camera[] allCameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include);
            foreach (Camera c in allCameras)
            {
                Undo.DestroyObjectImmediate(c.gameObject);
            }
        }

        private static Material CreateColoredMaterial(string name, Color color, float smoothness = 0.4f, Color? emissive = null, float emissiveIntensity = 1800f)
        {
            Shader shader = Shader.Find("HDRP/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Diffuse");

            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (emissive.HasValue && mat.HasProperty("_EmissiveColor"))
            {
                mat.SetColor("_EmissiveColor", emissive.Value);
                if (mat.HasProperty("_EmissiveColorLDR")) mat.SetColor("_EmissiveColorLDR", Color.white);
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emissive.Value);
                if (mat.HasProperty("_EmissiveExposureWeight")) mat.SetFloat("_EmissiveExposureWeight", 0f);
                if (mat.HasProperty("_UseEmissiveIntensity")) mat.SetFloat("_UseEmissiveIntensity", 1f);
                if (mat.HasProperty("_EmissiveIntensity")) mat.SetFloat("_EmissiveIntensity", emissiveIntensity);
                mat.EnableKeyword("_EMISSIVE_COLOR_MAP");
                mat.EnableKeyword("_EMISSION");
            }
            return mat;
        }

        private static Material CreateGlassMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("HDRP/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_SurfaceType")) mat.SetFloat("_SurfaceType", 1f);
            if (mat.HasProperty("_BlendMode")) mat.SetFloat("_BlendMode", 0f);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.95f);
            return mat;
        }

        private static GameObject CreatePlayerApartment(Transform parent, out Transform spawnPoint, out Transform doorHinge, out GameObject homeBike)
        {
            Material wallMat = CreateColoredMaterial("Mat_AptWall", new Color(0.85f, 0.82f, 0.78f), 0.2f);
            Material floorMat = CreateColoredMaterial("Mat_AptWoodFloor", new Color(0.45f, 0.32f, 0.22f), 0.4f);
            Material bedMat = CreateColoredMaterial("Mat_AptBed", new Color(0.2f, 0.35f, 0.65f), 0.3f);
            Material deskMat = CreateColoredMaterial("Mat_AptDesk", new Color(0.35f, 0.25f, 0.18f), 0.4f);
            Material paperMat = CreateColoredMaterial("Mat_RentNotice", new Color(0.95f, 0.95f, 0.85f), 0.1f);
            Material porchMat = CreateColoredMaterial("Mat_PorchDriveway", new Color(0.22f, 0.24f, 0.26f), 0.3f);
            Material doorMat = CreateColoredMaterial("Mat_AptDoorWood", new Color(0.35f, 0.22f, 0.15f), 0.3f);
            Material frameMat = CreateColoredMaterial("Mat_AptMetalFrame", new Color(0.18f, 0.19f, 0.22f), 0.6f);

            GameObject aptRoot = new GameObject("Player_Home_Apartment");
            aptRoot.transform.SetParent(parent);
            aptRoot.transform.position = new Vector3(0f, 0f, -294.0f); // Đặt ở đầu con đường 270m uốn lượn qua rừng cây

            // Sàn phòng trọ (5m x 4m) dạng khối Cube đặc kết thúc chuẩn xác tại Z = +2.0m (chống Z-fighting với sân ngoài)
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Apt_Floor";
            floor.transform.SetParent(aptRoot.transform);
            floor.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            floor.transform.localScale = new Vector3(5.0f, 0.10f, 4.0f);
            floor.GetComponent<Renderer>().sharedMaterial = floorMat;

            // 4 Bức tường & Trần phòng trọ kín khít 100%:
            // Tường sau (Z = -2.0m)
            CreateWall("Apt_Wall_Back", aptRoot.transform, new Vector3(0f, 1.4f, -2.0f), new Vector3(5f, 2.8f, 0.15f), wallMat);
            // Tường trái & Tường phải
            CreateWall("Apt_Wall_Left", aptRoot.transform, new Vector3(-2.5f, 1.4f, 0f), new Vector3(0.15f, 2.8f, 4.0f), wallMat);
            CreateWall("Apt_Wall_Right", aptRoot.transform, new Vector3(2.5f, 1.4f, 0f), new Vector3(0.15f, 2.8f, 4.0f), wallMat);
            // Trần nhà
            CreateWall("Apt_Ceiling", aptRoot.transform, new Vector3(0f, 2.8f, 0f), new Vector3(5f, 0.15f, 4.0f), wallMat);

            // Tường trước (Z = +2.0m - Khớp tuyệt đối không còn kẽ hở nhìn ra ngoài)
            // Khung cửa rộng đúng 1.1m (từ X = -0.55m tới X = +0.55m), cao 2.1m
            CreateWall("Apt_Wall_Front_Left", aptRoot.transform, new Vector3(-1.525f, 1.4f, 2.0f), new Vector3(1.95f, 2.8f, 0.15f), wallMat);
            CreateWall("Apt_Wall_Front_Right", aptRoot.transform, new Vector3(1.525f, 1.4f, 2.0f), new Vector3(1.95f, 2.8f, 0.15f), wallMat);
            CreateWall("Apt_Wall_Front_Top", aptRoot.transform, new Vector3(0f, 2.45f, 2.0f), new Vector3(1.15f, 0.7f, 0.15f), wallMat);

            // Nẹp khung cửa kim loại (Door Jambs & Header) bọc kín khe hở viền tường
            CreateWall("Apt_Door_Frame_Left", aptRoot.transform, new Vector3(-0.58f, 1.05f, 2.0f), new Vector3(0.08f, 2.15f, 0.18f), frameMat);
            CreateWall("Apt_Door_Frame_Right", aptRoot.transform, new Vector3(0.58f, 1.05f, 2.0f), new Vector3(0.08f, 2.15f, 0.18f), frameMat);
            CreateWall("Apt_Door_Frame_Top", aptRoot.transform, new Vector3(0f, 2.12f, 2.0f), new Vector3(1.20f, 0.08f, 0.18f), frameMat);

            // Bản lề Cửa phòng trọ (Door Hinge)
            GameObject dHingeObj = new GameObject("Apartment_Door_Hinge");
            dHingeObj.transform.SetParent(aptRoot.transform);
            dHingeObj.transform.localPosition = new Vector3(-0.55f, 1.05f, 2.0f);
            doorHinge = dHingeObj.transform;

            // Cánh cửa phòng mở được (Treo vào bản lề, phủ kín 100% ô cửa)
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "Apartment_Exit_Door";
            door.transform.SetParent(dHingeObj.transform);
            door.transform.localPosition = new Vector3(0.55f, 0f, 0f);
            door.transform.localScale = new Vector3(1.10f, 2.1f, 0.12f);
            door.GetComponent<Renderer>().sharedMaterial = doorMat;

            // Tay nắm cửa
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            handle.name = "Door_Handle";
            handle.transform.SetParent(door.transform);
            handle.transform.localPosition = new Vector3(0.42f, 0f, -0.07f);
            handle.transform.localScale = new Vector3(0.06f, 0.25f, 0.06f);
            handle.GetComponent<Renderer>().sharedMaterial = frameMat;
            Object.DestroyImmediate(handle.GetComponent<Collider>());

            BoxCollider doorCol = door.GetComponent<BoxCollider>();
            if (doorCol != null) doorCol.size = new Vector3(1.4f, 2.2f, 0.4f);
            HomeInteractable doorInteract = door.AddComponent<HomeInteractable>();
            SerializedObject doorSO = new SerializedObject(doorInteract);
            SafeSetEnum(doorSO, "_type", 1); // ApartmentDoor
            doorSO.ApplyModifiedProperties();

            // Giường ngủ (Có gắn tương tác để người chơi lên giường đi ngủ khi kết thúc ca trực trở về nhà)
            GameObject bed = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bed.name = "Bed";
            bed.transform.SetParent(aptRoot.transform);
            bed.transform.localPosition = new Vector3(-1.4f, 0.35f, -0.8f);
            bed.transform.localScale = new Vector3(1.6f, 0.5f, 2.0f);
            bed.GetComponent<Renderer>().sharedMaterial = bedMat;
            bed.AddComponent<MidnightMart.Prologue.ApartmentBedInteractable>();

            // Bàn làm việc
            GameObject desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            desk.name = "Desk_With_Job_Notice";
            desk.transform.SetParent(aptRoot.transform);
            desk.transform.localPosition = new Vector3(1.4f, 0.4f, -0.8f);
            desk.transform.localScale = new Vector3(1.4f, 0.8f, 0.7f);
            desk.GetComponent<Renderer>().sharedMaterial = deskMat;
            BoxCollider deskCol = desk.GetComponent<BoxCollider>();
            if (deskCol != null) deskCol.size = new Vector3(1.5f, 1.2f, 1.0f);
            HomeInteractable deskInteract = desk.AddComponent<HomeInteractable>();
            SerializedObject deskSO = new SerializedObject(deskInteract);
            SafeSetEnum(deskSO, "_type", 0); // DeskNotice
            deskSO.ApplyModifiedProperties();

            // Laptop
            GameObject laptop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            laptop.name = "Laptop";
            laptop.transform.SetParent(desk.transform);
            laptop.transform.localPosition = new Vector3(0.2f, 0.52f, 0f);
            laptop.transform.localScale = new Vector3(0.35f, 0.04f, 0.3f);
            laptop.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_Laptop", Color.black, 0.7f);

            // Giấy nợ & Tin nhắn việc làm
            GameObject notice = GameObject.CreatePrimitive(PrimitiveType.Cube);
            notice.name = "Job_And_Rent_Notice";
            notice.transform.SetParent(desk.transform);
            notice.transform.localPosition = new Vector3(-0.25f, 0.55f, 0f);
            notice.transform.localScale = new Vector3(0.45f, 0.08f, 0.35f);
            notice.GetComponent<Renderer>().sharedMaterial = paperMat;
            BoxCollider noticeCol = notice.GetComponent<BoxCollider>();
            if (noticeCol != null) noticeCol.size = new Vector3(0.6f, 0.4f, 0.5f);
            HomeInteractable noticeInteract = notice.AddComponent<HomeInteractable>();
            SerializedObject noticeSO = new SerializedObject(noticeInteract);
            SafeSetEnum(noticeSO, "_type", 0); // DeskNotice
            noticeSO.ApplyModifiedProperties();

            // Đèn bàn học
            GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lamp.name = "Desk_Lamp";
            lamp.transform.SetParent(desk.transform);
            lamp.transform.localPosition = new Vector3(0.5f, 0.65f, 0.2f);
            lamp.transform.localScale = new Vector3(0.12f, 0.2f, 0.12f);
            Light lampLight = lamp.AddComponent<Light>();
            lampLight.type = LightType.Point;
            lampLight.color = new Color(1.0f, 0.85f, 0.6f);
            lampLight.range = 4.0f;
            lampLight.intensity = 120f;

            // Đèn trần phòng trọ
            GameObject ceilingLightObj = new GameObject("Apt_Ceiling_Warm_Light");
            ceilingLightObj.transform.SetParent(aptRoot.transform);
            ceilingLightObj.transform.localPosition = new Vector3(0f, 2.5f, 0f);
            Light cLight = ceilingLightObj.AddComponent<Light>();
            cLight.type = LightType.Point;
            cLight.color = new Color(1.0f, 0.92f, 0.82f);
            cLight.range = 6.0f;
            cLight.intensity = 180f;

            // Điểm sinh nhân vật trong phòng trọ (Nhìn thẳng ra bàn và cửa phòng)
            GameObject pSpawn = new GameObject("Apartment_Player_Spawn");
            pSpawn.transform.SetParent(aptRoot.transform);
            pSpawn.transform.localPosition = new Vector3(0f, 1.0f, -0.6f);
            pSpawn.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            spawnPoint = pSpawn.transform;

            // Sân trước & Lối chạy xe (Driveway nối liền ra quốc lộ Z = -280m)
            // Đặt bắt đầu từ ngoài cửa (Z = 2.05m đến 15.0m), hạ bậc thềm sạch sẽ Y = -0.02m (TRIỆT TIÊU 100% Z-FIGHTING)
            GameObject driveway = GameObject.CreatePrimitive(PrimitiveType.Cube);
            driveway.name = "Apartment_Front_Driveway";
            driveway.transform.SetParent(aptRoot.transform);
            driveway.transform.localPosition = new Vector3(0f, -0.04f, 8.525f);
            driveway.transform.localScale = new Vector3(7.5f, 0.04f, 12.95f);
            driveway.GetComponent<Renderer>().sharedMaterial = porchMat;

            // Đèn hiên nhà chiếu sáng chiếc xe máy đỗ ngoài sân (ánh vàng dịu ấm)
            GameObject porchLightObj = new GameObject("Porch_Warm_Lantern");
            porchLightObj.transform.SetParent(aptRoot.transform);
            porchLightObj.transform.localPosition = new Vector3(0.9f, 2.2f, 2.15f);
            Light porchLight = porchLightObj.AddComponent<Light>();
            porchLight.type = LightType.Point;
            porchLight.range = 6f;
            porchLight.intensity = 150f;
            porchLight.color = new Color(1.0f, 0.85f, 0.6f);

            // Chiếc xe máy dựng sẵn ngoài sân nhà (Nhìn thấy ngay khi mở cửa phòng)
            Material poleMat = CreateColoredMaterial("Mat_StreetLampPole", new Color(0.18f, 0.2f, 0.22f), 0.5f);
            Material rubberMat = CreateColoredMaterial("Mat_TireRubber", new Color(0.06f, 0.06f, 0.06f), 0.2f);
            Material chromeMat = CreateColoredMaterial("Mat_ChromeMetal", new Color(0.85f, 0.85f, 0.88f), 0.9f);
            Material bikeTankMat = CreateColoredMaterial("Mat_MotorbikeTank", new Color(0.08f, 0.14f, 0.32f), 0.85f);

            GameObject homeMotorbike = CreateParkedMotorbike(aptRoot.transform, new Vector3(0.8f, 0.08f, -288.5f), Quaternion.Euler(0f, 10f, -6f), poleMat, rubberMat, chromeMat, bikeTankMat);
            homeMotorbike.name = "Motorbike_Home_Prop";
            homeMotorbike.AddComponent<MotorbikeMountInteractable>();
            homeBike = homeMotorbike;

            return aptRoot;
        }

        private static void CreateStoreRoom(Transform parent, out GameObject mainLightsRoot, out Light emergencyLight, out StoreAutomaticDoors autoDoors)
        {
            Material floorMat = CreateColoredMaterial("Mat_FloorTiles", new Color(0.28f, 0.30f, 0.33f), 0.25f);
            Material wallMat = CreateColoredMaterial("Mat_Walls", new Color(0.60f, 0.62f, 0.65f), 0.2f);
            Material ceilingMat = CreateColoredMaterial("Mat_Ceiling", new Color(0.35f, 0.35f, 0.38f), 0.1f);
            Material glassMat = CreateGlassMaterial("Mat_StoreGlass", new Color(0.4f, 0.6f, 0.8f, 0.35f));
            Material frameMat = CreateColoredMaterial("Mat_MetalFrame", new Color(0.15f, 0.15f, 0.18f), 0.6f);

            // Sàn nhà siêu thị mở rộng gấp đôi (22m x 20m = 440m2 cho không gian bắn quái kiting thoải mái)
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor_Tiles";
            floor.transform.SetParent(parent);
            floor.transform.position = new Vector3(0f, 0f, 5.0f);
            floor.transform.localScale = new Vector3(2.2f, 1.0f, 2.0f);
            floor.GetComponent<Renderer>().sharedMaterial = floorMat;

            // Tường Sau, Trái, Phải và Trần nhà
            CreateWall("Wall_Back_BehindCounter", parent, new Vector3(0f, 1.9f, 15.0f), new Vector3(22.0f, 3.8f, 0.2f), wallMat);
            CreateWall("Wall_Left", parent, new Vector3(-11.0f, 1.9f, 5.0f), new Vector3(0.2f, 3.8f, 20.0f), wallMat);
            CreateWall("Wall_Right", parent, new Vector3(11.0f, 1.9f, 5.0f), new Vector3(0.2f, 3.8f, 20.0f), wallMat);
            CreateWall("Ceiling", parent, new Vector3(0f, 3.8f, 5.0f), new Vector3(22.0f, 0.2f, 20.0f), ceilingMat);

            // Mặt tiền: Tường đặc bê tông kiên cố hai bên, CHỈ CÓ CỬA RA VÀO CHÍNH GIỮA LÀ KÍNH (Theo yêu cầu người chơi)
            CreateWall("Storefront_Top_Frame", parent, new Vector3(0f, 3.5f, -5.0f), new Vector3(22.0f, 0.6f, 0.2f), frameMat);
            CreateWall("Storefront_Solid_Wall_Left", parent, new Vector3(-6.075f, 1.6f, -5.0f), new Vector3(9.85f, 3.2f, 0.2f), wallMat);
            CreateWall("Storefront_Solid_Wall_Right", parent, new Vector3(6.075f, 1.6f, -5.0f), new Vector3(9.85f, 3.2f, 0.2f), wallMat);

            // Cửa kính tự động cảm ứng 2 cánh trượt sang 2 bên (Storefront Automatic Sensor Sliding Doors)
            GameObject autoDoorRoot = new GameObject("Storefront_Automatic_Doors");
            autoDoorRoot.transform.SetParent(parent);
            autoDoorRoot.transform.position = new Vector3(0f, 1.85f, -5.0f);

            // Cánh cửa kính bên trái (Tự động trượt sang trái khi có người lại gần)
            GameObject doorLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorLeft.name = "Auto_Door_Panel_Left";
            doorLeft.transform.SetParent(autoDoorRoot.transform);
            doorLeft.transform.localPosition = new Vector3(-0.58f, 0f, 0f);
            doorLeft.transform.localScale = new Vector3(1.15f, 2.7f, 0.08f);
            doorLeft.GetComponent<Renderer>().sharedMaterial = glassMat;

            // Khung viền cánh trái
            GameObject frameLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frameLeft.name = "Door_Frame_Left";
            frameLeft.transform.SetParent(doorLeft.transform);
            frameLeft.transform.localPosition = new Vector3(0.48f, 0f, 0f);
            frameLeft.transform.localScale = new Vector3(0.06f, 1.0f, 1.2f);
            frameLeft.GetComponent<Renderer>().sharedMaterial = frameMat;
            Object.DestroyImmediate(frameLeft.GetComponent<Collider>());

            // Cánh cửa kính bên phải (Tự động trượt sang phải khi có người lại gần)
            GameObject doorRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorRight.name = "Auto_Door_Panel_Right";
            doorRight.transform.SetParent(autoDoorRoot.transform);
            doorRight.transform.localPosition = new Vector3(0.58f, 0f, 0f);
            doorRight.transform.localScale = new Vector3(1.15f, 2.7f, 0.08f);
            doorRight.GetComponent<Renderer>().sharedMaterial = glassMat;

            // Khung viền cánh phải
            GameObject frameRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frameRight.name = "Door_Frame_Right";
            frameRight.transform.SetParent(doorRight.transform);
            frameRight.transform.localPosition = new Vector3(-0.48f, 0f, 0f);
            frameRight.transform.localScale = new Vector3(0.06f, 1.0f, 1.2f);
            frameRight.GetComponent<Renderer>().sharedMaterial = frameMat;
            Object.DestroyImmediate(frameRight.GetComponent<Collider>());

            // Hộp máy cảm biến phía trên cửa (Overhead Sensor Box)
            GameObject sensorBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sensorBox.name = "Sensor_Drive_Box_Top";
            sensorBox.transform.SetParent(autoDoorRoot.transform);
            sensorBox.transform.localPosition = new Vector3(0f, 1.45f, 0f);
            sensorBox.transform.localScale = new Vector3(2.5f, 0.22f, 0.25f);
            sensorBox.GetComponent<Renderer>().sharedMaterial = frameMat;
            Object.DestroyImmediate(sensorBox.GetComponent<Collider>());

            // Đèn led cảm biến nhỏ màu xanh lục
            GameObject sensorEye = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sensorEye.name = "Motion_Sensor_Eye";
            sensorEye.transform.SetParent(sensorBox.transform);
            sensorEye.transform.localPosition = new Vector3(0f, -0.2f, -0.55f);
            sensorEye.transform.localScale = new Vector3(0.15f, 0.08f, 0.1f);
            sensorEye.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_SensorEyeGlow", new Color(0.1f, 0.9f, 0.3f), 0.5f, new Color(0.1f, 0.9f, 0.3f) * 2f);
            Object.DestroyImmediate(sensorEye.GetComponent<Collider>());

            autoDoors = autoDoorRoot.AddComponent<StoreAutomaticDoors>();
            SerializedObject autoDoorSO = new SerializedObject(autoDoors);
            SafeSetObject(autoDoorSO, "_leftDoorPanel", doorLeft.transform);
            SafeSetObject(autoDoorSO, "_rightDoorPanel", doorRight.transform);
            SafeSetFloat(autoDoorSO, "_slideDistance", 1.15f);
            SafeSetFloat(autoDoorSO, "_slideSpeed", 3.5f);
            SafeSetFloat(autoDoorSO, "_sensorDistance", 3.2f);
            SafeSetBool(autoDoorSO, "_isLocked", true);
            SafeSetObject(autoDoorSO, "_doorChimeClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Door_Chime_DingDong.wav"));
            autoDoorSO.ApplyModifiedProperties();

            // Tờ giấy note hướng dẫn ca đêm của Quản Lý (Boss Shift Memo) dán chắc chắn trên tường bên cạnh cửa (Storefront_Solid_Wall_Left)
            // Tuyệt đối không dán lên cánh cửa trượt để tránh bị lơ lửng khi cửa cảm ứng tự động mở sang 2 bên!
            GameObject memoObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            memoObj.name = "Boss_Door_Memo_Note";
            memoObj.transform.SetParent(parent);
            memoObj.transform.position = new Vector3(-1.55f, 1.65f, -5.11f);
            memoObj.transform.rotation = Quaternion.identity;
            memoObj.transform.localScale = new Vector3(0.28f, 0.38f, 0.012f);
            memoObj.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_BossMemoYellow", new Color(0.98f, 0.95f, 0.45f), 0.3f);

            // Băng dính đen dán mép trên tờ note cố định trên tường
            GameObject tapeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tapeObj.name = "Memo_Tape_Strip";
            tapeObj.transform.SetParent(memoObj.transform);
            tapeObj.transform.localPosition = new Vector3(0f, 0.48f, -0.6f);
            tapeObj.transform.localScale = new Vector3(0.55f, 0.12f, 0.4f);
            tapeObj.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_MemoTape", new Color(0.25f, 0.25f, 0.28f), 0.2f);
            Object.DestroyImmediate(tapeObj.GetComponent<Collider>());

            BossDoorMemo memo = memoObj.AddComponent<BossDoorMemo>();
            SerializedObject memoSO = new SerializedObject(memo);
            SafeSetObject(memoSO, "_storeDoors", autoDoors);
            SafeSetObject(memoSO, "_paperRustleClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Paper_Rustle.wav"));
            memoSO.ApplyModifiedProperties();

            // Hệ thống 9 Đèn tuýp huỳnh quang treo trần phân bổ đều khắp không gian 22m x 20m
            mainLightsRoot = new GameObject("Main_Store_Lights");
            mainLightsRoot.transform.SetParent(parent);

            Vector3[] lightPositions = new Vector3[]
            {
                new Vector3(-6.0f, 3.5f, -1.5f), new Vector3(0f, 3.5f, -1.5f), new Vector3(6.0f, 3.5f, -1.5f),
                new Vector3(-6.0f, 3.5f, 4.0f),  new Vector3(0f, 3.5f, 4.0f),  new Vector3(6.0f, 3.5f, 4.0f),
                new Vector3(-6.0f, 3.5f, 9.5f),  new Vector3(0f, 3.5f, 9.5f),  new Vector3(6.0f, 3.5f, 9.5f)
            };

            for (int i = 0; i < lightPositions.Length; i++)
            {
                GameObject tubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tubeObj.name = $"Fluorescent_Fixture_{i + 1}";
                tubeObj.transform.SetParent(mainLightsRoot.transform);
                tubeObj.transform.position = lightPositions[i];
                tubeObj.transform.localScale = new Vector3(0.3f, 0.1f, 2.5f);
                tubeObj.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_Fixture", Color.white, 0.5f, new Color(2.5f, 2.7f, 3.0f));

                // Ánh sáng tản mờ hắt trần chống tối góc khuất (dịu nhẹ vừa phải)
                Light tubeLight = tubeObj.AddComponent<Light>();
                tubeLight.type = LightType.Point;
                tubeLight.range = 5f;
                tubeLight.intensity = 80f;
                tubeLight.color = new Color(0.95f, 0.98f, 1.0f);
                tubeLight.shadows = LightShadows.None;

                // Nguồn sáng Downlight Spot CHIẾU THẲNG VUÔNG GÓC XUỐNG SÀN NHÀ (Rotation = 90, 0, 0)
                GameObject spotObj = new GameObject($"Fixture_Downlight_Spot_{i + 1}");
                spotObj.transform.SetParent(tubeObj.transform);
                spotObj.transform.localPosition = new Vector3(0f, -0.15f, 0f);
                spotObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                Light downLight = spotObj.AddComponent<Light>();
                downLight.type = LightType.Spot;
                downLight.range = 8.5f;
                downLight.spotAngle = 105f;
                downLight.innerSpotAngle = 65f;
                downLight.intensity = 750f;
                downLight.color = new Color(0.96f, 0.98f, 1.0f);
                downLight.shadows = LightShadows.Soft;
            }

            // Đèn khẩn cấp màu đỏ (Cho sự kiện Cúp điện 03:30 AM - đặt tại trung tâm trần Z = 5.0m)
            GameObject redLightObj = new GameObject("Emergency_Red_Alarm_Light");
            redLightObj.transform.SetParent(parent);
            redLightObj.transform.position = new Vector3(0f, 3.5f, 5.0f);
            emergencyLight = redLightObj.AddComponent<Light>();
            emergencyLight.type = LightType.Point;
            emergencyLight.color = Color.red;
            emergencyLight.range = 28f;
            emergencyLight.intensity = 1400f;
            emergencyLight.enabled = false;
        }

        private static void CreatePineTree(Transform parent, Vector3 position, float scale, Material trunkMat, Material foliageMat)
        {
            GameObject treeRoot = new GameObject($"Pine_Tree_{position.z:0}_{position.x:0}");
            treeRoot.transform.SetParent(parent);
            treeRoot.transform.position = position;

            GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Detailed_PineTree.obj");
            if (treePrefab != null)
            {
                GameObject visual = Object.Instantiate(treePrefab, treeRoot.transform);
                visual.name = "Detailed_Pine_Mesh";
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                visual.transform.localScale = Vector3.one * (scale * 0.95f);
                return;
            }

            float trunkHeight = 6.5f * scale;

            // Thân cây gỗ xám đen
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(treeRoot.transform);
            trunk.transform.localPosition = new Vector3(0f, trunkHeight * 0.5f, 0f);
            trunk.transform.localScale = new Vector3(0.38f * scale, trunkHeight * 0.5f, 0.38f * scale);
            trunk.GetComponent<Renderer>().sharedMaterial = trunkMat;

            // Tầng lá thông 1 (dưới cùng, rộng nhất)
            GameObject tier1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tier1.name = "Canopy_Tier1";
            tier1.transform.SetParent(treeRoot.transform);
            tier1.transform.localPosition = new Vector3(0f, trunkHeight * 0.52f, 0f);
            tier1.transform.localScale = new Vector3(3.2f * scale, 1.8f * scale, 3.2f * scale);
            tier1.GetComponent<Renderer>().sharedMaterial = foliageMat;
            Object.DestroyImmediate(tier1.GetComponent<Collider>());

            // Tầng lá thông 2
            GameObject tier2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tier2.name = "Canopy_Tier2";
            tier2.transform.SetParent(treeRoot.transform);
            tier2.transform.localPosition = new Vector3(0f, trunkHeight * 0.72f, 0f);
            tier2.transform.localScale = new Vector3(2.4f * scale, 1.6f * scale, 2.4f * scale);
            tier2.GetComponent<Renderer>().sharedMaterial = foliageMat;
            Object.DestroyImmediate(tier2.GetComponent<Collider>());

            // Tầng lá thông 3
            GameObject tier3 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tier3.name = "Canopy_Tier3";
            tier3.transform.SetParent(treeRoot.transform);
            tier3.transform.localPosition = new Vector3(0f, trunkHeight * 0.90f, 0f);
            tier3.transform.localScale = new Vector3(1.6f * scale, 1.4f * scale, 1.6f * scale);
            tier3.GetComponent<Renderer>().sharedMaterial = foliageMat;
            Object.DestroyImmediate(tier3.GetComponent<Collider>());

            // Đỉnh ngọn thông
            GameObject tier4 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tier4.name = "Canopy_Tier4";
            tier4.transform.SetParent(treeRoot.transform);
            tier4.transform.localPosition = new Vector3(0f, trunkHeight * 1.05f, 0f);
            tier4.transform.localScale = new Vector3(0.9f * scale, 1.0f * scale, 0.9f * scale);
            tier4.GetComponent<Renderer>().sharedMaterial = foliageMat;
            Object.DestroyImmediate(tier4.GetComponent<Collider>());
        }

        private static void CreateDeadTree(Transform parent, Vector3 position, float scale, Material deadWoodMat, float tiltAngle)
        {
            GameObject deadRoot = new GameObject($"Dead_Gnarled_Tree_{position.z:0}");
            deadRoot.transform.SetParent(parent);
            deadRoot.transform.position = position;
            deadRoot.transform.rotation = Quaternion.Euler(tiltAngle, 0f, 0f);

            GameObject deadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Detailed_DeadTree.obj");
            if (deadPrefab != null)
            {
                GameObject visual = Object.Instantiate(deadPrefab, deadRoot.transform);
                visual.name = "Detailed_DeadTree_Mesh";
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                visual.transform.localScale = Vector3.one * (scale * 0.95f);
                return;
            }

            float trunkHeight = 5.5f * scale;

            // Thân cây gãy gục cằn cỗi
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Dead_Trunk";
            trunk.transform.SetParent(deadRoot.transform);
            trunk.transform.localPosition = new Vector3(0f, trunkHeight * 0.5f, 0f);
            trunk.transform.localScale = new Vector3(0.32f * scale, trunkHeight * 0.5f, 0.32f * scale);
            trunk.GetComponent<Renderer>().sharedMaterial = deadWoodMat;

            // Cành cây khẳng khiu vươn ra đường
            GameObject branch1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            branch1.name = "Branch1";
            branch1.transform.SetParent(deadRoot.transform);
            branch1.transform.localPosition = new Vector3(0.6f * scale, trunkHeight * 0.7f, 0f);
            branch1.transform.localRotation = Quaternion.Euler(0f, 0f, -45f);
            branch1.transform.localScale = new Vector3(0.12f * scale, 1.0f * scale, 0.12f * scale);
            branch1.GetComponent<Renderer>().sharedMaterial = deadWoodMat;
            Object.DestroyImmediate(branch1.GetComponent<Collider>());

            GameObject branch2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            branch2.name = "Branch2";
            branch2.transform.SetParent(deadRoot.transform);
            branch2.transform.localPosition = new Vector3(-0.5f * scale, trunkHeight * 0.82f, 0f);
            branch2.transform.localRotation = Quaternion.Euler(0f, 0f, 40f);
            branch2.transform.localScale = new Vector3(0.1f * scale, 0.8f * scale, 0.1f * scale);
            branch2.GetComponent<Renderer>().sharedMaterial = deadWoodMat;
            Object.DestroyImmediate(branch2.GetComponent<Collider>());
        }

        private static GameObject CreateParkedMotorbike(Transform parent, Vector3 position, Quaternion rotation, Material frameMat, Material wheelMat, Material chromeMat, Material tankMat)
        {
            GameObject bikeObj = new GameObject("Parked_Motorbike_World_Prop");
            bikeObj.transform.SetParent(parent);
            bikeObj.transform.position = position;
            bikeObj.transform.rotation = rotation;

            GameObject bikePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Detailed_Motorbike.obj");
            if (bikePrefab != null)
            {
                GameObject visual = Object.Instantiate(bikePrefab, bikeObj.transform);
                visual.name = "Detailed_Motorbike_Mesh";
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                visual.transform.localScale = Vector3.one;

                BoxCollider bikeCol = bikeObj.AddComponent<BoxCollider>();
                bikeCol.center = new Vector3(0f, 0.55f, 0f);
                bikeCol.size = new Vector3(0.7f, 1.1f, 1.9f);
                return bikeObj;
            }

            // Bánh trước
            GameObject fWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fWheel.name = "Front_Wheel";
            fWheel.transform.SetParent(bikeObj.transform);
            fWheel.transform.localPosition = new Vector3(0f, 0.32f, 0.85f);
            fWheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            fWheel.transform.localScale = new Vector3(0.65f, 0.08f, 0.65f);
            fWheel.GetComponent<Renderer>().sharedMaterial = wheelMat;
            Object.DestroyImmediate(fWheel.GetComponent<Collider>());

            // Vành bánh trước
            GameObject fRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fRim.name = "Front_Rim";
            fRim.transform.SetParent(fWheel.transform);
            fRim.transform.localPosition = Vector3.zero;
            fRim.transform.localRotation = Quaternion.identity;
            fRim.transform.localScale = new Vector3(0.75f, 1.05f, 0.75f);
            fRim.GetComponent<Renderer>().sharedMaterial = chromeMat;
            Object.DestroyImmediate(fRim.GetComponent<Collider>());

            // Bánh sau
            GameObject rWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rWheel.name = "Rear_Wheel";
            rWheel.transform.SetParent(bikeObj.transform);
            rWheel.transform.localPosition = new Vector3(0f, 0.32f, -0.75f);
            rWheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            rWheel.transform.localScale = new Vector3(0.65f, 0.12f, 0.65f);
            rWheel.GetComponent<Renderer>().sharedMaterial = wheelMat;
            Object.DestroyImmediate(rWheel.GetComponent<Collider>());

            // Vành bánh sau
            GameObject rRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rRim.name = "Rear_Rim";
            rRim.transform.SetParent(rWheel.transform);
            rRim.transform.localPosition = Vector3.zero;
            rRim.transform.localRotation = Quaternion.identity;
            rRim.transform.localScale = new Vector3(0.75f, 1.05f, 0.75f);
            rRim.GetComponent<Renderer>().sharedMaterial = chromeMat;
            Object.DestroyImmediate(rRim.GetComponent<Collider>());

            // Khung máy chính
            GameObject engine = GameObject.CreatePrimitive(PrimitiveType.Cube);
            engine.name = "Engine_Block";
            engine.transform.SetParent(bikeObj.transform);
            engine.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            engine.transform.localScale = new Vector3(0.26f, 0.36f, 0.55f);
            engine.GetComponent<Renderer>().sharedMaterial = frameMat;
            Object.DestroyImmediate(engine.GetComponent<Collider>());

            // Bình xăng kim loại màu xanh đen bóng
            GameObject tank = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tank.name = "Fuel_Tank";
            tank.transform.SetParent(bikeObj.transform);
            tank.transform.localPosition = new Vector3(0f, 0.72f, 0.25f);
            tank.transform.localScale = new Vector3(0.3f, 0.22f, 0.55f);
            tank.GetComponent<Renderer>().sharedMaterial = tankMat;
            Object.DestroyImmediate(tank.GetComponent<Collider>());

            // Yên da xe
            GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "Leather_Seat";
            seat.transform.SetParent(bikeObj.transform);
            seat.transform.localPosition = new Vector3(0f, 0.68f, -0.3f);
            seat.transform.localScale = new Vector3(0.28f, 0.12f, 0.6f);
            seat.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_LeatherSeat", new Color(0.12f, 0.10f, 0.08f), 0.25f);
            Object.DestroyImmediate(seat.GetComponent<Collider>());

            // Ghi đông & Phuộc trước
            GameObject forkL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            forkL.name = "Front_Fork_L";
            forkL.transform.SetParent(bikeObj.transform);
            forkL.transform.localPosition = new Vector3(-0.1f, 0.65f, 0.75f);
            forkL.transform.localRotation = Quaternion.Euler(-18f, 0f, 0f);
            forkL.transform.localScale = new Vector3(0.04f, 0.45f, 0.04f);
            forkL.GetComponent<Renderer>().sharedMaterial = chromeMat;
            Object.DestroyImmediate(forkL.GetComponent<Collider>());

            GameObject forkR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            forkR.name = "Front_Fork_R";
            forkR.transform.SetParent(bikeObj.transform);
            forkR.transform.localPosition = new Vector3(0.1f, 0.65f, 0.75f);
            forkR.transform.localRotation = Quaternion.Euler(-18f, 0f, 0f);
            forkR.transform.localScale = new Vector3(0.04f, 0.45f, 0.04f);
            forkR.GetComponent<Renderer>().sharedMaterial = chromeMat;
            Object.DestroyImmediate(forkR.GetComponent<Collider>());

            // Ghi-đông
            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bar.name = "Handlebar";
            bar.transform.SetParent(bikeObj.transform);
            bar.transform.localPosition = new Vector3(0f, 0.98f, 0.65f);
            bar.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            bar.transform.localScale = new Vector3(0.035f, 0.35f, 0.035f);
            bar.GetComponent<Renderer>().sharedMaterial = frameMat;
            Object.DestroyImmediate(bar.GetComponent<Collider>());

            // Đèn pha tròn
            GameObject headLight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            headLight.name = "Headlight";
            headLight.transform.SetParent(bikeObj.transform);
            headLight.transform.localPosition = new Vector3(0f, 0.88f, 0.8f);
            headLight.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            headLight.transform.localScale = new Vector3(0.18f, 0.08f, 0.18f);
            headLight.GetComponent<Renderer>().sharedMaterial = chromeMat;
            Object.DestroyImmediate(headLight.GetComponent<Collider>());

            // Ống xả mạ crom
            GameObject exhaust = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            exhaust.name = "Exhaust_Pipe";
            exhaust.transform.SetParent(bikeObj.transform);
            exhaust.transform.localPosition = new Vector3(0.18f, 0.28f, -0.4f);
            exhaust.transform.localRotation = Quaternion.Euler(70f, 0f, 0f);
            exhaust.transform.localScale = new Vector3(0.06f, 0.45f, 0.06f);
            exhaust.GetComponent<Renderer>().sharedMaterial = chromeMat;
            Object.DestroyImmediate(exhaust.GetComponent<Collider>());

            // Chân chống nghiêng
            GameObject kickstand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            kickstand.name = "Kickstand";
            kickstand.transform.SetParent(bikeObj.transform);
            kickstand.transform.localPosition = new Vector3(-0.16f, 0.16f, -0.1f);
            kickstand.transform.localRotation = Quaternion.Euler(0f, 0f, 30f);
            kickstand.transform.localScale = new Vector3(0.03f, 0.2f, 0.03f);
            kickstand.GetComponent<Renderer>().sharedMaterial = frameMat;
            Object.DestroyImmediate(kickstand.GetComponent<Collider>());

            // Box Collider tổng thể để người chơi có thể va chạm tự nhiên nếu đi bộ lại gần
            BoxCollider col = bikeObj.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, 0.5f, 0f);
            col.size = new Vector3(0.7f, 1.0f, 1.9f);

            return bikeObj;
        }

        private static void CreateGloomyForestRoad(Transform parent, out Transform roadStartPoint, out GameObject parkedBike)
        {
            // Độ nhám cao, không bắt phản chiếu, đen than chì ẩm ướt để loại bỏ hiện tượng đường bị sáng
            Material asphaltMat = CreateColoredMaterial("Mat_AsphaltRoad", new Color(0.012f, 0.014f, 0.016f), 0.02f);
            Material forestGroundMat = CreateColoredMaterial("Mat_DarkForestGround", new Color(0.008f, 0.010f, 0.008f), 0.0f);
            Material curbMat = CreateColoredMaterial("Mat_RoadCurb", new Color(0.12f, 0.12f, 0.14f), 0.1f);
            Material guardrailMat = CreateColoredMaterial("Mat_GuardrailSteel", new Color(0.16f, 0.18f, 0.20f), 0.3f);
            Material yellowLineMat = CreateColoredMaterial("Mat_RoadYellowLine", new Color(0.35f, 0.28f, 0.06f), 0.1f);
            Material whiteLineMat = CreateColoredMaterial("Mat_RoadWhiteLine", new Color(0.25f, 0.25f, 0.25f), 0.1f);
            Material treeTrunkMat = CreateColoredMaterial("Mat_TreeTrunk", new Color(0.10f, 0.07f, 0.05f), 0.05f);
            Material pineFoliageMat = CreateColoredMaterial("Mat_DarkPineFoliage", new Color(0.02f, 0.045f, 0.025f), 0.05f);
            Material deadWoodMat = CreateColoredMaterial("Mat_DeadWood", new Color(0.11f, 0.10f, 0.09f), 0.05f);
            Material poleMat = CreateColoredMaterial("Mat_StreetLampPole", new Color(0.15f, 0.16f, 0.18f), 0.4f);
            Material patrolCarMat = CreateColoredMaterial("Mat_PatrolCarBody", new Color(0.06f, 0.07f, 0.08f), 0.5f);
            Material bikeTankMat = CreateColoredMaterial("Mat_MotorbikeTank", new Color(0.08f, 0.14f, 0.32f), 0.85f);
            Material chromeMat = CreateColoredMaterial("Mat_ChromeMetal", new Color(0.85f, 0.85f, 0.88f), 0.9f);
            Material rubberMat = CreateColoredMaterial("Mat_TireRubber", new Color(0.05f, 0.05f, 0.05f), 0.1f);

            GameObject exteriorRoot = new GameObject("Gloomy_Forest_Road_Exterior");
            exteriorRoot.transform.SetParent(parent);

            // 1. Mặt đất rừng ẩm ướt mênh mông (Bao phủ toàn bộ chiều dài 280m)
            GameObject forestGround = GameObject.CreatePrimitive(PrimitiveType.Plane);
            forestGround.name = "Dark_Forest_Ground";
            forestGround.transform.SetParent(exteriorRoot.transform);
            forestGround.transform.position = new Vector3(0f, -0.06f, -145.0f);
            forestGround.transform.localScale = new Vector3(16.0f, 1.0f, 32.0f);
            forestGround.GetComponent<Renderer>().sharedMaterial = forestGroundMat;

            // 2. Mặt đường nhựa uốn lượn S-Curve dài gấp đôi (270m, từ Z = -282m tới Z = -8m)
            for (float z = -282.0f; z <= -8.0f; z += 4.0f)
            {
                float xMid = RoadCurveUtility.GetRoadCenterX(z);
                float angle = RoadCurveUtility.GetRoadTangentAngle(z);
                Quaternion rot = Quaternion.Euler(0f, angle, 0f);

                // Bản mặt đường nhựa 2 làn
                GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slab.name = $"Asphalt_Road_Slab_{z:0}";
                slab.transform.SetParent(exteriorRoot.transform);
                slab.transform.position = new Vector3(xMid, -0.01f, z);
                slab.transform.rotation = rot;
                slab.transform.localScale = new Vector3(7.2f, 0.06f, 4.15f);
                slab.GetComponent<Renderer>().sharedMaterial = asphaltMat;

                // Vạch kẻ tim đường vàng đứt đoạn
                GameObject dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
                dash.name = $"Road_Center_Dash_{z:0}";
                dash.transform.SetParent(exteriorRoot.transform);
                dash.transform.position = new Vector3(xMid, 0.025f, z);
                dash.transform.rotation = rot;
                dash.transform.localScale = new Vector3(0.16f, 0.01f, 2.5f);
                dash.GetComponent<Renderer>().sharedMaterial = yellowLineMat;
                Object.DestroyImmediate(dash.GetComponent<Collider>());

                // Vạch kẻ mép đường trắng liền bên trái
                Vector3 leftLinePos = new Vector3(xMid, 0.025f, z) + rot * (Vector3.left * 3.1f);
                GameObject leftLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leftLine.name = $"Road_White_Line_L_{z:0}";
                leftLine.transform.SetParent(exteriorRoot.transform);
                leftLine.transform.position = leftLinePos;
                leftLine.transform.rotation = rot;
                leftLine.transform.localScale = new Vector3(0.12f, 0.01f, 4.15f);
                leftLine.GetComponent<Renderer>().sharedMaterial = whiteLineMat;
                Object.DestroyImmediate(leftLine.GetComponent<Collider>());

                // Vạch kẻ mép đường trắng liền bên phải
                Vector3 rightLinePos = new Vector3(xMid, 0.025f, z) + rot * (Vector3.right * 3.1f);
                GameObject rightLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rightLine.name = $"Road_White_Line_R_{z:0}";
                rightLine.transform.SetParent(exteriorRoot.transform);
                rightLine.transform.position = rightLinePos;
                rightLine.transform.rotation = rot;
                rightLine.transform.localScale = new Vector3(0.12f, 0.01f, 4.15f);
                rightLine.GetComponent<Renderer>().sharedMaterial = whiteLineMat;
                Object.DestroyImmediate(rightLine.GetComponent<Collider>());

                // Hộ lan thép uốn cong mép trái
                Vector3 leftRailPos = new Vector3(xMid, 0.55f, z) + rot * (Vector3.left * 3.75f);
                GameObject leftRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leftRail.name = $"Guardrail_Beam_L_{z:0}";
                leftRail.transform.SetParent(exteriorRoot.transform);
                leftRail.transform.position = leftRailPos;
                leftRail.transform.rotation = rot;
                leftRail.transform.localScale = new Vector3(0.12f, 0.35f, 4.15f);
                leftRail.GetComponent<Renderer>().sharedMaterial = guardrailMat;

                GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                postL.name = $"Guardrail_Post_L_{z:0}";
                postL.transform.SetParent(exteriorRoot.transform);
                postL.transform.position = new Vector3(xMid, 0.3f, z) + rot * (Vector3.left * 3.75f);
                postL.transform.rotation = rot;
                postL.transform.localScale = new Vector3(0.08f, 0.6f, 0.08f);
                postL.GetComponent<Renderer>().sharedMaterial = guardrailMat;

                // Hộ lan thép uốn cong mép phải (chừa khoảng Z = -128m -> -122m cho hiện trường xe cảnh sát lật)
                if (z < -128f || z > -122f)
                {
                    Vector3 rightRailPos = new Vector3(xMid, 0.55f, z) + rot * (Vector3.right * 3.75f);
                    GameObject rightRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    rightRail.name = $"Guardrail_Beam_R_{z:0}";
                    rightRail.transform.SetParent(exteriorRoot.transform);
                    rightRail.transform.position = rightRailPos;
                    rightRail.transform.rotation = rot;
                    rightRail.transform.localScale = new Vector3(0.12f, 0.35f, 4.15f);
                    rightRail.GetComponent<Renderer>().sharedMaterial = guardrailMat;

                    GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    postR.name = $"Guardrail_Post_R_{z:0}";
                    postR.transform.SetParent(exteriorRoot.transform);
                    postR.transform.position = new Vector3(xMid, 0.3f, z) + rot * (Vector3.right * 3.75f);
                    postR.transform.rotation = rot;
                    postR.transform.localScale = new Vector3(0.08f, 0.6f, 0.08f);
                    postR.GetComponent<Renderer>().sharedMaterial = guardrailMat;
                }
            }

            // 3. Vỉa hè trước cửa tiệm
            GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sidewalk.name = "Storefront_Sidewalk";
            sidewalk.transform.SetParent(exteriorRoot.transform);
            sidewalk.transform.position = new Vector3(0f, 0.06f, -6.3f);
            sidewalk.transform.localScale = new Vector3(16f, 0.14f, 2.6f);
            sidewalk.GetComponent<Renderer>().sharedMaterial = curbMat;

            // 4. Đoạn hộ lan bị đâm móp méo tại khúc cua tai nạn xe cảnh sát (Z = -125.0m)
            float crashX = RoadCurveUtility.GetRoadCenterX(-125.0f);
            GameObject crumpledRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crumpledRail.name = "Guardrail_Crumpled_CrashSegment";
            crumpledRail.transform.SetParent(exteriorRoot.transform);
            crumpledRail.transform.position = new Vector3(crashX + 3.95f, 0.25f, -125.0f);
            crumpledRail.transform.rotation = Quaternion.Euler(15f, 0f, -25f);
            crumpledRail.transform.localScale = new Vector3(0.12f, 0.35f, 7.5f);
            crumpledRail.GetComponent<Renderer>().sharedMaterial = guardrailMat;

            // 5. Rừng cây uốn lượn bao bọc hai bên con đường 270m (160+ Cây thông & Cây khô cằn cỗi)
            for (float z = -282f; z <= -8f; z += 3.6f)
            {
                float roadX = RoadCurveUtility.GetRoadCenterX(z);

                // Dãy thông bên trái (Hàng trong & Hàng ngoài)
                float x1L = roadX - 5.8f - Mathf.Abs(Mathf.Sin(z * 0.35f)) * 2.5f;
                float scale1 = 0.9f + Mathf.Abs(Mathf.Cos(z * 0.7f)) * 0.4f;
                CreatePineTree(exteriorRoot.transform, new Vector3(x1L, 0f, z), scale1, treeTrunkMat, pineFoliageMat);

                float x2L = roadX - 10.8f - Mathf.Abs(Mathf.Cos(z * 0.45f)) * 3.5f;
                float scale2 = 1.1f + Mathf.Abs(Mathf.Sin(z * 0.8f)) * 0.4f;
                CreatePineTree(exteriorRoot.transform, new Vector3(x2L, 0f, z + 1.8f), scale2, treeTrunkMat, pineFoliageMat);

                // Dãy thông bên phải (Nhường chỗ tại đoạn xe cảnh sát lật Z = -128m -> -122m)
                if (z < -128f || z > -122f)
                {
                    float x1R = roadX + 5.8f + Mathf.Abs(Mathf.Cos(z * 0.35f)) * 2.5f;
                    CreatePineTree(exteriorRoot.transform, new Vector3(x1R, 0f, z), scale1, treeTrunkMat, pineFoliageMat);
                }

                float x2R = roadX + 10.8f + Mathf.Abs(Mathf.Sin(z * 0.45f)) * 3.5f;
                CreatePineTree(exteriorRoot.transform, new Vector3(x2R, 0f, z + 1.8f), scale2, treeTrunkMat, pineFoliageMat);
            }

            // Cây khô khẳng khiu chìa cành ra đường tại các góc cua
            float[] deadTreeZsL = new float[] { -265f, -225f, -185f, -145f, -105f, -65f, -35f, -15f };
            foreach (float z in deadTreeZsL)
            {
                float rX = RoadCurveUtility.GetRoadCenterX(z);
                CreateDeadTree(exteriorRoot.transform, new Vector3(rX - 4.8f, 0f, z), 1.0f, deadWoodMat, 12f);
            }

            float[] deadTreeZsR = new float[] { -245f, -205f, -165f, -115f, -85f, -50f, -20f };
            foreach (float z in deadTreeZsR)
            {
                float rX = RoadCurveUtility.GetRoadCenterX(z);
                CreateDeadTree(exteriorRoot.transform, new Vector3(rX + 4.8f, 0f, z), 1.0f, deadWoodMat, -12f);
            }

            // Tường cây tối mịt hai bên để chặn tầm nhìn chân trời (Tạo cảm giác rừng sâu ngột ngạt)
            for (float z = -284f; z <= 0f; z += 6f)
            {
                float rX = RoadCurveUtility.GetRoadCenterX(z);
                GameObject wallL = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                wallL.name = $"Forest_Wall_L_{z:0}";
                wallL.transform.SetParent(exteriorRoot.transform);
                wallL.transform.position = new Vector3(rX - 19.0f, 4.5f, z);
                wallL.transform.localScale = new Vector3(4.5f, 5.0f, 6.0f);
                wallL.GetComponent<Renderer>().sharedMaterial = pineFoliageMat;
                Object.DestroyImmediate(wallL.GetComponent<Collider>());

                GameObject wallR = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                wallR.name = $"Forest_Wall_R_{z:0}";
                wallR.transform.SetParent(exteriorRoot.transform);
                wallR.transform.position = new Vector3(rX + 19.0f, 4.5f, z);
                wallR.transform.localScale = new Vector3(4.5f, 5.0f, 6.0f);
                wallR.GetComponent<Renderer>().sharedMaterial = pineFoliageMat;
                Object.DestroyImmediate(wallR.GetComponent<Collider>());
            }

            // 6. Hệ thống 14 Cột đèn đường chiếu sáng bố trí đều dọc cung đường 270m
            // Thiết kế dạng Cần Vươn (Overhead Cantilever) - Đèn chiếu rọi THẲNG ĐỨNG XUỐNG MẶT ĐƯỜNG (Downlight Spot)
            // TUYỆT ĐỐI KHÔNG DÙNG ĐÈN SÁNG TỎA (Point Light) để tránh làm sáng bệt không gian xung quanh
            float[] lampZs = new float[] { -265f, -245f, -225f, -205f, -185f, -165f, -145f, -125f, -105f, -85f, -65f, -48f, -32f, -18f };
            Material bulbMat = CreateColoredMaterial("Mat_StreetLampBulbGlow", new Color(1.0f, 0.92f, 0.65f), 0.2f, new Color(1.0f, 0.88f, 0.55f) * 4f);

            for (int i = 0; i < lampZs.Length; i++)
            {
                float lz = lampZs[i];
                float lx = RoadCurveUtility.GetRoadCenterX(lz);
                float side = (i % 2 == 0) ? -1f : 1f;

                // Root không tỉ lệ (Scale 1,1,1) dựng tại vị trí lề đường để các phần tử con giữ đúng kích thước vật lý
                GameObject lampRoot = new GameObject($"Street_Lamp_{i + 1}_Z{lz:0}");
                lampRoot.transform.SetParent(exteriorRoot.transform);
                lampRoot.transform.position = new Vector3(lx + side * 4.2f, 0f, lz);

                // Thân cột đèn kim loại cao 5.6m dựng sát lề cỏ
                GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pole.name = $"Lamp_Pole_{i + 1}";
                pole.transform.SetParent(lampRoot.transform);
                pole.transform.localPosition = new Vector3(0f, 2.8f, 0f);
                pole.transform.localScale = new Vector3(0.18f, 2.8f, 0.18f);
                pole.GetComponent<Renderer>().sharedMaterial = poleMat;

                // Cần đèn vươn ngang 2.6m ra phía trên làn đường di chuyển (vươn vào phía tâm đường)
                GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
                arm.name = $"Lamp_Arm_{i + 1}";
                arm.transform.SetParent(lampRoot.transform);
                arm.transform.localPosition = new Vector3(-side * 1.3f, 5.4f, 0f);
                arm.transform.localScale = new Vector3(2.6f, 0.12f, 0.12f);
                arm.GetComponent<Renderer>().sharedMaterial = poleMat;
                Object.DestroyImmediate(arm.GetComponent<Collider>());

                // Chao đèn rọi (Lamp Fixture) treo ở đầu cần đèn, ngay phía trên làn xe chạy
                GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
                head.name = $"Lamp_Head_{i + 1}";
                head.transform.SetParent(lampRoot.transform);
                head.transform.localPosition = new Vector3(-side * 2.5f, 5.3f, 0f);
                head.transform.localScale = new Vector3(0.75f, 0.24f, 0.95f);
                head.GetComponent<Renderer>().sharedMaterial = poleMat;
                Object.DestroyImmediate(head.GetComponent<Collider>());

                // Mặt kính bóng đèn phát sáng vàng hướng xuống đáy chao đèn
                GameObject bulb = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bulb.name = $"Lamp_Bulb_{i + 1}";
                bulb.transform.SetParent(head.transform);
                bulb.transform.localPosition = new Vector3(0f, -0.12f, 0f);
                bulb.transform.localScale = new Vector3(0.85f, 0.05f, 0.85f);
                bulb.GetComponent<Renderer>().sharedMaterial = bulbMat;
                Object.DestroyImmediate(bulb.GetComponent<Collider>());

                // Nguồn sáng Spot Light CHIẾU THẲNG ĐỨNG XUỐNG LÒNG ĐƯỜNG (Rotation = 90, 0, 0)
                // Treo ở độ cao 5.15m trực tiếp phía trên làn xe chạy (cách tâm đường 1.7m), tạo thành vệt sáng tròn vàng rực rỡ, rõ nét trên mặt đường nhựa
                GameObject spotObj = new GameObject($"Lamp_Downlight_Spot_{i + 1}");
                spotObj.transform.SetParent(lampRoot.transform);
                spotObj.transform.localPosition = new Vector3(-side * 2.5f, 5.15f, 0f);
                spotObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Chiếu vuông góc thẳng xuống mặt đất

                Light lampLight = spotObj.AddComponent<Light>();
                lampLight.type = LightType.Spot;
                lampLight.range = 12f;
                lampLight.spotAngle = 42f;
                lampLight.innerSpotAngle = 20f;
                lampLight.intensity = 600f; // Độ sáng tập trung vừa phải, tạo không gian đêm u ám rùng rợn
                lampLight.color = new Color(1.0f, 0.88f, 0.58f);
                lampLight.shadows = LightShadows.Soft;

                // Một vài ngọn đèn chập chờn tạo cảm giác rùng rợn cô độc
                if (i == 2 || i == 7 || i == 11)
                {
                    spotObj.AddComponent<StreetLampFlicker>();
                }
            }

            // 7. Điểm mốc & Yếu tố kinh dị dọa người chơi dọc cung đường 270m (Horror Landmarks):
            // Mốc 1 (Z = -255m): Biển gỗ cảnh báo khu rừng cấm
            float sign1X = RoadCurveUtility.GetRoadCenterX(-255.0f);
            GameObject sign1Obj = new GameObject("Landmark_Evacuated_Warning_Sign");
            sign1Obj.transform.SetParent(exteriorRoot.transform);
            sign1Obj.transform.position = new Vector3(sign1X - 4.6f, 0f, -255.0f);

            GameObject s1Pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            s1Pole.transform.SetParent(sign1Obj.transform);
            s1Pole.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            s1Pole.transform.localScale = new Vector3(0.1f, 1.2f, 0.1f);
            s1Pole.GetComponent<Renderer>().sharedMaterial = treeTrunkMat;

            GameObject s1Plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s1Plate.name = "Warning_Plate";
            s1Plate.transform.SetParent(sign1Obj.transform);
            s1Plate.transform.localPosition = new Vector3(0f, 1.8f, 0f);
            s1Plate.transform.localScale = new Vector3(0.08f, 1.0f, 1.8f);
            s1Plate.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_DangerWarningWood", new Color(0.25f, 0.18f, 0.12f), 0.2f);

            // Vết cào máu trên biển
            GameObject bloodStripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bloodStripe.transform.SetParent(s1Plate.transform);
            bloodStripe.transform.localPosition = new Vector3(0.6f, 0f, 0f);
            bloodStripe.transform.localScale = new Vector3(0.1f, 0.6f, 1.2f);
            bloodStripe.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_BloodStripe", new Color(0.65f, 0.04f, 0.04f), 0.3f);
            Object.DestroyImmediate(bloodStripe.GetComponent<Collider>());

            // Mốc 2 (Z = -210m): Biển báo tam giác vàng "CẨN THẬN: QUÁI VẬT BĂNG ĐƯỜNG"
            float sign2X = RoadCurveUtility.GetRoadCenterX(-210.0f);
            GameObject sign2Obj = new GameObject("Landmark_Monster_Caution_Sign");
            sign2Obj.transform.SetParent(exteriorRoot.transform);
            sign2Obj.transform.position = new Vector3(sign2X + 4.6f, 0f, -210.0f);

            GameObject s2Pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            s2Pole.transform.SetParent(sign2Obj.transform);
            s2Pole.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            s2Pole.transform.localScale = new Vector3(0.08f, 1.2f, 0.08f);
            s2Pole.GetComponent<Renderer>().sharedMaterial = poleMat;

            GameObject s2Plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s2Plate.transform.SetParent(sign2Obj.transform);
            s2Plate.transform.localPosition = new Vector3(0f, 1.8f, 0f);
            s2Plate.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            s2Plate.transform.localScale = new Vector3(0.06f, 0.85f, 0.85f);
            s2Plate.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_CautionYellow", new Color(0.85f, 0.70f, 0.10f), 0.3f);

            // Mốc 3 (Z = -165m): Bù nhìn cành cây ma quái (Totem Effigy) đứng lấp ló trong rừng cây với mắt đỏ le lói
            float totemX = RoadCurveUtility.GetRoadCenterX(-165.0f);
            GameObject totemObj = new GameObject("Landmark_Spooky_Forest_Totem");
            totemObj.transform.SetParent(exteriorRoot.transform);
            totemObj.transform.position = new Vector3(totemX - 6.5f, 0f, -165.0f);

            GameObject totemBody = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            totemBody.transform.SetParent(totemObj.transform);
            totemBody.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            totemBody.transform.localScale = new Vector3(0.2f, 1.2f, 0.2f);
            totemBody.GetComponent<Renderer>().sharedMaterial = deadWoodMat;

            GameObject totemArms = GameObject.CreatePrimitive(PrimitiveType.Cube);
            totemArms.transform.SetParent(totemObj.transform);
            totemArms.transform.localPosition = new Vector3(0f, 1.7f, 0f);
            totemArms.transform.localScale = new Vector3(0.12f, 0.12f, 1.8f);
            totemArms.GetComponent<Renderer>().sharedMaterial = deadWoodMat;

            GameObject totemHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            totemHead.transform.SetParent(totemObj.transform);
            totemHead.transform.localPosition = new Vector3(0f, 2.3f, 0f);
            totemHead.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            totemHead.GetComponent<Renderer>().sharedMaterial = deadWoodMat;

            GameObject eyeLightObj = new GameObject("Totem_Red_Eyes");
            eyeLightObj.transform.SetParent(totemHead.transform);
            eyeLightObj.transform.localPosition = new Vector3(0.25f, 0f, 0f);
            Light eyeLight = eyeLightObj.AddComponent<Light>();
            eyeLight.type = LightType.Point;
            eyeLight.color = new Color(1.0f, 0.1f, 0.1f);
            eyeLight.range = 3.5f;
            eyeLight.intensity = 25f;

            // Mốc 4 (Z = -125m): Xe cảnh sát bỏ hoang cắm đầu xuống mương tại khúc cua gắt
            GameObject carRoot = new GameObject("Landmark_Abandoned_Patrol_Car");
            carRoot.transform.SetParent(exteriorRoot.transform);
            carRoot.transform.position = new Vector3(crashX + 5.2f, 0.45f, -125.0f);
            carRoot.transform.rotation = Quaternion.Euler(14f, 22f, -14f);

            GameObject carBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carBody.name = "Patrol_Car_Chassis";
            carBody.transform.SetParent(carRoot.transform);
            carBody.transform.localPosition = Vector3.zero;
            carBody.transform.localScale = new Vector3(1.8f, 0.65f, 4.2f);
            carBody.GetComponent<Renderer>().sharedMaterial = patrolCarMat;

            GameObject carCabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carCabin.name = "Patrol_Car_Cabin";
            carCabin.transform.SetParent(carRoot.transform);
            carCabin.transform.localPosition = new Vector3(0f, 0.55f, -0.2f);
            carCabin.transform.localScale = new Vector3(1.4f, 0.52f, 2.0f);
            carCabin.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_CarWhiteCabin", new Color(0.85f, 0.85f, 0.85f), 0.5f);

            GameObject lightBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lightBar.name = "Emergency_Lightbar";
            lightBar.transform.SetParent(carCabin.transform);
            lightBar.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            lightBar.transform.localScale = new Vector3(0.85f, 0.2f, 0.25f);
            lightBar.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_HazardAmber", new Color(1.0f, 0.55f, 0.1f), 0.5f, new Color(1.0f, 0.6f, 0.1f) * 2f);

            Light hazardLight = lightBar.AddComponent<Light>();
            hazardLight.type = LightType.Point;
            hazardLight.range = 8f;
            hazardLight.color = new Color(1.0f, 0.6f, 0.1f);
            hazardLight.intensity = 100f;
            lightBar.AddComponent<HazardLightPulse>();

            // Mốc 5 (Z = -80m): Tấm biển chào Hạt Blackwood bị sơn máu gạch chéo
            float sign5X = RoadCurveUtility.GetRoadCenterX(-80.0f);
            GameObject signBoardRoot = new GameObject("Landmark_Welcome_Blackwood_Sign");
            signBoardRoot.transform.SetParent(exteriorRoot.transform);
            signBoardRoot.transform.position = new Vector3(sign5X - 4.8f, 0f, -80.0f);

            GameObject signPost1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            signPost1.name = "Sign_Post_1";
            signPost1.transform.SetParent(signBoardRoot.transform);
            signPost1.transform.localPosition = new Vector3(0f, 1.4f, -1.0f);
            signPost1.transform.localScale = new Vector3(0.12f, 1.4f, 0.12f);
            signPost1.GetComponent<Renderer>().sharedMaterial = treeTrunkMat;

            GameObject signPost2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            signPost2.name = "Sign_Post_2";
            signPost2.transform.SetParent(signBoardRoot.transform);
            signPost2.transform.localPosition = new Vector3(0f, 1.4f, 1.0f);
            signPost2.transform.localScale = new Vector3(0.12f, 1.4f, 0.12f);
            signPost2.GetComponent<Renderer>().sharedMaterial = treeTrunkMat;

            GameObject boardPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boardPlate.name = "Welcome_Board_Plate";
            boardPlate.transform.SetParent(signBoardRoot.transform);
            boardPlate.transform.localPosition = new Vector3(0.06f, 1.8f, 0f);
            boardPlate.transform.localScale = new Vector3(0.08f, 1.2f, 2.2f);
            boardPlate.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_WelcomeSign", new Color(0.14f, 0.32f, 0.18f), 0.2f);

            GameObject redGraffiti = GameObject.CreatePrimitive(PrimitiveType.Cube);
            redGraffiti.name = "Red_Graffiti_Stripe";
            redGraffiti.transform.SetParent(boardPlate.transform);
            redGraffiti.transform.localPosition = new Vector3(0.6f, -0.2f, 0.15f);
            redGraffiti.transform.localScale = new Vector3(0.1f, 0.25f, 0.7f);
            redGraffiti.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_BloodGraffiti", new Color(0.65f, 0.05f, 0.05f), 0.3f);
            Object.DestroyImmediate(redGraffiti.GetComponent<Collider>());

            // Mốc 6 (Z = -45m): Biển cảnh báo cua gắt tử thần
            float sign6X = RoadCurveUtility.GetRoadCenterX(-45.0f);
            GameObject cautionSign = new GameObject("Landmark_Caution_Warning_Sign");
            cautionSign.transform.SetParent(exteriorRoot.transform);
            cautionSign.transform.position = new Vector3(sign6X - 4.4f, 0f, -45.0f);

            GameObject cautionPole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cautionPole.transform.SetParent(cautionSign.transform);
            cautionPole.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            cautionPole.transform.localScale = new Vector3(0.08f, 1.2f, 0.08f);
            cautionPole.GetComponent<Renderer>().sharedMaterial = poleMat;

            GameObject diamondPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            diamondPlate.transform.SetParent(cautionSign.transform);
            diamondPlate.transform.localPosition = new Vector3(0.05f, 1.9f, 0f);
            diamondPlate.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            diamondPlate.transform.localScale = new Vector3(0.05f, 0.75f, 0.75f);
            diamondPlate.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_CautionDiamond", new Color(0.9f, 0.75f, 0.1f), 0.3f);

            // Mốc 7 (Mặt tiền siêu thị Z = -5.1m): Biển Neon phát sáng rực rỡ "MIDNIGHT MART - OPEN 24/7"
            GameObject neonSignObj = new GameObject("Storefront_Neon_Sign_Roof");
            neonSignObj.transform.SetParent(exteriorRoot.transform);
            neonSignObj.transform.position = new Vector3(0f, 4.35f, -5.15f);

            GameObject signBacking = GameObject.CreatePrimitive(PrimitiveType.Cube);
            signBacking.name = "Sign_Backing";
            signBacking.transform.SetParent(neonSignObj.transform);
            signBacking.transform.localPosition = Vector3.zero;
            signBacking.transform.localScale = new Vector3(7.2f, 0.95f, 0.12f);
            signBacking.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_SignBacking", new Color(0.12f, 0.12f, 0.14f), 0.5f);

            GameObject neonRed = GameObject.CreatePrimitive(PrimitiveType.Cube);
            neonRed.name = "Neon_MIDNIGHT_MART_Red";
            neonRed.transform.SetParent(neonSignObj.transform);
            neonRed.transform.localPosition = new Vector3(-1.0f, 0.05f, -0.08f);
            neonRed.transform.localScale = new Vector3(4.4f, 0.45f, 0.04f);
            neonRed.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_NeonRed", new Color(1.0f, 0.15f, 0.15f), 0.5f, new Color(1.0f, 0.1f, 0.1f) * 3.5f);
            Object.DestroyImmediate(neonRed.GetComponent<Collider>());

            GameObject neonCyan = GameObject.CreatePrimitive(PrimitiveType.Cube);
            neonCyan.name = "Neon_OPEN_24_7_Cyan";
            neonCyan.transform.SetParent(neonSignObj.transform);
            neonCyan.transform.localPosition = new Vector3(2.2f, -0.05f, -0.08f);
            neonCyan.transform.localScale = new Vector3(1.6f, 0.32f, 0.04f);
            neonCyan.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_NeonCyan", new Color(0.1f, 0.85f, 0.95f), 0.5f, new Color(0.1f, 0.9f, 1.0f) * 3.5f);
            Object.DestroyImmediate(neonCyan.GetComponent<Collider>());

            Light neonGlow = neonSignObj.AddComponent<Light>();
            neonGlow.type = LightType.Point;
            neonGlow.range = 6f;
            neonGlow.intensity = 200f;
            neonGlow.color = new Color(1.0f, 0.3f, 0.4f);

            // 8. Chiếc xe máy đỗ gọn gàng ở vỉa hè trước cửa tiệm (Parked Motorbike World Prop)
            parkedBike = CreateParkedMotorbike(exteriorRoot.transform, new Vector3(-2.2f, 0.08f, -7.5f), Quaternion.Euler(0f, 25f, -6f), poleMat, rubberMat, chromeMat, bikeTankMat);

            // 9. Điểm xuất phát lái xe máy đầu con đường âm u dài gấp đôi (Z = -280m)
            GameObject startPoint = new GameObject("Gloomy_Road_Start_Point");
            startPoint.transform.SetParent(exteriorRoot.transform);
            startPoint.transform.position = new Vector3(0f, 1.0f, -280.0f);
            startPoint.transform.rotation = Quaternion.identity;
            roadStartPoint = startPoint.transform;
        }

        private static GameObject CreateCheckoutCounter(Transform parent, out CashRegister cashRegister, out SecretDrawer secretDrawer)
        {
            Material woodMat = CreateColoredMaterial("Mat_CounterWood", new Color(0.32f, 0.20f, 0.14f), 0.35f);
            Material metalMat = CreateColoredMaterial("Mat_CashRegisterMetal", new Color(0.18f, 0.20f, 0.23f), 0.7f);

            // Bàn quầy thu ngân đặt ở góc dọc gần cửa mặt tiền (X = -2.4m, Z = -3.2m, chạy dọc theo trục Z)
            GameObject counterObj = new GameObject("Checkout_Counter_Station");
            counterObj.transform.SetParent(parent);
            counterObj.transform.position = new Vector3(-2.4f, 0.5f, -3.2f);

            // Mặt bàn quầy chính dáng dọc (Rộng 0.95m, Cao 1.0m, Dài 2.7m từ Z = -4.55m đến Z = -1.85m)
            // Quầy chính dành riêng cho tính tiền, máy POS, súng quét mã vạch và đặt đồ của khách - KHÔNG để máy tính làm chật chội
            GameObject counterTable = GameObject.CreatePrimitive(PrimitiveType.Cube);
            counterTable.name = "Counter_Desk";
            counterTable.transform.SetParent(counterObj.transform);
            counterTable.transform.localPosition = Vector3.zero;
            counterTable.transform.localScale = new Vector3(0.95f, 1.0f, 2.7f);
            counterTable.GetComponent<Renderer>().sharedMaterial = woodMat;

            // Máy tính tiền cổ điển (Bố trí ở đầu bàn quầy phía gần cửa Z = -0.75m, quay mặt về phía thu ngân -X)
            GameObject registerObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            registerObj.name = "Cash_Register_Machine";
            registerObj.transform.SetParent(counterObj.transform);
            registerObj.transform.localPosition = new Vector3(0f, 0.7f, -0.75f);
            registerObj.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            registerObj.transform.localScale = new Vector3(0.55f, 0.4f, 0.45f);
            registerObj.GetComponent<Renderer>().sharedMaterial = metalMat;
            cashRegister = registerObj.AddComponent<CashRegister>();

            // Màn hình hiển thị giá tiền (quay mặt về phía nhân viên)
            GameObject screenObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screenObj.name = "Register_Screen";
            screenObj.transform.SetParent(registerObj.transform);
            screenObj.transform.localPosition = new Vector3(0f, 0.7f, -0.1f);
            screenObj.transform.localScale = new Vector3(0.6f, 0.35f, 0.1f);

            GameObject screenCanvas = new GameObject("Screen_Canvas");
            screenCanvas.transform.SetParent(screenObj.transform);
            screenCanvas.transform.localPosition = new Vector3(0f, 0f, -0.6f);
            screenCanvas.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            Canvas sc = screenCanvas.AddComponent<Canvas>();
            sc.renderMode = RenderMode.WorldSpace;
            RectTransform scRect = screenCanvas.GetComponent<RectTransform>();
            scRect.sizeDelta = new Vector2(200f, 80f);
            scRect.localScale = Vector3.one * 0.005f;

            TextMeshProUGUI displayTMP = screenCanvas.AddComponent<TextMeshProUGUI>();
            displayTMP.alignment = TextAlignmentOptions.Center;
            displayTMP.fontSize = 32f;
            displayTMP.color = Color.green;
            displayTMP.text = "$0.00";

            // Khay tiền nhỏ bên trong máy tính tiền
            GameObject tillDrawer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tillDrawer.name = "Cash_Till_Drawer";
            tillDrawer.transform.SetParent(registerObj.transform);
            tillDrawer.transform.localPosition = new Vector3(0f, -0.25f, 0.05f);
            tillDrawer.transform.localScale = new Vector3(0.92f, 0.22f, 0.88f);
            tillDrawer.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_TillDrawer", new Color(0.12f, 0.14f, 0.16f), 0.8f);
            Object.DestroyImmediate(tillDrawer.GetComponent<Collider>());

            // Gán an toàn tham chiếu máy tính tiền & âm thanh keng vui nhộn
            SerializedObject regSO = new SerializedObject(cashRegister);
            SafeSetObject(regSO, "_screenDisplayTMP", displayTMP);
            SafeSetObject(regSO, "_drawerTransform", tillDrawer.transform);
            SafeSetVector3(regSO, "_drawerOpenOffset", new Vector3(0f, 0f, 0.45f));
            SafeSetObject(regSO, "_drawerOpenSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Cash_Register_Ding.wav"));
            regSO.ApplyModifiedProperties();

            BoxCollider regCol = registerObj.GetComponent<BoxCollider>();
            if (regCol != null) regCol.size = new Vector3(0.7f, 0.6f, 0.6f);

            // Máy quẹt thẻ POS hiện đại bên cạnh máy tính tiền (Z = -0.35m)
            GameObject posCardObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            posCardObj.name = "POS_Card_Swipe_Terminal";
            posCardObj.transform.SetParent(counterObj.transform);
            posCardObj.transform.localPosition = new Vector3(0.12f, 0.55f, -0.35f);
            posCardObj.transform.localRotation = Quaternion.Euler(15f, -75f, 0f);
            posCardObj.transform.localScale = new Vector3(0.18f, 0.08f, 0.24f);
            posCardObj.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_POSTerminal", new Color(0.15f, 0.16f, 0.18f), 0.7f);

            // Đế đỡ máy quét mã vạch và Súng quét trên quầy thu ngân (Scanner_Cradle.obj & Barcode_Scanner.obj)
            // Giữ đúng vị trí hợp lý và ưu tiên bảo tồn tọa độ người chơi đã căn chỉnh thủ công
            GameObject scannerStand = new GameObject("Scanner_Cradle_Stand");
            scannerStand.transform.SetParent(counterObj.transform);
            scannerStand.transform.localPosition = new Vector3(-0.35f, 0.52f, -0.15f);
            scannerStand.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            scannerStand.transform.localScale = Vector3.one;

            GameObject cradlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Scanner_Cradle.obj");
            if (cradlePrefab != null)
            {
                GameObject cradleVisual = Object.Instantiate(cradlePrefab, scannerStand.transform);
                cradleVisual.name = "Cradle_Visual";
                cradleVisual.transform.localPosition = Vector3.zero;
                cradleVisual.transform.localRotation = Quaternion.identity;
                cradleVisual.transform.localScale = Vector3.one;
                RemoveCollidersRecursively(cradleVisual);
            }
            else
            {
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                placeholder.transform.SetParent(scannerStand.transform);
                placeholder.transform.localPosition = Vector3.zero;
                placeholder.transform.localScale = new Vector3(0.22f, 0.04f, 0.22f);
                placeholder.GetComponent<Renderer>().sharedMaterial = metalMat;
                Object.DestroyImmediate(placeholder.GetComponent<Collider>());
            }

            BoxCollider standCol = scannerStand.AddComponent<BoxCollider>();
            standCol.center = new Vector3(0f, 0.10f, 0f);
            standCol.size = new Vector3(0.18f, 0.22f, 0.18f); // Kích thước đế gọn gàng, tránh bắt nhầm tương tác

            // Súng quét mã vạch nằm trên giá đỡ (Barcode_Scanner_Gun_Visual)
            GameObject gunPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Barcode_Scanner.obj");
            GameObject scannerGunVisual;
            if (gunPrefab != null)
            {
                scannerGunVisual = Object.Instantiate(gunPrefab, scannerStand.transform);
                scannerGunVisual.name = "Barcode_Scanner_Gun_Visual";
                scannerGunVisual.transform.localPosition = new Vector3(0f, 0.05f, 0.02f);
                scannerGunVisual.transform.localRotation = Quaternion.Euler(15f, 180f, 0f);
                scannerGunVisual.transform.localScale = Vector3.one;
                RemoveCollidersRecursively(scannerGunVisual);
            }
            else
            {
                scannerGunVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                scannerGunVisual.name = "Barcode_Scanner_Gun_Visual";
                scannerGunVisual.transform.SetParent(scannerStand.transform);
                scannerGunVisual.transform.localPosition = new Vector3(0f, 0.12f, 0f);
                scannerGunVisual.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                scannerGunVisual.transform.localScale = new Vector3(0.08f, 0.16f, 0.12f);
                scannerGunVisual.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_ScannerPlastic", new Color(0.12f, 0.13f, 0.15f), 0.7f);
                Object.DestroyImmediate(scannerGunVisual.GetComponent<Collider>());
            }

            BarcodeScanner dockScanner = scannerStand.AddComponent<BarcodeScanner>();
            SerializedObject dockSO = new SerializedObject(dockScanner);
            SafeSetBool(dockSO, "_isFPSTool", false);
            SafeSetObject(dockSO, "_dockGunModel", scannerGunVisual);
            SafeSetObject(dockSO, "_beepSuccessSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Barcode_Scan_Beep.wav"));
            dockSO.ApplyModifiedProperties();

            // Ngăn kéo bí mật dưới quầy chính (quay mặt về phía thu ngân -X, kéo thẳng ra theo trục -X)
            GameObject drawerObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            drawerObj.name = "Secret_Under_Drawer";
            drawerObj.transform.SetParent(counterObj.transform);
            drawerObj.transform.localPosition = new Vector3(-0.48f, 0.35f, 0f);
            drawerObj.transform.localRotation = Quaternion.identity;
            drawerObj.transform.localScale = new Vector3(0.42f, 0.26f, 0.65f);
            drawerObj.GetComponent<Renderer>().sharedMaterial = woodMat;
            BoxCollider drawerCol = drawerObj.GetComponent<BoxCollider>();
            if (drawerCol != null)
            {
                drawerCol.center = new Vector3(0f, 0f, 0f);
                drawerCol.size = new Vector3(0.55f, 0.35f, 0.95f);
            }
            secretDrawer = drawerObj.AddComponent<SecretDrawer>();

            // BÀN CÁNH CHỮ L CHO MÁY TÍNH (Nối vuông góc với quầy tại X = -3.4m, Z = -4.1m tạo hình chữ L tiện lợi)
            // Bố trí khoa học, tinh tế: Màn hình, bàn phím, chuột ngay bên tay phải nhân viên thu ngân
            GameObject backDeskObj = new GameObject("Back_Staff_Desk");
            backDeskObj.transform.SetParent(parent);
            backDeskObj.transform.position = new Vector3(-3.4f, 0f, -4.1f);

            // Mặt bàn gỗ làm việc (Tabletop cao 0.74m tiêu chuẩn văn phòng, trải dài 2.0m theo trục X, rộng 0.85m theo trục Z)
            GameObject backDeskTable = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backDeskTable.name = "Back_Desk_Table";
            backDeskTable.transform.SetParent(backDeskObj.transform);
            backDeskTable.transform.localPosition = new Vector3(0f, 0.74f, 0f);
            backDeskTable.transform.localScale = new Vector3(2.0f, 0.04f, 0.85f);
            backDeskTable.GetComponent<Renderer>().sharedMaterial = woodMat;

            // 4 Chân bàn kim loại vững chắc
            Material deskLegMat = CreateColoredMaterial("Mat_DeskLegMetal", new Color(0.18f, 0.20f, 0.22f), 0.7f);
            Vector3[] legOffsets = new Vector3[]
            {
                new Vector3(-0.94f, 0.36f, -0.38f),
                new Vector3(0.94f, 0.36f, -0.38f),
                new Vector3(-0.94f, 0.36f, 0.38f),
                new Vector3(0.94f, 0.36f, 0.38f)
            };
            for (int i = 0; i < legOffsets.Length; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = $"Desk_Leg_{i + 1}";
                leg.transform.SetParent(backDeskObj.transform);
                leg.transform.localPosition = legOffsets[i];
                leg.transform.localScale = new Vector3(0.04f, 0.72f, 0.04f);
                leg.GetComponent<Renderer>().sharedMaterial = deskLegMat;
            }

            // Hộc tủ hồ sơ dưới gầm bàn bên trái
            GameObject filingCabinet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            filingCabinet.name = "Filing_Cabinet_Left";
            filingCabinet.transform.SetParent(backDeskObj.transform);
            filingCabinet.transform.localPosition = new Vector3(-0.65f, 0.36f, 0f);
            filingCabinet.transform.localScale = new Vector3(0.42f, 0.68f, 0.48f);
            filingCabinet.GetComponent<Renderer>().sharedMaterial = woodMat;

            // Thùng case CPU máy tính đặt gọn gàng trên sàn dưới gầm bàn bên phải
            GameObject pcTower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pcTower.name = "PC_Tower_Case";
            pcTower.transform.SetParent(backDeskObj.transform);
            pcTower.transform.localPosition = new Vector3(0.65f, 0.22f, 0f);
            pcTower.transform.localScale = new Vector3(0.22f, 0.42f, 0.45f);
            pcTower.GetComponent<Renderer>().sharedMaterial = metalMat;

            // Đèn bàn làm việc văn phòng đặt ở góc sau bàn bên trái (không vướng màn hình hay bàn phím)
            GameObject deskLamp = new GameObject("Back_Desk_Lamp");
            deskLamp.transform.SetParent(backDeskObj.transform);
            deskLamp.transform.localPosition = new Vector3(-0.80f, 0.76f, 0.25f);
            deskLamp.transform.localRotation = Quaternion.identity;

            GameObject lampBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lampBase.name = "Lamp_Base";
            lampBase.transform.SetParent(deskLamp.transform);
            lampBase.transform.localPosition = new Vector3(0f, 0.012f, 0f);
            lampBase.transform.localScale = new Vector3(0.14f, 0.012f, 0.14f);
            lampBase.GetComponent<Renderer>().sharedMaterial = metalMat;
            Object.DestroyImmediate(lampBase.GetComponent<Collider>());

            GameObject lampArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lampArm.name = "Lamp_Arm";
            lampArm.transform.SetParent(deskLamp.transform);
            lampArm.transform.localPosition = new Vector3(0.04f, 0.18f, -0.04f);
            lampArm.transform.localRotation = Quaternion.Euler(15f, 0f, -12f);
            lampArm.transform.localScale = new Vector3(0.02f, 0.16f, 0.02f);
            lampArm.GetComponent<Renderer>().sharedMaterial = metalMat;
            Object.DestroyImmediate(lampArm.GetComponent<Collider>());

            GameObject lampHead = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lampHead.name = "Lamp_Head_Shade";
            lampHead.transform.SetParent(deskLamp.transform);
            lampHead.transform.localPosition = new Vector3(0.08f, 0.32f, -0.08f);
            lampHead.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
            lampHead.transform.localScale = new Vector3(0.09f, 0.06f, 0.09f);
            lampHead.GetComponent<Renderer>().sharedMaterial = metalMat;
            Object.DestroyImmediate(lampHead.GetComponent<Collider>());

            Light lampL = deskLamp.AddComponent<Light>();
            lampL.type = LightType.Point;
            lampL.range = 2.8f;
            lampL.intensity = 35f;
            lampL.color = new Color(1.0f, 0.88f, 0.72f);

            // Máy tính quầy thu ngân (Store Desktop Computer Terminal) đặt trang trọng trên Bàn cánh chữ L
            // Xoay 180 độ theo trục Y để màn hình và bàn phím quay trực diện vào mắt người chơi
            GameObject pcTerminal = new GameObject("Store_Desktop_Computer");
            pcTerminal.transform.SetParent(backDeskObj.transform);
            pcTerminal.transform.localPosition = new Vector3(0f, 0.76f, 0f);
            pcTerminal.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            // Chân đế & Trụ màn hình
            GameObject pcBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pcBase.name = "Monitor_Base";
            pcBase.transform.SetParent(pcTerminal.transform);
            pcBase.transform.localPosition = new Vector3(0f, 0.01f, 0.12f);
            pcBase.transform.localRotation = Quaternion.identity;
            pcBase.transform.localScale = new Vector3(0.24f, 0.02f, 0.20f);
            pcBase.GetComponent<Renderer>().sharedMaterial = metalMat;
            Object.DestroyImmediate(pcBase.GetComponent<Collider>());

            GameObject pcNeck = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pcNeck.name = "Monitor_Neck";
            pcNeck.transform.SetParent(pcTerminal.transform);
            pcNeck.transform.localPosition = new Vector3(0f, 0.14f, 0.12f);
            pcNeck.transform.localRotation = Quaternion.identity;
            pcNeck.transform.localScale = new Vector3(0.04f, 0.14f, 0.04f);
            pcNeck.GetComponent<Renderer>().sharedMaterial = metalMat;
            Object.DestroyImmediate(pcNeck.GetComponent<Collider>());

            // Màn hình máy tính
            GameObject pcScreen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pcScreen.name = "Monitor_Screen";
            pcScreen.transform.SetParent(pcTerminal.transform);
            pcScreen.transform.localPosition = new Vector3(0f, 0.34f, 0.12f);
            pcScreen.transform.localRotation = Quaternion.identity;
            pcScreen.transform.localScale = new Vector3(0.68f, 0.42f, 0.05f);
            pcScreen.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_PCMonitorFrame", new Color(0.12f, 0.14f, 0.16f), 0.6f);

            // Mặt hiển thị màn hình phát sáng (Emissive Screen) quay mặt về phía nhân viên (-Z)
            GameObject pcFace = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pcFace.name = "Monitor_Face_Display";
            pcFace.transform.SetParent(pcScreen.transform);
            pcFace.transform.localPosition = new Vector3(0f, 0f, -0.52f);
            pcFace.transform.localRotation = Quaternion.identity;
            pcFace.transform.localScale = new Vector3(0.94f, 0.90f, 0.1f);
            Material pcGlowMat = CreateColoredMaterial("Mat_ComputerScreenGlow", new Color(0.05f, 0.2f, 0.25f), 0.8f, new Color(0.1f, 0.7f, 0.8f) * 1.8f);
            pcFace.GetComponent<Renderer>().sharedMaterial = pcGlowMat;
            Object.DestroyImmediate(pcFace.GetComponent<Collider>());

            // Bàn phím máy tính đặt ngay ngắn phía trước màn hình (Z = -0.16m)
            GameObject pcKeyboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pcKeyboard.name = "PC_Keyboard";
            pcKeyboard.transform.SetParent(pcTerminal.transform);
            pcKeyboard.transform.localPosition = new Vector3(0f, 0.015f, -0.16f);
            pcKeyboard.transform.localRotation = Quaternion.identity;
            pcKeyboard.transform.localScale = new Vector3(0.48f, 0.025f, 0.16f);
            pcKeyboard.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_PCKeyboard", new Color(0.15f, 0.16f, 0.18f), 0.4f);
            Object.DestroyImmediate(pcKeyboard.GetComponent<Collider>());

            // Lót chuột & Chuột quang đặt ngay ngắn bên phải bàn phím (+X = 0.34m)
            GameObject pcMousepad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pcMousepad.name = "PC_Mousepad";
            pcMousepad.transform.SetParent(pcTerminal.transform);
            pcMousepad.transform.localPosition = new Vector3(0.34f, 0.005f, -0.16f);
            pcMousepad.transform.localRotation = Quaternion.identity;
            pcMousepad.transform.localScale = new Vector3(0.20f, 0.005f, 0.22f);
            pcMousepad.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_Mousepad", new Color(0.10f, 0.10f, 0.12f), 0.2f);
            Object.DestroyImmediate(pcMousepad.GetComponent<Collider>());

            GameObject pcMouse = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pcMouse.name = "PC_Mouse";
            pcMouse.transform.SetParent(pcTerminal.transform);
            pcMouse.transform.localPosition = new Vector3(0.34f, 0.022f, -0.16f);
            pcMouse.transform.localRotation = Quaternion.identity;
            pcMouse.transform.localScale = new Vector3(0.07f, 0.030f, 0.11f);
            pcMouse.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_PCMouse", new Color(0.15f, 0.16f, 0.18f), 0.5f);
            Object.DestroyImmediate(pcMouse.GetComponent<Collider>());

            // Thêm BoxCollider tương tác StoreComputer bao trọn toàn bộ cụm máy tính & bàn phím
            BoxCollider pcCol = pcTerminal.AddComponent<BoxCollider>();
            pcCol.center = new Vector3(0f, 0.25f, 0f);
            pcCol.size = new Vector3(0.95f, 0.65f, 0.75f);
            pcTerminal.AddComponent<StoreComputer>();

            // Điện thoại vỡ của David & Khẩu Súng Lục 9mm bên trong ngăn kéo
            GameObject phoneObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            phoneObj.name = "Davids_Broken_Phone";
            phoneObj.transform.SetParent(drawerObj.transform);
            phoneObj.transform.localPosition = new Vector3(0f, 0.05f, -0.2f);
            phoneObj.transform.localScale = new Vector3(0.18f, 0.02f, 0.1f);
            phoneObj.SetActive(false);

            // Khẩu Súng Lục 9mm bên trong ngăn kéo (Pistol_9mm.obj tỉ lệ chuẩn 1:1)
            GameObject pistolWorldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Pistol_9mm.obj");
            GameObject pistolModel;
            if (pistolWorldPrefab != null)
            {
                pistolModel = Object.Instantiate(pistolWorldPrefab, drawerObj.transform);
                pistolModel.name = "Pistol_Pickup_Model";
                pistolModel.transform.localPosition = new Vector3(0f, 0.02f, 0.15f);
                pistolModel.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f); // Nằm ngửa phẳng trong ngăn kéo
                pistolModel.transform.localScale = Vector3.one;
                RemoveCollidersRecursively(pistolModel);
            }
            else
            {
                pistolModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pistolModel.name = "Pistol_Pickup_Model";
                pistolModel.transform.SetParent(drawerObj.transform);
                pistolModel.transform.localPosition = new Vector3(0f, 0.05f, 0.15f);
                pistolModel.transform.localScale = new Vector3(0.28f, 0.12f, 0.06f);
                pistolModel.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_PistolMetal", new Color(0.12f, 0.13f, 0.15f), 0.85f);
            }
            pistolModel.SetActive(false);

            SerializedObject drawerSO = new SerializedObject(secretDrawer);
            SafeSetObject(drawerSO, "_drawerModel", drawerObj.transform);
            SafeSetObject(drawerSO, "_davidsBrokenPhone", phoneObj);
            SafeSetObject(drawerSO, "_shotgunWorldModel", pistolModel);
            SafeSetVector3(drawerSO, "_openedOffset", new Vector3(-0.38f, 0f, 0f));
            SafeSetObject(drawerSO, "_openStuckSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Drawer_Slide_Open.wav"));
            drawerSO.ApplyModifiedProperties();

            return counterObj;
        }

        private static void CreateSupermarketAislesAndMessItems(Transform parent)
        {
            Material frameMat = CreateColoredMaterial("Mat_ShelfFrame", new Color(0.22f, 0.24f, 0.27f), 0.6f);
            Material plankMat = CreateColoredMaterial("Mat_ShelfPlank", new Color(0.75f, 0.77f, 0.80f), 0.3f);
            Material railMat = CreateColoredMaterial("Mat_ShelfGuardRail", new Color(0.35f, 0.38f, 0.42f), 0.7f);
            Material trimMat = CreateColoredMaterial("Mat_ShelfPriceTrim", new Color(0.12f, 0.28f, 0.55f), 0.4f);

            GameObject aislesRoot = new GameObject("Supermarket_Aisles_And_Items");
            aislesRoot.transform.SetParent(parent);

            // Dãy 6 kệ hàng siêu thị Gondola phân bổ đều đặn khắp lòng tiệm 22m (X = -7.5m, -4.5m, -1.5m, 1.5m, 4.5m, 7.5m):
            // Lối đi giữa mỗi kệ rộng 2.1m cực kỳ thoáng đãng, cân xứng hai bên và thoải mái cho người chơi chạy trốn Boss
            // Kệ 1 (Gia dụng & Hóa phẩm): Cạnh quầy thu ngân và tường trái
            CreateAisleShelf("Aisle_1_Household", aislesRoot.transform, new Vector3(-7.5f, 0f, 5.5f), frameMat, plankMat, railMat, trimMat);

            // Kệ 2 (Nước giải khát & Sữa hộp): Đầy đủ hàng hóa
            CreateAisleShelf("Aisle_2_Beverages", aislesRoot.transform, new Vector3(-4.5f, 0f, 5.5f), frameMat, plankMat, railMat, trimMat);

            // Kệ 3 (Bánh snack): ô trống ở mặt phải (skipSide = 1), tầng 1 (skipTier = 1), slot 2 (skipSlot = 2)
            // Tọa độ ô trống: (-1.5 + 0.22, 1.045, 5.5 + 1.1) = (-1.28f, 1.045f, 6.60f)
            CreateAisleShelf("Aisle_3_Snacks", aislesRoot.transform, new Vector3(-1.5f, 0f, 5.5f), frameMat, plankMat, railMat, trimMat, skipSide: 1, skipTier: 1, skipSlot: 2);

            // Kệ 4 (Mì ramen): ô trống ở mặt trái (skipSide = -1), tầng 1 (skipTier = 1), slot 0 (skipSlot = 0)
            // Tọa độ ô trống: (1.5 - 0.22, 1.045, 5.5 + 0.0) = (1.28f, 1.045f, 5.50f)
            CreateAisleShelf("Aisle_4_Ramen", aislesRoot.transform, new Vector3(1.5f, 0f, 5.5f), frameMat, plankMat, railMat, trimMat, skipSide: -1, skipTier: 1, skipSlot: 0);

            // Kệ 5 (Đồ hộp & Ngũ cốc): ô trống ở mặt trái (skipSide = -1), tầng 1 (skipTier = 1), slot -2 (skipSlot = -2)
            // Tọa độ ô trống: (4.5 - 0.22, 1.045, 5.5 - 1.1) = (4.28f, 1.045f, 4.40f)
            CreateAisleShelf("Aisle_5_Canned", aislesRoot.transform, new Vector3(4.5f, 0f, 5.5f), frameMat, plankMat, railMat, trimMat, skipSide: -1, skipTier: 1, skipSlot: -2);

            // Kệ 6 (Bánh kẹo & Đồ ngọt): Cạnh tủ lạnh tường phải
            CreateAisleShelf("Aisle_6_Confectionery", aislesRoot.transform, new Vector3(7.5f, 0f, 5.5f), frameMat, plankMat, railMat, trimMat);

            // 3 Món đồ bừa bộn rơi trên sàn tương ứng với ô trống của Kệ 3, 4, 5
            Material redMat = CreateColoredMaterial("Mat_MessRed", new Color(0.9f, 0.15f, 0.15f));
            Material blueMat = CreateColoredMaterial("Mat_MessBlue", new Color(0.15f, 0.45f, 0.95f));
            Material yellowMat = CreateColoredMaterial("Mat_MessYellow", new Color(0.95f, 0.8f, 0.1f));

            CreateMessItem(aislesRoot.transform, "Gói bánh Snack Đỏ", new Vector3(-0.65f, 0.12f, 6.60f), new Vector3(-1.28f, 0.92f, 6.60f), 0f, redMat);
            CreateMessItem(aislesRoot.transform, "Chai nước ngọt Xanh", new Vector3(0.65f, 0.12f, 5.50f), new Vector3(1.28f, 0.92f, 5.50f), 180f, blueMat);
            CreateMessItem(aislesRoot.transform, "Hộp ngũ cốc Vàng", new Vector3(3.65f, 0.12f, 4.40f), new Vector3(4.28f, 0.92f, 4.40f), 180f, yellowMat);

            // 3 Vết bẩn sẫm màu bí ẩn dưới sàn gạch (Nhiệm vụ chuẩn bị ca trực: Dùng cây lau nhà hoặc [E] lau sạch)
            Material stainMat = CreateColoredMaterial("Mat_FloorDirtStain", new Color(0.08f, 0.05f, 0.04f, 0.95f), 0.1f);
            AudioClip mopAudio = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Mop_Scrub_Floor.wav");

            CreateFloorStain(aislesRoot.transform, "Floor_Stain_1_Entrance", new Vector3(0f, 0.012f, 0.8f), new Vector3(1.2f, 0.01f, 0.95f), stainMat, mopAudio);
            CreateFloorStain(aislesRoot.transform, "Floor_Stain_2_AisleWalkway", new Vector3(-3.0f, 0.012f, 5.2f), new Vector3(1.05f, 0.01f, 1.25f), stainMat, mopAudio);
            CreateFloorStain(aislesRoot.transform, "Floor_Stain_3_RightWalkway", new Vector3(3.0f, 0.012f, 5.8f), new Vector3(1.3f, 0.01f, 1.0f), stainMat, mopAudio);

            // Cây lau nhà và Xô nước đặt ở góc trước bên phải tiệm (X = 4.2m, Z = -3.8m, cách xa cửa chính X = 0m và giấy note X = -1.55m để tuyệt đối không bị tương tác nhầm khi đọc note)
            CreateCleaningMopProp(aislesRoot.transform, new Vector3(4.2f, 0f, -3.8f));
        }

        private static void CreateFloorStain(Transform parent, string stainName, Vector3 position, Vector3 scale, Material stainMat, AudioClip mopClip)
        {
            GameObject stainObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stainObj.name = stainName;
            stainObj.transform.SetParent(parent);
            stainObj.transform.position = position;
            stainObj.transform.localScale = scale;
            stainObj.GetComponent<Renderer>().sharedMaterial = stainMat;

            Collider oldCol = stainObj.GetComponent<Collider>();
            if (oldCol != null) Object.DestroyImmediate(oldCol);

            BoxCollider boxCol = stainObj.AddComponent<BoxCollider>();
            boxCol.size = new Vector3(1.3f, 0.5f, 1.3f);
            boxCol.isTrigger = false;

            StoreFloorStain stainComp = stainObj.AddComponent<StoreFloorStain>();
            SerializedObject stainSO = new SerializedObject(stainComp);
            SafeSetObject(stainSO, "_mopClip", mopClip);
            stainSO.ApplyModifiedProperties();
        }

        private static void CreateCleaningMopProp(Transform parent, Vector3 position)
        {
            GameObject mopRoot = new GameObject("Store_Cleaning_Mop_Bucket");
            mopRoot.transform.SetParent(parent);
            mopRoot.transform.position = position;

            GameObject bucketPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Mop_Bucket.obj");
            GameObject mopPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Store_Mop.obj");

            GameObject bucket;
            if (bucketPrefab != null)
            {
                bucket = Object.Instantiate(bucketPrefab, mopRoot.transform);
                bucket.name = "Bucket";
                bucket.transform.localPosition = Vector3.zero;
                bucket.transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
                bucket.transform.localScale = Vector3.one;
                RemoveCollidersRecursively(bucket);
            }
            else
            {
                Material bucketMat = CreateColoredMaterial("Mat_MopBucketYellow", new Color(0.9f, 0.8f, 0.1f), 0.4f);
                bucket = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bucket.name = "Bucket";
                bucket.transform.SetParent(mopRoot.transform);
                bucket.transform.localPosition = new Vector3(0f, 0.22f, 0f);
                bucket.transform.localScale = new Vector3(0.42f, 0.22f, 0.42f);
                bucket.GetComponent<Renderer>().sharedMaterial = bucketMat;
                Object.DestroyImmediate(bucket.GetComponent<Collider>());
            }

            GameObject mopHandle;
            if (mopPrefab != null)
            {
                mopHandle = Object.Instantiate(mopPrefab, mopRoot.transform);
                mopHandle.name = "Mop_Handle";
                mopHandle.transform.localPosition = new Vector3(0.06f, 0.05f, 0.04f);
                mopHandle.transform.localRotation = Quaternion.Euler(14f, 0f, -10f);
                mopHandle.transform.localScale = Vector3.one;
                RemoveCollidersRecursively(mopHandle);
            }
            else
            {
                Material handleMat = CreateColoredMaterial("Mat_MopHandleMetal", new Color(0.75f, 0.75f, 0.78f), 0.7f);
                Material mopHeadMat = CreateColoredMaterial("Mat_MopCloth", new Color(0.6f, 0.58f, 0.55f), 0.1f);

                mopHandle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                mopHandle.name = "Mop_Handle";
                mopHandle.transform.SetParent(mopRoot.transform);
                mopHandle.transform.localPosition = new Vector3(0.12f, 0.75f, 0.05f);
                mopHandle.transform.localRotation = Quaternion.Euler(15f, 0f, -12f);
                mopHandle.transform.localScale = new Vector3(0.045f, 0.65f, 0.045f);
                mopHandle.GetComponent<Renderer>().sharedMaterial = handleMat;
                Object.DestroyImmediate(mopHandle.GetComponent<Collider>());

                GameObject mopHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mopHead.name = "Mop_Head_Cloth";
                mopHead.transform.SetParent(mopHandle.transform);
                mopHead.transform.localPosition = new Vector3(0f, -0.9f, 0f);
                mopHead.transform.localScale = new Vector3(3.5f, 0.4f, 3.5f);
                mopHead.GetComponent<Renderer>().sharedMaterial = mopHeadMat;
                Object.DestroyImmediate(mopHead.GetComponent<Collider>());
            }

            // BoxCollider tương tác đặt trên mopRoot bao trọn xô và cây lau nhà
            BoxCollider mopCol = mopRoot.AddComponent<BoxCollider>();
            mopCol.center = new Vector3(0.05f, 0.75f, 0.02f);
            mopCol.size = new Vector3(0.85f, 1.6f, 0.85f);
            StoreCleaningMop cleaningMop = mopRoot.AddComponent<StoreCleaningMop>();
            SerializedObject mopSO = new SerializedObject(cleaningMop);
            SafeSetObject(mopSO, "_mopHandleVisual", mopHandle);
            SafeSetObject(mopSO, "_pickupClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Item_Pickup_Pop.wav"));
            mopSO.ApplyModifiedProperties();
        }

        private static void CreateAisleShelf(string name, Transform parent, Vector3 position, Material frameMat, Material plankMat, Material railMat, Material trimMat, int skipSide = 0, int skipTier = -1, int skipSlot = 999)
        {
            GameObject aisle = new GameObject(name);
            aisle.transform.SetParent(parent);
            aisle.transform.position = position;

            // 1. Tấm vách ngăn trung tâm (Backboard Spine chịu lực)
            GameObject spine = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spine.name = "Backboard_Spine";
            spine.transform.SetParent(aisle.transform);
            spine.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            spine.transform.localScale = new Vector3(0.06f, 2.0f, 7.2f);
            spine.GetComponent<Renderer>().sharedMaterial = frameMat;

            // 2. Hai cột khung chịu lực 2 đầu kệ (Endcaps)
            GameObject endcapFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
            endcapFront.name = "Endcap_Front";
            endcapFront.transform.SetParent(aisle.transform);
            endcapFront.transform.localPosition = new Vector3(0f, 1.05f, -3.6f);
            endcapFront.transform.localScale = new Vector3(0.92f, 2.1f, 0.10f);
            endcapFront.GetComponent<Renderer>().sharedMaterial = frameMat;

            GameObject endcapBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            endcapBack.name = "Endcap_Back";
            endcapBack.transform.SetParent(aisle.transform);
            endcapBack.transform.localPosition = new Vector3(0f, 1.05f, 3.6f);
            endcapBack.transform.localScale = new Vector3(0.92f, 2.1f, 0.10f);
            endcapBack.GetComponent<Renderer>().sharedMaterial = frameMat;

            // 3. Chân đế hộp kín chịu lực sát sàn (Kickplate Base chống xô lệch)
            GameObject kickplate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            kickplate.name = "Kickplate_Base";
            kickplate.transform.SetParent(aisle.transform);
            kickplate.transform.localPosition = new Vector3(0f, 0.10f, 0f);
            kickplate.transform.localScale = new Vector3(0.90f, 0.20f, 7.2f);
            kickplate.GetComponent<Renderer>().sharedMaterial = frameMat;

            // 4. Mái nóc kệ cố định trên cùng (Top Canopy)
            GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            canopy.name = "Top_Canopy";
            canopy.transform.SetParent(aisle.transform);
            canopy.transform.localPosition = new Vector3(0f, 2.06f, 0f);
            canopy.transform.localScale = new Vector3(0.90f, 0.08f, 7.2f);
            canopy.GetComponent<Renderer>().sharedMaterial = frameMat;

            // 5. Ba tầng mâm ván đỡ hàng (Shelf Decks) có thanh gờ chắn chống rơi đổ (Guard Rails)
            Material[] itemColors = new Material[]
            {
                CreateColoredMaterial("Mat_Stock_OrangeSnack", new Color(0.95f, 0.52f, 0.12f)),
                CreateColoredMaterial("Mat_Stock_CyanSoda", new Color(0.12f, 0.75f, 0.90f)),
                CreateColoredMaterial("Mat_Stock_PurpleRamen", new Color(0.68f, 0.22f, 0.78f)),
                CreateColoredMaterial("Mat_Stock_GreenTea", new Color(0.20f, 0.75f, 0.35f)),
                CreateColoredMaterial("Mat_Stock_RedCereal", new Color(0.88f, 0.20f, 0.20f))
            };

            // Tải trước các mô hình 3D thực tế chi tiết từ Assets/Models (tạo bằng Blender 5.1)
            GameObject ramenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Instant_Ramen.obj");
            GameObject chipsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Chips_Tube.obj");
            GameObject cannedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Canned_Food.obj");
            GameObject milkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Milk_Carton.obj");
            GameObject cerealPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Cereal_Box.obj");
            GameObject snackBagPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Snack_Bag.obj");
            GameObject sodaBottlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Soda_Bottle.obj");
            GameObject energyDrinkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Energy_Drink.obj");
            GameObject beerBottlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Beer_Bottle.obj");
            GameObject sodaCanPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Can_Soda.obj");

            for (int tier = 0; tier < 3; tier++)
            {
                float tierY = 0.35f + tier * 0.55f;

                int[] sides = new int[] { -1, 1 };
                foreach (int side in sides)
                {
                    string sideName = side == -1 ? "Left" : "Right";

                    // Mâm ván đỡ phẳng chịu tải
                    GameObject plank = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    plank.name = $"Plank_{sideName}_Tier_{tier}";
                    plank.transform.SetParent(aisle.transform);
                    plank.transform.localPosition = new Vector3(side * 0.22f, tierY, 0f);
                    plank.transform.localScale = new Vector3(0.40f, 0.04f, 7.15f);
                    plank.GetComponent<Renderer>().sharedMaterial = plankMat;

                    // Thanh gờ chắn chống rơi đổ hàng ở mép ngoài
                    GameObject guardRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    guardRail.name = $"GuardRail_{sideName}_Tier_{tier}";
                    guardRail.transform.SetParent(aisle.transform);
                    guardRail.transform.localPosition = new Vector3(side * 0.42f, tierY + 0.035f, 0f);
                    guardRail.transform.localScale = new Vector3(0.03f, 0.06f, 7.15f);
                    guardRail.GetComponent<Renderer>().sharedMaterial = railMat;

                    // Nẹp viền bảng giá
                    GameObject priceStrip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    priceStrip.name = $"PriceStrip_{sideName}_Tier_{tier}";
                    priceStrip.transform.SetParent(aisle.transform);
                    priceStrip.transform.localPosition = new Vector3(side * 0.436f, tierY, 0f);
                    priceStrip.transform.localScale = new Vector3(0.01f, 0.035f, 7.15f);
                    priceStrip.GetComponent<Renderer>().sharedMaterial = trimMat;

                    // Hàng hóa đặt ngay ngắn trên mâm (11 slot dọc chiều dài 7.2m)
                    for (int slot = -5; slot <= 5; slot++)
                    {
                        if (side == skipSide && tier == skipTier && slot == skipSlot)
                        {
                            continue; // Bỏ trống ô này cho món đồ rơi vãi
                        }

                        float localZ = slot * 0.55f;

                        GameObject chosenPrefab = null;
                        if (name.Contains("Ramen"))
                        {
                            chosenPrefab = ramenPrefab;
                        }
                        else if (name.Contains("Snack"))
                        {
                            chosenPrefab = (slot % 2 == 0) ? chipsPrefab : snackBagPrefab;
                        }
                        else if (name.Contains("Beverages"))
                        {
                            chosenPrefab = (slot % 3 == 0) ? sodaBottlePrefab : ((slot % 3 == 1) ? energyDrinkPrefab : sodaCanPrefab);
                        }
                        else if (name.Contains("Canned"))
                        {
                            chosenPrefab = (slot % 2 == 0) ? cannedPrefab : cerealPrefab;
                        }
                        else if (name.Contains("Household"))
                        {
                            chosenPrefab = (slot % 3 == 0) ? milkPrefab : ((slot % 3 == 1) ? cannedPrefab : cerealPrefab);
                        }
                        else
                        {
                            chosenPrefab = (slot % 3 == 0) ? chipsPrefab : ((slot % 3 == 1) ? ramenPrefab : snackBagPrefab);
                        }

                        if (chosenPrefab != null)
                        {
                            GameObject itemInstance = Object.Instantiate(chosenPrefab, aisle.transform);
                            itemInstance.name = $"Stock_{chosenPrefab.name}_{sideName}_T{tier}_S{slot}";
                            itemInstance.transform.localPosition = new Vector3(side * 0.22f, tierY + 0.02f, localZ);
                            float yRot = (side > 0 ? 0f : 180f) + ((slot % 3 - 1) * 8f);
                            itemInstance.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);
                            itemInstance.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                            RemoveCollidersRecursively(itemInstance);
                        }
                        else
                        {
                            GameObject stockItem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            stockItem.name = $"StockItem_{sideName}_T{tier}_S{slot}";
                            stockItem.transform.SetParent(aisle.transform);
                            stockItem.transform.localPosition = new Vector3(side * 0.22f, tierY + 0.145f, localZ);
                            stockItem.transform.localScale = new Vector3(0.24f, 0.25f, 0.40f);
                            int matIndex = Mathf.Abs(tier * 3 + slot * 2 + (side > 0 ? 1 : 0)) % itemColors.Length;
                            stockItem.GetComponent<Renderer>().sharedMaterial = itemColors[matIndex];
                        }
                    }
                }
            }
        }

        private static void CreateDrinkCoolers(Transform parent)
        {
            Material coolerFrameMat = CreateColoredMaterial("Mat_CoolerFrame", new Color(0.15f, 0.18f, 0.22f), 0.6f);
            Material coolerLightMat = CreateColoredMaterial("Mat_CoolerInteriorGlow", new Color(0.6f, 0.9f, 1.0f), 0.5f, new Color(0.3f, 0.7f, 1.0f) * 2.2f);
            Material coolerGlassMat = CreateGlassMaterial("Mat_CoolerGlassDoor", new Color(0.7f, 0.95f, 1.0f, 0.4f));
            Material coolerShelfMat = CreateColoredMaterial("Mat_CoolerWireShelf", new Color(0.7f, 0.72f, 0.75f), 0.8f);

            GameObject canPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Can_Soda.obj");
            GameObject bottlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Soda_Bottle.obj");
            GameObject energyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Energy_Drink.obj");
            GameObject beerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Beer_Bottle.obj");
            GameObject milkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Milk_Carton.obj");

            GameObject coolersRoot = new GameObject("Drink_Coolers_Wall");
            coolersRoot.transform.SetParent(parent);
            coolersRoot.transform.position = new Vector3(10.75f, 1.3f, 5.5f);

            // Dãy 6 Tủ lạnh kính cạnh tường phải (bao trùm suốt chiều dài tiệm)
            for (int i = 0; i < 6; i++)
            {
                GameObject cooler = new GameObject($"Cooler_Door_{i + 1}");
                cooler.transform.SetParent(coolersRoot.transform);
                cooler.transform.localPosition = new Vector3(0f, 0f, -4.5f + i * 1.8f);

                GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
                frame.name = "Frame";
                frame.transform.SetParent(cooler.transform);
                frame.transform.localPosition = Vector3.zero;
                frame.transform.localScale = new Vector3(0.4f, 2.4f, 1.5f);
                frame.GetComponent<Renderer>().sharedMaterial = coolerFrameMat;

                GameObject interior = GameObject.CreatePrimitive(PrimitiveType.Cube);
                interior.name = "Interior_Neon_Light";
                interior.transform.SetParent(cooler.transform);
                interior.transform.localPosition = new Vector3(0.08f, 0f, 0f);
                interior.transform.localScale = new Vector3(0.2f, 2.2f, 1.35f);
                interior.GetComponent<Renderer>().sharedMaterial = coolerLightMat;

                GameObject glass = GameObject.CreatePrimitive(PrimitiveType.Cube);
                glass.name = "Glass_Door";
                glass.transform.SetParent(cooler.transform);
                glass.transform.localPosition = new Vector3(-0.2f, 0f, 0f);
                glass.transform.localScale = new Vector3(0.04f, 2.25f, 1.4f);
                glass.GetComponent<Renderer>().sharedMaterial = coolerGlassMat;

                // 4 Tầng giá đỡ dây thép kim loại trong tủ lạnh & các chai/lon nước ngọt
                float[] shelfYs = new float[] { -0.75f, -0.30f, 0.18f, 0.65f };
                for (int s = 0; s < shelfYs.Length; s++)
                {
                    GameObject shelfWire = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    shelfWire.name = $"Cooler_Shelf_{s + 1}";
                    shelfWire.transform.SetParent(cooler.transform);
                    shelfWire.transform.localPosition = new Vector3(-0.02f, shelfYs[s], 0f);
                    shelfWire.transform.localScale = new Vector3(0.28f, 0.02f, 1.32f);
                    shelfWire.GetComponent<Renderer>().sharedMaterial = coolerShelfMat;
                    RemoveCollidersRecursively(shelfWire);

                    // Chọn prefab nước giải khát cho từng tủ
                    GameObject drinkPrefab = null;
                    if (i == 0) drinkPrefab = canPrefab;
                    else if (i == 1) drinkPrefab = energyPrefab;
                    else if (i == 2 || i == 3) drinkPrefab = bottlePrefab;
                    else if (i == 4) drinkPrefab = beerPrefab;
                    else drinkPrefab = milkPrefab;

                    if (drinkPrefab != null)
                    {
                        for (int col = -2; col <= 2; col++)
                        {
                            GameObject drink = Object.Instantiate(drinkPrefab, cooler.transform);
                            drink.name = $"Drink_{drinkPrefab.name}_S{s}_C{col}";
                            drink.transform.localPosition = new Vector3(-0.02f, shelfYs[s] + 0.01f, col * 0.24f);
                            drink.transform.localRotation = Quaternion.Euler(0f, col * 12f, 0f);
                            drink.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                            RemoveCollidersRecursively(drink);
                        }
                    }
                }
            }
        }

        private static void CreateStoreProps(Transform parent)
        {
            GameObject propsRoot = new GameObject("Store_Details_And_Props");
            propsRoot.transform.SetParent(parent);

            // 1. Gương Cầu Lồi Góc Tiệm
            GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mirror.name = "Convex_Security_Mirror";
            mirror.transform.SetParent(propsRoot.transform);
            mirror.transform.position = new Vector3(-10.6f, 3.4f, 14.6f);
            mirror.transform.localScale = new Vector3(0.85f, 0.85f, 0.35f);
            mirror.transform.rotation = Quaternion.Euler(30f, 135f, 0f);
            mirror.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_ConvexMirror", new Color(0.9f, 0.92f, 0.95f), 0.98f);

            // 2. Cửa Kho Sau "EMPLOYEES ONLY"
            GameObject storageDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            storageDoor.name = "Back_Storage_Door";
            storageDoor.transform.SetParent(propsRoot.transform);
            storageDoor.transform.position = new Vector3(0f, 1.3f, 14.92f);
            storageDoor.transform.localScale = new Vector3(1.4f, 2.6f, 0.08f);
            storageDoor.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_StorageDoor", new Color(0.25f, 0.28f, 0.3f), 0.3f);

            // Cửa thoát hiểm khẩn cấp (Emergency Fire Exit)
            GameObject exitDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            exitDoor.name = "Emergency_Fire_Exit";
            exitDoor.transform.SetParent(propsRoot.transform);
            exitDoor.transform.position = new Vector3(8.5f, 1.3f, 14.92f);
            exitDoor.transform.localScale = new Vector3(1.3f, 2.5f, 0.08f);
            exitDoor.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_ExitDoor", new Color(0.6f, 0.15f, 0.15f), 0.4f);

            // 3. Biển Báo Vàng "CẢNH BÁO SÀN ƯỚT"
            GameObject wetSign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wetSign.name = "Caution_Wet_Floor_Sign";
            wetSign.transform.SetParent(propsRoot.transform);
            wetSign.transform.position = new Vector3(0.6f, 0.35f, -2.5f);
            wetSign.transform.localScale = new Vector3(0.25f, 0.65f, 0.15f);
            wetSign.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_CautionYellow", new Color(0.95f, 0.85f, 0.1f), 0.5f);

            // 4. Thùng rác inox
            GameObject trashCan = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trashCan.name = "Store_Trash_Can";
            trashCan.transform.SetParent(propsRoot.transform);
            trashCan.transform.position = new Vector3(-9.5f, 0.45f, -4.2f);
            trashCan.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            trashCan.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_TrashMetal", new Color(0.7f, 0.72f, 0.75f), 0.8f);
        }

        private static void CreateMessItem(Transform parent, string itemName, Vector3 floorPos, Vector3 shelfPos, float shelfYRot, Material mat)
        {
            // 1. Ô vị trí được chỉ định trên kệ hàng (Shelf Slot)
            GameObject shelfSlot = new GameObject($"Shelf_Slot_{itemName}");
            shelfSlot.transform.SetParent(parent);
            shelfSlot.transform.position = shelfPos;
            shelfSlot.transform.rotation = Quaternion.Euler(0f, shelfYRot, 0f);
            BoxCollider slotCol = shelfSlot.AddComponent<BoxCollider>();
            slotCol.size = new Vector3(0.40f, 0.40f, 0.50f);
            ShelfPlacementSlot placement = shelfSlot.AddComponent<ShelfPlacementSlot>();

            // Khay định vị ô trống trực quan phát sáng nằm phẳng trực tiếp trên mặt mâm kệ
            GameObject slotIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slotIndicator.name = "Empty_Slot_Indicator_Tray";
            slotIndicator.transform.SetParent(shelfSlot.transform);
            slotIndicator.transform.localPosition = new Vector3(0f, 0.005f, 0f);
            slotIndicator.transform.localScale = new Vector3(0.36f, 0.01f, 0.48f);
            Material indicatorMat = CreateColoredMaterial("Mat_EmptySlotGlow", new Color(0.95f, 0.85f, 0.15f, 0.8f), 0.5f, new Color(1.0f, 0.85f, 0.1f) * 1.5f);
            slotIndicator.GetComponent<Renderer>().sharedMaterial = indicatorMat;
            Object.DestroyImmediate(slotIndicator.GetComponent<Collider>());

            // 2. Món đồ rơi vãi trên sàn (Sử dụng mô hình 3D to rõ hơn, tỉ lệ 1.35x)
            GameObject itemPrefab = null;
            if (itemName.Contains("Snack")) itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Chips_Tube.obj");
            else if (itemName.Contains("nước")) itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Soda_Bottle.obj");
            else itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Cereal_Box.obj");

            GameObject item = new GameObject(itemName);
            item.transform.SetParent(parent);
            item.transform.position = floorPos;

            if (itemPrefab != null)
            {
                GameObject visual = Object.Instantiate(itemPrefab, item.transform);
                visual.name = "Visual_Model";
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.Euler(75f, 30f, 0f);
                visual.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                RemoveCollidersRecursively(visual);
            }
            else
            {
                GameObject cubeVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeVisual.name = "Cube_Visual";
                cubeVisual.transform.SetParent(item.transform);
                cubeVisual.transform.localPosition = Vector3.zero;
                cubeVisual.transform.localScale = new Vector3(0.30f, 0.28f, 0.42f);
                cubeVisual.GetComponent<Renderer>().sharedMaterial = mat;
                Object.DestroyImmediate(cubeVisual.GetComponent<Collider>());
            }

            BoxCollider messCol = item.AddComponent<BoxCollider>();
            messCol.size = new Vector3(0.40f, 0.40f, 0.40f);

            Rigidbody rb = item.AddComponent<Rigidbody>();
            rb.mass = 0.5f;

            MessItem mess = item.AddComponent<MessItem>();
            SerializedObject so = new SerializedObject(mess);
            SafeSetString(so, "_itemName", itemName);
            SafeSetObject(so, "_targetShelfPosition", shelfSlot.transform);
            SafeSetObject(so, "_pickupSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Item_Pickup_Pop.wav"));
            so.ApplyModifiedProperties();

            placement.SetupSlot(mess, itemName, slotIndicator);
        }

        private static CustomerNPC CreateTutorialCustomer(Transform parent, Transform counter, out Transform doorPoint, out Transform waitingPoint)
        {
            Material coatMat = CreateColoredMaterial("Mat_OldManCoat", new Color(0.45f, 0.42f, 0.35f));

            GameObject customer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            customer.name = "Customer_OldMan_NPC";
            customer.transform.SetParent(parent);
            customer.transform.position = new Vector3(0f, 0f, -7.5f);
            customer.GetComponent<Renderer>().sharedMaterial = coatMat;

            CustomerNPC npc = customer.AddComponent<CustomerNPC>();

            Material cigMat = CreateColoredMaterial("Mat_CigaretteRed", new Color(0.85f, 0.15f, 0.15f));
            Material snackMat = CreateColoredMaterial("Mat_SnackYellow", new Color(0.95f, 0.8f, 0.15f));
            Material sodaMat = CreateColoredMaterial("Mat_SodaBlue", new Color(0.15f, 0.5f, 0.95f));

            GameObject cartRoot = new GameObject("Tutorial_Customer_Cart");
            cartRoot.transform.SetParent(customer.transform);
            cartRoot.transform.localPosition = new Vector3(0f, -50f, 0f); // Đặt sâu dưới lòng đất, tuyệt đối không xuất hiện dưới chân khách hàng!

            // 3 Món hàng khách mua từ Blender models chuẩn tỉ lệ 1:1
            // Món 1: Hộp ngũ cốc (Grocery_Cereal_Box.obj, 17cm x 24cm x 6cm)
            GameObject cerealPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Cereal_Box.obj");
            GameObject cigarettePack;
            if (cerealPrefab != null)
            {
                cigarettePack = Object.Instantiate(cerealPrefab, cartRoot.transform);
                cigarettePack.name = "Cereal_Box_Item";
                cigarettePack.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                RemoveCollidersRecursively(cigarettePack);
                BoxCollider col = cigarettePack.AddComponent<BoxCollider>();
                col.size = new Vector3(0.22f, 0.32f, 0.08f);
            }
            else
            {
                cigarettePack = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cigarettePack.name = "Cigarette_Pack_Item";
                cigarettePack.transform.SetParent(cartRoot.transform);
                cigarettePack.transform.localScale = new Vector3(0.18f, 0.28f, 0.10f);
                cigarettePack.GetComponent<Renderer>().sharedMaterial = cigMat;
            }
            cigarettePack.SetActive(false);
            ScannableItem scannable1 = cigarettePack.AddComponent<ScannableItem>();
            scannable1.SetupItem("Hộp Bánh Cereal Ngũ Cốc", 5.50f);

            // Món 2: Gói Bim Bim khoai tây (Grocery_Snack_Bag.obj, 13cm x 18.5cm x 4.6cm)
            GameObject snackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Snack_Bag.obj");
            GameObject snackPack;
            if (snackPrefab != null)
            {
                snackPack = Object.Instantiate(snackPrefab, cartRoot.transform);
                snackPack.name = "Snack_Chips_Item";
                snackPack.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                RemoveCollidersRecursively(snackPack);
                BoxCollider col = snackPack.AddComponent<BoxCollider>();
                col.size = new Vector3(0.18f, 0.26f, 0.07f);
            }
            else
            {
                snackPack = GameObject.CreatePrimitive(PrimitiveType.Cube);
                snackPack.name = "Snack_Chips_Item";
                snackPack.transform.SetParent(cartRoot.transform);
                snackPack.transform.localScale = new Vector3(0.24f, 0.30f, 0.10f);
                snackPack.GetComponent<Renderer>().sharedMaterial = snackMat;
            }
            snackPack.SetActive(false);
            ScannableItem scannable2 = snackPack.AddComponent<ScannableItem>();
            scannable2.SetupItem("Gói Snack Khoai Tây", 3.50f);

            // Món 3: Lon nước ngọt Soda chanh (Grocery_Can_Soda.obj, 6.7cm x 12.2cm)
            GameObject sodaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Can_Soda.obj");
            GameObject sodaCan;
            if (sodaPrefab != null)
            {
                sodaCan = Object.Instantiate(sodaPrefab, cartRoot.transform);
                sodaCan.name = "Soda_Can_Item";
                sodaCan.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                RemoveCollidersRecursively(sodaCan);
                CapsuleCollider col = sodaCan.AddComponent<CapsuleCollider>();
                col.height = 0.16f;
                col.radius = 0.045f;
            }
            else
            {
                sodaCan = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                sodaCan.name = "Soda_Can_Item";
                sodaCan.transform.SetParent(cartRoot.transform);
                sodaCan.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                sodaCan.GetComponent<Renderer>().sharedMaterial = sodaMat;
            }
            sodaCan.SetActive(false);
            ScannableItem scannable3 = sodaCan.AddComponent<ScannableItem>();
            scannable3.SetupItem("Lon Soda Chanh", 2.50f);

            // Món 4: Tô mì ăn liền cay (Grocery_Instant_Ramen.obj, 16cm x 11cm)
            GameObject ramenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Grocery_Instant_Ramen.obj");
            GameObject ramenBowl;
            if (ramenPrefab != null)
            {
                ramenBowl = Object.Instantiate(ramenPrefab, cartRoot.transform);
                ramenBowl.name = "Instant_Ramen_Item";
                ramenBowl.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                RemoveCollidersRecursively(ramenBowl);
                BoxCollider col = ramenBowl.AddComponent<BoxCollider>();
                col.size = new Vector3(0.22f, 0.15f, 0.22f);
            }
            else
            {
                ramenBowl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ramenBowl.name = "Instant_Ramen_Item";
                ramenBowl.transform.SetParent(cartRoot.transform);
                ramenBowl.transform.localScale = new Vector3(0.20f, 0.11f, 0.20f);
                ramenBowl.GetComponent<Renderer>().sharedMaterial = cigMat;
            }
            ramenBowl.SetActive(false);
            ScannableItem scannable4 = ramenBowl.AddComponent<ScannableItem>();
            scannable4.SetupItem("Tô Mì Ăn Liền Cay", 3.00f);

            // Thẻ Căn cước công dân (ID Card) của Ông lão khách quen (Prop_Citizen_ID.obj, 8.6cm x 5.4cm)
            GameObject idPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Prop_Citizen_ID.obj");
            GameObject idCardObj;
            if (idPrefab != null)
            {
                idCardObj = Object.Instantiate(idPrefab, cartRoot.transform);
                idCardObj.name = "Customer_ID_Card";
                idCardObj.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                RemoveCollidersRecursively(idCardObj);
                BoxCollider col = idCardObj.AddComponent<BoxCollider>();
                col.size = new Vector3(0.12f, 0.01f, 0.08f);
            }
            else
            {
                idCardObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                idCardObj.name = "Customer_ID_Card";
                idCardObj.transform.SetParent(cartRoot.transform);
                idCardObj.transform.localScale = new Vector3(0.22f, 0.02f, 0.32f);
                idCardObj.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_IDCardCiv", new Color(0.9f, 0.92f, 0.95f), 0.2f);
            }
            idCardObj.SetActive(false);
            CustomerIDCard idCard = idCardObj.AddComponent<CustomerIDCard>();
            idCard.SetupIDCard("Arthur Morgan", 1952, "BK-1049-CIV", "Đường số 3, Thị trấn Blackwood", false);

            var itemsList = new System.Collections.Generic.List<ScannableItem> { scannable1, scannable2, scannable3, scannable4 };
            npc.SetupCustomer("Ông lão khách quen", false, itemsList, idCard, 20.0f);

            // Gán mô hình 3D nam có gắn xương (Rigged Humanoid FBX) từ Blender
            GameObject maleModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Male_Rigged.fbx");
            if (maleModel == null) maleModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Male.obj");
            if (maleModel != null)
            {
                npc.ApplyVisualMesh(maleModel);
            }

            GameObject placementPoint = new GameObject("Counter_Item_Placement");
            placementPoint.transform.SetParent(counter);
            placementPoint.transform.localPosition = new Vector3(0.10f, 0.53f, 0.60f); // Bố trí ở góc trái bàn quầy hoàn toàn thông thoáng, tách biệt hẳn máy tính tiền

            GameObject doorObj = new GameObject("Door_Entrance_Point");
            doorObj.transform.SetParent(parent);
            doorObj.transform.position = new Vector3(0f, 0f, -7.5f);
            doorPoint = doorObj.transform;

            // 3 Điểm đứng lựa hàng ngẫu nhiên tại các lối đi siêu thị (Sát mặt sàn Y = 0, nằm giữa lối đi thông thoáng)
            GameObject shelfPointsRoot = new GameObject("Customer_Shelf_Browsing_Points");
            shelfPointsRoot.transform.SetParent(parent);

            GameObject shelfPoint1 = new GameObject("Shelf_Point_1_Snacks_Aisle");
            shelfPoint1.transform.SetParent(shelfPointsRoot.transform);
            shelfPoint1.transform.position = new Vector3(0f, 0f, 5.50f); // Lối đi chính giữa Kệ 3 và Kệ 4

            GameObject shelfPoint2 = new GameObject("Shelf_Point_2_Ramen_Aisle");
            shelfPoint2.transform.SetParent(shelfPointsRoot.transform);
            shelfPoint2.transform.position = new Vector3(3.0f, 0f, 5.50f); // Lối đi giữa Kệ 4 và Kệ 5

            GameObject shelfPoint3 = new GameObject("Shelf_Point_3_Left_Aisle");
            shelfPoint3.transform.SetParent(shelfPointsRoot.transform);
            shelfPoint3.transform.position = new Vector3(-3.0f, 0f, 5.50f); // Lối đi giữa Kệ 2 và Kệ 3

            Transform[] shelfBrowsingPoints = new Transform[] { shelfPoint1.transform, shelfPoint2.transform, shelfPoint3.transform };
            npc.SetShelfBrowsingPoints(shelfBrowsingPoints);

            // Điểm đứng chờ phía trước bàn quầy thu ngân (X = -1.45m, Z = -1.60m đối diện phần bàn trống góc trái)
            GameObject waitingObj = new GameObject("Customer_Counter_Waiting_Point");
            waitingObj.transform.SetParent(parent);
            waitingObj.transform.position = new Vector3(-1.45f, 0f, -1.60f);
            waitingPoint = waitingObj.transform;

            SerializedObject npcSO = new SerializedObject(npc);
            SafeSetObject(npcSO, "_doorEntrancePoint", doorPoint);
            SafeSetObject(npcSO, "_shelfBrowsingPoint", shelfPoint1.transform);
            SafeSetObject(npcSO, "_counterWaitingPoint", waitingPoint);
            SafeSetObject(npcSO, "_itemToBuy", scannable1);
            SafeSetObject(npcSO, "_idCard", idCard);
            SafeSetObject(npcSO, "_counterItemPlacement", placementPoint.transform);
            SafeSetObject(npcSO, "_doorBellSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Door_Chime_DingDong.wav"));

            SerializedProperty itemsProp = npcSO.FindProperty("_itemsToBuy");
            if (itemsProp != null)
            {
                itemsProp.arraySize = itemsList.Count;
                for (int i = 0; i < itemsList.Count; i++)
                {
                    itemsProp.GetArrayElementAtIndex(i).objectReferenceValue = itemsList[i];
                }
            }

            SerializedProperty arrayProp = npcSO.FindProperty("_shelfBrowsingPoints");
            if (arrayProp != null)
            {
                arrayProp.arraySize = shelfBrowsingPoints.Length;
                for (int i = 0; i < shelfBrowsingPoints.Length; i++)
                {
                    arrayProp.GetArrayElementAtIndex(i).objectReferenceValue = shelfBrowsingPoints[i];
                }
            }
            npcSO.ApplyModifiedProperties();

            customer.SetActive(false);
            return npc;
        }

        private static GameObject CreatePlayerRig(Vector3 spawnPosition, Quaternion spawnRotation, GameObject parkedBike)
        {
            GameObject player = new GameObject("Player_FPS_Character");
            player.tag = "Player";
            player.layer = LayerMask.NameToLayer("Ignore Raycast");

            player.transform.position = spawnPosition;
            player.transform.rotation = spawnRotation;

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;
            cc.center = new Vector3(0f, 0.9f, 0f);
            cc.skinWidth = 0.05f;
            cc.stepOffset = 0.3f;
            cc.minMoveDistance = 0f;

            // Gắn PlayerInput
            UnityEngine.InputSystem.PlayerInput playerInput = player.AddComponent<UnityEngine.InputSystem.PlayerInput>();
            UnityEngine.InputSystem.InputActionAsset actions = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (actions != null)
            {
                playerInput.actions = actions;
                playerInput.defaultActionMap = "Player";
                playerInput.notificationBehavior = UnityEngine.InputSystem.PlayerNotifications.SendMessages;
            }

            PlayerMovement movement = player.AddComponent<PlayerMovement>();
            PlayerLook look = player.AddComponent<PlayerLook>();
            PlayerInventory inventory = player.AddComponent<PlayerInventory>();
            PlayerInteraction interaction = player.AddComponent<PlayerInteraction>();
            PlayerFlashlight flashlight = player.AddComponent<PlayerFlashlight>();

            // Camera Chính của Player (Duy nhất trên Scene, có Tag "MainCamera")
            GameObject camObj = new GameObject("Main Camera");
            camObj.layer = LayerMask.NameToLayer("Ignore Raycast");
            camObj.transform.SetParent(player.transform);
            camObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            camObj.transform.localRotation = Quaternion.identity;
            
            Camera cam = camObj.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 100f;
            cam.fieldOfView = 75f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.002f, 0.003f, 0.005f);

            camObj.AddComponent<AudioListener>();

            // Đèn pin gắn trên người
            GameObject flashObj = new GameObject("Player_Flashlight");
            flashObj.transform.SetParent(camObj.transform);
            flashObj.transform.localPosition = new Vector3(0.2f, -0.1f, 0.2f);
            Light flashLight = flashObj.AddComponent<Light>();
            flashLight.type = LightType.Spot;
            flashLight.range = 30f;
            flashLight.spotAngle = 48f;
            flashLight.intensity = 2800f;
            flashLight.color = new Color(0.95f, 0.98f, 1.0f);
            flashLight.enabled = false;

            // Socket cầm đồ vật
            GameObject holdSocket = new GameObject("Hold_Item_Socket");
            holdSocket.transform.SetParent(camObj.transform);
            holdSocket.transform.localPosition = new Vector3(0.3f, -0.25f, 0.6f);

            // Khẩu Súng Lục 9mm trên tay nhân vật (FPS Weapon Model từ Pistol_9mm.obj chuẩn 1:1)
            GameObject pistolInHand = new GameObject("Pistol_9mm_FPS");
            pistolInHand.layer = LayerMask.NameToLayer("Ignore Raycast");
            pistolInHand.transform.SetParent(camObj.transform);
            pistolInHand.transform.localPosition = new Vector3(0.24f, -0.22f, 0.48f);
            pistolInHand.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // Xoay 180 độ để nòng súng chĩa thẳng về phía trước
            pistolInHand.transform.localScale = Vector3.one;

            GameObject pistolFpsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Pistol_9mm.obj");
            if (pistolFpsPrefab != null)
            {
                GameObject gunVisual = Object.Instantiate(pistolFpsPrefab, pistolInHand.transform);
                gunVisual.name = "Pistol_Mesh";
                gunVisual.transform.localPosition = Vector3.zero;
                gunVisual.transform.localRotation = Quaternion.identity;
                gunVisual.transform.localScale = Vector3.one;
                SetLayerRecursively(gunVisual, LayerMask.NameToLayer("Ignore Raycast"));
                RemoveCollidersRecursively(gunVisual);
            }
            else
            {
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placeholder.name = "Pistol_Placeholder";
                placeholder.layer = LayerMask.NameToLayer("Ignore Raycast");
                placeholder.transform.SetParent(pistolInHand.transform);
                placeholder.transform.localPosition = Vector3.zero;
                placeholder.transform.localScale = new Vector3(0.05f, 0.09f, 0.28f);
                placeholder.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_PistolFPS", new Color(0.14f, 0.15f, 0.18f), 0.85f);
                Object.DestroyImmediate(placeholder.GetComponent<Collider>());
            }

            // Hiệu ứng ánh lửa đầu nòng súng (Muzzle Flash Point Light & Emissive Star)
            GameObject muzzleFlashObj = new GameObject("Pistol_Muzzle_Flash_Light");
            muzzleFlashObj.transform.SetParent(pistolInHand.transform);
            muzzleFlashObj.transform.localPosition = new Vector3(0f, 0.09f, -0.16f);
            Light muzzleLight = muzzleFlashObj.AddComponent<Light>();
            muzzleLight.type = LightType.Point;
            muzzleLight.color = new Color(1.0f, 0.82f, 0.35f);
            muzzleLight.intensity = 1800f;
            muzzleLight.range = 5.0f;
            muzzleLight.enabled = false;

            GameObject flashVisualObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            flashVisualObj.name = "Pistol_Muzzle_Flash_Visual";
            flashVisualObj.layer = LayerMask.NameToLayer("Ignore Raycast");
            flashVisualObj.transform.SetParent(muzzleFlashObj.transform);
            flashVisualObj.transform.localPosition = Vector3.zero;
            flashVisualObj.transform.localScale = Vector3.one * 0.16f;
            flashVisualObj.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_MuzzleFlash", Color.yellow, 0.1f, new Color(1f, 0.85f, 0.2f) * 4.5f);
            Object.DestroyImmediate(flashVisualObj.GetComponent<Collider>());
            flashVisualObj.SetActive(false);

            pistolInHand.SetActive(false);

            ShotgunController shotgunCtrl = player.AddComponent<ShotgunController>();

            // Cây Lau Nhà trên tay nhân vật (FPS Cleaning Mop Model từ Store_Mop.obj)
            GameObject mopInHand = new GameObject("FPS_Cleaning_Mop_Model");
            mopInHand.layer = LayerMask.NameToLayer("Ignore Raycast");
            mopInHand.transform.SetParent(camObj.transform);
            mopInHand.transform.localPosition = new Vector3(0.24f, -0.28f, 0.45f);
            mopInHand.transform.localRotation = Quaternion.Euler(42f, -16f, -10f); // Chĩa cán chổi xuôi xuống sàn trước mặt
            mopInHand.transform.localScale = Vector3.one * 0.70f; // Scale tỉ lệ FPS hiển thị vừa mắt

            GameObject fpsMopPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Store_Mop.obj");
            if (fpsMopPrefab != null)
            {
                GameObject mopVisual = Object.Instantiate(fpsMopPrefab, mopInHand.transform);
                mopVisual.name = "Mop_Mesh";
                mopVisual.transform.localPosition = new Vector3(0f, -0.45f, 0f);
                mopVisual.transform.localRotation = Quaternion.identity;
                mopVisual.transform.localScale = Vector3.one;
                SetLayerRecursively(mopVisual, LayerMask.NameToLayer("Ignore Raycast"));
                RemoveCollidersRecursively(mopVisual);
            }
            else
            {
                GameObject fpsMopHandle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                fpsMopHandle.name = "FPS_Mop_Handle";
                fpsMopHandle.layer = LayerMask.NameToLayer("Ignore Raycast");
                fpsMopHandle.transform.SetParent(mopInHand.transform);
                fpsMopHandle.transform.localPosition = Vector3.zero;
                fpsMopHandle.transform.localScale = new Vector3(0.025f, 0.42f, 0.025f);
                fpsMopHandle.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_MopHandleMetal", new Color(0.75f, 0.75f, 0.78f), 0.7f);
                Object.DestroyImmediate(fpsMopHandle.GetComponent<Collider>());
            }
            mopInHand.SetActive(false);

            // Máy Quét Mã Vạch trên tay nhân vật (FPS Barcode Scanner Tool từ Barcode_Scanner.obj)
            GameObject scannerInHand = new GameObject("FPS_Barcode_Scanner_Model");
            scannerInHand.layer = LayerMask.NameToLayer("Ignore Raycast");
            scannerInHand.transform.SetParent(camObj.transform);
            scannerInHand.transform.localPosition = new Vector3(0.20f, -0.20f, 0.42f);
            scannerInHand.transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
            scannerInHand.transform.localScale = Vector3.one;

            GameObject fpsScannerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Barcode_Scanner.obj");
            if (fpsScannerPrefab != null)
            {
                GameObject scanVisual = Object.Instantiate(fpsScannerPrefab, scannerInHand.transform);
                scanVisual.name = "Scanner_Mesh";
                scanVisual.transform.localPosition = Vector3.zero;
                scanVisual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                scanVisual.transform.localScale = Vector3.one;
                SetLayerRecursively(scanVisual, LayerMask.NameToLayer("Ignore Raycast"));
                RemoveCollidersRecursively(scanVisual);
            }
            else
            {
                GameObject scanBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
                scanBody.name = "Scanner_Body";
                scanBody.layer = LayerMask.NameToLayer("Ignore Raycast");
                scanBody.transform.SetParent(scannerInHand.transform);
                scanBody.transform.localPosition = new Vector3(0f, 0.04f, 0f);
                scanBody.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                scanBody.transform.localScale = new Vector3(0.06f, 0.08f, 0.16f);
                scanBody.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_ScannerBody", new Color(0.12f, 0.13f, 0.15f), 0.7f);
                Object.DestroyImmediate(scanBody.GetComponent<Collider>());
            }

            // Điểm xuất phát tia laser (Laser Emitter Point ở đầu cửa sổ quét)
            GameObject laserEmitterPt = new GameObject("Laser_Emitter_Point");
            laserEmitterPt.transform.SetParent(scannerInHand.transform);
            laserEmitterPt.transform.localPosition = new Vector3(0f, 0.16f, 0.10f);
            laserEmitterPt.transform.localRotation = Quaternion.identity;

            // LineRenderer vẽ tia laser đỏ
            LineRenderer laserLR = scannerInHand.AddComponent<LineRenderer>();
            laserLR.material = CreateColoredMaterial("Mat_RedLaserLine", Color.red, 0.5f, Color.red * 3.0f);
            laserLR.startWidth = 0.005f;
            laserLR.endWidth = 0.005f;
            laserLR.positionCount = 2;
            laserLR.useWorldSpace = true;
            laserLR.enabled = false;

            BarcodeScanner fpsScannerComp = scannerInHand.AddComponent<BarcodeScanner>();
            SerializedObject fpsScanSO = new SerializedObject(fpsScannerComp);
            SafeSetBool(fpsScanSO, "_isFPSTool", true);
            SafeSetObject(fpsScanSO, "_laserEmitter", laserEmitterPt.transform);
            SafeSetObject(fpsScanSO, "_laserLine", laserLR);
            SafeSetFloat(fpsScanSO, "_scanRange", 3.5f);
            SafeSetObject(fpsScanSO, "_beepSuccessSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Barcode_Scan_Beep.wav"));
            fpsScanSO.ApplyModifiedProperties();

            scannerInHand.SetActive(false);

            SerializedObject invSO = new SerializedObject(inventory);
            SafeSetObject(invSO, "_fpsMopModel", mopInHand);
            SafeSetObject(invSO, "_fpsScannerModel", scannerInHand);
            SafeSetObject(invSO, "_shotgunController", shotgunCtrl);
            SafeSetObject(invSO, "_playerInteraction", interaction);
            invSO.ApplyModifiedProperties();

            // Gán tham chiếu chuẩn cho các component
            SerializedObject moveSO = new SerializedObject(movement);
            SafeSetObject(moveSO, "_cameraContainer", camObj.transform);
            moveSO.ApplyModifiedProperties();

            SerializedObject shotSO = new SerializedObject(shotgunCtrl);
            SafeSetObject(shotSO, "_weaponModel", pistolInHand.transform);
            SafeSetObject(shotSO, "_firePoint", camObj.transform);
            SafeSetObject(shotSO, "_muzzleFlashLight", muzzleLight);
            SafeSetObject(shotSO, "_muzzleFlashVisual", flashVisualObj);
            SafeSetInt(shotSO, "_maxAmmo", 5);
            SafeSetInt(shotSO, "_reserveAmmo", 15);
            SafeSetInt(shotSO, "_pelletCount", 1);
            SafeSetFloat(shotSO, "_damagePerPellet", 35f);
            SafeSetFloat(shotSO, "_spreadAngle", 0.5f);
            SafeSetFloat(shotSO, "_reloadDuration", 1.2f);
            SafeSetVector3(shotSO, "_recoilKickback", new Vector3(-4.5f, 0.4f, 0f));
            SafeSetObject(shotSO, "_fireSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Pistol_Fire.wav"));
            SafeSetObject(shotSO, "_reloadSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Pistol_Reload.wav"));
            SafeSetObject(shotSO, "_emptyClickSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Gun_Empty_Click.wav"));
            shotSO.ApplyModifiedProperties();

            SerializedObject lookSO = new SerializedObject(look);
            SafeSetObject(lookSO, "_cameraTransform", camObj.transform);
            lookSO.ApplyModifiedProperties();

            SerializedObject interactSO = new SerializedObject(interaction);
            SafeSetObject(interactSO, "_cameraTransform", camObj.transform);
            SafeSetObject(interactSO, "_itemHoldSocket", holdSocket.transform);
            SafeSetInt(interactSO, "_interactionLayers", ~0);
            SafeSetFloat(interactSO, "_maxInteractionDistance", 4.5f);
            SafeSetFloat(interactSO, "_sphereCastRadius", 0.35f);
            interactSO.ApplyModifiedProperties();

            SerializedObject flashSO = new SerializedObject(flashlight);
            SafeSetObject(flashSO, "_flashlightLight", flashLight);
            SafeSetObject(flashSO, "_clickSound", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Flashlight_Click.wav"));
            flashSO.ApplyModifiedProperties();

            // Ghi-đông xe máy góc nhìn thứ nhất (FPS Motorbike Handlebars)
            GameObject bikeHandlebars = new GameObject("Motorbike_Handlebars_FPS");
            bikeHandlebars.layer = LayerMask.NameToLayer("Ignore Raycast");
            bikeHandlebars.transform.SetParent(player.transform);
            bikeHandlebars.transform.localPosition = new Vector3(0f, 0.82f, 0.48f);
            bikeHandlebars.transform.localRotation = Quaternion.identity;

            Material bikeMetalMat = CreateColoredMaterial("Mat_BikeMetal", new Color(0.18f, 0.19f, 0.22f), 0.7f);
            Material bikeGripMat = CreateColoredMaterial("Mat_BikeGrip", new Color(0.08f, 0.08f, 0.08f), 0.2f);
            Material bikeChromeMat = CreateColoredMaterial("Mat_BikeChrome", new Color(0.85f, 0.85f, 0.88f), 0.9f);

            // Thanh ngang ghi đông
            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bar.name = "Handlebar_Bar";
            bar.layer = LayerMask.NameToLayer("Ignore Raycast");
            bar.transform.SetParent(bikeHandlebars.transform);
            bar.transform.localPosition = Vector3.zero;
            bar.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            bar.transform.localScale = new Vector3(0.035f, 0.38f, 0.035f);
            bar.GetComponent<Renderer>().sharedMaterial = bikeMetalMat;
            Object.DestroyImmediate(bar.GetComponent<Collider>());

            // Tay nắm cao su trái và phải
            GameObject gripL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gripL.name = "Grip_Left";
            gripL.layer = LayerMask.NameToLayer("Ignore Raycast");
            gripL.transform.SetParent(bikeHandlebars.transform);
            gripL.transform.localPosition = new Vector3(-0.35f, 0f, 0f);
            gripL.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            gripL.transform.localScale = new Vector3(0.05f, 0.08f, 0.05f);
            gripL.GetComponent<Renderer>().sharedMaterial = bikeGripMat;
            Object.DestroyImmediate(gripL.GetComponent<Collider>());

            GameObject gripR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gripR.name = "Grip_Right";
            gripR.layer = LayerMask.NameToLayer("Ignore Raycast");
            gripR.transform.SetParent(bikeHandlebars.transform);
            gripR.transform.localPosition = new Vector3(0.35f, 0f, 0f);
            gripR.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            gripR.transform.localScale = new Vector3(0.05f, 0.08f, 0.05f);
            gripR.GetComponent<Renderer>().sharedMaterial = bikeGripMat;
            Object.DestroyImmediate(gripR.GetComponent<Collider>());

            // Tay phanh kim loại
            GameObject leverL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leverL.name = "Brake_Lever_Left";
            leverL.layer = LayerMask.NameToLayer("Ignore Raycast");
            leverL.transform.SetParent(bikeHandlebars.transform);
            leverL.transform.localPosition = new Vector3(-0.3f, -0.02f, 0.08f);
            leverL.transform.localRotation = Quaternion.Euler(0f, -12f, 0f);
            leverL.transform.localScale = new Vector3(0.12f, 0.015f, 0.02f);
            leverL.GetComponent<Renderer>().sharedMaterial = bikeChromeMat;
            Object.DestroyImmediate(leverL.GetComponent<Collider>());

            GameObject leverR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leverR.name = "Brake_Lever_Right";
            leverR.layer = LayerMask.NameToLayer("Ignore Raycast");
            leverR.transform.SetParent(bikeHandlebars.transform);
            leverR.transform.localPosition = new Vector3(0.3f, -0.02f, 0.08f);
            leverR.transform.localRotation = Quaternion.Euler(0f, 12f, 0f);
            leverR.transform.localScale = new Vector3(0.12f, 0.015f, 0.02f);
            leverR.GetComponent<Renderer>().sharedMaterial = bikeChromeMat;
            Object.DestroyImmediate(leverR.GetComponent<Collider>());

            // Cụm đồng hồ công tơ mét phát sáng
            GameObject speedo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            speedo.name = "Speedometer_Dial";
            speedo.layer = LayerMask.NameToLayer("Ignore Raycast");
            speedo.transform.SetParent(bikeHandlebars.transform);
            speedo.transform.localPosition = new Vector3(0f, 0.08f, 0.04f);
            speedo.transform.localRotation = Quaternion.Euler(-35f, 0f, 0f);
            speedo.transform.localScale = new Vector3(0.12f, 0.02f, 0.12f);
            speedo.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_SpeedoGlow", new Color(0.1f, 0.8f, 0.6f), 0.3f, new Color(0.1f, 0.9f, 0.7f) * 1.5f);
            Object.DestroyImmediate(speedo.GetComponent<Collider>());

            // Gương chiếu hậu 2 bên
            GameObject mirrorL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mirrorL.name = "Mirror_Left";
            mirrorL.layer = LayerMask.NameToLayer("Ignore Raycast");
            mirrorL.transform.SetParent(bikeHandlebars.transform);
            mirrorL.transform.localPosition = new Vector3(-0.38f, 0.14f, 0.06f);
            mirrorL.transform.localRotation = Quaternion.Euler(15f, 25f, 0f);
            mirrorL.transform.localScale = new Vector3(0.12f, 0.08f, 0.02f);
            mirrorL.GetComponent<Renderer>().sharedMaterial = bikeChromeMat;
            Object.DestroyImmediate(mirrorL.GetComponent<Collider>());

            GameObject mirrorR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mirrorR.name = "Mirror_Right";
            mirrorR.layer = LayerMask.NameToLayer("Ignore Raycast");
            mirrorR.transform.SetParent(bikeHandlebars.transform);
            mirrorR.transform.localPosition = new Vector3(0.38f, 0.14f, 0.06f);
            mirrorR.transform.localRotation = Quaternion.Euler(15f, -25f, 0f);
            mirrorR.transform.localScale = new Vector3(0.12f, 0.08f, 0.02f);
            mirrorR.GetComponent<Renderer>().sharedMaterial = bikeChromeMat;
            Object.DestroyImmediate(mirrorR.GetComponent<Collider>());

            // Đèn pha xe máy hướng thẳng xuống lòng đường phía trước xe, không rọi thẳng mặt người chơi
            GameObject bikeHeadlightObj = new GameObject("Motorbike_Headlight");
            bikeHeadlightObj.transform.SetParent(player.transform);
            bikeHeadlightObj.transform.localPosition = new Vector3(0f, 0.65f, 0.95f);
            bikeHeadlightObj.transform.localRotation = Quaternion.Euler(12f, 0f, 0f);

            Light bikeLight = bikeHeadlightObj.AddComponent<Light>();
            bikeLight.type = LightType.Spot;
            bikeLight.range = 35f;
            bikeLight.spotAngle = 38f;
            bikeLight.innerSpotAngle = 20f;
            bikeLight.intensity = 950f;
            bikeLight.color = new Color(1.0f, 0.95f, 0.85f);
            bikeLight.enabled = false;

            // Gắn và thiết lập MotorbikeRideController
            MotorbikeRideController bikeRideCtrl = player.AddComponent<MotorbikeRideController>();
            SerializedObject bikeSO = new SerializedObject(bikeRideCtrl);
            SafeSetObject(bikeSO, "_bikeHandlebarsFPS", bikeHandlebars);
            SafeSetObject(bikeSO, "_bikeHeadlight", bikeLight);
            SafeSetObject(bikeSO, "_parkedBikeWorldModel", parkedBike);
            SafeSetFloat(bikeSO, "_maxSpeed", 11.0f);
            SafeSetFloat(bikeSO, "_acceleration", 6.0f);
            SafeSetFloat(bikeSO, "_roadWidthLimit", 2.4f);
            SafeSetFloat(bikeSO, "_arrivalZ", -8.5f);
            SafeSetVector3(bikeSO, "_parkedPosition", new Vector3(-2.2f, 0.1f, -7.5f));
            SafeSetObject(bikeSO, "_engineRevClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Motorbike_Engine_Loop.wav"));
            bikeSO.ApplyModifiedProperties();

            bikeHandlebars.SetActive(false);

            return player;
        }

        private static void CreateTutorialCanvas(GameObject player, bool startFromHome = true, Transform homePlayerSpawn = null)
        {
            // Đảm bảo Scene có EventSystem và InputSystemUIInputModule cho New Input System
            UnityEngine.EventSystems.EventSystem es = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (es == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            GameObject canvasObj = new GameObject("Tutorial_HUD_Canvas");
            canvasObj.transform.position = Vector3.zero;
            canvasObj.transform.rotation = Quaternion.identity;
            canvasObj.transform.localScale = Vector3.one;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            TutorialHUD hud = canvasObj.AddComponent<TutorialHUD>();

            // Helper thiết lập Transform phẳng cho UI
            System.Action<GameObject> ResetUITransform = (go) =>
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            };

            // 1. Objective Panel (Góc trên bên trái - Kích thước chữ to gấp đôi, panel đen tự động co dãn)
            GameObject objPanel = new GameObject("Objective_Badge");
            objPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(objPanel);
            RectTransform objPanelRect = objPanel.AddComponent<RectTransform>();
            objPanelRect.anchorMin = new Vector2(0f, 1f);
            objPanelRect.anchorMax = new Vector2(0f, 1f);
            objPanelRect.pivot = new Vector2(0f, 1f);
            objPanelRect.anchoredPosition = new Vector2(24f, -20f);
            objPanelRect.sizeDelta = new Vector2(760f, 58f);

            Image objBg = objPanel.AddComponent<Image>();
            objBg.color = new Color(0.04f, 0.05f, 0.08f, 0.88f);

            GameObject objTextObj = new GameObject("Objective_Text");
            objTextObj.transform.SetParent(objPanel.transform, false);
            ResetUITransform(objTextObj);
            TextMeshProUGUI objTMP = objTextObj.AddComponent<TextMeshProUGUI>();
            objTMP.rectTransform.anchorMin = Vector2.zero;
            objTMP.rectTransform.anchorMax = Vector2.one;
            objTMP.rectTransform.sizeDelta = new Vector2(-24f, 0f);
            objTMP.rectTransform.anchoredPosition = Vector2.zero;
            objTMP.alignment = TextAlignmentOptions.MidlineLeft;
            objTMP.enableAutoSizing = true;
            objTMP.fontSizeMin = 18f;
            objTMP.fontSizeMax = 24f;
            objTMP.fontStyle = FontStyles.Bold;
            objTMP.color = new Color(1.0f, 0.85f, 0.15f);
            objTMP.text = startFromHome ? "MỤC TIÊU: Đọc giấy nợ tiền phòng & Tin nhắn việc làm trên bàn" : "MỤC TIÊU: Dọn dẹp đồ rơi vãi trên sàn (0/3)";

            // 2. Clock & Money Display (Góc trên bên phải - Kích thước gấp đôi)
            GameObject clockPanel = new GameObject("Clock_Money_Badge");
            clockPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(clockPanel);
            RectTransform clockRect = clockPanel.AddComponent<RectTransform>();
            clockRect.anchorMin = new Vector2(1f, 1f);
            clockRect.anchorMax = new Vector2(1f, 1f);
            clockRect.pivot = new Vector2(1f, 1f);
            clockRect.anchoredPosition = new Vector2(-92f, -20f);
            clockRect.sizeDelta = new Vector2(400f, 58f);

            Image clockBg = clockPanel.AddComponent<Image>();
            clockBg.color = new Color(0.04f, 0.05f, 0.08f, 0.88f);

            GameObject clockTextObj = new GameObject("Clock_Money_Text");
            clockTextObj.transform.SetParent(clockPanel.transform, false);
            ResetUITransform(clockTextObj);
            TextMeshProUGUI clockTMP = clockTextObj.AddComponent<TextMeshProUGUI>();
            clockTMP.rectTransform.anchorMin = Vector2.zero;
            clockTMP.rectTransform.anchorMax = Vector2.one;
            clockTMP.rectTransform.sizeDelta = Vector2.zero;
            clockTMP.alignment = TextAlignmentOptions.Center;
            clockTMP.fontSize = 24f;
            clockTMP.fontStyle = FontStyles.Bold;
            clockTMP.color = new Color(0.4f, 1.0f, 0.6f);
            clockTMP.text = startFromHome ? "11:30 PM  |  $150" : "00:00 AM  |  $150";

            // Nút Bánh Răng Tạm Dừng / Cài Đặt Trên HUD (HUD Pause Button)
            GameObject hudPauseBtnObj = new GameObject("HUD_Pause_Gear_Button");
            hudPauseBtnObj.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(hudPauseBtnObj);
            RectTransform pauseBtnRt = hudPauseBtnObj.AddComponent<RectTransform>();
            pauseBtnRt.anchorMin = new Vector2(1f, 1f);
            pauseBtnRt.anchorMax = new Vector2(1f, 1f);
            pauseBtnRt.pivot = new Vector2(1f, 1f);
            pauseBtnRt.anchoredPosition = new Vector2(-24f, -20f);
            pauseBtnRt.sizeDelta = new Vector2(58f, 58f);

            Image pauseBtnBg = hudPauseBtnObj.AddComponent<Image>();
            pauseBtnBg.color = new Color(0.10f, 0.12f, 0.18f, 0.92f);
            Button hudPauseBtn = hudPauseBtnObj.AddComponent<Button>();

            GameObject gearTxtObj = new GameObject("Gear_Icon_Text");
            gearTxtObj.transform.SetParent(hudPauseBtnObj.transform, false);
            ResetUITransform(gearTxtObj);
            TextMeshProUGUI gearTMP = gearTxtObj.AddComponent<TextMeshProUGUI>();
            gearTMP.rectTransform.anchorMin = Vector2.zero;
            gearTMP.rectTransform.anchorMax = Vector2.one;
            gearTMP.rectTransform.sizeDelta = Vector2.zero;
            gearTMP.alignment = TextAlignmentOptions.Center;
            gearTMP.fontSize = 28f;
            gearTMP.fontStyle = FontStyles.Bold;
            gearTMP.color = new Color(1.0f, 0.85f, 0.15f);
            gearTMP.text = "[*]";

            // 3. Crosshair Dot (Tâm ngắm giữa màn hình - Nhỏ gọn 4px)
            GameObject crosshairObj = new GameObject("Crosshair_Dot");
            crosshairObj.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(crosshairObj);
            RectTransform crossRect = crosshairObj.AddComponent<RectTransform>();
            crossRect.anchorMin = new Vector2(0.5f, 0.5f);
            crossRect.anchorMax = new Vector2(0.5f, 0.5f);
            crossRect.pivot = new Vector2(0.5f, 0.5f);
            crossRect.anchoredPosition = Vector2.zero;
            crossRect.sizeDelta = new Vector2(4f, 4f);
            Image crossImg = crosshairObj.AddComponent<Image>();
            crossImg.color = new Color(1f, 1f, 1f, 0.85f);

            // 4. Interaction Prompt Text (Gợi ý tương tác [E] - Ngay dưới tâm ngắm, gấp đôi kích thước)
            GameObject promptPanel = new GameObject("Prompt_Badge");
            promptPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(promptPanel);
            RectTransform promptPanelRect = promptPanel.AddComponent<RectTransform>();
            promptPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            promptPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            promptPanelRect.pivot = new Vector2(0.5f, 0.5f);
            promptPanelRect.anchoredPosition = new Vector2(0f, -70f);
            promptPanelRect.sizeDelta = new Vector2(720f, 68f);

            Image promptBg = promptPanel.AddComponent<Image>();
            promptBg.color = new Color(0.04f, 0.05f, 0.08f, 0.92f);

            GameObject promptTextObj = new GameObject("Interaction_Prompt_Text");
            promptTextObj.transform.SetParent(promptPanel.transform, false);
            ResetUITransform(promptTextObj);
            TextMeshProUGUI promptTMP = promptTextObj.AddComponent<TextMeshProUGUI>();
            promptTMP.rectTransform.anchorMin = Vector2.zero;
            promptTMP.rectTransform.anchorMax = Vector2.one;
            promptTMP.rectTransform.sizeDelta = Vector2.zero;
            promptTMP.alignment = TextAlignmentOptions.Center;
            promptTMP.fontSize = 24f;
            promptTMP.fontStyle = FontStyles.Bold;
            promptTMP.color = Color.white;
            promptTMP.text = string.Empty;

            promptPanel.SetActive(false);

            // 5. Kho đồ nhanh 3 Slot Động (310x96) - Món nhặt trước ở ô 1, các món tiếp theo lần lượt
            GameObject invPanel = new GameObject("Quick_Inventory_Bar");
            invPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(invPanel);
            RectTransform invRect = invPanel.AddComponent<RectTransform>();
            invRect.anchorMin = new Vector2(0.5f, 0f);
            invRect.anchorMax = new Vector2(0.5f, 0f);
            invRect.pivot = new Vector2(0.5f, 0f);
            invRect.anchoredPosition = new Vector2(0f, 16f);
            invRect.sizeDelta = new Vector2(310f, 96f);

            // Slot 1 (Ô vuông 90x90 bên trái)
            GameObject slot1Obj = new GameObject("Slot_1_Square");
            slot1Obj.transform.SetParent(invPanel.transform, false);
            ResetUITransform(slot1Obj);
            RectTransform slot1Rect = slot1Obj.AddComponent<RectTransform>();
            slot1Rect.anchorMin = new Vector2(0.5f, 0.5f);
            slot1Rect.anchorMax = new Vector2(0.5f, 0.5f);
            slot1Rect.pivot = new Vector2(0.5f, 0.5f);
            slot1Rect.anchoredPosition = new Vector2(-105f, 0f);
            slot1Rect.sizeDelta = new Vector2(90f, 90f);
            Image slot1Bg = slot1Obj.AddComponent<Image>();
            slot1Bg.color = new Color(0.12f, 0.14f, 0.18f, 0.75f);

            GameObject slot1TextObj = new GameObject("Slot_1_Text");
            slot1TextObj.transform.SetParent(slot1Obj.transform, false);
            ResetUITransform(slot1TextObj);
            TextMeshProUGUI slot1TMP = slot1TextObj.AddComponent<TextMeshProUGUI>();
            slot1TMP.rectTransform.anchorMin = Vector2.zero;
            slot1TMP.rectTransform.anchorMax = Vector2.one;
            slot1TMP.rectTransform.sizeDelta = Vector2.zero;
            slot1TMP.alignment = TextAlignmentOptions.Center;
            slot1TMP.fontSize = 17f;
            slot1TMP.fontStyle = FontStyles.Bold;
            slot1TMP.color = Color.white;
            slot1TMP.text = string.Empty;

            // Slot 2 (Ô vuông 90x90 ở giữa)
            GameObject slot2Obj = new GameObject("Slot_2_Square");
            slot2Obj.transform.SetParent(invPanel.transform, false);
            ResetUITransform(slot2Obj);
            RectTransform slot2Rect = slot2Obj.AddComponent<RectTransform>();
            slot2Rect.anchorMin = new Vector2(0.5f, 0.5f);
            slot2Rect.anchorMax = new Vector2(0.5f, 0.5f);
            slot2Rect.pivot = new Vector2(0.5f, 0.5f);
            slot2Rect.anchoredPosition = new Vector2(0f, 0f);
            slot2Rect.sizeDelta = new Vector2(90f, 90f);
            Image slot2Bg = slot2Obj.AddComponent<Image>();
            slot2Bg.color = new Color(0.12f, 0.14f, 0.18f, 0.75f);

            GameObject slot2TextObj = new GameObject("Slot_2_Text");
            slot2TextObj.transform.SetParent(slot2Obj.transform, false);
            ResetUITransform(slot2TextObj);
            TextMeshProUGUI slot2TMP = slot2TextObj.AddComponent<TextMeshProUGUI>();
            slot2TMP.rectTransform.anchorMin = Vector2.zero;
            slot2TMP.rectTransform.anchorMax = Vector2.one;
            slot2TMP.rectTransform.sizeDelta = Vector2.zero;
            slot2TMP.alignment = TextAlignmentOptions.Center;
            slot2TMP.fontSize = 17f;
            slot2TMP.fontStyle = FontStyles.Bold;
            slot2TMP.color = Color.white;
            slot2TMP.text = string.Empty;

            // Slot 3 (Ô vuông 90x90 bên phải)
            GameObject slot3Obj = new GameObject("Slot_3_Square");
            slot3Obj.transform.SetParent(invPanel.transform, false);
            ResetUITransform(slot3Obj);
            RectTransform slot3Rect = slot3Obj.AddComponent<RectTransform>();
            slot3Rect.anchorMin = new Vector2(0.5f, 0.5f);
            slot3Rect.anchorMax = new Vector2(0.5f, 0.5f);
            slot3Rect.pivot = new Vector2(0.5f, 0.5f);
            slot3Rect.anchoredPosition = new Vector2(105f, 0f);
            slot3Rect.sizeDelta = new Vector2(90f, 90f);
            Image slot3Bg = slot3Obj.AddComponent<Image>();
            slot3Bg.color = new Color(0.12f, 0.14f, 0.18f, 0.75f);

            GameObject slot3TextObj = new GameObject("Slot_3_Text");
            slot3TextObj.transform.SetParent(slot3Obj.transform, false);
            ResetUITransform(slot3TextObj);
            TextMeshProUGUI slot3TMP = slot3TextObj.AddComponent<TextMeshProUGUI>();
            slot3TMP.rectTransform.anchorMin = Vector2.zero;
            slot3TMP.rectTransform.anchorMax = Vector2.one;
            slot3TMP.rectTransform.sizeDelta = Vector2.zero;
            slot3TMP.alignment = TextAlignmentOptions.Center;
            slot3TMP.fontSize = 17f;
            slot3TMP.fontStyle = FontStyles.Bold;
            slot3TMP.color = Color.white;
            slot3TMP.text = string.Empty;

            // 6. Subtitle & Reading Dialogue Banner (ĐẶT Ở PHẦN DƯỚI MÀN HÌNH - BANNER GẤP ĐÔI RỘNG RÃI)
            GameObject subPanel = new GameObject("Subtitle_Reading_Panel");
            subPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(subPanel);
            RectTransform subRect = subPanel.AddComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0.5f, 0f);
            subRect.anchorMax = new Vector2(0.5f, 0f);
            subRect.pivot = new Vector2(0.5f, 0f);
            subRect.anchoredPosition = new Vector2(0f, 105f);
            subRect.sizeDelta = new Vector2(1320f, 240f);

            Image subBg = subPanel.AddComponent<Image>();
            subBg.color = new Color(0.04f, 0.06f, 0.10f, 0.92f);

            // Tiêu đề / Người nói (Header)
            GameObject speakerTextObj = new GameObject("Subtitle_Speaker_Text");
            speakerTextObj.transform.SetParent(subPanel.transform, false);
            ResetUITransform(speakerTextObj);
            TextMeshProUGUI subSpeakerTMP = speakerTextObj.AddComponent<TextMeshProUGUI>();
            subSpeakerTMP.rectTransform.anchorMin = new Vector2(0f, 1f);
            subSpeakerTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            subSpeakerTMP.rectTransform.pivot = new Vector2(0.5f, 1f);
            subSpeakerTMP.rectTransform.anchoredPosition = new Vector2(0f, -14f);
            subSpeakerTMP.rectTransform.sizeDelta = new Vector2(-48f, 32f);
            subSpeakerTMP.alignment = TextAlignmentOptions.Center;
            subSpeakerTMP.fontSize = 26f;
            subSpeakerTMP.fontStyle = FontStyles.Bold;
            subSpeakerTMP.color = new Color(1.0f, 0.85f, 0.15f);
            subSpeakerTMP.text = "[ THÔNG BÁO ]";

            // Nội dung chi tiết (Content - Tự động co dãn chữ chống đè chữ)
            GameObject subTextObj = new GameObject("Subtitle_Content_Text");
            subTextObj.transform.SetParent(subPanel.transform, false);
            ResetUITransform(subTextObj);
            TextMeshProUGUI subContentTMP = subTextObj.AddComponent<TextMeshProUGUI>();
            subContentTMP.rectTransform.anchorMin = Vector2.zero;
            subContentTMP.rectTransform.anchorMax = Vector2.one;
            subContentTMP.rectTransform.sizeDelta = new Vector2(-54f, -76f);
            subContentTMP.rectTransform.anchoredPosition = new Vector2(0f, -4f);
            subContentTMP.alignment = TextAlignmentOptions.Center;
            subContentTMP.enableAutoSizing = true;
            subContentTMP.fontSizeMin = 18f;
            subContentTMP.fontSizeMax = 26f;
            subContentTMP.lineSpacing = 6f;
            subContentTMP.textWrappingMode = TextWrappingModes.Normal;
            subContentTMP.color = new Color(0.95f, 0.98f, 1.0f);
            subContentTMP.text = string.Empty;

            // Hướng dẫn nhấn phím [Space] để tiếp tục (Prompt)
            GameObject subPromptTextObj = new GameObject("Subtitle_Prompt_Text");
            subPromptTextObj.transform.SetParent(subPanel.transform, false);
            ResetUITransform(subPromptTextObj);
            TextMeshProUGUI subPromptTMP = subPromptTextObj.AddComponent<TextMeshProUGUI>();
            subPromptTMP.rectTransform.anchorMin = new Vector2(0f, 0f);
            subPromptTMP.rectTransform.anchorMax = new Vector2(1f, 0f);
            subPromptTMP.rectTransform.pivot = new Vector2(0.5f, 0f);
            subPromptTMP.rectTransform.anchoredPosition = new Vector2(0f, 12f);
            subPromptTMP.rectTransform.sizeDelta = new Vector2(-48f, 28f);
            subPromptTMP.alignment = TextAlignmentOptions.Center;
            subPromptTMP.fontSize = 20f;
            subPromptTMP.fontStyle = FontStyles.Bold;
            subPromptTMP.color = new Color(1.0f, 0.85f, 0.15f);
            subPromptTMP.text = "<color=#FFE600>[Space] Tiếp tục >></color>";

            subPanel.SetActive(false);

            // 7. Ammo Badge
            GameObject ammoPanel = new GameObject("Ammo_Badge");
            ammoPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(ammoPanel);
            RectTransform ammoRect = ammoPanel.AddComponent<RectTransform>();
            ammoRect.anchorMin = new Vector2(1f, 0f);
            ammoRect.anchorMax = new Vector2(1f, 0f);
            ammoRect.pivot = new Vector2(1f, 0f);
            ammoRect.anchoredPosition = new Vector2(-24f, 16f);
            ammoRect.sizeDelta = new Vector2(160f, 54f);

            Image ammoBg = ammoPanel.AddComponent<Image>();
            ammoBg.color = new Color(0.25f, 0.03f, 0.03f, 0.88f);

            GameObject ammoTextObj = new GameObject("Ammo_Text");
            ammoTextObj.transform.SetParent(ammoPanel.transform, false);
            ResetUITransform(ammoTextObj);
            TextMeshProUGUI ammoTMP = ammoTextObj.AddComponent<TextMeshProUGUI>();
            ammoTMP.rectTransform.anchorMin = Vector2.zero;
            ammoTMP.rectTransform.anchorMax = Vector2.one;
            ammoTMP.rectTransform.sizeDelta = Vector2.zero;
            ammoTMP.alignment = TextAlignmentOptions.Center;
            ammoTMP.fontSize = 28f;
            ammoTMP.fontStyle = FontStyles.Bold;
            ammoTMP.color = new Color(1f, 0.35f, 0.35f);
            ammoTMP.text = "2 / 2";

            ammoPanel.SetActive(false);

            // 8. Bảng Tổng Kết Ca Trực Lúc 06:00 AM (Mở rộng 980x580)
            GameObject sumPanel = new GameObject("Summary_Panel");
            sumPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(sumPanel);
            RectTransform sumRect = sumPanel.AddComponent<RectTransform>();
            sumRect.anchorMin = new Vector2(0.5f, 0.5f);
            sumRect.anchorMax = new Vector2(0.5f, 0.5f);
            sumRect.pivot = new Vector2(0.5f, 0.5f);
            sumRect.anchoredPosition = Vector2.zero;
            sumRect.sizeDelta = new Vector2(980f, 580f);

            Image sumBg = sumPanel.AddComponent<Image>();
            sumBg.color = new Color(0.04f, 0.05f, 0.08f, 0.96f);

            GameObject sumTextObj = new GameObject("Summary_Content_Text");
            sumTextObj.transform.SetParent(sumPanel.transform, false);
            ResetUITransform(sumTextObj);
            TextMeshProUGUI sumTMP = sumTextObj.AddComponent<TextMeshProUGUI>();
            sumTMP.rectTransform.anchorMin = Vector2.zero;
            sumTMP.rectTransform.anchorMax = Vector2.one;
            sumTMP.rectTransform.sizeDelta = new Vector2(-50f, -50f);
            sumTMP.alignment = TextAlignmentOptions.Center;
            sumTMP.fontSize = 24f;
            sumTMP.color = Color.white;
            sumTMP.text = string.Empty;

            sumPanel.SetActive(false);

            // 9. Thanh Thời gian Chuẩn bị Boss (Countdown Bar - Đỉnh màn hình, 860x65)
            GameObject bossCountdownPanel = new GameObject("Boss_Countdown_Badge");
            bossCountdownPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(bossCountdownPanel);
            RectTransform bossCdRect = bossCountdownPanel.AddComponent<RectTransform>();
            bossCdRect.anchorMin = new Vector2(0.5f, 1f);
            bossCdRect.anchorMax = new Vector2(0.5f, 1f);
            bossCdRect.pivot = new Vector2(0.5f, 1f);
            bossCdRect.anchoredPosition = new Vector2(0f, -75f);
            bossCdRect.sizeDelta = new Vector2(860f, 65f);

            Image bossCdBg = bossCountdownPanel.AddComponent<Image>();
            bossCdBg.color = new Color(0.12f, 0.03f, 0.03f, 0.92f);

            GameObject bossCdFillObj = new GameObject("Countdown_Fill");
            bossCdFillObj.transform.SetParent(bossCountdownPanel.transform, false);
            ResetUITransform(bossCdFillObj);
            Image bossCdFill = bossCdFillObj.AddComponent<Image>();
            bossCdFill.color = new Color(0.95f, 0.65f, 0.1f, 0.85f);
            bossCdFill.type = Image.Type.Filled;
            bossCdFill.fillMethod = Image.FillMethod.Horizontal;
            bossCdFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            bossCdFill.fillAmount = 1f;
            RectTransform bossCdFillRect = bossCdFillObj.GetComponent<RectTransform>();
            bossCdFillRect.anchorMin = Vector2.zero;
            bossCdFillRect.anchorMax = Vector2.one;
            bossCdFillRect.sizeDelta = Vector2.zero;

            GameObject bossCdTextObj = new GameObject("Countdown_Text");
            bossCdTextObj.transform.SetParent(bossCountdownPanel.transform, false);
            ResetUITransform(bossCdTextObj);
            TextMeshProUGUI bossCdTMP = bossCdTextObj.AddComponent<TextMeshProUGUI>();
            bossCdTMP.rectTransform.anchorMin = Vector2.zero;
            bossCdTMP.rectTransform.anchorMax = Vector2.one;
            bossCdTMP.rectTransform.sizeDelta = Vector2.zero;
            bossCdTMP.alignment = TextAlignmentOptions.Center;
            bossCdTMP.fontSize = 24f;
            bossCdTMP.fontStyle = FontStyles.Bold;
            bossCdTMP.color = Color.white;
            bossCdTMP.text = "[!] CẢNH BÁO: CON TRÙM ĐANG ĐẾN! CHUẨN BỊ VŨ KHÍ (30s)";

            bossCountdownPanel.SetActive(false);

            // 10. Thanh Máu Boss (Boss Health Bar Panel - Đỉnh màn hình, 920x85)
            GameObject bossHealthPanel = new GameObject("Boss_Health_Bar_Panel");
            bossHealthPanel.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(bossHealthPanel);
            RectTransform bossHpRect = bossHealthPanel.AddComponent<RectTransform>();
            bossHpRect.anchorMin = new Vector2(0.5f, 1f);
            bossHpRect.anchorMax = new Vector2(0.5f, 1f);
            bossHpRect.pivot = new Vector2(0.5f, 1f);
            bossHpRect.anchoredPosition = new Vector2(0f, -25f);
            bossHpRect.sizeDelta = new Vector2(920f, 85f);

            Image bossHpBg = bossHealthPanel.AddComponent<Image>();
            bossHpBg.color = new Color(0.08f, 0.08f, 0.1f, 0.95f);

            // Tên Boss
            GameObject bossNameObj = new GameObject("Boss_Name_Text");
            bossNameObj.transform.SetParent(bossHealthPanel.transform, false);
            ResetUITransform(bossNameObj);
            TextMeshProUGUI bossNameTMP = bossNameObj.AddComponent<TextMeshProUGUI>();
            bossNameTMP.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            bossNameTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            bossNameTMP.rectTransform.sizeDelta = new Vector2(-24f, 0f);
            bossNameTMP.rectTransform.anchoredPosition = Vector2.zero;
            bossNameTMP.alignment = TextAlignmentOptions.Center;
            bossNameTMP.fontSize = 22f;
            bossNameTMP.fontStyle = FontStyles.Bold;
            bossNameTMP.color = new Color(1f, 0.3f, 0.3f);
            bossNameTMP.text = "THE FLESH STALKER - TRÙM DỊ BIẾN ĐÊM 1";

            // Khung máu & Thanh máu
            GameObject bossHpBarFrame = new GameObject("Health_Bar_Frame");
            bossHpBarFrame.transform.SetParent(bossHealthPanel.transform, false);
            ResetUITransform(bossHpBarFrame);
            RectTransform bossHpBarRect = bossHpBarFrame.AddComponent<RectTransform>();
            bossHpBarRect.anchorMin = new Vector2(0f, 0f);
            bossHpBarRect.anchorMax = new Vector2(1f, 0.5f);
            bossHpBarRect.sizeDelta = new Vector2(-24f, -8f);
            bossHpBarRect.anchoredPosition = new Vector2(0f, 4f);
            Image hpFrameBg = bossHpBarFrame.AddComponent<Image>();
            hpFrameBg.color = new Color(0.2f, 0.05f, 0.05f, 0.9f);

            GameObject bossHpFillObj = new GameObject("Health_Fill");
            bossHpFillObj.transform.SetParent(bossHpBarFrame.transform, false);
            ResetUITransform(bossHpFillObj);
            Image bossHpFill = bossHpFillObj.AddComponent<Image>();
            bossHpFill.color = new Color(0.85f, 0.12f, 0.12f, 1.0f);
            bossHpFill.type = Image.Type.Filled;
            bossHpFill.fillMethod = Image.FillMethod.Horizontal;
            bossHpFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            bossHpFill.fillAmount = 1f;
            RectTransform bossHpFillRect = bossHpFillObj.GetComponent<RectTransform>();
            bossHpFillRect.anchorMin = Vector2.zero;
            bossHpFillRect.anchorMax = Vector2.one;
            bossHpFillRect.sizeDelta = Vector2.zero;

            GameObject bossHpTextObj = new GameObject("Health_TMP");
            bossHpTextObj.transform.SetParent(bossHpBarFrame.transform, false);
            ResetUITransform(bossHpTextObj);
            TextMeshProUGUI bossHpTMP = bossHpTextObj.AddComponent<TextMeshProUGUI>();
            bossHpTMP.rectTransform.anchorMin = Vector2.zero;
            bossHpTMP.rectTransform.anchorMax = Vector2.one;
            bossHpTMP.rectTransform.sizeDelta = Vector2.zero;
            bossHpTMP.alignment = TextAlignmentOptions.Center;
            bossHpTMP.fontSize = 18f;
            bossHpTMP.fontStyle = FontStyles.Bold;
            bossHpTMP.color = Color.white;
            bossHpTMP.text = "1000 / 1000 HP";

            bossHealthPanel.SetActive(false);

            // 11. Giao diện Máy tính quầy thu ngân (Store Desktop Terminal UI)
            CreateStoreComputerUI(canvasObj, ResetUITransform);

            // 12. Giao diện Cài đặt & Tạm dừng Menu (Pause & Settings Menu UI)
            CreatePauseSettingsUI(canvasObj, hudPauseBtn, ResetUITransform);

            // 13. Giao diện Menu Chính khi mở game (Main Menu UI)
            CreateMainMenuUI(canvasObj, player, homePlayerSpawn, ResetUITransform);

            // Gán đầy đủ tham chiếu vào TutorialHUD
            SerializedObject hudSO = new SerializedObject(hud);
            SafeSetObject(hudSO, "_objectiveTMP", objTMP);
            SafeSetObject(hudSO, "_clockMoneyPanel", clockPanel);
            SafeSetObject(hudSO, "_clockMoneyTMP", clockTMP);
            SafeSetObject(hudSO, "_crosshairObject", crosshairObj);
            SafeSetObject(hudSO, "_promptPanel", promptPanel);
            SafeSetObject(hudSO, "_interactionPromptTMP", promptTMP);
            SafeSetObject(hudSO, "_inventoryPanel", invPanel);
            SafeSetObject(hudSO, "_slot1Bg", slot1Bg);
            SafeSetObject(hudSO, "_slot2Bg", slot2Bg);
            SafeSetObject(hudSO, "_slot3Bg", slot3Bg);
            SafeSetObject(hudSO, "_slot1Text", slot1TMP);
            SafeSetObject(hudSO, "_slot2Text", slot2TMP);
            SafeSetObject(hudSO, "_slot3Text", slot3TMP);
            SafeSetObject(hudSO, "_subtitlePanel", subPanel);
            SafeSetObject(hudSO, "_subtitleSpeakerTMP", subSpeakerTMP);
            SafeSetObject(hudSO, "_subtitleContentTMP", subContentTMP);
            SafeSetObject(hudSO, "_subtitlePromptTMP", subPromptTMP);
            SafeSetObject(hudSO, "_ammoPanel", ammoPanel);
            SafeSetObject(hudSO, "_ammoTMP", ammoTMP);
            SafeSetObject(hudSO, "_summaryPanel", sumPanel);
            SafeSetObject(hudSO, "_summaryText", sumTMP);
            SafeSetObject(hudSO, "_bossCountdownPanel", bossCountdownPanel);
            SafeSetObject(hudSO, "_bossCountdownTMP", bossCdTMP);
            SafeSetObject(hudSO, "_bossCountdownFill", bossCdFill);
            SafeSetObject(hudSO, "_bossHealthPanel", bossHealthPanel);
            SafeSetObject(hudSO, "_bossNameTMP", bossNameTMP);
            SafeSetObject(hudSO, "_bossHealthTMP", bossHpTMP);
            SafeSetObject(hudSO, "_bossHealthFill", bossHpFill);
            hudSO.ApplyModifiedProperties();
        }

        private static void CreateStoreComputerUI(GameObject canvasObj, System.Action<GameObject> ResetUITransform)
        {
            GameObject compRoot = new GameObject("Store_Computer_Terminal_UI");
            compRoot.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(compRoot);

            RectTransform compRect = compRoot.AddComponent<RectTransform>();
            compRect.anchorMin = Vector2.zero;
            compRect.anchorMax = Vector2.one;
            compRect.sizeDelta = Vector2.zero;

            StoreComputerUI compUI = compRoot.AddComponent<StoreComputerUI>();

            // Cửa sổ giao diện máy tính (Window Panel - bị ẩn/hiện, compRoot luôn giữ Active để lắng nghe sự kiện)
            GameObject windowPanel = new GameObject("Computer_Window_Panel");
            windowPanel.transform.SetParent(compRoot.transform, false);
            ResetUITransform(windowPanel);

            RectTransform winRect = windowPanel.AddComponent<RectTransform>();
            winRect.anchorMin = new Vector2(0.5f, 0.5f);
            winRect.anchorMax = new Vector2(0.5f, 0.5f);
            winRect.pivot = new Vector2(0.5f, 0.5f);
            winRect.anchoredPosition = Vector2.zero;
            winRect.sizeDelta = new Vector2(1600f, 960f); // Phóng to gấp đôi theo yêu cầu người chơi

            Image winBg = windowPanel.AddComponent<Image>();
            winBg.color = new Color(0.05f, 0.07f, 0.1f, 0.98f);

            // Header Bar
            GameObject headerBar = new GameObject("Terminal_Header_Bar");
            headerBar.transform.SetParent(windowPanel.transform, false);
            ResetUITransform(headerBar);
            RectTransform headerRect = headerBar.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0f, 76f);
            Image headerBg = headerBar.AddComponent<Image>();
            headerBg.color = new Color(0.08f, 0.12f, 0.18f, 1f);

            GameObject titleObj = new GameObject("Terminal_Title_Text");
            titleObj.transform.SetParent(headerBar.transform, false);
            ResetUITransform(titleObj);
            TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.rectTransform.anchorMin = new Vector2(0f, 0f);
            titleTMP.rectTransform.anchorMax = new Vector2(0.6f, 1f);
            titleTMP.rectTransform.anchoredPosition = new Vector2(24f, 0f);
            titleTMP.rectTransform.sizeDelta = new Vector2(-24f, 0f);
            titleTMP.alignment = TextAlignmentOptions.MidlineLeft;
            titleTMP.fontSize = 26f;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(0.1f, 0.85f, 0.95f);
            titleTMP.text = "🖥️ MIDNIGHT MART TERMINAL [OS v2.4]";

            // Money TMP
            GameObject moneyObj = new GameObject("Terminal_Money_Text");
            moneyObj.transform.SetParent(headerBar.transform, false);
            ResetUITransform(moneyObj);
            TextMeshProUGUI moneyTMP = moneyObj.AddComponent<TextMeshProUGUI>();
            moneyTMP.rectTransform.anchorMin = new Vector2(0.55f, 0f);
            moneyTMP.rectTransform.anchorMax = new Vector2(0.82f, 1f);
            moneyTMP.rectTransform.anchoredPosition = Vector2.zero;
            moneyTMP.rectTransform.sizeDelta = Vector2.zero;
            moneyTMP.alignment = TextAlignmentOptions.MidlineRight;
            moneyTMP.fontSize = 24f;
            moneyTMP.fontStyle = FontStyles.Bold;
            moneyTMP.color = new Color(0.3f, 1.0f, 0.5f);
            moneyTMP.text = "QUỸ TIỆM: $150.00";

            // Close Button
            GameObject closeBtnObj = new GameObject("Close_Button");
            closeBtnObj.transform.SetParent(headerBar.transform, false);
            ResetUITransform(closeBtnObj);
            RectTransform closeRect = closeBtnObj.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1f, 0.5f);
            closeRect.anchorMax = new Vector2(1f, 0.5f);
            closeRect.pivot = new Vector2(1f, 0.5f);
            closeRect.anchoredPosition = new Vector2(-16f, 0f);
            closeRect.sizeDelta = new Vector2(190f, 50f);
            Image closeBtnBg = closeBtnObj.AddComponent<Image>();
            closeBtnBg.color = new Color(0.7f, 0.15f, 0.15f, 0.9f);
            Button closeBtn = closeBtnObj.AddComponent<Button>();

            GameObject closeTextObj = new GameObject("Text");
            closeTextObj.transform.SetParent(closeBtnObj.transform, false);
            ResetUITransform(closeTextObj);
            TextMeshProUGUI closeTMP = closeTextObj.AddComponent<TextMeshProUGUI>();
            closeTMP.rectTransform.anchorMin = Vector2.zero;
            closeTMP.rectTransform.anchorMax = Vector2.one;
            closeTMP.rectTransform.sizeDelta = Vector2.zero;
            closeTMP.alignment = TextAlignmentOptions.Center;
            closeTMP.fontSize = 18f;
            closeTMP.fontStyle = FontStyles.Bold;
            closeTMP.color = Color.white;
            closeTMP.text = "[X] ĐÓNG (ESC)";

            // Tab Bar Buttons (F1: Database, F2: Supply)
            GameObject tabBar = new GameObject("Tab_Bar");
            tabBar.transform.SetParent(windowPanel.transform, false);
            ResetUITransform(tabBar);
            RectTransform tabRect = tabBar.AddComponent<RectTransform>();
            tabRect.anchorMin = new Vector2(0f, 1f);
            tabRect.anchorMax = new Vector2(1f, 1f);
            tabRect.pivot = new Vector2(0.5f, 1f);
            tabRect.anchoredPosition = new Vector2(0f, -80f);
            tabRect.sizeDelta = new Vector2(0f, 58f);

            // Tab 1 Button: Database
            GameObject tabDbObj = new GameObject("Tab_Database_Btn");
            tabDbObj.transform.SetParent(tabBar.transform, false);
            ResetUITransform(tabDbObj);
            RectTransform tabDbRect = tabDbObj.AddComponent<RectTransform>();
            tabDbRect.anchorMin = new Vector2(0f, 0f);
            tabDbRect.anchorMax = new Vector2(0.5f, 1f);
            tabDbRect.anchoredPosition = new Vector2(16f, 0f);
            tabDbRect.sizeDelta = new Vector2(-24f, 0f);
            Image tabDbBg = tabDbObj.AddComponent<Image>();
            tabDbBg.color = new Color(0.12f, 0.18f, 0.25f, 1f);
            Button tabDbBtn = tabDbObj.AddComponent<Button>();
            GameObject tabDbTextObj = new GameObject("Text");
            tabDbTextObj.transform.SetParent(tabDbObj.transform, false);
            ResetUITransform(tabDbTextObj);
            TextMeshProUGUI tabDbTMP = tabDbTextObj.AddComponent<TextMeshProUGUI>();
            tabDbTMP.rectTransform.anchorMin = Vector2.zero;
            tabDbTMP.rectTransform.anchorMax = Vector2.one;
            tabDbTMP.rectTransform.sizeDelta = Vector2.zero;
            tabDbTMP.alignment = TextAlignmentOptions.Center;
            tabDbTMP.fontSize = 20f;
            tabDbTMP.fontStyle = FontStyles.Bold;
            tabDbTMP.color = new Color(0.2f, 0.9f, 1f);
            tabDbTMP.text = "[F1] CSDL CÔNG DÂN BLACKWOOD";

            // Tab 2 Button: Supply Store
            GameObject tabSpObj = new GameObject("Tab_Supply_Btn");
            tabSpObj.transform.SetParent(tabBar.transform, false);
            ResetUITransform(tabSpObj);
            RectTransform tabSpRect = tabSpObj.AddComponent<RectTransform>();
            tabSpRect.anchorMin = new Vector2(0.5f, 0f);
            tabSpRect.anchorMax = new Vector2(1f, 1f);
            tabSpRect.anchoredPosition = new Vector2(8f, 0f);
            tabSpRect.sizeDelta = new Vector2(-24f, 0f);
            Image tabSpBg = tabSpObj.AddComponent<Image>();
            tabSpBg.color = new Color(0.1f, 0.14f, 0.18f, 1f);
            Button tabSpBtn = tabSpObj.AddComponent<Button>();
            GameObject tabSpTextObj = new GameObject("Text");
            tabSpTextObj.transform.SetParent(tabSpObj.transform, false);
            ResetUITransform(tabSpTextObj);
            TextMeshProUGUI tabSpTMP = tabSpTextObj.AddComponent<TextMeshProUGUI>();
            tabSpTMP.rectTransform.anchorMin = Vector2.zero;
            tabSpTMP.rectTransform.anchorMax = Vector2.one;
            tabSpTMP.rectTransform.sizeDelta = Vector2.zero;
            tabSpTMP.alignment = TextAlignmentOptions.Center;
            tabSpTMP.fontSize = 20f;
            tabSpTMP.fontStyle = FontStyles.Bold;
            tabSpTMP.color = new Color(1.0f, 0.85f, 0.2f);
            tabSpTMP.text = "[F2] TIẾP TẾ ĐÊM KHẨN CẤP";

            // Content Area Container
            GameObject contentArea = new GameObject("Content_Area");
            contentArea.transform.SetParent(windowPanel.transform, false);
            ResetUITransform(contentArea);
            RectTransform caRect = contentArea.AddComponent<RectTransform>();
            caRect.anchorMin = Vector2.zero;
            caRect.anchorMax = Vector2.one;
            caRect.sizeDelta = new Vector2(-32f, -160f);
            caRect.anchoredPosition = new Vector2(0f, -72f);

            // TAB 1: Database Panel
            GameObject dbPanel = new GameObject("Tab_Database_Panel");
            dbPanel.transform.SetParent(contentArea.transform, false);
            ResetUITransform(dbPanel);
            RectTransform dbRect = dbPanel.AddComponent<RectTransform>();
            dbRect.anchorMin = Vector2.zero;
            dbRect.anchorMax = Vector2.one;
            dbRect.sizeDelta = Vector2.zero;
            Image dbBg = dbPanel.AddComponent<Image>();
            dbBg.color = new Color(0.07f, 0.09f, 0.13f, 0.95f);

            GameObject citNameObj = new GameObject("Citizen_Name_TMP");
            citNameObj.transform.SetParent(dbPanel.transform, false);
            ResetUITransform(citNameObj);
            TextMeshProUGUI citNameTMP = citNameObj.AddComponent<TextMeshProUGUI>();
            citNameTMP.rectTransform.anchorMin = new Vector2(0f, 1f);
            citNameTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            citNameTMP.rectTransform.pivot = new Vector2(0f, 1f);
            citNameTMP.rectTransform.anchoredPosition = new Vector2(36f, -24f);
            citNameTMP.rectTransform.sizeDelta = new Vector2(-72f, 48f);
            citNameTMP.fontSize = 28f;
            citNameTMP.fontStyle = FontStyles.Bold;
            citNameTMP.color = new Color(0.2f, 0.9f, 1f);
            citNameTMP.text = "HỒ SƠ: CHƯA CÓ KHÁCH HÀNG TẠI QUẦY";

            GameObject citStatusObj = new GameObject("Citizen_Status_TMP");
            citStatusObj.transform.SetParent(dbPanel.transform, false);
            ResetUITransform(citStatusObj);
            TextMeshProUGUI citStatusTMP = citStatusObj.AddComponent<TextMeshProUGUI>();
            citStatusTMP.rectTransform.anchorMin = new Vector2(0f, 1f);
            citStatusTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            citStatusTMP.rectTransform.pivot = new Vector2(0f, 1f);
            citStatusTMP.rectTransform.anchoredPosition = new Vector2(36f, -80f);
            citStatusTMP.rectTransform.sizeDelta = new Vector2(-72f, 40f);
            citStatusTMP.fontSize = 22f;
            citStatusTMP.fontStyle = FontStyles.Bold;
            citStatusTMP.color = Color.gray;
            citStatusTMP.text = "TRẠNG THÁI: CHỜ KẾT NỐI CAMERA...";

            GameObject citDetailsObj = new GameObject("Citizen_Details_TMP");
            citDetailsObj.transform.SetParent(dbPanel.transform, false);
            ResetUITransform(citDetailsObj);
            TextMeshProUGUI citDetailsTMP = citDetailsObj.AddComponent<TextMeshProUGUI>();
            citDetailsTMP.rectTransform.anchorMin = Vector2.zero;
            citDetailsTMP.rectTransform.anchorMax = Vector2.one;
            citDetailsTMP.rectTransform.sizeDelta = new Vector2(-72f, -145f);
            citDetailsTMP.rectTransform.anchoredPosition = new Vector2(0f, -60f);
            citDetailsTMP.fontSize = 22f;
            citDetailsTMP.lineSpacing = 22f;
            citDetailsTMP.textWrappingMode = TextWrappingModes.Normal;
            citDetailsTMP.color = new Color(0.85f, 0.9f, 0.95f);
            citDetailsTMP.text = "Hệ thống đang quét mặt tiền siêu thị... Vui lòng chờ khách mang đồ lên quầy.";

            // TAB 2: Supply Panel
            GameObject spPanel = new GameObject("Tab_Supply_Panel");
            spPanel.transform.SetParent(contentArea.transform, false);
            ResetUITransform(spPanel);
            RectTransform spRect = spPanel.AddComponent<RectTransform>();
            spRect.anchorMin = Vector2.zero;
            spRect.anchorMax = Vector2.one;
            spRect.sizeDelta = Vector2.zero;
            Image spBg = spPanel.AddComponent<Image>();
            spBg.color = new Color(0.07f, 0.09f, 0.13f, 0.95f);

            // Feedback Text
            GameObject spFeedObj = new GameObject("Supply_Feedback_TMP");
            spFeedObj.transform.SetParent(spPanel.transform, false);
            ResetUITransform(spFeedObj);
            TextMeshProUGUI spFeedTMP = spFeedObj.AddComponent<TextMeshProUGUI>();
            spFeedTMP.rectTransform.anchorMin = new Vector2(0f, 1f);
            spFeedTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            spFeedTMP.rectTransform.pivot = new Vector2(0f, 1f);
            spFeedTMP.rectTransform.anchoredPosition = new Vector2(36f, -18f);
            spFeedTMP.rectTransform.sizeDelta = new Vector2(-72f, 38f);
            spFeedTMP.fontSize = 22f;
            spFeedTMP.fontStyle = FontStyles.Bold;
            spFeedTMP.color = new Color(1f, 0.85f, 0.2f);
            spFeedTMP.text = "Chọn gói tiếp tế cần mua để sẵn sàng chiến đấu:";

            // Helper tạo nút Mua hàng (Gấp đôi kích thước thẻ: 720x190)
            System.Func<string, string, Vector2, Button> CreateBuyCard = (itemName, desc, pos) =>
            {
                GameObject card = new GameObject($"Card_{itemName}");
                card.transform.SetParent(spPanel.transform, false);
                ResetUITransform(card);
                RectTransform crt = card.AddComponent<RectTransform>();
                crt.anchorMin = new Vector2(0.5f, 0.5f);
                crt.anchorMax = new Vector2(0.5f, 0.5f);
                crt.pivot = new Vector2(0.5f, 0.5f);
                crt.anchoredPosition = pos;
                crt.sizeDelta = new Vector2(720f, 190f);
                Image cardBg = card.AddComponent<Image>();
                cardBg.color = new Color(0.12f, 0.16f, 0.22f, 0.9f);
                Button cardBtn = card.AddComponent<Button>();

                GameObject cardText = new GameObject("Text");
                cardText.transform.SetParent(card.transform, false);
                ResetUITransform(cardText);
                TextMeshProUGUI cardTMP = cardText.AddComponent<TextMeshProUGUI>();
                cardTMP.rectTransform.anchorMin = Vector2.zero;
                cardTMP.rectTransform.anchorMax = Vector2.one;
                cardTMP.rectTransform.sizeDelta = new Vector2(-32f, -24f);
                cardTMP.fontSize = 20f;
                cardTMP.lineSpacing = 16f;
                cardTMP.alignment = TextAlignmentOptions.MidlineLeft;
                cardTMP.color = Color.white;
                cardTMP.text = $"<size=24><b>{itemName}</b></size>\n<color=#A0C0E0>{desc}</color>";

                return cardBtn;
            };

            Button buyPistolBtn = CreateBuyCard("[MUA] Gói Đạn 9mm ($15)", "10 viên đạn súng lục. Dùng để đối phó quái vật nhanh.", new Vector2(-380f, 110f));
            Button buyShotgunBtn = CreateBuyCard("[MUA] Hộp Đạn Shotgun ($30)", "4 viên đạn ria 12-Gauge. Sát thương cực lớn ở cự ly gần.", new Vector2(380f, 110f));
            Button buyEnergyBtn = CreateBuyCard("[MUA] Nước Tăng Lực ($12)", "Hồi phục 100% thể lực chạy nước rút (Sprint) để thả diều Boss.", new Vector2(-380f, -110f));
            Button buyBarricadeBtn = CreateBuyCard("[MUA] Thanh Gỗ Chặn Cửa ($25)", "Gia cố che chắn quầy thu ngân phòng thủ.", new Vector2(380f, -110f));

            spPanel.SetActive(false); // Tab database mở trước

            // Gán Serialized Properties vào StoreComputerUI
            SerializedObject compSO = new SerializedObject(compUI);
            SafeSetObject(compSO, "_mainPanel", windowPanel);
            SafeSetObject(compSO, "_moneyTMP", moneyTMP);
            SafeSetObject(compSO, "_databaseTabPanel", dbPanel);
            SafeSetObject(compSO, "_citizenNameTMP", citNameTMP);
            SafeSetObject(compSO, "_citizenDetailsTMP", citDetailsTMP);
            SafeSetObject(compSO, "_citizenStatusTMP", citStatusTMP);
            SafeSetObject(compSO, "_supplyTabPanel", spPanel);
            SafeSetObject(compSO, "_supplyFeedbackTMP", spFeedTMP);
            SafeSetObject(compSO, "_closeBtn", closeBtn);
            SafeSetObject(compSO, "_tabDatabaseBtn", tabDbBtn);
            SafeSetObject(compSO, "_tabSupplyBtn", tabSpBtn);
            SafeSetObject(compSO, "_buyPistolAmmoBtn", buyPistolBtn);
            SafeSetObject(compSO, "_buyShotgunAmmoBtn", buyShotgunBtn);
            SafeSetObject(compSO, "_buyEnergyDrinkBtn", buyEnergyBtn);
            SafeSetObject(compSO, "_buyBarricadeBtn", buyBarricadeBtn);
            compSO.ApplyModifiedProperties();

            windowPanel.SetActive(false); // Cửa sổ máy tính mặc định ẩn (compRoot giữ Active để nhận sự kiện [E])
        }

        private static void CreatePauseSettingsUI(GameObject canvasObj, Button hudPauseBtn, System.Action<GameObject> ResetUITransform)
        {
            GameObject pauseRoot = new GameObject("Pause_Settings_Menu_UI");
            pauseRoot.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(pauseRoot);

            RectTransform rootRt = pauseRoot.AddComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.sizeDelta = Vector2.zero;

            PauseSettingsUI pauseUI = pauseRoot.AddComponent<PauseSettingsUI>();

            // Khung mờ đen toàn màn hình (Modal Overlay Panel)
            GameObject modalPanel = new GameObject("Pause_Menu_Panel");
            modalPanel.transform.SetParent(pauseRoot.transform, false);
            ResetUITransform(modalPanel);
            RectTransform modalRt = modalPanel.AddComponent<RectTransform>();
            modalRt.anchorMin = Vector2.zero;
            modalRt.anchorMax = Vector2.one;
            modalRt.sizeDelta = Vector2.zero;
            Image modalBg = modalPanel.AddComponent<Image>();
            modalBg.color = new Color(0.02f, 0.03f, 0.05f, 0.90f);

            // Hộp hội thoại Menu trung tâm (Center Dialog Box 680x560)
            GameObject dialogBox = new GameObject("Center_Dialog_Box");
            dialogBox.transform.SetParent(modalPanel.transform, false);
            ResetUITransform(dialogBox);
            RectTransform dialogRt = dialogBox.AddComponent<RectTransform>();
            dialogRt.anchorMin = new Vector2(0.5f, 0.5f);
            dialogRt.anchorMax = new Vector2(0.5f, 0.5f);
            dialogRt.pivot = new Vector2(0.5f, 0.5f);
            dialogRt.sizeDelta = new Vector2(720f, 640f);
            Image dialogBg = dialogBox.AddComponent<Image>();
            dialogBg.color = new Color(0.08f, 0.10f, 0.15f, 0.98f);

            // Tiêu đề
            GameObject titleObj = new GameObject("Pause_Title_Text");
            titleObj.transform.SetParent(dialogBox.transform, false);
            ResetUITransform(titleObj);
            TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.rectTransform.anchorMin = new Vector2(0f, 1f);
            titleTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            titleTMP.rectTransform.pivot = new Vector2(0.5f, 1f);
            titleTMP.rectTransform.anchoredPosition = new Vector2(0f, -24f);
            titleTMP.rectTransform.sizeDelta = new Vector2(-40f, 40f);
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.fontSize = 28f;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(1.0f, 0.85f, 0.15f);
            titleTMP.text = "⏸️ TẠM DỪNG & CÀI ĐẶT";

            // Phần 1: Ngôn ngữ (Language Selection)
            GameObject langSectionObj = new GameObject("Language_Section_Text");
            langSectionObj.transform.SetParent(dialogBox.transform, false);
            ResetUITransform(langSectionObj);
            TextMeshProUGUI langTitleTMP = langSectionObj.AddComponent<TextMeshProUGUI>();
            langTitleTMP.rectTransform.anchorMin = new Vector2(0f, 1f);
            langTitleTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            langTitleTMP.rectTransform.pivot = new Vector2(0.5f, 1f);
            langTitleTMP.rectTransform.anchoredPosition = new Vector2(0f, -80f);
            langTitleTMP.rectTransform.sizeDelta = new Vector2(-60f, 28f);
            langTitleTMP.alignment = TextAlignmentOptions.MidlineLeft;
            langTitleTMP.fontSize = 18f;
            langTitleTMP.fontStyle = FontStyles.Bold;
            langTitleTMP.color = new Color(0.85f, 0.90f, 0.95f);
            langTitleTMP.text = "🌐 NGÔN NGỮ / LANGUAGE:";

            // Nút Tiếng Việt
            GameObject btnViObj = new GameObject("Btn_Vietnamese");
            btnViObj.transform.SetParent(dialogBox.transform, false);
            ResetUITransform(btnViObj);
            RectTransform btnViRt = btnViObj.AddComponent<RectTransform>();
            btnViRt.anchorMin = new Vector2(0.5f, 1f);
            btnViRt.anchorMax = new Vector2(0.5f, 1f);
            btnViRt.pivot = new Vector2(1f, 1f);
            btnViRt.anchoredPosition = new Vector2(-15f, -115f);
            btnViRt.sizeDelta = new Vector2(260f, 46f);
            Image btnViBg = btnViObj.AddComponent<Image>();
            btnViBg.color = new Color(0.18f, 0.35f, 0.65f, 1f);
            Button btnVi = btnViObj.AddComponent<Button>();

            GameObject viTxtObj = new GameObject("Text");
            viTxtObj.transform.SetParent(btnViObj.transform, false);
            ResetUITransform(viTxtObj);
            TextMeshProUGUI viTMP = viTxtObj.AddComponent<TextMeshProUGUI>();
            viTMP.rectTransform.anchorMin = Vector2.zero;
            viTMP.rectTransform.anchorMax = Vector2.one;
            viTMP.rectTransform.sizeDelta = Vector2.zero;
            viTMP.alignment = TextAlignmentOptions.Center;
            viTMP.fontSize = 18f;
            viTMP.fontStyle = FontStyles.Bold;
            viTMP.color = Color.white;
            viTMP.text = "[VN] Tiếng Việt";

            // Nút English
            GameObject btnEnObj = new GameObject("Btn_English");
            btnEnObj.transform.SetParent(dialogBox.transform, false);
            ResetUITransform(btnEnObj);
            RectTransform btnEnRt = btnEnObj.AddComponent<RectTransform>();
            btnEnRt.anchorMin = new Vector2(0.5f, 1f);
            btnEnRt.anchorMax = new Vector2(0.5f, 1f);
            btnEnRt.pivot = new Vector2(0f, 1f);
            btnEnRt.anchoredPosition = new Vector2(15f, -115f);
            btnEnRt.sizeDelta = new Vector2(260f, 46f);
            Image btnEnBg = btnEnObj.AddComponent<Image>();
            btnEnBg.color = new Color(0.12f, 0.15f, 0.22f, 1f);
            Button btnEn = btnEnObj.AddComponent<Button>();

            GameObject enTxtObj = new GameObject("Text");
            enTxtObj.transform.SetParent(btnEnObj.transform, false);
            ResetUITransform(enTxtObj);
            TextMeshProUGUI enTMP = enTxtObj.AddComponent<TextMeshProUGUI>();
            enTMP.rectTransform.anchorMin = Vector2.zero;
            enTMP.rectTransform.anchorMax = Vector2.one;
            enTMP.rectTransform.sizeDelta = Vector2.zero;
            enTMP.alignment = TextAlignmentOptions.Center;
            enTMP.fontSize = 18f;
            enTMP.fontStyle = FontStyles.Bold;
            enTMP.color = Color.white;
            enTMP.text = "[EN] English";

            // Phần 2: Âm thanh (Audio Settings)
            GameObject audioSectionObj = new GameObject("Audio_Section_Text");
            audioSectionObj.transform.SetParent(dialogBox.transform, false);
            ResetUITransform(audioSectionObj);
            TextMeshProUGUI audioTitleTMP = audioSectionObj.AddComponent<TextMeshProUGUI>();
            audioTitleTMP.rectTransform.anchorMin = new Vector2(0f, 1f);
            audioTitleTMP.rectTransform.anchorMax = new Vector2(1f, 1f);
            audioTitleTMP.rectTransform.pivot = new Vector2(0.5f, 1f);
            audioTitleTMP.rectTransform.anchoredPosition = new Vector2(0f, -180f);
            audioTitleTMP.rectTransform.sizeDelta = new Vector2(-60f, 28f);
            audioTitleTMP.alignment = TextAlignmentOptions.MidlineLeft;
            audioTitleTMP.fontSize = 18f;
            audioTitleTMP.fontStyle = FontStyles.Bold;
            audioTitleTMP.color = new Color(0.85f, 0.90f, 0.95f);
            audioTitleTMP.text = "ÂM LƯỢNG / AUDIO VOLUME:";

            // Slider SFX / Ambience
            CreateSliderRow(dialogBox, -215f, "Âm thanh môi trường & Hiệu ứng (SFX):", 0.85f, out Slider sfxSlider, out TextMeshProUGUI sfxLabelTMP, out TextMeshProUGUI sfxValueTMP, ResetUITransform);

            // Slider Music / BGM
            CreateSliderRow(dialogBox, -275f, "Nhạc nền & Bầu không khí (Music/BGM):", 0.75f, out Slider musicSlider, out TextMeshProUGUI musicLabelTMP, out TextMeshProUGUI musicValueTMP, ResetUITransform);

            // Phần 3: Các Nút Hành Động
            CreateMenuActionButton(dialogBox, -330f, new Color(0.12f, 0.55f, 0.30f), "TIẾP TỤC CHƠI [ESC]", out Button resumeBtn, out TextMeshProUGUI resumeBtnTMP, ResetUITransform);
            CreateMenuActionButton(dialogBox, -385f, new Color(0.20f, 0.25f, 0.38f), "CHƠI LẠI CA TRỰC NÀY", out Button restartBtn, out TextMeshProUGUI restartBtnTMP, ResetUITransform);
            CreateMenuActionButton(dialogBox, -440f, new Color(0.45f, 0.20f, 0.20f), "RESET TIẾN ĐỘ VỀ ĐẦU (ĐÊM 1)", out Button resetProgBtn, out TextMeshProUGUI resetProgBtnTMP, ResetUITransform);
            CreateMenuActionButton(dialogBox, -495f, new Color(0.55f, 0.15f, 0.15f), "THOÁT GAME", out Button quitBtn, out TextMeshProUGUI quitBtnTMP, ResetUITransform);

            // Wire up properties to PauseSettingsUI
            SerializedObject pSO = new SerializedObject(pauseUI);
            SafeSetObject(pSO, "_pauseMenuPanel", modalPanel);
            SafeSetObject(pSO, "_titleTMP", titleTMP);
            SafeSetObject(pSO, "_langSectionTitleTMP", langTitleTMP);
            SafeSetObject(pSO, "_btnVietnamese", btnVi);
            SafeSetObject(pSO, "_btnEnglish", btnEn);
            SafeSetObject(pSO, "_btnViBg", btnViBg);
            SafeSetObject(pSO, "_btnEnBg", btnEnBg);
            SafeSetObject(pSO, "_audioSectionTitleTMP", audioTitleTMP);
            SafeSetObject(pSO, "_sfxLabelTMP", sfxLabelTMP);
            SafeSetObject(pSO, "_sfxSlider", sfxSlider);
            SafeSetObject(pSO, "_sfxValueTMP", sfxValueTMP);
            SafeSetObject(pSO, "_musicLabelTMP", musicLabelTMP);
            SafeSetObject(pSO, "_musicSlider", musicSlider);
            SafeSetObject(pSO, "_musicValueTMP", musicValueTMP);
            SafeSetObject(pSO, "_resumeBtn", resumeBtn);
            SafeSetObject(pSO, "_resumeBtnTMP", resumeBtnTMP);
            SafeSetObject(pSO, "_restartBtn", restartBtn);
            SafeSetObject(pSO, "_restartBtnTMP", restartBtnTMP);
            SafeSetObject(pSO, "_resetAllProgressBtn", resetProgBtn);
            SafeSetObject(pSO, "_resetAllProgressBtnTMP", resetProgBtnTMP);
            SafeSetObject(pSO, "_quitBtn", quitBtn);
            SafeSetObject(pSO, "_quitBtnTMP", quitBtnTMP);
            SafeSetObject(pSO, "_hudPauseBtn", hudPauseBtn);
            pSO.ApplyModifiedProperties();

            modalPanel.SetActive(false);
        }

        private static void CreateSliderRow(GameObject parent, float posY, string labelText, float initialValue, out Slider slider, out TextMeshProUGUI labelTMP, out TextMeshProUGUI valueTMP, System.Action<GameObject> ResetUITransform)
        {
            GameObject rowObj = new GameObject($"Slider_Row_{posY:0}");
            rowObj.transform.SetParent(parent.transform, false);
            ResetUITransform(rowObj);
            RectTransform rowRt = rowObj.AddComponent<RectTransform>();
            rowRt.anchorMin = new Vector2(0f, 1f);
            rowRt.anchorMax = new Vector2(1f, 1f);
            rowRt.pivot = new Vector2(0.5f, 1f);
            rowRt.anchoredPosition = new Vector2(0f, posY);
            rowRt.sizeDelta = new Vector2(-60f, 44f);

            GameObject lblObj = new GameObject("Label");
            lblObj.transform.SetParent(rowObj.transform, false);
            ResetUITransform(lblObj);
            labelTMP = lblObj.AddComponent<TextMeshProUGUI>();
            labelTMP.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            labelTMP.rectTransform.anchorMax = new Vector2(0.55f, 0.5f);
            labelTMP.rectTransform.pivot = new Vector2(0f, 0.5f);
            labelTMP.rectTransform.anchoredPosition = Vector2.zero;
            labelTMP.rectTransform.sizeDelta = new Vector2(0f, 32f);
            labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
            labelTMP.fontSize = 15f;
            labelTMP.fontStyle = FontStyles.Bold;
            labelTMP.color = Color.white;
            labelTMP.text = labelText;

            GameObject sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(rowObj.transform, false);
            ResetUITransform(sliderObj);
            RectTransform sliderRt = sliderObj.AddComponent<RectTransform>();
            sliderRt.anchorMin = new Vector2(0.58f, 0.5f);
            sliderRt.anchorMax = new Vector2(0.86f, 0.5f);
            sliderRt.pivot = new Vector2(0.5f, 0.5f);
            sliderRt.anchoredPosition = Vector2.zero;
            sliderRt.sizeDelta = new Vector2(0f, 22f);

            slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = initialValue;

            GameObject sBgObj = new GameObject("Background");
            sBgObj.transform.SetParent(sliderObj.transform, false);
            ResetUITransform(sBgObj);
            RectTransform sBgRt = sBgObj.AddComponent<RectTransform>();
            sBgRt.anchorMin = Vector2.zero;
            sBgRt.anchorMax = Vector2.one;
            sBgRt.sizeDelta = Vector2.zero;
            Image sBg = sBgObj.AddComponent<Image>();
            sBg.color = new Color(0.12f, 0.15f, 0.22f, 1f);

            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            ResetUITransform(fillArea);
            RectTransform fillAreaRt = fillArea.AddComponent<RectTransform>();
            fillAreaRt.anchorMin = Vector2.zero;
            fillAreaRt.anchorMax = Vector2.one;
            fillAreaRt.sizeDelta = Vector2.zero;

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            ResetUITransform(fillObj);
            RectTransform fillRt = fillObj.AddComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.sizeDelta = Vector2.zero;
            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.color = new Color(0.2f, 0.7f, 0.95f, 1f);
            slider.fillRect = fillRt;

            GameObject valObj = new GameObject("Value_Text");
            valObj.transform.SetParent(rowObj.transform, false);
            ResetUITransform(valObj);
            valueTMP = valObj.AddComponent<TextMeshProUGUI>();
            valueTMP.rectTransform.anchorMin = new Vector2(0.88f, 0.5f);
            valueTMP.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            valueTMP.rectTransform.pivot = new Vector2(1f, 0.5f);
            valueTMP.rectTransform.anchoredPosition = Vector2.zero;
            valueTMP.rectTransform.sizeDelta = new Vector2(0f, 32f);
            valueTMP.alignment = TextAlignmentOptions.MidlineRight;
            valueTMP.fontSize = 16f;
            valueTMP.fontStyle = FontStyles.Bold;
            valueTMP.color = new Color(0.2f, 0.95f, 0.75f);
            valueTMP.text = $"{Mathf.RoundToInt(initialValue * 100f)}%";
        }

        private static void CreateMenuActionButton(GameObject parent, float posY, Color bgColor, string labelText, out Button button, out TextMeshProUGUI btnTMP, System.Action<GameObject> ResetUITransform)
        {
            GameObject btnObj = new GameObject($"Action_Btn_{posY:0}");
            btnObj.transform.SetParent(parent.transform, false);
            ResetUITransform(btnObj);
            RectTransform btnRt = btnObj.AddComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0.5f, 1f);
            btnRt.anchorMax = new Vector2(0.5f, 1f);
            btnRt.pivot = new Vector2(0.5f, 1f);
            btnRt.anchoredPosition = new Vector2(0f, posY);
            btnRt.sizeDelta = new Vector2(560f, 48f);

            Image bg = btnObj.AddComponent<Image>();
            bg.color = bgColor;
            button = btnObj.AddComponent<Button>();

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            ResetUITransform(txtObj);
            btnTMP = txtObj.AddComponent<TextMeshProUGUI>();
            btnTMP.rectTransform.anchorMin = Vector2.zero;
            btnTMP.rectTransform.anchorMax = Vector2.one;
            btnTMP.rectTransform.sizeDelta = Vector2.zero;
            btnTMP.alignment = TextAlignmentOptions.Center;
            btnTMP.fontSize = 18f;
            btnTMP.fontStyle = FontStyles.Bold;
            btnTMP.color = Color.white;
            btnTMP.text = labelText;
        }

        private static void CreateMainMenuUI(GameObject canvasObj, GameObject player, Transform homeSpawn, System.Action<GameObject> ResetUITransform)
        {
            GameObject menuRoot = new GameObject("Main_Menu_UI");
            menuRoot.transform.SetParent(canvasObj.transform, false);
            ResetUITransform(menuRoot);

            RectTransform rootRt = menuRoot.AddComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.sizeDelta = Vector2.zero;

            MainMenuManager menuManager = menuRoot.AddComponent<MainMenuManager>();
            CanvasGroup canvasGroup = menuRoot.AddComponent<CanvasGroup>();

            // 1. Toàn màn hình nền đen kinh dị (Full Screen Horror Atmosphere Backdrop)
            GameObject overlay = new GameObject("Menu_Fullscreen_Backdrop");
            overlay.transform.SetParent(menuRoot.transform, false);
            ResetUITransform(overlay);
            RectTransform overlayRt = overlay.AddComponent<RectTransform>();
            overlayRt.anchorMin = Vector2.zero;
            overlayRt.anchorMax = Vector2.one;
            overlayRt.sizeDelta = Vector2.zero;
            Image overlayImg = overlay.AddComponent<Image>();
            overlayImg.color = new Color(0f, 0f, 0f, 1f); // 100% Đen tuyền che kín hoàn toàn khung cảnh 3D phía sau

            // 2. Tiêu đề Game: MIDNIGHT MART (Toàn màn hình, đặt ở nửa trên)
            GameObject titleObj = new GameObject("Menu_Title");
            titleObj.transform.SetParent(overlay.transform, false);
            ResetUITransform(titleObj);
            RectTransform titleRt = titleObj.AddComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 0.72f);
            titleRt.anchorMax = new Vector2(0.5f, 0.72f);
            titleRt.pivot = new Vector2(0.5f, 0.5f);
            titleRt.anchoredPosition = Vector2.zero;
            titleRt.sizeDelta = new Vector2(1000f, 100f);

            TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "MIDNIGHT MART";
            titleTMP.fontSize = 76f;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(0.95f, 0.18f, 0.18f);
            titleTMP.alignment = TextAlignmentOptions.Center;

            // Phụ đề rùng rợn
            GameObject subTitleObj = new GameObject("Menu_Subtitle");
            subTitleObj.transform.SetParent(overlay.transform, false);
            ResetUITransform(subTitleObj);
            RectTransform subTitleRt = subTitleObj.AddComponent<RectTransform>();
            subTitleRt.anchorMin = new Vector2(0.5f, 0.63f);
            subTitleRt.anchorMax = new Vector2(0.5f, 0.63f);
            subTitleRt.pivot = new Vector2(0.5f, 0.5f);
            subTitleRt.anchoredPosition = Vector2.zero;
            subTitleRt.sizeDelta = new Vector2(1000f, 40f);

            TextMeshProUGUI subTitleTMP = subTitleObj.AddComponent<TextMeshProUGUI>();
            subTitleTMP.text = "— CA ĐÊM KINH HOÀNG • THE NIGHT SHIFT —";
            subTitleTMP.fontSize = 20f;
            subTitleTMP.fontStyle = FontStyles.Italic;
            subTitleTMP.color = new Color(0.70f, 0.74f, 0.80f);
            subTitleTMP.alignment = TextAlignmentOptions.Center;

            // Thanh gạch ngang phân cách
            GameObject dividerObj = new GameObject("Menu_Divider");
            dividerObj.transform.SetParent(overlay.transform, false);
            ResetUITransform(dividerObj);
            RectTransform divRt = dividerObj.AddComponent<RectTransform>();
            divRt.anchorMin = new Vector2(0.5f, 0.58f);
            divRt.anchorMax = new Vector2(0.5f, 0.58f);
            divRt.pivot = new Vector2(0.5f, 0.5f);
            divRt.anchoredPosition = Vector2.zero;
            divRt.sizeDelta = new Vector2(650f, 2.5f);
            Image divImg = dividerObj.AddComponent<Image>();
            divImg.color = new Color(0.8f, 0.2f, 0.2f, 0.7f);

            // Helper tạo nút Menu tối giản
            System.Func<string, string, Color, Vector2, Vector2, float, Button> CreateMenuButton = (btnName, labelText, accentColor, anchorPos, size, fontSize) =>
            {
                GameObject bObj = new GameObject(btnName);
                bObj.transform.SetParent(overlay.transform, false);
                ResetUITransform(bObj);
                RectTransform bRt = bObj.AddComponent<RectTransform>();
                bRt.anchorMin = anchorPos;
                bRt.anchorMax = anchorPos;
                bRt.pivot = new Vector2(0.5f, 0.5f);
                bRt.anchoredPosition = Vector2.zero;
                bRt.sizeDelta = size;

                Image bBg = bObj.AddComponent<Image>();
                bBg.color = new Color(0.10f, 0.12f, 0.16f, 0.95f);

                Button btn = bObj.AddComponent<Button>();
                ColorBlock cb = btn.colors;
                cb.normalColor = new Color(0.10f, 0.12f, 0.16f, 0.95f);
                cb.highlightedColor = accentColor;
                cb.pressedColor = accentColor * 0.8f;
                btn.colors = cb;

                Outline bOutline = bObj.AddComponent<Outline>();
                bOutline.effectColor = accentColor * 0.7f;
                bOutline.effectDistance = new Vector2(1.5f, -1.5f);

                GameObject tObj = new GameObject("Text");
                tObj.transform.SetParent(bObj.transform, false);
                ResetUITransform(tObj);
                RectTransform tRt = tObj.AddComponent<RectTransform>();
                tRt.anchorMin = Vector2.zero;
                tRt.anchorMax = Vector2.one;
                tRt.sizeDelta = Vector2.zero;

                TextMeshProUGUI bTMP = tObj.AddComponent<TextMeshProUGUI>();
                bTMP.text = labelText;
                bTMP.fontSize = fontSize;
                bTMP.fontStyle = FontStyles.Bold;
                bTMP.color = Color.white;
                bTMP.alignment = TextAlignmentOptions.Center;

                return btn;
            };

            // Đúng 2 nút bấm chuẩn theo yêu cầu: PLAY & QUIT
            Button btnPlay = CreateMenuButton("Btn_Play", "[>] BẮT ĐẦU CHƠI / PLAY", new Color(0.18f, 0.62f, 0.32f), new Vector2(0.5f, 0.45f), new Vector2(520f, 68f), 24f);
            Button btnQuit = CreateMenuButton("Btn_Quit", "[X] THOÁT GAME / QUIT", new Color(0.72f, 0.20f, 0.20f), new Vector2(0.5f, 0.34f), new Vector2(520f, 62f), 22f);

            // Dòng thông tin phiên bản ở cạnh dưới màn hình
            GameObject footerObj = new GameObject("Menu_Footer");
            footerObj.transform.SetParent(overlay.transform, false);
            ResetUITransform(footerObj);
            RectTransform footerRt = footerObj.AddComponent<RectTransform>();
            footerRt.anchorMin = new Vector2(0.5f, 0.05f);
            footerRt.anchorMax = new Vector2(0.5f, 0.05f);
            footerRt.pivot = new Vector2(0.5f, 0.5f);
            footerRt.anchoredPosition = Vector2.zero;
            footerRt.sizeDelta = new Vector2(800f, 30f);

            TextMeshProUGUI footerTMP = footerObj.AddComponent<TextMeshProUGUI>();
            footerTMP.text = "MIDNIGHT MART v1.2.0 • PSYCHOLOGICAL HORROR EXPERIENCE";
            footerTMP.fontSize = 14f;
            footerTMP.color = new Color(0.45f, 0.48f, 0.52f, 0.6f);
            footerTMP.alignment = TextAlignmentOptions.Center;

            // Wire serialized fields to MainMenuManager
            SerializedObject mSO = new SerializedObject(menuManager);
            SafeSetObject(mSO, "_mainMenuPanel", menuRoot);
            SafeSetObject(mSO, "_canvasGroup", canvasGroup);
            SafeSetObject(mSO, "_btnPlay", btnPlay);
            SafeSetObject(mSO, "_btnQuit", btnQuit);
            if (player != null) SafeSetObject(mSO, "_playerTransform", player.transform);
            if (homeSpawn != null) SafeSetObject(mSO, "_homeSpawnPoint", homeSpawn);
            SafeSetObject(mSO, "_playerHomeManager", Object.FindAnyObjectByType<PlayerHomeManager>());
            SafeSetObject(mSO, "_tutorialManager", Object.FindAnyObjectByType<TutorialManager>());
            mSO.ApplyModifiedProperties();
        }

        private static void CreateTutorialManager(CustomerNPC customer, GameObject envRoot, out TutorialManager mgr)
        {
            GameObject mgrObj = new GameObject("--- TUTORIAL MANAGER ---");
            mgr = mgrObj.AddComponent<TutorialManager>();

            SerializedObject so = new SerializedObject(mgr);
            SafeSetObject(so, "_firstCustomer", customer);
            SafeSetObject(so, "_lightsRoot", envRoot);
            so.ApplyModifiedProperties();
        }

        private static void CreateNightShiftManager(GameObject mainLights, Light emergencyLight, CustomerNPC customerPrefab, Transform doorPoint, Transform waitingPoint, GameObject returnBike, MotorbikeRideController bikeCtrl, out NightShiftManager nightMgr)
        {
            GameObject nightObj = new GameObject("--- NIGHT SHIFT MANAGER ---");
            nightMgr = nightObj.AddComponent<NightShiftManager>();

            // Khởi tạo Boss Quái vật Đêm 1: THE FLESH STALKER (Mô hình quái vật có khung xương Rigged FBX cao 3.4m)
            GameObject bossObj = new GameObject("Boss_Monster_TheFleshStalker");
            bossObj.transform.position = new Vector3(0f, 0f, 14.0f);

            GameObject bossModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Boss_Monster_Rigged.fbx");
            if (bossModelPrefab == null) bossModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Boss_True_Abyssal.obj");
            if (bossModelPrefab != null)
            {
                GameObject bossVisual = Object.Instantiate(bossModelPrefab, bossObj.transform);
                bossVisual.name = "Boss_Visual";
                bossVisual.transform.localPosition = Vector3.zero;
                bossVisual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                bossVisual.transform.localScale = Vector3.one; // Chiều cao chuẩn 3.4m thực
                RemoveCollidersRecursively(bossVisual);
            }
            else
            {
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                placeholder.name = "Boss_Placeholder";
                placeholder.transform.SetParent(bossObj.transform);
                placeholder.transform.localPosition = new Vector3(0f, 1.4f, 0f);
                placeholder.transform.localScale = new Vector3(1.2f, 2.8f, 1.2f);
                placeholder.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial("Mat_BossPlaceholder", new Color(0.2f, 0.05f, 0.05f), 0.7f);
            }

            CapsuleCollider bossCol = bossObj.AddComponent<CapsuleCollider>();
            bossCol.center = new Vector3(0f, 1.7f, 0f);
            bossCol.height = 3.4f;
            bossCol.radius = 1.0f;

            BossMonsterAI bossAI = bossObj.AddComponent<BossMonsterAI>();
            bossObj.SetActive(false); // Ẩn Boss khi bắt đầu, kích hoạt lúc 04:30 AM hoặc khi quái vật thoát

            // Tải mô hình nhân vật Nam, Nữ và Kẻ giả mạo có gắn xương (Rigged Humanoid FBX)
            GameObject maleModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Male_Rigged.fbx");
            if (maleModel == null) maleModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Male.obj");

            GameObject femaleModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Female_Rigged.fbx");
            if (femaleModel == null) femaleModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Female.obj");

            GameObject doppelModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Doppelganger_Rigged.fbx");
            if (doppelModel == null) doppelModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Customer_Doppelganger.obj");

            SerializedObject nightSO = new SerializedObject(nightMgr);
            SafeSetObject(nightSO, "_mainLightsRoot", mainLights);
            SafeSetObject(nightSO, "_emergencyRedLight", emergencyLight);
            SafeSetObject(nightSO, "_npcPrefab", customerPrefab);
            SafeSetObject(nightSO, "_bossPrefab", bossAI);
            SafeSetObject(nightSO, "_femaleCustomerModel", femaleModel);
            SafeSetObject(nightSO, "_maleCustomerModel", maleModel);
            SafeSetObject(nightSO, "_doppelgangerModel", doppelModel);
            SafeSetObject(nightSO, "_doorEntrancePoint", doorPoint);
            SafeSetObject(nightSO, "_counterWaitingPoint", waitingPoint);
            SafeSetObject(nightSO, "_returnMotorbikeProp", returnBike);
            SafeSetObject(nightSO, "_motorbikeRideController", bikeCtrl);
            nightSO.ApplyModifiedProperties();
        }

        private static void CreatePlayerHomeManager(Transform player, GameObject apartmentRoot, Transform doorHinge, GameObject homeBike, Transform roadStartPoint, MotorbikeRideController bikeCtrl, TutorialManager tutMgr, bool startFromHome)
        {
            GameObject homeMgrObj = new GameObject("--- HOME PROLOGUE MANAGER ---");
            PlayerHomeManager homeMgr = homeMgrObj.AddComponent<PlayerHomeManager>();

            SerializedObject so = new SerializedObject(homeMgr);
            SafeSetObject(so, "_playerTransform", player);
            SafeSetObject(so, "_apartmentRoomRoot", apartmentRoot);
            SafeSetObject(so, "_apartmentDoorHinge", doorHinge);
            SafeSetObject(so, "_homeMotorbikeProp", homeBike);
            SafeSetObject(so, "_roadStartPoint", roadStartPoint);
            SafeSetObject(so, "_motorbikeRideController", bikeCtrl);
            SafeSetObject(so, "_tutorialManager", tutMgr);
            SafeSetObject(so, "_storeChimeClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Door_Chime_DingDong.wav"));
            so.ApplyModifiedProperties();

            if (!startFromHome)
            {
                homeMgrObj.SetActive(false);
            }
        }

        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            if (obj == null) return;
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private static void RemoveCollidersRecursively(GameObject obj)
        {
            if (obj == null) return;
            Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                Object.DestroyImmediate(colliders[i]);
            }
        }

        private static void CreateWall(string name, Transform parent, Vector3 position, Vector3 scale, Material mat)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void SafeSetObject(SerializedObject so, string propertyName, UnityEngine.Object targetValue)
        {
            if (so == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.objectReferenceValue = targetValue;
            }
            else
            {
                Debug.LogWarning($"[StoreSceneBuilder] Cảnh báo: Thuộc tính '{propertyName}' không tồn tại trên '{so.targetObject?.GetType().Name}'");
            }
        }

        private static void SafeSetString(SerializedObject so, string propertyName, string targetValue)
        {
            if (so == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.stringValue = targetValue;
            }
            else
            {
                Debug.LogWarning($"[StoreSceneBuilder] Cảnh báo: Thuộc tính '{propertyName}' không tồn tại trên '{so.targetObject?.GetType().Name}'");
            }
        }

        private static void SafeSetBool(SerializedObject so, string propertyName, bool targetValue)
        {
            if (so == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.boolValue = targetValue;
            }
            else
            {
                Debug.LogWarning($"[StoreSceneBuilder] Cảnh báo: Thuộc tính '{propertyName}' không tồn tại trên '{so.targetObject?.GetType().Name}'");
            }
        }

        private static void SafeSetInt(SerializedObject so, string propertyName, int targetValue)
        {
            if (so == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.intValue = targetValue;
            }
            else
            {
                Debug.LogWarning($"[StoreSceneBuilder] Cảnh báo: Thuộc tính '{propertyName}' không tồn tại trên '{so.targetObject?.GetType().Name}'");
            }
        }

        private static void SafeSetFloat(SerializedObject so, string propertyName, float targetValue)
        {
            if (so == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.floatValue = targetValue;
            }
            else
            {
                Debug.LogWarning($"[StoreSceneBuilder] Cảnh báo: Thuộc tính '{propertyName}' không tồn tại trên '{so.targetObject?.GetType().Name}'");
            }
        }

        private static void SafeSetEnum(SerializedObject so, string propertyName, int targetValue)
        {
            if (so == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.enumValueIndex = targetValue;
            }
            else
            {
                Debug.LogWarning($"[StoreSceneBuilder] Cảnh báo: Thuộc tính '{propertyName}' không tồn tại trên '{so.targetObject?.GetType().Name}'");
            }
        }

        private static void SafeSetVector3(SerializedObject so, string propertyName, Vector3 targetValue)
        {
            if (so == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.vector3Value = targetValue;
            }
            else
            {
                Debug.LogWarning($"[StoreSceneBuilder] Cảnh báo: Thuộc tính '{propertyName}' không tồn tại trên '{so.targetObject?.GetType().Name}'");
            }
        }

        /// <summary>
        /// Tạo hoặc nạp Texture bầu trời đêm nhiều mây u ám và hàng nghìn ngôi sao lấp lánh (Equirectangular Lat-Long 2:1)
        /// </summary>
        private static Texture2D GetOrCreateStarryNightTexture()
        {
            string texturePath = "Assets/Textures/Sky_Night_Stars_And_Clouds.png";
            Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (existing != null) return existing;

            if (!System.IO.Directory.Exists("Assets/Textures"))
            {
                System.IO.Directory.CreateDirectory("Assets/Textures");
            }

            int width = 1024;
            int height = 512;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            // Bầu trời đêm sâu thẳm: Gradient từ thiên đỉnh (đen than/chàm đậm) xuống chân trời (xanh chàm mờ)
            Color zenithColor = new Color(0.003f, 0.005f, 0.012f, 1.0f);
            Color horizonColor = new Color(0.010f, 0.015f, 0.026f, 1.0f);
            Color cloudColor = new Color(0.018f, 0.024f, 0.038f, 1.0f);

            Color[] pixels = new Color[width * height];
            System.Random rand = new System.Random(42);

            for (int y = 0; y < height; y++)
            {
                float v = (float)y / height; // 0: Horizon, 1: Zenith
                Color baseSky = Color.Lerp(horizonColor, zenithColor, Mathf.Pow(v, 0.65f));

                for (int x = 0; x < width; x++)
                {
                    float u = (float)x / width;

                    // Mây đen u ám cuồn cuộn
                    float noise1 = Mathf.PerlinNoise(u * 5.2f, v * 3.8f);
                    float noise2 = Mathf.PerlinNoise(u * 11.5f + 4.2f, v * 9.1f + 1.7f) * 0.5f;
                    float cloudDensity = Mathf.Clamp01((noise1 + noise2 - 0.45f) * 1.6f);

                    Color pixel = Color.Lerp(baseSky, cloudColor, cloudDensity * 0.65f);
                    pixels[y * width + x] = pixel;
                }
            }

            // Gieo ~1,400 ngôi sao với độ sáng và tán sắc khác nhau
            int starCount = 1400;
            for (int i = 0; i < starCount; i++)
            {
                int sx = rand.Next(0, width);
                int sy = rand.Next((int)(height * 0.15f), height); // Sao tập trung nhiều ở nửa trên bầu trời
                float brightness = (float)rand.NextDouble();
                brightness = Mathf.Pow(brightness, 2.5f); // Nhiều sao nhỏ, ít sao lớn

                Color starColor;
                double colorType = rand.NextDouble();
                if (colorType < 0.65) starColor = new Color(0.85f, 0.92f, 1.0f); // Xanh trắng lạnh
                else if (colorType < 0.85) starColor = new Color(1.0f, 0.95f, 0.85f); // Trắng ấm
                else starColor = new Color(0.70f, 0.80f, 1.0f); // Xanh băng

                Color finalStar = starColor * (0.35f + brightness * 0.65f);

                int idx = sy * width + sx;
                pixels[idx] = Color.Lerp(pixels[idx], finalStar, finalStar.a);

                // Nếu là sao sáng: Thêm vầng sáng mềm 4 pixel xung quanh
                if (brightness > 0.65f)
                {
                    Color halo = finalStar * 0.35f;
                    if (sx > 0) pixels[sy * width + (sx - 1)] = Color.Lerp(pixels[sy * width + (sx - 1)], halo, 0.4f);
                    if (sx < width - 1) pixels[sy * width + (sx + 1)] = Color.Lerp(pixels[sy * width + (sx + 1)], halo, 0.4f);
                    if (sy > 0) pixels[(sy - 1) * width + sx] = Color.Lerp(pixels[(sy - 1) * width + sx], halo, 0.4f);
                    if (sy < height - 1) pixels[(sy + 1) * width + sx] = Color.Lerp(pixels[(sy + 1) * width + sx], halo, 0.4f);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            byte[] pngBytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(texturePath, pngBytes);
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.textureShape = TextureImporterShape.TextureCube;
                importer.generateCubemap = TextureImporterGenerateCubemap.Cylindrical;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.sRGBTexture = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        }

        /// <summary>
        /// Thiết lập toàn bộ bầu không khí đêm tối om, sương mù rùng rợn và bầu trời mây tối nhiều sao:
        /// - Tạo HDRP Global Volume với Fixed Exposure = 7.0 EV (TRIỆT TIÊU LỖI AUTO-EXPOSURE TỰ SÁNG TRẮNG ĐƯỜNG!)
        /// - Thiết lập sương mù Volumetric Fog xanh đen âm u dày đặc
        /// - Bầu trời đêm mây đen u ám và bầy sao lấp lánh (PhysicallyBasedSky)
        /// - Chuyển Camera sang nền bầu trời Skybox HDRP
        /// - Tắt hoàn toàn Directional Light (Ánh sáng duy nhất ngoài trời đến từ đèn đường & đèn pha xe máy)
        /// </summary>
        private static void SetupMysteriousPitchBlackNight(Camera mainCam)
        {
            // 1. Cấu hình Camera HDRP
            if (mainCam != null)
            {
                mainCam.clearFlags = CameraClearFlags.Skybox;
                mainCam.backgroundColor = Color.black;

                HDAdditionalCameraData hdCam = mainCam.GetComponent<HDAdditionalCameraData>();
                if (hdCam == null) hdCam = mainCam.gameObject.AddComponent<HDAdditionalCameraData>();
                hdCam.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
            }

            // 2. Thiết lập Ánh Trăng Đêm Dịu Nhẹ (Moonlight Directional Light)
            // Cung cấp ánh trăng xanh bạc mờ ảo soi rõ bóng dáng cây cối và lòng đường, không còn bị tối om mù mịt
            Light moonLight = null;
            Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include);
            foreach (Light l in allLights)
            {
                if (l != null && l.type == LightType.Directional)
                {
                    if (moonLight == null)
                    {
                        moonLight = l;
                    }
                    else
                    {
                        l.enabled = false;
                    }
                }
            }
            if (moonLight == null)
            {
                GameObject moonObj = new GameObject("Moonlight_Directional_Light");
                moonLight = moonObj.AddComponent<Light>();
                moonLight.type = LightType.Directional;
            }
            moonLight.enabled = true;
            moonLight.color = new Color(0.38f, 0.48f, 0.72f); // Ánh trăng xanh bạc rùng rợn
            moonLight.intensity = 0.04f; // Cường độ ánh trăng dịu nhẹ tạo bóng tối rừng đêm
            moonLight.transform.rotation = Quaternion.Euler(58f, -35f, 0f);

            // 3. Tạo bầu trời đêm mây tối và bầy sao lấp lánh
            Texture2D starTex = GetOrCreateStarryNightTexture();

            // 4. Tìm hoặc tạo Global Volume Profile cho Đêm kì bí
            GameObject volObj = GameObject.Find("Mysterious_Night_Global_Volume");
            if (volObj == null)
            {
                volObj = new GameObject("Mysterious_Night_Global_Volume");
            }
            Volume vol = volObj.GetComponent<Volume>();
            if (vol == null) vol = volObj.AddComponent<Volume>();
            vol.isGlobal = true;
            vol.priority = 100f;

            string profilePath = "Assets/Settings/SkyandFogSettingsProfile.asset";
            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            if (profile == null)
            {
                if (!System.IO.Directory.Exists("Assets/Settings"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Settings");
                }
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, profilePath);
            }
            vol.sharedProfile = profile;

            // 5. Cố định Exposure (Fixed Exposure = 9.0 EV) - Môi trường đêm chuẩn horror, không bị loá trắng
            if (!profile.TryGet<Exposure>(out Exposure exp))
            {
                exp = profile.Add<Exposure>(true);
            }
            exp.mode.Override(ExposureMode.Fixed);
            exp.fixedExposure.Override(9.5f);
            exp.compensation.Override(-0.4f);

            // 6. Sương mù dày đặc rùng rợn (Dark Eerie Fog) hắt màu xanh chàm u tối
            if (!profile.TryGet<Fog>(out Fog fog))
            {
                fog = profile.Add<Fog>(true);
            }
            fog.enabled.Override(true);
            fog.enableVolumetricFog.Override(true);
            fog.albedo.Override(new Color(0.035f, 0.050f, 0.085f)); // Sương mù xanh đêm mờ ảo
            fog.meanFreePath.Override(48.0f);
            fog.baseHeight.Override(0f);
            fog.maximumHeight.Override(14f);

            // 7. Visual Environment & Bầu trời HDRISky gắn texture cubemap mây đen bầy sao lấp lánh
            if (!profile.TryGet<VisualEnvironment>(out VisualEnvironment visualEnv))
            {
                visualEnv = profile.Add<VisualEnvironment>(true);
            }
            int skyID = 1; // HDRISky ID in HDRP is 1
            var skyAttr = System.Attribute.GetCustomAttribute(typeof(HDRISky), typeof(SkyUniqueID));
            if (skyAttr != null)
            {
                var fields = skyAttr.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fields.Length > 0 && fields[0].FieldType == typeof(int))
                {
                    skyID = (int)fields[0].GetValue(skyAttr);
                }
            }
            visualEnv.skyType.Override(skyID);
            visualEnv.skyAmbientMode.Override(SkyAmbientMode.Dynamic);

            if (!profile.TryGet<HDRISky>(out HDRISky hdriSky))
            {
                hdriSky = profile.Add<HDRISky>(true);
            }
            Cubemap starCube = AssetDatabase.LoadAssetAtPath<Cubemap>("Assets/Textures/Sky_Night_Stars_And_Clouds.png");
            if (starCube != null)
            {
                hdriSky.hdriSky.Override(starCube);
            }
            hdriSky.exposure.Override(0.5f);
            hdriSky.multiplier.Override(1.5f);

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();

            // Đồng bộ RenderSettings fallback cho Scene View
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.040f, 0.050f, 0.080f);
            RenderSettings.ambientIntensity = 0.35f;
            RenderSettings.reflectionIntensity = 0f;
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.025f, 0.035f, 0.060f);
            RenderSettings.fogDensity = 0.025f;
        }
    }
}
