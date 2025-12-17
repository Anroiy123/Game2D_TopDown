# Khắc Phục Lỗi ScriptableObject Không Hiển Thị Trong Unity

## 🔴 Triệu Chứng

- ScriptableObject assets hiển thị **icon `#`** thay vì icon cube bình thường
- Inspector hiển thị **"None (Script)"** khi click vào asset
- Unity không nhận diện được type của ScriptableObject
- Console có thể hiện warning: `"The referenced script (Unknown) on this Behaviour is missing!"`

![Example](https://i.imgur.com/example.png)

---

## 🔍 Nguyên Nhân

### 1. Nhiều Class Trong Cùng Một File

Unity có thể nhầm lẫn khi một file `.cs` chứa nhiều class, đặc biệt khi:
- Có cả `[Serializable]` class và `ScriptableObject` class
- Tên class tương tự nhau (VD: `VNScene` và `VNSceneData`)

**Ví dụ gây lỗi:**
```csharp
// ❌ File VisualNovelData.cs
[Serializable]
public class VNScene { }

[Serializable]
public class VNCharacterDisplay { }

[CreateAssetMenu]
public class VNSceneData : ScriptableObject { }  // Unity bị confused!
```

### 2. File `.meta` Bị Thiếu Hoặc Corrupt

File `.meta` bị thiếu thông tin `MonoImporter`:

```yaml
# ❌ Meta file bị lỗi
fileFormatVersion: 2
guid: 1dd63d338344c064f99a7accb0611b6b
# Thiếu MonoImporter section!
```

### 3. Unity Cache Bị Lỗi

Sau khi refactor code, Unity cache (`Library/` folder) có thể không sync với code mới.

---

## ✅ Cách Khắc Phục

### Bước 1: Tách Class Ra File Riêng

**Quy tắc vàng: 1 ScriptableObject = 1 File**

```
Assets/Scripts/VisualNovel/
├── VNScene.cs              # Chứa VNScene, VNCharacterDisplay
└── VNSceneData.cs          # CHỈ chứa VNSceneData : ScriptableObject
```

**File VNScene.cs:**
```csharp
using System;
using UnityEngine;

[Serializable]
public class VNScene { }

[Serializable]
public class VNCharacterDisplay { }
```

**File VNSceneData.cs:**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "NewVNScene", menuName = "Visual Novel/VN Scene Data")]
public class VNSceneData : ScriptableObject
{
    public VNScene sceneData;
    // ...
}
```

### Bước 2: Kiểm Tra File `.meta`

Mở file `.meta` của script, đảm bảo có đầy đủ thông tin:

```yaml
fileFormatVersion: 2
guid: 1dd63d338344c064f99a7accb0611b6b
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

**Nếu thiếu, thêm vào hoặc xóa file `.meta` và để Unity tạo lại.**

### Bước 3: Refresh Unity

Trong Unity Editor:
1. **Assets → Reimport All**
2. Hoặc nhấn **Ctrl+R** (Windows) / **Cmd+R** (Mac)

### Bước 4: Restart Unity

1. **Đóng Unity hoàn toàn**
2. **(Tùy chọn)** Xóa thư mục `Library/` trong project folder
   - ⚠️ Unity sẽ mất 5-10 phút để rebuild
   - Chỉ làm nếu các bước trên không hiệu quả
3. **Mở lại Unity**

### Bước 5: Force Regenerate Thumbnails (Nếu Icon Vẫn Lỗi)

Chạy lệnh PowerShell trong project folder:

```powershell
Get-ChildItem -Path "Assets" -Filter "*.asset.meta" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    [System.IO.File]::WriteAllText($_.FullName, $content, [System.Text.UTF8Encoding]::new($false))
}
```

Sau đó **Ctrl+R** trong Unity.

---

## 🛡️ Cách Phòng Tránh

| Quy Tắc | Mô Tả | Ví Dụ |
|---------|-------|-------|
| **1 ScriptableObject = 1 File** | Mỗi class kế thừa `ScriptableObject` phải ở file riêng | `VNSceneData.cs` chỉ chứa `VNSceneData` |
| **Tên file = Tên class** | Tên file phải trùng với tên class chính | `DialogueData.cs` → `class DialogueData` |
| **Serializable class tách riêng** | Các helper class `[Serializable]` nên ở file riêng | `VNScene.cs` chứa các data class |
| **KHÔNG xóa `.meta` file** | Giữ nguyên GUID để không mất reference trong scene | Commit `.meta` vào Git |
| **Commit thường xuyên** | Dễ rollback nếu có lỗi | Commit sau mỗi thay đổi lớn |

---

## 🔧 Checklist Khi Gặp Lại

```
□ 1. Kiểm tra file .meta có đầy đủ MonoImporter section không
□ 2. Kiểm tra có nhiều class trong cùng file không → Tách ra
□ 3. Kiểm tra tên class có trùng/tương tự không
□ 4. Assets → Reimport All
□ 5. Đóng Unity và mở lại
□ 6. (Cuối cùng) Xóa thư mục Library/ và mở lại Unity
```

---

## 📁 Cấu Trúc File Đề Xuất

```
Assets/Scripts/
├── Dialogue/
│   ├── DialogueData.cs           # ScriptableObject
│   ├── DialogueNode.cs           # [Serializable] class
│   └── DialogueChoice.cs         # [Serializable] class
├── VisualNovel/
│   ├── VNSceneData.cs            # ScriptableObject
│   ├── VNSequenceData.cs         # ScriptableObject
│   ├── VNScene.cs                # [Serializable] classes
│   ├── VisualNovelManager.cs     # MonoBehaviour
│   └── VNTrigger.cs              # MonoBehaviour
└── Data/
    ├── Dialogues/                # .asset files
    └── VisualNovel/              # .asset files
```

---

## 🐛 Các Lỗi Liên Quan

### "Script class cannot be found"

**Nguyên nhân:** Tên file không khớp với tên class.

**Khắc phục:** Đổi tên file cho khớp với class name.

### "The associated script can not be loaded"

**Nguyên nhân:** File `.meta` bị corrupt hoặc GUID bị trùng.

**Khắc phục:**
1. Xóa file `.meta`
2. Đóng Unity
3. Mở lại Unity (Unity sẽ tạo `.meta` mới)
4. Reassign references trong scene/prefab

### Assets Mất Reference Sau Khi Refactor

**Nguyên nhân:** GUID trong `.meta` bị thay đổi.

**Khắc phục:**
1. Dùng Git để restore file `.meta` cũ
2. Hoặc dùng Unity's "Find References in Scene" để reassign

---

## 📚 Tham Khảo

- [Unity Manual: ScriptableObject](https://docs.unity3d.com/Manual/class-ScriptableObject.html)
- [Unity Forum: Script Missing Issue](https://forum.unity.com/threads/script-missing.12345/)
- Project: `docs/CLAUDE.md` - Visual Novel System section

---

**Lần gặp lỗi:** 2025-12-17  
**Giải pháp:** Tách `VNScene` ra file riêng, sửa `.meta`, restart Unity  
**Thời gian khắc phục:** ~10 phút

