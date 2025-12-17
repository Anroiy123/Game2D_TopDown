# TÀI LIỆU THAM KHẢO: SCENE VÀ FLAGS

## MỤC LỤC
1. [Tổng quan hệ thống](#tổng-quan-hệ-thống)
2. [Danh sách Scene theo thứ tự](#danh-sách-scene-theo-thứ-tự)
3. [Danh sách Flags](#danh-sách-flags)
4. [Danh sách Variables](#danh-sách-variables)
5. [Bảng tham chiếu nhanh](#bảng-tham-chiếu-nhanh)

---

## TỔNG QUAN HỆ THỐNG

### Quy ước đặt tên Scene ID
- **Day1_Scene[X]_[Location]** - Ngày 1, Scene X, Địa điểm
- **Week1_Scene[X]_[Location]** - Sau 1 tuần, Scene X, Địa điểm
- **Day2_Scene[X]_[Location]** - Ngày tiếp theo (sau Week1)
- **Week3_Scene[X]_[Location]** - Sau 1-2 tuần (Phase 2)
- **CriticalDay_Scene[X]_[Location]** - Ngày quyết định quan trọng
- **Ending[X]_Scene[X]_[Location]** - Scene trong kết cục

### Quy ước đặt tên Flag
- **[phase]_scene[X]_completed** - Đánh dấu scene đã hoàn thành
- **[action]_[target]** - Đánh dấu hành động đã thực hiện
- **choice_[description]** - Đánh dấu lựa chọn đã chọn

---

## DANH SÁCH SCENE THEO THỨ TỰ

### 📅 NGÀY 1: NGÀY ĐẦU TIÊN

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| 1 | `Day1_Scene1_Bedroom` | 🎬 VN | Phòng ngủ | Mẹ đánh thức Đức |
| 2 | `Day1_Scene2_Livingroom` | 🎬 VN | Phòng khách | Ăn sáng, đi học |
| 3 | `Day1_Scene3_Classroom` | 🎬 VN | Lớp học | Giới thiệu bản thân |
| 4 | `Day1_Scene4_Street` | 🎬 VN | Đường phố | Suy nghĩ trên đường về |
| 5 | *(Top-down gameplay)* | 🎮 TD | Đường phố | Đi về nhà, bị theo dõi |
| 6 | `Day1_Scene6_Street_Noticed` | 🎬 VN | Đường phố | Phát hiện bị theo dõi |
| 7A | `Day1_Scene7A_Confrontation` | 🎬 VN | Đường phố | Đối mặt tụi bắt nạt (Lựa chọn A) |
| 7B | *(Top-down gameplay)* | 🎮 TD | Đường phố | Bỏ chạy về nhà (Lựa chọn B) |
| 8 | `Day1_Scene8_Home_Livingroom` | 🎮 TD | Phòng khách | Về nhà, mẹ hỏi thăm |
| 9 | *(Top-down gameplay)* | 🎮 TD | Cầu thang | Lên phòng |

### 📅 SAU 1 TUẦN

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| 10 | `Week1_Scene1_Classroom_Break` | 🎬 VN | Lớp học | Bạn rủ ra chơi |
| 11 | *(Top-down gameplay)* | 🎮 TD | Lớp học | Đám bạn đi ra |
| 12 | `Week1_Scene2_TeacherRoom` | 🎬 VN | Phòng GV | Cô giáo khuyên nhủ |
| 13 | `Week1_Scene3_Street_Followed` | 🎮 TD | Đường phố | Bị theo dõi lần 2 |
| 14A | `Week1_Scene4A_SecondEncounter` | 🎬 VN | Đường phố | Đối mặt lần 2 |
| 14A-1 | *(Top-down gameplay)* | 🎮 TD | Đường phố | Bị vây quanh |
| 14A-2 | `Week1_Scene4A2_Forced_Friend` | 🎬 VN | Đường phố | Ép buộc kết bạn |
| 14B | *(Top-down gameplay)* | 🎮 TD | Đường phố | Bỏ chạy lần 2 |
| 15 | `Week1_Scene5_Home` |  TD | Phòng khách | Về nhà, mẹ nhắc nhở |
| 16 | *(Top-down gameplay)* |  TD | Cầu thang | Lên phòng |

###  NGÀY TIẾP THEO (Day 2)

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| 17 | `Day2_Scene1_Classroom` |  TD | Lớp học | Cuối giờ học |
| 18 | *(Top-down gameplay)* |  TD | Lớp học | Bạn đi ra ngoài |
| 19 | *(Top-down gameplay)* |  TD | Lớp học | Chuẩn bị về nhà |
| 20 | `Day2_Scene2_Street` |  TD | Đường phố | Gặp tụi bắt nạt xin tiền |

###  SAU 1-2 TUẦN (Phase 2 - Week 3)

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| 21 | `Week3_Scene1_Home_Morning` |  TD | Phòng khách | Tin tức TV về bạo lực |
| 22 | `Week3_Scene2_Door_News` |  VN | Cửa nhà | Tin tức radio |
| 23 | *(Màn hình đen)* |  | - | Thông điệp quan trọng |
| 24 | `Week3_Scene3_Classroom` |  TD | Lớp học | Bạn rủ ra chơi lần nữa |
| 25 | `Week3_Scene4_Street_Blocked` |  TD | Đường phố | Bị chặn đầu |

###  NGÀY QUYẾT ĐỊNH (Critical Day)

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| 26 | `CriticalDay_Scene1_Home_Morning` |  TD | Phòng khách | Mẹ dặn về sớm |
| 27 | `CriticalDay_Scene2_School` |  TD | Lớp học | Suy nghĩ về chiều nay |
| 28 | `CriticalDay_Scene3_Street` |  TD | Đường phố | Gặp tụi bắt nạt, trò chơi |

---

## DANH SÁCH SCENE KẾT CỤC

###  ENDING 1: Good_StandUp (Đứng lên chống lại)

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| E1-1 | `Ending1_Scene1_Home` |  TD | Phòng khách | Về nhà thú nhận |
| E1-2 | `Ending1_Scene2_PhoneCall` |  VN | Phòng khách | Mẹ gọi điện cô giáo |
| E1-3 | `Ending1_Scene3_Classroom` |  VN | Lớp học | Cô giáo hỏi thăm |
| E1-4 | `Ending1_Scene4_School_Protected` |  TD | Trường | Bạn bè hộ tống |

###  ENDING 2: True_TellParents (Chia sẻ với gia đình)

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| E2-1 | `Ending2_Scene1_Home` |  TD | Phòng khách | Về nhà thú nhận |
| E2-2 | `Ending2_Scene2_PhoneCall` |  VN | Phòng khách | Mẹ gọi điện |
| E2-3 | `Ending2_Scene3_Resolution` |  VN | - | Chuyển trường an toàn |

###  ENDING 3: Bad_Murder (Bi kịch mang dao)

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| E3-1 | `Ending3_Scene1_Bedroom_Knife` |  VN | Phòng ngủ | Quyết định mang dao |
| E3-2 | `Ending3_Scene2_Morning` |  TD | Phòng khách | Sáng hôm sau |
| E3-3 | `Ending3_Scene3_Classroom` |  VN | Lớp học | Hoảng loạn |
| E3-4 | `Ending3_Scene4_Street_Knife` | / | Đường phố | Đối đầu với dao |
| E3-5 | `Ending3_Scene5_Home_News` |  VN | Nhà | Hàng xóm báo tin |
| E3-6 | `Ending3_Scene6_Court` |  VN | Tòa án | 3 tháng sau - Tuyên án |

###  ENDING 4: Bad_Death (Cái giá của im lặng)

| # | Scene ID | Chế độ | Địa điểm | Mô tả |
|---|----------|--------|----------|-------|
| E4-1 | `Ending4_Scene1_Bedroom_NoKnife` |  VN | Phòng ngủ | Không mang dao |
| E4-2 | `Ending4_Scene2_Morning` |  TD | Phòng khách | Sáng hôm sau |
| E4-3 | `Ending4_Scene3_Classroom` |  VN | Lớp học | Hoảng loạn |
| E4-4 | `Ending4_Scene4_Street_Game` |  VN | Đường phố | Trò chơi Ai là vua |
| E4-5 | `Ending4_Scene5_Death` |  VN | Đường phố | Bị đánh chết |
| E4-6 | `Ending4_Scene6_Funeral` |  VN | Đám tang | Đám tang |

---

## DANH SÁCH FLAGS

###  FLAGS TIẾN TRÌNH SCENE (Scene Progress)

| Flag Name | Mô tả | Set tại Scene |
|-----------|-------|---------------|
| `day1_started` | Bắt đầu ngày 1 | Day1_Scene1 |
| `day1_scene1_completed` | Hoàn thành Scene 1 | Day1_Scene1 |
| `day1_scene2_completed` | Hoàn thành Scene 2 | Day1_Scene2 |
| `day1_classroom_completed` | Hoàn thành lớp học | Day1_Scene3 |
| `day1_scene4_street_completed` | Hoàn thành đường phố | Day1_Scene4 |
| `day1_scene5_bullies_spawned` | Tụi bắt nạt xuất hiện | Day1_Scene5 (TD) |
| `Day1_Scene6_Completed` | Phát hiện bị theo dõi | Day1_Scene6 |
| `day1_scene7_completed` | Hoàn thành đối mặt/chạy | Day1_Scene7 |
| `day1_scene8_completed` | Về đến nhà | Day1_Scene8 |
| `week1_started` | Bắt đầu tuần 1 | Week1_Scene1 |
| `week1_classroom_break_completed` | Hoàn thành giờ ra chơi | Week1_Scene1 |
| `week1_teacher_talk_completed` | Nói chuyện với cô giáo | Week1_Scene2 |
| `week1_second_encounter_completed` | Gặp lần 2 xong | Week1_Scene4 |
| `day2_started` | Bắt đầu ngày 2 | Day2_Scene1 |
| `week3_started` | Bắt đầu tuần 3 | Week3_Scene1 |
| `critical_day_started` | Bắt đầu ngày quyết định | CriticalDay_Scene1 |

###  FLAGS CỐT TRUYỆN CHÍNH (Story Flags)

| Flag Name | Constant | Mô tả | Điều kiện set |
|-----------|----------|-------|---------------|
| `met_bullies` | `FlagKeys.MET_BULLIES` | Đã gặp tụi bắt nạt | Khi gặp lần đầu (Scene 6-7) |
| `befriended_bullies` | `FlagKeys.BEFRIENDED_BULLIES` | Đã kết bạn với tụi bắt nạt | Chọn đối mặt ở Week1_Scene4A |
| `got_beaten` | `FlagKeys.GOT_BEATEN` | Đã bị đánh | Khi bị đánh (nhiều scene) |
| `talked_to_teacher` | `FlagKeys.TALKED_TO_TEACHER` | Đã nói chuyện với cô giáo | Week1_Scene2 |
| `invited_by_classmate` | `FlagKeys.INVITED_BY_CLASSMATE` | Được bạn rủ đi chơi | Week1_Scene1, Week3_Scene3 |
| `rejected_classmate` | `FlagKeys.REJECTED_CLASSMATE` | Từ chối bạn | Khi từ chối lời mời |
| `mom_worried` | `FlagKeys.MOM_WORRIED` | Mẹ lo lắng | Khi mẹ nhận ra bất thường |
| `confessed_to_mom` | `FlagKeys.CONFESSED_TO_MOM` | Đã thú nhận với mẹ | Ending 1, 2 |
| `brought_knife` | `FlagKeys.BROUGHT_KNIFE` | Đã mang dao | Ending 3 |
| `stood_up_to_bullies` | *(không có constant)* | Đã đứng lên chống lại | Ending 1 |

###  FLAGS LỰA CHỌN (Choice Flags)

| Flag Name | Mô tả | Set khi |
|-----------|-------|---------|
| `choice_confronted_day1` | Chọn đối mặt ngày 1 | Lựa chọn 1.1A |
| `choice_ran_away_day1` | Chọn bỏ chạy ngày 1 | Lựa chọn 1.1B |
| `choice_confronted_week1` | Chọn đối mặt tuần 1 | Lựa chọn 1.2A |
| `choice_ran_away_week1` | Chọn bỏ chạy tuần 1 | Lựa chọn 1.2B |
| `choice_gave_money` | Đã đưa tiền | Lựa chọn 2.1 |
| `choice_refused_money` | Từ chối đưa tiền | Lựa chọn 2.1 |
| `choice_fight_back` | Chọn đánh trả | Lựa chọn cuối |
| `choice_submit` | Chọn chịu đựng | Lựa chọn cuối |
| `choice_tell_mom` | Chọn thú nhận với mẹ | Sau khi bị đánh |
| `choice_hide_from_mom` | Chọn giấu mẹ | Sau khi bị đánh |
| `choice_bring_knife` | Chọn mang dao | Ending 3 |
| `choice_no_knife` | Chọn không mang dao | Ending 4 |

###  FLAGS TRẠNG THÁI (State Flags)

| Flag Name | Mô tả | Sử dụng |
|-----------|-------|---------|
| `bedroom_scene_visited` | Đã vào phòng ngủ | Kiểm tra lần đầu |
| `confronted_bullies` | Đã đối mặt với tụi bắt nạt | Conditional dialogue |
| `is_being_bullied` | Đang bị bắt nạt | Trạng thái hiện tại |
| `saw_news_about_bullying` | Đã xem tin tức | Week3_Scene1-2 |
| `received_warning_message` | Đã nhận thông điệp cảnh báo | Week3 |

---

## DANH SÁCH VARIABLES

###  VARIABLES HỆ THỐNG

| Variable Name | Constant | Giá trị mặc định | Mô tả |
|---------------|----------|------------------|-------|
| `current_day` | `VarKeys.CURRENT_DAY` | 1 | Ngày hiện tại trong game |
| `money` | `VarKeys.MONEY` | 50000 | Số tiền còn lại (VND) |
| `fear_level` | `VarKeys.FEAR_LEVEL` | 0 | Mức độ sợ hãi (0-100) |
| `escaped_count` | `VarKeys.ESCAPED_COUNT` | 0 | Số lần bỏ chạy |
| `gave_money_count` | `VarKeys.GAVE_MONEY_COUNT` | 0 | Số lần đưa tiền |
| `relationship_classmate` | `VarKeys.RELATIONSHIP_CLASSMATE` | 0 | Mối quan hệ với bạn cùng lớp |

###  BIẾN ĐỔI VARIABLES THEO SCENE

| Scene | Variable | Thay đổi | Ghi chú |
|-------|----------|----------|---------|
| Day1_Scene1 | `current_day` | Set = 1 | Bắt đầu game |
| Week1_Scene1 | `current_day` | Set = 8 | Sau 1 tuần |
| Day2_Scene1 | `current_day` | Add + 1 | Ngày tiếp theo |
| Week3_Scene1 | `current_day` | Set = 15-21 | Sau 1-2 tuần |
| CriticalDay | `current_day` | Add + 1 | Ngày quyết định |
| Khi đưa tiền | `money` | Sub - [amount] | Mất tiền |
| Khi bị đánh | `fear_level` | Add + 20-30 | Tăng sợ hãi |
| Khi bỏ chạy | `escaped_count` | Add + 1 | Đếm số lần chạy |
| Khi đưa tiền | `gave_money_count` | Add + 1 | Đếm số lần đưa |
| Khi từ chối bạn | `relationship_classmate` | Sub - 5 | Giảm quan hệ |
| Khi chấp nhận bạn | `relationship_classmate` | Add + 10 | Tăng quan hệ |

---

## BẢNG THAM CHIẾU NHANH

###  FLOW CHÍNH VÀ FLAGS CẦN SET

```
Day1_Scene1_Bedroom
 setFlagsOnEnter: [day1_started, day1_scene1_completed]
 variableChanges: [current_day = 1]
 forbiddenFlags: [day1_scene1_completed]

Day1_Scene2_Livingroom
 setFlagsOnEnter: [day1_scene2_completed]
 forbiddenFlags: [day1_scene2_completed]

Day1_Scene3_Classroom
 setFlagsOnComplete: [day1_classroom_completed]
 requiredFlags: [day1_scene2_completed]

Day1_Scene4_Street
 setFlagsOnComplete: [day1_scene4_street_completed]
 forbiddenFlags: [day1_scene4_street_completed]

Day1_Scene5 (Top-down)
 setFlags: [day1_scene5_bullies_spawned]
 requiredFlags: [day1_scene4_street_completed]

Day1_Scene6_Street_Noticed
 setFlagsOnComplete: [Day1_Scene6_Completed, met_bullies]
 requiredFlags: [day1_scene5_bullies_spawned]
 forbiddenFlags: [Day1_Scene6_Completed]

Day1_Scene7A_Confrontation (Lựa chọn A)
 setFlagsOnComplete: [day1_scene7_completed, choice_confronted_day1]
 requiredFlags: [Day1_Scene6_Completed]

Day1_Scene7B (Lựa chọn B - Top-down)
 setFlags: [day1_scene7_completed, choice_ran_away_day1]
 variableChanges: [escaped_count + 1]
 requiredFlags: [Day1_Scene6_Completed]

Day1_Scene8_Home_Livingroom
 setFlagsOnComplete: [day1_scene8_completed]
 requiredFlags: [day1_scene7_completed]
```

###  ĐIỀU KIỆN ENDING

| Ending | Flags cần có | Flags không được có | Variables |
|--------|--------------|---------------------|-----------|
| **Good_StandUp** | `stood_up_to_bullies`, `confessed_to_mom` | `brought_knife` | - |
| **True_TellParents** | `confessed_to_mom`, `got_beaten` | `stood_up_to_bullies`, `brought_knife` | - |
| **Bad_Murder** | `brought_knife` | - | - |
| **Bad_Death** | `got_beaten` | `confessed_to_mom`, `brought_knife`, `stood_up_to_bullies` | `fear_level >= 100` |

###  CHECKLIST TẠO SCENE MỚI

Khi tạo VNSceneData mới, cần điền:

1. **Scene Info**
   - [ ] `sceneName`: ID duy nhất (VD: `Day1_Scene1_Bedroom`)
   - [ ] `locationText`: Hiển thị cho người chơi (VD: Phòng ngủ - 7:00 AM)

2. **Background và Characters**
   - [ ] `backgroundImage`: Sprite background
   - [ ] `characters`: Danh sách nhân vật hiển thị

3. **Dialogue**
   - [ ] `dialogue`: Reference đến DialogueData asset

4. **Transition**
   - [ ] `nextScene`: Scene VN tiếp theo (nếu có)
   - [ ] `returnToTopDown`: True nếu quay về gameplay
   - [ ] `topDownSceneName`: Tên scene Unity để load
   - [ ] `spawnPointId`: Vị trí spawn trong scene

5. **Conditions**
   - [ ] `requiredFlags`: Flags cần có để trigger scene
   - [ ] `forbiddenFlags`: Flags không được có (tránh lặp)

6. **Effects**
   - [ ] `setFlagsOnEnter`: Flags set khi bắt đầu scene
   - [ ] `setFlagsOnComplete`: Flags set khi hoàn thành
   - [ ] `variableChangesOnEnter`: Thay đổi biến khi bắt đầu
   - [ ] `variableChangesOnComplete`: Thay đổi biến khi hoàn thành

---

## GHI CHÚ

### Quy tắc quan trọng

1. **Tránh lặp scene**: Luôn thêm flag `[scene_id]_completed` vào `forbiddenFlags`
2. **Thứ tự scene**: Sử dụng `requiredFlags` để đảm bảo thứ tự đúng
3. **Biến tiền**: Luôn kiểm tra `money >= amount` trước khi trừ
4. **Fear level**: Tối đa 100, khi đạt 100 + `got_beaten` -> Bad_Death

### Tham khảo code

```csharp
// Trong StoryManager.cs
public static class FlagKeys
{
    public const string DAY_1_COMPLETED = "day_1_completed";
    public const string MET_BULLIES = "met_bullies";
    public const string BEFRIENDED_BULLIES = "befriended_bullies";
    public const string GOT_BEATEN = "got_beaten";
    public const string TALKED_TO_TEACHER = "talked_to_teacher";
    public const string INVITED_BY_CLASSMATE = "invited_by_classmate";
    public const string REJECTED_CLASSMATE = "rejected_classmate";
    public const string MOM_WORRIED = "mom_worried";
    public const string CONFESSED_TO_MOM = "confessed_to_mom";
    public const string BROUGHT_KNIFE = "brought_knife";
}

public static class VarKeys
{
    public const string CURRENT_DAY = "current_day";
    public const string MONEY = "money";
    public const string FEAR_LEVEL = "fear_level";
    public const string ESCAPED_COUNT = "escaped_count";
    public const string GAVE_MONEY_COUNT = "gave_money_count";
    public const string RELATIONSHIP_CLASSMATE = "relationship_classmate";
}
```

---

*Tài liệu này được tạo dựa trên docs/story.md và code trong Assets/Scripts/Core/StoryManager.cs*
