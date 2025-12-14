# 📅 DAY 1 - DIALOGUE & SCENES

**Status:** Scene 1 Complete ✅ | Scenes 2-6 Planned ⏳  
**Last Updated:** 2025-12-13

---

## 📂 FILES

```
Day1/
├── 📄 Day1_Scene1_Bedroom_Dialogue.json    - Simple version ✅
├── 📄 Day1_Scene1_Bedroom_Full.json        - Full version ✅ (Recommended)
└── 📄 README.md                            - This file
```

---

## 🚀 QUICK START

### **Import vào Unity (3 bước):**

```
1. Unity → Tools → Dialogue → Import JSON to DialogueData
2. Chọn: Day1_Scene1_Bedroom_Full.json
3. Import → Tạo VNSceneData → Test!
```

---

## 📋 2 VERSIONS

| File                                | Nodes | Choices | Flags | Recommended   |
| ----------------------------------- | ----- | ------- | ----- | ------------- |
| `Day1_Scene1_Bedroom_Dialogue.json` | 3     | 0       | 0     | ❌ Prototype  |
| `Day1_Scene1_Bedroom_Full.json`     | 17    | 6       | 5     | ✅ Production |

---

## ✅ SCENE 1 STATUS

**Completed:**

- [x] JSON dialogue files (2 versions)
- [x] Complete documentation
- [x] Visual flowcharts
- [x] JSON validation
- [x] Flag/variable system design

**TODO:**

- [ ] Import to Unity
- [ ] Create VNSceneData
- [ ] Add background/character sprites
- [ ] Test in-game
- [ ] Polish & transitions

---

## 🎯 NEXT SCENES (Planned)

| Scene       | Location    | Priority  | Status     |
| ----------- | ----------- | --------- | ---------- |
| **Scene 1** | Bedroom     | 🔥 High   | ✅ Done    |
| **Scene 2** | On the way  | 🟡 Low    | ⏳ Planned |
| **Scene 3** | School gate | 🟠 Medium | ⏳ Planned |
| **Scene 4** | Classroom   | 🔥 High   | ⏳ Planned |
| **Scene 5** | Break time  | 🔥 High   | ⏳ Planned |
| **Scene 6** | End of day  | 🟠 Medium | ⏳ Planned |

See `DAY1_INDEX.md` for detailed outlines.

---

## 📊 METRICS

### **Content Volume:**

- **Total dialogue lines:** ~50 (Full version)
- **Choice points:** 6
- **Unique branches:** 5
- **Flags tracked:** 5
- **Variables tracked:** 2

### **Estimated Playtime:**

- **First playthrough:** 2-3 minutes
- **Full exploration:** 8-10 minutes (all branches)

### **Replayability:**

- **Meaningful choices:** 6
- **Different outcomes:** 5 unique paths
- **Max relationship:** +10 points
- **Min relationship:** 0 points

---

## 🎨 ASSET REQUIREMENTS

### **Required (Minimum):**

- ✅ Dialogue JSON ✅ (Done)
- ⬜ Background sprite: Bedroom
- ⬜ Character sprite: Mẹ
- ⬜ BGM: Morning theme

### **Recommended:**

- ⬜ Multiple character expressions
- ⬜ Sound effects (dialogue beeps, choices)
- ⬜ Ambience audio

---

## 🔗 REFERENCES

### **External Docs:**

- Main project: `README.md`
- Dialogue system: `docs/dialogue.md`
- JSON workflow: `docs/json_workflow.md`
- Quick start: `docs/json_import_quickstart.md`

### **Code References:**

- `DialogueSystem.cs` - UI logic
- `DialogueData.cs` - Data structure
- `DialogueJsonImporter.cs` - Import tool
- `StoryManager.cs` - Flags & variables

---

## 💡 TIPS

### **For Writers:**

- Use Full version as template for other scenes
- Keep dialogue natural and concise
- Always include player agency (choices)
- Balance story vs gameplay

### **For Developers:**

- Import JSON (don't create manually)
- Test all branches thoroughly
- Use Preview feature before importing
- Track flags in Debug logs

### **For Artists:**

- Background: Warm morning lighting
- Character: School-appropriate clothing
- Expressions: Neutral, worried, happy minimum
- Style: Consistent with game art direction

---

## 📞 SUPPORT

**Issues?**

1. Verify JSON syntax with Preview tool
2. Check Unity console for errors
3. Review `docs/dialogue.md` for system details

**Questions?**

- See documentation in `docs/` folder
- Check `CLAUDE.md` for technical details

---

## 🎉 READY TO START!

**Tất cả files đã sẵn sàng để bạn bắt đầu implement Scene 1!**

**Start here:** Unity → Tools → Dialogue → Import JSON to DialogueData 🚀

---

**Made with ❤️ by Claude Agent**  
**Project:** DoAn2D - School Bullying VN Game
