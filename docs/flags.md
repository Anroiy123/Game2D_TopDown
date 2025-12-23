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

- **Day1*Scene[X]*[Location]** - Ngày 1, Scene X, Địa điểm
- **Week1*Scene[X]*[Location]** - Sau 1 tuần, Scene X, Địa điểm
- **Day2*Scene[X]*[Location]** - Ngày tiếp theo (sau Week1)
- **Week3*Scene[X]*[Location]** - Sau 1-2 tuần (Phase 2)
- **CriticalDay*Scene[X]*[Location]** - Ngày quyết định quan trọng
- **Ending[X]_Scene[X]_[Location]** - Scene trong kết cục

### Quy ước đặt tên Flag

- **[phase]\_scene[X]\_completed** - Đánh dấu scene đã hoàn thành
- **[action]\_[target]** - Đánh dấu hành động đã thực hiện
- **choice\_[description]** - Đánh dấu lựa chọn đã chọn

---

## DANH SÁCH SCENE THEO THỨ TỰ

### 📅 NGÀY 1: NGÀY ĐẦU TIÊN (Scene 1-9)

| Scene | Scene ID                      | Chế độ | Địa điểm    | Mô tả                            |
| ----- | ----------------------------- | ------ | ----------- | -------------------------------- |
| 1     | `Day1_Scene1_Bedroom`         | 🎬 VN  | Phòng ngủ   | Mẹ đánh thức Đức                 |
| 2     | `Day1_Scene2_Livingroom`      | 🎬 VN  | Phòng khách | Ăn sáng, đi học                  |
| 3     | `Day1_Scene3_Classroom`       | 🎬 VN  | Lớp học     | Giới thiệu bản thân              |
| 4     | `Day1_Scene4_Street`          | 🎬 VN  | Đường phố   | Suy nghĩ trên đường về           |
| 5     | _(Top-down gameplay)_         | 🎮 TD  | Đường phố   | Đi về nhà, bị theo dõi           |
| 6     | `Day1_Scene6_Street_Noticed`  | 🎬 VN  | Đường phố   | Phát hiện bị theo dõi            |
| 7A    | `Day1_Scene7A_Confrontation`  | 🎬 VN  | Đường phố   | Đối mặt tụi bắt nạt (Lựa chọn A) |
| 7B    | _(Top-down gameplay)_         | 🎮 TD  | Đường phố   | Bỏ chạy về nhà (Lựa chọn B)      |
| 8     | `Day1_Scene8_Home_Livingroom` | 🎮 TD  | Phòng khách | Về nhà, mẹ hỏi thăm              |
| 9     | _(Top-down gameplay)_         | 🎮 TD  | Cầu thang   | Lên phòng                        |

### 📅 SAU 1 TUẦN (Scene 10-16)

| Scene | Scene ID                         | Chế độ | Địa điểm    | Mô tả               |
| ----- | -------------------------------- | ------ | ----------- | ------------------- |
| 10    | `Week1_Scene10_Classroom_Break`  | 🎬 VN  | Lớp học     | Bạn rủ ra chơi      |
| 11    | _(Top-down gameplay)_            | 🎮 TD  | Lớp học     | Đám bạn đi ra       |
| 12    | `Week1_Scene12_TeacherRoom`      | 🎬 VN  | Phòng GV    | Cô giáo khuyên nhủ  |
| 13    | `Week1_Scene13_Street_Followed`  | 🎮 TD  | Đường phố   | Bị theo dõi lần 2   |
| 14A   | `Week1_Scene14A_SecondEncounter` | 🎬 VN  | Đường phố   | Đối mặt lần 2       |
| 14A-1 | _(Top-down gameplay)_            | 🎮 TD  | Đường phố   | Bị vây quanh        |
| 14A-2 | `Week1_Scene14A2_Forced_Friend`  | 🎬 VN  | Đường phố   | Ép buộc kết bạn     |
| 14B   | _(Top-down gameplay)_            | 🎮 TD  | Đường phố   | Bỏ chạy lần 2       |
| 15    | `Week1_Scene15_Home`             | 🎮 TD  | Phòng khách | Về nhà, mẹ nhắc nhở |
| 16    | _(Top-down gameplay)_            | 🎮 TD  | Cầu thang   | Lên phòng           |

### 📅 NGÀY TIẾP THEO - Day 2 (Scene 17-20)

| Scene | Scene ID                       | Chế độ | Địa điểm    | Mô tả                            |
| ----- | ------------------------------ | ------ | ----------- | -------------------------------- |
| 17    | `Day2_Scene17_Classroom`       | 🎮 TD  | Lớp học     | Cuối giờ học                     |
| 18    | _(Top-down gameplay)_          | 🎮 TD  | Lớp học     | Bạn đi ra ngoài                  |
| 19    | `Day2_Scene19_PrepareToGoHome` | 🎬 VN  | Lớp học     | Chuẩn bị về nhà                  |
| 20    | `Day2_Scene20_Street`          | 🎮 TD  | Đường phố   | Gặp tụi bắt nạt xin tiền lần đầu |
| -     | `Day2_Scene20_Home`            | 🎮 TD  | Phòng khách | Về nhà sau Scene 20              |

### 📅 SAU 1-2 TUẦN - Phase 2 (Scene 21-24)

| Scene | Scene ID                       | Chế độ | Địa điểm    | Mô tả                                  |
| ----- | ------------------------------ | ------ | ----------- | -------------------------------------- |
| 21    | `Week3_Scene21_Home_Morning`   | 🎮 TD  | Phòng khách | TV tin tức + Mẹ nhắc nhở               |
| 22    | `Week3_Scene22_Door_News`      | 🎬 VN  | Cửa nhà     | Loa phát thanh + Thông điệp quan trọng |
| 23    | `Week3_Scene23_Classroom`      | 🎮 TD  | Lớp học     | Bạn rủ ra chơi lần nữa                 |
| 24    | `Week3_Scene24_Street_Blocked` | 🎮 TD  | Đường phố   | Bị chặn đầu xin tiền                   |
| -     | `Week3_Scene24_Home`           | 🎮 TD  | Phòng khách | Về nhà sau Scene 24                    |

### 📅 NGÀY QUYẾT ĐỊNH - Critical Day (Scene 25-27)

| Scene | Scene ID                           | Chế độ | Địa điểm    | Mô tả                           |
| ----- | ---------------------------------- | ------ | ----------- | ------------------------------- |
| 25    | `CriticalDay_Scene25_Home_Morning` | 🎮 TD  | Phòng khách | Mẹ dặn về sớm                   |
| 26    | `CriticalDay_Scene26_Classroom`    | 🎮 TD  | Lớp học     | Suy nghĩ về chiều nay           |
| 27    | `CriticalDay_Scene27_Street`       | 🎮 TD  | Đường phố   | Gặp tụi bắt nạt, trò chơi Tù xì |

---

## DANH SÁCH SCENE KẾT CỤC (CẤU TRÚC MỚI - ĐƠN GIẢN HÓA)

### 🏆 ENDING 1: Good_StandUp (Đứng lên chống lại)

| Scene | Scene ID                             | Chế độ   | Địa điểm    | Mô tả                                         |
| ----- | ------------------------------------ | -------- | ----------- | --------------------------------------------- |
| 28A   | `CriticalDay_Scene28A_Home_AfterWin` | 🎬 VN    | Phòng khách | Về nhà thú nhận sau khi thắng (dialogue ngắn) |
| -     | **STORYTELLING ENDING 1**            | 📖 Story | -           | Text cuộn kể cuộc đời sau đó                  |

### 🏆 ENDING 2: True_TellParents (Chia sẻ với gia đình)

| Scene | Scene ID                           | Chế độ   | Địa điểm    | Mô tả                                      |
| ----- | ---------------------------------- | -------- | ----------- | ------------------------------------------ |
| 28B   | `CriticalDay_Scene28B_Home_Choice` | 🎬 VN    | Phòng khách | Về nhà - Lựa chọn thú nhận (dialogue ngắn) |
| -     | **STORYTELLING ENDING 2**          | 📖 Story | -           | Text cuộn kể cuộc đời sau đó               |

### 💀 ENDING 3: Bad_DarkLife (Cuộc đời đen tối)

| Scene | Scene ID                           | Chế độ   | Địa điểm    | Mô tả                                     |
| ----- | ---------------------------------- | -------- | ----------- | ----------------------------------------- |
| 28B   | `CriticalDay_Scene28B_Home_Choice` | 🎬 VN    | Phòng khách | Về nhà - Lựa chọn giấu mẹ (dialogue ngắn) |
| -     | **STORYTELLING ENDING 3**          | 📖 Story | -           | Text cuộn kể cuộc đời đen tối             |

### 📝 THAY ĐỔI QUAN TRỌNG

**Đã bỏ:**

- ❌ Scene phòng ngủ quyết định mang dao
- ❌ Scene tòa án, đám tang
- ❌ Scene giết người, chết
- ❌ Tổng cộng bỏ ~20 scene phức tạp

**Thay thế bằng:**

- ✅ Chỉ 2 scene dialogue ngắn (28A, 28B)
- ✅ 3 storytelling sequences (text-based)
- ✅ Giảm 90% công sức phát triển

---

## DANH SÁCH FLAGS

### 🚩 FLAGS TIẾN TRÌNH SCENE (Scene Progress)

| Flag Name                    | Mô tả                   | Set tại Scene |
| ---------------------------- | ----------------------- | ------------- |
| `day1_started`               | Bắt đầu ngày 1          | Scene 1       |
| `day1_scene1_completed`      | Hoàn thành Scene 1      | Scene 1       |
| `day1_scene2_completed`      | Hoàn thành Scene 2      | Scene 2       |
| `day1_scene3_completed`      | Hoàn thành Scene 3      | Scene 3       |
| `day1_scene4_completed`      | Hoàn thành Scene 4      | Scene 4       |
| `day1_scene6_completed`      | Phát hiện bị theo dõi   | Scene 6       |
| `day1_scene7_completed`      | Hoàn thành đối mặt/chạy | Scene 7       |
| `day1_scene8_completed`      | Về đến nhà              | Scene 8       |
| `week1_started`              | Bắt đầu tuần 1          | TimeSkip      |
| `week1_scene10_completed`    | Hoàn thành giờ ra chơi  | Scene 10      |
| `week1_scene12_completed`    | Nói chuyện với cô giáo  | Scene 12      |
| `week1_scene14_completed`    | Gặp lần 2 xong          | Scene 14      |
| `week1_scene15_completed`    | Về nhà tuần 1           | Scene 15      |
| `day2_started`               | Bắt đầu ngày 2          | TimeSkip      |
| `day2_scene17_completed`     | Hoàn thành Scene 17     | Scene 17      |
| `day2_scene19_completed`     | Hoàn thành Scene 19     | Scene 19      |
| `day2_scene20_completed`     | Hoàn thành Scene 20     | Scene 20      |
| `day2_scene21_completed`     | Về nhà sau Scene 20     | Scene 20 Home |
| `week3_started`              | Bắt đầu Phase 2         | TimeSkip      |
| `week3_scene21_completed`    | Hoàn thành Scene 21     | Scene 21      |
| `week3_scene22_completed`    | Hoàn thành Scene 22     | Scene 22      |
| `week3_scene23_completed`    | Hoàn thành Scene 23     | Scene 23      |
| `week3_scene24_completed`    | Hoàn thành Scene 24     | Scene 24      |
| `critical_day_started`       | Bắt đầu ngày quyết định | TimeSkip      |
| `critical_scene25_completed` | Hoàn thành Scene 25     | Scene 25      |
| `critical_scene26_completed` | Hoàn thành Scene 26     | Scene 26      |
| `critical_scene27_completed` | Hoàn thành Scene 27     | Scene 27      |

### 🚩 FLAGS CỐT TRUYỆN CHÍNH (Story Flags)

| Flag Name                     | Constant                        | Mô tả                        | Điều kiện set              |
| ----------------------------- | ------------------------------- | ---------------------------- | -------------------------- |
| `met_bullies`                 | `FlagKeys.MET_BULLIES`          | Đã gặp tụi bắt nạt           | Scene 6-7                  |
| `befriended_bullies`          | `FlagKeys.BEFRIENDED_BULLIES`   | Đã "kết bạn" với tụi bắt nạt | Scene 14A-2                |
| `got_beaten`                  | `FlagKeys.GOT_BEATEN`           | Đã bị đánh                   | Khi bị đánh                |
| `talked_to_teacher`           | `FlagKeys.TALKED_TO_TEACHER`    | Đã nói chuyện với cô giáo    | Scene 12                   |
| `invited_by_classmate`        | `FlagKeys.INVITED_BY_CLASSMATE` | Được bạn rủ đi chơi          | Scene 10, 23               |
| `rejected_classmate`          | `FlagKeys.REJECTED_CLASSMATE`   | Từ chối bạn                  | Khi từ chối                |
| `mom_worried`                 | `FlagKeys.MOM_WORRIED`          | Mẹ lo lắng                   | Khi mẹ nhận ra             |
| `mom_noticed_something_wrong` | -                               | Mẹ nhận ra có vấn đề         | Scene 20 Home (got_beaten) |
| `confessed_to_mom`            | `FlagKeys.CONFESSED_TO_MOM`     | Đã thú nhận với mẹ           | Scene 28A, 28B (choice A)  |
| `hid_from_mom`                | `FlagKeys.HID_FROM_MOM`         | Đã giấu mẹ                   | Scene 28B (choice B)       |
| `stood_up_to_bullies`         | -                               | Đã đứng lên chống lại        | Scene 27 (fight back)      |
| `ending1_good_standup`        | -                               | Trigger Ending 1             | Scene 28A                  |
| `ending2_true_tellparents`    | -                               | Trigger Ending 2             | Scene 28B (choice A)       |
| `ending3_bad_darklife`        | -                               | Trigger Ending 3             | Scene 28B (choice B)       |

### 🚩 FLAGS LỰA CHỌN (Choice Flags)

| Flag Name                   | Mô tả                       | Set khi              |
| --------------------------- | --------------------------- | -------------------- |
| `choice_confronted_day1`    | Chọn đối mặt ngày 1         | Scene 7A             |
| `choice_ran_away_day1`      | Chọn bỏ chạy ngày 1         | Scene 7B             |
| `choice_confronted_week1`   | Chọn đối mặt tuần 1         | Scene 14A            |
| `choice_ran_away_week1`     | Chọn bỏ chạy tuần 1         | Scene 14B            |
| `choice_confronted_day2`    | Chọn đối mặt ngày 2         | Scene 20             |
| `choice_ran_away_day2`      | Chọn bỏ chạy ngày 2         | Scene 20             |
| `gave_money_to_bullies`     | Đã đưa tiền cho tụi bắt nạt | Scene 20, 24         |
| `refused_money_got_beaten`  | Từ chối đưa tiền và bị đánh | Scene 20, 24         |
| `week3_gave_money`          | Đưa tiền ngay Scene 24      | Scene 24             |
| `week3_gave_money_hesitant` | Do dự rồi đưa tiền Scene 24 | Scene 24             |
| `week3_refused_got_beaten`  | Từ chối và bị đánh Scene 24 | Scene 24             |
| `choice_fight_back`         | Chọn đánh trả               | Scene 27             |
| `choice_submit`             | Chọn chịu đựng              | Scene 27             |
| `choice_tell_mom`           | Chọn thú nhận với mẹ        | Scene 28B (choice A) |
| `choice_hide_from_mom`      | Chọn giấu mẹ                | Scene 28B (choice B) |

### 🚩 FLAGS TRẠNG THÁI (State Flags)

| Flag Name                   | Mô tả                          | Sử dụng      |
| --------------------------- | ------------------------------ | ------------ |
| `player_decided_action`     | Player đã quyết định hành động | Scene 20     |
| `bullies_surrounded_player` | Bullies đã vây quanh player    | Scene 14, 20 |
| `phase2_started`            | Đã bắt đầu Phase 2             | TimeSkip     |
| `phase2_warning_shown`      | Đã hiện thông điệp cảnh báo    | Scene 22     |
| `saw_news_about_bullying`   | Đã xem tin tức                 | Scene 21     |

---

## DANH SÁCH VARIABLES

### 📊 VARIABLES HỆ THỐNG

| Variable Name            | Constant                         | Giá trị mặc định | Mô tả                        |
| ------------------------ | -------------------------------- | ---------------- | ---------------------------- |
| `current_day`            | `VarKeys.CURRENT_DAY`            | 1                | Ngày hiện tại trong game     |
| `money`                  | `VarKeys.MONEY`                  | 50000            | Số tiền còn lại (VND)        |
| `fear_level`             | `VarKeys.FEAR_LEVEL`             | 0                | Mức độ sợ hãi (0-100)        |
| `escaped_count`          | `VarKeys.ESCAPED_COUNT`          | 0                | Số lần bỏ chạy               |
| `gave_money_count`       | `VarKeys.GAVE_MONEY_COUNT`       | 0                | Số lần đưa tiền              |
| `relationship_classmate` | `VarKeys.RELATIONSHIP_CLASSMATE` | 0                | Mối quan hệ với bạn cùng lớp |

### 📊 BIẾN ĐỔI VARIABLES THEO SCENE

| Scene               | Variable           | Thay đổi     | Ghi chú         |
| ------------------- | ------------------ | ------------ | --------------- |
| Scene 1             | `current_day`      | Set = 1      | Bắt đầu game    |
| TimeSkip Week1      | `current_day`      | Set = 8      | Sau 1 tuần      |
| TimeSkip Day2       | `current_day`      | Add + 1      | Ngày tiếp theo  |
| TimeSkip Week3      | `current_day`      | Set = 15-21  | Sau 1-2 tuần    |
| TimeSkip Critical   | `current_day`      | Add + 1      | Ngày quyết định |
| Scene 20 (đưa tiền) | `money`            | Sub - 500000 | Mất 500k        |
| Scene 20 (bị đánh)  | `fear_level`       | Add + 25     | Tăng sợ hãi     |
| Scene 24 (đưa tiền) | `money`            | Sub - 50000  | Mất 50k         |
| Scene 24 (bị đánh)  | `fear_level`       | Add + 30     | Tăng sợ hãi     |
| Khi bỏ chạy         | `escaped_count`    | Add + 1      | Đếm số lần chạy |
| Khi đưa tiền        | `gave_money_count` | Add + 1      | Đếm số lần đưa  |

---

## BẢNG THAM CHIẾU NHANH

### 🔄 FLOW CHÍNH VÀ FLAGS CẦN SET

```
NGÀY 1 (Day1)
├── Scene 1 (Bedroom) → [day1_started, day1_scene1_completed]
├── Scene 2 (Livingroom) → [day1_scene2_completed]
├── Scene 3 (Classroom) → [day1_scene3_completed]
├── Scene 4 (Street) → [day1_scene4_completed]
├── Scene 5 (Top-down) → Bullies spawn
├── Scene 6 (Noticed) → [day1_scene6_completed, met_bullies]
├── Scene 7A/7B → [day1_scene7_completed, choice_confronted/ran_away_day1]
├── Scene 8 (Home) → [day1_scene8_completed]
└── Scene 9 (Lên phòng) → TimeSkip "Trôi qua 1 tuần..."

TUẦN 1 (Week1)
├── Scene 10 (Classroom Break) → [week1_scene10_completed, invited_by_classmate]
├── Scene 11 (Bạn đi ra) → Top-down
├── Scene 12 (Teacher) → [week1_scene12_completed, talked_to_teacher]
├── Scene 13 (Street Followed) → Top-down
├── Scene 14A/14B → [week1_scene14_completed, befriended_bullies/choice_ran_away_week1]
├── Scene 15 (Home) → [week1_scene15_completed]
└── Scene 16 (Lên phòng) → TimeSkip "Ngày tiếp theo..."

NGÀY 2 (Day2)
├── Scene 17 (Classroom) → [day2_scene17_completed]
├── Scene 18 (Bạn đi ra) → Top-down
├── Scene 19 (Prepare) → [day2_scene19_completed]
├── Scene 20 (Street Bully) → [day2_scene20_completed, gave_money/got_beaten]
└── Scene 20 Home → [day2_scene21_completed, mom_noticed_something_wrong]
    └── TimeSkip "Trôi qua 1-2 tuần..."

PHASE 2 (Week3)
├── Scene 21 (Home Morning) → [week3_scene21_completed, saw_news_about_bullying]
├── Scene 22 (Door News) → [week3_scene22_completed, phase2_warning_shown]
├── Scene 23 (Classroom) → [week3_scene23_completed]
├── Scene 24 (Street Blocked) → [week3_scene24_completed, gave_money/got_beaten]
└── Scene 24 Home → TimeSkip "Hôm sau..."

CRITICAL DAY
├── Scene 25 (Home Morning) → [critical_scene25_completed, mom_worried]
├── Scene 26 (Classroom) → [critical_scene26_completed]
└── Scene 27 (Street Final) → [critical_scene27_completed]
    ├── Đánh trả thắng → Scene 28A → [stood_up_to_bullies, confessed_to_mom, ending1_good_standup]
    │   └── STORYTELLING Ending 1
    └── Bị đánh → Scene 28B (Lựa chọn cuối)
        ├── Thú nhận → [confessed_to_mom, ending2_true_tellparents]
        │   └── STORYTELLING Ending 2
        └── Giấu mẹ → [hid_from_mom, ending3_bad_darklife]
            └── STORYTELLING Ending 3
```

### ⚡ ĐIỀU KIỆN ENDING

| Ending               | Flags cần có                 | Flags không được có | Variables | Chế độ       |
| -------------------- | ---------------------------- | ------------------- | --------- | ------------ |
| **Good_StandUp**     | `stood_up_to_bullies`        | -                   | -         | Storytelling |
| **True_TellParents** | `confessed_to_mom`           | `hid_from_mom`      | -         | Storytelling |
| **Bad_DarkLife**     | `hid_from_mom`, `got_beaten` | `confessed_to_mom`  | -         | Storytelling |

### 📝 CHECKLIST TẠO SCENE MỚI

Khi tạo VNSceneData hoặc DialogueTrigger mới:

1. **Conditions**

   - [ ] `requiredFlags`: Flags cần có để trigger
   - [ ] `forbiddenFlags`: Flags không được có (tránh lặp)

2. **Effects**

   - [ ] `setFlagsOnComplete`: Flags set khi hoàn thành
   - [ ] `setFlagsFalse`: Flags cần reset
   - [ ] `variableChanges`: Thay đổi biến

3. **After Dialogue/Scene**
   - [ ] `nextVNScene`: VN scene tiếp theo
   - [ ] `nextDialogue`: Dialogue tiếp theo
   - [ ] `nextSceneName`: Unity scene để load
   - [ ] `nextSpawnPointId`: Spawn point trong scene đích

---

## GHI CHÚ

### Quy tắc quan trọng

1. **Tránh lặp scene**: Luôn thêm flag `[scene]_completed` vào `forbiddenFlags`
2. **Thứ tự scene**: Sử dụng `requiredFlags` để đảm bảo thứ tự đúng
3. **Biến tiền**: Luôn kiểm tra `money >= amount` trước khi trừ
4. **Fear level**: Tối đa 100, chỉ dùng để tracking tâm lý (không ảnh hưởng ending)
5. **Storytelling**: Sử dụng actionId để trigger storytelling sequence thay vì tạo nhiều scene

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
    public const string HID_FROM_MOM = "hid_from_mom";
    // Bỏ: BROUGHT_KNIFE (không còn sử dụng)
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

// Action IDs cho Storytelling
public static class ActionIds
{
    public const string TRIGGER_ENDING1_STORYTELLING = "trigger_ending1_storytelling";
    public const string TRIGGER_ENDING2_STORYTELLING = "trigger_ending2_storytelling";
    public const string TRIGGER_ENDING3_STORYTELLING = "trigger_ending3_storytelling";
}
```

---

_Tài liệu này được cập nhật dựa trên docs/story.md_
