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
                
                // NEW!
                // 8. 主题变换
                await switchSkybox(sphereScene: immersiveContentEntity)
                
                // 4. 音符加载
                content.add(gameManager.root)
                
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
    
    // NEW!
    // 8. 主题变换
    func switchSkybox(sphereScene: Entity) async {
        if let skybox = sphereScene.findEntity(named: "SkySphere"),
           let skyboxModel = skybox as? ModelEntity
        {
            guard var skyboxMaterial = skyboxModel.model?.materials.first as? ShaderGraphMaterial else {
                print("Unable to load skybox material")
                return
            }
            guard let texture =
                try? await TextureResource(named: gameManager.gameScene.rawValue)
            else {
                fatalError("Unable to load texture.")
            }
            try? skyboxMaterial.setParameter(name: "SkySphere_Texture", value: .textureResource(texture))
            skyboxModel.model?.materials = [skyboxMaterial]
        }
    }
}

#Preview(immersionStyle: .mixed) {
    ImmersiveView()
        .environment(AppModel())
}
