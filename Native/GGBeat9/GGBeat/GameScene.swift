// NEW!
// 8. 主题变换
enum GameScene: String, CaseIterable, Identifiable {
    case cyberpunk = "Cyberpunk"
    case painting = "VanGogh"
    case cartoon = "Cartoon"

    var id: String {
        self.rawValue
    }
}
