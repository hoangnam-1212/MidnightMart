using System.Collections;
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Interactions;

namespace MidnightMart.Customer
{
    /// <summary>
    /// Điều khiển toàn bộ hành vi của Khách hàng (Người thật hoặc Quái vật giả dạng Doppelgänger).
    /// Hỗ trợ:
    /// - Di chuyển chính xác đến điểm đứng chờ trước quầy (KHÔNG đi xuyên bàn tính tiền).
    /// - Dị thường Đêm 1: Quái vật không có bóng đổ, đầu co giật nhẹ.
    /// - Phản ứng khi bị bắn: Bắn trúng Quái vật -> +$50 thưởng; Bắn nhầm Người thật -> Phạt 10% tiền.
    /// - Thống kê quái vật thoát ra ngoài để dồn dập tấn công lúc 03:30 AM cúp điện.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class CustomerNPC : MonoBehaviour
    {
        [Header("Thông tin Nhân vật")]
        [SerializeField] private string _customerName = "Ông lão khách quen";
        [SerializeField] private bool _isDoppelganger = false;
        [SerializeField] private float _health = 100f;
        [SerializeField] private float _maxHealth = 100f;

        [Header("Vị trí Di chuyển (Đảm bảo không xuyên vật thể)")]
        [SerializeField] private Transform _doorEntrancePoint;
        [SerializeField] private Transform _shelfBrowsingPoint;  // Điểm fallback
        [SerializeField] private Transform[] _shelfBrowsingPoints; // 3 Điểm lựa đồ tại 3 dãy kệ hàng
        [SerializeField] private Transform _counterWaitingPoint; // Điểm đứng phía trước quầy
        [SerializeField] private float _walkSpeed = 1.8f;

        [Header("Món hàng mua sắm (Hỗ trợ 3 - 4 món)")]
        [SerializeField] private ScannableItem _itemToBuy;
        [SerializeField] private System.Collections.Generic.List<ScannableItem> _itemsToBuy = new System.Collections.Generic.List<ScannableItem>();
        [SerializeField] private CustomerIDCard _idCard;
        [SerializeField] private Transform _counterItemPlacement;

        [Header("Thông tin Giao dịch")]
        [SerializeField] private float _cashGiven = 20.0f;

        [Header("Âm thanh & Lời thoại")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _doorBellSound;

        private bool _hasArrivedAtCounter;
        private bool _hasPaid;
        private bool _isLeaving;
        private bool _isDead;
        private Renderer _renderer;
        private Coroutine _twitchCoroutine;
        private Coroutine _patienceCoroutine;
        private int _reminderIndex = 0;
        private ProceduralCharacterAnimator _characterAnimator;

        public string CustomerName => _customerName;
        public CustomerIDCard IDCard => _idCard;
        public bool IsDoppelganger => _isDoppelganger;
        public bool HasArrivedAtCounter => _hasArrivedAtCounter;
        public bool HasPaid => _hasPaid;
        public bool IsLeaving => _isLeaving;
        public bool IsDead => _isDead;

        public static System.Collections.Generic.List<CustomerNPC> ActiveCustomers { get; } = new System.Collections.Generic.List<CustomerNPC>();

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _characterAnimator = GetComponent<ProceduralCharacterAnimator>();
            if (_characterAnimator == null)
            {
                _characterAnimator = gameObject.AddComponent<ProceduralCharacterAnimator>();
            }

            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col != null)
            {
                col.center = new Vector3(0f, 0.9f, 0f);
                col.height = 1.8f;
                col.radius = 0.35f;
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;

#if UNITY_EDITOR
            if (_doorBellSound == null)
            {
                _doorBellSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Door_Chime_DingDong.wav");
            }
#endif
            if (_doorBellSound == null)
            {
                _doorBellSound = ProceduralAudioGenerator.CreateDoorChimeClip();
            }

            // Đảm bảo ban đầu toàn bộ hàng hóa trong giỏ đều ẩn (người không)
            HideAllShoppingItems();
            _health = _maxHealth;
        }

        private void OnEnable()
        {
            if (!ActiveCustomers.Contains(this)) ActiveCustomers.Add(this);
            GameEvents.OnCheckoutCompleted += HandleCheckoutCompleted;
        }

        private void OnDisable()
        {
            ActiveCustomers.Remove(this);
            GameEvents.OnCheckoutCompleted -= HandleCheckoutCompleted;
            if (_twitchCoroutine != null) StopCoroutine(_twitchCoroutine);
            if (_patienceCoroutine != null) StopCoroutine(_patienceCoroutine);
        }

        /// <summary>
        /// Khởi tạo trạng thái của khách khi bước vào tiệm (Hỗ trợ 1 món đơn lẻ).
        /// </summary>
        public void SetupCustomer(string customerName, bool isDoppelganger, ScannableItem item, float cash)
        {
            var list = new System.Collections.Generic.List<ScannableItem>();
            if (item != null) list.Add(item);
            SetupCustomer(customerName, isDoppelganger, list, null, cash);
        }

        /// <summary>
        /// Khởi tạo trạng thái của khách với 3 - 4 món hàng và thẻ căn cước (Giai đoạn A).
        /// </summary>
        public void SetupCustomer(string customerName, bool isDoppelganger, System.Collections.Generic.List<ScannableItem> items, CustomerIDCard idCard, float cash)
        {
            _customerName = customerName;
            _isDoppelganger = isDoppelganger;
            _itemsToBuy = items ?? new System.Collections.Generic.List<ScannableItem>();
            _itemToBuy = _itemsToBuy.Count > 0 ? _itemsToBuy[0] : null;
            _idCard = idCard;
            _cashGiven = cash;
            _hasArrivedAtCounter = false;
            _hasPaid = false;
            _isLeaving = false;
            _isDead = false;
            _reminderIndex = 0;
            if (_patienceCoroutine != null) StopCoroutine(_patienceCoroutine);

            // Đảm bảo tuyệt đối khách không cầm hàng trên tay khi mới thiết lập
            HideAllShoppingItems();

            // Dấu hiệu Quái vật Đêm 1: Không có bóng đổ dưới sàn (No Shadow)
            UpdateShadowMode();

            // Nếu là quái vật: Kích hoạt hiệu ứng co giật nhẹ
            if (_isDoppelganger)
            {
                _twitchCoroutine = StartCoroutine(TwitchRoutine());
            }
        }

        /// <summary>
        /// Gán mô hình 3D trực quan tùy chỉnh (Nam, Nữ, Kẻ giả mạo từ Blender).
        /// </summary>
        public void ApplyVisualMesh(GameObject modelPrefab)
        {
            if (modelPrefab == null) return;

            MeshRenderer rootMR = GetComponent<MeshRenderer>();
            if (rootMR != null) rootMR.enabled = false;

            Transform oldVisual = transform.Find("VisualModel");
            if (oldVisual != null)
            {
                if (Application.isPlaying) Destroy(oldVisual.gameObject);
                else DestroyImmediate(oldVisual.gameObject);
            }

            GameObject visualObj = Instantiate(modelPrefab, transform);
            visualObj.name = "VisualModel";
            visualObj.transform.localPosition = Vector3.zero;
            visualObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // Model OBJ mặt quay về -Z, xoay 180 độ để đồng bộ với Unity forward (+Z)
            visualObj.transform.localScale = Vector3.one;

            // Xóa collider thừa trên model con để tránh xung đột vật lý
            Collider[] childCols = visualObj.GetComponentsInChildren<Collider>();
            for (int i = 0; i < childCols.Length; i++)
            {
                if (Application.isPlaying) Destroy(childCols[i]);
                else DestroyImmediate(childCols[i]);
            }

            UpdateShadowMode();
            if (_characterAnimator != null)
            {
                _characterAnimator.CacheBodyParts();
            }
        }

        public void UpdateShadowMode()
        {
            Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < allRenderers.Length; i++)
            {
                if (allRenderers[i] != null)
                {
                    allRenderers[i].shadowCastingMode = _isDoppelganger ? 
                        UnityEngine.Rendering.ShadowCastingMode.Off : 
                        UnityEngine.Rendering.ShadowCastingMode.On;
                }
            }
        }

        /// <summary>
        /// Hiệu ứng co giật dị thường của Quái vật giả dạng Đêm 1.
        /// </summary>
        private IEnumerator TwitchRoutine()
        {
            while (!_isDead)
            {
                yield return new WaitForSeconds(Random.Range(2.5f, 4.5f));
                if (_isDead) yield break;

                // Giật nhẹ góc quay
                transform.Rotate(0f, Random.Range(-15f, 15f), 0f);
                yield return new WaitForSeconds(0.1f);
                transform.Rotate(0f, Random.Range(-15f, 15f), 0f);
            }
        }

        /// <summary>
        /// Gán danh sách các điểm lựa đồ tại 3 dãy kệ hàng siêu thị.
        /// </summary>
        public void SetShelfBrowsingPoints(Transform[] points)
        {
            _shelfBrowsingPoints = points;
        }

        /// <summary>
        /// Kích hoạt chuỗi hành vi khách hàng bước vào tiệm.
        /// </summary>
        public void StartCustomerArrival()
        {
            // 1. Đảm bảo khách bước vào NGƯỜI KHÔNG, toàn bộ hàng hóa ẩn hoàn toàn
            HideAllShoppingItems();

            // 2. Đặt vị trí xuất phát ở ngoài vỉa hè trước cửa tiệm
            Vector3 spawnOutside = _doorEntrancePoint != null ? _doorEntrancePoint.position : new Vector3(0f, 0f, -7.5f);
            transform.position = spawnOutside;
            transform.rotation = Quaternion.identity; // Hướng mặt nhìn thẳng vào cửa tiệm (+Z)

            gameObject.SetActive(true);

            StartCoroutine(WalkToCounterRoutine());
        }

        /// <summary>
        /// Di chuyển từ ngoài cửa vào tiệm (người không) -> đến kệ hàng lựa đồ ("qua quầy lấy") -> đến quầy thanh toán mới đặt đồ ra ("vứt ra bàn").
        /// </summary>
        private IEnumerator WalkToCounterRoutine()
        {
            // Đảm bảo tuyệt đối người không khi bước vào cửa
            HideAllShoppingItems();
            if (_characterAnimator != null) _characterAnimator.IsWalking = true;

            // CHẶNG 0: Khách đi từ ngoài vỉa hè (Z = -7.5m) tiến qua cửa tự động cảm ứng (Z = -4.5m)
            Vector3 indoorEntrance = new Vector3(0f, transform.position.y, -4.5f);
            while ((transform.position - indoorEntrance).sqrMagnitude > 0.04f)
            {
                if (_isDead) yield break;
                transform.position = Vector3.MoveTowards(transform.position, indoorEntrance, _walkSpeed * Time.deltaTime);
                Vector3 look = new Vector3(indoorEntrance.x, transform.position.y, indoorEntrance.z);
                if ((look - transform.position).sqrMagnitude > 0.001f)
                {
                    transform.LookAt(look);
                }
                yield return null;
            }

            // Tiếng chuông cửa leng keng khi khách bước qua cửa kính vào tiệm
            if (_doorBellSound != null)
            {
                AudioSource.PlayClipAtPoint(_doorBellSound, transform.position);
            }

            // CHẶNG 1: Khách đi đến một kệ hàng ngẫu nhiên để lựa đồ (Vẫn người không cho tới khi lựa xong)
            Transform chosenShelf = null;
            if (_shelfBrowsingPoints != null && _shelfBrowsingPoints.Length > 0)
            {
                int randIndex = Random.Range(0, _shelfBrowsingPoints.Length);
                chosenShelf = _shelfBrowsingPoints[randIndex];
            }
            else if (_shelfBrowsingPoint != null)
            {
                chosenShelf = _shelfBrowsingPoint;
            }

            if (chosenShelf != null)
            {
                Vector3 shelfPos = chosenShelf.position;
                while ((transform.position - shelfPos).sqrMagnitude > 0.04f)
                {
                    if (_isDead) yield break;
                    transform.position = Vector3.MoveTowards(transform.position, shelfPos, _walkSpeed * Time.deltaTime);
                    Vector3 shelfLook = new Vector3(shelfPos.x, transform.position.y, shelfPos.z);
                    if ((shelfLook - transform.position).sqrMagnitude > 0.001f)
                    {
                        transform.LookAt(shelfLook);
                    }
                    yield return null;
                }

                // Dừng lại lựa đồ tại kệ trong 2.0 giây
                transform.position = shelfPos;
                yield return new WaitForSeconds(2.0f);
                if (_isDead) yield break;

                // Âm thanh nhặt đồ trên kệ siêu thị
                AudioClip popClip = ProceduralAudioGenerator.CreateItemPickupPopClip();
                if (popClip != null)
                {
                    AudioSource.PlayClipAtPoint(popClip, transform.position);
                }
            }

            // CHẶNG 2: Khách đi từ kệ tới điểm đứng chờ trước quầy tính tiền
            if (_counterWaitingPoint == null) yield break;

            Vector3 targetPos = _counterWaitingPoint.position;

            // ĐIỀU HƯỚNG THÔNG MINH TRÁNH XUYÊN KỆ:
            // Nếu khách đang đứng sâu trong các dãy kệ (Z > 0.5m), khách phải đi dọc hành lang ra lối đi ngang chính (Z = 0.5m) trước
            if (transform.position.z > 0.8f)
            {
                // Chặng 2a: Đi dọc lối đi giữa 2 kệ ra đến ngã ba lối đi ngang phía trước kệ (Z = 0.5m)
                Vector3 crossAisleExit = new Vector3(transform.position.x, transform.position.y, 0.5f);
                while ((transform.position - crossAisleExit).sqrMagnitude > 0.04f)
                {
                    if (_isDead) yield break;
                    transform.position = Vector3.MoveTowards(transform.position, crossAisleExit, _walkSpeed * Time.deltaTime);
                    Vector3 look = new Vector3(crossAisleExit.x, transform.position.y, crossAisleExit.z);
                    if ((look - transform.position).sqrMagnitude > 0.001f)
                    {
                        transform.LookAt(look);
                    }
                    yield return null;
                }

                // Chặng 2b: Rẽ ngang đi sang làn quầy thu ngân (X = targetPos.x, Z = 0.5m)
                Vector3 counterLaneEntry = new Vector3(targetPos.x, transform.position.y, 0.5f);
                while ((transform.position - counterLaneEntry).sqrMagnitude > 0.04f)
                {
                    if (_isDead) yield break;
                    transform.position = Vector3.MoveTowards(transform.position, counterLaneEntry, _walkSpeed * Time.deltaTime);
                    Vector3 look = new Vector3(counterLaneEntry.x, transform.position.y, counterLaneEntry.z);
                    if ((look - transform.position).sqrMagnitude > 0.001f)
                    {
                        transform.LookAt(look);
                    }
                    yield return null;
                }
            }

            // Chặng 2c: Tiến thẳng vào vị trí đứng chờ phía trước bàn quầy thu ngân
            while ((transform.position - targetPos).sqrMagnitude > 0.04f)
            {
                if (_isDead) yield break;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _walkSpeed * Time.deltaTime);
                Vector3 stepLook = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                if ((stepLook - transform.position).sqrMagnitude > 0.001f)
                {
                    transform.LookAt(stepLook);
                }
                yield return null;
            }

            // Dừng hẳn trước quầy và quay mặt nhìn trực diện vào bàn quầy và nhân viên thu ngân
            transform.position = targetPos;
            Vector3 counterLook = _counterItemPlacement != null ? _counterItemPlacement.position : (targetPos + Vector3.left);
            counterLook.y = transform.position.y;
            Vector3 toCounter = (counterLook - transform.position).normalized;
            if (toCounter.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(toCounter);
            }

            _hasArrivedAtCounter = true;
            if (_characterAnimator != null) _characterAnimator.IsWalking = false;

            // Dừng một nhịp ngắn trước khi đặt đồ lên bàn
            yield return new WaitForSeconds(0.35f);

            // CHẶNG 3: "Đến bàn thanh toán với vứt ra"
            // Đặt 3 - 4 món hàng và Thẻ Căn cước (ID Card) dàn hàng dọc trên mặt bàn quầy ở góc trái trống
            if (_counterItemPlacement == null)
            {
                GameObject placementObj = GameObject.Find("Counter_Item_Placement");
                if (placementObj != null) _counterItemPlacement = placementObj.transform;
            }
            Vector3 basePlacement = _counterItemPlacement != null ? _counterItemPlacement.position : (targetPos + Vector3.left * 0.7f + Vector3.up * 0.05f);
            Transform counterParent = _counterItemPlacement != null ? _counterItemPlacement.parent : null;
            
            if (_itemsToBuy != null && _itemsToBuy.Count > 0)
            {
                int count = _itemsToBuy.Count;
                float spacing = 0.18f;
                float startZ = -((count - 1) * spacing) * 0.5f;

                for (int i = 0; i < count; i++)
                {
                    ScannableItem item = _itemsToBuy[i];
                    if (item != null)
                    {
                        if (counterParent != null) item.transform.SetParent(counterParent, true);
                        item.gameObject.SetActive(true);
                        Vector3 itemPos = basePlacement + new Vector3(0f, 0f, startZ + i * spacing);
                        item.transform.position = itemPos;
                        item.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

                        // Cố định vật lý trên bàn, không cho rơi vãi hay cầm lên tay
                        Rigidbody rb = item.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                            rb.useGravity = false;
                        }
                    }
                }
            }
            else if (_itemToBuy != null)
            {
                if (counterParent != null) _itemToBuy.transform.SetParent(counterParent, true);
                _itemToBuy.gameObject.SetActive(true);
                _itemToBuy.transform.position = basePlacement;
                _itemToBuy.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                Rigidbody rb = _itemToBuy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }

            // Đặt Thẻ Căn cước (ID Card) nằm phẳng trên bàn ở góc trống cạnh hàng hóa (-0.16m về phía nhân viên)
            if (_idCard != null)
            {
                if (counterParent != null) _idCard.transform.SetParent(counterParent, true);
                _idCard.gameObject.SetActive(true);
                _idCard.transform.position = basePlacement + new Vector3(-0.16f, 0.01f, 0f);
                _idCard.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }

            // Âm thanh đặt hàng hóa lên bàn quầy
            AudioClip placeClip = ProceduralAudioGenerator.CreateItemPickupPopClip();
            if (placeClip != null)
            {
                AudioSource.PlayClipAtPoint(placeClip, basePlacement);
            }

            // Thoại chào hỏi và hướng dẫn người chơi quét mã
            if (_isDoppelganger)
            {
                GameEvents.TriggerSubtitle(_customerName, "Cho... tôi... lấy... những thứ này... (Giọng trầm khàn bất thường)", 4.0f);
            }
            else
            {
                GameEvents.TriggerSubtitle(_customerName, "Chào cậu. Cho tôi thanh toán những món này nhé.", 3.0f);
            }

            int totalCount = _itemsToBuy != null && _itemsToBuy.Count > 0 ? _itemsToBuy.Count : (_itemToBuy != null ? 1 : 0);
            GameEvents.TriggerObjectiveUpdated($"MỤC TIÊU: Quét mã vạch ({totalCount} món hàng trên quầy) & Soi thẻ ID");

            // Kích hoạt chuỗi kiên nhẫn: Khách kiên nhẫn đứng đợi, nhắc nhở nếu người chơi đi nghịch lâu
            if (_patienceCoroutine != null) StopCoroutine(_patienceCoroutine);
            _patienceCoroutine = StartCoroutine(CustomerPatienceRoutine());

            // Chờ người chơi quét mã món hàng để đưa tiền
            StartCoroutine(WaitAndPayRoutine());
        }

        /// <summary>
        /// Khách hàng đứng đợi kiên nhẫn tại quầy.
        /// - Nếu người chơi bỏ đi nghịch lâu (> 18s): Người thật sẽ ho nhẹ, giục tính tiền; Quái vật sẽ xoay đầu theo dõi rùng rợn và thì thầm.
        /// - Tuyệt đối KHÔNG bao giờ tự ý biến mất!
        /// </summary>
        private IEnumerator CustomerPatienceRoutine()
        {
            float timer = 0f;
            float reminderInterval = 18f;

            while (!_isLeaving && !_isDead)
            {
                yield return null;
                timer += Time.deltaTime;

                // Nếu là Quái vật Doppelgänger: Đầu/ánh mắt tự động xoay dõi theo bước chân của người chơi khắp tiệm (Head Tracking)
                if (_isDoppelganger && Camera.main != null)
                {
                    Vector3 playerPos = Camera.main.transform.position;
                    Vector3 lookTarget = new Vector3(playerPos.x, transform.position.y, playerPos.z);
                    if ((lookTarget - transform.position).sqrMagnitude > 0.05f)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(lookTarget - transform.position);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3.2f);
                    }
                }

                // Nếu người chơi mải nghịch chưa quét xong hoặc chưa thối tiền sau 18-22 giây
                if (timer >= reminderInterval)
                {
                    timer = 0f;
                    reminderInterval = Random.Range(16f, 22f);

                    if (!_isDead && !_isLeaving)
                    {
                        if (!_hasPaid)
                        {
                            if (_isDoppelganger)
                            {
                                string[] monsterLines = new string[]
                                {
                                    "...Sao lâu thế...? Ngươi đang tìm cái gì à...?",
                                    "...Mau lên... ta không chờ được lâu đâu...",
                                    "...Quét mã đi... hay là sợ...?"
                                };
                                string line = monsterLines[_reminderIndex % monsterLines.Length];
                                _reminderIndex++;
                                GameEvents.TriggerSubtitle(_customerName, line, 4.0f);
                            }
                            else
                            {
                                string[] civLines = new string[]
                                {
                                    "Này cậu thu ngân ơi? Tôi còn vội về, tính tiền giúp tôi với...",
                                    "Cậu đi đâu thế? Tôi vẫn đang chờ ở quầy này...",
                                    "Cậu ơi, tính tiền mấy món này giùm tôi với nhé."
                                };
                                string line = civLines[_reminderIndex % civLines.Length];
                                _reminderIndex++;
                                GameEvents.TriggerSubtitle(_customerName, line, 3.5f);
                            }
                        }
                        else
                        {
                            if (_isDoppelganger)
                            {
                                GameEvents.TriggerSubtitle(_customerName, "...Tiền thừa của ta đâu...? Mau thối tiền lại...", 4.0f);
                            }
                            else
                            {
                                GameEvents.TriggerSubtitle(_customerName, "Cậu ơi, thối tiền thừa giùm tôi với, tôi gửi tiền rồi nè.", 3.5f);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator WaitAndPayRoutine()
        {
            int totalCount = _itemsToBuy != null && _itemsToBuy.Count > 0 ? _itemsToBuy.Count : (_itemToBuy != null ? 1 : 0);

            while (!_isDead)
            {
                int scannedCount = 0;
                if (_itemsToBuy != null && _itemsToBuy.Count > 0)
                {
                    for (int i = 0; i < _itemsToBuy.Count; i++)
                    {
                        if (_itemsToBuy[i] != null && _itemsToBuy[i].IsScanned) scannedCount++;
                    }
                }
                else if (_itemToBuy != null && _itemToBuy.IsScanned)
                {
                    scannedCount = 1;
                }

                if (scannedCount >= totalCount)
                {
                    break;
                }

                yield return new WaitForSeconds(0.3f);
            }

            if (_isDead) yield break;

            yield return new WaitForSeconds(0.6f);

            // Tính tổng tiền toàn bộ các món hàng
            float total = 0f;
            if (_itemsToBuy != null && _itemsToBuy.Count > 0)
            {
                for (int i = 0; i < _itemsToBuy.Count; i++)
                {
                    if (_itemsToBuy[i] != null) total += _itemsToBuy[i].Price;
                }
            }
            else if (_itemToBuy != null)
            {
                total = _itemToBuy.Price;
            }

            if (_cashGiven < total)
            {
                _cashGiven = Mathf.Ceil((total + 2f) / 5f) * 5f;
            }

            // Khách đưa tiền mặt lên máy tính tiền
            if (!_isDead)
            {
                _hasPaid = true;
                GameEvents.TriggerCustomerPaid(total, _cashGiven);
                GameEvents.TriggerSubtitle(_customerName, $"Gửi cậu tờ ${_cashGiven:F0} tiền mặt nhé.", 2.5f);
                float change = Mathf.Max(0f, _cashGiven - total);
                GameEvents.TriggerObjectiveUpdated($"MỤC TIÊU: Nhấn [E] vào Máy tính tiền để thối lại ${change:F2} cho khách");
            }
        }

        /// <summary>
        /// Khi người chơi thối tiền xong, khách nhận hàng và rời đi.
        /// </summary>
        private void HandleCheckoutCompleted()
        {
            if (_isLeaving || _isDead) return;
            _isLeaving = true;
            if (_patienceCoroutine != null) StopCoroutine(_patienceCoroutine);

            StartCoroutine(DeliverDialogueAndLeaveRoutine());
        }

        private IEnumerator DeliverDialogueAndLeaveRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            if (_isDoppelganger)
            {
                GameEvents.TriggerSubtitle(_customerName, "...He he... Cảm ơn cậu... Hẹn gặp lại lúc 3h30...", 4.5f);
            }
            else
            {
                GameEvents.TriggerSubtitle(_customerName, "Cậu thanh niên... tối qua tôi thấy thằng David bị kéo vào rừng... cẩn thận đấy.", 5.0f);
            }

            yield return new WaitForSeconds(0.6f);

            // Thu dọn/xóa sạch hàng hóa và thẻ ID trên bàn quầy khi rời tiệm (không để lại rác trên bàn)
            CleanupCounterItems();

            if (_characterAnimator != null) _characterAnimator.IsWalking = true;

            // CHẶNG 1: Khách lùi bước, xoay người đi từ bàn quầy (X = -1.45m) ra trục hành lang giữa (X = 0m, Z = -3.2m)
            Vector3 aisleCenter = new Vector3(0f, transform.position.y, -3.2f);
            while ((transform.position - aisleCenter).sqrMagnitude > 0.05f)
            {
                if (_isDead) yield break;
                transform.position = Vector3.MoveTowards(transform.position, aisleCenter, _walkSpeed * Time.deltaTime);
                Vector3 lookTarget = new Vector3(aisleCenter.x, transform.position.y, aisleCenter.z);
                if ((lookTarget - transform.position).sqrMagnitude > 0.001f) transform.LookAt(lookTarget);
                yield return null;
            }

            // CHẶNG 2: Đi dọc hành lang chính tới thềm trong cửa kính tự động (X = 0m, Z = -4.8m)
            Vector3 doorThreshold = new Vector3(0f, transform.position.y, -4.8f);
            while ((transform.position - doorThreshold).sqrMagnitude > 0.05f)
            {
                if (_isDead) yield break;
                transform.position = Vector3.MoveTowards(transform.position, doorThreshold, _walkSpeed * Time.deltaTime);
                Vector3 lookTarget = new Vector3(doorThreshold.x, transform.position.y, doorThreshold.z);
                if ((lookTarget - transform.position).sqrMagnitude > 0.001f) transform.LookAt(lookTarget);
                yield return null;
            }

            // CHẶNG 3: Cửa trượt mở ra, khách đi xuyên qua cửa bước ra ngoài vỉa hè sương mù (Z = -6.5m)
            Vector3 doorExit = new Vector3(0f, transform.position.y, -6.5f);
            while ((transform.position - doorExit).sqrMagnitude > 0.08f)
            {
                if (_isDead) yield break;
                transform.position = Vector3.MoveTowards(transform.position, doorExit, _walkSpeed * Time.deltaTime);
                Vector3 lookTarget = new Vector3(doorExit.x, transform.position.y, doorExit.z);
                if ((lookTarget - transform.position).sqrMagnitude > 0.001f) transform.LookAt(lookTarget);
                yield return null;
            }

            // CHẶNG 4: Khách tiếp tục bước đi xa dần vào màn đêm sương mù sâu thẳm (Z = -14.0m)
            Vector3 deepMistExit = new Vector3(0.5f, transform.position.y, -14.0f);
            while ((transform.position - deepMistExit).sqrMagnitude > 0.12f)
            {
                if (_isDead) yield break;
                transform.position = Vector3.MoveTowards(transform.position, deepMistExit, _walkSpeed * Time.deltaTime);
                Vector3 lookTarget = new Vector3(deepMistExit.x, transform.position.y, deepMistExit.z);
                if ((lookTarget - transform.position).sqrMagnitude > 0.001f) transform.LookAt(lookTarget);
                yield return null;
            }

            if (_characterAnimator != null) _characterAnimator.IsWalking = false;

            // Dừng lại 3.5 giây trong sương mù mờ ảo ngoài tầm nhìn trước khi giải phóng
            yield return new WaitForSeconds(3.5f);

            // Nếu để Quái vật mua hàng thoát ra cửa: Tăng độ hung hãn của bầy quái lúc 03:30 AM!
            if (_isDoppelganger)
            {
                Night.NightShiftManager.Instance?.ReportMonsterEscaped();
            }

            gameObject.SetActive(false);

            // Báo cho Tutorial Manager (nếu đang ở màn hướng dẫn)
            Tutorial.TutorialManager.Instance?.ReportCustomerLeft();
        }

        private void HideAllShoppingItems()
        {
            Transform tutorialCart = transform.Find("Tutorial_Customer_Cart");
            if (tutorialCart != null)
            {
                tutorialCart.localPosition = new Vector3(0f, -50f, 0f);
            }
            Transform nightCart = transform.Find("Night_Customer_Cart");
            if (nightCart != null)
            {
                nightCart.localPosition = new Vector3(0f, -50f, 0f);
            }

            if (_itemsToBuy != null)
            {
                for (int i = 0; i < _itemsToBuy.Count; i++)
                {
                    if (_itemsToBuy[i] != null)
                    {
                        _itemsToBuy[i].gameObject.SetActive(false);
                        _itemsToBuy[i].transform.position = new Vector3(0f, -50f, 0f);
                    }
                }
            }
            if (_itemToBuy != null)
            {
                _itemToBuy.gameObject.SetActive(false);
                _itemToBuy.transform.position = new Vector3(0f, -50f, 0f);
            }
            if (_idCard != null)
            {
                _idCard.gameObject.SetActive(false);
                _idCard.transform.position = new Vector3(0f, -50f, 0f);
            }
        }

        private void CleanupCounterItems()
        {
            if (_itemsToBuy != null)
            {
                for (int i = 0; i < _itemsToBuy.Count; i++)
                {
                    if (_itemsToBuy[i] != null) Destroy(_itemsToBuy[i].gameObject);
                }
                _itemsToBuy.Clear();
            }
            if (_itemToBuy != null)
            {
                Destroy(_itemToBuy.gameObject);
                _itemToBuy = null;
            }
            if (_idCard != null)
            {
                Destroy(_idCard.gameObject);
                _idCard = null;
            }
            HideAllShoppingItems();
        }

        /// <summary>
        /// Xử lý sát thương khi bị súng bắn trúng.
        /// - Bắn vào đầu (Headshot): Hạ gục ngay lập tức chỉ với 1 viên!
        /// - Bắn vào thân (Body shot): Cần 2 viên mới bị tiêu diệt!
        /// - Kèm hiệu ứng máu bắn tung tóe và vũng máu loang dưới sàn.
        /// </summary>
        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 shotDirection, bool isHeadshot = false)
        {
            if (_isDead) return;

            // Sinh hiệu ứng máu bắn tung tóe và vũng máu loang dưới sàn
            SpawnBloodEffects(hitPoint, shotDirection, isHeadshot);

            float actualDamage = isHeadshot ? 120f : 55f;
            _health -= actualDamage;

            if (_health > 0f)
            {
                // Trúng thân viên đầu tiên: Flinch giật lùi và thông báo
                GameEvents.TriggerSubtitle("Trúng đạn", "<color=#FFAA00>Trúng thân (Phát 1/2)! Cần thêm 1 viên hoặc ngắm bắn chuẩn vào đầu!</color>", 2.0f);
                StartCoroutine(FlinchRoutine(shotDirection));
                return;
            }

            // Tiêu diệt thành công
            _isDead = true;
            if (_characterAnimator != null) _characterAnimator.IsWalking = false;
            if (_patienceCoroutine != null) StopCoroutine(_patienceCoroutine);
            StopAllCoroutines();

            if (isHeadshot)
            {
                GameEvents.TriggerSubtitle("XẠ THỦ!", "<color=#FF2222>★ HEADSHOT! Bắn trúng đầu - Hạ gục ngay lập tức!</color>", 3.0f);
            }

            if (_isDoppelganger)
            {
                // Ghi nhận ngầm tiêu diệt quái vật giả dạng (+50$ thưởng)
                GameEvents.TriggerMonsterKilled(50f);
            }
            else
            {
                // Ghi nhận ngầm bắn nhầm dân thường (phạt trừ 10% tiền)
                GameEvents.TriggerCivilianShot(0.10f);
            }

            // Ngã gục nằm xuống sàn nhà thực tế (KHÔNG thu nhỏ hay bóp méo scale)
            transform.position = new Vector3(transform.position.x, 0.12f, transform.position.z);
            transform.rotation = Quaternion.Euler(-90f, transform.eulerAngles.y, 0f);

            CleanupCounterItems();
            StartCoroutine(DisappearAfterDelay(5.0f));
        }

        private IEnumerator FlinchRoutine(Vector3 shotDir)
        {
            Vector3 push = shotDir.normalized * 0.15f;
            push.y = 0f;
            transform.position += push;
            yield return new WaitForSeconds(0.1f);
        }

        private void SpawnBloodEffects(Vector3 hitPoint, Vector3 shotDir, bool isHeadshot)
        {
            // 1. Vết máu loang trên mặt sàn gạch (Blood Puddle Decal)
            GameObject bloodPuddle = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bloodPuddle.name = "Blood_Puddle_Decal";
            bloodPuddle.transform.position = new Vector3(hitPoint.x, 0.015f, hitPoint.z);
            bloodPuddle.transform.rotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
            float puddleSize = isHeadshot ? Random.Range(0.75f, 1.15f) : Random.Range(0.45f, 0.70f);
            bloodPuddle.transform.localScale = new Vector3(puddleSize, puddleSize, 1f);

            Shader s = Shader.Find("HDRP/Lit") ?? Shader.Find("Standard");
            if (s != null)
            {
                Material bloodMat = new Material(s);
                bloodMat.color = new Color(0.42f, 0.02f, 0.02f, 0.95f);
                bloodPuddle.GetComponent<Renderer>().sharedMaterial = bloodMat;
            }
            Destroy(bloodPuddle.GetComponent<Collider>());
            Destroy(bloodPuddle, 40f);

            // 2. Tia máu bắn văng theo hướng phát đạn (Blood Spray Particles)
            GameObject bloodSpray = new GameObject("Blood_Spray_Burst");
            bloodSpray.transform.position = hitPoint;
            bloodSpray.transform.rotation = Quaternion.LookRotation(shotDir != Vector3.zero ? shotDir : Vector3.back);

            ParticleSystem ps = bloodSpray.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new Color(0.55f, 0.03f, 0.03f, 0.95f);
            main.startSize = isHeadshot ? 0.10f : 0.06f;
            main.startSpeed = isHeadshot ? 5.0f : 3.2f;
            main.startLifetime = 0.40f;
            main.loop = false;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, isHeadshot ? 30 : 15) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 25f;
            shape.radius = 0.08f;

            ps.Play();
            Destroy(bloodSpray, 1.0f);
        }

        private IEnumerator DisappearAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
