import AVFAudio

struct Song {
    let name: String
    let player: AVAudioPlayer

    init(name: String) {
        self.name = name
        self.player = try! AVAudioPlayer(contentsOf: Bundle.main.url(forResource: name, withExtension: "mp3")!)
    }
}
