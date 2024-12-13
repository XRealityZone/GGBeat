//
// This custom View is referenced by SwiftUISampleInjectedScene
// to provide the body of a WindowGroup. It's part of the Unity-VisionOS
// target because it lives inside a "SwiftAppSupport" directory (and Unity
// will move it to that target).
//

import Foundation
import SwiftUI
import UnityFramework

enum Song: String, CaseIterable, Identifiable {
    case twoThousandAndSeventySeven = "2077"
    case missingU = "Missing U"
    case saintPerros = "SaintPerros"
    
    var id: String {
        self.rawValue
    }
}

let songs: [Song] = [.missingU, .twoThousandAndSeventySeven, .saintPerros]
let sceneForSongs: [Song: GameScene] = [
    .missingU: .cyberpunk,
    .twoThousandAndSeventySeven: .painting,
    .saintPerros: .cartoon
]

enum GameScene: String, CaseIterable, Identifiable {
    case cyberpunk = "Cyberpunk"
    case painting = "VanGogh"
    case cartoon = "Cartoon"
    
    var id: String {
        self.rawValue
    }
}

enum ImmersiveSpaceState {
    case closed
    case open
}

class GameManager: ObservableObject {
    static let shared = GameManager()

    @Published var selectedSong: Song = .missingU
    @Published var gameScene: GameScene = .cyberpunk
    @Published var immersiveSpaceState: ImmersiveSpaceState = .closed
}

struct MainContentView: View {

    var body: some View {
        VStack {
            TitleView()
            SongView()
            SceneView()
        }
    }
}

struct TitleView: View {
    var body: some View {
        HStack {
            Text("GG")
                .font(.system(size: 70, weight: .thin))
                .shadow(color: .red, radius: 5)
                .shadow(color: .red, radius: 5)
                .shadow(color: .red, radius: 50)
            
            Text("Beat")
                .font(.system(size: 70, weight: .thin))
                .shadow(color: .blue, radius: 5)
                .shadow(color: .blue, radius: 5)
                .shadow(color: .blue, radius: 50)
        }
        .padding()
    }
}

struct SongView: View {
    
    @StateObject var gameManager: GameManager = GameManager.shared
    
    var body: some View {
        ScrollView(.horizontal, showsIndicators: false) {
            HStack(spacing: 20) {
                ForEach(0 ..< songs.count, id: \.self) { index in
                let song = songs[index]
                    VStack {
                        Image(uiImage: UIImage(contentsOfFile: Bundle.main.path(forResource: song.rawValue, ofType: "png", inDirectory: "Data/Raw") ?? "") ?? UIImage())
                            .resizable()
                            .scaledToFit()
                            .cornerRadius(30)
                        
                        Text(song.rawValue)
                            .font(.title)
                    }
                    .padding()
                    .glassBackgroundEffect()
                    .onTapGesture {
                        self.gameManager.selectedSong = song
                        self.gameManager.gameScene = sceneForSongs[song] ?? .cyberpunk
                    }
                }
            }
        }
        .frame(height: 350)
        .onDisappear {
            callCSharp(command: "closePlayground", argsJson: "{}")
            callCSharp(command: "CloseMainMenu", argsJson: "{}")
            gameManager.immersiveSpaceState = .closed
        }
    }
}

struct SceneView: View {
    @StateObject var gameManager: GameManager = GameManager.shared

    var body: some View {
        VStack {
            Picker("Select Game Scene", selection: $gameManager.gameScene) {
                ForEach(GameScene.allCases) { scene in
                    Text(scene.rawValue).tag(scene)
                }
            }
            .pickerStyle(SegmentedPickerStyle())
            .padding()
            
            ToggleImmersiveSpaceButton()
        }
        .padding()
        .glassBackgroundEffect()
    }
}

struct ToggleImmersiveSpaceButton: View {
    
    @StateObject private var gameManager = GameManager.shared

    var body: some View {
        Button {
            if gameManager.immersiveSpaceState == .closed {
                let scene = gameManager.gameScene.rawValue
                let song = gameManager.selectedSong.rawValue
                let argsJson = "{\"song\": \"\(song)\", \"scene\": \"\(scene)\"}"
                callCSharp(command: "openPlayground", argsJson: argsJson)
                gameManager.immersiveSpaceState = .open
            } else {
                callCSharp(command: "closePlayground", argsJson: "{}")
                gameManager.immersiveSpaceState = .closed
            }
        } label: {
            Text(gameManager.immersiveSpaceState == .closed ? "Play" : "Exit")
        }
        .animation(.none, value: 0)
        .fontWeight(.semibold)
    }
}

#Preview(windowStyle: .automatic) {
    MainContentView()
}
