import RealityKit
import SwiftUI

// NEW!
import RealityKitContent

class GameManager: ObservableObject {
    static let shared = GameManager()
    
    @Published var selectedSong: Song
    
    var songs: [Song] = []
    
    // 4. 音符加载
    let root = Entity()
    // NEW!
    private var boxTemplate: Entity?
    
    private init() {
        songs = [
            Song(name: "2077"),
            Song(name: "Missing U"),
            Song(name: "SaintPerros"),
        ]
        selectedSong = songs[0]
        
        // 4. 音符加载
        // NEW!
        boxTemplate = try? Entity.load(named: "Box", in: realityKitContentBundle)
        if boxTemplate == nil {
            print("Failed to load boxTemplate.")
        }
    }
    
    func start() {
        selectedSong.player.play()
        // NEW!
        // 4. 音符加载
        root.addChild(spawnBox())
    }
    
    func stop() {
        selectedSong.player.stop()
    }
    
    // NEW!
    // 4. 音符加载
    func spawnBox() -> Entity {
        guard let boxTemplate = boxTemplate else {
            fatalError("Box template is nil")
        }
        let box = boxTemplate.clone(recursive: true)
        box.position = [0, 1, -1.5]
        root.addChild(box)
        return box
    }
}
