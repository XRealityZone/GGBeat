import RealityKit
import RealityKitContent
import SwiftUI

struct ImmersiveView: View {
    @StateObject var gameManager: GameManager = .shared

    var body: some View {
        RealityView { content in
            // Add the initial RealityKit content
            if let immersiveContentEntity = try? await Entity(named: "Immersive", in: realityKitContentBundle) {
                content.add(immersiveContentEntity)

                // 4. 音符加载
                content.add(gameManager.root)

                // NEW!
                // 7. 安装拳套
                gameManager.handleCollision(content: content)

                // Put skybox here.  See example in World project available at
                // https://developer.apple.com/
            }
        }
        // 3. 音乐融合
        .onAppear {
            gameManager.start()
        }
        .onDisappear {
            gameManager.stop()
        }
        // 6. 精准打击
        .gesture(
            SpatialTapGesture()
                .targetedToAnyEntity()
                .onEnded { value in
                    gameManager.handlePunch(box: value.entity)
                }
        )
    }
}

#Preview(immersionStyle: .mixed) {
    ImmersiveView()
        .environment(AppModel())
}
