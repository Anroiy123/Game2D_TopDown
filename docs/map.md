Để trả lời câu hỏi này, tôi cần tìm hiểu cấu trúc scene và map hiện tại của game trước:

Tôi đã hiểu cấu trúc hiện tại. Game hiện chỉ có **1 scene** (`SampleScene.unity`) và **không có hệ thống scene management**. Để có 3 bối cảnh (lớp học, nhà, và nơi khác), đây là các giải pháp:

---

# 🗺️ HƯỚNG DẪN TẠO NHIỀU BỐI CẢNH (MAP) CHO GAME

## 📊 TÌNH TRẠNG HIỆN TẠI

| Thành phần               | Có/Không                      |
| ------------------------ | ----------------------------- |
| Scene hiện tại           | `SampleScene.unity` (Lớp học) |
| SceneManager/GameManager | ❌ Không có                   |
| Door/Portal system       | ❌ Không có                   |
| Scene transition         | ❌ Không có                   |
| Player data persistence  | ❌ Không có                   |

---

## 🎯 3 PHƯƠNG PHÁP TẠO NHIỀU BỐI CẢNH

### **PHƯƠNG PHÁP 1: Multi-Scene (KHUYẾN NGHỊ cho game cốt truyện)**

```
┌─────────────────────────────────────────────────────────────────┐
│  Mỗi bối cảnh = 1 Scene riêng biệt                              │
├─────────────────────────────────────────────────────────────────┤
│  Assets/Scenes/                                                 │
│  ├── ClassroomScene.unity    ← Lớp học (hiện tại)              │
│  ├── HomeScene.unity         ← Nhà của nhân vật                │
│  └── ThirdLocationScene.unity ← Bối cảnh thứ 3                 │
│                                                                 │
│  Ưu điểm:                                                       │
│  ✅ Dễ quản lý, dễ edit riêng từng map                         │
│  ✅ Load/Unload tiết kiệm RAM                                   │
│  ✅ Phù hợp game cốt truyện có nhiều chapter                   │
│                                                                 │
│  Nhược điểm:                                                    │
│  ⚠️ Cần SceneManager để chuyển scene                           │
│  ⚠️ Cần persist player data qua các scene                      │
└─────────────────────────────────────────────────────────────────┘
```

---

### **PHƯƠNG PHÁP 2: Single Scene với Multiple Areas**

```
┌─────────────────────────────────────────────────────────────────┐
│  1 Scene duy nhất, các khu vực được Enable/Disable              │
├─────────────────────────────────────────────────────────────────┤
│  SampleScene                                                    │
│  ├── [Area_Classroom] ← SetActive(true/false)                  │
│  │   ├── Tilemap                                               │
│  │   ├── NPCs                                                  │
│  │   └── Objects                                               │
│  ├── [Area_Home] ← SetActive(true/false)                       │
│  │   ├── Tilemap                                               │
│  │   └── Objects                                               │
│  └── [Area_ThirdLocation] ← SetActive(true/false)              │
│                                                                 │
│  Ưu điểm:                                                       │
│  ✅ Không cần scene loading                                     │
│  ✅ Transition nhanh (instant)                                  │
│                                                                 │
│  Nhược điểm:                                                    │
│  ⚠️ Scene editor phức tạp                                       │
│  ⚠️ RAM cao (load tất cả cùng lúc)                              │
│  ⚠️ Khó mở rộng khi có nhiều map                               │
└─────────────────────────────────────────────────────────────────┘
```

---

### **PHƯƠNG PHÁP 3: Additive Scene Loading (NÂNG CAO)**

```
┌─────────────────────────────────────────────────────────────────┐
│  1 Scene chính + Load thêm scene khác additively                │
├─────────────────────────────────────────────────────────────────┤
│  GameScene (Persistent - Player, UI, Managers)                  │
│  ├── Player                                                     │
│  ├── Camera                                                     │
│  ├── DialogueSystem                                             │
│  └── GameManager                                                │
│                                                                 │
│  + LoadSceneMode.Additive                                       │
│  ├── ClassroomMap.unity                                         │
│  ├── HomeMap.unity                                              │
│  └── ThirdLocationMap.unity                                     │
│                                                                 │
│  Ưu điểm:                                                       │
│  ✅ Player/UI persist tự nhiên                                  │
│  ✅ Linh hoạt, chuyên nghiệp                                    │
│                                                                 │
│  Nhược điểm:                                                    │
│  ⚠️ Phức tạp hơn để implement                                  │
│  ⚠️ Cần quản lý active scene                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🛠️ IMPLEMENT PHƯƠNG PHÁP 1 (Multi-Scene - KHUYẾN NGHỊ)

### **BƯỚC 1: Tạo các Script cần thiết**

```
Scripts cần tạo:
├── GameManager.cs         ← Singleton, DontDestroyOnLoad, quản lý game state
├── SceneTransition.cs     ← Script cho Door/Portal chuyển scene
├── PlayerDataPersist.cs   ← Lưu vị trí, state của player qua các scene
└── SpawnPoint.cs          ← Điểm xuất hiện khi vào scene
```

---

### **BƯỚC 2: Cấu trúc Scene mới**

```
Classroom (Scene cũ - rename)
├── Player (có DontDestroyOnLoad)
├── GameManager (Singleton, DontDestroyOnLoad)
├── DialogueSystem (DontDestroyOnLoad)
├── Camera (Cinemachine, DontDestroyOnLoad)
├── [--- SCENE SPECIFIC ---]
├── Tilemap
├── NPCs (Adam, etc.)
├── Doors
│   └── Door_ToHome (SceneTransition → "HomeScene")
└── SpawnPoints
    └── SpawnPoint_FromHome

HomeScene (Scene mới)
├── [--- SCENE SPECIFIC ---]
├── Tilemap (nhà)
├── NPCs (gia đình?)
├── Doors
│   └── Door_ToClassroom (SceneTransition → "ClassroomScene")
└── SpawnPoints
    └── SpawnPoint_FromClassroom
```

---

### **BƯỚC 3: Flow chuyển scene**

```
┌──────────────────────────────────────────────────────────────────────────────┐
│  Player đi vào Door/Portal (OnTriggerEnter2D)                                │
│       ↓                                                                      │
│  SceneTransition component phát hiện collision                               │
│       ↓                                                                      │
│  (Optional) Fade out animation                                               │
│       ↓                                                                      │
│  GameManager.Instance.LoadScene("HomeScene", "SpawnPoint_FromClassroom")     │
│       ↓                                                                      │
│  SceneManager.LoadScene("HomeScene")                                         │
│       ↓                                                                      │
│  Player (DontDestroyOnLoad) tìm SpawnPoint và teleport đến                   │
│       ↓                                                                      │
│  (Optional) Fade in animation                                                │
│       ↓                                                                      │
│  Gameplay tiếp tục tại HomeScene                                             │
└──────────────────────────────────────────────────────────────────────────────┘
```

---

## ❓ BẠN MUỐN TIẾP TỤC NHƯ THẾ NÀO?

| #     | Lựa chọn                                | Mô tả                                       |
| ----- | --------------------------------------- | ------------------------------------------- |
| **1** | **Tạo GameManager.cs**                  | Singleton quản lý game state, scene loading |
| **2** | **Tạo SceneTransition.cs**              | Door/Portal để chuyển scene                 |
| **3** | **Tạo SpawnPoint.cs**                   | Điểm spawn khi vào scene mới                |
| **4** | **Tạo cả 3 script cơ bản (1, 2, 3)**    | Implement đủ để chạy được                   |
| **5** | **Hướng dẫn tạo Scene mới trong Unity** | Step-by-step tạo HomeScene.unity            |
| **6** | **Tất cả (1-5)**                        | Full implementation + hướng dẫn             |

Bạn muốn tôi làm gì trước? Hay bạn có câu hỏi về 3 bối cảnh cụ thể (lớp học, nhà, và nơi thứ 3 là gì)?
