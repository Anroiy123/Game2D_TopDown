# 🎬 STORYTELLING SYSTEM - HƯỚNG DẪN SỬ DỤNG

## 📋 MỤC LỤC
1. [Tổng quan](#tổng-quan)
2. [Tạo Storytelling Sequence](#tạo-storytelling-sequence)
3. [Cấu hình Segments](#cấu-hình-segments)
4. [Trigger Storytelling](#trigger-storytelling)
5. [Ví dụ cụ thể](#ví-dụ-cụ-thể)
6. [API Reference](#api-reference)

---

## 1. TỔNG QUAN

### ✨ Tính năng:
- ✅ Hiển thị **nhiều background** khác nhau (fade transition)
- ✅ Hiển thị **nhiều đoạn text** tuần tự (typewriter effect)
- ✅ Hỗ trợ **ảnh minh họa** (optional) cho từng đoạn
- ✅ Tích hợp **ScreenFader** (fade to black)
- ✅ Tích hợp **StoryManager** (set flags, trigger endings)
- ✅ Tự động chuyển về **Main Menu** hoặc **Credits**
- ✅ Có thể **skip** (nhấn ESC)
- ✅ Hỗ trợ **audio** (BGM, SFX)

### 🏗️ Kiến trúc:
```
StorytellingManager (Singleton)
├── StorytellingSequenceData (ScriptableObject)
│   └── StorySegment[] (array of segments)
├── StorytellingTrigger (Component)
└── StorytellingQuickTest (Component)
```

---

## 2. TẠO STORYTELLING SEQUENCE

### Cách 1: Dùng Editor Tool (Khuyến nghị)

1. **Mở Editor Tool:**
   - `Tools → Storytelling → Create Sequence`

2. **Điền thông tin:**
   - **Sequence Name:** `Ending1_GoodStandUp`
   - **Description:** `Ending tốt - Đức tự đứng lên chống lại bắt nạt`
   - **Ending Type:** `Good_StandUp`
   - **Number of Segments:** `5` (số đoạn story)
   - **Next Scene:** `MainMenu` (scene load sau khi xong)
   - **Save Path:** `Assets/Data/Storytelling/`

3. **Click "Create Sequence"**
   - Asset sẽ được tạo tại `Assets/Data/Storytelling/Ending1_GoodStandUp_Sequence.asset`

### Cách 2: Tạo thủ công

1. **Right-click trong Project:**
   - `Create → Storytelling → Sequence Data`

2. **Rename:** `Ending1_GoodStandUp_Sequence`

3. **Cấu hình trong Inspector** (xem phần 3)

---

## 3. CẤU HÌNH SEGMENTS

Mỗi **StorySegment** đại diện cho một đoạn trong storytelling.

### 📦 Cấu trúc Segment:

```
StorySegment
├── Visual
│   ├── Background Image (Sprite)
│   ├── Background Tint (Color)
│   ├── Illustration Image (Sprite, optional)
│   ├── Illustration Position (Center/Top/Bottom/Left/Right/Custom)
│   └── Illustration Scale (0.1 - 3.0)
├── Text Content
│   ├── Text (TextArea)
│   ├── Typewriter Speed (0 = instant, 0.03 = normal)
│   └── Display Duration (giây)
├── Transition
│   ├── Fade To Black Before (bool)
│   ├── Delay Before (giây)
│   └── Background Transition (None/Fade/CrossFade)
└── Audio
    ├── BGM (AudioClip)
    ├── SFX (AudioClip)
    ├── BGM Volume (0-1)
    └── SFX Volume (0-1)
```

### 📝 Ví dụ cấu hình:

**Segment 1: Intro**
```
Background Image: [SchoolHallway_Sprite]
Background Tint: White
Text: "Sau khi đối mặt với bọn bắt nạt, Đức cảm thấy nhẹ nhõm hơn..."
Typewriter Speed: 0.03
Display Duration: 3
Fade To Black Before: ✅ (checked)
BGM: [HopefulMusic_AudioClip]
```

**Segment 2: Classroom**
```
Background Image: [Classroom_Sprite]
Illustration Image: [DucSmiling_Sprite]
Illustration Position: Center
Text: "Đức quay lại lớp với nụ cười tự tin."
Typewriter Speed: 0.03
Display Duration: 3
Background Transition: Fade
```

---

## 4. TRIGGER STORYTELLING

### Cách 1: Trigger từ Scene (StorytellingTrigger)

1. **Tạo GameObject:**
   - Hierarchy → Right-click → `Create Empty`
   - Rename: `EndingTrigger`

2. **Add Component:**
   - `Add Component → Storytelling Trigger`

3. **Cấu hình:**
   - **Sequence Data:** Kéo `Ending1_GoodStandUp_Sequence` vào
   - **Trigger Mode:** `OnSceneStart` (hoặc `OnTriggerEnter`, `OnInteract`)
   - **Required Flags:** `stood_up_to_bullies` (nếu cần điều kiện)

4. **Nếu dùng OnTriggerEnter/OnInteract:**
   - Add `Box Collider 2D`
   - ✅ Check `Is Trigger`

### Cách 2: Trigger từ Code

```csharp
// Trong DialogueSystem hoặc script khác
StorytellingSequenceData endingSequence = /* load từ Resources hoặc serialize field */;
StorytellingManager.Instance.PlaySequence(endingSequence, OnEndingComplete);

void OnEndingComplete()
{
    Debug.Log("Ending finished!");
}
```

### Cách 3: Test nhanh (StorytellingQuickTest)

1. **Add Component vào GameObject bất kỳ:**
   - `Add Component → Storytelling Quick Test`

2. **Cấu hình:**
   - **Sequence To Test:** Kéo sequence vào
   - **Test Key:** `T`
   - **Show Instructions:** ✅

3. **Play Mode:**
   - Nhấn `T` để test sequence

---

## 5. VÍ DỤ CỤ THỂ

### Ví dụ 1: Ending 1 - Good StandUp

**File:** `Assets/Data/Storytelling/Ending1_GoodStandUp_Sequence.asset`

**Segments:**
1. **Đoạn 1:** Fade from black → School hallway background → "Sau khi đối mặt..."
2. **Đoạn 2:** Classroom background + Đức smiling illustration → "Đức quay lại lớp..."
3. **Đoạn 3:** School gate background → "Những ngày sau đó..."
4. **Đoạn 4:** Home background + Family illustration → "Gia đình Đức..."
5. **Đoạn 5:** Black background → "THE END - Ending 1: Tự Đứng Lên"

**Settings:**
- Ending Type: `Good_StandUp`
- Next Scene: `MainMenu`
- Allow Skip: ✅
- Skip Key: `Escape`

---

## 6. API REFERENCE

### StorytellingManager

```csharp
// Singleton instance
StorytellingManager.Instance

// Play sequence
void PlaySequence(StorytellingSequenceData sequenceData, Action onComplete = null)

// Stop current sequence
void StopSequence()

// Check if playing
bool IsPlaying { get; }
```

### StorytellingSequenceData

```csharp
// Apply effects
void ApplyOnStartEffects()
void ApplyOnCompleteEffects()
```

### StorytellingTrigger

```csharp
// Trigger manually
void TriggerSequence()
```

---

## 📌 LƯU Ý

1. **Sprite Assets:**
   - Đặt background sprites trong `Assets/Art/Backgrounds/`
   - Đặt illustration sprites trong `Assets/Art/Characters/`

2. **Audio Assets:**
   - Đặt BGM trong `Assets/Audio/Music/`
   - Đặt SFX trong `Assets/Audio/SFX/`

3. **Performance:**
   - Sử dụng `Typewriter Speed = 0` để hiện text ngay (nhanh hơn)
   - Sử dụng `Background Transition = None` để chuyển background ngay (nhanh hơn)

4. **Testing:**
   - Dùng `StorytellingQuickTest` để test nhanh trong Play Mode
   - Dùng `Skip` (ESC) để bỏ qua khi test

---

## 🎯 WORKFLOW KHUYẾN NGHỊ

1. **Tạo Sequence:** `Tools → Storytelling → Create Sequence`
2. **Cấu hình Segments:** Điền text, kéo sprites, audio vào Inspector
3. **Test:** Add `StorytellingQuickTest`, nhấn `T` trong Play Mode
4. **Deploy:** Add `StorytellingTrigger` vào scene với điều kiện phù hợp
5. **Polish:** Điều chỉnh timing, transition, audio

---

**Tác giả:** Storytelling System v1.0  
**Ngày tạo:** 2025-12-22

