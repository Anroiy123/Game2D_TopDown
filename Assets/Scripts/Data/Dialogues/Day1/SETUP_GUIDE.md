# 🎬 HƯỚNG DẪN SETUP VN SCENE 1 - ĐẦY ĐỦ

**Mục tiêu:** Setup cảnh VN đầy tiên với background, character sprites (Mẹ + Đức), và dialogue hoàn chỉnh.

---

## 📋 CHECKLIST CHUẨN BỊ

- [x] ✅ DialogueData đã import (`Day1_Scene1_Bedroom_Full`)
- [ ] Background sprite (phòng ngủ)
- [ ] Character sprite: Mẹ
- [ ] Character sprite: Đức (player avatar)
- [ ] BGM: Nhạc nền buổi sáng (optional)
- [ ] VNSceneData asset (sẽ tạo ngay)

---

## 🎨 BƯỚC 1: CHUẨN BỊ SPRITES

### **1.1. Background Sprite**

**Cần:** Ảnh phòng ngủ (kích thước đề xuất: 1920x1080)

**Có 2 cách:**

**Cách A - Sử dụng ảnh có sẵn:**
```
1. Tìm ảnh bedroom background (Google: "bedroom anime background")
2. Kéo vào Unity: Assets/Art/Backgrounds/
3. Inspector → Texture Type: Sprite (2D and UI)
4. Apply
```

**Cách B - Tạo placeholder:**
```
1. Tạo ảnh đơn sắc 1920x1080 (màu kem/trắng)
2. Kéo vào Unity như trên
```

### **1.2. Character Sprite: Mẹ**

**Cần:** Sprite nhân vật Mẹ (đứng, toàn thân hoặc nửa thân)

**Kích thước đề xuất:** 500-800px chiều cao

**Tìm asset:**
- Vẽ tay
- Sử dụng character creator tools
- Mua asset từ Unity Asset Store
- AI generation (stable diffusion, etc.)

**Import:**
```
1. Kéo vào: Assets/Art/Characters/Mom/
2. Texture Type: Sprite (2D and UI)
3. Pixels Per Unit: 100 (default)
4. Filter Mode: Bilinear
5. Apply
```

**Lưu ý:** Nếu chưa có sprite, có thể dùng placeholder (hình chữ nhật màu)

### **1.3. Character Sprite: Đức (Optional cho scene này)**

Scene 1 chủ yếu là Mẹ nói, Đức trả lời (có thể chỉ dùng dialogue box).

Nếu muốn hiển thị Đức:
```
- Tương tự như Mẹ
- Kéo vào: Assets/Art/Characters/Player/
```

---

## 🛠️ BƯỚC 2: TẠO VNSceneData ASSET

### **2.1. Tạo Asset**

```
1. Unity Project → Assets/Scripts/Data/VisualNovel/
2. Right-click → Create → Visual Novel → VN Scene Data
3. Đặt tên: "Day1_Scene1_Bedroom"
```

### **2.2. Cấu hình Scene Data**

**Mở Inspector của `Day1_Scene1_Bedroom`:**

```yaml
## SCENE DATA

Scene Name: "Day1_Scene1_Bedroom"
Location Text: "Phòng ngủ Đức - 7:00 AM"

## BACKGROUND
Background Image: [Kéo bedroom background sprite vào đây]
Background Tint: (R:1, G:1, B:1, A:1)  # Trắng = không tint

## CHARACTERS ON SCREEN
Characters: Array size = 1  # Chỉ có Mẹ

  Element 0:
    Character Sprite: [Kéo Mom sprite vào đây]
    Character Name: "Mẹ"
    Position: Left  # Mẹ đứng bên trái
    Position Offset: (X:0, Y:-50)  # Điều chỉnh để feet không bị cắt
    Scale: (X:1, Y:1)  # Kích thước bình thường
    Flip X: ☐ (false)  # Không flip

## DIALOGUE
Dialogue: [Kéo Day1_Scene1_Bedroom_Full vào đây]

## AUDIO (Optional)
BGM: [Kéo morning music clip vào đây hoặc để None]
Ambience: [Birds chirping sound hoặc None]

## NEXT SCENE
Next Scene: None  # Scene đầu tiên, chưa có next
Return To Top Down: ☑ true  # Quay lại top-down sau dialogue
Top Down Scene Name: "HomeScene"  # Tên scene nhà của bạn
Spawn Point Id: "bedroom_spawn"  # SpawnPoint trong phòng ngủ
```

---

## 🏗️ BƯỚC 3: SETUP TOP-DOWN SCENE (HomeScene)

Để quay về top-down mode sau VN, cần có:

### **3.1. Tạo SpawnPoint trong HomeScene**

```
1. Mở scene: HomeScene (hoặc scene nhà bạn)
2. GameObject → Create Empty
3. Đặt tên: "SpawnPoint_BedroomAfterVN"
4. Add Component → SpawnPoint
5. Inspector:
   - Spawn Point Id: "bedroom_spawn"
   - Is Default Spawn: ☐ false
   - Facing Direction: Down  # Đức nhìn xuống sau khi dậy
6. Đặt position: Vị trí giữa phòng ngủ
```

### **3.2. Tạo VNTrigger để test**

```
1. GameObject → Create Empty
2. Đặt tên: "TestVNTrigger_Day1Scene1"
3. Add Component → Box Collider 2D
   - Is Trigger: ☑ true
   - Size: (2, 2)
4. Add Component → VNTrigger
5. Inspector:
   - Trigger Mode: On Interact (E key)
   - VN Scene: [Kéo Day1_Scene1_Bedroom vào]
   - Play Once: ☑ true
   - Required Flags: (empty)
   - Forbidden Flags: (empty)
```

Đặt trigger này ở giường → Nhấn E để trigger VN!

---

## 🧪 BƯỚC 4: TEST

### **4.1. Test cơ bản**

```
1. Play mode
2. Di chuyển player đến VNTrigger
3. Nhấn E
4. → Dialogue hiện lên!
```

### **4.2. Test checklist**

- [ ] Background hiển thị đúng
- [ ] Sprite Mẹ hiển thị đúng vị trí
- [ ] Dialogue text hiển thị
- [ ] Speaker name = "Mẹ" khi Mẹ nói
- [ ] Speaker name = "Đức" khi player nói
- [ ] Choices hiển thị (3 options)
- [ ] Chọn choice → Dialogue tiếp tục
- [ ] Variables thay đổi (money +20000, relationship +X)
- [ ] Kết thúc dialogue → Quay về top-down
- [ ] Player spawn đúng vị trí bedroom_spawn

---

## 🐛 TROUBLESHOOTING

### **❌ "Dialogue không hiện"**
✅ Check: VNSceneData có link đúng DialogueData chưa?

### **❌ "Sprite Mẹ không hiển thị"**
✅ Check: 
- Texture Type = Sprite (2D and UI)?
- Character Sprite field có sprite chưa?

### **❌ "Sprite bị cắt/vị trí sai"**
✅ Adjust: Position Offset (Y:-50 hoặc Y:-100)
✅ Adjust: Scale (0.8 nếu quá to, 1.2 nếu quá nhỏ)

### **❌ "Không quay về top-down"**
✅ Check:
- Return To Top Down = true?
- Top Down Scene Name đúng tên scene?
- SpawnPoint tồn tại với đúng ID?

### **❌ "Variables không thay đổi"**
✅ Check StoryManager initialized chưa:
```csharp
Debug.Log($"Money: {StoryManager.Instance.GetVariable("money")}");
```

---

## 📊 KẾT QUẢ CUỐI CÙNG

Sau khi setup xong:

✅ **VN Mode:**
- Background: Phòng ngủ
- Character: Mẹ (bên trái)
- Dialogue: Full 17 nodes với choices
- Variables: Money +20000, Relationship +0 to +10

✅ **Top-Down Mode:**
- Quay về HomeScene
- Spawn tại bedroom_spawn
- Player có thể di chuyển tiếp

---

**🎉 HOÀN TẤT! Scene 1 đã ready!** 🎉

