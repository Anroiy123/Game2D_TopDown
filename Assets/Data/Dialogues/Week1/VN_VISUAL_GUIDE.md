# 📸 VISUAL GUIDE - VN SCENE SETUP

**Hướng dẫn bằng hình ảnh và sơ đồ**

---

## 🎯 TỔNG QUAN WORKFLOW

```
┌─────────────────────────────────────────────────────────────┐
│                    VN SCENE SETUP WORKFLOW                  │
└─────────────────────────────────────────────────────────────┘

Step 1: Import JSON
    📄 Week1_Scene14A_SecondEncounter.json
         ↓ [Tools → Dialogue → Import JSON]
    📦 Week1_Scene14A_SecondEncounter_Dialogue.asset

Step 2: Create VNSceneData
    Right-click → Create → Visual Novel → VN Scene Data
         ↓
    📦 Week1_Scene14A_SecondEncounter_VNScene.asset

Step 3: Configure Inspector
    ├─ Scene Data
    │   ├─ Scene Name
    │   ├─ Location Text
    │   ├─ Background Image
    │   ├─ Characters [Array]
    │   └─ Dialogue [Reference]
    ├─ Conditions
    │   ├─ Required Flags
    │   └─ Forbidden Flags
    └─ Effects
        └─ Set Flags On Complete

Step 4: Test
    VNSceneQuickTest → Press T → Play VN
```

---

## 📋 INSPECTOR LAYOUT - SCENE 14A

```
┌────────────────────────────────────────────────────────────┐
│ Week1_Scene14A_SecondEncounter_VNScene (VNSceneData)      │
├────────────────────────────────────────────────────────────┤
│                                                            │
│ ▼ Scene Data                                               │
│   ┌──────────────────────────────────────────────────┐    │
│   │ Scene Name: Week1_Scene14A_SecondEncounter       │    │
│   │ Location Text: Đường phố - Gặp lại tụi bắt nạt   │    │
│   │                                                   │    │
│   │ Background Image: [🖼️ street_background]        │    │
│   │ Background Tint: ⬜ (1, 1, 1, 1)                 │    │
│   │                                                   │    │
│   │ ▼ Characters                          Size: 3    │    │
│   │   ┌─────────────────────────────────────────┐   │    │
│   │   │ Element 0                               │   │    │
│   │   │ Character Sprite: [🖼️ Bully_idle]      │   │    │
│   │   │ Character Name: Thủ lĩnh                │   │    │
│   │   │ Position: Center ▼                      │   │    │
│   │   │ Position Offset: (0, -50)               │   │    │
│   │   │ Scale: (1, 1)                           │   │    │
│   │   │ Flip X: ☐                               │   │    │
│   │   └─────────────────────────────────────────┘   │    │
│   │   ┌─────────────────────────────────────────┐   │    │
│   │   │ Element 1                               │   │    │
│   │   │ Character Sprite: [🖼️ DanEm1_idle]     │   │    │
│   │   │ Character Name: Đàn em                  │   │    │
│   │   │ Position: Left ▼                        │   │    │
│   │   │ Position Offset: (0, -50)               │   │    │
│   │   │ Scale: (0.9, 0.9)                       │   │    │
│   │   │ Flip X: ☐                               │   │    │
│   │   └─────────────────────────────────────────┘   │    │
│   │   ┌─────────────────────────────────────────┐   │    │
│   │   │ Element 2                               │   │    │
│   │   │ Character Sprite: [🖼️ DanEm2_idle]     │   │    │
│   │   │ Character Name: Đàn em                  │   │    │
│   │   │ Position: Right ▼                       │   │    │
│   │   │ Position Offset: (0, -50)               │   │    │
│   │   │ Scale: (0.9, 0.9)                       │   │    │
│   │   │ Flip X: ☑                               │   │    │
│   │   └─────────────────────────────────────────┘   │    │
│   │                                                   │    │
│   │ Dialogue: [📦 Week1_Scene14A_..._Dialogue]      │    │
│   │ BGM: None (Audio Clip)                           │    │
│   │ Ambience: None (Audio Clip)                      │    │
│   │                                                   │    │
│   │ Next Scene: None (VN Scene Data)                 │    │
│   │ Return To Top Down: ☐ false                      │    │
│   │ Top Down Scene Name:                             │    │
│   │ Spawn Point Id:                                  │    │
│   └──────────────────────────────────────────────────┘    │
│                                                            │
│ ▼ Conditions                                               │
│   ┌──────────────────────────────────────────────────┐    │
│   │ ▼ Required Flags                      Size: 1    │    │
│   │   Element 0: bullies_following_week1             │    │
│   │                                                   │    │
│   │ ▼ Forbidden Flags                     Size: 1    │    │
│   │   Element 0: week1_encounter_complete            │    │
│   └──────────────────────────────────────────────────┘    │
│                                                            │
│ ▼ Effects On Enter                                         │
│   ┌──────────────────────────────────────────────────┐    │
│   │ Set Flags On Enter:                   Size: 0    │    │
│   │ Variable Changes On Enter:            Size: 0    │    │
│   └──────────────────────────────────────────────────┘    │
│                                                            │
│ ▼ Effects On Complete                                      │
│   ┌──────────────────────────────────────────────────┐    │
│   │ Set Flags On Complete:                Size: 0    │    │
│   └──────────────────────────────────────────────────┘    │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

---

## 🎨 CHARACTER POSITIONS DIAGRAM

### **Scene 14A - 3 Characters:**

```
┌────────────────────────────────────────────────────┐
│                  STREET BACKGROUND                 │
│                                                    │
│                                                    │
│         👤                👤              👤       │
│       Đàn em           Thủ lĩnh         Đàn em     │
│       (Left)           (Center)         (Right)    │
│      Scale 0.9         Scale 1.0       Scale 0.9   │
│      Flip: No          Flip: No        Flip: Yes   │
│                                                    │
│                                                    │
│              [Dialogue Panel Here]                 │
│                                                    │
└────────────────────────────────────────────────────┘
```

### **Scene 14A-2 - 5 Characters (Surrounded):**

```
┌────────────────────────────────────────────────────┐
│              STREET BACKGROUND (Darker)            │
│                                                    │
│  👤        👤           👤          👤        👤   │
│ Đàn em   Đàn em      Thủ lĩnh     Đàn em   Đàn em │
│(FarLeft)  (Left)     (Center)     (Right) (FarRight)│
│Scale 0.85 Scale 0.9  Scale 1.1  Scale 0.9 Scale 0.85│
│                                                    │
│                    [Player]                        │
│                  (Bị vây quanh)                    │
│                                                    │
│              [Dialogue Panel Here]                 │
│                                                    │
└────────────────────────────────────────────────────┘
```

---

## 🔄 SCENE FLOW DIAGRAM

```
┌─────────────────────────────────────────────────────────┐
│                   SCENE 13 → 14 FLOW                    │
└─────────────────────────────────────────────────────────┘

Scene 12: Teacher Room (VN)
    ↓
[Player exits school]
    ↓
┌─────────────────────────────────────┐
│ Scene 13: Street (Top-down)         │
│ - Player walks                      │
│ - Bullies appear behind             │
│ - NPCFollowPlayer activated         │
└─────────────────────────────────────┘
    ↓ [VNTrigger when close]
┌─────────────────────────────────────┐
│ Scene 14A: Second Encounter (VN)    │
│ - Thủ lĩnh invites friendship       │
│ - Đức silent                        │
│ - Set flag: bully_invitation        │
└─────────────────────────────────────┘
    ↓ [Return to top-down: false]
┌─────────────────────────────────────┐
│ Scene 14A-1: Surrounded (Top-down)  │
│ - Animation: Bullies surround       │
│ - Player can't move                 │
│ - Short cutscene                    │
└─────────────────────────────────────┘
    ↓ [VNTrigger immediate]
┌─────────────────────────────────────┐
│ Scene 14A-2: Forced Friend (VN)     │
│ - Đàn em pressure                   │
│ - Đức accepts                       │
│ - Set flag: BEFRIENDED_BULLIES      │
│ - FEAR_LEVEL +10                    │
└─────────────────────────────────────┘
    ↓ [Return to top-down: true]
┌─────────────────────────────────────┐
│ StreetScene (Top-down)              │
│ - Spawn at: after_week1_encounter   │
│ - Bullies walk away                 │
│ - Player can continue home          │
└─────────────────────────────────────┘
    ↓
Scene 15: Home
```

---

## 🎯 QUICK REFERENCE TABLE

| Scene    | VN Mode | Characters | Return Top-Down | Spawn Point           |
|----------|---------|------------|-----------------|----------------------|
| Scene 13 | Optional| 0          | true            | (current position)   |
| Scene 14A| Yes     | 3          | **false**       | N/A                  |
| Scene 14A-2| Yes  | 5          | **true**        | after_week1_encounter|

---

## 📝 COPY-PASTE VALUES

### **Scene 14A Character Positions:**

```
Character 0 (Thủ lĩnh):
  Position: Center
  Offset: (0, -50)
  Scale: (1, 1)
  Flip: false

Character 1 (Đàn em Left):
  Position: Left
  Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip: false

Character 2 (Đàn em Right):
  Position: Right
  Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip: true
```

### **Scene 14A-2 Character Positions:**

```
Character 0 (Thủ lĩnh):
  Position: Center
  Offset: (0, -50)
  Scale: (1.1, 1.1)
  Flip: false

Character 1 (Đàn em FarLeft):
  Position: FarLeft
  Offset: (0, -50)
  Scale: (0.85, 0.85)
  Flip: false

Character 2 (Đàn em Left):
  Position: Left
  Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip: false

Character 3 (Đàn em Right):
  Position: Right
  Offset: (0, -50)
  Scale: (0.9, 0.9)
  Flip: true

Character 4 (Đàn em FarRight):
  Position: FarRight
  Offset: (0, -50)
  Scale: (0.85, 0.85)
  Flip: true
```

---

## ⚠️ COMMON MISTAKES

### **❌ Mistake 1: Wrong Return To Top Down**
```
Scene 14A:
  Return To Top Down: true  ← WRONG!
```
**Fix:** Set to `false` vì cần chuyển sang 14A-1 animation

### **❌ Mistake 2: Missing Spawn Point**
```
Scene 14A-2:
  Spawn Point Id: [Empty]  ← WRONG!
```
**Fix:** Set to `after_week1_encounter`

### **❌ Mistake 3: Wrong Character Scale**
```
All characters: Scale (1, 1)  ← Looks weird
```
**Fix:** Leader = 1.0-1.1, Minions = 0.85-0.9

### **❌ Mistake 4: Forgot Flip X**
```
Right side characters: Flip X = false  ← Face wrong way
```
**Fix:** Right/FarRight characters should Flip X = true

---

## 🧪 TESTING CHECKLIST

### **Before Testing:**
- [ ] All 3 DialogueData imported
- [ ] All VNSceneData created
- [ ] Character sprites assigned
- [ ] Background assigned
- [ ] Flags configured

### **Test Scene 14A:**
- [ ] VN starts correctly
- [ ] 3 characters visible
- [ ] Dialogue flows correctly
- [ ] Ends without returning to top-down
- [ ] Sets correct flags

### **Test Scene 14A-2:**
- [ ] VN starts after 14A
- [ ] 5 characters visible (surrounded)
- [ ] Dialogue flows correctly
- [ ] Returns to top-down
- [ ] Spawns at correct position
- [ ] Sets BEFRIENDED_BULLIES flag

---

**Next:** Setup StreetScene với trigger zones và NPCs
**See:** `STREETSCENE_SETUP_GUIDE.md`

