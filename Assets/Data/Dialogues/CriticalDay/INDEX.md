# 📚 INDEX - CRITICAL DAY SCENE 27-28 & ENDINGS

> **Tổng hợp tất cả files và tài liệu cho Scene 27-28 và 3 Endings**

---

## 📁 JSON FILES (DIALOGUE DATA)

### Scene 27-28 Dialogues:
| File | Mục đích | Nodes | Choices |
|------|----------|-------|---------|
| `CriticalDay_Scene27_Street_NEW.json` | Scene 27 - Gặp tụi bắt nạt | 0-330 | 3 (node 40) |
| `CriticalDay_Scene28A_Home_AfterWin.json` | Scene 28A - Về nhà sau thắng | 0-30 | 0 |
| `CriticalDay_Scene28B_Home_Choice.json` | Scene 28B - Lựa chọn cuối | 0-220 | 2 (node 0) |

### Fight Cutscene Dialogues:
| File | Mục đích | Background |
|------|----------|------------|
| `FightCutscene_Scene1_Confrontation.json` | Đối mặt | fightScene1.png |
| `FightCutscene_Scene2_Fighting.json` | Đánh nhau | fightScene2.png |
| `FightCutscene_Scene3_Victory.json` | Thắng lợi | fightScene3.png |

---

## 📖 DOCUMENTATION FILES

### Hướng dẫn chính:
1. **`SUMMARY.md`** - Tóm tắt tổng quan (ĐỌC ĐẦU TIÊN)
2. **`README_SCENE27_28.md`** - Chi tiết cấu trúc Scene 27-28
3. **`IMPLEMENTATION_GUIDE.md`** - Hướng dẫn implement đầy đủ
4. **`FIGHT_CUTSCENE_SETUP.md`** - Hướng dẫn setup Fight Cutscene

### Nội dung chi tiết:
5. **`ENDING_CONTENT.md`** - Nội dung storytelling 3 endings
6. **`FightCutscene_Storytelling.md`** - Nội dung Fight Cutscene

---

## 🌳 DECISION TREE

```
Scene 27: Gặp tụi bắt nạt
│
├─[A] Đối mặt 1v1
│   └─→ FightCutscene (3 scenes)
│       └─→ Scene 28A: Về nhà
│           └─→ Thú nhận với mẹ
│               └─→ ✅ ENDING 1: Good_StandUp
│
├─[B] Chơi Tù xì
│   └─→ Bị đánh
│       └─→ Scene 28B: Về nhà
│           ├─[1] Thú nhận → ✅ ENDING 2: True_TellParents
│           └─[2] Giấu mẹ → ❌ ENDING 3: Bad_DarkLife
│
└─[C] Từ chối chơi
    └─→ Bị đánh
        └─→ Scene 28B: Về nhà
            ├─[1] Thú nhận → ✅ ENDING 2: True_TellParents
            └─[2] Giấu mẹ → ❌ ENDING 3: Bad_DarkLife
```

---

## 🚩 FLAGS & ACTION IDS

### Scene 27 Flags:
- `critical_scene27_completed`
- `stood_up_to_bullies`
- `got_beaten`
- `played_game_and_lost`
- `refused_game_and_beaten`

### Scene 28 Flags:
- `confessed_to_mom`
- `hid_from_mom`
- `ending1_good_standup`
- `ending2_true_tellparents`
- `ending3_bad_darklife`

### Fight Cutscene Flags:
- `fight_won`
- `stood_up_successfully`

### Action IDs:
- `trigger_fight_cutscene` - Scene 27, Node 130
- `trigger_scene28a` - FightCutscene Scene 3
- `trigger_scene28b` - Scene 27, Node 260/330
- `trigger_ending1_storytelling` - Scene 28A, Node 30
- `trigger_ending2_storytelling` - Scene 28B, Node 120
- `trigger_ending3_storytelling` - Scene 28B, Node 220

---

## 📊 IMPLEMENTATION CHECKLIST

### ✅ Phase 1: JSON & Documentation (COMPLETED)
- [x] Scene 27 JSON
- [x] Scene 28A JSON
- [x] Scene 28B JSON
- [x] FightCutscene JSONs (3 files)
- [x] Documentation files (6 files)

### 🔲 Phase 2: Unity Setup (TODO)
- [ ] Import all JSON files
- [ ] Create VNSceneData for FightCutscene (3 assets)
- [ ] Create VNSequenceData for FightCutscene (1 asset)
- [ ] Create spawn point "after_fight" in HomeScene
- [ ] Update DialogueSystem.cs (action IDs)

### 🔲 Phase 3: Endings (TODO)
- [ ] Create VNSequenceData for Ending 1
- [ ] Create VNSequenceData for Ending 2
- [ ] Create VNSequenceData for Ending 3
- [ ] Create backgrounds/sprites for endings
- [ ] Add BGM for endings

### 🔲 Phase 4: Testing (TODO)
- [ ] Test Scene 27 choices
- [ ] Test FightCutscene flow
- [ ] Test Scene 28A → Ending 1
- [ ] Test Scene 28B → Ending 2
- [ ] Test Scene 28B → Ending 3
- [ ] Test full playthrough (all 3 endings)

---

## 🎯 QUICK START GUIDE

### Bước 1: Đọc tài liệu
1. Đọc `SUMMARY.md` để hiểu tổng quan
2. Đọc `README_SCENE27_28.md` để hiểu cấu trúc
3. Đọc `FIGHT_CUTSCENE_SETUP.md` để setup Fight Cutscene

### Bước 2: Import JSON
1. Menu: `Tools → Dialogue → Import JSON to DialogueData`
2. Import 6 JSON files (Scene 27, 28A, 28B, Fight x3)

### Bước 3: Setup Fight Cutscene
1. Tạo 3 VNSceneData (theo `FIGHT_CUTSCENE_SETUP.md`)
2. Tạo 1 VNSequenceData
3. Tạo spawn point "after_fight"

### Bước 4: Update Code
1. Edit `DialogueSystem.cs`
2. Implement action IDs (theo `IMPLEMENTATION_GUIDE.md`)

### Bước 5: Test
1. Test FightCutscene riêng
2. Test Scene 27 → FightCutscene → Scene 28A
3. Test Scene 27 → Scene 28B

---

## 📚 REFERENCES

- **Story Source**: `docs/story.md` (line 600-960)
- **Ending Structure**: `docs/newEnding.md`
- **Flags Reference**: `docs/flags.md`

---

## ⏱️ ESTIMATED TIME

| Phase | Time |
|-------|------|
| Phase 1: JSON & Docs | ✅ DONE |
| Phase 2: Unity Setup | 30-45 min |
| Phase 3: Endings | 4-6 hours |
| Phase 4: Testing | 30-60 min |
| **TOTAL** | **5-8 hours** |

---

## 🎨 ASSETS NEEDED

### Backgrounds (có sẵn):
- ✅ fightScene1.png
- ✅ fightScene2.png
- ✅ fightScene3.png

### Backgrounds (cần tạo):
- [ ] Ending 1 backgrounds (4-6 images)
- [ ] Ending 2 backgrounds (4-6 images)
- [ ] Ending 3 backgrounds (4-6 images)

### Audio:
- [ ] Fight BGM (căng thẳng)
- [ ] Ending 1 BGM (hy vọng)
- [ ] Ending 2 BGM (ấm áp)
- [ ] Ending 3 BGM (u ám)

---

## 📞 SUPPORT

Nếu gặp vấn đề, tham khảo:
1. `FIGHT_CUTSCENE_SETUP.md` → Troubleshooting section
2. `IMPLEMENTATION_GUIDE.md` → Code examples
3. `docs/troubleshooting/` → General Unity issues

---

**Last Updated**: 2025-12-22
**Version**: 1.0
**Status**: Phase 1 Complete, Phase 2-4 Pending

