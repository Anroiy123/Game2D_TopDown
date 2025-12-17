# TỔNG QUAN HỆ THỐNG TƯƠNG TÁC

Hệ thống hiển thị visual feedback để player biết đâu là đối tượng có thể tương tác.

---

## 📦 2 LOẠI INDICATORS

### **1. InteractionIndicator (Animated)**

**Dùng cho:** NPCs, quest givers, important objects

**Đặc điểm:**
- ✅ Animation nhiều frames (dấu hỏi nhấp nháy, chấm than, v.v.)
- ✅ Bounce effect (nhảy lên xuống)
- ✅ Tự động ẩn sau khi tương tác
- ✅ Tích hợp với NPCInteraction

**Sprites:** `UI_thinking_emote`, `UI_angry_emote`, `UI_mail`, `UI_arrow_pointing`

---

### **2. SimpleInteractionPrompt (Static/Text)**

**Dùng cho:** Doors, beds, chairs, teleporters

**Đặc điểm:**
- ✅ Đơn giản (1 sprite hoặc text "E")
- ✅ Fade in/out smooth
- ✅ Nhẹ, tối ưu performance
- ✅ Dễ setup

**Sprites:** Key icons, hand icons, hoặc text

---

## 🎨 SO SÁNH

| Feature | InteractionIndicator | SimpleInteractionPrompt |
|---------|---------------------|------------------------|
| **Animation** | ✅ Multi-frame | ❌ Single sprite/text |
| **Bounce** | ✅ Yes | ❌ No |
| **Fade** | ❌ No | ✅ Yes |
| **Auto Hide** | ✅ Yes | ❌ Manual |
| **Performance** | Medium | High |
| **Setup Time** | 2-3 min | 30 sec |
| **Use Case** | NPCs, Quests | Doors, Objects |

---

## 🚀 QUICK START

### **Cho NPCs (Animated):**

```
1. Chọn NPC GameObject
2. Menu → Tools → Interaction → Setup Indicator on Selected NPC
3. Kéo animation frames vào Inspector
4. Done!
```

### **Cho Objects (Simple):**

```
1. Chọn Object (Door, Bed, Chair)
2. Add Component → Simple Interaction Prompt
3. Chọn Use Text = true, Prompt Text = "E"
4. Done!
```

---

## 📋 VÍ DỤ THỰC TẾ

### **Mom NPC:**
```
Component: InteractionIndicator
Sprites: UI_thinking_emote_dots (6 frames)
Offset: (0, 1.5, 0)
Show Distance: 3
Hide After Interaction: ✓
```

### **Door:**
```
Component: SimpleInteractionPrompt
Use Text: ✓
Prompt Text: "E"
Offset: (0, 1, 0)
Show Distance: 2
Use Fade: ✓
```

### **Bed:**
```
Component: SimpleInteractionPrompt
Use Text: ✓
Prompt Text: "E"
Offset: (0, 0.5, 0)
Show Distance: 1.5
```

---

## 🎯 BEST PRACTICES

### **Khi nào dùng InteractionIndicator:**
- ✅ NPCs quan trọng (Mom, Teacher, Bullies)
- ✅ Quest givers
- ✅ Story-critical objects
- ✅ Cần thu hút attention của player

### **Khi nào dùng SimpleInteractionPrompt:**
- ✅ Doors, teleporters
- ✅ Beds, chairs
- ✅ Decorative objects
- ✅ Cần tối ưu performance

---

## 🛠️ EDITOR TOOLS

### **Tools → Interaction → Setup Indicator on Selected NPC**
Tự động setup InteractionIndicator cho NPC.

### **Tools → Interaction → Create Indicator Prefabs**
Tạo prefabs cho các loại indicators.

---

## 📚 TÀI LIỆU CHI TIẾT

- **[interaction_indicator_guide.md](./interaction_indicator_guide.md)** - Hướng dẫn chi tiết InteractionIndicator
- **[NPC.md](./NPC.md)** - Hướng dẫn setup NPCs

---

## 🎮 INTEGRATION VỚI HỆ THỐNG KHÁC

### **NPCInteraction:**
```csharp
[SerializeField] private InteractionIndicator interactionIndicator;

private void OnDialogueEnd()
{
    if (interactionIndicator != null)
    {
        interactionIndicator.OnInteracted(); // Ẩn indicator
    }
}
```

### **DoorController:**
```csharp
private SimpleInteractionPrompt prompt;

private void Start()
{
    prompt = GetComponent<SimpleInteractionPrompt>();
}

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player") && prompt != null)
    {
        prompt.Show();
    }
}
```

---

## ✅ CHECKLIST SETUP

### **Cho NPCs:**
- [ ] Import sprites từ Animated_Spritesheets
- [ ] Add InteractionIndicator component
- [ ] Gán animation frames
- [ ] Link với NPCInteraction
- [ ] Test: Icon xuất hiện khi đến gần

### **Cho Objects:**
- [ ] Add SimpleInteractionPrompt component
- [ ] Chọn Use Text hoặc gán Sprite
- [ ] Điều chỉnh Offset và Show Distance
- [ ] Test: Prompt xuất hiện khi đến gần

---

## 🎨 ASSETS REQUIREMENTS

**Animated Indicators:**
- `UI_thinking_emote_dots_*.gif` (6-8 frames)
- `UI_angry_emote_*.gif` (8 frames)
- `UI_mail_*.gif` (4 frames)
- `UI_arrow_pointing_*.gif` (4 frames)

**Simple Prompts:**
- Key icons (E, Space, etc.)
- Hand icons
- Hoặc dùng TextMesh (không cần sprites)

---

## 🐛 COMMON ISSUES

**Icon không hiển thị:**
- Kiểm tra Animation Frames đã gán
- Kiểm tra Show Distance
- Kiểm tra Sorting Layer = "UI"

**Icon bị che:**
- Tăng Sorting Order
- Tăng Offset.y

**Performance issues:**
- Dùng SimpleInteractionPrompt thay vì InteractionIndicator
- Giảm số lượng animation frames
- Disable bounce effect

