import AVFAudio

struct Song {
    let name: String
    let player: AVAudioPlayer
    // 5. 节奏同步
    let stage: Stage
    // NEW!
    // 8. 主题变换
    let defaultGameScene: GameScene
    
    // NEW!
    // 8. 主题变换 - defaultGameScene
    init(name: String, defaultGameScene: GameScene) {
        self.name = name
        self.player = try! AVAudioPlayer(contentsOf: Bundle.main.url(forResource: name, withExtension: "mp3")!)
        
        // 5. 节奏同步
        let url = Bundle.main.url(forResource: self.name, withExtension: "json")!
        let data = try! Data(contentsOf: url)
        self.stage = try! JSONDecoder().decode(Stage.self, from: data)
        
        // NEW!
        // 8. 主题变换
        self.defaultGameScene = defaultGameScene
    }
}
