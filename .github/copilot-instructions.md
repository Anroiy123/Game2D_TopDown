# Unity 2D Classroom Game - Copilot Instructions

## Project Overview

Game 2D top-down classroom simulation built with Unity 6. Player can move around, sit on chairs, and interact with NPCs through dialogue.

## Architecture

### Core Scripts (`Assets/Scripts/`)

| Script              | Purpose                                                      |
| ------------------- | ------------------------------------------------------------ |
| `PlayerMovement.cs` | Player movement, sitting on chairs, talking state management |
| `NPCInteraction.cs` | NPC detection, facing player, triggering dialogue            |
| `DialogueSystem.cs` | Dialogue UI, typewriter effect, conversation flow            |

### Key Patterns

#### State Management

Player uses boolean flags for state control:

```csharp
private bool isSitting = false;
private bool isTalking = false;
private bool isNearNPC = false;
```

#### Animator Parameters (Hash Optimized)

```csharp
private readonly int speedHash = Animator.StringToHash("Speed");
private readonly int horizontalHash = Animator.StringToHash("Horizontal");
private readonly int verticalHash = Animator.StringToHash("Vertical");
```

#### Priority System

NPC interaction takes priority over chair sitting when both are in range (via `isNearNPC` flag).

## Coordinate System

- **Y positive (+)** = Down/Forward (towards bottom of screen)
- **Y negative (-)** = Up/Back (towards top of screen)
- Example: `sitOffset = (0, 1.5, 0)` moves player DOWN to sit in front of chair

## Tags & Layers

| Tag      | Used For                                                    |
| -------- | ----------------------------------------------------------- |
| `Player` | Main player character                                       |
| `NPC`    | All NPC characters (Adam, etc.)                             |
| `Chair`  | Interactive chairs with Box Collider 2D (Is Trigger = true) |

## Animator Setup

### Player Animator Parameters

- `Speed` (Float), `Horizontal` (Float), `Vertical` (Float), `IsSitting` (Bool)

### NPC Animator (Blend Tree)

- Uses 2D Simple Directional blend tree
- Positions: `(0,1)` = back/up, `(0,-1)` = front/down, `(-1,0)` = left, `(1,0)` = right

## UI Structure

```
DialogueCanvas (Screen Space - Overlay)
├── NPCNameText (outside panel, hidden by default)
└── DialoguePanel
    ├── DialogueText
    └── ContinueIcon

NPC/Adam
└── NameCanvas (World Space) - shows name above NPC when player is near
```

## Input Keys

- **WASD/Arrows**: Movement
- **E**: Interact (sit/stand, talk to NPC, continue dialogue)
- **ESC**: Exit dialogue

## Important SerializeField Values

- `sitOffset`: Adjust Y to position player on chair (typically Y = 1.2 to 1.5)
- `interactionRange`: NPC detection radius (default: 2)
- `textSpeed`: Typewriter effect speed (default: 0.05)

## Searching Codebase

- always use codebase-retrieval tool when :
  Don't know which file contains the code you're looking for
  Need an overview of the codebase
  Learn how to implement a feature
  Find code related to a concept
  Start a new task, need to gather context
  Semantic search
