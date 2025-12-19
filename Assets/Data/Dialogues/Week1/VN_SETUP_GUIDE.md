# 🎬 HƯỚNG DẪN SETUP VN SCENE - SCENE 13 & 14

**Mục tiêu:** Tạo VNSceneData assets cho Scene 13, 14A, và 14A-2

---

## 📋 CHUẨN BỊ

### **Bước 0: Import JSON Files**

1. Mở Unity Editor
2. `Tools → Dialogue → Import JSON to DialogueData`
3. Chọn file: `Week1_Scene13_Street_Followed.json`
4. Click **Import** → Tạo asset tại `Assets/Data/Dialogues/Week1/`
5. Lặp lại với 2 file còn lại:
   - `Week1_Scene14A_SecondEncounter.json`
   - `Week1_Scene14A2_Forced_Friend.json`

✅ **Kết quả:** 3 DialogueData assets đã được tạo

---

## 🎬 SCENE 13: Street Followed (Optional VN)

**Lưu ý:** Scene 13 chủ yếu là top-down, VN chỉ dùng cho intro ngắn.

### **Cách 1: Không cần VNSceneData (Recommended)**
- Dùng VNTrigger với mode `OnSceneStart`
- Trigger dialogue ngắn rồi chuyển về top-down ngay

### **Cách 2: Tạo VNSceneData (Optional)**

**Nếu muốn tạo:**

1. **Right-click** trong `Assets/Data/VisualNovel/Week1/`
2. `Create → Visual Novel → VN Scene Data`
3. Đặt tên: `Week1_Scene13_Street_Followed_VNScene`

**Cấu hình:**

```yaml
═══════════════════════════════════════════════════════
SCENE DATA
═══════════════════════════════════════════════════════

Scene Name: Week1_Scene13_Street_Followed
Location Text: Đường về nhà - Chiều tà

Background Image: [Để trống hoặc dùng black screen]
Background Tint: (1, 1, 1, 1)

Characters: Size = 0  [Không có character, chỉ narrator]

Dialogue: Week1_Scene13_Street_Followed_Dialogue  [Kéo asset vào]

BGM: [Để trống hoặc dùng nhạc căng thẳng]
Ambience: [Để trống]

Next Scene: None

Return To Top Down: ☑ true
Top Down Scene Name: StreetScene
Spawn Point Id: [Để trống - giữ nguyên vị trí]

═══════════════════════════════════════════════════════
CONDITIONS (Để trống)
═══════════════════════════════════════════════════════

Required Flags: Size = 0
Forbidden Flags: Size = 0

═══════════════════════════════════════════════════════
EFFECTS ON ENTER (Không cần)
═══════════════════════════════════════════════════════

Set Flags On Enter: Size = 0
Variable Changes On Enter: Size = 0
```

---

## 🎬 SCENE 14A: Second Encounter (QUAN TRỌNG)

### **Bước 1: Tạo VNSceneData**

1. **Right-click** trong `Assets/Data/VisualNovel/Week1/`
2. `Create → Visual Novel → VN Scene Data`
3. Đặt tên: `Week1_Scene14A_SecondEncounter_VNScene`

### **Bước 2: Cấu hình Scene Data**

```yaml
═══════════════════════════════════════════════════════
SCENE DATA
═══════════════════════════════════════════════════════

Scene Name: Week1_Scene14A_SecondEncounter
Location Text: Đường phố - Gặp lại tụi bắt nạt

Background Image: [Kéo street background sprite vào]
  📁 Tìm tại: Assets/Sprites/Backgrounds/street.png
  ⚠️ Nếu chưa có: Dùng background tạm hoặc màu xám

Background Tint: (1, 1, 1, 1)  [Màu trắng bình thường]

═══════════════════════════════════════════════════════
CHARACTERS
═══════════════════════════════════════════════════════

Characters: Size = 3

Element 0 (Thủ lĩnh - Center):
  Character Sprite: [Kéo Bully leader sprite]
    📁 Assets/Sprites/Characters/Bully/Bully_idle.png
  Character Name: Thủ lĩnh
  Position: Center
  Position Offset: (0, -50)
  Scale: (1, 1)
  Flip X: ☐ false

Element 1 (Đàn em - Left):
  Character Sprite: [Kéo Bully minion sprite]
    📁 Assets/Sprites/Characters/DanEm1/DanEm_idle.png
  Character Name: Đàn em
  Position: Left
  Position Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip X: ☐ false

Element 2 (Đàn em - Right):
  Character Sprite: [Kéo Bully minion sprite]
    📁 Assets/Sprites/Characters/DanEm2/DanEm_idle.png
  Character Name: Đàn em
  Position: Right
  Position Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip X: ☑ true  [Flip để nhìn vào giữa]

═══════════════════════════════════════════════════════
DIALOGUE & AUDIO
═══════════════════════════════════════════════════════

Dialogue: Week1_Scene14A_SecondEncounter_Dialogue
  [Kéo DialogueData asset vào]

BGM: [Để trống hoặc dùng nhạc căng thẳng]
Ambience: [Để trống]

═══════════════════════════════════════════════════════
NEXT SCENE (Không dùng - chuyển về top-down)
═══════════════════════════════════════════════════════

Next Scene: None

Return To Top Down: ☐ false  [QUAN TRỌNG: false vì cần 14A-1]
Top Down Scene Name: [Để trống]
Spawn Point Id: [Để trống]

⚠️ Lý do false: Sau scene này cần chuyển sang 14A-1 (top-down)
   để show animation bị vây quanh, rồi mới đến 14A-2

═══════════════════════════════════════════════════════
CONDITIONS
═══════════════════════════════════════════════════════

Required Flags: Size = 1
  Element 0: bullies_following_week1

Forbidden Flags: Size = 1
  Element 0: week1_encounter_complete

═══════════════════════════════════════════════════════
EFFECTS ON ENTER
═══════════════════════════════════════════════════════

Set Flags On Enter: Size = 0
Variable Changes On Enter: Size = 0
```

---

## 🎬 SCENE 14A-2: Forced Friend (QUAN TRỌNG)

### **Bước 1: Tạo VNSceneData**

1. **Right-click** trong `Assets/Data/VisualNovel/Week1/`
2. `Create → Visual Novel → VN Scene Data`
3. Đặt tên: `Week1_Scene14A2_Forced_Friend_VNScene`

### **Bước 2: Cấu hình Scene Data**

```yaml
═══════════════════════════════════════════════════════
SCENE DATA
═══════════════════════════════════════════════════════

Scene Name: Week1_Scene14A2_Forced_Friend
Location Text: Đường phố - Bị vây quanh

Background Image: [Cùng street background như 14A]
Background Tint: (0.8, 0.8, 0.8, 1)  [Tối hơn một chút]

═══════════════════════════════════════════════════════
CHARACTERS (Nhiều hơn - bị vây quanh)
═══════════════════════════════════════════════════════

Characters: Size = 5

Element 0 (Thủ lĩnh - Center):
  Character Sprite: [Bully leader]
  Character Name: Thủ lĩnh
  Position: Center
  Position Offset: (0, -50)
  Scale: (1.1, 1.1)  [To hơn - đứng gần]
  Flip X: ☐ false

Element 1 (Đàn em - FarLeft):
  Character Sprite: [Bully minion]
  Character Name: Đàn em
  Position: FarLeft
  Position Offset: (0, -50)
  Scale: (0.85, 0.85)
  Flip X: ☐ false

Element 2 (Đàn em - Left):
  Character Sprite: [Bully minion]
  Character Name: Đàn em
  Position: Left
  Position Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip X: ☐ false

Element 3 (Đàn em - Right):
  Character Sprite: [Bully minion]
  Character Name: Đàn em
  Position: Right
  Position Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip X: ☑ true

Element 4 (Đàn em - FarRight):
  Character Sprite: [Bully minion]
  Character Name: Đàn em
  Position: FarRight
  Position Offset: (0, -50)
  Scale: (0.85, 0.85)
  Flip X: ☑ true

═══════════════════════════════════════════════════════
DIALOGUE & AUDIO
═══════════════════════════════════════════════════════

Dialogue: Week1_Scene14A2_Forced_Friend_Dialogue
  [Kéo DialogueData asset vào]

BGM: [Nhạc căng thẳng/đe dọa]
Ambience: [Để trống]

═══════════════════════════════════════════════════════
NEXT SCENE (Return to top-down)
═══════════════════════════════════════════════════════

Next Scene: None

Return To Top Down: ☑ true  [QUAN TRỌNG: true - về top-down]
Top Down Scene Name: StreetScene
Spawn Point Id: after_week1_encounter

⚠️ Spawn point "after_week1_encounter" phải được tạo trong StreetScene

═══════════════════════════════════════════════════════
CONDITIONS
═══════════════════════════════════════════════════════

Required Flags: Size = 1
  Element 0: week1_scene14a_completed

Forbidden Flags: Size = 1
  Element 0: week1_encounter_complete

═══════════════════════════════════════════════════════
EFFECTS ON COMPLETE (Sau khi dialogue kết thúc)
═══════════════════════════════════════════════════════

Set Flags On Complete: Size = 1
  Element 0: week1_encounter_complete

⚠️ Flag này ngăn encounter lặp lại
```

---

## ✅ CHECKLIST HOÀN THÀNH

### **Scene 13:**
- [ ] Import JSON → DialogueData ✅
- [ ] (Optional) Tạo VNSceneData
- [ ] Test dialogue flow

### **Scene 14A:**
- [ ] Import JSON → DialogueData ✅
- [ ] Tạo VNSceneData ⬜
- [ ] Cấu hình characters (3 bullies) ⬜
- [ ] Set conditions (required/forbidden flags) ⬜
- [ ] Test VN scene ⬜

### **Scene 14A-2:**
- [ ] Import JSON → DialogueData ✅
- [ ] Tạo VNSceneData ⬜
- [ ] Cấu hình characters (5 bullies) ⬜
- [ ] Set return to top-down ⬜
- [ ] Set flags on complete ⬜
- [ ] Test VN scene ⬜

---

## 🎨 NẾU CHƯA CÓ SPRITES

### **Tạm thời dùng placeholder:**

1. **Background:**
   - Tạo solid color sprite (xám/nâu)
   - Hoặc dùng background có sẵn từ Day1

2. **Characters:**
   - Dùng sprite có sẵn từ project
   - Hoặc tạo colored squares tạm (đỏ = leader, xanh = minions)

3. **Sau này:**
   - Thay thế bằng sprite thật
   - VNSceneData sẽ tự động update

---

## 🧪 TEST VN SCENES

### **Cách 1: Dùng VNSceneQuickTest**

1. Tạo empty scene mới: `TestScene`
2. Tạo GameObject: `VNTester`
3. Add component: `VNSceneQuickTest`
4. Kéo VNSceneData vào field `VN Scene To Test`
5. Play → Nhấn phím **T** để test

### **Cách 2: Test trong StreetScene**

1. Mở StreetScene
2. Tạo GameObject: `TestTrigger`
3. Add component: `VNTrigger`
4. Cấu hình:
   - Mode: `OnSceneStart`
   - VN Scene: [Kéo VNSceneData vào]
   - Trigger Once: true
5. Play → VN tự động chạy

---

## 🐛 TROUBLESHOOTING

### **Lỗi: "None (Script)" trong Inspector**
- DialogueData bị mất reference
- Fix: Re-import JSON file

### **Lỗi: Characters không hiển thị**
- Check sprite reference không null
- Check scale > 0
- Check background tint alpha = 1

### **Lỗi: Không return về top-down**
- Check `returnToTopDown = true`
- Check `topDownSceneName` đúng tên scene
- Check spawn point ID tồn tại

### **Lỗi: VN không trigger**
- Check required flags đã được set
- Check forbidden flags chưa được set
- Check VNTrigger collider overlap với player

---

## 📞 NEXT STEPS

Sau khi setup xong VN scenes:

1. **Setup StreetScene** (trigger zones, NPCs)
2. **Create spawn points**
3. **Test full gameplay flow**
4. **Add polish** (music, SFX, transitions)

Xem file: `STREETSCENE_SETUP_GUIDE.md` (sẽ tạo tiếp)

---

**Created:** 2025-12-18  
**Version:** 1.0

