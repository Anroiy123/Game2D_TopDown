# 📝 JSON DIALOGUE IMPORT - HƯỚNG DẪN NHANH

## 🎯 MỤC ĐÍCH

Tool này giúp bạn tạo DialogueData (ScriptableObject) từ file JSON đơn giản, dễ viết và dễ quản lý hơn việc tạo thủ công trong Inspector.

---

## 🚀 CÁCH SỬ DỤNG (3 BƯỚC)

### **Bước 1: Tạo file JSON**

Tạo file `.json` trong folder `Assets/Data/Dialogues/` (hoặc bất kỳ đâu):

```json
{
  "conversationName": "MyDialogue",
  "startNodeId": 0,
  "nodes": [
    {
      "id": 0,
      "speaker": "Mẹ",
      "lines": ["Đức ơi, dậy đi con!"],
      "next": 1
    },
    {
      "id": 1,
      "speaker": "Đức",
      "isPlayer": true,
      "lines": ["Dạ..."],
      "next": -1
    }
  ]
}
```

### **Bước 2: Import trong Unity**

```
Unity Menu → Tools → Dialogue → Import JSON to DialogueData
```

1. Kéo file JSON vào trường **"JSON File"**
2. Chọn **Output Folder** (mặc định: `Assets/Data/Dialogues`)
3. Nhấn **"Preview JSON"** để xem trước (optional)
4. Nhấn **"Import & Create DialogueData"**
5. ✅ File `.asset` được tạo!

### **Bước 3: Sử dụng trong game**

1. Kéo file `DialogueData.asset` vào `NPCInteraction` component
2. Tick ✅ **"Use Advanced Dialogue"**
3. Play game!

---

## 📋 JSON FORMAT CHEAT SHEET

### **Cấu trúc cơ bản:**

```json
{
  "conversationName": "string",   // Tên hội thoại
  "startNodeId": 0,                // Node bắt đầu (default: 0)
  "nodes": [...]                   // Danh sách nodes
}
```

### **Node đơn giản:**

```json
{
  "id": 0,                         // ID duy nhất (bắt buộc)
  "speaker": "Tên NPC",            // Người nói (rỗng = narrator)
  "isPlayer": false,               // Player nói? (default: false)
  "lines": ["Text 1", "Text 2"],   // Nội dung (bắt buộc)
  "next": 1                        // Node tiếp theo (-1 = kết thúc)
}
```

### **Node với choices:**

```json
{
  "id": 1,
  "speaker": "Đức",
  "isPlayer": true,
  "lines": ["Tôi phải làm gì?"],
  "choices": [
    {
      "text": "Lựa chọn A",
      "next": 2
    },
    {
      "text": "Lựa chọn B",
      "next": 3
    }
  ]
}
```

### **Node nâng cao (conditions + effects):**

```json
{
  "id": 5,
  "speaker": "Thằng Béo",
  "lines": ["Đưa tiền đi!"],
  "choices": [
    {
      "text": "Đưa tiền (10000đ)",
      "next": 6,
      "varConditions": [                      // Điều kiện hiển thị
        { "name": "money", "op": ">=", "value": 10000 }
      ],
      "varChanges": [                         // Thay đổi khi chọn
        { "name": "money", "op": "sub", "value": 10000 }
      ],
      "setTrue": ["gave_money"]               // Set flag TRUE
    },
    {
      "text": "Từ chối",
      "next": 7,
      "requireFlags": ["met_bullies"],        // Cần có flag này
      "forbidFlags": ["angered_bullies"]      // KHÔNG được có flag này
    }
  ]
}
```

---

## 🔑 BẢNG TRA CỨU NHANH

### **Operators:**

| Loại          | Operators                         | Ví dụ                             |
| ------------- | --------------------------------- | --------------------------------- |
| So sánh       | `>`, `>=`, `<`, `<=`, `==`, `!=`  | `{ "op": ">=", "value": 10 }`     |
| Thay đổi biến | `set`, `add`, `sub`               | `{ "op": "sub", "value": 5 }`     |

### **Fields không bắt buộc:**

- `startNodeId` (default: 0)
- `speaker` (rỗng = narrator)
- `isPlayer` (default: false)
- `next` (không có = dùng choices)
- `choices` (không có = dùng next)
- `setFlags`, `varChanges`, `requireFlags`, `forbidFlags`, `varConditions`

---

## 📚 FILE MẪU

- `example_dialogue.json` - Ví dụ cơ bản
- `advanced_example.json` - Ví dụ đầy đủ (conditions, variables, flags)

---

## 💡 TIPS

| Tip                    | Mô tả                                       |
| ---------------------- | ------------------------------------------- |
| ✅ **Preview trước**   | Dùng "Preview JSON" để kiểm tra lỗi        |
| ✅ **Overwrite**       | Tool hỏi trước khi ghi đè file cũ          |
| ✅ **Auto folder**     | Tool tự tạo folder nếu chưa tồn tại        |
| ✅ **Version control** | Commit JSON files, dễ review trên GitHub   |
| ✅ **Copy-paste**      | Dễ duplicate nodes/choices                 |

---

## ⚠️ LƯU Ý

- JSON **KHÔNG** hỗ trợ comment (`//`, `/* */`)
- Dùng https://jsonlint.com/ để validate JSON
- Node ID phải **duy nhất** trong mỗi conversation
- `next: -1` = kết thúc dialogue

---

## 📖 XEM THÊM

- **Chi tiết:** `docs/dialogue.md`
- **CLAUDE.md:** Phần "Dialogue System"

