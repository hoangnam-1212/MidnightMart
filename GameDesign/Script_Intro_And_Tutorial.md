# 🎬 KỊCH BẢN CHI TIẾT: TOÀN BỘ CỐT TRUYỆN & HƯỚNG DẪN CHƠI (GAME SCRIPT & TUTORIAL)
## DỰ ÁN: MIDNIGHT MART - CA TRỰC ĐÊM KINH HOÀNG (HORROR SHOPKEEPING + TACTICAL FPS)

---

## 📖 BỐI CẢNH TỔNG THỂ & TIỀN ĐỀ CÂU CHUYỆN

Thị trấn Blackwood nằm lọt thỏm giữa thung lũng rừng thông bạt ngàn quanh năm sương mù bao phủ. Trong vòng hai tuần qua, cư dân thị trấn liên tiếp truyền tai nhau về những vụ mất tích kỳ quái: những người đi rừng không bao giờ trở về, gia súc bị xé toạc, và gần đây nhất là tin đồn về **"những kẻ đội lốt người"** bước đi trong đêm mà không có bóng đổ dưới ánh đèn.

Bạn vào vai một thanh niên nghèo sống tại căn phòng trọ tồi tàn ven rừng. Tiền thuê nhà đã nợ $800 đến hạn chót phải trả trong đêm nay. Trong lúc tuyệt vọng nhất, chiếc điện thoại nhận được tin nhắn tuyển dụng bí ẩn từ Quản lý của **Midnight Mart** — siêu thị tiện lợi 24/7 duy nhất nằm đơn độc giữa quốc lộ xuyên rừng Blackwood:
> *"Cần tuyển gấp nhân viên trực ca đêm từ 00:00 đến 06:00 sáng. Lương nóng $50/giờ tiền mặt trả ngay tại quầy cuối ca. Nhân viên ca trước tên David đã mất tích bí ẩn đêm qua. Đừng hỏi nhiều, có mặt trước 00:00."*

Để có tiền cứu vãn cuộc sống, bạn không còn lựa chọn nào khác ngoài việc nổ máy chiếc xe máy cũ lao vào màn đêm mịt mù của khu rừng tử thần...

---

## MÀN 1: MỞ ĐẦU – "CÔNG VIỆC LƯƠNG CAO ĐỘT XUẤT"

### [CẢNH 01: PHÒNG TRỌ BẾ TẮC (APARTMENT PROLOGUE - TỌA ĐỘ Z = -294M)]
- **Bối cảnh:** Căn phòng trọ nhỏ hẹp 5m x 4m kín khít 100% không một kẽ hở nhìn ra ngoài lúc 23:30 đêm. Ánh đèn tuýp vàng ấm soi rọi chiếc giường ngủ đơn sơ, chiếc bàn làm việc cũ kỹ và cánh cửa gỗ khép chặt có nẹp khung kim loại bảo vệ.
- **Tương tác cốt truyện:**
  1. Người chơi thức dậy trong phòng trọ, tiến lại bàn làm việc.
  2. Trên bàn có **Tờ giấy đòi nợ tiền nhà màu đỏ chót**:
     > `GIẤY BÁO NỢ TIỀN NHÀ: $800.00 - Hạn chót đêm nay! Nếu không nộp đủ tiền mặt trước sáng mai, toàn bộ đồ đạc sẽ bị tống ra ngoài đường!`
  3. Chiếc điện thoại cục gạch rung lên bần bật: Tin nhắn tuyển dụng nóng từ Quản lý Midnight Mart.
  4. Người chơi nhặt chùm chìa khóa xe máy trên bàn (`[E] Nhặt chìa khóa xe máy`):
     > *"50 đô một giờ cho một ca bán hàng đêm... Quá vô lý cho một công việc tiện lợi thông thường, nhưng với món nợ $800 này, mình không có quyền kén chọn."*
  5. Tiến lại cửa phòng, nhấn `[E]` mở cửa phòng ra ngoài. Cánh cửa gỗ mở xoay 75° hé lộ khoảng hiên nhà và bầu không khí đêm tối om mịt mùng.
  6. Bên ngoài hiên nhà (`Z = -288.5m`), chiếc xe máy cũ đang dựng sẵn ngoài sân, đèn đuôi xe phản chiếu ánh sáng mờ ảo.
  7. Người chơi bước ra sân, tiến lại gần xe máy và nhấn `[E]` để lên xe nổ máy xuất phát.

---

### [CẢNH 02: CHUYẾN XE MÁY ĐÊM MƯA – CUNG ĐƯỜNG RỪNG CONG 270M TỐI OM BÍ ẨN]
- **Bầu không khí & Ánh sáng (Pitch-Black Darkness):**
  - Môi trường ngoài trời tối đen tuyệt đối (Ambient Light = 0, toàn bộ ánh sáng mặt trời/Directional Light bị ngắt hoàn toàn).
  - Ánh sáng duy nhất bên ngoài đến từ **luồng đèn pha xe máy (Spotlight 280f, góc 40°, tầm xa 24m)** quét qua lớp sương mù đêm dày đặc (`ExponentialSquared Fog, mật độ 0.040`) và những chao đèn đường hiu hắt ven rừng.
- **Cơ chế trải nghiệm (Liminal Transition Gameplay - S-Curve Highway 270m, Z = -280m đến Z = -10m):**
  - Người chơi ngồi trên xe máy, góc nhìn thứ nhất (FPS) hướng ra tay lái kim loại, đồng hồ tốc độ phát sáng xanh mờ và gương chiếu hậu phản chiếu bóng tối rợn người sau lưng.
  - Giữ phím `[W]` để tăng tốc vít ga chạy xe.
  - Con đường nhựa quốc lộ 2 làn dài gấp đôi (270m), uốn lượn hình chữ S đôi (`double S-curve`) xuyên qua khu rừng rậm Blackwood với hơn 100 cây thông u ám. Xe máy tự động ôm cua mượt mà, người chơi dùng `[A]`/`[D]` để lách xe sang hai bên tim đường.
- **5 Cột mốc & Điểm kinh dị dọa người chơi dọc cung đường 270m:**
  - **Z = -255m (Mốc 1 - Ranh giới rừng cấm):** Biển gỗ lớn ven đường mang dòng chữ sơn trắng: *"KHU VỰC RỪNG NGUY HIỂM - CẤM ĐI ĐÊM MỘT MÌNH"*, trên mặt gỗ có nhiều vết cào máu sâu hoắm và vệt sơn đỏ chéo rợn người.
  - **Z = -240m:** Cột đèn đường số 1 chao đảo, bóng đèn sợi đốt chớp giật tạch tạch soi rọi những bóng cây khô cằn cỗi vươn cành như những bàn tay quỷ.
  - **Z = -210m (Mốc 2 - Biển cảnh báo quái vật):** Biển báo tam giác màu vàng phản quang nghiêng ngả ven lề: *"CẨN THẬN: THÚ DỮ & QUÁI VẬT BĂNG ĐƯỜNG"*.
  - **Z = -195m:** Cột đèn đường số 2 ánh sáng vàng úa le lói giữa màn sương đen.
  - **Z = -165m (Mốc 3 - Bù nhìn ma quái Totem Effigy):** Lấp ló trong lùm cây thông bên trái là một hình nộm gỗ/rơm ma quái (Totem) đứng sừng sững, hai hốc mắt phát ra **ánh sáng đỏ rực le lói** trừng trừng nhìn theo xe người chơi khi lướt qua.
  - **Z = -150m:** Cột đèn đường số 3 gãy nghiêng, dây điện lòng thòng tóe tia lửa nhỏ.
  - **Z = -125m (Mốc 4 - Khúc cua tử thần & Xe cảnh sát gặp nạn):** Tại khúc cua gắt nhất của cung đường, rào chắn hộ lan thép bị tông bẹp dúm. Dưới mương bùn xuất hiện **chiếc xe tuần tra cảnh sát lật nghiêng bỏ hoang**, đèn ưu tiên màu cam trên nóc xe vẫn chớp nháy cô độc trong bóng tối, cửa xe bung mở với vệt máu kéo dài vào rừng.
  - **Z = -105m:** Cột đèn đường số 4 nhấp nháy chập chờn.
  - **Z = -80m (Mốc 5 - Biển chào đẫm máu):** Tấm biển kim loại gỉ sét: *"Chào mừng đến Hạt Blackwood - Dân số: 4,120"*, nhưng con số này đã bị gạch xóa vẽ đè bằng máu thành: *4,119... rồi 4,118...*
  - **Z = -65m & Z = -22m:** Hai cột đèn đường cuối cùng soi rõ lối vào bãi đỗ xe siêu thị.
  - **Z = -20m đến -10m:** Từ xa trong làn sương đêm, ánh đèn LED rực rỡ màu đỏ - cyan của siêu thị hiện lên giải thoát người chơi khỏi nỗi kinh hoàng: **"MIDNIGHT MART - OPEN 24/7"**.

---

### [CẢNH 03: TỚI SIÊU THỊ & BẢN GHI NHỚ TRÊN CỬA KÍNH]
- **Đỗ xe máy:** Xe tới gần siêu thị (`Z ~ -8.5m`), xe tự động giảm tốc tấp vào vỉa hè (`Vector3(-2.2f, 0.1f, -7.5f)`), người chơi xuống xe, chiếc xe máy hiện ra đỗ gọn gàng cạnh lề đường.
- **Cửa kính tự động bị KHÓA chặt:** Người chơi bước lên bậc vỉa hè tiến lại cửa kính siêu thị. Cửa tự động cảm ứng không mở ra vì đang bị khóa an toàn từ bên ngoài.
- **Tờ Giấy Ghi Nhớ Ca Đêm của Quản Lý (Boss Shift Memo):**
  - Dán nổi bật trên khung kính cửa trước là một tờ giấy note màu vàng neon (`Boss_Door_Memo_Note`).
  - **UI Nhiệm vụ:** `[MỤC TIÊU: Đọc Bản Ghi Nhớ Ca Đêm của Quản Lý dán trên cửa kính]`
  - Người chơi tiến lại gần nhấn `[E]` để bóc đọc nội dung:
    > **QUY TẮC CA ĐÊM (QUẢN LÝ MIDNIGHT MART):**  
    > 1. *Cậu trực thay David. Đừng có tò mò lục lọi lung tung.*  
    > 2. *Trước 01:00 AM: Nhặt 3 món đồ rơi lên kệ và lấy Cây Lau Nhà ở góc phải tiệm chà sạch 3 vết bẩn.*  
    > 3. *Khách không có bóng đổ: Quay ra Bàn Sau Quầy dùng Máy Tính tra cứu CSDL công dân.*  
    > 4. *Quái vật sẽ tấn công lúc cúp điện 03:30 AM! Súng đạn giấu trong ngăn kéo bí mật dưới quầy.*
- **Mặt tiền Cửa tiệm:** Hai bên cửa là tường bê tông đặc kiên cố, chỉ có cánh cửa ra vào chính giữa là kính cường lực trượt tự động (tạo cảm giác cô lập, rùng rợn, không thể nhìn thấu ra bên ngoài).
- **Mở khóa cửa tiệm:** Ngay khi đọc xong, khóa chốt điện tử kêu tách một tiếng giòn giã. Hai cánh cửa kính tự động trượt cuốn sang 2 bên êm ái, chuông cửa vang lên: **LENG KENG!**
- Người chơi bước vào không gian sáng rực rỡ của 9 dàn đèn tuýp huỳnh quang siêu thị, chính thức bước vào chuẩn bị ca trực.

---

## MÀN 2: CHUẨN BỊ CA TRỰC & HƯỚNG DẪN NGHIỆP VỤ (PRE-SHIFT & TUTORIAL)

### [BƯỚC 1: XẾP ĐỒ RƠI LÊN KỆ & LAU VẾT BẨN BÍ ẨN]
- **Không gian Siêu thị Mới (22m x 20m = 440m²):**
  - Không gian rộng gấp đôi thoáng đãng với **6 dãy kệ hàng Gondola** phân bổ đều đặn khắp lòng tiệm (`X = -7.5m, -4.5m, -1.5m, +1.5m, +4.5m, +7.5m`).
  - Lối đi giữa các dãy kệ rộng rãi 2.1m, các tầng mâm đều có thanh gờ kim loại cao 6cm bảo vệ chống đổ.
- **UI Nhiệm vụ kép:**  
  `[MỤC TIÊU: Chuẩn bị ca trực - Xếp đồ rơi (0/3) & Lau vệt bẩn (0/3)]`
- **Thực hiện Nhiệm vụ 1 - Xếp 3 món đồ rơi:**
  - Nhặt **Gói bánh Snack Đỏ** dưới sàn xếp vào ô trống phát sáng trên Kệ 3 (`X = -1.5m`).
  - Nhặt **Chai nước ngọt Xanh** dưới sàn xếp vào ô trống phát sáng trên Kệ 4 (`X = 1.5m`).
  - Nhặt **Hộp ngũ cốc Vàng** dưới sàn xếp vào ô trống phát sáng trên Kệ 5 (`X = 4.5m`).
  - *Mỗi khi xếp đúng, món đồ đặt khít vào mâm kệ sau gờ chắn, hoàn toàn không đè lên hàng có sẵn, phát âm thanh pop nhẹ nhàng.*
- **Thực hiện Nhiệm vụ 2 - Lau 3 vệt bẩn sẫm màu dưới sàn:**
  - **YÊU CẦU BẮT BUỘC:** Người chơi **PHẢI** tiến lại xô nước góc trước bên phải tiệm (`X = 4.2m, Z = -3.8m`) nhấn `[E]` nhặt **Cây Lau Nhà** vào Ô 1 kho đồ trước khi có thể lau. Nếu dùng tay không tiến lại vết bẩn, hệ thống sẽ nhắc nhở: *"[!] Cần nhặt Cây Lau Nhà trong tiệm để lau sàn"* và từ chối hành động. Cây lau nhà sẽ luôn hiển thị ở ô kho đồ động và có mô hình FPS trên tay nhân vật.
  - Vết bẩn 1: Gần lối vào cửa chính (`Floor_Stain_1_Entrance`).
  - Vết bẩn 2: Lối đi giữa Kệ 2 và Kệ 3 (`Floor_Stain_2_AisleWalkway`).
  - Vết bẩn 3: Lối đi bên phải giữa Kệ 4 và Kệ 5 (`Floor_Stain_3_RightWalkway`).
  - *Khi cầm cây lau nhà nhấn [E]: Tiếng chà sàn sột soạt, vệt bẩn co nhỏ dần rồi biến mất hoàn toàn.*
- Khi cả hai đạt `3/3`: Chuông đồng hồ điểm **01:00 AM**, đèn tiệm chớp giật báo hiệu vị khách đầu tiên bước vào.

---

### [BƯỚC 2: PHỤC VỤ VỊ KHÁCH ĐẦU TIÊN & QUY TẮC KHÁCH ĐỢI TẠI QUẦY]
- **Chuông cửa reo:** `LENG KENG!` Cửa tự động mở ra 2 bên.
- **Bố trí Quầy Thu ngân Góc Cửa Dọc:** Quầy thu ngân đặt tại góc dọc sát cửa (`X = -2.4m, Z = -3.2m`), mặt quầy chỉ để máy tính tiền, máy quét mã vạch và khay thanh toán. Chiếc máy tính cồng kềnh trước đây đã được chuyển sang **Bàn nhân viên phía sau (`Back_Staff_Desk` tại X = -4.3m, Z = -3.2m)** giúp quầy thanh toán cực kỳ gọn gàng.
- **Vị khách:** Ông lão khách quen **Arthur Morgan** bước vào tiệm, đi lại các dãy kệ lựa đồ rồi tiến ra quầy thu ngân.
- **Tư thế của Khách:** Đứng tại vị trí chờ (`X = -1.45m, Y = 0.0m, Z = -3.2m`), hai bàn chân chạm sàn vững vàng, xoay người nhìn trực diện vào thu ngân qua mặt bàn (khắc phục hoàn toàn lỗi quay lưng vào quầy).
- Ông lão đặt lên mặt bàn **3 món hàng** cùng **Thẻ Căn cước công dân (ID Card)**:
  1. *Bao thuốc lá Red ($8.50)*
  2. *Gói Snack Khoai Tây ($3.50)*
  3. *Lon Soda Chanh ($2.50)*

#### ⚠️ QUY TẮC KHÁCH KIÊN NHẪN ĐỢI & CHỐNG BIẾN MẤT (ANTI-DESPAWN):
- Khách hàng **tuyệt đối không bao giờ tự ý biến mất**. Dù người chơi bỏ quầy đi nghịch đồ, khám phá hay chạy khắp siêu thị thì khách vẫn kiên nhẫn đứng đợi tại quầy.
- Khi người chơi đi nghịch lâu (> 18 giây):
  - *Người thật:* Sốt ruột ho nhẹ (*khụ khụ*), giục thanh toán nhẹ nhàng:
    - *"Này cậu thu ngân ơi? Tôi còn vội về, tính giùm tôi với..."*
  - *Quái vật Doppelgänger:* Đầu và ánh mắt tự động xoay dõi theo bước chân người chơi khắp tiệm (Head Tracking), đầu giật nhẹ và thì thầm rùng rợn:
    - *"...Sao lâu thế...? Ngươi đang tìm cái gì à...?"*
- **Quy trình Thanh toán:**
  1. Soi Thẻ ID: Nhấn `[E]` vào thẻ -> Tên Arthur Morgan, sinh năm 1952 (74 tuổi - hợp lệ), có bóng đổ bình thường dưới sàn.
  2. Quét 3 món hàng: Nhấn `[E]` hoặc dùng Barcode Scanner -> Máy kêu `TÍT!`, máy tính tiền cộng dồn tổng đơn: **`$14.50`**.
  3. Khách đưa tờ tiền mặt: **`$20.00`**.
  4. Mở máy tính tiền, thối lại: **`$5.50`**.
- **Cảnh báo ghê rợn & Lối ra tức thì (~1.9m):**
  > *"Cậu thanh niên... tối qua tôi thấy thằng David bị kéo vào rừng... cẩn thận đấy."*
  - Nói xong sau 0.6s, khách quay người bước thẳng ra ngoài cửa trượt tự động (khoảng cách cực gần chỉ ~1.9m), cửa mở và khách hòa vào màn đêm, không còn nấn ná ở quầy.

---

### [BƯỚC 3: MỞ NGĂN KÉO BÍ MẬT & TRANG BỊ VŨ KHÍ]
- Sau khi khách rời đi, ngăn kéo tiền phát ra tiếng kêu kẹt cứng.
- **UI Nhiệm vụ:** `[MỤC TIÊU: Kéo mạnh ngăn kéo bí mật dưới quầy]`
- Người chơi nhấn `[E]` kéo bung ngăn kéo bí mật:
  1. **Điện thoại vỡ nát của David:** Màn hình nhấp nháy dòng tin nhắn dang dở:
     > *"CHÚNG ĐANG BẮT CHƯỚC HÌNH DẠNG CON NGƯỜI... CHÚNG KHÔNG CÓ BÓNG ĐỔ DƯỚI ĐÈN... TRA CỨU MÁY TÍNH BÀN SAU... ĐỪNG MỞ CỬA..."*
  2. **Khẩu Súng Lục 9mm (Pistol):** Nạp sẵn 5 viên trong băng nòng và 15 viên dự trữ (`5 / 15`).
- **Thao tác chiến đấu:**
  - Phím `[2]`: Rút / Cất súng lục.
  - Phím `[R]`: Nạp đạn (1.2s).
  - Chuột trái: Bắn đạn chính xác.
  - Sau khi nhặt súng, ngăn kéo tự động đóng lại giải phóng không gian quầy.

---

## MÀN 3: BÀN LÀM VIỆC SAU QUẦY & MÁY TÍNH CỬA HÀNG (BACK STAFF TERMINAL)

- **Vị trí Trạm Máy tính:** Đặt trang trọng trên chiếc bàn gỗ riêng biệt sau lưng quầy thu ngân (`Back_Staff_Desk` tại `X = -4.35m, Z = -3.2m`) với layout công thái học: màn hình quay về phía thu ngân (+X), bàn phím và chuột quang đặt ngay ngắn, đèn bàn góc hắt sáng vàng ấm dịu (không che khuất màn hình hay đè lên máy tính), ghế xoay văn phòng và case CPU dưới gầm bàn.
- **Tương tác:** Người chơi chỉ cần quay lưng lại nhấn `[E]` vào máy tính để bật giao diện Midnight Mart OS v2.4 (con trỏ chuột được tự do di chuyển, camera FPS tạm dừng xoay để người chơi thoải mái click chuột mua đồ hoặc dùng phím tắt [1]/[2], dùng [ESC] hoặc [E] để thoát):
  1. **Tab 1: Tra cứu CSDL Công dân Blackwood (`[F1]` / `[1]`):**
     - Tự động quét khách hàng đang đứng trước quầy.
     - *Dân thường:* Báo xanh `✓ HỒ SƠ CÔNG DÂN HỢP LỆ`.
     - *Kẻ giả mạo Doppelgänger (Thomas Vance):* Màn hình nhấp nháy đỏ rực:  
       `⚠️ BÁO ĐỘNG ĐỎ: ĐỐI TƯỢNG DỊ BIẾN #ANOMALY-1845-DEAD`  
       Cảnh báo dị thường: Không có bóng đổ, năm sinh 1820 (chết cách đây 104 năm trong đầm lầy), ban bố lệnh tiêu diệt tức khắc!
  2. **Tab 2: Cửa hàng Tiếp tế Đêm khẩn cấp (`[F2]` / `[2]`):**
     - Kết nối trực tiếp vào quỹ tiền mặt của cửa hàng, tự động trừ tiền khi mua:
       - **Gói Đạn 9mm ($15):** +10 viên đạn súng lục.
       - **Hộp Đạn Shotgun ($30):** +4 viên đạn ria 12-Gauge.
       - **Nước Tăng Lực ($12):** Hồi phục 100% thể lực chạy nước rút (Sprint) để thả diều Boss.
       - **Thanh Gỗ Chặn Cửa ($25):** Gia cố các góc che chắn phòng thủ.

---

## MÀN 4: ĐÊM DỊ BIẾN & SỰ CỐ CÚP ĐIỆN (03:30 AM)

- **Diễn biến:**
  - Nhiều đợt khách hàng tiếp tục tới tiệm: Người phụ nữ đi làm về muộn (Sarah Jenkins), kẻ lang thang dị biến (Thomas Vance).
  - Người chơi phải liên tục đối chiếu: Soi bóng đổ dưới đèn, kiểm tra thẻ ID, và chạy lại máy tính bàn sau để tra cứu CSDL trước khi quyết định bán hàng hay nổ súng.
  - **Hậu quả nếu để kẻ giả mạo thoát ra:** Mỗi con quái thoát ra ngoài cửa sẽ báo tin cho đồng bọn, khiến bầy quái kéo đến tiệm lúc cúp điện đông hơn (+2 con/mỗi con thoát) và kích hoạt Boss xuất hiện sớm hơn!
- **03:30 AM - Cúp điện toàn phần:**
  - Toàn bộ 9 dàn đèn huỳnh quang nổ lách tách rồi vụt tắt ngúm!
  - Đèn báo động đỏ khẩn cấp trên trần kích hoạt, quay tròn tỏa ánh sáng đỏ rực rợn người.
  - 6 tủ lạnh nước ngọt tường phải hắt ra ánh neon xanh lạnh lẽo.
  - Tiếng đập cửa kính rầm rầm từ bên ngoài màn đêm: Bầy quái vật vây quanh tiệm! Người chơi bật đèn pin cá nhân `[F]`, rút súng thủ quầy và bắn hạ từng con quái tràn vào.

---

## MÀN 5: ĐỈNH ĐIỂM 04:30 AM – CUỘC TẤN CÔNG CỦA QUÁI TRÙM (THE FLESH STALKER)

### [CẢNH 01: BÁO ĐỘNG ĐỎ & 30 GIÂY ĐẾM NGƯỢC CHUẨN BỊ]
- **Thời điểm:** 04:30 AM (hoặc sớm hơn nếu để nhiều kẻ giả dạng thoát ra ngoài cửa).
- **Diễn biến:**
  - Còi báo động khẩn cấp rú liên hồi, toàn bộ đèn chớp giật dữ dội.
  - **Thanh Thời gian Chuẩn bị (Countdown Bar)** màu vàng cam xuất hiện ở đỉnh màn hình:  
    `⚠️ CẢNH BÁO: CON TRÙM ĐANG ĐẾN! CHUẨN BỊ VŨ KHÍ (30s)`
  - **UI Nhiệm vụ khẩn cấp:**  
    `[MỤC TIÊU: DÙNG MÁY TÍNH MUA THÊM ĐẠN, NẠP SÚNG [R] VÀ NÉP VÀO GÓC!]`
  - Người chơi có đúng 30 giây quý giá để chạy lại máy tính bàn sau mua đạn súng lục/shotgun, nạp đầy băng đạn và chọn vị trí che chắn chiến thuật giữa 6 dãy kệ hàng.

---

### [CẢNH 02: TRÙM XUẤT HIỆN & CUỘC CHIẾN KITING TRONG SIÊU THỊ 440M²]
- Hết 30 giây, cánh cửa kho phía sau tiệm rầm rầm rung chuyển rồi vỡ toang!
- **Quái trùm THE FLESH STALKER** khổng lồ cao 2.8m với cơ bắp biến dị, gai nhọn tua tủa trên lưng và đôi móng vuốt cào xé dài ngoằng xông vào tiệm, gầm lên một tiếng kinh thiên động địa!
- **Thanh Máu Boss (Boss Health Bar)** đồ sộ hiện ra ở đỉnh màn hình:  
  `THE FLESH STALKER - TRÙM DỊ BIẾN ĐÊM 1 [ 1000 / 1000 HP ]`
- **Lối chơi Kiting FPS đỉnh cao trong Không gian 440m²:**
  - 6 dãy kệ hàng Gondola tạo thành hệ thống vòng lặp di chuyển (Looping Paths) hoàn hảo:
    - Kệ 1 đến Kệ 6 song song nhau với lối đi rộng 2.1m.
    - Hai đầu kệ có khoảng trống rộng hơn 5m cho phép người chơi vòng qua vòng lại để lừa hướng di chuyển của Boss.
  - Người chơi sử dụng tốc độ chạy nước rút (Shift), lùi bắn (Backpedal), nấp sau các góc kệ để tránh đòn cào của trùm (25 DMG/đòn).
  - Sử dụng Súng Lục 9mm bắn tỉa từ xa và Shotgun 12-Gauge bắn cực mạnh ở cự ly gần khi trùm rẽ góc.
  - Khi trùm bị tiêu diệt: Quái vật gầm rú đổ gục xuống sàn, bốc khói đen tan biến.
  - Màn hình hiện thông báo chiến thắng vang dội: Thưởng nóng **+$200** tiền bảo vệ cửa hàng vào quỹ tiệm!

---

## MÀN 6: KẾT THÚC CA TRỰC ĐÊM 1 – BẢNG LƯƠNG & CHUYẾN XE MÁY TRỞ VỀ NHÀ (06:00 AM)

- Đồng hồ chuyển sang **06:00 AM**.
- Ánh bình minh le lói đầu ngày bắt đầu xua tan màn sương mù đen đặc ngoài khung cửa kính.
- Chiếc chuông cửa reo lên: `LENG KENG!` Hệ thống phát thanh vang lên thông báo ca trực an toàn.
- **Bảng Tổng Kết Ca Trực Hiện Ra (Gấp đôi kích thước, song ngữ VI/EN):**
  - Số khách hàng đã phục vụ: `<served>` (+$15 tiền tip/khách)
  - Quái vật giả dạng đã tiêu diệt: `<killed>` (+$50/con)
  - Bắn nhầm khách hàng thật: `<penalties>` (-10% phạt)
  - **TỔNG LƯƠNG & TIỀN HIỆN CÓ: `<money>`**
  - Dòng hướng dẫn: `[Space] Nhận Tiền Lương & Lái Xe Về Nhà >>`
- **Bước ra xe máy về nhà:**
  - Nhấn `[Space]` hoặc click chuột: Bảng tổng kết đóng lại.
  - **UI Nhiệm vụ mới:**  
    `[MỤC TIÊU: Bước ra trước cửa tiệm và [E] Lên xe máy lái về nhà nghỉ ngơi]`
  - Người chơi bước qua cánh cửa tự động ra vỉa hè (`Z = -7.5m`). Chiếc xe máy cũ đã đỗ sẵn tại lề đường, có gắn tương tác `[E] Lên xe máy lái về nhà nghỉ ngơi`.
  - Nhấn `[E]` lên xe: Bộ điều khiển xe máy kích hoạt chế độ chạy ngược về nhà (`StartMotorbikeRideReturnHome`).
  - Người chơi giữ phím `[W]` vít ga chạy về hướng Nam qua cung đường cong 270m dưới ánh sáng của 14 cột đèn đường và sương sớm.
  - Khi xe về đến sân phòng trọ (`Z <= -276m`), xe tự động giảm tốc tấp vào hiên nhà (`Z = -282m`), tắt máy và người chơi xuống xe an toàn.

---

## MÀN 7: VỀ PHÒNG TRỌ, LÊN GIƯỜNG NGỦ & CHUYỂN TIẾP SANG NGÀY 2 (DAY 2 TRANSITION LOOP)

### [CẢNH 01: BƯỚC VÀO PHÒNG TRỌ & LÊN GIƯỜNG ĐI NGỦ]
- **Bối cảnh:** Người chơi đứng trước sân hiên phòng trọ quen thuộc.
- **UI Nhiệm vụ:**  
  `[MỤC TIÊU: Bước vào phòng trọ và [E] Lên giường đi ngủ để qua Ngày 2]`
- Người chơi bước qua cánh cửa phòng vào trong. Chiếc giường ngủ đơn sơ bên góc trái có gắn tương tác:
  `[E] Lên giường đi ngủ (Chuyển sang Ngày 2)`
- Người chơi tiến lại gần nhấn `[E]`:
  - Màn hình mờ dần chuyển sang màu đen đặc.
  - Âm thanh tiếng gió hú và nhịp tim đập chậm lại.
  - Dòng phụ đề tâm lý xuất hiện:
    > *"Bạn chìm vào giấc ngủ nặng nề sau ca trực đêm kinh hoàng... Những cơn ác mộng về quái vật không đầu giữa rừng Blackwood và tiếng cào cửa lộc cộc bủa vây tâm trí..."*

---

### [CẢNH 02: BÁO THỨC ĐIỆN THOẠI & CA TRỰC NGÀY 2 BẮT ĐẦU (11:30 PM)]
- Sau vài giây ác mộng, tiếng chuông báo thức điện thoại vang lên inh ỏi: **RENG! RENG! RENG!**
- Màn hình sáng dần: Đồng hồ hiển thị **11:30 PM**, người chơi thức dậy đứng cạnh giường ngủ.
- **Độc thoại:** *"11:30 PM: Tiếng chuông báo thức réo vang! Đã đến giờ chuẩn bị cho Ca trực Ngày 2. Cần phải kiếm đủ tiền trả hết nợ..."*
- **UI Nhiệm vụ Ngày 2:**  
  `[MỤC TIÊU: Bước ra sân và [E] Lên xe máy đi làm Ngày 2]`
- Người chơi bước ra sân, lên xe máy tiếp tục chuyến hành trình ca trực Ngày 2 với độ khó leo thang!

---

## MÀN 8: QUY TẮC NÂNG CẤP & ĐỘ KHÓ LEOC THANG TRONG CA TRỰC NGÀY 2

1. **Khách hàng Mua sắm Phức tạp hơn:**
   - Mỗi khách mua từ 4 đến 5 món hàng (thay vì 3 món như Đêm 1).
   - Tốc độ di chuyển và thời gian sốt ruột của khách nhanh hơn.
2. **Kẻ Giả Mạo Doppelgänger Tinh vi hơn:**
   - Sử dụng thẻ căn cước có vết xước làm mờ năm sinh, mã công dân bị sửa chữa.
   - Khi đứng đợi tại quầy, mắt của kẻ giả mạo chuyển sang màu đỏ ngầu hoặc đầu giật bất thường.
3. **Cúp Điện Toàn Phần Sớm Hơn:**
   - Mất điện lúc **03:00 AM** (sớm hơn 30 phút so với Đêm 1).
   - Đèn pin cá nhân `[F]` tiêu hao năng lượng nhanh hơn, bầy quái vật vây tiệm đông hơn.
4. **Boss Trùm Cường Hóa:**
   - Boss xuất hiện với đòn nhảy dậm vồ cự ly xa (Leap Attack).
   - Đòi hỏi người chơi phải tận dụng tối đa 6 dãy kệ hàng siêu thị 440m² để thả diều (kiting) và mua tiếp tế kịp thời từ máy tính quầy thu ngân.

---

## ⚙️ HỆ THỐNG MENU CÀI ĐẶT & TẠM DỪNG GAME (PAUSE & SETTINGS MENU SYSTEM)

Trò chơi tích hợp giao diện Tạm dừng & Cài đặt chuyên nghiệp, hỗ trợ đầy đủ 2 ngôn ngữ và điều chỉnh âm lượng:
- **Phím tắt kích hoạt:** Nhấn phím `[ESC]` hoặc `[P]` trên bàn phím, hoặc click trực tiếp vào **Nút bánh răng ⚙ trên góc phải HUD**.
- **Cơ chế:**
  - `Time.timeScale = 0`: Đóng băng toàn bộ hoạt động vật lý, di chuyển của quái vật, khách hàng và đồng hồ ca trực.
  - Mở khóa chuột (`CursorLockMode.None`), hiện con trỏ chuột để người chơi thao tác menu.
  - Phủ một lớp màn mờ đen toàn màn hình tạo chiều sâu.
- **Các tính năng cài đặt:**
  1. **Chuyển đổi Ngôn ngữ Tức thời (Instant Localization):**
     - Nút `[🇻🇳 Tiếng Việt]` và `[🇬🇧 English]`.
     - Click chuyển đổi ngôn ngữ tức khắc cho toàn bộ UI: Nhiệm vụ, Gợi ý phím `[E]`, Hộp thoại phụ đề, Máy tính quầy, Bảng tổng kết và Menu cài đặt.
     - Lưu lại lựa chọn vào `PlayerPrefs` để ghi nhớ cho các lần chơi sau.
  2. **Thanh Trượt Âm Lượng Môi Trường & Hiệu Ứng (SFX & Ambience Volume Slider):**
     - Điều chỉnh âm lượng từ `0%` đến `100%`.
     - Tác động trực tiếp lên tiếng mưa gió, sấm sét, tiếng xe máy, bước chân, tiếng súng đạn, tiếng chà sàn và tiếng chuông cửa.
  3. **Thanh Trượt Âm Lượng Nhạc Nền (Music & BGM Volume Slider):**
     - Điều chỉnh âm lượng nhạc nền hồi hộp, rùng rợn và nhạc căng thẳng khi Boss xuất hiện.
  4. **Các nút điều hướng:**
     - `TIẾP TỤC CHƠI [ESC]` / `RESUME`: Đóng menu, khôi phục `Time.timeScale = 1`, khóa lại chuột vào tâm ngắm FPS.
     - `CHƠI LẠI CA TRỰC` / `RESTART`: Nạp lại màn chơi từ đầu ca.
     - `THOÁT GAME` / `QUIT`: Đóng ứng dụng an toàn.
