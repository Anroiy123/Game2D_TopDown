# 🔄 JSON DIALOGUE WORKFLOW - BEST PRACTICES

## 📁 RECOMMENDED FOLDER STRUCTURE

```
Assets/Data/Dialogues/
├── NPCs/
│   ├── adam_dialogue.json
│   ├── teacher_dialogue.json
│   └── bully_dialogue.json
├── Story/
│   ├── day1_morning.json
│   ├── day1_school.json
│   └── endings/
│       ├── good_ending.json
│       ├── bad_ending.json
│       └── true_ending.json
├── Tutorial/
│   └── intro_dialogue.json
└── _Examples/
    ├── example_dialogue.json
    └── advanced_example.json
```

## ✅ BEST PRACTICES

### **1. Naming Conventions**

```
✅ GOOD:
- mom_morning_day1.json
- bully_encounter_school.json
- teacher_talk_after_fight.json

❌ BAD:
- dialogue1.json
- test.json
- new_file_copy_final_v2.json
```

### **2. Node ID Strategy**

```json
// ✅ Structured IDs (recommended)
{
  "nodes": [
    { "id": 0, "speaker": "...", ... },    // Intro
    { "id": 10, "speaker": "...", ... },   // Main branch A
    { "id": 20, "speaker": "...", ... },   // Main branch B
    { "id": 100, "speaker": "...", ... }   // Ending
  ]
}

// ❌ Sequential IDs (hard to insert)
{ "id": 0 }, { "id": 1 }, { "id": 2 }, { "id": 3 }
```

**Lý do:** Dễ insert node mới giữa chừng (VD: id 11, 12, 13...)

### **3. Comments in JSON (Workaround)**

```json
{
  "_comment": "This is Day 1 morning dialogue with mom",
  "conversationName": "Day1_Morning",
  "nodes": [
    {
      "id": 0,
      "_comment": "Mom wakes up player",
      "speaker": "Mẹ",
      "lines": ["Đức ơi, dậy đi con!"],
      "next": 1
    }
  ]
}
```

Tool sẽ **ignore** fields bắt đầu bằng `_`.

### **4. Modular Design**

```
✅ Tách nhỏ file:
- mom_greeting.json (5-10 nodes)
- mom_worried.json (10-15 nodes)
- mom_angry.json (8-12 nodes)

❌ File quá lớn:
- mom_all_dialogues.json (100+ nodes)
```

### **5. Reusable Choices**

```json
// ✅ Tái sử dụng choice text
{
  "choices": [
    { "text": "Đồng ý", "next": 10 },
    { "text": "Từ chối", "next": 20 },
    { "text": "Hỏi thêm thông tin", "next": 5 }
  ]
}
```

---

## 🛠️ EXTERNAL TOOLS

### **1. JSON Validators**

- https://jsonlint.com/
- https://jsonformatter.curiousconcept.com/
- VS Code Extension: **JSON Tools**

### **2. Excel/Google Sheets → JSON**

```python
# Python script example
import json
import pandas as pd

df = pd.read_excel("dialogues.xlsx")
data = {
    "conversationName": df["conversation"][0],
    "nodes": []
}
for _, row in df.iterrows():
    data["nodes"].append({
        "id": int(row["id"]),
        "speaker": row["speaker"],
        "lines": [row["line1"], row["line2"]],
        "next": int(row["next"])
    })

with open("output.json", "w", encoding="utf-8") as f:
    json.dump(data, f, ensure_ascii=False, indent=2)
```

### **3. Visual Dialogue Editors**

- **Yarn Spinner** (export to custom JSON format)
- **Twine** (convert HTML to JSON)
- **articy:draft** (professional tool)

---

## 🔍 DEBUGGING

### **Common Errors:**

| Error | Cause | Fix |
|-------|-------|-----|
| `JSON Error: Unexpected token` | Thiếu dấu phẩy/ngoặc | Dùng jsonlint.com |
| `Node not found: X` | `next` trỏ tới ID không tồn tại | Kiểm tra tất cả `next` |
| `Duplicate node ID` | 2 nodes cùng ID | Đổi ID cho unique |
| `Choice not showing` | `varConditions` không đủ | Check StoryManager variables |

### **Debug Workflow:**

```
1. Preview JSON trong tool
2. Check error message
3. Fix trong text editor
4. Re-import
5. Test in-game
```

---

## 📊 VERSION CONTROL

### **Git .gitignore:**

```gitignore
# IGNORE generated .asset files
Assets/Data/Dialogues/*.asset
Assets/Data/Dialogues/*.asset.meta

# KEEP source .json files
!Assets/Data/Dialogues/*.json
!Assets/Data/Dialogues/*.json.meta
```

**Lý do:** JSON là source of truth, .asset có thể re-generate.

### **Git Commit Messages:**

```
✅ GOOD:
- "Add bully encounter dialogue (day 1)"
- "Update mom dialogue: add worried branch"
- "Fix typo in teacher dialogue line 5"

❌ BAD:
- "Update"
- "Fix stuff"
- "asdfasdf"
```

---

## 🚀 ADVANCED TIPS

### **1. Dynamic Text with Variables**

```json
{
  "lines": [
    "Bạn có {money}đ.",
    "Level hiện tại: {player_level}"
  ]
}
```

(Cần implement `TextReplacement` system trong `DialogueSystem.cs`)

### **2. Batch Import Script**

```csharp
// Assets/Editor/BatchImportDialogues.cs
[MenuItem("Tools/Batch Import All JSON Dialogues")]
static void BatchImport()
{
    string[] jsonFiles = Directory.GetFiles(
        "Assets/Data/Dialogues", 
        "*.json", 
        SearchOption.AllDirectories
    );
    foreach (var file in jsonFiles)
    {
        // Import logic...
    }
}
```

### **3. Localization Support**

```
Dialogues/
├── en/
│   ├── mom_dialogue.json
│   └── teacher_dialogue.json
└── vi/
    ├── mom_dialogue.json
    └── teacher_dialogue.json
```

---

## 📚 RESOURCES

- [JSON Syntax](https://www.json.org/)
- [Unity ScriptableObjects](https://docs.unity3d.com/Manual/class-ScriptableObject.html)
- Project Docs: `docs/dialogue.md`
- Quick Start: `docs/json_import_quickstart.md`

