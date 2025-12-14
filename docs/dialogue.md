## 📊 PHÂN TÍCH HỆ THỐNG HỘI THOẠI & NPC

### 📁 Các File Liên Quan

| File                                            | Chức năng                                                                    |
| ----------------------------------------------- | ---------------------------------------------------------------------------- |
| `Assets/Scripts/Dialogue/DialogueSystem.cs`     | Quản lý hiển thị UI hội thoại, xử lý logic chuyển đổi dialogue               |
| `Assets/Scripts/Dialogue/DialogueData.cs`       | Định nghĩa cấu trúc dữ liệu cho dialogue (ScriptableObject)                  |
| `Assets/Scripts/NPC/NPCInteraction.cs`          | Xử lý tương tác giữa Player và NPC                                           |
| `Assets/Scripts/Player/PlayerMovement.cs`       | Điều khiển player, có tích hợp trạng thái nói chuyện                         |
| `Assets/Scripts/Editor/DialogueJsonImporter.cs` | **[NEW]** Tool import JSON thành DialogueData                                |
| `Assets/Scripts/Data/AdamDialogue.asset`        | Instance của DialogueData cho NPC Adam                                       |
| `Assets/Scripts/Data/Dialogues/*.json`          | **[NEW]** JSON dialogue files (example_dialogue.json, advanced_example.json) |
| `Assets/Prefabs/Adam.prefab`                    | Prefab NPC Adam                                                              |
| `Assets/Prefabs/ChoiceButton.prefab`            | Prefab button cho lựa chọn dialogue                                          |

---

### 🏗️ KIẾN TRÚC HỆ THỐNG

```
┌─────────────────────────────────────────────────────────────────┐
│                         DATA LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│  DialogueData (ScriptableObject)                                │
│  ├── conversationName: string                                   │
│  ├── startNodeId: int                                           │
│  └── nodes: DialogueNode[]                                      │
│       ├── nodeId: int                                           │
│       ├── speakerName: string                                   │
│       ├── isPlayerSpeaking: bool                                │
│       ├── dialogueLines: string[]                               │
│       ├── choices: DialogueChoice[]                             │
│       │    ├── choiceText: string                               │
│       │    ├── nextNodeId: int                                  │
│       │    └── actionId: string (callback trigger)              │
│       └── nextNodeId: int (-1 = end)                            │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                       CONTROLLER LAYER                          │
├─────────────────────────────────────────────────────────────────┤
│  NPCInteraction                                                 │
│  ├── Detect player trong interactionRange                       │
│  ├── Hiển thị nameUI (World Space Canvas)                       │
│  ├── Xử lý input phím E để bắt đầu dialogue                     │
│  ├── FacePlayer() - Quay NPC nhìn về Player                     │
│  ├── Gọi DialogueSystem để bắt đầu hội thoại                    │
│  └── OnDialogueAction() - Xử lý action callbacks                │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                          UI LAYER                               │
├─────────────────────────────────────────────────────────────────┤
│  DialogueSystem (Screen Space UI)                               │
│  ├── dialoguePanel: GameObject                                  │
│  ├── speakerNameText: Text                                      │
│  ├── dialogueText: Text                                         │
│  ├── continueIcon: GameObject                                   │
│  ├── choicePanel: GameObject                                    │
│  ├── choiceButtonPrefab: GameObject                             │
│  └── choiceContainer: Transform                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

### 🔄 LUỒNG DỮ LIỆU CHI TIẾT

#### **1. Khởi tạo (Start)**

```
Scene Load
    │
    ├─► NPCInteraction.Start()
    │   ├── FindGameObjectWithTag("Player") → player
    │   ├── FindObjectOfType<DialogueSystem>() → dialogueSystem
    │   └── nameUI.SetActive(false)
    │
    └─► DialogueSystem.Start()
        ├── dialoguePanel.SetActive(false)
        ├── continueIcon.SetActive(false)
        ├── speakerNameText.SetActive(false)
        └── choicePanel.SetActive(false)
```

#### **2. Phát hiện Player (Update Loop)**

```
NPCInteraction.Update()
    │
    ├── Tính distance = Vector2.Distance(NPC, Player)
    │
    ├── IF distance <= interactionRange
    │   ├── playerInRange = true
    │   ├── playerMovement.SetNearNPC(true) ─► Ưu tiên NPC hơn ghế
    │   └── nameUI.SetActive(true) ─► Hiện tên NPC
    │
    └── IF Input.GetKeyDown(KeyCode.E) && !isTalking
        └── StartDialogue()
```

#### **3. Bắt đầu Hội thoại**

```
NPCInteraction.StartDialogue()
    │
    ├── isTalking = true
    ├── nameUI.SetActive(false)
    ├── FacePlayer() ─► Quay NPC về phía Player
    │   └── animator.SetFloat(Horizontal/Vertical)
    │
    ├── playerMovement.SetTalkingState(true) ─► Khóa di chuyển Player
    │
    └── IF useAdvancedDialogue && dialogueData != null
        ├── dialogueSystem.StartDialogueWithChoices(dialogueData, OnDialogueEnd, OnDialogueAction)
        └── [Advanced Mode - Có lựa chọn]

        ELSE
        ├── dialogueSystem.StartDialogue(npcName, dialogueLines, OnDialogueEnd)
        └── [Legacy Mode - Chỉ text tuần tự]
```

#### **4. Xử lý Dialogue (DialogueSystem)**

**Legacy Mode:**

```
StartDialogue(npcName, lines, callback)
    │
    ├── currentDialogueLines = lines
    ├── currentLineIndex = 0
    ├── dialogueActive = true
    │
    ├── dialoguePanel.SetActive(true)
    ├── speakerNameText.text = npcName
    │
    └── DisplayLine()
        └── TypeLine(line) ─► Typewriter effect
            └── foreach char → dialogueText += char → WaitForSeconds(textSpeed)
```

**Advanced Mode (With Choices):**

```
StartDialogueWithChoices(dialogueData, onComplete, onAction)
    │
    ├── currentDialogueData = dialogueData
    ├── dialogueActive = true
    │
    └── GoToNode(startNodeId)
        │
        ├── currentNode = dialogueData.GetNodeById(nodeId)
        │
        └── DisplayNodeLine()
            │
            ├── speakerNameText.text = currentNode.speakerName
            ├── speakerNameText.color = isPlayerSpeaking ? playerNameColor : npcNameColor
            │
            └── TypeLine(line)
                │
                └── CheckAndShowChoices()
                    │
                    ├── IF isLastLine && hasChoices
                    │   └── ShowChoices()
                    │       ├── choicePanel.SetActive(true)
                    │       └── CreateChoiceButton() for each choice
                    │
                    └── ELSE ShowContinueIcon()
```

#### **5. Xử lý Lựa chọn (Choice Selection)**

```
HandleChoiceInput() [Phím số 1-9] OR Button.onClick
    │
    └── OnChoiceSelected(nextNodeId, actionId)
        │
        ├── IF actionId != null
        │   └── onChoiceAction?.Invoke(actionId) ─► NPCInteraction.OnDialogueAction()
        │       ├── "give_item" → Debug.Log
        │       ├── "open_shop" → Debug.Log
        │       └── "start_quest" → Debug.Log
        │
        └── GoToNode(nextNodeId) ─► Tiếp tục dialogue hoặc kết thúc (-1)
```

#### **6. Kết thúc Hội thoại**

```
EndDialogue() [khi nextNodeId == -1 HOẶC nhấn ESC]
    │
    ├── dialogueActive = false
    ├── ClearChoiceButtons()
    ├── dialoguePanel.SetActive(false)
    ├── choicePanel.SetActive(false)
    │
    └── onDialogueComplete?.Invoke() ─► NPCInteraction.OnDialogueEnd()
        │
        ├── isTalking = false
        ├── playerMovement.SetTalkingState(false) ─► Mở khóa di chuyển
        │
        └── animator.SetFloat(Horizontal=0, Vertical=1) ─► Quay lưng về camera
```

---

### 📌 LUỒNG TÓM TẮT

```
Player tiến gần NPC (distance <= interactionRange)
    ↓
NPCInteraction hiển thị nameUI + SetNearNPC(true)
    ↓
Player nhấn E
    ↓
NPCInteraction.StartDialogue()
    ↓
PlayerMovement.SetTalkingState(true) → Khóa di chuyển
    ↓
NPC.FacePlayer() → Quay về phía Player
    ↓
DialogueSystem.StartDialogue() hoặc StartDialogueWithChoices()
    ↓
DialoguePanel hiện lên + Typewriter effect
    ↓
[Loop] Player nhấn E để tiếp tục / Nhấn số để chọn choice
    ↓
OnChoiceSelected() → Action callback (nếu có) + GoToNode()
    ↓
Khi nextNodeId = -1 → EndDialogue()
    ↓
OnDialogueEnd() → SetTalkingState(false) → Player di chuyển lại
```

---

### 🎯 ĐẶC ĐIỂM CHÍNH

| Tính năng              | Mô tả                                                            |
| ---------------------- | ---------------------------------------------------------------- |
| **Dual Mode**          | Hỗ trợ Legacy (text tuần tự) và Advanced (branching với choices) |
| **Typewriter Effect**  | Text hiện từng ký tự với tốc độ `textSpeed`                      |
| **Branching Dialogue** | Sử dụng DialogueNode với choices dẫn đến các node khác           |
| **Action Callbacks**   | `actionId` cho phép trigger event như give_item, open_shop       |
| **Input Support**      | Phím E để tiếp tục, số 1-9 để chọn, ESC để thoát                 |
| **NPC Animation**      | Quay mặt về phía player khi nói chuyện                           |
| **Player Lock**        | Khóa di chuyển player khi đang trong dialogue                    |
| **Priority System**    | NPC được ưu tiên hơn ghế khi cả hai gần nhau                     |

---

---

---

---

---

## 🆕 HƯỚNG DẪN SỬ DỤNG JSON IMPORT TOOL

### 🎯 TẠI SAO DÙNG JSON?

| Lợi ích                | Mô tả                                      |
| ---------------------- | ------------------------------------------ |
| ✅ **Dễ viết**         | Cú pháp đơn giản hơn Inspector UI          |
| ✅ **Dễ sửa**          | Mở bằng text editor, search/replace nhanh  |
| ✅ **Version Control** | Git diff/merge JSON dễ hơn .asset binary   |
| ✅ **Backup**          | Copy/paste, duplicate dễ dàng              |
| ✅ **External Tools**  | Có thể generate từ Excel, Google Sheets... |

### 📝 CÚ PHÁP JSON CHI TIẾT

#### **JSON Cơ bản:**

```json
{
  "conversationName": "Tên_Hội_Thoại",
  "startNodeId": 0,
  "nodes": [...]
}
```

#### **Node đơn giản (narrator/NPC nói):**

```json
{
  "id": 0,
  "speaker": "Mẹ",
  "isPlayer": false,
  "lines": ["Đức ơi, dậy đi con!", "Hôm nay là ngày đầu tiên đi học mà."],
  "next": 1
}
```

#### **Node với choices (player chọn):**

```json
{
  "id": 1,
  "speaker": "Đức",
  "isPlayer": true,
  "lines": ["..."],
  "choices": [
    {
      "text": "Chào mẹ",
      "next": 2,
      "setTrue": ["greeted_mom"]
    },
    {
      "text": "Im lặng",
      "next": 3
    }
  ]
}
```

#### **Node với conditions (nâng cao):**

```json
{
  "id": 5,
  "speaker": "Thằng Béo",
  "lines": ["Đưa tiền đi!"],
  "choices": [
    {
      "text": "Đưa tiền (10000đ)",
      "next": 6,
      "varConditions": [{ "name": "money", "op": ">=", "value": 10000 }],
      "varChanges": [
        { "name": "money", "op": "sub", "value": 10000 },
        { "name": "gave_money_count", "op": "add", "value": 1 }
      ],
      "setTrue": ["gave_money_to_bullies"]
    },
    {
      "text": "Từ chối",
      "next": 7,
      "requireFlags": ["met_bullies"],
      "forbidFlags": ["angered_bullies"]
    }
  ]
}
```

### 🔑 BẢNG TRA CỨU FIELDS

#### **Root Level:**

| Field              | Type   | Bắt buộc        | Mô tả           |
| ------------------ | ------ | --------------- | --------------- |
| `conversationName` | string | ✅              | Tên hội thoại   |
| `startNodeId`      | int    | ❌ (default: 0) | Node bắt đầu    |
| `nodes`            | array  | ✅              | Danh sách nodes |

#### **Node Fields:**

| Field        | Type     | Bắt buộc            | Mô tả                           |
| ------------ | -------- | ------------------- | ------------------------------- |
| `id`         | int      | ✅                  | ID node (unique)                |
| `speaker`    | string   | ❌                  | Tên người nói (rỗng = narrator) |
| `isPlayer`   | bool     | ❌ (default: false) | Player đang nói?                |
| `lines`      | string[] | ✅                  | Nội dung thoại                  |
| `next`       | int      | ❌                  | Node tiếp theo (-1 = end)       |
| `choices`    | array    | ❌                  | Lựa chọn (nếu có)               |
| `setFlags`   | string[] | ❌                  | Set flags TRUE khi vào node     |
| `varChanges` | array    | ❌                  | Thay đổi biến khi vào node      |

#### **Choice Fields:**

| Field           | Type     | Bắt buộc | Mô tả                |
| --------------- | -------- | -------- | -------------------- |
| `text`          | string   | ✅       | Text hiển thị        |
| `next`          | int      | ✅       | Node tiếp theo       |
| `action`        | string   | ❌       | Action ID (callback) |
| `requireFlags`  | string[] | ❌       | Flags CẦN có         |
| `forbidFlags`   | string[] | ❌       | Flags KHÔNG được có  |
| `setTrue`       | string[] | ❌       | Set flags TRUE       |
| `setFalse`      | string[] | ❌       | Set flags FALSE      |
| `varConditions` | array    | ❌       | Điều kiện biến       |
| `varChanges`    | array    | ❌       | Thay đổi biến        |

#### **Variable Condition:**

```json
{ "name": "money", "op": ">=", "value": 10000 }
```

| Field   | Values                           | Mô tả                                |
| ------- | -------------------------------- | ------------------------------------ |
| `name`  | string                           | Tên biến (VD: "money", "fear_level") |
| `op`    | `>`, `>=`, `<`, `<=`, `==`, `!=` | Phép so sánh                         |
| `value` | int                              | Giá trị so sánh                      |

#### **Variable Change:**

```json
{ "name": "money", "op": "sub", "value": 10000 }
```

| Field   | Values              | Mô tả                    |
| ------- | ------------------- | ------------------------ |
| `name`  | string              | Tên biến                 |
| `op`    | `set`, `add`, `sub` | Phép toán (gán/cộng/trừ) |
| `value` | int                 | Giá trị thay đổi         |

### 🚀 WORKFLOW GỢI Ý

#### **1. Viết JSON:**

```
📝 Dùng VS Code / Notepad++ / Editor yêu thích
   ↓
💾 Lưu vào Assets/Scripts/Data/Dialogues/MyDialogue.json
   ↓
📂 (Unity tự import thành TextAsset)
```

#### **2. Import vào Unity:**

```
🔧 Tools → Dialogue → Import JSON to DialogueData
   ↓
📄 Kéo JSON vào "JSON File"
   ↓
👁️ Nhấn "Preview JSON" (kiểm tra trước)
   ↓
✅ Nhấn "Import & Create DialogueData"
   ↓
🎯 File .asset được tạo tại Output Folder
```

#### **3. Sử dụng:**

```
🎮 Kéo DialogueData.asset vào NPCInteraction.dialogueData
   ↓
✅ Tick "Use Advanced Dialogue"
   ↓
▶️ Play game và test!
```

### 📚 FILE MẪU TRONG PROJECT

- **`example_dialogue.json`** - Ví dụ đơn giản (choices cơ bản)
- **`advanced_example.json`** - Ví dụ nâng cao (conditions, variables, flags)

### 🔧 TIPS & TRICKS

| Tip                        | Mô tả                                                             |
| -------------------------- | ----------------------------------------------------------------- |
| 💡 **Copy-paste nodes**    | Dễ dàng duplicate các nodes tương tự                              |
| 💡 **Search-replace**      | Thay speaker name hàng loạt                                       |
| 💡 **Comment trick**       | JSON không hỗ trợ comment, dùng field `_comment` (tool sẽ bỏ qua) |
| 💡 **Version control**     | Commit JSON files, dễ review changes trên GitHub                  |
| 💡 **External generation** | Viết script Python/Node.js generate JSON từ CSV/Excel             |

---

---

# 📋 DEEP DIVE ANALYSIS: Hệ thống Dialogue

## 🔄 FLOW TỔNG QUÁT (A → B → C)

```
┌──────────────────────────────────────────────────────────────────────────────┐
│  PLAYER gần NPC (distance <= interactionRange)                               │
│       ↓                                                                      │
│  NPCInteraction.Update() → playerInRange = true + nameUI hiện                │
│       ↓                                                                      │
│  Player nhấn E → NPCInteraction.StartDialogue()                              │
│       ↓                                                                      │
│  PlayerMovement.SetTalkingState(true) → Khóa di chuyển                       │
│       ↓                                                                      │
│  NPC.FacePlayer() → Animator quay hướng về player                            │
│       ↓                                                                      │
│  ┌─────────────────────────────────────────────────────────────┐             │
│  │ IF useAdvancedDialogue && dialogueData != null              │             │
│  │     └→ DialogueSystem.StartDialogueWithChoices()            │             │
│  │ ELSE                                                        │             │
│  │     └→ DialogueSystem.StartDialogue() [Legacy]              │             │
│  └─────────────────────────────────────────────────────────────┘             │
│       ↓                                                                      │
│  DialogueSystem.GoToNode(startNodeId)                                        │
│       ↓                                                                      │
│  DisplayNodeLine() → TypeLine(text) [Typewriter Effect]                      │
│       ↓                                                                      │
│  ┌─────────────────────────────────────────────────────────────┐             │
│  │ LOOP: Player nhấn E để tiếp tục                             │             │
│  │     ├─ IF isLastLine && hasChoices                          │             │
│  │     │       └→ ShowChoices() → Hiện choice buttons          │             │
│  │     │            └→ Player nhấn số 1-9 hoặc click           │             │
│  │     │                  └→ OnChoiceSelected(nextNodeId, actionId)          │
│  │     │                        ├─ onChoiceAction?.Invoke(actionId)          │
│  │     │                        └─ GoToNode(nextNodeId)        │             │
│  │     └─ ELSE                                                 │             │
│  │           └→ GoToNode(currentNode.nextNodeId)               │             │
│  └─────────────────────────────────────────────────────────────┘             │
│       ↓                                                                      │
│  KGHI nextNodeId == -1 → EndDialogue()                                       │
│       ↓                                                                      │
│  NPCInteraction.OnDialogueEnd() → PlayerMovement.SetTalkingState(false)      │
│       ↓                                                                      │
│  Player có thể di chuyển lại                                                 │
└──────────────────────────────────────────────────────────────────────────────┘
```

---

## 📁 CẤU TRÚC DỮ LIỆU HIỆN TẠI

```
DialogueData (ScriptableObject)
├── conversationName: string
├── startNodeId: int
└── nodes: DialogueNode[]
     ├── nodeId: int
     ├── speakerName: string
     ├── isPlayerSpeaking: bool
     ├── dialogueLines: string[]
     ├── choices: DialogueChoice[]
     │    ├── choiceText: string
     │    ├── nextNodeId: int
     │    └── actionId: string
     └── nextNodeId: int (-1 = kết thúc)
```

---

## ⚠️ NHỮNG GÌ THIẾU CHO GAME CỐT TRUYỆN RẼ NHÁNH

| #   | Thiếu                               | Mức độ quan trọng | Giải thích                                                                                     |
| --- | ----------------------------------- | ----------------- | ---------------------------------------------------------------------------------------------- |
| 1   | **🔴 Story State/Flags System**     | **RẤT CAO**       | Không có cách lưu trữ quyết định của player (VD: "đã cứu NPC A", "đã chọn đường xấu")          |
| 2   | **🔴 Conditional Choices**          | **RẤT CAO**       | Không thể ẩn/hiện lựa chọn dựa trên điều kiện (VD: chỉ hiện nếu có item X, hoặc flag Y = true) |
| 3   | **🔴 Conditional Node Branching**   | **RẤT CAO**       | Không thể chuyển tới node khác dựa trên điều kiện ngoài việc click choice                      |
| 4   | **🔴 Save/Load Dialogue Progress**  | **RẤT CAO**       | Không lưu tiến trình dialogue khi thoát game                                                   |
| 5   | **🟠 Relationship/Affinity System** | **CAO**           | Không có điểm quan hệ với NPC                                                                  |
| 6   | **🟠 Dynamic StartNodeId**          | **CAO**           | startNodeId là static, không thay đổi dựa trên story state                                     |
| 7   | **🟠 Global Story Manager**         | **CAO**           | Thiếu singleton quản lý tổng thể cốt truyện                                                    |
| 8   | **🟡 Portrait/Emotion**             | **TRUNG BÌNH**    | Không có hình ảnh nhân vật, biểu cảm (happy, sad, angry...)                                    |
| 9   | **🟡 Audio/Voice Acting**           | **TRUNG BÌNH**    | Không có tiếng nói, sound effects                                                              |
| 10  | **🟡 Dialogue History Log**         | **TRUNG BÌNH**    | Không có log xem lại hội thoại đã qua                                                          |
| 11  | **🟢 Rich Text Support**            | **THẤP**          | Chưa hỗ trợ bold, italic, màu sắc trong text                                                   |

---

## 🛠️ YÊU CẦU NÂNG CẤP CỤ THỂ

### **📦 UPGRADE 1: Story State Manager (Singleton)**

```
┌─────────────────────────────────────────────────────────────────┐
│  StoryManager.cs (Singleton - DontDestroyOnLoad)               │
├─────────────────────────────────────────────────────────────────┤
│  storyFlags: Dictionary<string, bool>                          │
│       VD: "saved_villager" = true                              │
│           "killed_boss" = false                                │
│                                                                 │
│  storyVariables: Dictionary<string, int>                       │
│       VD: "gold" = 500                                         │
│           "relationship_adam" = 50                             │
│           "chapter" = 2                                        │
│                                                                 │
│  Methods:                                                       │
│  ├── SetFlag(string key, bool value)                           │
│  ├── GetFlag(string key) → bool                                │
│  ├── SetVariable(string key, int value)                        │
│  ├── GetVariable(string key) → int                             │
│  ├── ModifyVariable(string key, int delta)                     │
│  ├── SaveProgress() → PlayerPrefs/JSON                         │
│  └── LoadProgress() → từ PlayerPrefs/JSON                      │
└─────────────────────────────────────────────────────────────────┘
```

---

### **📦 UPGRADE 2: Conditional Dialogue Data**

```
DialogueChoice (MỞ RỘNG)
├── choiceText: string
├── nextNodeId: int
├── actionId: string
├── ⭐ NEW: requiredFlags: string[]       ← Cần flags này = true mới hiện
├── ⭐ NEW: forbiddenFlags: string[]      ← Nếu có flags này = true thì ẩn
├── ⭐ NEW: requiredVariable: string      ← Tên variable cần kiểm tra
├── ⭐ NEW: minValue: int                 ← Giá trị tối thiểu
└── ⭐ NEW: maxValue: int                 ← Giá trị tối đa

DialogueNode (MỞ RỘNG)
├── nodeId: int
├── speakerName: string
├── isPlayerSpeaking: bool
├── dialogueLines: string[]
├── ⭐ NEW: portraitSprite: Sprite        ← Hình chân dung
├── ⭐ NEW: emotion: EmotionType          ← Enum: Neutral/Happy/Sad/Angry...
├── ⭐ NEW: audioClip: AudioClip          ← Voice/SFX
├── choices: DialogueChoice[]
├── nextNodeId: int
├── ⭐ NEW: conditionalNextNodes: ConditionalBranch[]
│        ├── condition: string (flag name)
│        ├── expectedValue: bool
│        └── targetNodeId: int
└── ⭐ NEW: setFlagsOnEnter: FlagAction[] ← Tự động set flag khi vào node
         ├── flagName: string
         └── value: bool
```

---

### **📦 UPGRADE 3: Dynamic Dialogue Start**

```
NPCInteraction.cs (MỞ RỘNG)
├── ⭐ NEW: conditionalDialogues: ConditionalDialogueEntry[]
│        ├── requiredFlag: string
│        ├── requiredValue: bool
│        └── dialogueData: DialogueData
│
│   → Khi bắt đầu dialogue:
│      Kiểm tra từ đầu mảng, nếu condition match → dùng dialogue đó
│      Nếu không match → dùng default dialogueData
```

---

### **📦 UPGRADE 4: Relationship System**

```
RelationshipManager.cs
├── relationships: Dictionary<string, int>  ← NPC ID → điểm quan hệ
├── GetRelationship(npcId) → int
├── ModifyRelationship(npcId, delta)
├── GetRelationshipLevel(npcId) → Enum(Stranger/Acquaintance/Friend/BestFriend/Lover)
│
DialogueChoice (THÊM)
├── ⭐ relationshipRequired: int           ← Cần >= X điểm mới hiện
├── ⭐ relationshipChange: int             ← +/- điểm khi chọn
```

---

### **📦 UPGRADE 5: Save/Load Progress**

```
DialogueSaveData.cs (Serializable)
├── completedConversations: List<string>
├── storyFlags: Dictionary<string, bool>
├── storyVariables: Dictionary<string, int>
├── relationships: Dictionary<string, int>
├── currentChapter: int
├── saveTimestamp: DateTime

SaveManager.cs
├── SaveGame(slot) → JSON file
├── LoadGame(slot) → DialogueSaveData
├── AutoSave() → sau mỗi dialogue quan trọng
```

---

### **📦 UPGRADE 6: Portrait & Emotion System**

```
DialogueSystem.cs (THÊM)
├── portraitImage: Image                   ← UI Image hiển thị portrait
├── UpdatePortrait(Sprite, EmotionType)

CharacterPortraitData.cs (ScriptableObject)
├── characterId: string
├── portraits: EmotionPortrait[]
│        ├── emotion: EmotionType
│        └── sprite: Sprite
```

---

### **📦 UPGRADE 7: Audio System**

```
DialogueSystem.cs (THÊM)
├── audioSource: AudioSource
├── typingSFX: AudioClip                   ← Tiếng gõ chữ
├── PlayNodeAudio(AudioClip)
├── PlayTypingSound()
```

---

## 📋 LUỒNG TÓM TẮT SAU NÂNG CẤP

```
Player nhấn E để nói chuyện
       ↓
NPCInteraction kiểm tra conditionalDialogues
       ↓
Chọn DialogueData phù hợp với StoryManager.storyFlags
       ↓
DialogueSystem.StartDialogueWithChoices()
       ↓
GoToNode() → Kiểm tra conditionalNextNodes
       ↓
DisplayNodeLine() + UpdatePortrait() + PlayAudio()
       ↓
ShowChoices() → Lọc choices theo conditions
       ↓
OnChoiceSelected() → setFlagsOnEnter + relationshipChange
       ↓
StoryManager lưu flags → AutoSave()
       ↓
EndDialogue() → Multiple endings dựa trên accumulated flags
```

---

## ❓ CÂU HỎI CHO DEVELOPER (Chọn hướng tiếp theo)

| #     | Lựa chọn                          | Mô tả                                           |
| ----- | --------------------------------- | ----------------------------------------------- |
| **1** | **Implement StoryManager**        | Tạo singleton quản lý flags/variables cơ bản    |
| **2** | **Nâng cấp DialogueData**         | Thêm conditions, portrait, audio vào data model |
| **3** | **Implement Save/Load**           | Tạo hệ thống lưu/load tiến trình                |
| **4** | **Implement Relationship System** | Thêm điểm quan hệ với NPC                       |
| **5** | **Implement Portrait System**     | Thêm hình chân dung nhân vật vào UI             |
| **6** | **Tất cả cơ bản (1, 2, 3)**       | Implement 3 tính năng quan trọng nhất           |
| **7** | **Full package (1-5)**            | Implement toàn bộ nâng cấp                      |

Bạn muốn tôi implement phần nào trước?
