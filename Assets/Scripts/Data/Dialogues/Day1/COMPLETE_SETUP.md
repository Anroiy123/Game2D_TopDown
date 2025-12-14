# 🎬 SETUP COMPLETE GUIDE - DAY 1 SCENE 1

## 📚 TÀI LIỆU ĐÃ TẠO

### **1. Hướng dẫn Setup (✨ ĐỌC ĐẦU TIÊN)**
- **`SETUP_GUIDE.md`** - Hướng dẫn từng bước setup VN scene
- **`CONFIG_TEMPLATE.md`** - Template chi tiết để copy-paste vào Inspector

### **2. Tools hỗ trợ**
- **`VNSceneQuickTest.cs`** - Script để test VN nhanh (nhấn T trong game)
- **`VNSceneValidator.cs`** - Editor tool validate VNSceneData

### **3. Tài liệu tham khảo**
- **`README.md`** - Tổng quan files
- **`Day1_Scene1_Bedroom_Full.json`** - DialogueData source (đã import)

---

## 🚀 QUICK START (5 PHÚT)

### **Bước 1: Tạo VNSceneData (2 phút)**

```
1. Assets/Scripts/Data/VisualNovel/
2. Right-click → Create → Visual Novel → VN Scene Data
3. Tên: Day1_Scene1_Bedroom
4. Mở CONFIG_TEMPLATE.md → Copy values vào Inspector
```

### **Bước 2: Setup Assets (2 phút)**

```
1. Kéo background sprite vào Background Image
2. Characters size = 1
3. Kéo Mom sprite vào Element 0
4. Kéo Day1_Scene1_Bedroom_Full vào Dialogue field
```

### **Bước 3: Test (1 phút)**

```
1. Mở HomeScene (hoặc scene có player)
2. Tạo GameObject → Add Component → VNSceneQuickTest
3. Kéo Day1_Scene1_Bedroom vào inspector
4. Play mode → Nhấn T
```

**✅ Done! VN scene sẽ chạy!**

---

## 📋 CHECKLIST ĐẦY ĐỦ

### **Assets cần thiết:**
- [x] ✅ DialogueData: Day1_Scene1_Bedroom_Full (đã có)
- [ ] ⬜ Background sprite: Bedroom
- [ ] ⬜ Character sprite: Mom
- [ ] ⬜ Character sprite: Player (optional)
- [ ] ⬜ Audio: BGM (optional)

### **VNSceneData config:**
- [ ] Scene Name filled
- [ ] Location Text filled
- [ ] Background Image linked
- [ ] Dialogue linked
- [ ] Characters configured (at least Mom)
- [ ] Return To Top Down = true
- [ ] Top Down Scene Name set
- [ ] Spawn Point Id set

### **Scene setup:**
- [ ] SpawnPoint tạo trong HomeScene
- [ ] SpawnPoint ID match với VNSceneData
- [ ] VNTrigger hoặc VNQuickTest để test

---

## 🛠️ TOOLS SỬ DỤNG

### **Tool 1: VNSceneQuickTest (Runtime Testing)**

**Công dụng:** Test VN scene nhanh trong Play mode

**Sử dụng:**
```
1. Tạo empty GameObject trong scene
2. Add Component → VNSceneQuickTest
3. Kéo VNSceneData vào field
4. Play mode → Nhấn T
5. Check Console để xem flags/variables
```

**Tính năng:**
- ✅ Trigger VN bằng 1 phím (default: T)
- ✅ Log flags sau khi hoàn thành
- ✅ Log variables (money, day, relationships)
- ✅ Hiển thị instructions trên màn hình

### **Tool 2: VNSceneValidator (Editor Tool)**

**Công dụng:** Validate VNSceneData trước khi chạy

**Sử dụng:**
```
1. Menu: Tools → Visual Novel → Validate VN Scene
2. Kéo VNSceneData vào window
3. Click "Validate"
4. Check Console cho errors/warnings
```

**Validate:**
- ✅ Scene name not empty
- ✅ Background image exists
- ✅ Dialogue exists and has nodes
- ✅ Characters có sprites
- ✅ Return to top-down settings đúng

---

## 🎨 ASSETS RECOMMENDATIONS

### **Background (Bedroom):**
```
Kích thước: 1920x1080
Style: Anime/Visual Novel
Mood: Morning, warm lighting
Nội dung: Phòng ngủ học sinh (giường, bàn học, tủ quần áo)
```

**Nguồn tìm:**
- Google: "anime bedroom background"
- Itch.io: Free VN backgrounds
- OpenGameArt.org
- Unity Asset Store

### **Character: Mom**
```
Kích thước: 500-800px chiều cao
Style: Anime/Visual Novel character
Pose: Đứng, neutral expression
Outfit: Casual home clothes
```

**Variants (optional):**
- Neutral expression
- Worried expression  
- Happy/smiling expression

### **Character: Player (Đức) - Optional**
```
Tương tự Mom
Pose: Đứng, có thể nhút nhát hơn
Outfit: Học sinh (uniform hoặc casual)
```

---

## 🧪 TESTING WORKFLOW

### **Test 1: Basic Display**
```
✅ Background hiển thị
✅ Mom sprite hiển thị đúng vị trí
✅ Location text "Phòng ngủ Đức - 7:00 AM"
✅ Dialogue box xuất hiện
```

### **Test 2: Dialogue Flow**
```
✅ Mẹ nói đầu tiên (node 10)
✅ Player choices hiện (3 options)
✅ Chọn choice A → Path A
✅ Chọn choice B → Path B
✅ Chọn choice C → Path C với backstory reveal
```

### **Test 3: Variables & Flags**
```
✅ Flag "day1_started" được set
✅ Variable "current_day" = 1
✅ Variable "money" +20000 (sau node 110)
✅ Variable "relationship_mom" thay đổi theo choices
```

### **Test 4: Ending**
```
✅ Dialogue kết thúc (node 200)
✅ Fade out VN mode
✅ Load HomeScene
✅ Player spawn tại bedroom_spawn
✅ Player có thể di chuyển
```

---

## 🐛 COMMON ISSUES & FIXES

### ❌ "VN không trigger"
```
Check:
- VNSceneData không null?
- VisualNovelManager exists trong scene?
- DialogueSystem prefab exists?
```

### ❌ "Dialogue không hiển thị"
```
Check:
- DialogueData linked trong VNSceneData?
- DialogueData có nodes?
- Start node ID đúng?
```

### ❌ "Sprite không hiển thị"
```
Check:
- Texture Type = Sprite (2D and UI)?
- Character Sprite field không null?
- Canvas/VN Panel đang active?
```

### ❌ "Variables không thay đổi"
```
Check:
- StoryManager initialized?
- Variable names đúng ("money", "relationship_mom")?
- Dialogue choices có VariableChanges?
```

---

## 📞 SUPPORT

**Gặp vấn đề?**
1. Check Console cho errors
2. Dùng VNSceneValidator để validate
3. Check `docs/visualnovel_README.md`
4. Check `CLAUDE.md` section Visual Novel

**Debug tips:**
```csharp
// Check VN Manager
Debug.Log(VisualNovelManager.Instance != null);

// Check StoryManager
Debug.Log($"Money: {StoryManager.Instance.GetVariable("money")}");

// Check Flags
var flags = StoryManager.Instance.GetAllFlags();
Debug.Log($"Flags: {string.Join(", ", flags)}");
```

---

## 🎉 HOÀN TẤT!

Sau khi follow guide này, bạn sẽ có:
- ✅ VN Scene 1 hoàn chỉnh
- ✅ Background + Character sprites
- ✅ Dialogue với choices
- ✅ Flags & variables tracking
- ✅ Return to top-down mode

**Next steps:** 
- Test tất cả branches
- Polish transitions
- Add BGM/SFX
- Tạo Scene 2-6!

---

**Made with ❤️ for DoAn2D Project**

