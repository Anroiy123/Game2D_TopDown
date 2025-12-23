# 🎯 SCENE 27-28: CẤU TRÚC PHÂN NHÁNH ENDING

## 📋 TỔNG QUAN

Scene 27-28 là điểm quyết định cuối cùng của game, dẫn đến 3 ending khác nhau:

```
Scene 27: Gặp tụi bắt nạt (Critical Day)
├── Choice A: "Đối mặt 1v1" → FightCutscene → Scene 28A → ENDING 1 (Good_StandUp)
├── Choice B: "Chơi Tù xì" → Bị đánh → Scene 28B → ENDING 2/3
└── Choice C: "Từ chối" → Bị đánh → Scene 28B → ENDING 2/3

Scene 28B: Về nhà - Lựa chọn cuối cùng
├── Choice A: "Thú nhận với mẹ" → ENDING 2 (True_TellParents)
└── Choice B: "Giấu mẹ" → ENDING 3 (Bad_DarkLife)
```

---

## 📁 CÁC FILE JSON

### 1. **CriticalDay_Scene27_Street_NEW.json**
- **Mục đích**: Dialogue chính của Scene 27 - Gặp tụi bắt nạt
- **Nodes quan trọng**:
  - Node 0-30: Intro (Thủ lĩnh đòi chơi Tù xì)
  - Node 40: **LỰA CHỌN ĐỊNH MỆNH** (3 choices)
  - Node 100-130: Branch "Đối mặt 1v1" → Ending 1
  - Node 200-260: Branch "Chơi Tù xì" → Ending 2/3
  - Node 300-330: Branch "Từ chối" → Ending 2/3

### 2. **CriticalDay_Scene28A_Home_AfterWin.json**
- **Mục đích**: Dialogue sau khi thắng fight (Ending 1)
- **Nodes**: 0-30 (4 nodes ngắn gọn)
- **Kết thúc**: Trigger `trigger_ending1_storytelling`

### 3. **CriticalDay_Scene28B_Home_Choice.json**
- **Mục đích**: Lựa chọn cuối cùng (Ending 2/3)
- **Nodes quan trọng**:
  - Node 0: **LỰA CHỌN CUỐI CÙNG** (2 choices)
  - Node 100-120: Branch "Thú nhận" → Ending 2
  - Node 200-220: Branch "Giấu mẹ" → Ending 3

---

## 🚩 FLAGS VÀ ACTION IDS

### Flags được set trong Scene 27:
- `critical_scene27_completed` - Hoàn thành Scene 27
- `stood_up_to_bullies` - Đã đứng lên chống lại (Choice A)
- `got_beaten` - Bị đánh (Choice B/C)
- `played_game_and_lost` - Chơi Tù xì và thua (Choice B)
- `refused_game_and_beaten` - Từ chối chơi và bị đánh (Choice C)

### Flags được set trong Scene 28:
- `confessed_to_mom` - Thú nhận với mẹ
- `hid_from_mom` - Giấu mẹ
- `ending1_good_standup` - Trigger Ending 1
- `ending2_true_tellparents` - Trigger Ending 2
- `ending3_bad_darklife` - Trigger Ending 3

### Action IDs:
- `trigger_fight_cutscene` - Bắt đầu FightCutscene (Scene 27, Node 130)
- `trigger_scene28b` - Chuyển sang Scene 28B (Scene 27, Node 260/330)
- `trigger_ending1_storytelling` - Bắt đầu Storytelling Ending 1 (Scene 28A, Node 30)
- `trigger_ending2_storytelling` - Bắt đầu Storytelling Ending 2 (Scene 28B, Node 120)
- `trigger_ending3_storytelling` - Bắt đầu Storytelling Ending 3 (Scene 28B, Node 220)

---

## 🎬 FLOW DIAGRAM CHI TIẾT

```
[Scene 27 Start]
    ↓
[Node 0-30: Intro - Thủ lĩnh đòi chơi Tù xì]
    ↓
[Node 40: LỰA CHỌN ĐỊNH MỆNH]
    ├─→ [Choice A: Đối mặt 1v1]
    │       ↓
    │   [Node 100-130: Dialogue thách đấu]
    │       ↓
    │   [ActionID: trigger_fight_cutscene]
    │       ↓
    │   [FightCutscene: Đánh nhau]
    │       ↓
    │   [Scene 28A: Về nhà sau thắng]
    │       ↓
    │   [Node 0-30: Thú nhận với mẹ]
    │       ↓
    │   [ActionID: trigger_ending1_storytelling]
    │       ↓
    │   ✅ ENDING 1: Good_StandUp
    │
    ├─→ [Choice B: Chơi Tù xì]
    │       ↓
    │   [Node 200-260: Chơi và thua → Bị đánh]
    │       ↓
    │   [ActionID: trigger_scene28b]
    │       ↓
    │   [Scene 28B: Về nhà - Lựa chọn cuối]
    │       ↓
    │   [Node 0: LỰA CHỌN CUỐI CÙNG]
    │       ├─→ [Thú nhận] → ✅ ENDING 2: True_TellParents
    │       └─→ [Giấu mẹ] → ❌ ENDING 3: Bad_DarkLife
    │
    └─→ [Choice C: Từ chối]
            ↓
        [Node 300-330: Từ chối → Bị đánh]
            ↓
        [ActionID: trigger_scene28b]
            ↓
        [Scene 28B: Về nhà - Lựa chọn cuối]
            ↓
        [Node 0: LỰA CHỌN CUỐI CÙNG]
            ├─→ [Thú nhận] → ✅ ENDING 2: True_TellParents
            └─→ [Giấu mẹ] → ❌ ENDING 3: Bad_DarkLife
```

---

## 🔧 CÁCH SỬ DỤNG

### Bước 1: Import JSON vào Unity
1. Mở Unity Editor
2. Menu: `Tools → Dialogue → Import JSON to DialogueData`
3. Chọn 3 file JSON:
   - `CriticalDay_Scene27_Street_NEW.json`
   - `CriticalDay_Scene28A_Home_AfterWin.json`
   - `CriticalDay_Scene28B_Home_Choice.json`
4. Import → Tạo DialogueData assets

### Bước 2: Xử lý Action IDs trong DialogueSystem
Cần implement các action IDs sau trong `DialogueSystem.cs`:

```csharp
// Trong DialogueSystem.ProcessActionId()
case "trigger_fight_cutscene":
    // TODO: Trigger FightCutscene
    break;

case "trigger_scene28b":
    // TODO: Chuyển sang Scene 28B (HomeScene)
    break;

case "trigger_ending1_storytelling":
    // TODO: Bắt đầu Storytelling Ending 1
    break;

case "trigger_ending2_storytelling":
    // TODO: Bắt đầu Storytelling Ending 2
    break;

case "trigger_ending3_storytelling":
    // TODO: Bắt đầu Storytelling Ending 3
    break;
```

### Bước 3: Tạo FightCutscene
- Tạo scene hoặc VN sequence cho fight cutscene
- Sau khi kết thúc → Chuyển sang Scene 28A

---

## 📝 GHI CHÚ

- **Scene 27** có 3 lựa chọn nhưng chỉ dẫn đến 2 nhánh chính:
  - Nhánh 1: Đối mặt 1v1 → Ending 1
  - Nhánh 2: Bị đánh (Chơi Tù xì hoặc Từ chối) → Scene 28B → Ending 2/3

- **Scene 28B** là điểm quyết định cuối cùng giữa Ending 2 và Ending 3

- Tất cả 3 ending đều sử dụng **Storytelling mode** (không phải dialogue phức tạp)

