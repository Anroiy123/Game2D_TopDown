# 🥊 HƯỚNG DẪN SETUP FIGHT CUTSCENE TRONG UNITY

## 📋 CHECKLIST

### ✅ Đã có:
- [x] 3 backgrounds: `fightScene1.png`, `fightScene2.png`, `fightScene3.png`
- [x] 3 JSON files cho dialogue

### 🔲 Cần làm:
- [ ] Import 3 JSON files
- [ ] Tạo 3 VNSceneData assets
- [ ] Tạo 1 VNSequenceData asset
- [ ] Link backgrounds vào VNSceneData
- [ ] Link DialogueData vào VNSceneData
- [ ] Test FightCutscene

---

## 🔧 BƯỚC 1: IMPORT JSON FILES

1. Mở Unity Editor
2. Menu: `Tools → Dialogue → Import JSON to DialogueData`
3. Chọn 3 files:
   - `FightCutscene_Scene1_Confrontation.json`
   - `FightCutscene_Scene2_Fighting.json`
   - `FightCutscene_Scene3_Victory.json`
4. Click **Import**

**Kết quả**: 3 DialogueData assets được tạo trong `Assets/Data/Dialogues/CriticalDay/`

---

## 🔧 BƯỚC 2: TẠO VNSceneData (3 ASSETS)

### Scene 1: Confrontation (Đối mặt)

1. Right-click trong Project → `Visual Novel → VN Scene Data`
2. Đặt tên: `FightScene1_Confrontation`
3. Cấu hình:

```
Scene Data:
├── Scene Name: "Fight - Confrontation"
├── Location Text: "Đường về nhà - Chiều muộn"
├── Background Image: fightScene1.png (kéo từ Assets/BackGround&Avatar/background/)
├── Background Tint: White (255, 255, 255, 255)
├── Characters: [] (để trống - không có character sprites)
├── Dialogue: FightCutscene_Scene1_Confrontation_Dialogue (kéo từ Data/Dialogues/CriticalDay/)
├── BGM: (Chọn nhạc căng thẳng nếu có)
├── Ambience: (Để trống)
├── Next Scene: FightScene2_Fighting (sẽ link sau)
├── Return To TopDown: false
```

---

### Scene 2: Fighting (Đánh nhau)

1. Right-click → `Visual Novel → VN Scene Data`
2. Đặt tên: `FightScene2_Fighting`
3. Cấu hình:

```
Scene Data:
├── Scene Name: "Fight - Fighting"
├── Location Text: "" (để trống)
├── Background Image: fightScene2.png
├── Background Tint: White
├── Characters: []
├── Dialogue: FightCutscene_Scene2_Fighting_Dialogue
├── BGM: (Giữ nguyên từ Scene 1)
├── Next Scene: FightScene3_Victory
├── Return To TopDown: false
```

---

### Scene 3: Victory (Thắng lợi)

1. Right-click → `Visual Novel → VN Scene Data`
2. Đặt tên: `FightScene3_Victory`
3. Cấu hình:

```
Scene Data:
├── Scene Name: "Fight - Victory"
├── Location Text: "" (để trống)
├── Background Image: fightScene3.png
├── Background Tint: White
├── Characters: []
├── Dialogue: FightCutscene_Scene3_Victory_Dialogue
├── BGM: (Có thể đổi sang nhạc chiến thắng)
├── Next Scene: null (để trống)
├── Return To TopDown: true ✅
├── TopDown Scene Name: "HomeScene" ✅
├── Spawn Point Id: "after_fight" ✅
```

**⚠️ QUAN TRỌNG**: Scene 3 phải set `Return To TopDown = true` để chuyển về HomeScene!

---

## 🔧 BƯỚC 3: LINK NEXT SCENES

Quay lại các VNSceneData và link:

1. **FightScene1_Confrontation**:
   - `Next Scene` → Kéo `FightScene2_Fighting` vào

2. **FightScene2_Fighting**:
   - `Next Scene` → Kéo `FightScene3_Victory` vào

3. **FightScene3_Victory**:
   - `Next Scene` → Để trống (null)

---

## 🔧 BƯỚC 4: TẠO VNSequenceData

1. Right-click → `Visual Novel → VN Sequence`
2. Đặt tên: `FightCutscene`
3. Cấu hình:

```
VNSequenceData:
├── Sequence Name: "Fight Cutscene"
├── Day Number: 8 (Critical Day)
├── Time Of Day: Afternoon
├── Scenes: (Size = 3)
│   ├── [0] FightScene1_Confrontation
│   ├── [1] FightScene2_Fighting
│   └── [2] FightScene3_Victory
├── On Complete Effects:
│   ├── Set Flags On Complete: [] (để trống - đã set trong dialogue)
│   └── Variable Changes On Complete: [] (để trống)
```

---

## 🔧 BƯỚC 5: TẠO SPAWN POINT "after_fight" TRONG HOMESCENE

1. Mở scene `HomeScene`
2. Tạo Empty GameObject: `SpawnPoint_AfterFight`
3. Add component: `SpawnPoint`
4. Cấu hình:

```
SpawnPoint:
├── Spawn Point Id: "after_fight"
├── Is Default Spawn: false
├── Facing Direction: Down (hoặc Up tùy layout)
```

5. Đặt vị trí: Gần cửa nhà (vị trí Đức về sau khi đánh nhau)

---

## 🔧 BƯỚC 6: UPDATE DIALOGUESYSTEM.CS

Thêm action ID handler cho `trigger_fight_cutscene`:

```csharp
// Trong DialogueSystem.ProcessActionId()
case "trigger_fight_cutscene":
    StartFightCutscene();
    break;

// Thêm method mới
private void StartFightCutscene()
{
    // Load VNSequenceData
    VNSequenceData fightSequence = Resources.Load<VNSequenceData>("VNSequences/FightCutscene");
    
    if (fightSequence != null)
    {
        VisualNovelManager.Instance.StartVNSequence(fightSequence, () =>
        {
            Debug.Log("Fight Cutscene completed!");
            // Sequence tự động chuyển về HomeScene nhờ returnToTopDown
        });
    }
    else
    {
        Debug.LogError("FightCutscene VNSequenceData not found!");
    }
}
```

**⚠️ LƯU Ý**: VNSequenceData phải nằm trong thư mục `Resources/VNSequences/` để `Resources.Load()` hoạt động!

---

## 📁 CẤU TRÚC THƯ MỤC CUỐI CÙNG

```
Assets/
├── BackGround&Avatar/
│   └── background/
│       ├── fightScene1.png ✅
│       ├── fightScene2.png ✅
│       └── fightScene3.png ✅
│
├── Data/
│   ├── Dialogues/
│   │   └── CriticalDay/
│   │       ├── FightCutscene_Scene1_Confrontation.json ✅
│   │       ├── FightCutscene_Scene1_Confrontation_Dialogue.asset (auto-generated)
│   │       ├── FightCutscene_Scene2_Fighting.json ✅
│   │       ├── FightCutscene_Scene2_Fighting_Dialogue.asset (auto-generated)
│   │       ├── FightCutscene_Scene3_Victory.json ✅
│   │       └── FightCutscene_Scene3_Victory_Dialogue.asset (auto-generated)
│   │
│   └── VisualNovel/
│       ├── Scenes/
│       │   ├── FightScene1_Confrontation.asset (TODO)
│       │   ├── FightScene2_Fighting.asset (TODO)
│       │   └── FightScene3_Victory.asset (TODO)
│       │
│       └── Sequences/ (hoặc Resources/VNSequences/)
│           └── FightCutscene.asset (TODO)
```

---

## 🧪 BƯỚC 7: TEST

### Test riêng FightCutscene:

1. Tạo test scene hoặc dùng existing scene
2. Tạo button hoặc trigger để gọi:
   ```csharp
   VNSequenceData fight = Resources.Load<VNSequenceData>("VNSequences/FightCutscene");
   VisualNovelManager.Instance.StartVNSequence(fight);
   ```
3. Play và kiểm tra:
   - ✅ 3 backgrounds hiển thị đúng thứ tự
   - ✅ Text dialogue hiển thị đúng
   - ✅ Chuyển scene về HomeScene sau khi kết thúc
   - ✅ Player spawn đúng vị trí "after_fight"

### Test toàn bộ flow từ Scene 27:

1. Play Scene 27
2. Chọn "Đối mặt 1v1"
3. Kiểm tra FightCutscene trigger
4. Kiểm tra chuyển về HomeScene
5. Kiểm tra Scene 28A dialogue

---

## 🎯 TROUBLESHOOTING

### Lỗi: "VNSequenceData not found"
- **Nguyên nhân**: File không nằm trong `Resources/` folder
- **Giải pháp**: Di chuyển `FightCutscene.asset` vào `Assets/Resources/VNSequences/`

### Lỗi: Background không hiển thị
- **Nguyên nhân**: Sprite import settings sai
- **Giải pháp**: 
  1. Select background image
  2. Inspector → Texture Type: `Sprite (2D and UI)`
  3. Apply

### Lỗi: Không chuyển về HomeScene
- **Nguyên nhân**: `returnToTopDown` chưa set hoặc spawn point không tồn tại
- **Giải pháp**:
  1. Kiểm tra `FightScene3_Victory.returnToTopDown = true`
  2. Kiểm tra spawn point "after_fight" tồn tại trong HomeScene

---

## ⏱️ THỜI GIAN ƯỚC TÍNH

- Import JSON: 2 phút
- Tạo 3 VNSceneData: 10 phút
- Tạo VNSequenceData: 3 phút
- Tạo spawn point: 2 phút
- Update DialogueSystem: 5 phút
- Test: 5 phút

**Tổng**: ~30 phút

