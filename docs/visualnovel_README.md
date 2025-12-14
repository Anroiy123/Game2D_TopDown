# 🎬 HỆ THỐNG VISUAL NOVEL - GAME THANH HÓA

## 📚 MỤC LỤC

1. [Giới thiệu](#giới-thiệu)
2. [Hướng dẫn nhanh](#hướng-dẫn-nhanh)
3. [API Reference](#api-reference)
4. [Tài liệu chi tiết](#tài-liệu-chi-tiết)

---

## 🎯 GIỚI THIỆU

Hệ thống Visual Novel cho phép game chuyển đổi giữa 2 chế độ:
- **Top-Down Mode**: Di chuyển tự do, tương tác với NPC
- **Visual Novel Mode**: Ảnh nền tĩnh + dialogue + nhân vật (giống Doki Doki Literature Club)

### Tính năng:
✅ Hiển thị background fullscreen  
✅ Character sprites với vị trí tùy chỉnh  
✅ Tích hợp DialogueSystem hiện có  
✅ Story flags & variables  
✅ Branching narrative với choices  
✅ Chuyển cảnh tự động  
✅ Fade transitions  
✅ Nhạc nền & sound effects  

---

## 🚀 HƯỚNG DẪN NHANH

### Bước 1: Tạo VN Scene bằng Editor Tool

```
Unity Editor → Menu → Tools → Visual Novel → Create VN Scene Quick Setup
```

Điền thông tin:
- **Scene Name**: Day1_Morning
- **Location Text**: Phòng ngủ Đức
- **Background Sprite**: Kéo ảnh vào đây
- ✅ **Tạo DialogueData mẫu**

Nhấn **"Tạo VN Scene"**

### Bước 2: Chỉnh sửa Dialogue

File được tạo: `Assets/Scripts/Data/VisualNovel/Day1_Morning_Dialogue.asset`

Mở trong Inspector và chỉnh sửa:
```yaml
Node 0:
  Speaker Name: "Narrator"
  Dialogue Lines:
    - "Ánh sáng ban mai chiếu vào phòng..."
  Next Node ID: 1

Node 1:
  Speaker Name: "Mẹ"
  Dialogue Lines:
    - "Đức ơi, dậy đi con!"
  Next Node ID: -1
```

### Bước 3: Trigger trong game

**Cách 1: Trigger tự động khi vào scene**
```
1. Tạo GameObject trong scene
2. Add Component: VN Trigger
3. VN Scene: [Kéo Day1_Morning_VNScene vào]
4. Mode: On Scene Start
```

**Cách 2: Trigger bằng code**
```csharp
public VNSceneData vnScene;

void Start()
{
    VisualNovelManager.Instance.StartVNScene(vnScene, OnComplete);
}

void OnComplete()
{
    Debug.Log("VN scene done!");
}
```

---

## 📖 API REFERENCE

### VisualNovelManager (Singleton)

```csharp
// Bắt đầu VN scene
VisualNovelManager.Instance.StartVNScene(VNSceneData sceneData, Action onComplete);

// Kết thúc VN mode
VisualNovelManager.Instance.EndVNMode();

// Kiểm tra VN mode có đang active không
bool isActive = VisualNovelManager.Instance.IsVNModeActive;
```

### VNSequenceManager (Static)

```csharp
// Chơi một sequence nhiều cảnh
VNSequenceManager.PlaySequence(VNSequenceData sequence, Action onComplete);
```

### VNTrigger (Component)

```csharp
// Trigger thủ công
vnTrigger.TriggerManually();

// Reset trigger để có thể chơi lại
vnTrigger.ResetTrigger();
```

---

## 📁 CẤU TRÚC DỮ LIỆU

### VNSceneData (ScriptableObject)

```
VNSceneData
├── Scene Data
│   ├── Scene Name: string
│   ├── Location Text: string (hiển thị trên màn hình)
│   ├── Background Image: Sprite
│   ├── Background Tint: Color
│   ├── Characters: VNCharacterDisplay[]
│   │   ├── Character Sprite
│   │   ├── Position (Left/Center/Right/FarLeft/FarRight/Custom)
│   │   ├── Scale, Flip X
│   ├── Dialogue: DialogueData
│   ├── BGM: AudioClip
│   ├── Ambience: AudioClip
│   ├── Next Scene: VNSceneData (null = kết thúc)
│   ├── Return To Top Down: bool
│   └── Top Down Scene Name, Spawn Point ID
├── Conditions
│   ├── Required Flags
│   └── Forbidden Flags
└── Effects On Enter
    ├── Set Flags On Enter
    └── Variable Changes On Enter
```

### DialogueData (ScriptableObject - Đã có sẵn)

```
DialogueData
├── Conversation Name
├── Start Node ID
└── Nodes: DialogueNode[]
    ├── Node ID
    ├── Speaker Name
    ├── Is Player Speaking
    ├── Dialogue Lines
    ├── Choices: DialogueChoice[]
    │   ├── Choice Text
    │   ├── Next Node ID
    │   ├── Required Flags, Forbidden Flags
    │   ├── Set Flags True/False
    │   └── Variable Changes
    └── Next Node ID (-1 = end)
```

---

## 🎮 VÍ DỤ SỬ DỤNG

### Ví dụ 1: Cảnh đơn giản (Narrator only)

```yaml
VNSceneData: "Intro"
  Location Text: ""
  Background: black screen
  Characters: (empty)
  Dialogue: 
    Node 0:
      Speaker: ""
      Lines: ["Năm 2024, thành phố Thanh Hóa..."]
      Next: -1
  Next Scene: null
  Return To Top Down: true
  Scene Name: "SchoolScene"
```

### Ví dụ 2: Cảnh có nhân vật

```yaml
VNSceneData: "Talk_With_Mom"
  Location Text: "Phòng khách"
  Background: livingroom_bg
  Characters:
    - Sprite: mom_sprite
      Position: Left
      Scale: (1, 1)
  Dialogue: Mom_Dialogue
  Next Scene: null
  Return To Top Down: true
```

### Ví dụ 3: Chuỗi cảnh (Sequence)

```yaml
VNSequenceData: "Day1_Complete"
  Scenes:
    - Day1_Scene1_Bedroom
    - Day1_Scene2_Kitchen
    - Day1_Scene3_School
  Day Number: 1
  Set Flags On Complete: ["day1_completed"]
```

```csharp
// Chơi toàn bộ Day 1
VNSequenceManager.PlaySequence(day1Sequence, () => {
    Debug.Log("Day 1 done!");
});
```

---

## 🎨 VỊ TRÍ NHÂN VẬT

```
         FarLeft      Left       Center      Right      FarRight
            |          |           |           |           |
          -600px     -400px       0px       +400px      +600px
```

**Custom Position**: Dùng Position Offset để điều chỉnh pixel-perfect

---

## 🔧 EDITOR TOOLS

### Tools → Visual Novel → Create VN Scene Quick Setup
- Tạo nhanh VNSceneData + DialogueData
- Tự động tạo folder `Assets/Scripts/Data/VisualNovel/`

---

## 🎯 SPECIAL ACTION IDS

Các action ID đặc biệt trong DialogueChoice:

| Action ID | Hiệu ứng |
|-----------|----------|
| `end_vn_mode` | Thoát VN mode ngay lập tức |
| `trigger_good_ending` | Set flag "stood_up_to_bullies" |
| `trigger_true_ending` | Set flag CONFESSED_TO_MOM |
| `trigger_bad_murder` | Set flag BROUGHT_KNIFE |

Sử dụng trong DialogueChoice:
```yaml
Choice:
  Choice Text: "Thú nhận với mẹ"
  Action ID: "trigger_true_ending"
  Next Node ID: 10
```

---

## 📊 LUỒNG HOẠT ĐỘNG

```
VNTrigger.TriggerManually() hoặc Auto trigger
              ↓
VisualNovelManager.StartVNScene(sceneData)
              ↓
Check conditions (flags, variables)
              ↓
Apply OnEnter effects (set flags, change vars)
              ↓
Fade out → Hide player → Show VN panel
              ↓
Display background + characters
              ↓
DialogueSystem.StartDialogueWithChoices()
              ↓
Player nhấn E để tiếp tục / chọn choices
              ↓
Dialogue kết thúc
              ↓
Check Next Scene?
  YES → Load next VN scene (loop)
  NO → EndVNMode()
              ↓
Fade out VN panel
              ↓
Return to Top Down? 
  YES → Load scene with spawn point
  NO → Show player lại
              ↓
Fade in → Done
```

---

## 📚 TÀI LIỆU CHI TIẾT

1. **[visualnovel_quickstart.md](./visualnovel_quickstart.md)**  
   Hướng dẫn tạo cảnh VN đầu tiên (từng bước chi tiết)

2. **[visualnovel_day1_complete.md](./visualnovel_day1_complete.md)**  
   Hướng dẫn tạo toàn bộ Day 1 với 5 cảnh + branching choices

3. **[CLAUDE.md](../CLAUDE.md)**  
   Project structure + conventions

---

## ❓ FAQ

**Q: Làm sao để thêm nhiều nhân vật trong 1 cảnh?**  
A: Trong VNSceneData → Characters → Tăng Array Size → Thêm Character Sprite + Position

**Q: Làm sao để link nhiều cảnh liên tiếp?**  
A: Set Next Scene trong VNSceneData, hoặc dùng VNSequenceData

**Q: Làm sao để chỉ chạy VN 1 lần duy nhất?**  
A: Dùng Forbidden Flags trong VNSceneData và Set Flags On Enter

**Q: Làm sao để có nhiều ending?**  
A: Dùng Choices trong DialogueData với Set Flags True + Required Flags

**Q: Có thể trigger VN từ NPC dialogue không?**  
A: Có! Trong NPCInteraction, gọi VisualNovelManager.Instance.StartVNScene()

---

## ✅ CHECKLIST TẠO VN SCENE

- [ ] Chuẩn bị background sprite
- [ ] Chuẩn bị character sprites (nếu có)
- [ ] Tạo DialogueData với nodes
- [ ] Tạo VNSceneData
- [ ] Link Dialogue vào VNSceneData
- [ ] Set conditions (flags) nếu cần
- [ ] Set effects (set flags, change vars)
- [ ] Tạo VNTrigger trong scene hoặc code
- [ ] Test trong Unity Editor

---

✨ **Chúc bạn tạo game thành công!**

