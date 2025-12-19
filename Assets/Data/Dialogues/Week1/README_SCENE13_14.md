# 📅 WEEK 1 - SCENE 13 & 14 - DIALOGUE FILES

**Status:** JSON Files Created ✅ | Ready for Import ⏳  
**Last Updated:** 2025-12-18

---

## 📂 FILES CREATED

```
Week1/
├── 📄 Week1_Scene13_Street_Followed.json          ✅ NEW
├── 📄 Week1_Scene14A_SecondEncounter.json         ✅ NEW
├── 📄 Week1_Scene14A2_Forced_Friend.json          ✅ NEW
├── 📄 Week1_Scene10_Classroom_Break.json          ✅ Existing
├── 📄 Week1_Scene12_TeacherRoom.json              ✅ Existing
└── 📄 README_SCENE13_14.md                        ✅ This file
```

---

## 🎬 SCENE OVERVIEW

### **Scene 13: Trên đường về (Street - Followed)**
- **Mode**: Top-down → VN transition
- **Location**: StreetScene
- **Content**: Đức phát hiện tụi bắt nạt lại đi theo mình
- **Flags Set**: `week1_scene13_completed`, `bullies_following_week1`

### **Scene 14A: Đối mặt lần 2 (Second Encounter)**
- **Mode**: Visual Novel
- **Location**: Street background
- **Content**: Thủ lĩnh mời Đức kết bạn
- **Flags Set**: `week1_scene14a_completed`, `bully_invitation_received`

### **Scene 14A-2: Ép buộc kết bạn (Forced Friend)**
- **Mode**: Visual Novel
- **Location**: Street background
- **Content**: Đàn em ép buộc, Đức đồng ý kết bạn
- **Flags Set**: `BEFRIENDED_BULLIES`, `week1_scene14a2_completed`
- **Variables**: `FEAR_LEVEL +10`

---

## 🚀 IMPORT INSTRUCTIONS

### **Step 1: Import JSON to DialogueData**

1. Open Unity Editor
2. Go to: `Tools → Dialogue → Import JSON to DialogueData`
3. Import each file:
   - `Week1_Scene13_Street_Followed.json`
   - `Week1_Scene14A_SecondEncounter.json`
   - `Week1_Scene14A2_Forced_Friend.json`

### **Step 2: Create VNSceneData Assets**

#### **For Scene 13 (Optional - mostly top-down):**
```
Scene Name: Week1_Scene13_Street_Followed
Location Text: Đường về nhà - Chiều tà
Background: Street background (or black for narrator)
Characters: None (narrator only)
Dialogue: Week1_Scene13_Street_Followed_Dialogue
Return To Top Down: true
Top Down Scene Name: StreetScene
Spawn Point Id: (current position - no teleport)
```

#### **For Scene 14A:**
```
Scene Name: Week1_Scene14A_SecondEncounter
Location Text: Đường phố - Gặp lại tụi bắt nạt
Background: Street background
Characters:
  - Thủ lĩnh (Center, scale 1.0)
  - Đàn em 1 (Left, scale 0.9)
  - Đàn em 2 (Right, scale 0.9)
Dialogue: Week1_Scene14A_SecondEncounter_Dialogue
Next Scene: None (transition to 14A-1 top-down)
Return To Top Down: false
```

#### **For Scene 14A-2:**
```
Scene Name: Week1_Scene14A2_Forced_Friend
Location Text: Đường phố - Bị vây quanh
Background: Street background
Characters:
  - Thủ lĩnh (Center)
  - Đàn em 1 (FarLeft)
  - Đàn em 2 (Left)
  - Đàn em 3 (Right)
  - Đàn em 4 (FarRight)
Dialogue: Week1_Scene14A2_Forced_Friend_Dialogue
Return To Top Down: true
Top Down Scene Name: StreetScene
Spawn Point Id: after_week1_encounter
Set Flags On Complete: week1_encounter_complete
```

---

## 🎮 GAMEPLAY FLOW

```
Scene 12 (Teacher Room)
    ↓
[Player leaves school]
    ↓
Scene 13 (Street - Top-down)
    ↓ (Player walks)
[Bullies appear and follow]
    ↓ (VNTrigger when close)
Scene 14A (VN - Second Encounter)
    ↓
[Transition to top-down]
    ↓
Scene 14A-1 (Top-down - Surrounded)
    ↓ (VNTrigger)
Scene 14A-2 (VN - Forced Friend)
    ↓
[Return to top-down]
    ↓
Continue to Scene 15 (Home)
```

---

## 🔧 TECHNICAL REQUIREMENTS

### **StreetScene Setup Needed:**

1. **BullyFollowTrigger Zone**
   - Component: `BullyEncounterZone`
   - Position: Near school exit
   - Trigger Flags: `week1_scene12_completed`
   - Skip If Flag: `week1_encounter_complete`

2. **Bully NPCs**
   - Thủ lĩnh (Leader) - with `NPCFollowPlayer`
   - Đàn em 1, 2, 3 - with `NPCFollowPlayer`
   - Initially inactive (activated by trigger)

3. **VNTrigger for Scene 14A**
   - Mode: OnTriggerEnter or OnInteract
   - VN Scene: Week1_Scene14A_SecondEncounter_VNScene
   - Required Flags: `bullies_following_week1`
   - Trigger Once: true

4. **VNTrigger for Scene 14A-2**
   - Mode: OnTriggerEnter (after 14A-1 animation)
   - VN Scene: Week1_Scene14A2_Forced_Friend_VNScene
   - Required Flags: `week1_scene14a_completed`
   - Trigger Once: true

5. **SpawnPoint**
   - ID: `after_week1_encounter`
   - Position: Safe spot after encounter
   - Facing: Down (towards home)

---

## 📊 STORY FLAGS & VARIABLES

### **Flags:**
- `week1_scene13_completed` - Scene 13 đã hoàn thành
- `bullies_following_week1` - Tụi bắt nạt đang đi theo
- `week1_scene14a_completed` - Scene 14A đã hoàn thành
- `bully_invitation_received` - Đã nhận lời mời kết bạn
- `BEFRIENDED_BULLIES` - Đã "kết bạn" với tụi bắt nạt (QUAN TRỌNG)
- `week1_scene14a2_completed` - Scene 14A-2 đã hoàn thành
- `week1_encounter_complete` - Toàn bộ encounter đã xong

### **Variables:**
- `FEAR_LEVEL` +10 (Scene 14A-2) - Tăng mức độ sợ hãi

---

## 🎨 ASSET REQUIREMENTS

### **Required:**
- ✅ Dialogue JSON files (Done)
- ⬜ Street background sprite
- ⬜ Thủ lĩnh character sprite
- ⬜ Đàn em character sprites (x3-4)
- ⬜ BGM: Tense/threatening music

### **Optional:**
- ⬜ Sound effects (footsteps, tension)
- ⬜ Character expressions (intimidating, scared)
- ⬜ Ambient sounds (street noise)

---

## 🔀 ALTERNATIVE PATH: Scene 14B (Run Away)

**Note:** Scene 14B (Chạy lẹ về nhà) is a top-down gameplay section where player controls Duc running home. This doesn't require a JSON dialogue file, but needs:

- Escape trigger zone
- Chase mechanics (bullies follow faster)
- Success condition (reach home door)
- Failure condition (caught by bullies → Scene 14A)

---

## 📝 NEXT STEPS

### **Immediate:**
1. ✅ Create JSON files (DONE)
2. ⬜ Import to Unity as DialogueData
3. ⬜ Create VNSceneData assets
4. ⬜ Test dialogue flow in VN mode

### **Scene Setup:**
5. ⬜ Setup StreetScene with trigger zones
6. ⬜ Create/configure Bully NPC prefabs
7. ⬜ Add NPCFollowPlayer components
8. ⬜ Create spawn points
9. ⬜ Test full gameplay flow

### **Polish:**
10. ⬜ Add character sprites
11. ⬜ Add background art
12. ⬜ Add music/SFX
13. ⬜ Playtest and balance

---

## 🐛 TROUBLESHOOTING

### **Issue: Bullies don't follow player**
- Check `NPCFollowPlayer` component is enabled
- Verify `isFollowing` is set to true
- Check `minDistance` and `maxDistance` settings
- Ensure animator has Speed parameter

### **Issue: VN Scene doesn't trigger**
- Check `VNTrigger` required flags
- Verify `VNSceneData` reference is set
- Check collider is trigger and overlaps player
- Look for errors in Console

### **Issue: Player can't move after VN**
- Check `returnToTopDown` is true
- Verify spawn point exists
- Check `PlayerMovement` isn't stuck in dialogue state

---

## 📞 REFERENCES

- Main story: `docs/story.md` (lines 199-250)
- VN system: `docs/visualnovel_README.md`
- Dialogue system: `docs/dialogue.md`
- JSON workflow: `docs/json_workflow.md`

---

**Created by:** Augment Agent  
**Date:** 2025-12-18  
**Version:** 1.0

