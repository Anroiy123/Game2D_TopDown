# 📋 KẾ HOẠCH PHÁT TRIỂN GAME: "GAME THANH HÓA"

## 📊 TỔNG QUAN PHÂN TÍCH

### Cốt truyện chính:

- **Nhân vật chính**: Đức - học sinh chuyển trường, nhút nhát, Wibu, dễ bị bắt nạt
- **Chủ đề**: Bạo lực học đường
- **3 Giai đoạn**: Những ngày đầu → Leo thang → Cao trào & Kết thúc
- **4 Endings**: 1 Good, 1 True, 2 Bad

### Tài nguyên hiện có:

| Loại                  | Có sẵn      | Ghi chú                                                 |
| --------------------- | ----------- | ------------------------------------------------------- |
| **Character sprites** | ✅ 4 bộ     | Alex, Amelia, Bob, Adam (16x16)                         |
| **Classroom assets**  | ✅ Đầy đủ   | Lớp học, phòng máy, cafeteria, toilet, Principal Office |
| **Home assets**       | ⚠️ Một phần | `Interiors_free_16x16`, `Room_Builder_free_16x16`       |
| **Street assets**     | ❌ Chưa có  | Cần tìm thêm                                            |

---

## 🗺️ KẾ HOẠCH MAP (3 BỐI CẢNH)

### **MAP 1: NHÀ ĐỨC (HomeScene)**

```
┌─────────────────────────────────────────────────────────────────┐
│  📍 HomeScene - Nhà của Đức                                     │
├─────────────────────────────────────────────────────────────────┤
│  Phòng:                                                         │
│  ├── 🛏️ Phòng ngủ Đức (Sáng sớm - scene mở đầu game)           │
│  │   ├── Giường, bàn học, máy tính (cày game)                  │
│  │   └── Poster anime/wibu                                      │
│  ├── 🏠 Phòng khách (Nơi mẹ đứng đợi)                          │
│  │   ├── Tivi (hiển thị tin tức bạo lực học đường)             │
│  │   └── Cửa ra đường (Door → StreetScene)                     │
│  └── 🍳 Bếp (Optional)                                          │
│                                                                 │
│  NPCs:                                                          │
│  ├── 👩 Mẹ Đức (hỏi thăm, nhắc nhở)                            │
│  └── 👨 Bố Đức (xuất hiện trong True Ending)                   │
│                                                                 │
│  Assets sử dụng:                                                │
│  ├── Room_Builder_free_16x16.png (nền, tường)                  │
│  ├── Interiors_free_16x16.png (nội thất)                       │
│  └── InteriorTilesLITE.png (bổ sung)                           │
└─────────────────────────────────────────────────────────────────┘
```

### **MAP 2: LỚP HỌC (ClassroomScene) - ĐÃ CÓ**

```
┌─────────────────────────────────────────────────────────────────┐
│  📍 ClassroomScene - Trường học                                 │
├─────────────────────────────────────────────────────────────────┤
│  Khu vực:                                                       │
│  ├── 🏫 Lớp học chính (hiện có)                                │
│  │   ├── Bàn ghế, bảng, cô giáo                                │
│  │   └── Bạn kế bàn (rủ đi chơi)                               │
│  ├── 🚪 Phòng giáo viên (Principal Office assets)              │
│  │   └── Cô giáo nói chuyện riêng với Đức                      │
│  ├── 🏃 Sân trường (Optional - nơi bạn bè rủ chơi bóng)        │
│  └── 🚻 Toilet (Optional - nơi bị chặn)                        │
│                                                                 │
│  NPCs:                                                          │
│  ├── 👩‍🏫 Cô giáo (chủ nhiệm, khuyên nhủ)                       │
│  ├── 🧑 Bạn kế bàn (thân thiện, rủ đi chơi)                    │
│  └── 👥 Các bạn khác (background)                              │
│                                                                 │
│  Assets sử dụng:                                                │
│  ├── 2dClassroomAssetPackByStyloo/Classroom/                   │
│  ├── 2dClassroomAssetPackByStyloo/Principal Office/            │
│  └── 2dClassroomAssetPackByStyloo/Toilet/                      │
└─────────────────────────────────────────────────────────────────┘
```

### **MAP 3: ĐƯỜNG VỀ NHÀ (StreetScene)**

```
┌─────────────────────────────────────────────────────────────────┐
│  📍 StreetScene - Đường đi học về                               │
├─────────────────────────────────────────────────────────────────┤
│  Khu vực:                                                       │
│  ├── 🛣️ Con đường chính (dài, horizontal scrolling)           │
│  │   ├── Điểm A: Cổng trường (Door từ ClassroomScene)          │
│  │   ├── Điểm B: Nơi bắt gặp nhóm bắt nạt (BullyEncounter Zone)│
│  │   └── Điểm C: Cổng nhà (Door → HomeScene)                   │
│  └── 🌳 Background: Cây cối, nhà dân, đèn đường                │
│                                                                 │
│  NPCs:                                                          │
│  ├── 😈 Thằng cầm đầu (L) - Boss của nhóm bắt nạt              │
│  ├── 👿 Đàn em 1 (chuyên dọa nạt)                              │
│  └── 👿 Đàn em 2 (phụ họa)                                     │
│                                                                 │
│  Assets cần tìm:                                                │
│  ⚠️ Cần download thêm street/outdoor tileset                   │
│  Gợi ý: "Modern City Tileset" hoặc tự vẽ đơn giản              │
└─────────────────────────────────────────────────────────────────┘
```

---

## 👤 KẾ HOẠCH NHÂN VẬT

### **DANH SÁCH NHÂN VẬT CẦN TẠO**

| #   | Tên                | Vai trò                   | Sprite gợi ý                           | Animations cần           |
| --- | ------------------ | ------------------------- | -------------------------------------- | ------------------------ |
| 1   | **Đức** (Player)   | Nhân vật chính, nhút nhát | `prototype_character` (customize)      | idle, walk, sit, bị đánh |
| 2   | **Mẹ Đức**         | NPC nhà                   | `Amelia`                               | idle, walk               |
| 3   | **Bố Đức**         | NPC nhà (True Ending)     | `Bob`                                  | idle                     |
| 4   | **Cô giáo**        | NPC trường                | Cần tạo mới (nữ, trang phục giáo viên) | idle, walk               |
| 5   | **Bạn kế bàn**     | NPC bạn bè, thân thiện    | `Adam` (đã có)                         | idle, walk, sit          |
| 6   | **Tk cầm đầu (L)** | Antagonist chính          | `prototype_character_red`              | idle, walk, đánh         |
| 7   | **Đàn em 1**       | Villain phụ               | Cần sprite mới                         | idle, walk               |
| 8   | **Đàn em 2**       | Villain phụ               | Cần sprite mới                         | idle                     |

### **MAPPING SPRITE HIỆN CÓ**

```
Sprites/Characters/
├── Alex/         → Có thể dùng cho: Đức (Player)
├── Amelia/       → Có thể dùng cho: Mẹ Đức, Cô giáo
├── Bob/          → Có thể dùng cho: Bố Đức
├── adam/         → Đã dùng: Bạn kế bàn
├── prototype_character.png       → Backup cho Player
├── prototype_character_red.png   → Tk cầm đầu (L)
└── prototype_character_blue.png  → Đàn em
```

---

## 💻 KẾ HOẠCH SYSTEM

### **PRIORITY 1: Core Systems (BẮT BUỘC)** ✅ HOÀN THÀNH

| #   | System                    | Mô tả                                       | Files                                          | Status |
| --- | ------------------------- | ------------------------------------------- | ---------------------------------------------- | ------ |
| 1   | **GameManager**           | Singleton quản lý game state, scene loading | `GameManager.cs`                               | ✅     |
| 2   | **StoryManager**          | Quản lý story flags, variables              | `StoryManager.cs`, `SerializableDictionary.cs` | ✅     |
| 3   | **SceneTransition**       | Door/Portal chuyển scene                    | `SceneTransition.cs`, `SpawnPoint.cs`          | ✅     |
| 4   | **Enhanced DialogueData** | Thêm conditions, portrait                   | `DialogueData.cs` (đã nâng cấp)                | ✅     |
| 5   | **Save/Load System**      | Lưu tiến trình                              | `SaveManager.cs`, `SaveData.cs`                | ✅     |

### **PRIORITY 2: Gameplay Systems (QUAN TRỌNG)**

| #   | System             | Mô tả                           | Files cần tạo                  |
| --- | ------------------ | ------------------------------- | ------------------------------ |
| 6   | **DaySystem**      | Quản lý ngày trong game         | `DayManager.cs`                |
| 7   | **ChoiceTracker**  | Đếm lựa chọn để xác định ending | Tích hợp vào `StoryManager.cs` |
| 8   | **BullyEncounter** | Logic gặp nhóm bắt nạt          | `BullyEncounterZone.cs`        |
| 9   | **MoneySystem**    | Tiền của Đức (bị trấn lột)      | `PlayerInventory.cs`           |

### **PRIORITY 3: Polish Systems (NÊN CÓ)**

| #   | System               | Mô tả                    | Files cần tạo           |
| --- | -------------------- | ------------------------ | ----------------------- |
| 10  | **TVNewsDisplay**    | Hiển thị tin tức trên TV | `TVController.cs`       |
| 11  | **FadeTransition**   | Hiệu ứng chuyển scene    | `SceneFader.cs`         |
| 12  | **TextNotification** | Hiện thông điệp game     | `NotificationSystem.cs` |

---

## 📦 CẤU TRÚC STORY FLAGS

```
StoryManager.storyFlags:
├── day_1_completed: bool           ← Hoàn thành ngày 1
├── day_2_completed: bool           ← Hoàn thành tuần 1
├── met_bullies: bool               ← Đã gặp nhóm bắt nạt
├── befriended_bullies: bool        ← Đã "kết bạn" với nhóm bắt nạt (lần 1 đối mặt)
├── escaped_bullies_count: int      ← Số lần bỏ chạy (0-3)
├── got_beaten: bool                ← Đã bị đánh
├── gave_money_count: int           ← Số lần đưa tiền
├── talked_to_teacher: bool         ← Đã nói chuyện với cô giáo
├── invited_by_classmate: bool      ← Được bạn rủ đi chơi
├── rejected_classmate: bool        ← Từ chối bạn
├── mom_worried: bool               ← Mẹ lo lắng
├── confessed_to_mom: bool          ← Thú nhận với mẹ (True Ending)
├── brought_knife: bool             ← Mang dao (Bad Ending trigger)
└── current_ending: string          ← "good"/"true"/"bad_murder"/"bad_death"

StoryManager.storyVariables:
├── current_day: int                ← Ngày hiện tại (1-14)
├── money: int                      ← Tiền còn lại
├── fear_level: int                 ← Mức độ sợ hãi (0-100)
└── relationship_classmate: int     ← Quan hệ với bạn kế bàn
```

---

## 🎮 FLOW GAME THEO CỐT TRUYỆN

```
┌────────────────────────────────────────────────────────────────────────────┐
│  NGÀY 1: Ngày đầu đi học                                                   │
├────────────────────────────────────────────────────────────────────────────┤
│  HomeScene (Sáng)                                                          │
│  └→ Mẹ đánh thức → Đức ăn sáng → Đi học                                   │
│       ↓                                                                    │
│  ClassroomScene                                                            │
│  └→ Cô giáo giới thiệu → Đức tự giới thiệu (nhút nhát) → Bạn kế bàn chào  │
│       ↓                                                                    │
│  StreetScene (Chiều)                                                       │
│  └→ Đức đi về → Gặp nhóm bắt nạt đi theo                                  │
│       ↓                                                                    │
│  🔴 CHOICE 1: [Quay lại hỏi] / [Chạy về nhà]                              │
│       ↓                                                                    │
│  HomeScene (Tối)                                                           │
│  └→ Mẹ hỏi thăm → Đức phớt lờ lên phòng                                   │
└────────────────────────────────────────────────────────────────────────────┘
         ↓ (1 tuần trôi qua - Fade to black)
┌────────────────────────────────────────────────────────────────────────────┐
│  NGÀY 7: Một tuần sau                                                      │
├────────────────────────────────────────────────────────────────────────────┤
│  ClassroomScene                                                            │
│  └→ Bạn kế bàn rủ đi chơi → Đức từ chối                                   │
│       ↓                                                                    │
│  ClassroomScene (Phòng giáo viên)                                          │
│  └→ Cô giáo gọi riêng, khuyên nhủ kết bạn                                 │
│       ↓                                                                    │
│  StreetScene (Chiều)                                                       │
│  └→ Nhóm bắt nạt lại xuất hiện                                            │
│       ↓                                                                    │
│  🔴 CHOICE 2: [Đối mặt - "kết bạn"] / [Bỏ chạy lần 2]                     │
└────────────────────────────────────────────────────────────────────────────┘
         ↓
┌────────────────────────────────────────────────────────────────────────────┐
│  NGÀY 8+: Bắt đầu trấn lột                                                 │
├────────────────────────────────────────────────────────────────────────────┤
│  StreetScene                                                               │
│  └→ Nhóm bắt nạt đòi tiền                                                 │
│       ↓                                                                    │
│  🔴 CHOICE 3: [Đưa tiền] / [Từ chối] (→ bị đánh)                          │
│       ↓                                                                    │
│  (Vòng lặp 2 ngày/lần trong 1-2 tuần)                                     │
└────────────────────────────────────────────────────────────────────────────┘
         ↓
┌────────────────────────────────────────────────────────────────────────────┐
│  NGÀY CUỐI: Cao trào                                                       │
├────────────────────────────────────────────────────────────────────────────┤
│  HomeScene (Sáng)                                                          │
│  └→ Mẹ: "Chiều về sớm mẹ có chuyện muốn nói"                              │
│       ↓                                                                    │
│  StreetScene (Chiều)                                                       │
│  └→ Đức đưa tiền nhưng bị ép chơi "Tù xì bạt tai"                         │
│       ↓                                                                    │
│  🔴 ENDING BRANCH:                                                         │
│  ├→ [Bật lại, solo 1-1] → END 1: TỰ ĐỨNG LÊN (Good)                       │
│  ├→ [Về nhà, thú nhận] → END 2: NHỜ NGƯỜI LỚN (True)                      │
│  └→ [Giấu mẹ]                                                              │
│       ├→ [Mang dao] → END 3: ÁN MẠNG (Bad)                                │
│       └→ [Không mang] → END 4: TỬ VONG (Bad)                              │
└────────────────────────────────────────────────────────────────────────────┘
```

---

## ✅ TIẾN ĐỘ THỰC HIỆN

| #      | Task                               | Mô tả                        | Status      |
| ------ | ---------------------------------- | ---------------------------- | ----------- |
| **1**  | **Tạo GameManager + StoryManager** | Core systems quản lý game    | ✅ XONG     |
| **2**  | **Tạo HomeScene**                  | Map nhà Đức + NPCs           | 🔄 ĐANG LÀM |
| **3**  | **Tạo StreetScene**                | Map đường về + nhóm bắt nạt  | ⏳ CHỜ      |
| **4**  | **Tạo Scene Transition System**    | Door, SpawnPoint             | ✅ XONG     |
| **5**  | **Nâng cấp DialogueData**          | Thêm conditions, story flags | ✅ XONG     |
| **6**  | **Tạo nhân vật mới**               | Mẹ, Cô giáo, Nhóm bắt nạt    | ⏳ CHỜ      |
| **7**  | **Tạo Day System**                 | Quản lý thời gian trong game | ⏳ CHỜ      |
| **8**  | **Implement Ending Logic**         | 4 endings dựa trên choices   | ⏳ CHỜ      |
| **9**  | **Tìm/Download Street Assets**     | Tileset cho đường phố        | ⏳ CHỜ      |
| **10** | **Full implementation (1-8)**      | Làm tất cả theo thứ tự       | 🔄 ĐANG LÀM |

---

## 📚 HƯỚNG DẪN SỬ DỤNG CORE SYSTEMS

### 1. GameManager

```csharp
// Load scene với spawn point cụ thể
GameManager.Instance.LoadScene("HomeScene", "from_street");

// Kiểm tra scene hiện tại
string currentScene = GameManager.Instance.GetCurrentSceneName();

// Pause/Resume game
GameManager.Instance.PauseGame();
GameManager.Instance.ResumeGame();
```

### 2. StoryManager

```csharp
// Set/Get story flags
StoryManager.Instance.SetFlag("met_bullies", true);
bool hasMet = StoryManager.Instance.GetFlag("met_bullies");

// Set/Get variables
StoryManager.Instance.SetVariable("money", 50000);
StoryManager.Instance.AddVariable("fear_level", 10);
int money = StoryManager.Instance.GetMoney();

// Day system
StoryManager.Instance.AdvanceDay(1);
int day = StoryManager.Instance.GetCurrentDay();

// Determine ending
StoryManager.EndingType ending = StoryManager.Instance.DetermineEnding();
```

### 3. SceneTransition (Door/Portal)

1. Tạo GameObject với Collider2D (Is Trigger = true)
2. Add component `SceneTransition`
3. Cấu hình:
   - `Target Scene Name`: Tên scene đích
   - `Target Spawn Point Id`: ID spawn point
   - `Mode`: OnTriggerEnter hoặc OnInteract (nhấn E)
   - `Required Flags`: Flags cần có để chuyển scene
   - `Forbidden Flags`: Flags cấm (không được có)

### 4. SpawnPoint

1. Tạo Empty GameObject tại vị trí spawn
2. Add component `SpawnPoint`
3. Cấu hình:

   - `Spawn Point Id`: ID duy nhất (VD: "from_home", "from_school")
   - `Is Default Spawn`: Đánh dấu là spawn mặc định
   - `Facing Direction`: Hướng player nhìn khi spawn
   - `From Scene Name`: Scene nguồn (optional)

4. Add component `SpawnManager` vào 1 GameObject trong scene

### 5. DialogueData với Conditions

```csharp
// Trong DialogueChoice:
requiredFlags = new string[] { "met_bullies" };     // Cần có flag này mới hiện choice
forbiddenFlags = new string[] { "got_beaten" };    // Nếu có flag này thì ẩn choice

// Variable conditions
variableConditions = new VariableCondition[] {
    new VariableCondition { variableName = "money", comparison = GreaterOrEqual, value = 10000 }
};

// Effects khi chọn
setFlagsTrue = new string[] { "gave_money" };
variableChanges = new VariableChange[] {
    new VariableChange { variableName = "money", operation = Subtract, value = 10000 }
};
```

### 6. SaveManager

```csharp
// Lưu game
SaveManager.Instance.SaveGame();

// Load game
if (SaveManager.Instance.HasSaveFile())
{
    SaveManager.Instance.LoadGame();
}

// Xóa save
SaveManager.Instance.DeleteSave();
```

---

## 📁 CẤU TRÚC FILES ĐÃ TẠO

```
Assets/Scripts/
├── GameManager.cs          ← Singleton quản lý game state
├── StoryManager.cs         ← Quản lý story flags/variables
├── SerializableDictionary.cs ← Dictionary serialize được
├── SceneTransition.cs      ← Component Door/Portal
├── SpawnPoint.cs           ← SpawnPoint + SpawnManager
├── DialogueData.cs         ← Đã nâng cấp với conditions
├── SaveManager.cs          ← Lưu/Load game
└── SaveData.cs             ← Cấu trúc data lưu game
```
