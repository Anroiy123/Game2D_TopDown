# 🎮 Game Thanh Hóa - Bạo Lực Học Đường

Game 2D top-down mô phỏng về chủ đề **bạo lực học đường** (school bullying), xây dựng bằng Unity 6.

## 📖 Giới thiệu

### Bối cảnh
- **Nhân vật chính**: Đức - học sinh chuyển trường, nhút nhát, cô đơn, dễ bị bắt nạt
- **Chủ đề**: Cuộc sống của Đức trước, trong và sau khi bị bắt nạt
- **Gameplay**: Kết hợp Top-down exploration + Visual Novel storytelling

### Tính năng chính
- 🚶 Di chuyển 4 hướng (WASD/Arrows) trong môi trường 2D top-down
- 💬 Hệ thống Dialogue với branching choices và conditions
- 🎬 Visual Novel Mode cho các cảnh kể chuyện
- 📖 Storytelling System cho các ending sequences
- 🎯 Story System với flags/variables quyết định kết cục
- 🎭 NPC Behaviors: Follow player, surround player, waypoint walking
- ⚔️ Cutscene System: Fight 1v1, Bully beat
- 🔔 Interaction Indicators: Animated (NPCs) và Simple (doors, beds)

### 3 Kết cục
| Ending | Tên | Điều kiện | Thông điệp |
|--------|-----|-----------|------------|
| **Good** | Đứng lên chống lại | Đánh trả thành công | Đứng lên và tìm kiếm sự giúp đỡ |
| **True** | Chia sẻ với gia đình | Thú nhận với mẹ | Gia đình luôn ở bên bạn |
| **Bad** | Cuộc đời đen tối | Giấu mẹ | Im lặng chịu đựng hủy hoại cuộc đời |

## 🎮 Điều khiển

| Phím | Hành động |
|------|-----------|
| **WASD / Arrows** | Di chuyển |
| **Shift** | Sprint (chạy nhanh) |
| **E** | Tương tác (ngồi, nói chuyện, tiếp tục dialogue) |
| **ESC** | Thoát dialogue / Skip storytelling |
| **1-9** | Chọn dialogue choices |
| **F1** | Debug Flag Menu (Editor only) |
| **T** | Test VN Scene (nếu có VNSceneQuickTest) |

## 🔧 Yêu cầu

- **Unity 6** (6000.2.14f1+)
- **2D Sprite** package
- **Cinemachine** 3.1.5 (camera follow)
- **Input System** 1.16.0 (chưa dùng, đang dùng legacy Input)
- **Super Tiled2Unity** 2.4.0 (import Tiled maps)

## 🚀 Cài đặt

1. Clone repository
2. Mở project bằng Unity Hub (Unity 6000.2.14f1+)
3. Mở scene `Assets/Scenes/MainMenu.unity` hoặc `HomeScene.unity`
4. Nhấn **Play** để chạy game

## 🏗️ Cấu trúc dự án

```
Assets/
├── Scripts/
│   ├── Core/                    # Singleton Managers (DontDestroyOnLoad)
│   │   ├── GameManager.cs       # Game state, scene loading, spawn player
│   │   ├── StoryManager.cs      # Story flags/variables, ending determination
│   │   ├── SaveManager.cs       # Save/Load game (JSON)
│   │   └── SaveData.cs          # Save data structure
│   ├── Player/
│   │   ├── PlayerMovement.cs    # Di chuyển, ngồi ghế, trạng thái
│   │   └── PlayerSelfDialogue.cs # Player tự thoại (inner thoughts)
│   ├── NPC/
│   │   ├── NPCInteraction.cs    # Tương tác NPC, trigger dialogue
│   │   ├── NPCFollowPlayer.cs   # NPC đi theo player (bullies)
│   │   ├── NPCSurroundPlayer.cs # NPCs vây quanh player
│   │   ├── FightCutscene.cs     # Cutscene đánh nhau 1v1
│   │   └── BullyBeatCutscene.cs # Cutscene bullies đánh player
│   ├── Dialogue/
│   │   ├── DialogueSystem.cs    # UI dialogue, typewriter, choices
│   │   ├── DialogueData.cs      # ScriptableObject dialogue structure
│   │   └── DialogueTrigger.cs   # Trigger dialogue từ objects
│   ├── VisualNovel/
│   │   ├── VisualNovelManager.cs # VN mode singleton
│   │   ├── VNSceneData.cs       # VNSceneData ScriptableObject
│   │   └── VNTrigger.cs         # Trigger VN scenes
│   ├── Storytelling/            # Ending/Cutscene storytelling system
│   │   ├── StorytellingManager.cs
│   │   ├── StorytellingSequenceData.cs
│   │   └── StorytellingTrigger.cs
│   ├── Scene/
│   │   ├── SceneTransition.cs   # Door/Portal chuyển scene
│   │   ├── SpawnManager.cs      # Quản lý spawn points
│   │   ├── ScreenFader.cs       # Fade in/out + transition text
│   │   └── TimeSkipTrigger.cs   # Trigger time skip
│   ├── Interaction/
│   │   ├── BedInteraction.cs    # Ngủ, tăng ngày
│   │   ├── DoorController.cs    # Mở/đóng cửa animation
│   │   └── InteractionIndicator.cs # Animated indicator (NPCs)
│   ├── Debug/                   # Debug tools (Editor only)
│   │   └── DebugFlagMenu.cs     # F1 menu set flags runtime
│   └── Editor/                  # Editor tools
│       ├── DialogueJsonImporter.cs
│       ├── VNSceneCreator.cs
│       └── StorytellingSequenceCreator.cs
├── Data/
│   ├── Dialogues/               # JSON dialogue files + DialogueData assets
│   │   ├── Day1/, Day2/, Week1/, Week3/, CriticalDay/
│   │   └── NPCs/
│   ├── VisualNovel/             # VNSceneData assets
│   └── Storytelling/            # StorytellingSequenceData assets (endings)
├── Scenes/                      # Unity scenes
│   ├── MainMenu.unity
│   ├── HomeScene.unity
│   ├── ClassroomScene.unity
│   └── StreetScene.unity
├── Prefabs/                     # Prefabs (Player, NPC, Chair)
├── Animation/                   # Animation clips & controllers
├── Sprites/                     # Sprite assets
├── TileMap/                     # Tilemap assets (Super Tiled2Unity)
└── BackGround&Avatar/           # VN backgrounds và character avatars
```

## 📝 Hệ thống Scripts

### Core (Singleton Managers)

| Script | Chức năng |
|--------|-----------|
| `GameManager.cs` | Quản lý game state, scene loading, DontDestroyOnLoad |
| `StoryManager.cs` | Story flags/variables, ending determination |
| `SaveManager.cs` | Lưu/Load game vào JSON file |

### Player

| Script | Chức năng |
|--------|-----------|
| `PlayerMovement.cs` | Di chuyển, ngồi ghế, trạng thái (talking, sleeping, locked) |
| `PlayerSelfDialogue.cs` | Player tự thoại, auto-trigger by flags |

### Dialogue & Visual Novel

| Script | Chức năng |
|--------|-----------|
| `DialogueSystem.cs` | UI dialogue, typewriter effect, choices, avatar mode |
| `DialogueData.cs` | ScriptableObject cho dialogue với conditions |
| `VisualNovelManager.cs` | VN mode: background + characters + dialogue |
| `VNSceneData.cs` | ScriptableObject cho VN scenes |

### Storytelling (Endings)

| Script | Chức năng |
|--------|-----------|
| `StorytellingManager.cs` | Singleton quản lý storytelling sequences |
| `StorytellingSequenceData.cs` | ScriptableObject cho ending sequences |
| `StorytellingTrigger.cs` | Trigger storytelling từ scene |

## 🛠️ Editor Tools

| Menu | Chức năng |
|------|-----------|
| `Tools → Game Setup → Create Managers (All)` | Tạo tất cả managers |
| `Tools → Dialogue → Import JSON to DialogueData` | Import JSON thành DialogueData |
| `Tools → Visual Novel → Create VN Scene Quick Setup` | Tạo nhanh VN scene |
| `Tools → Visual Novel → Validate VN Scene` | Kiểm tra VN scene |
| `Tools → Storytelling → Create Sequence` | Tạo storytelling sequence |
| `Tools → NPC Animator Generator` | Generate NPC animators |
| `Tools → Interaction → Setup Indicator on Selected NPC` | Setup interaction indicator |

## 📋 Tạo NPC mới

1. Tạo GameObject với Sprite, Animator, Box Collider 2D
2. Thêm tag `NPC`
3. Add component `NPCInteraction`
4. Cấu hình:
   - `NPC Name`: Tên hiển thị
   - `Dialogue Data`: Kéo DialogueData asset vào
   - `Interaction Range`: Khoảng cách tương tác
5. Tạo `NameCanvas` (World Space) làm con của NPC

### Tạo Dialogue bằng JSON (Khuyên dùng!)

1. **Viết JSON:**
   ```json
   {
     "conversationName": "NPC_Dialogue",
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
         "choices": [
           { "text": "Chào mẹ", "next": 2, "setTrue": ["greeted_mom"] },
           { "text": "Im lặng", "next": 3 }
         ]
       }
     ]
   }
   ```
2. **Lưu vào** `Assets/Data/Dialogues/YourFile.json`
3. **Import:** `Tools → Dialogue → Import JSON to DialogueData`
4. **Kéo asset** vào `NPCInteraction.dialogueData`
5. **Tick** `Use Advanced Dialogue`

## 📊 Story Flow

```
NGÀY 1 (Day1): Scene 1-9
├── Phòng ngủ → Phòng khách → Lớp học → Đường phố
├── Gặp tụi bắt nạt lần đầu
└── Về nhà

TUẦN 1 (Week1): Scene 10-16
├── Giờ ra chơi → Gặp cô giáo
├── Gặp lại tụi bắt nạt
└── Bị ép "kết bạn"

NGÀY 2 (Day2): Scene 17-21
├── Lớp học → Đường phố
├── Bị xin tiền lần đầu
└── Về nhà

TUẦN 3 (Week3): Scene 21-24
├── Tin tức bạo lực học đường
├── Thông điệp cảnh báo
└── Bị chặn đường xin tiền

CRITICAL DAY: Scene 25-28
├── Mẹ dặn về sớm
├── Gặp tụi bắt nạt lần cuối
└── Lựa chọn quyết định → 3 Endings
```

## ⚠️ Lưu ý quan trọng

- **Hệ tọa độ**: Y dương (+) = xuống/phía trước, Y âm (-) = lên/phía sau
- **Animator Parameters**: Speed, Horizontal, Vertical, IsSitting
- **Tags**: `Player`, `NPC`, `Chair`
- **Sorting Layers**: Default, Ground, Low Decoration, Main, High/Overhead

## 📚 Tài liệu chi tiết

- `docs/story.md` - Cốt truyện chi tiết
- `docs/dialogue.md` - Hướng dẫn hệ thống dialogue
- `docs/flags.md` - Danh sách flags và variables
- `docs/visualnovel_README.md` - Hướng dẫn VN system
- `docs/README_STORYTELLING.md` - Hướng dẫn Storytelling system
- `docs/interaction_system_overview.md` - Hệ thống interaction indicators

## 🎯 API Reference

### PlayerMovement
```csharp
SetTalkingState(bool talking)    // Khóa khi dialogue
SetSleepingState(bool sleeping)  // Khóa khi ngủ
SetMovementEnabled(bool enabled) // Khóa từ bên ngoài (cutscene)
SetNearNPC(bool nearNPC)         // Ưu tiên NPC hơn ghế
```

### VisualNovelManager
```csharp
StartVNScene(VNSceneData, Action onComplete)
EndVNMode()
IsVNModeActive // Property
```

### StoryManager
```csharp
SetFlag(string key, bool value)
GetFlag(string key) → bool
SetVariable(string key, int value)
GetVariable(string key) → int
DetermineEnding() → EndingType
```

### StorytellingManager
```csharp
StartSequence(StorytellingSequenceData, Action onComplete)
StopSequence()
IsPlaying // Property
```

### ScreenFader
```csharp
FadeOut(Action onComplete)
FadeIn(Action onComplete)
FadeWithTextCoroutine(string text, float duration) → IEnumerator
```

## 📄 License

Project này được phát triển cho mục đích giáo dục về chủ đề bạo lực học đường.
