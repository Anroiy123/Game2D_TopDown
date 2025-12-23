# 📖 HƯỚNG DẪN SETUP 3 ENDINGS (STORYTELLING)

> Tài liệu chi tiết từng segment, text lines, và background cần thiết cho mỗi ending.

---

## 📁 CẤU TRÚC FILE

```
Assets/Data/Storytelling/
├── Ending1_GoodStandUp_Sequence.asset
├── Ending2_TrueTellParents_Sequence.asset
├── Ending3_BadDarkLife_Sequence.asset
└── ENDINGS_SETUP_GUIDE.md (file này)

Assets/Resources/Storytelling/  (QUAN TRỌNG - để Resources.Load() hoạt động)
├── Ending1_Good_StandUp.asset (copy từ trên)
├── Ending2_True_TellParents.asset
└── Ending3_Bad_DarkLife.asset
```

---

## ✅ ENDING 1: GOOD_STANDUP (6 Segments)

**Điều kiện trigger**: `stood_up_to_bullies` = true + `confessed_to_mom` = true
**Action ID**: `trigger_ending1_storytelling`
**Ending Type**: `Good_StandUp`
**Next Scene**: `MainMenu`
**Tone**: Tích cực, hy vọng

---

### Segment 0: Tối hôm đó

| Field | Value |
|-------|-------|
| Background Image | `bg_livingroom_night.png` (Phòng khách ban đêm, ánh đèn vàng) |
| Illustration | (none) |
| Fade To Black Before | ✅ true |
| BGM | `bgm_emotional_hope.mp3` |

**Text Lines (3 dòng):**
```
[0]: "Tối hôm đó..."
[1]: "Mẹ Đức gọi điện cho cô giáo chủ nhiệm."
[2]: "Nhà trường phối hợp với công an xử lý tụi bắt nạt."
```

---

### Segment 1: Sáng hôm sau - Cô giáo

| Field | Value |
|-------|-------|
| Background Image | `bg_teacher_office.png` (Phòng cô giáo, ánh sáng ban ngày) |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (3 dòng):**
```
[0]: "Sáng hôm sau..."
[1]: "Cô giáo gọi Đức lên phòng, hỏi thăm và an ủi."
[2]: "\"Em đừng lo nữa nhé. Cô sẽ xử lý chuyện này.\""
```

---

### Segment 2: Một tuần sau - Bạn bè

| Field | Value |
|-------|-------|
| Background Image | `bg_school_gate_afternoon.png` (Cổng trường chiều) |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Trong một tuần..."
[1]: "Hai bạn trong lớp tình nguyện đi cùng Đức về nhà."
[2]: "\"Ê Đức, đi về cùng tụi tao nhé.\""
[3]: "\"Có tụi tao đây, tụi nó không dám làm gì đâu.\""
```

---

### Segment 3: Một tháng sau - Mở lòng

| Field | Value |
|-------|-------|
| Background Image | `bg_school_yard_sunny.png` (Sân trường nắng đẹp) |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Một tháng sau..."
[1]: "Đức dần mở lòng với bạn bè."
[2]: "Những người bạn mới giúp Đức vượt qua nỗi sợ hãi."
[3]: "Tụi bắt nạt không dám xuất hiện nữa."
```

---

### Segment 4: Bài học - Ảnh chính

| Field | Value |
|-------|-------|
| Background Image | `bg_school_yard_sunny.png` |
| Illustration | `illus_duc_friends_happy.png` (Đức cười cùng bạn bè) |
| Illustration Position | Center |
| Illustration Scale | 1.2 |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Đức học cách đứng lên cho bản thân..."
[1]: "Nhưng cũng biết khi nào cần nhờ người lớn giúp đỡ."
[2]: "Cuộc sống không phải lúc nào cũng dễ dàng."
[3]: "Nhưng khi bạn có dũng khí đứng lên, bạn sẽ không còn cô đơn nữa."
```

---

### Segment 5: THE END

| Field | Value |
|-------|-------|
| Background Image | `bg_black.png` hoặc (none - màn đen) |
| Background Tint | Black |
| Illustration | (none) |
| Fade To Black Before | ✅ true |
| BGM | (giữ nguyên hoặc fade out) |

**Text Lines (3 dòng):**
```
[0]: "ENDING 1: SẼ CÓ NHỮNG CON CÁ PHẢI GIẢ CHÓ"
[1]: "\"Đứng lên chống lại bạo lực, và tìm kiếm sự giúp đỡ từ gia đình và nhà trường.\""
[2]: "[THE END]"
```

---

## ✅ ENDING 2: TRUE_TELLPARENTS (6 Segments)

**Điều kiện trigger**: `confessed_to_mom` = true (sau khi bị đánh)
**Action ID**: `trigger_ending2_storytelling`
**Ending Type**: `True_TellParents`
**Next Scene**: `MainMenu`
**Tone**: Ấm áp, cảm động

---

### Segment 0: Khóc trong vòng tay mẹ

| Field | Value |
|-------|-------|
| Background Image | `bg_livingroom_night.png` |
| Illustration | `illus_duc_mom_hug.png` (Mẹ ôm Đức) |
| Illustration Position | Center |
| Fade To Black Before | ✅ true |
| BGM | `bgm_emotional_sad.mp3` |

**Text Lines (4 dòng):**
```
[0]: "Đức khóc trong vòng tay mẹ."
[1]: "Lần đầu tiên sau nhiều tuần, Đức cảm thấy nhẹ lòng."
[2]: "Mẹ vuốt tóc con, an ủi:"
[3]: "\"Con đừng sợ nữa. Mẹ ở đây mà.\""
```

---

### Segment 1: Mẹ hành động

| Field | Value |
|-------|-------|
| Background Image | `bg_livingroom_night.png` |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Tối hôm đó..."
[1]: "Mẹ Đức gọi điện cho cô giáo và báo cảnh sát."
[2]: "Nhà trường và công an phối hợp điều tra."
[3]: "Tụi bắt nạt bị đưa vào trại giáo dưỡng."
```

---

### Segment 2: Chuyển trường

| Field | Value |
|-------|-------|
| Background Image | `bg_new_school_gate.png` (Trường mới, sáng sủa) |
| Illustration | (none) |
| Fade To Black Before | ❌ false |
| BGM | `bgm_emotional_hope.mp3` (chuyển nhạc) |

**Text Lines (4 dòng):**
```
[0]: "Một tuần sau..."
[1]: "Mẹ Đức quyết định cho con chuyển trường."
[2]: "\"Con ơi, mẹ đã tìm được trường mới rồi. Gần nhà hơn, an toàn hơn.\""
[3]: "\"Mẹ sẽ đưa đón con mỗi ngày.\""
```

---

### Segment 3: Ngày đầu tiên

| Field | Value |
|-------|-------|
| Background Image | `bg_new_school_yard.png` |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (3 dòng):**
```
[0]: "Sáng ngày đầu tiên ở trường mới..."
[1]: "Đức vẫn còn e dè, nhưng không còn sợ hãi như trước."
[2]: "Vì Đức biết: Mẹ luôn ở đây, sẵn sàng bảo vệ con."
```

---

### Segment 4: Một năm sau - Ảnh chính

| Field | Value |
|-------|-------|
| Background Image | `bg_street_sunset.png` (Đường phố hoàng hôn) |
| Illustration | `illus_duc_mom_holding_hands.png` (Đức và mẹ nắm tay) |
| Illustration Position | Center |
| Illustration Scale | 1.2 |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Một năm sau..."
[1]: "Đức đã có những người bạn mới."
[2]: "Đức học cách chia sẻ, học cách tin tưởng."
[3]: "Gia đình là nơi bạn luôn có thể quay về."
```

---

### Segment 5: THE END

| Field | Value |
|-------|-------|
| Background Image | (none - màn đen) |
| Background Tint | Black |
| Illustration | (none) |
| Fade To Black Before | ✅ true |

**Text Lines (3 dòng):**
```
[0]: "ENDING 2: CHIA SẺ VỚI GIA ĐÌNH"
[1]: "\"Chia sẻ với gia đình là điều quan trọng nhất khi bị bắt nạt.\""
[2]: "[THE END]"
```

---

## ❌ ENDING 3: BAD_DARKLIFE (8 Segments)

**Điều kiện trigger**: `hid_from_mom` = true
**Action ID**: `trigger_ending3_storytelling`
**Ending Type**: `None` (hoặc tạo thêm `Bad_DarkLife`)
**Next Scene**: `MainMenu`
**Tone**: U ám, bi kịch, cảnh báo

---

### Segment 0: Nhìn gương

| Field | Value |
|-------|-------|
| Background Image | `bg_bedroom_dark.png` (Phòng ngủ tối, chỉ có ánh trăng) |
| Illustration | (none) |
| Fade To Black Before | ✅ true |
| BGM | `bgm_dark_sad.mp3` |

**Text Lines (4 dòng):**
```
[0]: "Đức lên phòng..."
[1]: "Nhìn vào gương, thấy khuôn mặt đầy vết thương."
[2]: "\"Tại sao mình lại yếu đuối đến thế...\""
[3]: "Đức khóc thầm, không dám để mẹ nghe thấy."
```

---

### Segment 1: Vẫn bị bắt nạt

| Field | Value |
|-------|-------|
| Background Image | `bg_black.png` hoặc `bg_alley_dark.png` |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (3 dòng):**
```
[0]: "Một tuần sau..."
[1]: "Tụi bắt nạt vẫn tiếp tục."
[2]: "Chúng biết Đức không dám nói với ai. Chúng càng ngày càng táo tợn hơn."
```

---

### Segment 2: Nghỉ học

| Field | Value |
|-------|-------|
| Background Image | `bg_bedroom_day_curtains_closed.png` (Phòng ngủ ban ngày, rèm đóng) |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Một tháng sau..."
[1]: "Đức bắt đầu nghỉ học."
[2]: "\"Mẹ ơi, con bị ốm...\""
[3]: "Mẹ lo lắng, nhưng không biết lý do thật sự."
```

---

### Segment 3: Bệnh viện

| Field | Value |
|-------|-------|
| Background Image | `bg_hospital_waiting.png` (Phòng chờ bệnh viện) |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Ba tháng sau..."
[1]: "Đức hoàn toàn không đi học nữa."
[2]: "Mẹ đưa con đi bác sĩ, nhưng không tìm ra bệnh gì."
[3]: "\"Con trai mẹ bị sao thế này?\"   "
```

---

### Segment 4: Suy nghĩ tiêu cực

| Field | Value |
|-------|-------|
| Background Image | `bg_black.png` |
| Illustration | `illus_duc_alone_darkness.png` (Đức ngồi co ro trong bóng tối) |
| Illustration Position | Center |
| Fade To Black Before | ✅ true |
| BGM | `bgm_dark_despair.mp3` |

**Text Lines (4 dòng):**
```
[0]: "Sáu tháng sau..."
[1]: "Đức bắt đầu có những suy nghĩ tiêu cực."
[2]: "\"Tại sao mình phải sống?\""
[3]: "\"Tại sao không ai hiểu mình?\""
```

---

### Segment 5: Mẹ già đi

| Field | Value |
|-------|-------|
| Background Image | `bg_livingroom_evening.png` (Phòng khách buổi tối, u ám) |
| Illustration | (none) |
| Fade To Black Before | ❌ false |

**Text Lines (4 dòng):**
```
[0]: "Một năm sau..."
[1]: "Đức vẫn chưa quay lại trường."
[2]: "Mẹ Đức đã già đi rất nhiều, tóc bạc trắng."
[3]: "Mẹ tự trách mình: \"Tại sao mẹ không nhận ra sớm hơn...\""
```

---

### Segment 6: Nhiều năm sau - Ảnh chính

| Field | Value |
|-------|-------|
| Background Image | `bg_park_dusk_empty.png` (Công viên hoàng hôn, vắng vẻ) |
| Illustration | `illus_adult_silhouette_alone.png` (Silhouette người lớn ngồi một mình) |
| Illustration Position | Center |
| Illustration Scale | 1.0 |
| Fade To Black Before | ❌ false |

**Text Lines (5 dòng):**
```
[0]: "Nhiều năm sau..."
[1]: "Đức trở thành một người trưởng thành..."
[2]: "Nhưng vẫn mang trong mình những vết thương tâm lý."
[3]: "Sợ đám đông. Sợ giao tiếp. Sợ tin tưởng người khác."
[4]: "Cuộc đời Đức như một bóng tối... không bao giờ tìm thấy ánh sáng."
```

---

### Segment 7: THE END

| Field | Value |
|-------|-------|
| Background Image | (none - màn đen) |
| Background Tint | Black |
| Illustration | (none) |
| Fade To Black Before | ✅ true |

**Text Lines (4 dòng):**
```
[0]: "ENDING 3: CUỘC ĐỜI ĐEN TỐI"
[1]: "Im lặng chịu đựng không phải là sức mạnh."
[2]: "\"Đừng bao giờ giấu giếm khi bị bắt nạt. Hãy chia sẻ với người thân.\""
[3]: "[THE END]"
```

---

## 🎨 DANH SÁCH ASSETS CẦN TẠO

### Backgrounds (12 ảnh)

| File Name | Mô tả | Dùng cho |
|-----------|-------|----------|
| `bg_livingroom_night.png` | Phòng khách ban đêm, ánh đèn vàng ấm | E1-S0, E2-S0, E2-S1 |
| `bg_teacher_office.png` | Phòng cô giáo, ánh sáng ban ngày | E1-S1 |
| `bg_school_gate_afternoon.png` | Cổng trường buổi chiều | E1-S2 |
| `bg_school_yard_sunny.png` | Sân trường nắng đẹp | E1-S3, E1-S4 |
| `bg_new_school_gate.png` | Cổng trường mới, sáng sủa | E2-S2 |
| `bg_new_school_yard.png` | Sân trường mới | E2-S3 |
| `bg_street_sunset.png` | Đường phố hoàng hôn | E2-S4 |
| `bg_bedroom_dark.png` | Phòng ngủ tối, ánh trăng | E3-S0 |
| `bg_bedroom_day_curtains_closed.png` | Phòng ngủ ban ngày, rèm đóng | E3-S2 |
| `bg_hospital_waiting.png` | Phòng chờ bệnh viện | E3-S3 |
| `bg_livingroom_evening.png` | Phòng khách buổi tối, u ám | E3-S5 |
| `bg_park_dusk_empty.png` | Công viên hoàng hôn, vắng vẻ | E3-S6 |

---

## 🖼️ AI IMAGE GENERATION PROMPTS

> Prompts chi tiết để gen ảnh background và illustration cho từng segment.
> Style chung: **2D anime/visual novel style, Vietnamese setting, emotional storytelling**

### 📍 ENDING 1: GOOD_STANDUP - Background Prompts

**E1-S0: bg_livingroom_night.png**
```
Vietnamese middle-class living room at night, warm yellow lamp light, 
wooden furniture, family photos on wall, sofa and coffee table, 
cozy atmosphere, soft shadows, 2D anime visual novel background style, 
no characters, emotional warm tone, 1920x1080
```

**E1-S1: bg_teacher_office.png**
```
Vietnamese high school teacher's office, daytime natural lighting from window,
wooden desk with papers and books, filing cabinets, potted plant,
certificates on wall, professional but caring atmosphere,
2D anime visual novel background style, no characters, 1920x1080
```

**E1-S2: bg_school_gate_afternoon.png**
```
Vietnamese high school front gate in afternoon, golden hour sunlight,
iron gate with school name sign, trees along the street,
students walking in background (blurred), hopeful atmosphere,
2D anime visual novel background style, warm colors, 1920x1080
```

**E1-S3 & E1-S4: bg_school_yard_sunny.png**
```
Vietnamese high school courtyard on sunny day, bright blue sky,
basketball court, benches under trees, school building in background,
cheerful and peaceful atmosphere, lens flare effect,
2D anime visual novel background style, vibrant colors, 1920x1080
```

---

### 📍 ENDING 2: TRUE_TELLPARENTS - Background Prompts

**E2-S0 & E2-S1: (dùng chung bg_livingroom_night.png)**

**E2-S2: bg_new_school_gate.png**
```
New Vietnamese high school entrance, modern design, morning sunlight,
clean white walls, colorful flower beds, welcoming atmosphere,
fresh start feeling, hope and new beginning,
2D anime visual novel background style, bright pastel colors, 1920x1080
```

**E2-S3: bg_new_school_yard.png**
```
New school courtyard, modern Vietnamese school, morning light,
green grass, modern benches, friendly atmosphere,
students chatting in distance (blurred), safe and welcoming feeling,
2D anime visual novel background style, soft warm colors, 1920x1080
```

**E2-S4: bg_street_sunset.png**
```
Vietnamese residential street at sunset, golden orange sky,
row of houses with warm lights, motorbikes parked,
peaceful evening atmosphere, nostalgic and heartwarming,
2D anime visual novel background style, warm sunset palette, 1920x1080
```

---

### 📍 ENDING 3: BAD_DARKLIFE - Background Prompts

**E3-S0: bg_bedroom_dark.png**
```
Vietnamese teenager bedroom at night, only moonlight through window,
messy bed, desk with scattered books, dark shadows,
lonely and depressing atmosphere, blue-gray color palette,
2D anime visual novel background style, melancholic mood, 1920x1080
```

**E3-S1: bg_alley_dark.png (alternative to bg_black)**
```
Dark narrow Vietnamese alley, dim street light, wet ground,
graffiti on walls, threatening atmosphere, danger feeling,
no characters, ominous shadows, urban decay,
2D anime visual novel background style, dark desaturated colors, 1920x1080
```

**E3-S2: bg_bedroom_day_curtains_closed.png**
```
Vietnamese teenager bedroom during day but curtains closed,
dim filtered light, unmade bed, clothes on floor,
depressing isolation feeling, dust particles in light beam,
2D anime visual novel background style, muted gray-blue tones, 1920x1080
```

**E3-S3: bg_hospital_waiting.png**
```
Vietnamese hospital waiting room, harsh fluorescent lights,
plastic chairs in rows, health posters on wall, sterile atmosphere,
anxious and uncomfortable feeling, clinical coldness,
2D anime visual novel background style, cold blue-white colors, 1920x1080
```

**E3-S5: bg_livingroom_evening.png**
```
Vietnamese living room in evening, dim lighting, gloomy atmosphere,
same furniture as night version but darker mood, shadows prominent,
sense of sadness and regret, time has passed feeling,
2D anime visual novel background style, desaturated warm colors, 1920x1080
```

**E3-S6: bg_park_dusk_empty.png**
```
Empty Vietnamese park at dusk, single bench under tree,
orange-purple twilight sky, fallen leaves, no people,
profound loneliness and isolation, melancholic beauty,
2D anime visual novel background style, muted sunset colors, 1920x1080
```

---

### 🎭 ILLUSTRATION PROMPTS

**E1-S4: illus_duc_friends_happy.png**
```
Vietnamese teenage boy (Duc, 15yo) smiling genuinely with 3-4 friends,
school uniforms (white shirt, dark pants), arms around shoulders,
school yard background, bright sunny day, genuine happiness,
2D anime illustration style, warm cheerful colors, full body shot, 1920x1080
```

**E2-S0: illus_duc_mom_hug.png**
```
Vietnamese mother (40s) hugging her crying teenage son (Duc, 15yo),
living room setting, warm lamp light, emotional moment,
mother's protective embrace, son's face buried in shoulder,
tears visible, 2D anime illustration style, warm emotional colors,
medium shot focusing on embrace, 1920x1080
```

**E2-S4: illus_duc_mom_holding_hands.png**
```
Vietnamese mother and teenage son (Duc) walking together holding hands,
sunset street background, seen from behind or side angle,
warm golden light, peaceful bonding moment, hope for future,
2D anime illustration style, warm sunset palette, full body shot, 1920x1080
```

**E3-S4: illus_duc_alone_darkness.png**
```
Vietnamese teenage boy (Duc, 15yo) sitting alone curled up,
hugging knees, head down, in complete darkness,
single dim light source from above, deep depression visual,
isolated and hopeless feeling, 2D anime illustration style,
dark blue-black palette with minimal lighting, 1920x1080
```

**E3-S6: illus_adult_silhouette_alone.png**
```
Adult man silhouette sitting alone on park bench,
dusk sky background, back view, hunched posture,
profound loneliness, years of trauma visible in body language,
2D anime illustration style, dark silhouette against muted sunset,
melancholic and haunting, 1920x1080
```

---

### 🎨 STYLE GUIDELINES CHUNG

**Base Prompt Suffix (thêm vào cuối mỗi prompt):**
```
, high quality, detailed background, visual novel game asset,
16:9 aspect ratio, no text, no watermark, clean composition
```

**Negative Prompt (dùng cho AI gen):**
```
blurry, low quality, text, watermark, signature, 3D render,
photorealistic, western cartoon style, chibi, deformed,
bad anatomy, extra limbs, cropped, out of frame
```

**Color Palette theo Ending:**
- **Ending 1 (Good)**: Warm yellows, soft oranges, bright blues, hopeful greens
- **Ending 2 (True)**: Warm amber, soft pinks, gentle purples, comforting browns
- **Ending 3 (Bad)**: Cold blues, dark grays, muted purples, desaturated colors

**Resolution:** 1920x1080 (16:9) hoặc 1280x720 nếu cần optimize

---

### Illustrations (5 ảnh)

| File Name | Mô tả | Dùng cho |
|-----------|-------|----------|
| `illus_duc_friends_happy.png` | Đức cười cùng 3-4 bạn bè | E1-S4 |
| `illus_duc_mom_hug.png` | Mẹ ôm Đức đang khóc | E2-S0 |
| `illus_duc_mom_holding_hands.png` | Đức và mẹ nắm tay đi trên đường | E2-S4 |
| `illus_duc_alone_darkness.png` | Đức ngồi co ro trong bóng tối | E3-S4 |
| `illus_adult_silhouette_alone.png` | Silhouette người lớn ngồi một mình | E3-S6 |

### Audio (4 files)

| File Name | Mô tả | Dùng cho |
|-----------|-------|----------|
| `bgm_emotional_hope.mp3` | Nhạc hy vọng, tích cực | E1, E2 (phần sau) |
| `bgm_emotional_sad.mp3` | Nhạc buồn, cảm động | E2 (phần đầu) |
| `bgm_dark_sad.mp3` | Nhạc u ám, buồn bã | E3 (phần đầu) |
| `bgm_dark_despair.mp3` | Nhạc tuyệt vọng, nặng nề | E3 (phần sau) |

---

## ⚙️ CÀI ĐẶT TRONG INSPECTOR

### Cài đặt chung cho mỗi Sequence:

```
Sequence Info:
├── Sequence Name: "Ending1_GoodStandUp" (hoặc 2, 3)
├── Description: "Good ending - Đứng lên chống lại"
├── Ending Type: Good_StandUp (hoặc True_TellParents, None)

After Sequence:
├── Next Scene Name: "MainMenu"
├── Delay Before Next Scene: 2

Skip Settings:
├── Allow Skip: ✅
├── Skip Key: Escape
├── Skip Hint Text: "Nhấn ESC để bỏ qua"
```

### Cài đặt cho mỗi Segment:

```
Visual:
├── Background Image: [Sprite]
├── Background Tint: White (hoặc Black cho màn đen)
├── Illustration Image: [Sprite hoặc None]
├── Illustration Position: Center
├── Illustration Scale: 1.0 - 1.2

Text Content:
├── Text Lines: [Array of strings]
├── Typewriter Speed: 0.03
├── Auto Advance Delay: 0 (chờ input)

Transition:
├── Fade To Black Before: true/false
├── Delay Before: 0
├── Background Transition: Fade

Audio:
├── BGM: [AudioClip hoặc None]
├── SFX: [AudioClip hoặc None]
├── BGM Volume: 0.5
├── SFX Volume: 1.0
```

---

## 🔗 CÁCH TRIGGER ENDINGS

### Từ Dialogue (JSON):

```json
{
  "id": 30,
  "speaker": "Đức",
  "lines": ["Con xin lỗi mẹ..."],
  "next": -1,
  "actionId": "trigger_ending1_storytelling"
}
```

### Từ Code:

```csharp
// Trong VisualNovelManager.OnDialogueAction()
case "trigger_ending1_storytelling":
    TriggerEnding1();
    break;

// Method TriggerEnding1()
private void TriggerEnding1()
{
    StorytellingSequenceData ending1 = Resources.Load<StorytellingSequenceData>("Storytelling/Ending1_Good_StandUp");
    if (ending1 != null)
    {
        EndVNMode();
        StorytellingManager.Instance.PlaySequence(ending1);
    }
}
```

---

## ✅ CHECKLIST IMPLEMENTATION

- [ ] Tạo 12 background images
- [ ] Tạo 5 illustration images
- [ ] Tạo/tìm 4 BGM files
- [ ] Setup Ending1_GoodStandUp_Sequence.asset (6 segments)
- [ ] Setup Ending2_TrueTellParents_Sequence.asset (6 segments)
- [ ] Setup Ending3_BadDarkLife_Sequence.asset (8 segments)
- [ ] Copy assets vào `Assets/Resources/Storytelling/` với tên đúng
- [ ] Test từng ending bằng Debug Flag Menu (F1)
- [ ] Test full flow từ Scene 27 → Scene 28 → Ending

---

_Cập nhật: December 2024_
_Thêm AI Image Prompts: December 23, 2024_
