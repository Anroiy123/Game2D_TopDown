# 🎮 Game 2D Top-Down Classroom

A 2D top-down classroom simulation game built with **Unity 6**.

![Unity](https://img.shields.io/badge/Unity-6-000000?style=flat&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp)
![License](https://img.shields.io/badge/License-MIT-blue)

## 📖 Mô tả

Game mô phỏng lớp học 2D theo góc nhìn top-down. Người chơi có thể di chuyển trong lớp học, ngồi vào ghế và tương tác với các NPC thông qua hệ thống đối thoại.

## ✨ Tính năng

- 🚶 **Di chuyển 4 hướng** - WASD hoặc phím mũi tên
- 🪑 **Ngồi ghế** - Nhấn E khi đứng gần ghế
- 💬 **Hệ thống đối thoại** - Tương tác với NPC, hiệu ứng typewriter
- 🔄 **NPC quay mặt** - NPC tự động quay về phía người chơi khi nói chuyện
- 🎨 **Animation Blend Tree** - Animation mượt mà cho cả Player và NPC

## 🎮 Điều khiển

| Phím              | Hành động                                             |
| ----------------- | ----------------------------------------------------- |
| **WASD / Arrows** | Di chuyển                                             |
| **E**             | Tương tác (ngồi/đứng, nói chuyện, tiếp tục đối thoại) |
| **ESC**           | Thoát đối thoại                                       |

## 🏗️ Cấu trúc dự án

```
Assets/
├── Scripts/
│   ├── Core/                    # Singleton Managers
│   │   ├── GameManager.cs       # Game state, scene loading
│   │   ├── StoryManager.cs      # Story flags/variables
│   │   ├── SaveManager.cs       # Save/Load game
│   │   └── SaveData.cs          # Save data structure
│   ├── Player/
│   │   └── PlayerMovement.cs    # Điều khiển player, ngồi ghế
│   ├── NPC/
│   │   ├── NPCInteraction.cs    # Tương tác NPC, hiển thị tên
│   │   └── BullyEncounterZone.cs # Trigger zones bắt nạt
│   ├── Dialogue/
│   │   ├── DialogueSystem.cs    # Hệ thống đối thoại UI
│   │   └── DialogueData.cs      # ScriptableObject dialogue
│   ├── Scene/
│   │   ├── SceneTransition.cs   # Door/Portal chuyển scene
│   │   ├── LocalTeleporter.cs   # Teleport trong scene
│   │   ├── SpawnManager.cs      # Quản lý spawn points
│   │   ├── SpawnPoint.cs        # Điểm spawn
│   │   └── ScreenFader.cs       # Fade in/out effect
│   ├── Interaction/
│   │   ├── BedInteraction.cs    # Tương tác giường ngủ
│   │   ├── DoorController.cs    # Điều khiển cửa
│   │   └── InteractableOutline.cs # Hiệu ứng outline
│   ├── Utilities/
│   │   ├── CameraHelper.cs      # Camera snap helper
│   │   └── SerializableDictionary.cs # Dictionary serialize
│   ├── Data/                    # ScriptableObject assets
│   │   └── AdamDialogue.asset
│   └── Editor/                  # Editor tools
│       ├── NPCAnimatorGenerator.cs
│       └── SceneSetupHelper.cs
├── Animation/                   # Animation clips & controllers
├── Prefabs/                     # Prefabs (Chair, NPC...)
├── Scenes/                      # Game scenes
├── Sprites/                     # Sprite assets
└── TileMap/                     # Tilemap assets
```

## 🔧 Yêu cầu

- **Unity 6** (6000.0.0 trở lên)
- **2D Sprite** package
- **Cinemachine** (cho camera follow)

## 🚀 Cài đặt

1. Clone repository:

   ```bash
   git clone https://github.com/Anroiy123/Game2D_TopDown.git
   ```

2. Mở project bằng Unity Hub

3. Mở scene `Assets/Scenes/SampleScene.unity`

4. Nhấn **Play** để chạy game

## 📝 Hệ thống Scripts

### Core (Singleton Managers)

| Script            | Chức năng                                            |
| ----------------- | ---------------------------------------------------- |
| `GameManager.cs`  | Quản lý game state, scene loading, DontDestroyOnLoad |
| `StoryManager.cs` | Story flags/variables, ending determination          |
| `SaveManager.cs`  | Lưu/Load game vào JSON file                          |
| `SaveData.cs`     | Cấu trúc dữ liệu save game                           |

### Player

| Script              | Chức năng                                  |
| ------------------- | ------------------------------------------ |
| `PlayerMovement.cs` | Di chuyển, ngồi ghế, trạng thái nói chuyện |

### NPC

| Script                  | Chức năng                                     |
| ----------------------- | --------------------------------------------- |
| `NPCInteraction.cs`     | Tương tác NPC, hiển thị tên, trigger dialogue |
| `BullyEncounterZone.cs` | Trigger zones cho bully encounters            |

### Dialogue

| Script                    | Chức năng                                               |
| ------------------------- | ------------------------------------------------------- |
| `DialogueSystem.cs`       | UI dialogue, typewriter effect, choices                 |
| `DialogueData.cs`         | ScriptableObject cho dialogue với conditions            |
| `DialogueJsonImporter.cs` | **[Editor Tool]** Import JSON thành DialogueData assets |

### Scene

| Script               | Chức năng                               |
| -------------------- | --------------------------------------- |
| `SceneTransition.cs` | Door/Portal chuyển scene với conditions |
| `LocalTeleporter.cs` | Teleport trong cùng scene (cầu thang)   |
| `SpawnManager.cs`    | Quản lý spawn points trong scene        |
| `SpawnPoint.cs`      | Điểm spawn với facing direction         |
| `ScreenFader.cs`     | Fade in/out effect khi chuyển scene     |

### Interaction

| Script                   | Chức năng                     |
| ------------------------ | ----------------------------- |
| `BedInteraction.cs`      | Ngủ, tăng ngày mới            |
| `DoorController.cs`      | Mở/đóng cửa animation         |
| `InteractableOutline.cs` | Viền trắng khi player đến gần |

### Utilities

| Script                      | Chức năng                          |
| --------------------------- | ---------------------------------- |
| `CameraHelper.cs`           | Snap camera khi teleport           |
| `SerializableDictionary.cs` | Dictionary serialize cho Inspector |

## Thêm NPC mới

1. Tạo GameObject với Sprite, Animator, Box Collider 2D
2. Thêm tag `NPC`
3. Add component `NPCInteraction`
4. Cấu hình:
   - `NPC Name`: Tên hiển thị
   - `Dialogue Lines`: Các câu đối thoại (Legacy mode)
   - **HOẶC** `Dialogue Data`: Kéo DialogueData asset vào (Advanced mode)
   - `Interaction Range`: Khoảng cách tương tác
5. Tạo `NameCanvas` (World Space) làm con của NPC

### 🆕 Tạo Dialogue bằng JSON (Khuyên dùng!)

1. **Viết JSON:**
   ```json
   {
     "conversationName": "NPC_Dialogue",
     "nodes": [
       { "id": 0, "speaker": "NPC Name", "lines": ["Hello!"], "next": -1 }
     ]
   }
   ```
2. **Lưu vào** `Assets/Scripts/Data/Dialogues/YourFile.json`
3. **Import:** `Tools → Dialogue → Import JSON to DialogueData`
4. **Kéo asset** vào `NPCInteraction.dialogueData`
5. **Tick** `Use Advanced Dialogue`

Xem thêm: `docs/dialogue.md` - Hướng dẫn JSON chi tiết

## Lưu ý quan trọng

- **Hệ tọa độ**: Y dương (+) = xuống/phía trước, Y âm (-) = lên/phía sau
- **Sit Offset**: Điều chỉnh Y trong Inspector để player ngồi đúng vị trí ghế
- **Animator Parameters**: Speed, Horizontal, Vertical, IsSitting

## Đóng góp

Mọi đóng góp đều được hoan nghênh! Vui lòng tạo Issue hoặc Pull Request.

## License

MIT License - Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

---

**Made with using Unity**

cốt truyện : https://docs.google.com/document/d/10_BDYeSPHmhrsQGLkuvwZt2ko-SPkldQgseSSnkpYgo/edit?tab=t.0
