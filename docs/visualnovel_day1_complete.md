# 📖 HƯỚNG DẪN TẠO CHUỖI CẢNH DAY 1 HOÀN CHỈNH

## 🎬 Tổng quan Day 1

Day 1 bao gồm **5 cảnh VN** liên tiếp:

```
1. Phòng ngủ Đức (Mẹ đánh thức)
   ↓
2. Phòng khách (Ăn sáng)
   ↓
3. Trước cổng trường (Lo lắng)
   ↓
4. Lớp học (Giới thiệu bản thân)
   ↓
5. Sân trường (Gặp bọn bully lần đầu)
```

---

## 🎯 CẢNH 1: Phòng ngủ Đức

### DialogueData: `Day1_Scene1_Bedroom_Dialogue`

**Node 0: Ánh sáng ban mai**

```yaml
Node ID: 0
Speaker Name: ""
Is Player Speaking: false
Dialogue Lines:
  - "Ánh sáng ban mai chiếu qua cửa sổ..."
  - "Hôm nay là ngày đầu tiên ở trường mới."
Next Node ID: 1
```

**Node 1: Mẹ gọi dậy**

```yaml
Node ID: 1
Speaker Name: "Mẹ"
Dialogue Lines:
  - "Đức ơi, dậy đi con!"
  - "Hôm nay là ngày đầu tiên đi học mà."
Next Node ID: 2
```

**Node 2: Đức trả lời**

```yaml
Node ID: 2
Speaker Name: "Đức"
Is Player Speaking: true
Dialogue Lines:
  - "Dạ... con dậy rồi ạ..."
  - "(Mình hơi lo lắng về trường mới...)"
Next Node ID: -1
```

### VNSceneData: `Day1_Scene1_Bedroom`

```yaml
Scene Name: "Day1_Scene1_Bedroom"
Location Text: "Phòng ngủ Đức - 7:00 AM"
Background Image: [bedroom_bg.png]
Characters: (không có nhân vật hiển thị)
Dialogue: Day1_Scene1_Bedroom_Dialogue
Next Scene: Day1_Scene2_LivingRoom  ← Link đến cảnh 2
Return To Top Down: false  ← Tiếp tục VN mode

Set Flags On Enter:
  - "day1_started"
Variable Changes On Enter:
  - Variable: CURRENT_DAY, Operation: Set, Value: 1
```

---

## 🍜 CẢNH 2: Phòng khách (Ăn sáng)

### DialogueData: `Day1_Scene2_LivingRoom_Dialogue`

**Node 0: Mô tả**

```yaml
Node ID: 0
Speaker Name: ""
Dialogue Lines:
  - "Mùi phở thơm phức bay ra từ nhà bếp."
  - "Đức ngồi xuống bàn ăn."
Next Node ID: 1
```

**Node 1: Mẹ quan tâm**

```yaml
Node ID: 1
Speaker Name: "Mẹ"
Dialogue Lines:
  - "Ăn no vào nhé con. Trường mới chắc nhiều chuyện lắm."
  - "Con có lo lắng gì không?"
Next Node ID: 2
```

**Node 2: Đức trấn an mẹ**

```yaml
Node ID: 2
Speaker Name: "Đức"
Is Player Speaking: true
Dialogue Lines:
  - "Con ổn mà mẹ. Mẹ đừng lo."
  - "(Mình không muốn mẹ lo lắng...)"
Next Node ID: 3
```

**Node 3: Mẹ khuyên**

```yaml
Node ID: 3
Speaker Name: "Mẹ"
Dialogue Lines:
  - "Nếu có chuyện gì thì nhớ nói với mẹ nhé."
  - "Mẹ luôn ở đây cho con."
Next Node ID: -1
```

### VNSceneData: `Day1_Scene2_LivingRoom`

```yaml
Scene Name: "Day1_Scene2_LivingRoom"
Location Text: "Phòng khách - 7:30 AM"
Background Image: [livingroom_bg.png]
Characters: Array size = 1
  Element 0:
    Character Sprite: [mom_sprite.png]
    Character Name: "Mẹ"
    Position: Left
    Scale: (1, 1)
Dialogue: Day1_Scene2_LivingRoom_Dialogue
Next Scene: Day1_Scene3_SchoolGate
Return To Top Down: false
```

---

## 🏫 CẢNH 3: Trước cổng trường

### DialogueData: `Day1_Scene3_SchoolGate_Dialogue`

**Node 0: Trước cổng**

```yaml
Node ID: 0
Speaker Name: ""
Dialogue Lines:
  - "Đức đứng trước cổng trường THPT Thanh Hóa."
  - "Những học sinh khác đang nô đùa, cười nói."
Next Node ID: 1
```

**Node 1: Suy nghĩ của Đức**

```yaml
Node ID: 1
Speaker Name: "Đức"
Is Player Speaking: true
Dialogue Lines:
  - "(Mọi người nhìn có vẻ thân thiện...)"
  - "(Hy vọng mình sẽ kết bạn được với họ.)"
Next Node ID: -1
```

### VNSceneData: `Day1_Scene3_SchoolGate`

```yaml
Scene Name: "Day1_Scene3_SchoolGate"
Location Text: "Cổng trường THPT Thanh Hóa - 8:00 AM"
Background Image: [school_gate_bg.png]
Characters: (không có)
Dialogue: Day1_Scene3_SchoolGate_Dialogue
Next Scene: Day1_Scene4_Classroom
Return To Top Down: false
```

---

## 📚 CẢNH 4: Lớp học (Giới thiệu)

### DialogueData: `Day1_Scene4_Classroom_Dialogue`

**Node 0: Giáo viên**

```yaml
Node ID: 0
Speaker Name: "Cô Lan"
Dialogue Lines:
  - "Các em chào cô ạ!"
  - "Hôm nay chúng ta có một bạn mới. Bạn lên giới thiệu đi."
Next Node ID: 1
```

**Node 1: Đức giới thiệu**

```yaml
Node ID: 1
Speaker Name: "Đức"
Is Player Speaking: true
Dialogue Lines:
  - "Dạ, em chào các bạn ạ..."
  - "Em tên là Đức, em vừa chuyển đến đây."
Next Node ID: 2
```

**Node 2: Cô giáo**

```yaml
Node ID: 2
Speaker Name: "Cô Lan"
Dialogue Lines:
  - "Tốt lắm. Các bạn hãy giúp đỡ Đức nhé."
  - "Đức ngồi chỗ trống bên kia nhé."
Next Node ID: -1
```

### VNSceneData: `Day1_Scene4_Classroom`

```yaml
Scene Name: "Day1_Scene4_Classroom"
Location Text: "Lớp 10A1 - 8:30 AM"
Background Image: [classroom_bg.png]
Characters: Array size = 1
  Element 0:
    Character Sprite: [teacher_sprite.png]
    Character Name: "Cô Lan"
    Position: Center
Dialogue: Day1_Scene4_Classroom_Dialogue
Next Scene: Day1_Scene5_Schoolyard
Return To Top Down: false
```

---

## 😨 CẢNH 5: Sân trường (Gặp bully)

### DialogueData: `Day1_Scene5_Schoolyard_Dialogue`

**Node 0: Giờ ra chơi**

```yaml
Node ID: 0
Speaker Name: ""
Dialogue Lines:
  - "Giờ ra chơi. Đức đi ra sân trường."
Next Node ID: 1
```

**Node 1: Bully xuất hiện**

```yaml
Node ID: 1
Speaker Name: "???"
Dialogue Lines:
  - "Ê, thằng mới kìa!"
Next Node ID: 2
```

**Node 2: Đức sợ**

```yaml
Node ID: 2
Speaker Name: "Đức"
Is Player Speaking: true
Dialogue Lines:
  - "(Mấy người này nhìn đáng sợ quá...)"
Next Node ID: 3
```

**Node 3: Vũ chặn đường**

```yaml
Node ID: 3
Speaker Name: "Vũ"
Dialogue Lines:
  - "Tao là Vũ. Còn đây là Minh và Hoàng."
  - "Từ giờ mày nghe lời tao là được."
Next Node ID: 4
```

**Node 4: Lựa chọn của Đức**

```yaml
Node ID: 4
Speaker Name: "Đức"
Is Player Speaking: true
Dialogue Lines:
  - "(Mình nên làm gì đây...?)"
Choices:
  Choice 1:
    Choice Text: "Im lặng, gật đầu"
    Next Node ID: 10
    Set Flags True: ["first_encounter_submit"]
    Variable Changes:
      - Variable: FEAR_LEVEL, Operation: Add, Value: 10

  Choice 2:
    Choice Text: "Cố gắng tránh né"
    Next Node ID: 20
    Set Flags True: ["first_encounter_avoid"]
    Variable Changes:
      - Variable: FEAR_LEVEL, Operation: Add, Value: 5
```

**Node 10: Nhánh phục tùng**

```yaml
Node ID: 10
Speaker Name: "Vũ"
Dialogue Lines:
  - "Thông minh đấy. Ngày mai nhớ mang tiền cho tao nhé."
  - "10,000 đồng là được."
Next Node ID: 11
```

**Node 11: Kết thúc nhánh 1**

```yaml
Node ID: 11
Speaker Name: ""
Dialogue Lines:
  - "(Đức cảm thấy hoảng sợ...)"
  - "(Mình đã mắc nợ chúng rồi...)"
Next Node ID: -1
```

**Node 20: Nhánh tránh né**

```yaml
Node ID: 20
Speaker Name: "Đức"
Is Player Speaking: true
Dialogue Lines:
  - "Ừm... mình phải về lớp rồi..."
Next Node ID: 21
```

**Node 21: Vũ đe dọa**

```yaml
Node ID: 21
Speaker Name: "Vũ"
Dialogue Lines:
  - "Được, lần này tao tha. Nhưng đừng tránh tao lần sau."
Next Node ID: 22
```

**Node 22: Kết thúc nhánh 2**

```yaml
Node ID: 22
Speaker Name: ""
Dialogue Lines:
  - "(Đức cảm thấy lo lắng...)"
  - "(Chuyện gì sẽ xảy ra tiếp theo?)"
Next Node ID: -1
```

### VNSceneData: `Day1_Scene5_Schoolyard`

```yaml
Scene Name: "Day1_Scene5_Schoolyard"
Location Text: "Sân trường - 10:00 AM"
Background Image: [schoolyard_bg.png]
Characters: Array size = 1
  Element 0:
    Character Sprite: [vu_angry_sprite.png]
    Character Name: "Vũ"
    Position: Center
    Scale: (1.1, 1.1)  # Lớn hơn để tạo cảm giác đe dọa
Dialogue: Day1_Scene5_Schoolyard_Dialogue
Next Scene: (null)  ← Cảnh cuối của Day 1
Return To Top Down: true  ← Quay về top-down mode
Top Down Scene Name: "SchoolScene"
Spawn Point Id: "courtyard_spawn"

Set Flags On Enter:
  - "met_bullies"
```

---

## 🔗 TẠO VN SEQUENCE CHO CẢ DAY 1

### Tạo VNSequenceData: `Day1_Complete_Sequence`

```
Right-click → Create → Visual Novel → VN Sequence
Tên: "Day1_Complete_Sequence"
```

### Cấu hình:

```yaml
Sequence Name: "Day 1 - Complete"
Description: "Ngày đầu tiên của Đức ở trường mới"

Scenes: Array size = 5
  Element 0: Day1_Scene1_Bedroom
  Element 1: Day1_Scene2_LivingRoom
  Element 2: Day1_Scene3_SchoolGate
  Element 3: Day1_Scene4_Classroom
  Element 4: Day1_Scene5_Schoolyard

Day Number: 1
Time Of Day: Morning

Required Flags: (không có)
Forbidden Flags:
  - "day1_completed"

Set Flags On Complete:
  - "day1_completed"
```

---

## 🎮 SỬ DỤNG SEQUENCE

### Cách 1: Chơi toàn bộ sequence

```csharp
public VNSequenceData day1Sequence;

void Start()
{
    VNSequenceManager.PlaySequence(day1Sequence, OnDay1Complete);
}

void OnDay1Complete()
{
    Debug.Log("Day 1 hoàn thành!");
    // Load scene tiếp theo hoặc trigger event
}
```

### Cách 2: Chơi từng cảnh riêng lẻ

```csharp
// Chỉ chơi cảnh 1
VisualNovelManager.Instance.StartVNScene(scene1Data);
```

---

## 📊 TỔNG KẾT

**Bạn đã tạo:**

- ✅ 5 DialogueData cho Day 1
- ✅ 5 VNSceneData tương ứng
- ✅ 1 VNSequenceData cho toàn bộ Day 1
- ✅ Branching story với choices (cảnh 5)
- ✅ Story flags + variables tracking

**Story flags được set:**

- `day1_started` (Cảnh 1)
- `met_bullies` (Cảnh 5)
- `first_encounter_submit` hoặc `first_encounter_avoid` (Cảnh 5)
- `day1_completed` (Khi hoàn thành sequence)

**Variables thay đổi:**

- `CURRENT_DAY` = 1
- `FEAR_LEVEL` += 5 hoặc 10

---

✨ **Tiếp theo**: Tạo Day 2, Day 3... cho đến Bad/Good/True Ending!
