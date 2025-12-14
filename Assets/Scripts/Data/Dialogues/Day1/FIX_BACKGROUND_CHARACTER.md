# 🔧 FIX: Background & Character không hiển thị

## ❌ VẤN ĐỀ

Dialogue hiển thị nhưng:
- Background không thấy (vẫn thấy top-down scene)
- Character sprite không hiển thị

---

## ✅ NGUYÊN NHÂN & FIX

### **1. Background Tint quá tối (FIXED in code)**

**Vấn đề:** `backgroundTint: {r: 0.004, a: 0.004}` → Gần như trong suốt!

**Fix tự động:** Code giờ sẽ auto-fix nếu alpha < 0.1:
```csharp
if (tint.a < 0.1f)
{
    tint = Color.white; // Force white
}
```

**Manual fix (recommended):**
```
1. Project → Day1_Scene1_Bedroom asset
2. Inspector → Scene Data → Background Tint
3. Set: R=1, G=1, B=1, A=1
4. Save (Ctrl+S)
```

### **2. Canvas Sorting Order (FIXED)**

**Vấn đề:** DialogueSystem canvas (sortingOrder=200) che VN canvas (sortingOrder=100)

**Fix:** VN canvas giờ có sortingOrder=300

### **3. Character Sprite thiếu**

**2 khả năng:**

**A) Sprite chưa được gán:**
```
1. Mở Day1_Scene1_Bedroom asset
2. Scene Data → Characters → Element 0
3. Character Sprite: [Kéo Mom sprite vào]
4. Character Name: "Mẹ"
5. Position: Left
6. Save
```

**B) Sprite path sai:**

Asset hiện tại link đến:
```yaml
characterSprite: {fileID: -4020990447090382813, guid: 43ad5034ea78a1f4681acbe53e020560}
```

**Check sprite tồn tại:**
```
1. Project → Search: "t:sprite 43ad5034ea78a1f4681acbe53e020560"
2. Nếu không tìm thấy → Cần import sprite mới
```

---

## 🎨 IMPORT MOM SPRITE (Nếu chưa có)

### **Option A: Tạo placeholder (1 phút)**

```
1. Unity → Assets → Create → Sprite → Square
2. Đặt tên: "Mom_Placeholder"
3. Inspector → Color: Pink (để dễ nhận biết)
4. Kéo vào Day1_Scene1_Bedroom → Characters[0] → Character Sprite
```

### **Option B: Import sprite thật**

```
1. Tìm/tạo Mom sprite (PNG, ~512x512px)
2. Kéo vào: Assets/Art/Characters/Mom/
3. Inspector → Texture Type: Sprite (2D and UI)
4. Kéo vào VNSceneData
```

---

## 🧪 TEST NGAY

### **Sau khi fix:**

1. **Stop Play mode**
2. **Save scene & assets**
3. **Play mode**
4. **Nhấn E ở giường**

### **Console log mới:**

```
[VNManager] Background set: sprite=True, tint=(1.0, 1.0, 1.0, 1.0)
[VNManager] Displaying 1 characters
[VNManager] Displaying character: Mẹ, position: Left
[DialogueSystem] Starting dialogue: Day1_Scene1_Bedroom_Full
```

### **Visual mới:**

```
┌─────────────────────────────────────────┐
│  📍 Phòng ngủ Đức - 7:00 AM             │
│                                         │
│  [Bedroom Background - FULL SCREEN]     │
│                                         │
│  👩 [Mom]                                │
│  (trái màn hình)                        │
│                                         │
│                                         │
├─────────────────────────────────────────┤
│ ┌─ Dialogue Panel ────────────────────┐ │
│ │ Mẹ                                  │ │
│ │                                     │ │
│ │ Đức ơi, dậy đi con!                 │ │
│ │                        ▼ Press E    │ │
│ └─────────────────────────────────────┘ │
└─────────────────────────────────────────┘
```

---

## 🔍 DEBUG: Check trong Play Mode

### **Hierarchy (khi VN đang chạy):**

```
VisualNovelManager
└── VNCanvas (Canvas, sortingOrder=300) ← Phải ở trên cùng
    └── VNPanel (RectTransform, fullscreen)
        ├── Background (Image) ← Check sprite & color
        ├── LocationText (Text) ← "Phòng ngủ Đức..."
        └── CharacterContainer
            └── Mẹ (Image) ← Check sprite hiển thị

DialogueSystem (Canvas, sortingOrder=200) ← Dưới VN
└── DialoguePanel
    ├── SpeakerName
    ├── DialogueText
    └── ContinueIcon
```

### **Inspector checks (trong Play Mode):**

**VNCanvas:**
- Sorting Order: **300** ✅

**Background Image:**
- Sprite: **[Your bedroom sprite]** ✅
- Color: **White (1,1,1,1)** ✅
- RectTransform: **Fullscreen** ✅

**Mom Character:**
- Active: **True** ✅
- Sprite: **[Mom sprite]** ✅
- Position: **(-400, 0)** (Left)

---

## 🚨 VẪN KHÔNG THẤY?

### **Background vẫn trong suốt:**

```
Console log:
[VNManager] Background tint too transparent (a=0.004), forcing to white
```

→ Asset vẫn có tint sai, fix thủ công như trên.

### **Character không hiển thị:**

```
Console log:
[VNManager] Character 'Mẹ' has no sprite!
```

→ Sprite chưa được gán, hoặc file không tồn tại.

**Fix:**
1. Tạo placeholder sprite
2. Gán vào VNSceneData
3. Test lại

### **Vẫn thấy top-down scene:**

```
Console log:
[VNManager] Background set: sprite=False, tint=(1.0, 1.0, 1.0, 1.0)
```

→ Background sprite = null!

**Fix:**
1. Tìm bedroom background sprite
2. Kéo vào Day1_Scene1_Bedroom → Scene Data → Background Image

---

## ✅ FINAL CHECKLIST

- [ ] Background Tint = (1, 1, 1, 1) hoặc auto-fixed
- [ ] Background Image có sprite
- [ ] Characters[0] có Mom sprite
- [ ] VN Canvas sortingOrder = 300
- [ ] DialogueSystem sortingOrder = 200
- [ ] Test: Background fullscreen
- [ ] Test: Mom sprite hiện ở bên trái
- [ ] Test: Dialogue text đọc được

---

## 🎊 SAU KHI FIX XONG

**Bạn sẽ thấy:**
- ✅ Bedroom background fullscreen
- ✅ Mom sprite bên trái
- ✅ Location text trên cùng
- ✅ Dialogue panel dưới cùng
- ✅ Clean VN presentation!

**DONE! 🚀**

