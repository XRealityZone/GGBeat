import AVFAudio

struct Song {
    let name: String
    let player: AVAudioPlayer
    // NEW!
    // 5. 节奏同步
    let stage: Stage

    init(name: String) {
        self.name = name
        self.player = try! AVAudioPlayer(contentsOf: Bundle.main.url(forResource: name, withExtension: "mp3")!)

        // NEW!
        // 5. 节奏同步
        let url = Bundle.main.url(forResource: self.name, withExtension: "json")!
        let data = try! Data(contentsOf: url)
        self.stage = try! JSONDecoder().decode(Stage.self, from: data)
    }
}
