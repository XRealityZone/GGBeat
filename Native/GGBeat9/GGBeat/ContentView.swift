import RealityKit
import RealityKitContent
import SwiftUI

struct ContentView: View {
    var body: some View {
        VStack {
            // NEW!
            // 9. 界面美化
            TitleView()
            // 8. 主题变换
            SongView()

            // NEW!
            // 9. 界面美化
            VStack {
                // 8. 主题变换
                SceneView()
                ToggleImmersiveSpaceButton()
            }
            .padding()
            .glassBackgroundEffect()
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

// NEW!
// 9. 界面美化
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

#Preview(windowStyle: .automatic) {
    ContentView()
        .environment(AppModel())
}
