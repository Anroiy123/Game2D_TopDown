# ✅ FIXED! - HƯỚNG DẪN SỬ DỤNG VNTRIGGER

## 🎉 ĐÃ FIX CÁC LỖI:

✅ **Fixed:** `StoryManager.GetAllFlags()` - Đã thêm method  
✅ **Fixed:** `transitionDuration` warning - Đã xóa biến không dùng  
✅ **Fixed:** VNSceneData script - Compile lại Unity sẽ ok  

---

## 🎮 SỬ DỤNG VNTRIGGER CÓ SẴN

**Bạn KHÔNG cần VNSceneQuickTest!** VNTrigger có sẵn là đủ!

### **Cách setup VNTrigger:**

```yaml
═══════════════════════════════════════════════
VN TRIGGER CONFIGURATION
═══════════════════════════════════════════════

┌─ VN Scene ───────────────────────────────────┐
│ VN Scene: [Day1_Scene1_Bedroom] ← Kéo vào    │
└──────────────────────────────────────────────┘

┌─ Trigger Settings ───────────────────────────┐
│ Mode: On Interact  ← Nhấn E để trigger       │
│ Interaction Key: E                            │
│ Trigger Once: ☑ true  ← Chỉ trigger 1 lần    │
└──────────────────────────────────────────────┘

┌─ Conditions (Optional) ──────────────────────┐
│ Required Flags: (empty)                       │
│ Forbidden Flags: (empty)                      │
│                                               │
│ 💡 Nếu muốn chặn replay:                      │
│   Forbidden Flags:                            │
│     - day1_bedroom_completed                  │
└──────────────────────────────────────────────┘

┌─ Visual Feedback (Optional) ─────────────────┐
│ Interaction Prompt: (None)                    │
│                                               │
│ 💡 Có thể tạo UI "Press E" để hiển thị       │
└──────────────────────────────────────────────┘
```

---

## 📋 3 TRIGGER MODES

### **Mode 1: On Trigger Enter (Auto)**
- Player đi vào collider → VN tự động bắt đầu
- Dùng cho: Cutscenes tự động, story triggers

```yaml
Mode: OnTriggerEnter
Trigger Once: ☑ true
```

### **Mode 2: On Interact (E key) ⭐ RECOMMENDED**
- Player đứng trong collider → Nhấn E để bắt đầu VN
- Dùng cho: Giường, cửa, NPCs, interactive objects

```yaml
Mode: OnInteract
Interaction Key: E
Trigger Once: ☑ true
```

### **Mode 3: On Scene Start**
- VN bắt đầu ngay khi scene load
- Dùng cho: Opening scene, day start cutscene

```yaml
Mode: OnSceneStart
Trigger Once: ☑ true
```

---

## 🛠️ SETUP CHI TIẾT

### **Bước 1: GameObject đã có**
```
✅ Bạn đã có GameObject với VNTrigger component
```

### **Bước 2: Kiểm tra Collider2D**
```
1. Inspector → Check có BoxCollider2D chưa?
2. Nếu chưa → Add Component → Box Collider 2D
3. Is Trigger: ☑ true  ← QUAN TRỌNG!
4. Size: (2, 2) hoặc tùy chỉnh
```

### **Bước 3: Link VNSceneData**
```
1. VNTrigger component → VN Scene field
2. Kéo asset "Day1_Scene1_Bedroom" vào
3. ✅ Done!
```

### **Bước 4: Chọn Trigger Mode**
```
Mode: On Interact
Interaction Key: E
Trigger Once: ☑ true
```

### **Bước 5: Đặt vị trí**
```
- Đặt GameObject ở giường ngủ trong HomeScene
- Player đứng gần giường → Nhấn E → VN bắt đầu!
```

---

## 🧪 TEST

### **Test trong Play Mode:**

1. **Play game**
2. **Di chuyển player** đến GameObject có VNTrigger
3. **Nhấn E**
4. **✅ VN Scene sẽ bắt đầu!**

### **Kiểm tra trong Console:**

```
[VNTrigger] GameObjectName: VN scene completed
[StoryManager] Flag 'day1_started' = True
[StoryManager] Variable 'current_day' = 1
[StoryManager] Variable 'money' = 70000  (50k ban đầu + 20k)
```

---

## 🎨 THÊM UI "PRESS E" (OPTIONAL)

### **Tạo UI Prompt:**

```
1. GameObject → UI → Image (tạo Canvas tự động)
2. Đặt tên: "InteractionPrompt_E"
3. Thêm Text: "Press E"
4. Position: Trên đầu trigger object
5. Disable GameObject (inactive)
```

### **Link vào VNTrigger:**

```
Visual Feedback:
  Interaction Prompt: [Kéo InteractionPrompt_E vào đây]
```

**Kết quả:**
- Player đi gần → "Press E" hiện lên
- Player đi xa → "Press E" biến mất
- Nhấn E → VN bắt đầu + UI biến mất

---

## 🔧 ADVANCED: CONDITIONS

### **Chỉ trigger nếu có flag:**

```yaml
Required Flags:
  - day1_started  ← Phải có flag này
```

### **Không trigger nếu đã làm rồi:**

```yaml
Forbidden Flags:
  - day1_bedroom_completed  ← Không có flag này
```

### **Example - Replay Protection:**

```yaml
Trigger Once: ☑ true  ← Chặn trong 1 session
Forbidden Flags:
  - day1_bedroom_completed  ← Chặn giữa các session
```

---

## 🐛 TROUBLESHOOTING

### ❌ "Nhấn E không có gì xảy ra"

**Check:**
- [ ] BoxCollider2D có `Is Trigger = true`?
- [ ] Player có tag "Player"?
- [ ] Mode = "On Interact"?
- [ ] VN Scene field không null?
- [ ] Player đứng TRONG collider?

**Debug:**
```csharp
// Thêm vào VNTrigger.cs line 85 để debug:
Debug.Log($"Player in range: {playerInRange}");
```

### ❌ "VN trigger nhưng không hiển thị"

**Check:**
- [ ] VNSceneData có link DialogueData?
- [ ] DialogueData có nodes?
- [ ] Background sprite không null?
- [ ] VisualNovelManager tồn tại trong scene?

**Validate:**
```
Menu: Tools → Visual Novel → Validate VN Scene
Kéo Day1_Scene1_Bedroom vào → Click Validate
```

### ❌ "Trigger nhiều lần dù đã set Trigger Once"

**Fix:**
```yaml
Trigger Once: ☑ true
Forbidden Flags:
  - day1_bedroom_completed  ← Thêm flag này

Và trong VNSceneData:
Set Flags On Enter:
  - day1_bedroom_completed  ← Set flag này
```

---

## 📊 SO SÁNH: VNTrigger vs VNQuickTest

| Feature | VNTrigger | VNQuickTest |
|---------|-----------|-------------|
| **Dùng cho** | Production | Debug/Testing |
| **Trigger** | Player interaction | Phím tắt (T) |
| **Collider** | Cần | Không cần |
| **UI Prompt** | Có | Không |
| **Conditions** | Có (flags) | Không |
| **Location** | In-world | Anywhere |
| **Modes** | 3 modes | 1 mode |

**Kết luận:** Dùng **VNTrigger** cho game thực, dùng **VNQuickTest** để test nhanh!

---

## ✅ FINAL CHECKLIST

- [ ] GameObject có VNTrigger component
- [ ] BoxCollider2D với Is Trigger = true
- [ ] VN Scene field = Day1_Scene1_Bedroom
- [ ] Mode = On Interact
- [ ] Interaction Key = E
- [ ] Trigger Once = true
- [ ] Position = Gần giường trong HomeScene
- [ ] Test: Play → Di chuyển → Nhấn E → VN bắt đầu!

---

## 🎉 BẠN ĐÃ SẴN SÀNG!

**Giờ bạn có thể:**
1. ✅ Trigger VN scene bằng VNTrigger
2. ✅ Player nhấn E để bắt đầu dialogue
3. ✅ Test toàn bộ 17 nodes với choices
4. ✅ Xem flags/variables thay đổi
5. ✅ Quay về top-down mode sau VN

**Next step:** Add background + Mom sprite để hoàn thiện! 🎨

