using System.Collections;
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Tutorial;

namespace MidnightMart.Prologue
{
    /// <summary>
    /// Quản lý Màn Mở Đầu tại Căn Hộ Của Người Chơi (Prologue at Player's Home).
    /// Người chơi bắt đầu trong căn phòng trọ nhỏ:
    /// - Đọc giấy đòi nợ tiền phòng $800 và tin nhắn tuyển dụng gấp (David ca trước mất tích bí ẩn).
    /// - Mở cửa lên xe đi làm trong đêm mưa bão.
    /// - Lắng nghe bản tin radio cảnh báo quái vật trên đường đi.
    /// - Cập bến siêu thị Midnight Mart để bắt đầu ca trực!
    /// </summary>
    public class PlayerHomeManager : MonoBehaviour
    {
        public static PlayerHomeManager Instance { get; private set; }

        [Header("Trạng thái Tiến trình")]
        [SerializeField] private bool _hasReadJobNotice = false;
        [SerializeField] private bool _isDoorOpen = false;
        [SerializeField] private bool _isTransitioningToStore = false;
        [SerializeField] private bool _canSleepForDay2 = false;

        private Quaternion _doorClosedRot;
        private Quaternion _doorOpenRot;
        private Coroutine _doorCoroutine;

        [Header("Cửa phòng trọ & Xe máy ngoài sân")]
        [SerializeField] private Transform _apartmentDoorHinge;
        [SerializeField] private GameObject _homeMotorbikeProp;

        [Header("Vị trí Dịch chuyển & Lái xe")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _storeEntranceArrivalPoint;
        [SerializeField] private GameObject _apartmentRoomRoot;

        [Header("Hành trình Lái xe máy qua Rừng Âm u (Motorbike Ride)")]
        [SerializeField] private Transform _roadStartPoint;
        [SerializeField] private MidnightMart.Player.MotorbikeRideController _motorbikeRideController;

        [Header("Tham chiếu Hệ thống")]
        [SerializeField] private TutorialManager _tutorialManager;
        [SerializeField] private AudioClip _storeChimeClip;

        public bool HasReadJobNotice => _hasReadJobNotice;
        public bool IsDoorOpen => _isDoorOpen;
        public bool CanSleepForDay2 => _canSleepForDay2;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (_apartmentDoorHinge != null)
            {
                _doorClosedRot = _apartmentDoorHinge.localRotation;
                _doorOpenRot = _doorClosedRot * Quaternion.Euler(0f, -75f, 0f);
            }
            else
            {
                _doorClosedRot = Quaternion.identity;
                _doorOpenRot = Quaternion.Euler(0f, -75f, 0f);
            }
        }

        private void Start()
        {
            // Bắt đầu màn mở đầu tại nhà chỉ khi MainMenu không còn mở
            if (MidnightMart.UI.MainMenuManager.Instance != null && MidnightMart.UI.MainMenuManager.Instance.IsMenuOpen)
            {
                return;
            }
            StartHomePrologue();
        }

        /// <summary>
        /// Khởi tạo mục tiêu đầu tiên khi người chơi thức dậy trong phòng trọ.
        /// </summary>
        public void StartHomePrologue()
        {
            GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Đọc giấy nợ tiền phòng & Tin nhắn việc làm trên bàn");
            GameEvents.TriggerTimeUpdated("11:30 PM");
            GameEvents.TriggerSubtitle("Độc thoại", "Hôm nay là hạn chót đóng tiền nhà... Mình phải tìm cách kiếm tiền gấp trong đêm nay.", 4.0f);
        }

        /// <summary>
        /// Khi người chơi nhấn E vào tờ giấy nợ & tin nhắn trên bàn.
        /// </summary>
        public void InteractWithNotice()
        {
            if (_hasReadJobNotice) return;
            _hasReadJobNotice = true;

            StartCoroutine(NoticeSequenceRoutine());
        }

        private IEnumerator NoticeSequenceRoutine()
        {
            GameEvents.TriggerSubtitle("Tin nhắn tuyển dụng", "CẦN GẤP NHÂN VIÊN CA ĐÊM ($50/GIỜ TRẢ TIỀN MẶT): Thằng nhóc David làm ca trước đột nhiên mất tích bỏ dở cửa hàng giữa rừng. Cần người trực thay ngay đêm nay từ 00:00!", 0f);
            GameEvents.TriggerSubtitle("Độc thoại", "David mất tích?... Lương 50 đô một giờ nhận tiền mặt. Quá vô lý cho một công việc bán hàng, nhưng mình không còn quyền lựa chọn.", 0f);
            GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Đi ra cửa phòng để mở cửa bước ra sân");
            yield break;
        }

        /// <summary>
        /// Khi người chơi nhấn E vào cửa phòng trọ: Hỗ trợ mở cửa ra sân hoặc đóng cửa lại!
        /// </summary>
        public void InteractWithDoor()
        {
            if (!_hasReadJobNotice)
            {
                GameEvents.TriggerSubtitle("Cửa phòng trọ", "Cửa đang khóa chặt! Hãy kiểm tra giấy nợ tiền nhà và tin nhắn việc làm trên bàn trước khi đi.", 3.5f);
                return;
            }

            _isDoorOpen = !_isDoorOpen;

            if (_doorCoroutine != null) StopCoroutine(_doorCoroutine);
            _doorCoroutine = StartCoroutine(AnimateDoorRoutine(_isDoorOpen));
        }

        private IEnumerator AnimateDoorRoutine(bool open)
        {
            if (_apartmentDoorHinge != null)
            {
                Quaternion startRot = _apartmentDoorHinge.localRotation;
                Quaternion targetRot = open ? _doorOpenRot : _doorClosedRot;
                float elapsed = 0f;

                while (elapsed < 1.0f)
                {
                    elapsed += Time.deltaTime * 3.5f;
                    _apartmentDoorHinge.localRotation = Quaternion.Slerp(startRot, targetRot, elapsed);
                    yield return null;
                }
                _apartmentDoorHinge.localRotation = targetRot;
            }

            if (open)
            {
                GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Bước ra sân và nhấn [E] lên xe máy");
                GameEvents.TriggerSubtitle("Độc thoại", "Cửa đã mở... Chiếc xe máy dựng sẵn ngoài sân, mau ra xe thôi.", 3.5f);
            }
            else
            {
                GameEvents.TriggerSubtitle("Cửa phòng trọ", "Đã đóng cửa phòng lại.", 2.0f);
            }
        }

        /// <summary>
        /// Khi người chơi bước ra sân và nhấn [E] vào chiếc xe máy.
        /// </summary>
        public void MountMotorbikeAndRide()
        {
            if (_isTransitioningToStore) return;
            _isTransitioningToStore = true;

            // Ẩn chiếc xe máy ngoài sân đi vì người chơi đã leo lên điều khiển
            if (_homeMotorbikeProp != null)
            {
                _homeMotorbikeProp.SetActive(false);
            }

            if (_motorbikeRideController != null && _roadStartPoint != null)
            {
                _motorbikeRideController.StartMotorbikeRide(_roadStartPoint.position);
            }
            else
            {
                // Fallback nếu không có bộ điều khiển xe máy
                StartCoroutine(DriveToStoreRoutine());
            }
        }

        /// <summary>
        /// Chuỗi chuyển cảnh fallback nếu không dùng xe máy.
        /// </summary>
        private IEnumerator DriveToStoreRoutine()
        {
            GameEvents.TriggerObjectiveUpdated("ĐANG RỜI KHỎI PHÒNG TRỌ...");
            GameEvents.TriggerSubtitle("Độc thoại", "Đêm nay mưa bão buốt giá... Phải nổ máy chạy xe đến siêu thị Midnight Mart ngay thôi.", 3.5f);

            yield return new WaitForSeconds(2.0f);

            // Ẩn phòng trọ đi để giải phóng tài nguyên
            if (_apartmentRoomRoot != null)
            {
                _apartmentRoomRoot.SetActive(false);
            }

            if (_motorbikeRideController != null && _roadStartPoint != null)
            {
                // Bắt đầu hành trình lái xe máy trên con đường u ám
                _motorbikeRideController.StartMotorbikeRide(_roadStartPoint.position);
            }
            else
            {
                // Fallback: Dịch chuyển trực tiếp tới trước cửa tiệm nếu không dùng xe máy
                if (_playerTransform != null && _storeEntranceArrivalPoint != null)
                {
                    CharacterController cc = _playerTransform.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    _playerTransform.position = _storeEntranceArrivalPoint.position;
                    _playerTransform.rotation = _storeEntranceArrivalPoint.rotation;

                    if (cc != null) cc.enabled = true;
                }

                // Tiếng chuông cửa leng keng khi đến tiệm
                if (_storeChimeClip == null)
                {
#if UNITY_EDITOR
                    _storeChimeClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Door_Chime_DingDong.wav");
#endif
                    if (_storeChimeClip == null) _storeChimeClip = ProceduralAudioGenerator.CreateDoorChimeClip();
                }

                if (_storeChimeClip != null && _storeEntranceArrivalPoint != null)
                {
                    AudioSource.PlayClipAtPoint(_storeChimeClip, _storeEntranceArrivalPoint.position);
                }

                GameEvents.TriggerTimeUpdated("00:00 AM");
                GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Đi lại cửa kính và nhấn [E] để mở cửa vào tiệm");
                GameEvents.TriggerSubtitle("Cửa hàng Midnight Mart", "Bạn đã đến trước cửa siêu thị Midnight Mart giữa đêm khuya vắng lặng.", 4.0f);

                // Kích hoạt Tutorial Manager nếu có
                if (_tutorialManager != null)
                {
                    _tutorialManager.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Gọi khi người chơi lái xe máy kết thúc ca trực trở về trước sân phòng trọ.
        /// </summary>
        public void HandleBikeArrivalAtHome()
        {
            if (_apartmentRoomRoot != null)
            {
                _apartmentRoomRoot.SetActive(true);
            }

            _canSleepForDay2 = true;
            _isTransitioningToStore = false;

            if (_homeMotorbikeProp != null)
            {
                _homeMotorbikeProp.SetActive(true);
                _homeMotorbikeProp.transform.position = new Vector3(0.8f, 0.08f, -282f);
            }

            GameEvents.TriggerObjectiveUpdated(LocalizationManager.T(
                "MỤC TIÊU: Bước vào phòng trọ và [E] Lên giường đi ngủ để qua Ngày 2",
                "OBJECTIVE: Enter room and [E] Sleep on bed to advance to Day 2"
            ));
            GameEvents.TriggerSubtitle(
                LocalizationManager.T("Độc thoại", "Monologue"),
                LocalizationManager.T("Về đến nhà rồi... Cần ngủ một giấc thật sâu trước khi ca trực tiếp theo bắt đầu.", "Finally back home... Need to sleep before the next night shift begins."),
                4.5f
            );
        }

        /// <summary>
        /// Khi người chơi nhấn [E] vào chiếc giường ngủ sau khi trở về từ ca trực Đêm 1.
        /// </summary>
        public void GoToSleepForDay2()
        {
            if (!_canSleepForDay2) return;
            _canSleepForDay2 = false;

            StartCoroutine(SleepTransitionRoutine());
        }

        private IEnumerator SleepTransitionRoutine()
        {
            GameEvents.TriggerDayTransitionStarted();

            GameEvents.TriggerSubtitle(
                LocalizationManager.T("Giấc ngủ ác mộng", "Nightmare"),
                LocalizationManager.T("Bạn chìm vào giấc ngủ nặng nề... Những cơn ác mộng về quái vật giữa rừng Blackwood bủa vây...", "You drift into a heavy sleep... Nightmares of shapeshifters in Blackwood forest haunt you..."),
                5.0f
            );

            yield return new WaitForSeconds(5.0f);

            // Nâng tiến trình sang Ngày 2
            GameEvents.TriggerDayAdvanced(2);

            if (MidnightMart.Night.NightShiftManager.Instance != null)
            {
                MidnightMart.Night.NightShiftManager.Instance.AdvanceToDay(2);
            }

            // Đặt người chơi đứng dậy cạnh giường
            if (_playerTransform != null)
            {
                CharacterController cc = _playerTransform.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                _playerTransform.position = new Vector3(0f, 1.0f, -289f);
                _playerTransform.rotation = Quaternion.identity;
                if (cc != null) cc.enabled = true;
            }

            GameEvents.TriggerTimeUpdated("11:30 PM");

            GameEvents.TriggerSubtitle(
                LocalizationManager.T("Báo thức điện thoại", "Phone Alarm"),
                LocalizationManager.T("11:30 PM: Tiếng chuông báo thức réo vang! Đã đến giờ chuẩn bị cho Ca trực Ngày 2.", "11:30 PM: Phone alarm rings! Time to get ready for Night Shift Day 2."),
                5.0f
            );

            GameEvents.TriggerObjectiveUpdated(LocalizationManager.T(
                "MỤC TIÊU: Bước ra sân và [E] Lên xe máy đi làm Ngày 2",
                "OBJECTIVE: Go to courtyard and [E] Mount motorbike for Day 2 shift"
            ));

            _isDoorOpen = true;
            _hasReadJobNotice = true;
            _isTransitioningToStore = false;
        }
    }
}

