import RealityKit
import RealityKitContent
import SwiftUI

struct ContentView: View {
    var body: some View {
        VStack {
            // 1.3 修改 Hello World 文案
            Text("GG Beat")
                .font(.title)

            // NEW!
            // 8. 主题变换
            SongView()
            // 8. 主题变换
            SceneView()

            ToggleImmersiveSpaceButton()
        }
        .padding()
    }
}

// 8. 主题变换
struct SceneView: View {
    @StateObject var gameManager: GameManager = .shared

    var body: some View {
        Picker("Select Game Scene", selection: $gameManager.gameScene) {
            ForEach(GameScene.allCases) { scene in
                Text(scene.rawValue).tag(scene)
            }
        }
        .pickerStyle(SegmentedPickerStyle())
        .padding()
    }
}

// NEW!
// 8. 主题变换
struct SongView: View {
    @StateObject var gameManager: GameManager = .shared
    var body: some View {
        ScrollView(.horizontal, showsIndicators: false) {
            HStack(spacing: 20) {
                ForEach(0 ..< gameManager.songs.count, id: \.self) { index in
                    VStack {
                        Image(gameManager.songs[index].name)
                            .resizable()
                            .scaledToFit()
                            .cornerRadius(30)

                        Text(gameManager.songs[index].name)
                            .font(.title)
                    }
                    .padding()
                    .glassBackgroundEffect()
                    .onTapGesture {
                        self.gameManager.selectedSong = gameManager.songs[index]
                        self.gameManager.gameScene = gameManager.songs[index].defaultGameScene
                    }
                }
            }
        }
        .frame(height: 300)
    }
}

#Preview(windowStyle: .automatic) {
    ContentView()
        .environment(AppModel())
}
