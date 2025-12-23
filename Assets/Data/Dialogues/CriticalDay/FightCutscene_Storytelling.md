# 🥊 FIGHT CUTSCENE - STORYTELLING CONTENT

> **Chế độ**: Visual Novel Storytelling (Background + Text)
> **Backgrounds**: fightScene1.png, fightScene2.png, fightScene3.png

---

## 📖 NỘI DUNG FIGHT CUTSCENE

### VNScene 1: Đối mặt (fightScene1)
**Background**: `fightScene1.png` (Đức và Thủ lĩnh đối mặt)

```
[Text hiển thị trên background]

Đức và Thủ lĩnh đối mặt nhau.

Không khí căng thẳng đến tột độ.

Thủ lĩnh cười khẩy: "Mày nghĩ mày thắng được tao à?"

Đức nắm chặt tay, tim đập thình thịch.

"Tao không còn sợ mày nữa."
```

---

### VNScene 2: Đánh nhau (fightScene2)
**Background**: `fightScene2.png` (Cảnh đánh nhau)

```
[Text hiển thị trên background]

Thủ lĩnh lao vào đánh trước.

Đức né tránh, rồi đánh trả.

Hai người vật lộn trên đường.

Đức bị đấm vào mặt, nhưng không ngã.

Đức đứng dậy, tiếp tục chiến đấu.

"Tao sẽ không chạy nữa!"
```

---

### VNScene 3: Đức thắng (fightScene3)
**Background**: `fightScene3.png` (Đức đứng, Thủ lĩnh nằm)

```
[Text hiển thị trên background]

Cuối cùng, Thủ lĩnh ngã xuống.

Đức đứng, thở hổn hển.

Tay run rẩy, nhưng trong lòng nhẹ nhõm.

Lần đầu tiên, Đức cảm thấy mình không còn yếu đuối nữa.

"Tao... tao thắng rồi..."

Đám bạn của Thủ lĩnh nhìn Đức, rồi bỏ chạy.

Không ai dám đụng đến Đức nữa.
```

---

## 🎯 VNSequenceData Structure

```
FightCutscene (VNSequenceData)
├── Scene 1: Đối mặt (fightScene1.png)
├── Scene 2: Đánh nhau (fightScene2.png)
└── Scene 3: Đức thắng (fightScene3.png)
    └── returnToTopDown = true
    └── topDownSceneName = "HomeScene"
    └── spawnPointId = "after_fight"
```

---

## 🎨 Background Mapping

| VNScene | Background File | Nội dung |
|---------|----------------|----------|
| Scene 1 | `fightScene1.png` | Đức và Thủ lĩnh đối mặt |
| Scene 2 | `fightScene2.png` | Cảnh đánh nhau |
| Scene 3 | `fightScene3.png` | Đức thắng, Thủ lĩnh nằm |

---

## 🔧 Implementation Steps

### Bước 1: Tạo VNSceneData cho từng scene

**Right-click → Visual Novel → VN Scene Data**

#### FightScene1_Confrontation.asset
- **Background**: `fightScene1.png`
- **Dialogue**: Không cần (chỉ dùng text overlay)
- **Location Text**: "Đường về nhà - Chiều muộn"
- **Next Scene**: FightScene2_Fighting.asset

#### FightScene2_Fighting.asset
- **Background**: `fightScene2.png`
- **Dialogue**: Không cần
- **Next Scene**: FightScene3_Victory.asset

#### FightScene3_Victory.asset
- **Background**: `fightScene3.png`
- **Dialogue**: Không cần
- **returnToTopDown**: true
- **topDownSceneName**: "HomeScene"
- **spawnPointId**: "after_fight"

---

### Bước 2: Tạo VNSequenceData

**Right-click → Visual Novel → VN Sequence**

**FightCutscene.asset**:
- **Sequence Name**: "Fight Cutscene"
- **Scenes**: [FightScene1, FightScene2, FightScene3]
- **Day Number**: 8 (Critical Day)
- **Time of Day**: Afternoon

---

### Bước 3: Tạo DialogueData cho text overlay

Vì VNScene không hỗ trợ text overlay trực tiếp, ta cần dùng **DialogueData** với **Narrator**:

**File**: `FightCutscene_Scene1_Dialogue.json`

```json
{
  "conversationName": "FightCutscene_Scene1_Confrontation",
  "startNodeId": 0,
  "nodes": [
    {
      "id": 0,
      "speaker": "Narrator",
      "isPlayer": false,
      "lines": [
        "Đức và Thủ lĩnh đối mặt nhau.",
        "Không khí căng thẳng đến tột độ."
      ],
      "next": 10
    },
    {
      "id": 10,
      "speaker": "Thủ lĩnh",
      "isPlayer": false,
      "lines": ["Mày nghĩ mày thắng được tao à?"],
      "next": 20
    },
    {
      "id": 20,
      "speaker": "Narrator",
      "isPlayer": false,
      "lines": [
        "Đức nắm chặt tay, tim đập thình thịch."
      ],
      "next": 30
    },
    {
      "id": 30,
      "speaker": "Đức",
      "isPlayer": true,
      "lines": ["Tao không còn sợ mày nữa."],
      "next": -1
    }
  ]
}
```

---

## 🎵 Audio Suggestions

- **BGM**: Nhạc căng thẳng, hồi hộp (battle theme)
- **SFX**: 
  - Tiếng đấm (punch)
  - Tiếng ngã (fall)
  - Tiếng thở hổn hển (breathing)

---

## 📝 NEXT STEPS

1. ✅ Đã có 3 backgrounds (fightScene1, fightScene2, fightScene3)
2. [ ] Tạo 3 DialogueData JSON cho text overlay
3. [ ] Import JSON vào Unity
4. [ ] Tạo 3 VNSceneData assets
5. [ ] Tạo 1 VNSequenceData asset
6. [ ] Link DialogueData vào VNSceneData
7. [ ] Test FightCutscene

**Thời gian ước tính**: 30-45 phút

