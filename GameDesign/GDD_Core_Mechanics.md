# 📋 TÀI LIỆU THIẾT KẾ GAME (GAME DESIGN DOCUMENT - GDD)
## DỰ ÁN: MIDNIGHT MART - CA TRỰC ĐÊM KINH HOÀNG
**Thể loại:** Kinh dị tâm lý + Mô phỏng bán hàng (Tactile Shopkeeping) + Bắn súng góc nhìn thứ nhất (Tactical FPS Action)  
**Nền tảng:** PC (Steam / Itch.io)  
**Engine:** Unity 6 / HDRP (High Definition Render Pipeline)  
**Góc nhìn:** First-Person (Góc nhìn thứ nhất)  
**Quy mô phát triển:** Solo Developer (1 Lập trình viên)

---

## 1. TỔNG QUAN VÒNG LẶP CỐT LÕI (CORE GAMEPLAY LOOP)

Trò chơi mở đầu bằng phân cảnh đời sống bế tắc, tiếp nối bằng chuyến xe máy đêm xuyên qua rừng đen lạnh lẽo trước khi bước vào ca trực sinh tử từ **00:00 đến 06:00 sáng**:

```
[Khởi đầu: Phòng trọ bế tắc Z = -294m kín 100%, Giấy nợ $800 & Xe máy dựng ngoài sân]
       │
       ▼
[Chuyến xe máy đêm: Cung đường cong 270m tối om mịt mùng, 5 điểm mốc kinh dị qua rừng Blackwood]
       │
       ▼
[Tới tiệm, xe tự đỗ vỉa hè & Cửa kính khóa: Đọc Bản Ghi Nhớ Ca Đêm của Quản Lý trên cửa kính]
       │ (Mở khóa cửa kính cảm ứng trượt cuốn sang 2 bên: Leng Keng!)
       ▼
[Nhiệm vụ chuẩn bị: Xếp 3 món đồ rơi lên kệ (Kệ 3, 4, 5) & Lau 3 vệt bẩn sẫm màu dưới sàn]
       │
       ▼
[Giai đoạn 1: Bán hàng xúc giác 3 - 4 món & Soi Thẻ Căn Cước ID Card]
       │ (Khách kiên nhẫn đợi ở quầy, quái vật xoay đầu dõi theo người chơi nếu đi nghịch)
       ▼
[Giai đoạn 2: Tra cứu Máy tính Bàn Sau Quầy (Back Staff Desk - CSDL Công dân & Mua tiếp tế đạn)]
       │
       ├─ Đúng người ──> Bán hàng, nhận tiền mặt & thối tiền ──> Khách ra cửa tức thì (~1.9m)
       └─ Quái vật ────> Rút Súng Lục 9mm / Shotgun từ ngăn kéo bí mật tiêu diệt!
       │
       ▼
[Giai đoạn 3: Đỉnh điểm 03:30 AM - Cúp điện toàn phần & Bầy quái vây tiệm]
       │
       ▼
[Giai đoạn 4: 04:30 AM - BÁO ĐỘNG QUÁI TRÙM CA ĐÊM (MINI-BOSS DEFENSE)]
       │ (Thanh đếm ngược 30s chuẩn bị ➜ Trùm dị biến xuất hiện ➜ Kiting bắn súng quanh kệ hàng)
       │
       ▼
[Giai đoạn 5: 06:00 AM Sáng - Tổng kết ca] ──(Bảng lương, thưởng diệt quái, trừ phạt bắn nhầm)
       │
       ▼
[Lái xe máy về phòng trọ & Lên giường ngủ ──> Chuyển sang Đêm tiếp theo (Độ khó leo thang Đêm 1 - 7)]
       │
       ▼ (Sau khi sống sót qua trọn vẹn 7 Đêm)
[SÁNG NGÀY THỨ 8: HỐ SỤT KHỔNG LỒ XUẤT HIỆN GIỮA SÀN SIÊU THỊ (THE SINKHOLE EVENT)]
       │
       ├─ Lựa chọn A: NHẢY XUỐNG HỐ ──> Đấu Trùm Cội Nguồn dưới lòng đất ──> [TRUE ENDING: Ánh Bình Minh]
       │                                (Cơ chế né đòn AOE, Bullet-Hell & Bắn điểm yếu Parry)
       │
       └─ Lựa chọn B: LÁI XE BỎ CHẠY ─> Trở về nhà trọ đóng cửa trốn ───────> [BAD ENDING: Ngày Tận Thế]
                                        (Quái vật tràn ngập thành phố, phá nát cửa nhà)
```

---

## 2. CHI TIẾT CÁC CƠ CHẾ CHÍNH (CORE MECHANICS)

### 2.1. CĂN HỘ KHỞI ĐẦU & CUNG ĐƯỜNG RỪNG CONG 270M TỐI OM BÍ ẨN
- **Căn hộ người chơi (Apartment Prologue - $Z = -294.0\text{m}$):**
  - Phòng trọ kích thước $5\text{m} \times 4\text{m}$, cao $2.8\text{m}$.
  - Thiết kế tường trước và khung cửa kim loại kín khít $100\%$ với nẹp viền bảo vệ, loại bỏ triệt để hiện tượng hở khe tường nhìn xuyên ra ngoài.
  - Trên bàn làm việc có tờ giấy đòi nợ tiền nhà $\$800$, điện thoại nhận tin nhắn tuyển dụng từ Quản lý siêu thị, và chùm chìa khóa xe máy.
  - Cửa phòng mở xoay $75^\circ$ hé lộ khoảng sân hiên nơi chiếc xe máy cũ đang dựng sẵn đón người chơi nổ máy xuất phát.
- **Cung đường rừng cong 270m ($Z \in [-280.0\text{m}, -10.0\text{m}]$):**
  - Con đường dài gấp đôi (270m), uốn lượn hình chữ S mềm mại qua rừng rậm Blackwood.
  - **Bầu không khí tối đen tuyệt đối (Pitch-Black Horror):** Toàn bộ Directional Light bị ngắt, Ambient Light $= 0$, sương mù đen dày đặc (`fogDensity = 0.040`). Ánh sáng duy nhất bên ngoài đến từ **luồng đèn pha xe máy (280f, góc 40°, tầm 24m)** và các cột đèn đường chập chờn ven đường.
  - **5 Cột mốc kinh dị dọa người chơi dọc đường:**
    1. $Z = -255\text{m}$: Biển gỗ cảnh báo khu rừng nguy hiểm bị cào máu sâu hoắm.
    2. $Z = -210\text{m}$: Biển báo tam giác vàng *"CẨN THẬN: QUÁI VẬT BĂNG ĐƯỜNG"*.
    3. $Z = -165\text{m}$: Bù nhìn cành cây ma quái (Totem) đứng lấp ló trong rừng với đôi mắt đỏ rực le lói nhìn trừng trừng theo xe.
    4. $Z = -125\text{m}$: Khúc cua tử thần với xe tuần tra cảnh sát lật nghiêng bỏ hoang dưới mương, đèn ưu tiên màu cam chớp nháy cô độc.
    5. $Z = -80\text{m}$: Tấm biển chào Hạt Blackwood bị gạch xóa vẽ đè bằng máu thành $4,118$...

---

### 2.2. BẢN GHI NHỚ CỦA QUẢN LÝ & NHIỆM VỤ CHUẨN BỊ CA TRỰC
- **Bản Ghi Nhớ Dán Cửa Kính (Boss Shift Memo Sticky Note):**
  - Khi xe máy tự đỗ vào vỉa hè ($Z = -8.5\text{m}$), cửa kính siêu thị ở trạng thái khóa chặt chốt cảm biến.
  - Người chơi phải tiến lại tờ giấy note màu vàng dán trên cửa kính nhấn `[E]` để đọc.
  - Nội dung giải thích: Nhân viên David ca trước đã mất tích bí ẩn; giao 2 việc phải làm trước khi đón khách lúc 01:00 AM; cảnh báo quái vật dị biến không bóng đổ; hướng dẫn dùng máy tính bàn sau và súng trong ngăn kéo quầy.
  - Đọc xong, chốt cửa kêu cạch, hai cánh cửa kính tự động trượt cuốn sang 2 bên êm ái, chuông leng keng vang lên.
- **Nhiệm vụ Chuẩn bị Ca trực (Pre-Shift Checklist):**
  - **Nhiệm vụ 1: Xếp 3 món đồ rơi lên kệ (0/3):**
    - Nhặt *Gói bánh Snack Đỏ* xếp vào Kệ 3 ($X = -1.5\text{m}$).
    - Nhặt *Chai nước ngọt Xanh* xếp vào Kệ 4 ($X = 1.5\text{m}$).
    - Nhặt *Hộp ngũ cốc Vàng* xếp vào Kệ 5 ($X = 4.5\text{m}$).
    - Ô trống được thiết kế khớp tuyệt đối, hàng xếp xong nằm gọn sau gờ chắn chống rơi đổ, không đè lên hàng có sẵn.
  - **Nhiệm vụ 2: Lau 3 vệt bẩn sẫm màu dưới sàn (0/3):**
    - Sử dụng cây lau nhà tại xô nước góc trước tiệm hoặc nhấn `[E]` vào từng vết bẩn.
    - Vết bẩn 1: Gần lối vào cửa chính ($Z = 0.8\text{m}$).
    - Vết bẩn 2: Lối đi giữa Kệ 2 và 3 ($X = -3.0\text{m}$).
    - Vết bẩn 3: Lối đi bên phải giữa Kệ 4 và 5 ($X = 3.0\text{m}$).
    - Vết bẩn phát tiếng chà sàn sột soạt, co nhỏ dần và biến mất.
  - Khi cả hai nhiệm vụ hoàn thành ($3/3$ & $3/3$), đồng hồ chuyển sang **01:00 AM**, mở khóa đón khách.

---

### 2.3. BỐ TRÍ KHÔNG GIAN SIÊU THỊ 440M² VỚI 6 DÃY KỆ HÀNG GONDOLA
- **Diện tích sàn rộng gấp đôi:** Kích thước $22\text{m} \times 20\text{m} = 440\text{m}^2$, trần cao $3.8\text{m}$ với 9 dàn đèn huỳnh quang phân bổ đều khắp tiệm.
- **Dãy 6 kệ hàng Gondola kim loại phân bổ đều đặn:**
  - Tọa độ trục X: $X = -7.5\text{m}, -4.5\text{m}, -1.5\text{m}, 1.5\text{m}, 4.5\text{m}, 7.5\text{m}$.
  - Chiều dài mỗi kệ $7.2\text{m}$, rộng $0.9\text{m}$.
  - Khoảng cách lối đi giữa các kệ rộng **$2.1\text{m}$** cực kỳ thoáng đãng, cân xứng hai bên, tối ưu cho góc nhìn FPS và thả diều Boss.
  - Mép ngoài mỗi tầng mâm có **thanh gờ chắn kim loại cao 6cm** kèm nẹp bảng giá, giữ hàng hóa chắc chắn chống rung đổ.
- **Dãy 6 tủ mát nước ngọt (Drink Coolers):** Trải dọc tường phải ($X = 10.75\text{m}$), phát ánh sáng neon xanh dương ma mị.

---

### 2.4. QUẦY THU NGÂN GỌN GÀNG & BÀN NHÂN VIÊN PHÍA SAU
- **Tách biệt 2 khu vực chức năng:**
  - **Quầy Thu ngân Phía Trước ($X = -2.4\text{m}, Z = -3.2\text{m}$):** Chỉ để máy tính tiền, máy quét mã vạch, khay đặt hàng của khách và ngăn kéo bí mật. Đã xóa bỏ kệ thuốc lá lơ lửng và chuyển máy tính cồng kềnh sang bàn sau, giúp mặt quầy gọn gàng, thoáng tầm mắt.
  - **Bàn Nhân Viên Phía Sau (`Back_Staff_Desk` tại $X = -4.3\text{m}, Z = -3.2\text{m}$):** Bàn làm việc riêng biệt kê sát tường sau quầy với đèn bàn amber ấm áp, thùng case PC và màn hình CRT.
- **Trạm Máy tính Cửa hàng (Store Computer Terminal):**
  - Người chơi tiếp cận bàn sau nhấn `[E]` để sử dụng giao diện Midnight Mart OS v2.4 (mở khóa chuột, nhấn `[ESC]` hoặc `[E]` để thoát).
  - Khắc phục triệt để lỗi không mua được đồ: Hệ thống EventSystem với InputSystemUIInputModule, giao diện phân tách root luôn Active, chống đóng nhầm bằng debounce 0.35s.
  - **Tab 1: CSDL Tra cứu Công dân (`[F1]` / `[1]`):** Tự động đồng bộ hồ sơ khách đang đứng trước quầy. Phát hiện đối tượng dị biến có mã số tử vong, năm sinh bất thường và ra lệnh tiêu diệt.
  - **Tab 2: Cửa hàng Tiếp tế Đêm (`[F2]` / `[2]`):**
    - Gói Đạn 9mm ($\$15$): +10 viên đạn súng lục.
    - Hộp Đạn Shotgun ($\$30$): +4 viên đạn 12-Gauge.
    - Nước Tăng Lực ($\$12$): Hồi phục $100\%$ thể lực Sprint.
    - Thanh Gỗ Chặn Cửa ($\$25$): Gia cố phòng thủ.
    - Tự động trừ tiền quỹ tiệm theo thời gian thực.

---

### 2.5. CƠ CHẾ BÁN HÀNG XÚC GIÁC & QUY TẮC KHÁCH ĐỢI TẠI QUẦY
- **Khách hàng mang từ 3 - 4 món đồ lên quầy:** Khách đặt ngay ngắn các món hàng đặc trưng kèm Thẻ Căn cước công dân (ID Card).
- **Vị trí và Hướng mặt của Khách:** Đứng tại $X = -1.45\text{m}, Y = 0.0\text{m}, Z = -3.2\text{m}$, hai chân chạm sàn vững vàng, xoay người nhìn trực diện vào thu ngân (khắc phục hoàn toàn lỗi quay lưng vào bàn).
- **⚠️ Quy tắc Khách kiên nhẫn đợi tại quầy (Anti-Despawn):**
  - Khách **tuyệt đối không bao giờ tự ý biến mất**. Khách kiên nhẫn đợi ở quầy cho đến khi giao dịch hoàn tất hoặc bị nổ súng hạ gục.
  - Khi người chơi đi nghịch lâu (> 18s): Người thật ho nhẹ giục thanh toán; Quái vật Doppelgänger tự động xoay đầu dõi theo bước chân người chơi khắp tiệm (Head Tracking) và thì thầm rùng rợn.
- **Lối ra tức thì (~1.9m):** Ngay sau khi thanh toán xong (sau 0.6s), khách quay người bước thẳng ra ngoài cửa trượt tự động (khoảng cách chỉ ~1.9m) và biến mất vào màn đêm, không còn nấn ná ở quầy.

---

### 2.6. HỆ THỐNG KHO ĐỒ ĐỘNG & VŨ KHÍ CHIẾN ĐẤU FPS (DYNAMIC INVENTORY SYSTEM)
- **Cơ chế Kho đồ Động 3 Slot Linh Hoạt (Món nào nhặt trước ở Ô 1, các món sau lần lượt):**
  - Không gán cứng cố định (súng không mặc định ở Ô 2): Người chơi nhặt Cây Lau Nhà trước thì Cây Lau Nhà ở Ô 1; nhặt Súng trước thì Súng ở Ô 1, các món tiếp theo tự động lấp đầy các ô trống kế tiếp theo đúng thứ tự.
  - Phím `[1]`, `[2]`, `[3]`: Chọn trực tiếp Ô 1, Ô 2, Ô 3.
  - Phím `[Q]` hoặc Con lăn chuột (`Mouse Scroll`): Luân chuyển nhanh giữa các ô trang bị.
  - Hiển thị UI 3 Ô vuông ở đáy màn hình với nhãn phím `[1]`, `[2]`, `[3]` và màu vàng nổi bật cho ô đang kích hoạt.
- **Dụng cụ Cây Lau Nhà (Cleaning Mop) & Xô Nước:**
  - Đặt tại góc trước bên phải siêu thị ($X = 4.2\text{m}, Z = -3.8\text{m}$), cách xa cửa chính ($X = 0\text{m}$) và bản ghi nhớ note ($X = -1.55\text{m}$), chống hoàn toàn lỗi tương tác nhầm khi đọc note.
  - Khi nhặt, Cây Lau Nhà hiện lên ô kho đồ đầu tiên, kèm mô hình FPS trên tay nhân vật.
  - Người chơi tiến lại các vệt bẩn dưới sàn nhấn `[E]` để chà sạch.
- **Khẩu Súng Lục 9mm (Pistol) trong Ngăn kéo Bí mật:**
  - Lấy từ ngăn kéo kẹt sau khi phục vụ vị khách đầu tiên cùng chiếc điện thoại vỡ của David.
  - Tự động vào ô trống tiếp theo trong kho đồ động (ví dụ Ô 2 nếu đã nhặt chổi lau trước đó).
  - Băng đạn: 5 viên trong nòng, 15 viên dự trữ (`5 / 15`).
  - Phím số tương ứng: Rút / Cất súng. Phím `[R]`: Nạp đạn (1.2s). Chuột trái: Bắn chính xác. Khóa bắn khi hết cả 2 (`0 / 0`).
- **Khẩu Shotgun 2 nòng (Double-Barrel Shotgun 12-Gauge):**
  - Vũ khí hạng nặng mở khóa qua máy tính tiếp tế hoặc ngăn tủ kho.
  - Sát thương cực lớn ở cự ly gần, đẩy lùi quái vật và phá giáp Boss.

---

### 2.7. CUỘC CHẠM TRÁN QUÁI TRÙM ĐÊM 1 (THE FLESH STALKER)
- **Điều kiện kích hoạt:** 04:30 AM (hoặc sớm hơn nếu để kẻ giả mạo thoát ra ngoài cửa).
- **Thanh Đếm ngược Chuẩn bị 30 Giây (Countdown Bar):**
  - Thanh màu vàng cam xuất hiện ở đỉnh màn hình: `⚠️ CẢNH BÁO: CON TRÙM ĐANG ĐẾN! CHUẨN BỊ VŨ KHÍ (30s)`.
  - Loa phát thanh rú còi báo động khẩn cấp, người chơi có 30 giây mua thêm đạn ở máy tính bàn sau và chọn vị trí nép góc.
- **Trùm Xuất hiện & Thanh Máu Boss (Boss Health Bar):**
  - Cửa kho sau bật tung, Boss khổng lồ cao 2.8m xông vào tiệm gầm vang trời.
  - Thanh máu Boss đồ sộ ở đỉnh màn hình: `THE FLESH STALKER - TRÙM DỊ BIẾN ĐÊM 1 [ 1000 / 1000 HP ]`.
- **Lối chơi Kiting FPS trong Không gian 440m²:**
  - 6 dãy kệ hàng Gondola tạo nên các vòng lặp di chuyển (looping routes) hoàn hảo để người chơi luồn lách, chạy nước rút, thả diều và bắn hạ Boss.
  - Hạ gục Boss thưởng nóng **+$200** vào quỹ tiệm và mở khóa kết ca an toàn lúc 06:00 AM.

---

### 2.8. VÒNG LẶP SỐNG SÓT: LÁI XE VỀ NHÀ, LÊN GIƯỜNG NGỦ & SANG NGÀY 2 (DAY TRANSITION & SLEEP LOOP)
- **Hoàn thành Ca trực 06:00 AM:**
  - Hiển thị bảng tổng kết tiền lương, thưởng diệt quái, trừ phạt bắn nhầm.
  - Nhấn `[Space]` đóng bảng tổng kết: Mở khóa chiếc xe máy dựng sẵn ngoài lề đường vỉa hè siêu thị (`Return_Motorbike_Prop`).
  - Gợi ý tương tác: `[E] Lên xe máy lái về nhà nghỉ ngơi` (`[E] Mount motorbike & ride home`).
- **Chuyến Lái Xe Ngược Về Nhà (Return Night Ride):**
  - Chuyển góc nhìn tay lái xe máy FPS hướng về phía Nam (+Z giảm dần, từ $Z = -8\text{m}$ về $Z = -280\text{m}$).
  - Giữ `[W]` tăng tốc, `[A]`/`[D]` điều hướng an toàn trong lòng đường dưới ánh sáng của 14 cột đèn đường và sương sớm.
  - Về tới $Z \le -276\text{m}$, xe tự động tấp vào hiên phòng trọ (`Z = -282\text{m}`), người chơi xuống xe và phòng trọ kích hoạt trở lại.
- **Tương tác Chiếc Giường Ngủ (Apartment Bed Interaction):**
  - Người chơi bước vào phòng trọ, tiến lại chiếc giường: `[E] Lên giường đi ngủ (Chuyển sang Ngày 2)`.
  - Màn hình mờ đen (Fade to black), kích hoạt chuỗi chuyển cảnh ác mộng tâm lý.
  - Chuông điện thoại báo thức reng inh ỏi lúc **11:30 PM**, người chơi thức dậy bước sang **Ngày 2 (Day 2)**.
- **Leo thang Độ khó Ngày 2 (Day 2 Escalation):**
  - Khách hàng yêu cầu 4 - 5 món hàng.
  - Kẻ giả mạo có dị biến tinh vi hơn (mã ID bị tẩy xóa, năm sinh chênh lệch).
  - Cúp điện sớm hơn lúc 03:00 AM, quái vật hung hãn hơn.

---

### 2.9. NÂNG CẤP ÁNH SÁNG & GIAO DIỆN CHỮ GẤP ĐÔI (LIGHTING & ACCESSIBLE UI)
- **Cân bằng Ánh sáng Đêm U Ám (Mysterious Night Exposure):**
  - Cố định HDRP Fixed Exposure tại **2.2 EV** (thay vì 5.4 EV quá tối) giúp mắt người chơi nhìn rõ lòng đường, hàng cây và các biển báo, trong khi bóng tối vẫn u ám rùng rợn.
  - Bổ sung **Ánh trăng lạnh dịu nhẹ (Moonlight Directional Light)** xanh bạc cường độ 1.8 lux tạo bóng đổ mềm mại cho rừng thông.
  - **14 Cột đèn đường dạng Cần Vươn (Overhead Cantilever Downlights):**
    - Chao đèn vươn ngang ra giữa làn đường di chuyển.
    - Nguồn sáng **Spot Light rọi THẲNG ĐỨNG XUỐNG MẶT ĐƯỜNG (Downlight - Rotation = 90°, 0°, 0°)** với cường độ 6,500f, tầm xa 22m, góc 68° tạo nên các vệt sáng tròn vàng rực rỡ trực tiếp trên mặt đường nhựa ngay tâm làn xe chạy.
    - Cấu trúc phân cấp chuẩn vật lý (Scale 1:1) tránh biến dạng nón sáng Spot Light và định vị chao đèn chuẩn xác $1.7\text{m}$ vào làn xe.
    - **TUYỆT ĐỐI KHÔNG DÙNG ĐÈN SÁNG TỎA (Point Light):** Loại bỏ hoàn toàn nguồn sáng tỏa đa hướng để không làm sáng bệt không gian.
  - Đèn pha xe máy nâng cấp lên 4,500f (tầm quét 55m) và đèn pin người chơi 2,800f (tầm quét 30m).
  - **Bản Ghi Nhớ Ca Đêm (Boss Door Memo):** Gắn kiên cố trên tường bê tông bên trái cửa (`Storefront_Solid_Wall_Left` tại $X = -1.55\text{m}, Y = 1.65\text{m}, Z = -5.11\text{m}$), cố định bằng dải băng dính đen (`Memo_Tape_Strip`), triệt tiêu 100% lỗi bay lơ lửng khi cửa kính tự động mở.
- **Giao Diện Chữ To Gấp Đôi & Panel Đen Co Dãn (Doubled Accessible UI):**
  - Kích thước chữ tăng gấp đôi (22 - 28pt) với tính năng Text Auto-Sizing chống tràn chữ.
  - Toàn bộ Panel nền đen (Objective, Clock/Money, Prompt, Dialogue Subtitle, Summary, Boss Countdown, Boss Health Bar) mở rộng gấp đôi diện tích để chữ nằm trọn vẹn, dễ đọc ở mọi độ phân giải.

---

### 2.10. HỆ THỐNG MENU CÀI ĐẶT & TẠM DỪNG GAME (PAUSE & SETTINGS MENU)
- **Kích hoạt:** Nhấn phím `[ESC]`, `[P]` hoặc click vào nút bánh răng `[⚙]` ở góc trên bên phải HUD.
- **Đóng băng Game:** `Time.timeScale = 0`, mở khóa chuột tự do thao tác.
- **Chức năng chính:**
  1. **Chuyển đổi Ngôn ngữ Tức thời:** Nút `[🇻🇳 Tiếng Việt]` và `[🇬🇧 English]` chuyển đổi tức thì toàn bộ ngôn ngữ hiển thị trong game và lưu vào `PlayerPrefs`.
  2. **Thanh Trượt Âm Lượng SFX / Ambience (0 - 100%):** Điều chỉnh tiếng mưa gió, sấm sét, tiếng xe máy, súng đạn, chà sàn, chuông cửa.
  3. **Thanh Trượt Âm Lượng Music / BGM (0 - 100%):** Điều chỉnh nhạc nền rùng rợn và nhạc đấu Boss.
  4. **Nút Thao Tác:** Tiếp tục chơi [ESC], Chơi lại ca trực, Thoát game.

---

### 2.11. MÁY QUÉT MÃ VẠCH CẦM TAY & QUY TRÌNH THANH TOÁN QUẦY (HANDHELD BARCODE SCANNER)
- **Quy trình Khách hàng Chân thực (Realistic Customer Sequence):**
  - Khách sinh ra ngoài vỉa hè ($Z = -7.5\text{m}$) với **hai bàn tay hoàn toàn trống rỗng** (không cầm sẵn hàng trước khi vào tiệm).
  - Bước qua cửa kính tự động (chuông leng keng), đi đến các kệ hàng siêu thị lựa chọn món đồ, dừng lại 2.0s phát âm thanh nhặt đồ.
  - Sau đó mới tiến lại quầy thu ngân ($X = -1.45\text{m}, Z = -3.2\text{m}$), đặt hàng hóa và thẻ căn cước (ID) lên mặt bàn thanh toán.
  - **Khóa vật lý hàng hóa (Kinematic Items):** Hàng hóa của khách nằm cố định trên mặt bàn, người chơi **không thể nhặt lên tay hoặc ném lung tung**, tránh làm lộn xộn quầy và hỏng luồng thanh toán.
- **Hệ thống Máy Quét Cầm tay (Handheld Scanner):**
  - Đặt tại giá đỡ dock trên mặt quầy thu ngân.
  - Người chơi nhấn `[E]` tại giá đỡ để trang bị máy quét vào kho đồ (hiển thị mô hình FPS Scanner trên tay).
  - Khi cầm máy quét, tia laser đỏ định hướng phóng ra từ đầu máy quét.
  - Hướng tâm laser vào món hàng trên bàn trong phạm vi 3.5m: món hàng phát sáng viền vàng (Highlight).
  - Nhấn Chuột Trái hoặc `[E]`: Máy quét phát tiếng kêu `*TÍT!*` đanh thép, màn hình máy tính tiền cộng dồn giá trị món hàng và đổi trạng thái món hàng sang đã quét.
  - Quét xong, người chơi có thể nhấn `[E]` vào giá đỡ dock để cất máy quét trở lại mặt quầy.

---

### 2.12. TIẾN TRÌNH 7 ĐÊM & BƯỚC NGOẶT NGÀY THỨ 8: HỐ SỤT KHỔNG LỒ (THE SINKHOLE EVENT)
- **Tiến trình Leo thang qua 7 Đêm (7-Day Dread Progression):**
  - **Đêm 1 - 2 (Xây dựng Bầu không khí & Làm quen):** Khách mua 2 - 3 món đồ, kẻ giả mạo có dấu hiệu dị thường cơ bản (không bóng đổ). Cúp điện ngắn, quái vật lẻ tẻ. Người chơi tích lũy tiền lương trả dần nợ nhà $800.
  - **Đêm 3 - 4 (Phá vỡ vùng an toàn):** Khách mua 4 - 5 món đồ, dị thường xuất hiện trên căn cước và năm sinh. Cúp điện đột ngột ngay lúc đang thanh toán. Quái vật bắt đầu cào cửa kính.
  - **Đêm 5 - 6 (Dấu hiệu Bất thường Dưới Lòng Đất):** Mặt sàn gạch men giữa tiệm xuất hiện các vết nứt mạng nhện rỉ dịch đen phát quang. Thỉnh thoảng mặt đất rung chuyển chấn động kèm tiếng rên rỉ trầm đục từ sâu trong lòng đất. Mở khóa khẩu **Shotgun 12-Gauge cưa nòng** trong tủ đồ quản lý.
  - **Đêm 7 (Hỗn loạn Cực độ):** Quái vật tấn công dồn dập, còi báo động rú vang. Người chơi sống sót qua đêm thứ 7, lái xe về phòng trọ ngủ.
- **Sáng Ngày Thứ 8 – Sự Xuất Hiện của Hố Sụt Khổng Lồ (The Day 8 Sinkhole):**
  - Người chơi thức dậy lúc 11:30 PM, lái xe máy đến quán như thường lệ.
  - Biển hiệu neon tắt ngấm, tiệm chìm trong tĩnh lặng ghê người.
  - Khi bước vào tiệm: **Toàn bộ khu vực kệ hàng trung tâm đã sập toác thành một HỐ SỤT KHỔNG LỒ** (đường kính 10 mét), sâu hun hút xuống lòng đất, sương mù đỏ sẫm bốc lên ma mị.
  - Sợi dây thừng bảo hộ của Quản lý treo lơ lửng bên mép hố cùng mảnh giấy note dính máu:
    > *"Nó đã thức giấc rồi... Bản sao trên mặt đất chỉ là rễ cây, cội nguồn của chúng ở dưới đáy vực. Nếu không diệt nó đêm nay, thành phố sẽ không còn ngày mai."*
- **Ngã Rẽ Quyết Định (The Ultimate Choice):**
  - **Nhánh A (Nhảy xuống hố):** Người chơi bấm `[E]` trượt theo dây thừng xuống đáy vực ➔ Kích hoạt màn quyết đấu với **Con Trùm Cội Nguồn (The True Final Boss)**.
  - **Nhánh B (Quay đầu bỏ chạy):** Người chơi quay lưng ra cửa, nổ máy xe chạy về phòng trọ trốn ➔ Kích hoạt **Bad Ending (Ngày Tận Thế)**.

---

### 2.13. THIẾT KẾ ĐẤU TRƯỜNG & CHIẾN ĐẤU TRÙM CỘI NGUỒN (THE TRUE ABYSSAL BOSS FIGHT)
- **Đấu Trường Hang Đá Ngầm Rộng Lớn (The Underground Cavern Arena):**
  - Kích thước hang ngầm rộng $30\text{m} \times 30\text{m}$, trần vòm cao 12m (giải quyết triệt để vấn đề chật chội và kẹt góc kệ hàng).
  - Tàn tích mỏ đá cổ kết hợp các mảnh vỡ của quầy kệ siêu thị rơi rụng tạo thành các góc nấp chắn (Cover System).
- **Thực Thể Trùm: "Chúa Tể Dị Thể Cội Nguồn" (The Doppelgänger Mother):**
  - Khối sinh vật khổng lồ lai giữa rễ cây hắc ám, xúc tu bọc giáp gai và hàng trăm khuôn mặt người biến dạng.
  - Toàn thân bọc lớp giáp sừng cứng chắc: **Bắn vào thân giáp chỉ gây 5-10% sát thương chip damage**. Người chơi bắt buộc phải né tránh và bắn trúng **ĐIỂM YẾU (Weakpoints)**.
- **3 Giai Đoạn Chiến Đấu & Bộ Chiêu Thức (Boss Moveset):**
  - **Giai đoạn 1: Đập Sàn AOE & Bắn Điểm Yếu Parry (100% - 70% Máu):**
    - *Đòn đập sàn chấn động (Ground Slam AOE):* Boss giơ cao xúc tu, mặt đất dưới chân người chơi rung chuyển và phát sáng đỏ trong 1.2s. Người chơi phải Sprint hoặc nhảy sang hai bên né vùng nguy hiểm.
    - *Cơ chế Parry ngắt chiêu (Weakpoint Golden Window):* Sau cú đập sàn, lớp giáp ngực Boss mở toang để lộ **Lõi Tim Đỏ Rực** phát sáng nhấp nháy trong 1.5s để nạp năng lượng. Người chơi kê tâm bắn trúng Lõi Tim:
      - Phát ra âm thanh **"KENGGG!"** cực đanh + hiệu ứng khựng hình (Hit-Stop 0.1s).
      - Boss bị **Ngắt chiêu, rống lên đau đớn và ngã khuỵu (Hard Stagger) trong 3.5 giây**, lớp giáp lưng bung ra để người chơi bồi đạn x3 sát thương!
  - **Giai đoạn 2: Mưa Gai Bullet-Hell & Phân Thân Hỗn Loạn (70% - 30% Máu):**
    - *Mưa Gai Hắc Ám:* Boss bắn ra hàng chục gai nhọn bay uốn lượn hình cầu. Người chơi phải vừa di chuyển strafe vừa luồn lách qua khe đạn.
    - *Triệu hồi quái phân thân:* Triệu hồi 2-3 bản sao không bóng đổ lao tới cắn trộm.
    - *Tương tác đạn dược nhanh:* Các hộp tiếp tế rơi từ trên tiệm xuống, người chơi chỉ cần chạy lướt qua nhấn nhanh phím **[E]** (1 chạm) để nhận đạn, không cần cắm mặt vào máy tính. Có thể bắn nổ bình gas mini rải rác để thiêu rụi bầy quái nhỏ.
  - **Giai đoạn 3: Cuồng Nộ & Con Mắt Thứ Ba (Dưới 30% Máu):**
    - *Tia Hủy Diệt (Abyssal Beam):* Toàn bộ đấu trường tối đen, Boss mở **Con Mắt Thứ Ba Khổng Lồ** trên trán tụ cầu năng lượng đen chuẩn bị quét sạch đấu trường.
    - *Thời khắc sinh tử (2 giây):* Người chơi có đúng 2 giây để bắn chính xác viên đạn Shotgun/Pistol vào con mắt đang phát sáng chói lọi. Bắn trúng: Con mắt nổ tung, Boss gục ngã hoàn toàn!

---

### 2.14. CHI TIẾT 2 KẾT CỤC ĐẠI CUỘC (THE 2 MULTIPLE ENDINGS)
- **1. BAD ENDING: "KẺ HÈN NHÁT & NGÀY TẬN THẾ" (THE RECKONING)**
  - *Điều kiện:* Người chơi chọn Nhánh B (không nhảy xuống hố, lái xe về phòng trọ trốn).
  - *Diễn biến:* Người chơi về phòng trọ, khóa chặt cửa, ôm bọc tiền lương $800 mừng rỡ vì giữ được mạng.
  - *Cao trào:* 06:00 AM không có bình minh, bầu trời nhuộm đỏ màu máu. Còi báo động khẩn cấp quốc gia rú vang. Màn sương đen nuốt trọn thành phố, hàng ngàn sinh vật không bóng đổ tràn ngập đường phố xé toạc mọi chốt chặn.
  - *Cảnh kết:* Tiếng *RẦM* kinh hoàng, cánh cửa phòng trọ người chơi bị xúc tu gai khổng lồ xé nát. Màn hình phụt đen.
  - *Thông điệp:* *"Bạn giữ được mạng sống của đêm nay, nhưng đã đánh mất ngày mai của toàn thể nhân loại."*

- **2. TRUE ENDING: "ÁNH BÌNH MINH CỨU RỖI" (THE SUNLIT DELIVERANCE)**
  - *Điều kiện:* Người chơi chọn Nhánh A (nhảy xuống hố và bắn hạ thành công Chúa Tể Dị Thể Cội Nguồn).
  - *Diễn biến:* Boss nổ tung tan biến thành tro tàn. Đáy hang ngầm bắt đầu sụp lở dữ dội. Người chơi nhanh chóng leo theo dây thừng ngược lên sàn siêu thị. Ngay khi bước lên mặt đất, hố sụt đổ sập và phong ấn vĩnh viễn vực sâu.
  - *Cao trào:* Đồng hồ điểm đúng 06:00 AM sáng. Cửa tiệm tự động trượt mở. Lần đầu tiên sau 7 đêm dài, **toàn bộ sương mù đen trên đèo tan biến hoàn toàn**. Những tia nắng vàng ban mai rực rỡ chiếu rọi qua tán rừng thông xanh ngắt.
  - *Cảnh kết:* Người chơi bước ra hiên quán hít thở bầu không khí trong lành, cầm bọc tiền sạch sẽ, nổ máy chiếc xe máy cũ thong dong chạy về thành phố chan hòa ánh nắng.
  - *Thông điệp:* *"Món nợ đã trả hết, ác mộng đã vĩnh viễn khép lại. Bạn chính là người hùng thầm lặng của ca đêm."*
