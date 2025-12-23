# 🛠️ HƯỚNG DẪN IMPLEMENT SCENE 27-28 VÀ ENDINGS

## 📋 CHECKLIST CÔNG VIỆC

### ✅ Đã hoàn thành:
- [x] Tạo JSON cho Scene 27 (3 lựa chọn)
- [x] Tạo JSON cho Scene 28A (Ending 1)
- [x] Tạo JSON cho Scene 28B (Ending 2/3)

### 🔲 Cần làm tiếp:

#### 1. Import JSON vào Unity
- [ ] Import `CriticalDay_Scene27_Street_NEW.json`
- [ ] Import `CriticalDay_Scene28A_Home_AfterWin.json`
- [ ] Import `CriticalDay_Scene28B_Home_Choice.json`

#### 2. Implement Action IDs trong DialogueSystem
- [ ] `trigger_fight_cutscene` - Bắt đầu FightCutscene
- [ ] `trigger_scene28b` - Chuyển sang Scene 28B
- [ ] `trigger_ending1_storytelling` - Ending 1
- [ ] `trigger_ending2_storytelling` - Ending 2
- [ ] `trigger_ending3_storytelling` - Ending 3

#### 3. Tạo FightCutscene
- [ ] Tạo VNSequenceData cho fight cutscene
- [ ] Tạo sprites/backgrounds cho fight scene
- [ ] Implement logic chuyển sang Scene 28A sau fight

#### 4. Tạo Storytelling Endings
- [ ] Tạo VNSequenceData cho Ending 1 (Good_StandUp)
- [ ] Tạo VNSequenceData cho Ending 2 (True_TellParents)
- [ ] Tạo VNSequenceData cho Ending 3 (Bad_DarkLife)
- [ ] Tạo sprites/backgrounds cho endings

---

## 🎬 CHI TIẾT IMPLEMENT

### 1. FIGHT CUTSCENE

**Mục đích**: Hiển thị cảnh đánh nhau giữa Đức và Thủ lĩnh

**Cách implement**:

#### Option A: Sử dụng VNSequenceData (Đơn giản)
```
Tạo VNSequenceData: "FightCutscene"
├── VNScene 1: Đức và Thủ lĩnh đối mặt
├── VNScene 2: Đánh nhau (hiệu ứng)
├── VNScene 3: Đức thắng
└── returnToTopDown = true → Scene 28A (HomeScene)
```

#### Option B: Sử dụng Animation/Timeline (Phức tạp hơn)
- Tạo Timeline với animation đánh nhau
- Sau khi kết thúc → Trigger chuyển scene

**Recommended**: Option A (VNSequenceData) vì đơn giản và phù hợp với storytelling style

---

### 2. STORYTELLING ENDINGS

Theo `docs/newEnding.md`, mỗi ending cần:

#### ENDING 1: Good_StandUp - "Sẽ có những con cá phải giả chó"

**Nội dung** (từ `docs/story.md` line 700-800):
```
VNSequenceData: "Ending1_Good_StandUp"
├── VNScene 1: Một tuần sau - Đức và mẹ gặp hiệu trưởng
├── VNScene 2: Tụi bắt nạt bị kỷ luật
├── VNScene 3: Đức quay lại trường
├── VNScene 4: Đức kết bạn với bạn cùng lớp
├── VNScene 5: Một năm sau - Đức tự tin hơn
└── VNScene 6: Message kết thúc
```

**Message**: "Đứng lên chống lại bạo lực và tìm kiếm sự giúp đỡ là điều đúng đắn."

---

#### ENDING 2: True_TellParents - "Gia đình là điểm tựa"

**Nội dung** (từ `docs/story.md` line 800-900):
```
VNSequenceData: "Ending2_True_TellParents"
├── VNScene 1: Mẹ gọi cho cô giáo
├── VNScene 2: Nhà trường can thiệp
├── VNScene 3: Tụi bắt nạt bị xử lý
├── VNScene 4: Đức được hỗ trợ tâm lý
├── VNScene 5: Một năm sau - Đức hồi phục
└── VNScene 6: Message kết thúc
```

**Message**: "Chia sẻ với gia đình là điều quan trọng nhất khi bị bắt nạt."

---

#### ENDING 3: Bad_DarkLife - "Cuộc đời đen tối"

**Nội dung** (từ `docs/story.md` line 900-960):
```
VNSequenceData: "Ending3_Bad_DarkLife"
├── VNScene 1: Một tuần sau - Đức vẫn bị bắt nạt
├── VNScene 2: Một tháng sau - Đức nghỉ học
├── VNScene 3: Sáu tháng sau - Đức có suy nghĩ tiêu cực
├── VNScene 4: Một năm sau - Mẹ tự trách
├── VNScene 5: Nhiều năm sau - Đức trưởng thành nhưng mang vết thương tâm lý
└── VNScene 6: Message kết thúc
```

**Message**: "Im lặng chịu đựng bạo lực có thể hủy hoại cả cuộc đời bạn."

---

## 🔧 CODE IMPLEMENTATION

### DialogueSystem.cs - ProcessActionId()

```csharp
private void ProcessActionId(string actionId)
{
    if (string.IsNullOrEmpty(actionId)) return;

    switch (actionId)
    {
        case "trigger_fight_cutscene":
            // Trigger FightCutscene
            StartFightCutscene();
            break;

        case "trigger_scene28b":
            // Chuyển sang Scene 28B (HomeScene)
            TransitionToScene28B();
            break;

        case "trigger_ending1_storytelling":
            // Bắt đầu Storytelling Ending 1
            StartEnding1();
            break;

        case "trigger_ending2_storytelling":
            // Bắt đầu Storytelling Ending 2
            StartEnding2();
            break;

        case "trigger_ending3_storytelling":
            // Bắt đầu Storytelling Ending 3
            StartEnding3();
            break;

        default:
            Debug.LogWarning($"Unknown action ID: {actionId}");
            break;
    }
}

private void StartFightCutscene()
{
    // Load VNSequenceData cho FightCutscene
    VNSequenceData fightSequence = Resources.Load<VNSequenceData>("VNSequences/FightCutscene");
    if (fightSequence != null)
    {
        VisualNovelManager.Instance.StartVNSequence(fightSequence, () =>
        {
            // Sau khi kết thúc fight → Chuyển sang Scene 28A
            GameManager.Instance.LoadScene("HomeScene", "after_fight");
        });
    }
}

private void TransitionToScene28B()
{
    // Chuyển sang HomeScene với spawn point "after_beaten"
    GameManager.Instance.LoadScene("HomeScene", "after_beaten");
}

private void StartEnding1()
{
    VNSequenceData ending1 = Resources.Load<VNSequenceData>("VNSequences/Ending1_Good_StandUp");
    if (ending1 != null)
    {
        VisualNovelManager.Instance.StartVNSequence(ending1, () =>
        {
            // Sau khi kết thúc → Về main menu hoặc credits
            GameManager.Instance.LoadScene("MainMenu");
        });
    }
}

// Tương tự cho StartEnding2() và StartEnding3()
```

---

## 📁 CẤU TRÚC THƯ MỤC ĐỀ XUẤT

```
Assets/
├── Data/
│   ├── Dialogues/
│   │   └── CriticalDay/
│   │       ├── CriticalDay_Scene27_Street_NEW.json ✅
│   │       ├── CriticalDay_Scene28A_Home_AfterWin.json ✅
│   │       └── CriticalDay_Scene28B_Home_Choice.json ✅
│   │
│   └── VisualNovel/
│       ├── Sequences/
│       │   ├── FightCutscene.asset (TODO)
│       │   ├── Ending1_Good_StandUp.asset (TODO)
│       │   ├── Ending2_True_TellParents.asset (TODO)
│       │   └── Ending3_Bad_DarkLife.asset (TODO)
│       │
│       └── Scenes/
│           ├── Fight_Scene1.asset (TODO)
│           ├── Fight_Scene2.asset (TODO)
│           ├── Ending1_Scene1.asset (TODO)
│           └── ... (TODO)
```

---

## 🎨 ASSETS CẦN TẠO

### Sprites/Backgrounds:
- [ ] Fight scene backgrounds (đường phố, góc tối)
- [ ] Character sprites cho fight (Đức, Thủ lĩnh)
- [ ] Ending backgrounds (trường học, nhà, phòng ngủ, v.v.)
- [ ] Silhouette sprites cho Ending 3

### Audio:
- [ ] BGM cho fight scene (căng thẳng)
- [ ] BGM cho Ending 1 (hy vọng)
- [ ] BGM cho Ending 2 (ấm áp)
- [ ] BGM cho Ending 3 (u ám)
- [ ] SFX cho fight (đấm, đá, v.v.)

---

## 🚀 NEXT STEPS

1. **Import JSON vào Unity** (5 phút)
2. **Test dialogue flow** trong Unity Editor (10 phút)
3. **Implement action IDs** trong DialogueSystem.cs (30 phút)
4. **Tạo FightCutscene VNSequenceData** (1-2 giờ)
5. **Tạo 3 Ending VNSequenceData** (3-4 giờ)
6. **Test toàn bộ flow** (30 phút)

**Tổng thời gian ước tính**: 5-7 giờ

