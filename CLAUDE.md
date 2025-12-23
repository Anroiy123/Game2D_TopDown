# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (6000.2.14f1) 2D top-down classroom simulation game about school bullying. Player controls Đức, a new student who encounters bullies with 4 possible endings based on choices.

## Build & Run

- Open in Unity Hub (Unity 6000.2.14f1+)
- Main scene: `Assets/Scenes/SampleScene.unity`
- Press Play in Unity Editor to run

## Project Structure

```
Assets/Scripts/
├── Core/          # Singleton managers (DontDestroyOnLoad)
├── Player/        # PlayerMovement, PlayerSelfDialogue
├── NPC/           # NPCInteraction, NPCFollowPlayer, NPCSurroundPlayer, BullyEncounterZone, FightCutscene, BullyBeatCutscene
├── Dialogue/      # DialogueSystem, DialogueData, DialogueTrigger
├── Scene/         # SceneTransition, SpawnManager, ScreenFader, LocalTeleporter, TimeSkipTrigger
├── VisualNovel/   # VN mode (static backgrounds + dialogue overlay)
├── Storytelling/  # StorytellingManager, StorytellingSequenceData (endings)
├── Interaction/   # BedInteraction, DoorController, InteractableOutline
├── Utilities/     # CameraHelper, SerializableDictionary
├── Debug/         # DebugFlagMenu, QuickFlagSetter
└── Editor/        # Editor tools (SceneSetupHelper, NPCAnimatorGenerator, StorytellingSequenceCreator)
```

## ScriptableObject Best Practices (CRITICAL)

**⚠️ GOLDEN RULE: 1 ScriptableObject = 1 File**

Unity can get confused when multiple classes exist in the same file, especially when mixing `[Serializable]` classes with `ScriptableObject` classes.

**✅ CORRECT Structure:**

```
Assets/Scripts/VisualNovel/
├── VNScene.cs              # [Serializable] helper classes
├── VNSceneData.cs          # ONLY VNSceneData : ScriptableObject
└── VNSequenceData.cs       # ONLY VNSequenceData : ScriptableObject
```

**❌ WRONG - Causes "None (Script)" error:**

```csharp
// File: VisualNovelData.cs
[Serializable]
public class VNScene { }

[CreateAssetMenu]
public class VNSceneData : ScriptableObject { }  // Unity gets confused!
```

**Rules:**

1. Each `ScriptableObject` class must be in its own file
2. File name must match class name exactly
3. `[Serializable]` helper classes should be in separate files
4. Never delete `.meta` files (they contain GUID references)
5. After refactoring, always: `Assets → Reimport All` or `Ctrl+R`

**If you see `#` icon or "None (Script)" in Inspector:**

- Check if multiple classes exist in same file → Split them
- Check if `.meta` file has `MonoImporter` section
- Restart Unity (close completely and reopen)
- See `docs/troubleshooting/scriptableobject-not-loading.md` for full guide

## Singleton Pattern (Critical)

GameManager, StoryManager, SaveManager, ScreenFader, and VisualNovelManager use lazy singleton with auto-creation:

```csharp
public static GameManager Instance {
    get {
        if (_instance == null) {
            _instance = FindFirstObjectByType<GameManager>();
            if (_instance == null) {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
            }
        }
        return _instance;
    }
}
```

**Important:** All singletons use `DontDestroyOnLoad` and destroy duplicates in `Awake()`.

## Coordinate System (Non-standard)

- **Y positive (+)** = Down/Forward (towards bottom of screen)
- **Y negative (-)** = Up/Back (towards top of screen)
- `sitOffset = (0, 1.2, 0)` moves player DOWN to sit in front of chair
- `standOffset = (0, 0.5, 0)` pushes player DOWN when standing up

## Animator Hash Optimization

**Always** use pre-hashed animator parameters (never string literals):

```csharp
private readonly int speedHash = Animator.StringToHash("Speed");
private readonly int horizontalHash = Animator.StringToHash("Horizontal");
private readonly int verticalHash = Animator.StringToHash("Vertical");
private readonly int sittingHash = Animator.StringToHash("IsSitting");
private readonly int sleepingHash = Animator.StringToHash("IsSleeping");
```

## Player State Management (PlayerMovement)

Player uses boolean flags with NPC priority over chairs:

```csharp
private bool isSitting = false;
private bool isTalking = false;
private bool isSleeping = false;
private bool isNearNPC = false; // When true, E key talks to NPC instead of sitting
```

**Key logic:** `if (isNearNPC)` → skip chair interaction, prioritize NPC dialogue.

## Dialogue System

**Two modes:**

1. **Legacy** - Simple: `StartDialogue(npcName, string[] lines, callback)`
2. **Advanced** - Choice-based: `StartDialogueWithChoices(DialogueData, callback, actionCallback)`

**NPCInteraction toggles modes via `useAdvancedDialogue` bool.**

**Conditional Dialogues (NPCInteraction):**

NPC có thể có nhiều DialogueData khác nhau với điều kiện để quyết định dialogue nào sẽ được sử dụng:

```
NPCInteraction
├── dialogueData (default fallback)
├── useAdvancedDialogue: bool
└── conditionalDialogues: ConditionalDialogueEntry[]
    ├── dialogueData: DialogueData
    ├── allowedScenes: string[] (chỉ trigger trong các scene này)
    ├── requiredFlags: string[] (flags cần có)
    ├── forbiddenFlags: string[] (flags không được có)
    ├── variableConditions: VariableCondition[] (VD: current_day == 8)
    └── priority: int (cao hơn = kiểm tra trước)
```

**Ví dụ sử dụng:**

- Mẹ có dialogue khác nhau vào buổi sáng/tối
- NPC nói khác khi player đã hoàn thành quest
- Dialogue chỉ xuất hiện ở scene cụ thể (VD: Scene 8)

**Logic chọn dialogue:**

1. Sắp xếp `conditionalDialogues` theo `priority` giảm dần
2. Kiểm tra từng entry, dùng entry đầu tiên thỏa mãn `CanUse()`
3. Nếu không có entry nào thỏa mãn → dùng `dialogueData` mặc định

**DialogueData Structure:**

```
DialogueData (ScriptableObject) [Create via: Right-click → Dialogue/Dialogue Data OR JSON Import]
├── conversationName, startNodeId
├── nodes: DialogueNode[]
│   ├── nodeId, speakerName, isPlayerSpeaking
│   ├── dialogueLines: string[] [TextArea]
│   ├── choices: DialogueChoice[]
│   │   ├── choiceText, nextNodeId, actionId
│   │   ├── requiredFlags[], forbiddenFlags[] (conditional visibility)
│   │   ├── setFlagsTrue[], setFlagsFalse[] (effects on select)
│   │   └── variableConditions[], variableChanges[]
│   ├── nextNodeId (-1 = end dialogue)
│   └── setFlagsOnEnter[], variableChangesOnEnter[]
```

**DialogueChoice.CanShow()** - Filters choices based on StoryManager state before display.

**JSON Import Tool** (`Tools/Dialogue/Import JSON to DialogueData`):

```json
{
  "conversationName": "Day1_Morning",
  "startNodeId": 0,
  "nodes": [
    {
      "id": 0,
      "speaker": "Mẹ",
      "isPlayer": false,
      "lines": ["Đức ơi, dậy đi con!"],
      "next": 1
    },
    {
      "id": 1,
      "speaker": "Đức",
      "isPlayer": true,
      "lines": ["Dạ..."],
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
  ]
}
```

**JSON Field Mapping:**

- `id` → `nodeId`
- `speaker` → `speakerName`
- `isPlayer` → `isPlayerSpeaking`
- `lines` → `dialogueLines`
- `next` → `nextNodeId` (-1 = end)
- `setFlags` → `setFlagsOnEnter`
- `varChanges` → `variableChangesOnEnter`
- Choice: `requireFlags` → `requiredFlags`, `forbidFlags` → `forbiddenFlags`
- VarChange: `op` = "set"/"add"/"sub", `name`, `value`
- VarCondition: `op` = ">"/">="/"<"/"<="/"=="/"!=", `name`, `value`

## Story System

**📚 FULL REFERENCE:** See `docs/flags.md` for complete list of scenes, flags, variables, and flow diagrams.

**StoryManager.FlagKeys constants:**

- `DAY_1_COMPLETED`, `MET_BULLIES`, `BEFRIENDED_BULLIES`, `GOT_BEATEN`
- `TALKED_TO_TEACHER`, `INVITED_BY_CLASSMATE`, `REJECTED_CLASSMATE`
- `MOM_WORRIED`, `CONFESSED_TO_MOM`, `HID_FROM_MOM`, `BROUGHT_KNIFE`

**StoryManager.VarKeys constants:**

- `CURRENT_DAY` (starts 1), `MONEY` (starts 50000), `FEAR_LEVEL` (0-100)
- `ESCAPED_COUNT`, `GAVE_MONEY_COUNT`, `RELATIONSHIP_CLASSMATE`

**Common Flag Patterns (see `docs/flags.md` for full list):**

- **Scene Progress:** `day1_scene[X]_completed`, `week1_scene[X]_completed`
- **Story Flags:** `met_bullies`, `confronted_bullies`, `ran_from_bullies`
- **Choice Flags:** `choice_confronted_day1`, `choice_ran_away_day1`, `choice_gave_money`
- **State Flags:** `is_being_bullied`, `saw_news_about_bullying`

**Ending Determination (StoryManager.DetermineEnding):**

```
if BROUGHT_KNIFE → Bad_Murder
else if CONFESSED_TO_MOM → True_TellParents
else if "stood_up_to_bullies" flag → Good_StandUp
else if GOT_BEATEN && FEAR_LEVEL >= 100 → Bad_Death
```

**Variable Conditions (VariableCondition class):**

```csharp
public enum ComparisonOperator { GreaterThan, GreaterOrEqual, LessThan, LessOrEqual, Equal, NotEqual }
```

**Variable Changes (VariableChange class):**

```csharp
public enum ChangeOperation { Set, Add, Subtract }
```

**Flag/Variable Usage Examples:**

```csharp
// Check if player met bullies
if (StoryManager.Instance.GetFlag("met_bullies")) { ... }

// Set choice flag
StoryManager.Instance.SetFlag("choice_confronted_day1", true);

// Check multiple required flags
string[] required = { "day1_scene6_completed", "met_bullies" };
if (StoryManager.Instance.CheckRequiredFlags(required)) { ... }

// Check forbidden flags (must NOT have)
string[] forbidden = { "ran_from_bullies" };
if (StoryManager.Instance.CheckForbiddenFlags(forbidden)) { ... }

// Get/Set variables
int day = StoryManager.Instance.GetVariable("current_day");
StoryManager.Instance.SetVariable("fear_level", 50);
StoryManager.Instance.ModifyVariable("money", -10000); // Subtract 10000
```

## Scene Transition System

**SceneTransition (Door/Portal):**

- `targetSceneName`, `targetSpawnPointId`
- `mode`: OnTriggerEnter (auto) or OnInteract (press E)
- `requiredFlags[]`, `forbiddenFlags[]` for conditional access
- GameManager handles fade via ScreenFader singleton

**SpawnPoint:**

- `spawnPointId` - Unique ID (e.g., "from_home", "from_school")
- `isDefaultSpawn` - Fallback spawn point
- `facingDirection` - FacingDirection enum (Up, Down, Left, Right)

**Auto-generated spawn ID pattern:**

```csharp
spawnId = "from_" + previousSceneName.ToLower().Replace("scene", "");
```

**LocalTeleporter** - In-scene teleportation (stairs, shortcuts):

- `targetPosition` - Transform reference
- `useFadeEffect` - Uses ScreenFader
- `facingAfterTeleport` - Player direction after teleport
- **Cache pattern:** Stores `cachedPlayerTransform` before teleport to handle OnTriggerExit during coroutine

## Camera System (CameraHelper)

**Special handling for Cinemachine via reflection:**

```csharp
// Uses reflection to call Cinemachine methods without direct dependency
CameraHelper.NotifyTargetTeleported(target, positionDelta);
CameraHelper.SnapCameraToTarget(target);
```

Searches for `CinemachineCamera` or `CinemachineVirtualCamera` MonoBehaviours and invokes `OnTargetObjectWarped` or `ForceCameraPosition`.

## Screen Fader System (ScreenFader)

**Auto-creates Canvas and Image if not configured:**

- Canvas: ScreenSpaceOverlay, sortingOrder 9999
- Uses `Time.unscaledDeltaTime` for fade during pause
- `ForceReset()` method to recover from stuck fade states

## Interaction Systems

**BedInteraction:**

- Sets `IsSleeping` animator bool
- Advances day via `StoryManager.VarKeys.CURRENT_DAY`
- Uses `sleepPosition` Transform or falls back to BoxCollider center

**DoorController:**

- Animated doors with `IsOpen` animator bool
- `blockingCollider` - Separate BoxCollider2D for physical blocking
- **Initialization:** `animator.Play(doorClosedStateHash, 0, 1f)` to skip open animation on start

**InteractableOutline:**

- Two methods: `Shader` or `DuplicateSprite`
- `DuplicateSprite` creates scaled child sprite behind main sprite
- Requires shader `Custom/SpriteOutline` for shader method (auto-fallback to duplicate)

**InteractionIndicator:**

- Animated sprite indicator above NPCs/objects (thinking emote, warning, etc.)
- Auto-creates child GameObject with SpriteRenderer
- `indicatorScale` - Configurable scale (default 1.0, was hardcoded 2.0 causing oversized indicators)
- Uses highest sorting layer ("High / Overhead" or "UI") with order 1000
- Supports bounce animation and distance-based visibility
- Linked to NPCInteraction via `interactionIndicator` field

## Tags & Layers

| Tag      | Used For                                           |
| -------- | -------------------------------------------------- |
| `Player` | Main player character                              |
| `NPC`    | All NPC characters                                 |
| `Chair`  | Interactive chairs (BoxCollider2D, IsTrigger=true) |

## Input Keys

- **WASD/Arrows**: Movement
- **Shift**: Sprint (run faster)
- **E**: Interact (sit/stand, talk, continue dialogue, doors, teleporters, bed)
- **ESC**: Exit dialogue / Skip storytelling
- **1-9 / Numpad1-9**: Select dialogue choices
- **F1**: Debug Flag Menu (Editor only)
- **T**: Test VN Scene (if VNSceneQuickTest component exists)

## External Dependencies

- **Super Tiled2Unity** (`com.seanba.super-tiled2unity` v2.4.0) - Tiled map imports
- **Cinemachine** (v3.1.5) - Camera follow
- **InputSystem** (v1.16.0) - Not actively used yet (legacy Input class in use)

## Editor Tools

**`Tools/Game Setup/` menu:**

- `Create Door` - SceneTransition with BoxCollider2D
- `Create SpawnPoint` - SpawnPoint component
- `Create SpawnManager` - SpawnManager (checks for existing)
- `Create Managers (All)` - GameManager, StoryManager, SaveManager, SpawnManager
- `Setup StreetScene Quick` - Pre-configured street scene setup

**`Tools/Dialogue/` menu:**

- `Import JSON to DialogueData` - Import JSON files to DialogueData assets
  - Supports preview before import
  - Auto-creates folder structure
  - Handles overwrite with confirmation
  - Example JSON files: `Assets/Scripts/Data/Dialogues/example_dialogue.json`, `advanced_example.json`

**`Tools/Storytelling/` menu:**

- `Create Sequence` - Create StorytellingSequenceData for endings
  - Configure segments with backgrounds, texts, illustrations
  - Set ending type and next scene
  - Supports skip functionality

**`Tools/NPC Setup/` menu:**

- `Setup Surround Formation` - Setup NPCs vây quanh player
  - Select NPCs in Hierarchy
  - Configure formation (Circle/Semicircle), radius, speed
  - Auto-creates NPCSurroundController
  - See `docs/npc_surround_setup.md` for full guide

## Save System (SaveManager)

- Saves to `Application.persistentDataPath/gamesave.json`
- Auto-save every 60 seconds (configurable)
- **SaveData structure:**
  - Scene name, player position, facing direction
  - Story flags (List<StoryFlagEntry>), variables (List<StoryVariableEntry>)
  - Current ending, play time, stats

## Common Patterns

**Finding Player:**

```csharp
GameObject.FindGameObjectWithTag("Player")
```

**NPC facing player (uses flipX for horizontal):**

```csharp
animator.SetFloat(horizontalHash, -1f); // Always use side animation position
spriteRenderer.flipX = (direction.x > 0); // Flip for right-facing
```

**Condition checking:**

```csharp
StoryManager.Instance.CheckRequiredFlags(string[] flags)  // All must be true
StoryManager.Instance.CheckForbiddenFlags(string[] flags) // All must be false
```

## Visual Novel System

The game uses a dual-mode system: **Top-Down Mode** (exploration) and **Visual Novel Mode** (story scenes).

**VisualNovelManager** - Singleton that manages VN mode:

```csharp
// Start a VN scene
VisualNovelManager.Instance.StartVNScene(vnSceneData, onComplete);

// Check if VN mode is active
bool isVN = VisualNovelManager.Instance.IsVNModeActive;

// Manually end VN mode
VisualNovelManager.Instance.EndVNMode();
```

**VN Mode và NPC Follow (QUAN TRỌNG):**

Khi VN mode bắt đầu, `VisualNovelManager` sẽ:

1. Lưu vị trí player vào `savedPlayerPosition`
2. Dừng tất cả `NPCFollowPlayer` components ngay lập tức
3. Disable player controls

`NPCFollowPlayer.Update()` cũng kiểm tra `IsVNModeActive` và dừng di chuyển nếu VN đang active. Điều này ngăn NPC đẩy player đi trong khi đang xem VN scene.

Khi VN mode kết thúc (cùng scene, không có spawn point), player sẽ được restore về vị trí đã lưu.

**VNSceneData** (ScriptableObject) [Create via: Right-click → Visual Novel/VN Scene Data]:

```
VNSceneData
├── sceneData: VNScene
│   ├── sceneName, locationText
│   ├── backgroundImage: Sprite
│   ├── backgroundTint: Color
│   ├── characters: VNCharacterDisplay[]
│   │   ├── characterSprite, characterName
│   │   ├── position: CharacterPosition (Left/Center/Right/FarLeft/FarRight/Custom)
│   │   ├── positionOffset, scale, flipX
│   ├── dialogue: DialogueData
│   ├── bgm, ambience: AudioClip
│   ├── nextScene: VNSceneData (chain scenes)
│   ├── returnToTopDown: bool
│   └── topDownSceneName, spawnPointId (for returning)
├── requiredFlags[], forbiddenFlags[] (conditions)
└── setFlagsOnEnter[], variableChangesOnEnter[] (effects)
```

**VNSequenceData** (ScriptableObject) [Create via: Right-click → Visual Novel/VN Sequence]:

- Used for complete story sequences (e.g., "Day 1 - Morning")
- Contains array of VNSceneData for sequential playback
- Has dayNumber and timeOfDay metadata
- Supports onComplete effects

**VNTrigger** - Component to trigger VN scenes:

```csharp
// Trigger modes: OnTriggerEnter, OnInteract (E key), OnSceneStart
// Can be placed in scenes as trigger zones
// Supports conditions (requiredFlags, forbiddenFlags)
```

**Special Action IDs in VN Dialogue:**

- `"end_vn_mode"` - Immediately exits VN mode
- `"trigger_good_ending"` - Sets "stood_up_to_bullies" flag
- `"trigger_true_ending"` - Sets CONFESSED_TO_MOM flag
- `"trigger_bad_murder"` - Sets BROUGHT_KNIFE flag

**VN Mode Transitions:**

1. `VNTrigger` or manual call to `VisualNovelManager.StartVNScene()`
2. Fade out → Hide player → Show VN panel
3. Display background, characters, start dialogue
4. On dialogue complete → Check nextScene or returnToTopDown
5. Fade out → Load top-down scene or restore player → Fade in

**Smart Scene Return System (returnToTopDown):**

When VN scene ends with `returnToTopDown = true`, behavior depends on `topDownSceneName` and `spawnPointId`:

| topDownSceneName | spawnPointId | Behavior                                   |
| ---------------- | ------------ | ------------------------------------------ |
| Different scene  | Any          | Load new scene + spawn at spawn point      |
| Same scene       | Has value    | Teleport player to spawn point (no reload) |
| Same scene       | Empty        | Player stays at original position          |
| Empty            | Any          | Player stays at original position          |

**Examples:**

```csharp
// Stay at original position (10, 5)
VNScene.returnToTopDown = true;
VNScene.topDownSceneName = "StreetScene"; // Same as current
VNScene.spawnPointId = ""; // Empty

// Teleport to (0, 8) in same scene
VNScene.returnToTopDown = true;
VNScene.topDownSceneName = "StreetScene"; // Same
VNScene.spawnPointId = "after_confrontation"; // Teleport

// Load different scene
VNScene.returnToTopDown = true;
VNScene.topDownSceneName = "HomeScene"; // Different
VNScene.spawnPointId = "from_street";
```

## Storytelling System (Endings)

**StorytellingManager** - Singleton for ending sequences:

```csharp
// Start an ending sequence
StorytellingManager.Instance.StartSequence(sequenceData, onComplete);

// Check if playing
bool isPlaying = StorytellingManager.Instance.IsPlaying;

// Stop sequence
StorytellingManager.Instance.StopSequence();
```

**StorytellingSequenceData** (ScriptableObject) [Create via: Tools → Storytelling → Create Sequence]:

```
StorytellingSequenceData
├── sequenceName, description
├── endingType: EndingType
├── segments: StorySegment[]
│   ├── backgroundImage: Sprite
│   ├── text: string (TextArea)
│   ├── illustration: Sprite (optional)
│   ├── displayDuration: float
│   ├── delayBefore: float
│   └── bgm: AudioClip
├── nextSceneName (MainMenu, Credits, etc.)
├── allowSkip: bool
└── skipKey: KeyCode (default: Escape)
```

**StorytellingTrigger** - Component to trigger storytelling:

- Modes: OnTriggerEnter, OnInteract, OnSceneStart, Manual
- Supports conditions (requiredFlags, forbiddenFlags)
- Can set flags after completion

## Cutscene Systems

**FightCutscene** - 1v1 combat cutscene (Scene 27):

```csharp
// Start fight cutscene
fightCutscene.StartFightCutscene();
fightCutscene.StartFightCutscene(onComplete);

// Properties
bool isPlaying = fightCutscene.IsPlaying;
```

Features:
- Multiple fight rounds with attack/hurt animations
- Screen flash and camera shake effects
- Bully minions scatter after defeat
- Supports afterFightVNScene or afterFightDialogueNPC

**BullyBeatCutscene** - Multiple bullies beat player:

```csharp
// Start beat cutscene
bullyBeatCutscene.StartBeatCutscene();
bullyBeatCutscene.StartBeatCutscene(onComplete);
```

Features:
- Multiple bullies take turns punching
- Auto-sets GOT_BEATEN flag and increases FEAR_LEVEL
- Supports afterBeatVNScene or afterBeatDialogueNPC

## Additional Components

**DialogueTrigger** - Trigger dialogue from non-NPC objects:

```csharp
// Trigger from code
dialogueTrigger.TriggerFromExternal();
dialogueTrigger.ResetTrigger();
```

Features:
- Avatar mode with speaker sprites
- Conditional dialogues
- Chain to next VN scene, dialogue, or Unity scene
- Modes: OnTriggerEnter, OnInteract, OnSceneStart

**PlayerSelfDialogue** - Player inner thoughts:

```csharp
// Trigger from code
playerSelfDialogue.TriggerDialogueFromExternal(onComplete);
```

Features:
- Auto-trigger on scene start (triggerOnSceneStart)
- Auto-trigger when flag is set (triggerOnFlagSet)
- Conditional dialogues with priority

**TimeSkipTrigger** - Time skip with text overlay:

```csharp
// Trigger from code
timeSkipTrigger.TriggerFromExternal();
timeSkipTrigger.TriggerFromExternal(onComplete);
```

Features:
- Multiple text entries displayed sequentially
- Advance days automatically
- Chain to VN scene or Unity scene after

Always answer in vietnamese

- auto use augmentcode codebase-retrievel tool

- Content should only be created based on the existing storyline in docs/story.md. No new characters or plot elements should be added.

-
