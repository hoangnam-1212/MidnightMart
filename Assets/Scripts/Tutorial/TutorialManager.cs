using System.Collections;
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Customer;

namespace MidnightMart.Tutorial
{
    /// <summary>
    /// Điều khiển toàn bộ tiến trình hướng dẫn chơi (Interactive Tutorial Manager).
    /// Quản lý từng bước nhiệm vụ: Dọn dẹp sàn ➜ Phục vụ khách đầu tiên ➜ Kẹt ngăn kéo ➜ Phát hiện súng Shotgun ➜ Bắt đầu ca đêm.
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        public enum TutorialStep
        {
            CleanMess,
            ServeFirstCustomer,
            CheckStuckDrawer,
            EquipShotgun,
            Completed
        }

        [Header("Trạng thái Hiện tại")]
        [SerializeField] private TutorialStep _currentStep = TutorialStep.CleanMess;
        [SerializeField] private int _totalMessCount = 3;
        [SerializeField] private int _totalStainsCount = 3;

        [Header("Tham chiếu Môi trường & Khách")]
        [SerializeField] private CustomerNPC _firstCustomer;
        [SerializeField] private GameObject _lightsRoot;

        private int _currentMessCleaned;
        private int _currentStainsCleaned;
        private bool _hasReadBossMemo = false;
        private bool _hasCleaningMop = false;

        public TutorialStep CurrentStep => _currentStep;
        public bool HasReadBossMemo => _hasReadBossMemo;
        public bool HasCleaningMop { get => _hasCleaningMop; set => _hasCleaningMop = value; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Bắt đầu bước chuẩn bị: Dọn dẹp sàn nhà & Lau vệt bẩn
            StartTutorialStep(TutorialStep.CleanMess);
        }

        /// <summary>
        /// Gọi khi người chơi đọc tờ giấy note của Sếp dán ngoài cửa tiệm.
        /// </summary>
        public void ReportBossMemoRead()
        {
            _hasReadBossMemo = true;
            StartTutorialStep(TutorialStep.CleanMess);
        }

        /// <summary>
        /// Kích hoạt một bước nhiệm vụ mới và cập nhật giao diện người chơi.
        /// </summary>
        public void StartTutorialStep(TutorialStep step)
        {
            _currentStep = step;

            switch (_currentStep)
            {
                case TutorialStep.CleanMess:
                    UpdatePreShiftObjective();
                    GameEvents.TriggerSubtitle("Lời dặn Quản lý", "Chào người mới! Nhặt 3 món đồ bị đổ xếp lại lên kệ và lau sạch 3 vệt bẩn dưới sàn trước khi mở bán nhé.", 5f);
                    break;

                case TutorialStep.ServeFirstCustomer:
                    GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Tiến lại quầy & Phục vụ vị khách đầu tiên");
                    if (_firstCustomer != null)
                    {
                        _firstCustomer.StartCustomerArrival();
                    }
                    break;

                case TutorialStep.CheckStuckDrawer:
                    GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Ngăn kéo tiền bị kẹt! Hãy kéo mạnh ngăn kéo dưới quầy");
                    break;

                case TutorialStep.EquipShotgun:
                    GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Rút súng lục (Phím 2) & Nạp đạn (Phím R)");
                    break;

                case TutorialStep.Completed:
                    GameEvents.TriggerObjectiveUpdated("HƯỚNG DẪN HOÀN TẤT: 01:00 AM - CA ĐÊM BẮT ĐẦU!");
                    StartCoroutine(NightShiftCommenceRoutine());
                    break;
            }
        }

        private void UpdatePreShiftObjective()
        {
            if (!_hasCleaningMop)
            {
                GameEvents.TriggerObjectiveUpdated($"MỤC TIÊU: Nhặt Cây Lau Nhà ở góc cửa & Xếp đồ rơi ({_currentMessCleaned}/{_totalMessCount})");
            }
            else
            {
                GameEvents.TriggerObjectiveUpdated($"MỤC TIÊU: Chuẩn bị ca trực - Xếp đồ rơi ({_currentMessCleaned}/{_totalMessCount}) & Lau vệt bẩn ({_currentStainsCleaned}/{_totalStainsCount})");
            }
        }

        /// <summary>
        /// Gọi khi người chơi nhặt cây lau nhà tại xô nước góc tiệm.
        /// </summary>
        public void ReportCleaningMopPickedUp()
        {
            _hasCleaningMop = true;
            UpdatePreShiftObjective();
            GameEvents.TriggerSubtitle("Trang bị", "Đã cầm Cây Lau Nhà! Hãy tiến lại các vệt bẩn dưới sàn nhấn [E] để lau sạch.", 4f);
        }

        /// <summary>
        /// Báo cáo khi xếp xong 1 món đồ bừa bộn lên kệ.
        /// </summary>
        public void ReportMessCleaned()
        {
            if (_currentStep != TutorialStep.CleanMess) return;

            _currentMessCleaned++;
            GameEvents.TriggerMessCleaned(_currentMessCleaned, _totalMessCount);
            UpdatePreShiftObjective();

            CheckPreShiftCompletion();
        }

        /// <summary>
        /// Báo cáo khi lau sạch 1 vệt bẩn dưới sàn.
        /// </summary>
        public void ReportStainCleaned()
        {
            if (_currentStep != TutorialStep.CleanMess) return;

            _currentStainsCleaned++;
            UpdatePreShiftObjective();

            CheckPreShiftCompletion();
        }

        private void CheckPreShiftCompletion()
        {
            if (_currentMessCleaned >= _totalMessCount && _currentStainsCleaned >= _totalStainsCount)
            {
                // Hoàn thành cả 2 công việc chuẩn bị: Xếp đồ & Lau sàn
                GameEvents.TriggerSubtitle("Hoàn thành chuẩn bị", "Cửa hàng đã ngăn nắp và sạch sẽ! Tiến lại quầy thu ngân để chuẩn bị đón khách.", 4.5f);
                StartCoroutine(TransitionToCustomerRoutine());
            }
        }

        private IEnumerator TransitionToCustomerRoutine()
        {
            yield return new WaitForSeconds(1.5f);
            StartTutorialStep(TutorialStep.ServeFirstCustomer);
        }

        /// <summary>
        /// Báo cáo khi khách hàng đầu tiên đã mua đồ và rời đi.
        /// </summary>
        public void ReportCustomerLeft()
        {
            if (_currentStep == TutorialStep.ServeFirstCustomer)
            {
                StartTutorialStep(TutorialStep.CheckStuckDrawer);
            }
        }

        /// <summary>
        /// Báo cáo khi người chơi đã nhặt khẩu súng săn từ ngăn kéo.
        /// </summary>
        public void ReportShotgunAcquired()
        {
            GameEvents.TriggerShotgunUnlocked();
            StartTutorialStep(TutorialStep.EquipShotgun);
            StartCoroutine(FinishTutorialRoutine());
        }

        private IEnumerator FinishTutorialRoutine()
        {
            yield return new WaitForSeconds(3.0f);
            StartTutorialStep(TutorialStep.Completed);
        }

        private IEnumerator NightShiftCommenceRoutine()
        {
            yield return new WaitForSeconds(1.0f);

            // Hiệu ứng đèn chớp giật báo hiệu đêm chính thức bắt đầu
            if (_lightsRoot != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    _lightsRoot.SetActive(false);
                    yield return new WaitForSeconds(0.08f);
                    _lightsRoot.SetActive(true);
                    yield return new WaitForSeconds(0.08f);
                }
            }

            GameEvents.TriggerSubtitle("Hệ thống", "Đồng hồ điểm 01:00 AM. Ca trực đêm bắt đầu: Hãy cẩn trọng kẻ giả dạng!", 5f);
            
            yield return new WaitForSeconds(1.5f);
            // Kích hoạt Ca trực Đêm chính thức
            Night.NightShiftManager.Instance?.StartNightShift();
        }
    }
}

