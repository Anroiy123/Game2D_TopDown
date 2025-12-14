Tôi sẽ đọc và phân tích các file documentation để tạo hướng dẫn chi tiết cho NPC "Mẹ Đức".

Tuyệt vời! Tôi thấy đã có folder `Mom` với sprites đầy đủ. Bây giờ tôi sẽ tạo hướng dẫn chi tiết để tạo NPC "Mẹ Đức".

---

# 📋 HƯỚNG DẪN TẠO NPC "MẸ ĐỨC" - STEP BY STEP

## 📊 PHÂN TÍCH NHÂN VẬT MẸ ĐỨC

### 🎭 Tính cách & Vai trò trong cốt truyện

| Khía cạnh           | Mô tả                                                               |
| ------------------- | ------------------------------------------------------------------- |
| **Tính cách**       | Lo lắng, quan tâm con, nhắc nhở nhiều, nhạy cảm với thái độ của con |
| **Vai trò**         | Người mẹ yêu thương, là chìa khóa cho **True Ending** (END 2)       |
| **Vị trí**          | Phòng khách HomeScene - đứng gần cửa ra vào                         |
| **Tương tác chính** | Hỏi thăm Đức mỗi ngày, nhắc nhở kết bạn, nhận ra con có vấn đề      |

### 🔑 Các tình huống tương tác quan trọng

| Ngày                   | Tình huống           | Dialogue chính                                                   | Story Flags liên quan |
| ---------------------- | -------------------- | ---------------------------------------------------------------- | --------------------- |
| **Ngày 1 (Sáng)**      | Đánh thức Đức đi học | "Đức ơi dậy đi con, nay buổi đầu đi nhận lớp đấy..."             | `day_1_completed`     |
| **Ngày 1 (Chiều)**     | Hỏi thăm ngày đầu    | "Nay đi học có vui không con, làm quen với thầy cô..."           | -                     |
| **Ngày 7 (Chiều)**     | Nhắc nhở kết bạn     | "Mày đi học về rồi đấy à, nay có gì mới không con..."            | `rejected_classmate`  |
| **Ngày 8+ (Chiều)**    | Nhận ra Đức lạ       | "Ơ hay nhỉ, thằng này hôm nay đi về không thèm thưa ai..."       | `mom_worried`         |
| **Giai đoạn 2 (Sáng)** | Xem TV + nhắc nhở    | "Nay đi học nhớ tập trung học đấy, với cả tìm đứa nào dễ gần..." | -                     |
| **Ngày cuối (Sáng)**   | ⚠️ QUAN TRỌNG        | "Chiều về sớm mẹ có chuyện muốn nói"                             | -                     |
| **Ngày cuối (Chiều)**  | 🔴 CHOICE POINT      | **[Thú nhận]** → True Ending / **[Giấu mẹ]** → Bad Endings       | `confessed_to_mom`    |

---

## ✅ CHECKLIST TẠO NPC MẸ ĐỨC

### 📦 BƯỚC 1: Tạo GameObject & Sprite

- [ ] **1.1. Tạo GameObject trong HomeScene**

  ```
  Hierarchy → Right-click → Create Empty
  Tên: "Mom" (hoặc "MeDuc")
  Position: Gần cửa phòng khách (VD: X: 5, Y: 3, Z: 0)
  ```

- [ ] **1.2. Add SpriteRenderer**

  ```
  Add Component → Rendering → Sprite Renderer
  Sprite: Assets/Sprites/Characters/Mom/Mom_idle.png (frame đầu tiên)
  Sorting Layer: "Characters" (hoặc tạo mới nếu chưa có)
  Order in Layer: 5
  ```

- [ ] **1.3. Add Collider2D**

  ```
  Add Component → Physics 2D → Box Collider 2D
  Is Trigger: ✅ (checked)
  Size: Điều chỉnh vừa với sprite (VD: 0.8 x 1.0)
  ```

- [ ] **1.4. Add Tag "NPC"**
  ```
  Inspector → Tag → Add Tag... → "NPC" (nếu chưa có)
  Chọn GameObject Mom → Tag: NPC
  ```

---

### 🎬 BƯỚC 2: Tạo Animator Controller

- [ ] **2.1. Tạo Animator Controller**

  ```
  Project → Assets/Data/ → Right-click → Create → Animator Controller
  Tên: "Mom_Animator"
  ```

- [ ] **2.2. Tạo Animation Clips**

  **Idle Animation:**

  ```
  Project → Right-click → Create → Animation
  Tên: "Mom_Idle_Down"

  Kéo sprite Mom_idle.png vào Animation window
  Slice sprite thành 4 frames (Down, Up, Side_Left, Side_Right)
  Tạo animation loop cho hướng Down (2-3 frames)
  ```

  **Walk Animation:**

  ```
  Tạo: "Mom_Walk_Down", "Mom_Walk_Up", "Mom_Walk_Side"
  Sử dụng sprite: Mom_walk.png
  Frame rate: 8-10 FPS
  ```

- [ ] **2.3. Setup Animator Controller**

  ```
  Mở Mom_Animator
  Tạo Parameters:
    - Speed (Float)
    - Horizontal (Float)
    - Vertical (Float)

  Tạo Blend Tree cho Idle và Walk (giống PlayerMovement)
  Transitions: Idle ↔ Walk dựa trên Speed > 0.01
  ```

- [ ] **2.4. Add Animator Component**
  ```
  GameObject Mom → Add Component → Animator
  Controller: Mom_Animator
  ```

---

### 💬 BƯỚC 3: Tạo DialogueData Assets

#### 📄 **3.1. Dialogue Ngày 1 - Sáng (Đánh thức)**

- [ ] **Tạo DialogueData Asset**

  ```
  Project → Assets/Data/ → Right-click → Create → DialogueData
  Tên: "Mom_Day1_Morning"
  ```

- [ ] **Cấu hình Nodes:**
  ```
  Node 0:
    Speaker Name: "Mẹ Đức"
    Is Player Speaking: ❌
    Dialogue Lines:
      - "Đức ơi dậy đi con, nay buổi đầu đi nhận lớp đấy."
      - "Dậy ăn sáng rồi đi không kẻo muộn."
    Next Node ID: -1 (kết thúc)
    Set Flags On Enter: (empty)
  ```

#### 📄 **3.2. Dialogue Ngày 1 - Chiều (Hỏi thăm)**

- [ ] **Tạo Asset: "Mom_Day1_Evening"**

  ```
  Node 0:
    Speaker Name: "Mẹ Đức"
    Dialogue Lines:
      - "Nay đi học có vui không con?"
      - "Làm quen với thầy cô với lớp học chưa?"
      - "Đã kết bạn được với ai chưa?"
    Next Node ID: 1

  Node 1:
    Speaker Name: "Đức"
    Is Player Speaking: ✅
    Dialogue Lines:
      - "Mẹ cứ từ từ đã, sao hỏi dồn dập thế."
      - "Nay con đi học bình thường, mẹ không cần gì phải lo cả đâu."
    Next Node ID: -1
  ```

#### 📄 **3.3. Dialogue Ngày 7 - Chiều (Nhắc nhở)**

- [ ] **Tạo Asset: "Mom_Day7_Evening"**

  ```
  Node 0:
    Speaker Name: "Mẹ Đức"
    Dialogue Lines:
      - "Mày đi học về rồi đấy à, nay có gì mới không con?"
      - "Học được 1 tuần rồi đã thân quen với đứa nào chưa?"
    Next Node ID: 1

  Node 1:
    Speaker Name: "Đức"
    Is Player Speaking: ✅
    Dialogue Lines:
      - "Con chưa, mẹ cứ kệ con đi."
    Next Node ID: 2

  Node 2:
    Speaker Name: "Mẹ Đức"
    Dialogue Lines:
      - "Tao đã nói với mày rồi, đã đi học thì phải kết bạn..."
      - "Huống chi mày còn mới chuyển trường tới đây, chân ướt chân ráo không biết gì thì lại càng cần bạn bè hơn..."
    Next Node ID: -1
    Set Flags On Enter: ["mom_worried"]
  ```

#### 📄 **3.4. Dialogue Ngày Cuối - Sáng (Nhắc về sớm)**

- [ ] **Tạo Asset: "Mom_FinalDay_Morning"**
  ```
  Node 0:
    Speaker Name: "Mẹ Đức"
    Dialogue Lines:
      - "Chiều về sớm mẹ có chuyện muốn nói."
    Next Node ID: -1
  ```

#### 📄 **3.5. Dialogue Ngày Cuối - Chiều (CHOICE POINT) ⚠️ QUAN TRỌNG**

- [ ] **Tạo Asset: "Mom_FinalDay_Evening_Choice"**

  ```
  Node 0:
    Speaker Name: "Mẹ Đức"
    Dialogue Lines:
      - "Con về rồi à? Mẹ thấy con dạo này có vẻ không được khỏe lắm."
      - "Con có chuyện gì muốn nói với mẹ không?"
    Choices:
      Choice 1:
        Text: "Thú nhận với mẹ về việc bị bắt nạt"
        Next Node ID: 10 (True Ending branch)
        Set Flags True: ["confessed_to_mom"]
        Required Flags: ["got_beaten"]

      Choice 2:
        Text: "Giấu mẹ, nói dối bị té"
        Next Node ID: 20 (Bad Ending branch)
        Set Flags True: ["lied_to_mom"]

  Node 10 (True Ending):
    Speaker Name: "Đức"
    Is Player Speaking: ✅
    Dialogue Lines:
      - "Con... con bị bọn chúng bắt nạt mẹ ạ..."
      - "Con không muốn đi học nữa..." (khóc)
    Next Node ID: 11

  Node 11:
    Speaker Name: "Mẹ Đức"
    Dialogue Lines:
      - "Sao con không nói với mẹ sớm hơn?"
      - "Thôi được rồi, mẹ sẽ lo cho con."
      - "Con nghỉ học 1 tuần, mẹ sẽ bàn với bố chuyển trường và báo nhà trường."
    Next Node ID: -1
    Action ID: "trigger_true_ending"

  Node 20 (Bad Ending branch):
    Speaker Name: "Đức"
    Is Player Speaking: ✅
    Dialogue Lines:
      - "Không có gì đâu mẹ, con bị té thôi."
    Next Node ID: 21

  Node 21:
    Speaker Name: "Mẹ Đức"
    Dialogue Lines:
      - "Vậy à? Mẹ thấy con có vẻ không ổn lắm..."
      - "Thôi được, con lên phòng nghỉ ngơi đi."
    Next Node ID: -1
  ```

---

### 🔧 BƯỚC 4: Add NPCInteraction Component

- [ ] **4.1. Add Component**

  ```
  GameObject Mom → Add Component → NPCInteraction
  ```

- [ ] **4.2. Cấu hình NPCInteraction**

  ```
  NPC Name: "Mẹ Đức"
  Interaction Range: 2.0
  Use Advanced Dialogue: ✅ (checked)

  Dialogue Data: (để trống, sẽ set động qua script)

  Show Name On Approach: ✅
  Face Player On Interact: ✅
  ```

---

### 🎯 BƯỚC 5: Tạo Script Quản Lý Dialogue Động

Vì Mẹ Đức có nhiều dialogue khác nhau tùy theo ngày và story flags, cần tạo script riêng:

- [ ] **5.1. Tạo Script "MomDialogueController.cs"**

```csharp
using UnityEngine;

public class MomDialogueController : MonoBehaviour
{
    [Header("Dialogue Assets")]
    [SerializeField] private DialogueData day1Morning;
    [SerializeField] private DialogueData day1Evening;
    [SerializeField] private DialogueData day7Evening;
    [SerializeField] private DialogueData finalDayMorning;
    [SerializeField] private DialogueData finalDayEveningChoice;
    [SerializeField] private DialogueData defaultDialogue;

    private NPCInteraction npcInteraction;

    void Start()
    {
        npcInteraction = GetComponent<NPCInteraction>();
        UpdateDialogue();
    }

    void OnEnable()
    {
        // Update dialogue mỗi khi scene load
        Invoke(nameof(UpdateDialogue), 0.1f);
    }

    public void UpdateDialogue()
    {
        int currentDay = StoryManager.Instance.GetCurrentDay();
        string timeOfDay = StoryManager.Instance.GetVariable("time_of_day") == 0 ? "morning" : "evening";

        DialogueData dialogueToUse = null;

        // Ngày 1
        if (currentDay == 1)
        {
            dialogueToUse = timeOfDay == "morning" ? day1Morning : day1Evening;
        }
        // Ngày 7
        else if (currentDay == 7 && timeOfDay == "evening")
        {
            dialogueToUse = day7Evening;
        }
        // Ngày cuối (14)
        else if (currentDay >= 14)
        {
            dialogueToUse = timeOfDay == "morning" ? finalDayMorning : finalDayEveningChoice;
        }
        // Default
        else
        {
            dialogueToUse = defaultDialogue;
        }

        if (dialogueToUse != null && npcInteraction != null)
        {
            npcInteraction.SetDialogueData(dialogueToUse);
        }
    }
}
```

- [ ] **5.2. Add Component vào GameObject Mom**

  ```
  Add Component → MomDialogueController

  Kéo các DialogueData assets vào slots:
    - Day1 Morning: Mom_Day1_Morning
    - Day1 Evening: Mom_Day1_Evening
    - Day7 Evening: Mom_Day7_Evening
    - Final Day Morning: Mom_FinalDay_Morning
    - Final Day Evening Choice: Mom_FinalDay_Evening_Choice
    - Default Dialogue: (tạo dialogue chung chung)
  ```

---

### 🏠 BƯỚC 6: Đặt NPC trong HomeScene

- [ ] **6.1. Vị trí đặt Mẹ Đức**

  ```
  Scene: HomeScene
  Vị trí: Phòng khách, gần cửa ra vào

  Gợi ý position:
    - Gần cửa chính (để đón Đức về)
    - Hoặc gần bếp (đang nấu ăn)
    - Hoặc gần sofa (đang xem TV)

  Transform:
    Position: (tùy theo layout HomeScene của bạn)
    Rotation: (0, 0, 0)
    Scale: (1, 1, 1)
  ```

- [ ] **6.2. Tạo Name Canvas (World Space)**

  ```
  GameObject Mom → Right-click → UI → Canvas
  Tên: "NameCanvas"

  Canvas:
    Render Mode: World Space
    Sorting Layer: UI
    Order in Layer: 10

  Canvas Scaler:
    Dynamic Pixels Per Unit: 10

  RectTransform:
    Width: 100
    Height: 30
    Position Y: 1.5 (phía trên đầu NPC)
  ```

- [ ] **6.3. Add Text cho tên**

  ```
  NameCanvas → Right-click → UI → Text - TextMeshPro
  Tên: "NameText"

  Text: "Mẹ Đức"
  Font Size: 12
  Alignment: Center
  Color: White

  Outline: ✅ (để dễ đọc)
  ```

---

### 🎮 BƯỚC 7: Cập nhật StoryManager

- [ ] **7.1. Thêm Story Flags mới (nếu chưa có)**

Mở `Assets/Scripts/Core/StoryManager.cs` và thêm vào class `FlagKeys`:

```csharp
public static class FlagKeys
{
    // ... existing flags ...

    // Mom-related flags
    public const string MOM_WORRIED = "mom_worried";
    public const string CONFESSED_TO_MOM = "confessed_to_mom";
    public const string LIED_TO_MOM = "lied_to_mom";
}
```

- [ ] **7.2. Thêm Variable cho time_of_day**

```csharp
public static class VarKeys
{
    // ... existing vars ...

    public const string TIME_OF_DAY = "time_of_day"; // 0 = morning, 1 = evening
}
```

- [ ] **7.3. Initialize trong Awake()**

```csharp
void Awake()
{
    // ... existing code ...

    // Initialize time of day
    if (!storyVariables.ContainsKey(VarKeys.TIME_OF_DAY))
    {
        storyVariables[VarKeys.TIME_OF_DAY] = 0; // morning
    }
}
```

---

### 🔗 BƯỚC 8: Tích hợp với Scene Transition

- [ ] **8.1. Cập nhật time_of_day khi chuyển scene**

Tạo script `TimeOfDayManager.cs`:

```csharp
using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    void Start()
    {
        // Khi vào HomeScene từ ClassroomScene = chiều
        string previousScene = GameManager.Instance.GetPreviousSceneName();

        if (previousScene == "ClassroomScene" || previousScene == "StreetScene")
        {
            StoryManager.Instance.SetVariable(StoryManager.VarKeys.TIME_OF_DAY, 1); // evening
        }
        else
        {
            StoryManager.Instance.SetVariable(StoryManager.VarKeys.TIME_OF_DAY, 0); // morning
        }

        // Trigger update dialogue cho Mom
        MomDialogueController mom = FindFirstObjectByType<MomDialogueController>();
        if (mom != null)
        {
            mom.UpdateDialogue();
        }
    }
}
```

- [ ] **8.2. Add vào HomeScene**
  ```
  Hierarchy → Create Empty → "TimeOfDayManager"
  Add Component → TimeOfDayManager
  ```

---

### 🎨 BƯỚC 9: Polish & Testing

- [ ] **9.1. Test Dialogue Flow**

  ```
  ✅ Ngày 1 sáng: Mẹ đánh thức
  ✅ Ngày 1 chiều: Mẹ hỏi thăm
  ✅ Ngày 7 chiều: Mẹ nhắc nhở
  ✅ Ngày 14 sáng: Mẹ nhắc về sớm
  ✅ Ngày 14 chiều: Choice point (Thú nhận/Giấu)
  ```

- [ ] **9.2. Test Story Flags**

  ```
  Console → StoryManager.Instance.SetFlag("got_beaten", true)
  → Choice "Thú nhận" phải xuất hiện

  Console → StoryManager.Instance.SetFlag("confessed_to_mom", true)
  → Trigger True Ending
  ```

- [ ] **9.3. Test Animations**
  ```
  ✅ Idle animation loop
  ✅ Face player khi tương tác
  ✅ Name canvas hiển thị khi player đến gần
  ```

---

## 🚨 LƯU Ý QUAN TRỌNG

### ⚠️ Điểm cốt truyện cần implement chính xác:

1. **True Ending Trigger:**

   - Chỉ xuất hiện choice "Thú nhận" khi `got_beaten = true`
   - Set flag `confessed_to_mom = true` → Trigger ending sequence
   - Cần implement scene True Ending (mẹ báo công an, chuyển trường)

2. **Bad Ending Branch:**

   - Nếu chọn "Giấu mẹ" → Đức tâm lý bất ổn
   - Dẫn đến choice "Mang dao" hoặc "Không mang dao"
   - Cần implement 2 bad endings riêng

3. **Mom Worried Flag:**

   - Set khi Đức về không chào hỏi (ngày 8+)
   - Ảnh hưởng đến dialogue sau này

4. **Time of Day System:**
   - Sáng: Mẹ ở nhà, dialogue đánh thức/nhắc nhở
   - Chiều: Mẹ đón Đức về, hỏi thăm
   - Cần sync với scene transitions

---

## 📝 CHECKLIST TỔNG HỢP

### Tài nguyên cần có:

- [x] Sprites: `Assets/Sprites/Characters/Mom/` (đã có)
- [ ] DialogueData assets: 5-6 files
- [ ] Animator Controller: Mom_Animator
- [ ] Animation Clips: Idle, Walk (4 hướng)
- [ ] Scripts: MomDialogueController, TimeOfDayManager

### Components trên GameObject Mom:

- [ ] SpriteRenderer
- [ ] BoxCollider2D (Is Trigger)
- [ ] Animator
- [ ] NPCInteraction
- [ ] MomDialogueController
- [ ] Tag: "NPC"

### Story System:

- [ ] Flags: `mom_worried`, `confessed_to_mom`, `lied_to_mom`
- [ ] Variable: `time_of_day`
- [ ] Ending logic: True Ending trigger

---

## 🎯 BƯỚC TIẾP THEO SAU KHI HOÀN THÀNH

1. **Test đầy đủ flow:**

   - Chơi từ ngày 1 → ngày 14
   - Test cả 2 nhánh: Thú nhận & Giấu mẹ

2. **Tạo Ending Scenes:**

   - True Ending scene (mẹ báo công an)
   - Bad Ending scenes (án mạng/tử vong)

3. **Tạo các NPC khác:**
   - Bố Đức (xuất hiện trong True Ending)
   - Cô giáo
   - Nhóm bắt nạt

Bạn muốn tôi giúp implement phần nào trước? Tôi có thể:

- Tạo các DialogueData assets
- Viết script MomDialogueController
- Tạo Animator Controller
- Hoặc hướng dẫn chi tiết hơn bất kỳ bước nào
