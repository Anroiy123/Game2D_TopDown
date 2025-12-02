# 🎮 Game 2D Top-Down Classroom

A 2D top-down classroom simulation game built with **Unity 6**.

![Unity](https://img.shields.io/badge/Unity-6-000000?style=flat&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp)
![License](https://img.shields.io/badge/License-MIT-blue)

## 📖 Mô tả

Game mô phỏng lớp học 2D theo góc nhìn top-down. Người chơi có thể di chuyển trong lớp học, ngồi vào ghế và tương tác với các NPC thông qua hệ thống đối thoại.

## ✨ Tính năng

- 🚶 **Di chuyển 4 hướng** - WASD hoặc phím mũi tên
- 🪑 **Ngồi ghế** - Nhấn E khi đứng gần ghế
- 💬 **Hệ thống đối thoại** - Tương tác với NPC, hiệu ứng typewriter
- 🔄 **NPC quay mặt** - NPC tự động quay về phía người chơi khi nói chuyện
- 🎨 **Animation Blend Tree** - Animation mượt mà cho cả Player và NPC

## 🎮 Điều khiển

| Phím              | Hành động                                             |
| ----------------- | ----------------------------------------------------- |
| **WASD / Arrows** | Di chuyển                                             |
| **E**             | Tương tác (ngồi/đứng, nói chuyện, tiếp tục đối thoại) |
| **ESC**           | Thoát đối thoại                                       |

## 🏗️ Cấu trúc dự án

```
Assets/
├── Scripts/
│   ├── PlayerMovement.cs    # Điều khiển player, ngồi ghế
│   ├── NPCInteraction.cs    # Tương tác NPC, hiển thị tên
│   └── DialogueSystem.cs    # Hệ thống đối thoại UI
├── Animation/               # Animation clips & controllers
├── Prefabs/                 # Prefabs (Chair, NPC...)
├── Scenes/                  # Game scenes
├── Sprites/                 # Sprite assets
└── TileMap/                 # Tilemap assets
```

## 🔧 Yêu cầu

- **Unity 6** (6000.0.0 trở lên)
- **2D Sprite** package
- **Cinemachine** (cho camera follow)

## 🚀 Cài đặt

1. Clone repository:

   ```bash
   git clone https://github.com/Anroiy123/Game2D_TopDown.git
   ```

2. Mở project bằng Unity Hub

3. Mở scene `Assets/Scenes/SampleScene.unity`

4. Nhấn **Play** để chạy game

## 📝 Hệ thống Scripts

### PlayerMovement.cs

- Quản lý di chuyển player
- Xử lý logic ngồi ghế với `sitOffset`
- Khóa input khi đang nói chuyện (`isTalking`)
- Ưu tiên NPC hơn ghế khi cả hai gần nhau

### NPCInteraction.cs

- Phát hiện player trong `interactionRange`
- Hiển thị tên NPC (World Space Canvas)
- Quay NPC về phía player khi đối thoại
- Trigger `DialogueSystem` khi nhấn E

### DialogueSystem.cs

- Hiển thị dialogue box (Screen Space UI)
- Hiệu ứng typewriter cho text
- Xử lý nhiều dòng đối thoại
- Callback khi kết thúc đối thoại

## 🎨 Thêm NPC mới

1. Tạo GameObject với Sprite, Animator, Box Collider 2D
2. Thêm tag `NPC`
3. Add component `NPCInteraction`
4. Cấu hình:
   - `NPC Name`: Tên hiển thị
   - `Dialogue Lines`: Các câu đối thoại
   - `Interaction Range`: Khoảng cách tương tác
5. Tạo `NameCanvas` (World Space) làm con của NPC

## 📌 Lưu ý quan trọng

- **Hệ tọa độ**: Y dương (+) = xuống/phía trước, Y âm (-) = lên/phía sau
- **Sit Offset**: Điều chỉnh Y trong Inspector để player ngồi đúng vị trí ghế
- **Animator Parameters**: Speed, Horizontal, Vertical, IsSitting

## 🤝 Đóng góp

Mọi đóng góp đều được hoan nghênh! Vui lòng tạo Issue hoặc Pull Request.

## 📄 License

MIT License - Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

---

**Made with ❤️ using Unity**
