import ARKit
import RealityKit
import RealityKitContent
import SwiftUI

// NEW!
// 7. 安装拳套
let BOX_ENTITY_NAME = "Box"
let GLOVE_ENTITY_NAME_SUFFIX = "Glove"

class GameManager: ObservableObject {
    static let shared = GameManager()
    
    @Published var selectedSong: Song
    
    var songs: [Song] = []
    
    // 4. 音符加载
    let root = Entity()
    private var boxTemplate: Entity?
    
    // 5. 节奏同步
    private var tasks: [DispatchWorkItem] = []
    
    // 6. 精准打击
    private var audioEntity: Entity?
    private var particleEmitter: Entity?
    private var hitAudio: AudioFileResource?
    
    // 7. 安装拳套
    private var leftHand: Entity?
    private var rightHand: Entity?
    private let session = ARKitSession()
    private let handTracking = HandTrackingProvider()
    
    // NEW!
    // 7. 安装拳套
    private var colisionSubs: EventSubscription?

    private init() {
        songs = [
            Song(name: "2077"),
            Song(name: "Missing U"),
            Song(name: "SaintPerros"),
        ]
        selectedSong = songs[0]
        
        // 4. 音符加载
        boxTemplate = try? Entity.load(named: "Box", in: realityKitContentBundle)
        if boxTemplate == nil {
            print("Failed to load boxTemplate.")
        }
        
        // 7. 安装拳套
        leftHand = try? Entity.load(named: "LeftGlove", in: realityKitContentBundle)
        rightHand = try? Entity.load(named: "RightGlove", in: realityKitContentBundle)
        
        // 6. 精准打击
        Task {
            print("Loading audio")
            hitAudio = try? await AudioFileResource(
                named: "hit.mp3",
                configuration: .init(shouldLoop: false)
            )
        }
    }
    
    func start() {
        // 5. 节奏同步
        selectedSong.player.numberOfLoops = 0
        selectedSong.player.currentTime = 0
        
        selectedSong.player.play()
        
        // 4. 音符加载
        // root.addChild(spawnBox())
        
        // 5. 节奏同步
        scheduleTasks(notes: selectedSong.stage.notes, startTime: DispatchTime.now())
        
        // 7. 安装拳套
        Task {
            if HandTrackingProvider.isSupported && handTracking.state == .initialized {
                try await self.session.run([self.handTracking])
                await self.processHandUpdates()
            }
        }
    }
    
    func stop() {
        selectedSong.player.stop()
        
        // 5. 节奏同步
        for task in tasks {
            task.cancel()
        }
        tasks.removeAll()
        for entity in root.children {
            entity.removeFromParent()
        }
    }
    
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
    
    // 5. 节奏同步
    func spawnBox(note: Note) -> Entity {
        guard let boxTemplate = boxTemplate else {
            fatalError("Box template is nil")
        }
        let box = boxTemplate.clone(recursive: true)
        
        let start = generateStart(note: note)
        box.position = simd_float(start.vector + .init(x: 0, y: 0, z: -0.7))
        let animation = generateBoxMovementAnimation(start: start)
        box.playAnimation(animation, transitionDuration: 1.0, startsPaused: false)
        
        root.addChild(box)
        return box
    }
    
    // 6. 精准打击
    func handlePunch(box: Entity) {
        guard let parent = box.parent,
              let audio = hitAudio,
              let audioEntity = parent.findEntity(named: "SpatialAudio"),
              let particleEmitter = parent.findEntity(named: "ParticleEmitter")
        else {
            return
        }
        parent.stopAllAnimations()
        audioEntity.playAudio(audio)
        particleEmitter.components[ParticleEmitterComponent.self]?.burst()
        box.removeFromParent()
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.5) {
            parent.removeFromParent()
        }
    }
    
    // 5. 节奏同步
    private func scheduleTasks(notes: [Note], startTime: DispatchTime) {
        for note in notes {
            let createTask = DispatchWorkItem {
                let box = self.spawnBox(note: note)
                let destroyTask = DispatchWorkItem {
                    box.removeFromParent()
                }
                self.tasks.append(destroyTask)
                DispatchQueue.main.asyncAfter(deadline: .now() + BoxSpawnParameters.lifeTime) {
                    destroyTask.perform()
                }
            }
            tasks.append(createTask)
            DispatchQueue.main.asyncAfter(deadline: startTime + note.time * 0.5 - BoxSpawnParameters.lifeTime / 2) {
                createTask.perform()
            }
        }
    }
    
    // 7. 安装拳套
    private func processHandUpdates() async {
        for await update in handTracking.anchorUpdates {
            let handAnchor = update.anchor
            guard
                handAnchor.isTracked,
                let handEntity = handAnchor.handSkeleton?.joint(.wrist),
                handEntity.isTracked else { continue }
            
            let originFromIndexFingerTip = handAnchor.originFromAnchorTransform * handEntity.anchorFromJointTransform
            
            var entity: Entity!
            if handAnchor.chirality == .left {
                entity = leftHand
            } else {
                entity = rightHand
            }
            if await entity.parent == nil {
                await root.addChild(entity)
            }
            await entity.setTransformMatrix(originFromIndexFingerTip, relativeTo: nil)
        }
    }
    
    // NEW!
    // 7. 安装拳套
    func handleCollision(content: RealityViewContent) {
        colisionSubs = content.subscribe(to: CollisionEvents.Began.self) { collisionEvent in
            let a = collisionEvent.entityA
            let b = collisionEvent.entityB
            var box: Entity!
            if a.name.hasSuffix(GLOVE_ENTITY_NAME_SUFFIX) && b.name == BOX_ENTITY_NAME {
                box = b
            } else if a.name == BOX_ENTITY_NAME && b.name.hasSuffix(GLOVE_ENTITY_NAME_SUFFIX) {
                box = a
            } else {
                return
            }
            self.handlePunch(box: box)
        }
    }
}
