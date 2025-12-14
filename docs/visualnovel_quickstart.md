# 🎬 HƯỚNG DẪN TẠO CẢNH VN ĐẦU TIÊN
## Day 1 - Buổi sáng tại phòng ngủ Đức

---

## 📋 CHUẨN BỊ

### 1. Tài nguyên cần có:
- ✅ Ảnh nền phòng ngủ Đức (background image)
- ✅ Sprite nhân vật Mẹ (nếu có)
- ✅ Sprite nhân vật Đức (nếu có)

### 2. Cốt truyện cảnh này:
```
Đức thức dậy trong phòng ngủ.
Mẹ đánh thức Đức đi học.
Đức chuẩn bị cho ngày đầu tiên ở trường mới.
```

---

## 🚀 PHƯƠNG PHÁP 1: SỬ DỤNG EDITOR TOOL (NHANH)

### Bước 1: Mở VN Scene Creator
```
Unity Editor → Menu → Tools → Visual Novel → Create VN Scene Quick Setup
```

### Bước 2: Điền thông tin
```
Scene Name: Day1_Morning_Bedroom
Location Text: Phòng ngủ Đức
Background Sprite: [Kéo ảnh nền vào đây]
✅ Tạo DialogueData mẫu
```

### Bước 3: Nhấn "Tạo VN Scene"
Tool sẽ tự động tạo:
- `Day1_Morning_Bedroom_Dialogue.asset` (DialogueData)
- `Day1_Morning_Bedroom_VNScene.asset` (VNSceneData)

Trong folder: `Assets/Scripts/Data/VisualNovel/`

---

## 🎨 PHƯƠNG PHÁP 2: TẠO THỦ CÔNG (CHI TIẾT)

### Bước 1: Tạo DialogueData

#### 1.1. Tạo Asset
```
Project → Assets/Scripts/Data/ → Right-click
→ Create → Dialogue → Dialogue Data
→ Đặt tên: "Day1_Morning_Bedroom_Dialogue"
```

#### 1.2. Cấu hình Dialogue (trong Inspector)

**Node 0: Narrator - Mở đầu**
```yaml
Node ID: 0
Speaker Name: ""  # Để trống cho narrator
Is Player Speaking: ☐ (false)
Dialogue Lines:
  - "Ánh sáng ban mai chiếu qua cửa sổ..."
  - "Đức từ từ mở mắt ra."
Choices: (để trống)
Next Node ID: 1
```

**Node 1: Mẹ gọi Đức dậy**
```yaml
Node ID: 1
Speaker Name: "Mẹ"
Is Player Speaking: ☐ (false)
Dialogue Lines:
  - "Đức ơi, dậy đi con!"
  - "Hôm nay là ngày đầu tiên đi học trường mới mà."
Choices: (để trống)
Next Node ID: 2
```

**Node 2: Đức trả lời**
```yaml
Node ID: 2
Speaker Name: "Đức"
Is Player Speaking: ☑ (true)
Dialogue Lines:
  - "Dạ, con dậy rồi ạ..."
Choices: (để trống)
Next Node ID: 3
```

**Node 3: Mẹ động viên**
```yaml
Node ID: 3
Speaker Name: "Mẹ"
Is Player Speaking: ☐ (false)
Dialogue Lines:
  - "Xuống ăn sáng đi con. Mẹ đã chuẩn bị phở rồi."
  - "Nhớ ăn no vào nhé!"
Choices: (để trống)
Next Node ID: -1  # Kết thúc dialogue
```

### Bước 2: Tạo VNSceneData

#### 2.1. Tạo Asset
```
Project → Assets/Scripts/Data/ → Right-click
→ Create → Visual Novel → VN Scene Data
→ Đặt tên: "Day1_Morning_Bedroom"
```

#### 2.2. Cấu hình VN Scene (trong Inspector)

**Scene Data:**
```yaml
Scene Name: "Day1_Morning_Bedroom"
Location Text: "Phòng ngủ Đức"
Background Image: [Kéo sprite ảnh nền vào đây]
Background Tint: White (255,255,255,255)

Characters: Array size = 0  # Cảnh này không có nhân vật hiển thị
                            # (chỉ có narrator + dialogue)

Dialogue: [Kéo Day1_Morning_Bedroom_Dialogue vào đây]

BGM: (để trống hoặc thêm nhạc buổi sáng)
Ambience: (để trống hoặc thêm tiếng chim chóc, xe cộ)

Next Scene: (để trống - tạm thời)
Return To Top Down: ☑ (true)
Top Down Scene Name: "HomeScene"  # Tên scene nhà Đức
Spawn Point Id: "bedroom_spawn"    # Vị trí spawn trong phòng
```

**Conditions (Optional):**
```yaml
Required Flags: (để trống)  # Không cần điều kiện
Forbidden Flags: (để trống)
```

**Effects On Enter:**
```yaml
Set Flags On Enter: 
  - "day1_started"
  - "woke_up"

Variable Changes On Enter:
  Size = 1
  Element 0:
    Variable Name: "CURRENT_DAY"
    Operation: Set
    Value: 1
```

---

## 🎮 PHƯƠNG PHÁP 3: TẠO CẢNH CÓ NHÂN VẬT (NÂNG CAO)

Nếu bạn muốn hiển thị sprite nhân vật Mẹ:

### Cấu hình Characters trong VNSceneData:

```yaml
Characters: Array size = 1
  Element 0:
    Character Sprite: [Kéo sprite Mẹ vào đây]
    Character Name: "Mẹ"
    Position: Left  # Mẹ đứng bên trái màn hình
    Position Offset: (0, -50)  # Điều chỉnh vị trí
    Scale: (1, 1)
    Flip X: ☐ (false)
```

**Vị trí nhân vật:**
- `Left` = -400px (bên trái)
- `Center` = 0px (giữa màn hình)
- `Right` = 400px (bên phải)
- `FarLeft` = -600px (xa bên trái)
- `FarRight` = 600px (xa bên phải)
- `Custom` = Dùng Position Offset

---

## 🔗 BƯỚC 3: TRIGGER CẢNH VN TRONG GAME

### Cách 1: Trigger khi vào Scene (Auto)

#### 3.1. Tạo GameObject trong scene "HomeScene"
```
Hierarchy → Right-click → Create Empty
→ Đặt tên: "VNTrigger_Day1Morning"
```

#### 3.2. Add Component VNTrigger
```
Inspector → Add Component → VN Trigger

VN Scene: [Kéo Day1_Morning_Bedroom vào đây]

Trigger Settings:
  Mode: On Scene Start  # Tự động chạy khi vào scene
  Trigger Once: ☑ (true)

Conditions:
  Required Flags: (để trống)
  Forbidden Flags: 
    - "day1_started"  # Không chạy nếu đã chạy rồi
```

### Cách 2: Trigger khi player vào vùng (Collision)

#### 3.1. Tạo Trigger Zone
```
Hierarchy → Right-click → Create Empty
→ Đặt tên: "VNTrigger_BedroomDoor"
```

#### 3.2. Add BoxCollider2D
```
Inspector → Add Component → Box Collider 2D
  Is Trigger: ☑ (true)
  Size: (2, 2)
```

#### 3.3. Add Component VNTrigger
```
VN Scene: [Kéo Day1_Morning_Bedroom vào đây]

Trigger Settings:
  Mode: On Trigger Enter  # Chạy khi player chạm vào
  Trigger Once: ☑ (true)

Conditions:
  Required Flags: (để trống)
  Forbidden Flags: 
    - "day1_started"
```

### Cách 3: Trigger bằng code

```csharp
// Từ bất kỳ script nào
public VNSceneData day1Morning;

void Start() 
{
    VisualNovelManager.Instance.StartVNScene(day1Morning, OnVNComplete);
}

void OnVNComplete()
{
    Debug.Log("Cảnh VN đã kết thúc!");
}
```

---

## ✅ KIỂM TRA

### 1. Chạy thử trong Unity Editor
```
1. Nhấn Play
2. Di chuyển player vào trigger zone (hoặc đợi auto trigger)
3. Kiểm tra:
   ✅ Fade out → Hiện background
   ✅ Dialogue hiện lên
   ✅ Nhấn E để tiếp tục
   ✅ Kết thúc dialogue → Fade in → Về top-down mode
```

### 2. Debug
```
Console sẽ hiện:
[VNManager] Starting VN scene: Day1_Morning_Bedroom
[DialogueSystem] Starting dialogue: Day1_Morning_Bedroom_Dialogue
[VNManager] VN scene completed
```

---

## 📊 LUỒNG HOẠT ĐỘNG

```
Player vào trigger zone
     ↓
VNTrigger.TryTriggerVN()
     ↓
VisualNovelManager.StartVNScene()
     ↓
Set flags: day1_started, woke_up
Set variable: CURRENT_DAY = 1
     ↓
Fade out current scene
     ↓
Hide player
     ↓
Show VN panel với background
     ↓
DialogueSystem.StartDialogueWithChoices()
     ↓
Hiển thị dialogue (Node 0 → 1 → 2 → 3)
     ↓
Dialogue kết thúc (Node -1)
     ↓
VNManager.OnDialogueComplete()
     ↓
Check nextScene? NO → EndVNMode()
     ↓
Fade out VN panel
     ↓
Load scene "HomeScene" với spawn "bedroom_spawn"
     ↓
Fade in → Player có thể di chuyển
```

---

## 🎯 TỔNG KẾT

**Bạn đã tạo:**
1. ✅ DialogueData cho cảnh buổi sáng
2. ✅ VNSceneData với background + dialogue
3. ✅ VNTrigger để kích hoạt cảnh

**Tiếp theo:**
- Tạo cảnh VN thứ 2: "Day1_Morning_Kitchen" (bữa sáng)
- Link các cảnh với nhau bằng `Next Scene`
- Thêm nhạc nền, sound effects
- Thêm sprite nhân vật

---

## 💡 MẸO

1. **Link cảnh liên tiếp**: Set `Next Scene` để tự động chuyển cảnh
2. **Điều kiện hiển thị**: Dùng `Required Flags` để cảnh chỉ chạy khi đủ điều kiện
3. **Multiple endings**: Dùng `Choices` trong DialogueData để branch story
4. **Debug nhanh**: Gọi trực tiếp `VisualNovelManager.Instance.StartVNScene()` từ Console

---

✨ **Chúc bạn tạo game thành công!**

