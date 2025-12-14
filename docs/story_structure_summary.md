# TÓM TẮT CẤU TRÚC CỐT TRUYỆN - PHÂN LOẠI CẢNH

## HƯỚNG DẪN PHÂN LOẠI

### 🎬 Visual Novel Mode (Background + Dialogue)
- Sử dụng background tĩnh
- Hiển thị avatar nhân vật
- Tập trung vào hội thoại
- **Ký hiệu**: `🎬` + `Visual Novel`

### 🎮 Top-down Mode (Gameplay)
- Người chơi điều khiển nhân vật
- Di chuyển tự do trong scene
- Tương tác với môi trường
- **Ký hiệu**: `🎮` + `Top-down`

### 🔀 Lựa chọn (Choice Point)
- Điểm phân nhánh câu chuyện
- **Ký hiệu**: `🔀`

---

## CẤU TRÚC TỔNG QUAN

### NGÀY 1: NGÀY ĐẦU TIÊN
1. 🎬 Cảnh 1: Phòng ngủ - Mẹ đánh thức (VN)
2. 🎬 Cảnh 2: Phòng khách - Ăn sáng (VN)
3. 🎬 Cảnh 3: Lớp học - Giới thiệu (VN)
4. 🎬 Cảnh 4: Trên đường - Suy nghĩ (VN)
5. 🎮 Cảnh 5: Đi đường về nhà (Top-down)
6. 🎬 Cảnh 6: Phát hiện bị theo dõi (VN)
7. 🔀 **LỰA CHỌN 1.1**: Gặp tụi bắt nạt lần đầu
   - 🎬 7A: Quay lại hỏi thăm (VN)
   - 🎮 7B: Bỏ chạy (Top-down)
8. 🎮 Cảnh 8: Về đến nhà (Top-down)
9. 🎮 Cảnh 9: Lên phòng (Top-down)

### SAU 1 TUẦN
10. 🎬 Cảnh 10: Giờ ra chơi - Bạn rủ (VN)
11. 🎮 Cảnh 11: Đám bạn đi ra (Top-down)
12. 🎬 Cảnh 12: Gặp giáo viên (VN)
13. 🎮 Cảnh 13: Trên đường về (Top-down)
14. 🔀 **LỰA CHỌN 1.2**: Gặp lại tụi bắt nạt
    - 🎬 14A: Đối mặt lần 2 (VN)
    - 🎮 14A-1: Bị vây quanh (Top-down)
    - 🎬 14A-2: Ép buộc kết bạn (VN)
    - 🎮 14B: Bỏ chạy lần 2 (Top-down)
15. 🎮 Cảnh 15: Về nhà (Top-down)
16. 🎮 Cảnh 16: Lên phòng (Top-down)

### NGÀY TIẾP THEO
17. 🎮 Cảnh 17: Cuối giờ học (Top-down)
18. 🎮 Cảnh 18: Bạn đi ra ngoài (Top-down)
19. 🎮 Cảnh 19: Chuẩn bị về nhà (Top-down)
20. 🎮 Cảnh 20: Trên đường về (Top-down)
21. 🔀 **LỰA CHỌN 2.1**: Xin tiền lần đầu
    - 🎬 Đối mặt (VN)
    - 🎮 Bỏ chạy lần 3 (Top-down)

### SAU 1-2 TUẦN (PHASE 2)
21. 🎮 Cảnh 21: Buổi sáng - Tin tức TV (Top-down)
22. 🎬 Cảnh 22: Khi mở cửa - Tin tức radio (VN)
23. ⚠️ **THÔNG ĐIỆP QUAN TRỌNG** (Màn hình đen)
24. 🎮 Cảnh 23: Trên trường (Top-down)
25. 🎮 Cảnh 24: Trên đường về - Bị chặn (Top-down)
26. 🔀 **LỰA CHỌN 3**: Quyết định quan trọng

### HÔM SAU - ĐIỂM QUYẾT ĐỊNH QUAN TRỌNG NHẤT
27. 🎮 Buổi sáng - Mẹ dặn về sớm (Top-down)
28. 🎮 Trên trường - Suy nghĩ (Top-down)
29. 🎮 Trên đường về - Gặp tụi bắt nạt (Top-down)
30. 🎬 Trò chơi "Tù xì bạt tai" (VN hoặc Top-down)
31. 🔀 **LỰA CHỌN CUỐI CÙNG - ĐỊNH MỆNH**
    - **Lựa chọn 1**: Đối mặt và chiến đấu → **ENDING 1** (Good_StandUp)
    - **Lựa chọn 2**: Từ chối và bị đánh
      - Thú nhận với mẹ → **ENDING 2** (True_TellParents)
      - Giấu mẹ → Lựa chọn mang dao
        - Mang dao → **ENDING 3** (Bad_Murder)
        - Không mang dao → **ENDING 4** (Bad_Death)

---

## CÁC KẾT CỤC

### 🎯 ENDING 1: Good_StandUp (Đứng lên chống lại)
- 🎬 Về nhà - Thú nhận với mẹ (VN)
- 🎬 Mẹ gọi điện cho giáo viên (VN)
- 🎬 Sáng hôm sau - Cô giáo hỏi thăm (VN)
- 🎮 Trong tuần - Bạn bè đi cùng về (Top-down)
- **Thông điệp**: Đứng lên chống lại và tìm kiếm sự giúp đỡ

### 🎯 ENDING 2: True_TellParents (Chia sẻ với gia đình)
- 🎬 Về nhà - Thú nhận với mẹ (VN)
- 🎬 Mẹ gọi điện cho giáo viên (VN)
- 🎬 Sau đó - Chuyển trường an toàn (VN)
- **Thông điệp**: Chia sẻ với gia đình khi gặp khó khăn

### 🎯 ENDING 3: Bad_Murder (Bi kịch của sự hoảng loạn)
- 🎬 Trong phòng - Suy nghĩ mang dao (VN)
- 🎮 Sáng hôm sau - Đi học (Top-down)
- 🎬 Trên lớp - Hoảng loạn (VN)
- 🎬/🎮 Chiều về - Đối đầu với dao (VN/Top-down)
- 🎬 Tại nhà - Hàng xóm báo tin (VN)
- 🎬 3 tháng sau - Tòa án (VN)
- **Thông điệp**: Bạo lực chỉ sinh ra bạo lực

### 🎯 ENDING 4: Bad_Death (Cái giá của sự im lặng)
- 🎬 Trong phòng - Quyết định không mang dao (VN)
- 🎮 Sáng hôm sau - Đi học (Top-down)
- 🎬 Trên lớp - Hoảng loạn (VN)
- 🎬 Chiều về - Trò chơi "Ai là vua" (VN)
- 🎬 Bị đánh chết (VN)
- 🎬 Đám tang (VN)
- **Thông điệp**: Im lặng chịu đựng dẫn đến bi kịch

---

## GHI CHÚ KỸ THUẬT

### Quy tắc chuyển đổi mode:
1. **VN → Top-down**: Khi cần người chơi di chuyển/khám phá
2. **Top-down → VN**: Khi cần tập trung vào hội thoại/cảm xúc
3. **Lựa chọn quan trọng**: Thường dùng VN để nhấn mạnh
4. **Combat/Action**: Có thể dùng Top-down hoặc VN tùy thiết kế

### Scene ID Format:
- `Day1_Scene1_Location` - Ngày 1, Cảnh 1, Địa điểm
- `Week1_Scene1_Location` - Tuần 1, Cảnh 1, Địa điểm
- `Ending1_Scene1_Location` - Kết cục 1, Cảnh 1, Địa điểm

