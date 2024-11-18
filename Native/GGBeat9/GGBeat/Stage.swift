import RealityKit
import Spatial

// 定义主模型
struct Stage: Decodable {
    let version: String
    let events: [String] // 假设 events 是一个字符串数组，根据实际情况调整
    let notes: [Note]
    let obstacles: [Obstacle]

    // 自定义键名映射
    enum CodingKeys: String, CodingKey {
        case version = "_version"
        case events = "_events"
        case notes = "_notes"
        case obstacles = "_obstacles"
    }
}

// 定义 Note 模型
struct Note: Decodable {
    enum LineIndex: Int, Decodable {
        case leftmost = 0
        case left = 1
        case right = 2
        case rightmost = 3

        var column: BeatmapColumn {
            switch self {
            case .leftmost:
                return .leftmost
            case .left:
                return .left
            case .right:
                return .right
            case .rightmost:
                return .rightmost
            }
        }
    }

    enum LineLayer: Int, Decodable {
        case bottom = 0
        case middle = 1
        case top = 2

        var row: BeatmapRow {
            switch self {
            case .bottom:
                return .bottom
            case .middle:
                return .middle
            case .top:
                return .top
            }
        }
    }

    let time: Double
    let lineIndex: LineIndex
    let lineLayer: LineLayer
    let type: Int
    let cutDirection: Int

    // 自定义键名映射
    enum CodingKeys: String, CodingKey {
        case time = "_time"
        case lineIndex = "_lineIndex"
        case lineLayer = "_lineLayer"
        case type = "_type"
        case cutDirection = "_cutDirection"
    }
}

// 节奏所在列
public enum BeatmapColumn: Int {
    case leftmost = 0
    case left = 1
    case right = 2
    case rightmost = 3
}

// 节奏所在行
public enum BeatmapRow: Int {
    case bottom = 0
    case middle = 1
    case top = 2
}

// 定义 Obstacle 模型
struct Obstacle: Codable {
    let time: Double
    let lineIndex: Int
    let type: Int
    let duration: Double
    let width: Int

    // 自定义键名映射
    enum CodingKeys: String, CodingKey {
        case time = "_time"
        case lineIndex = "_lineIndex"
        case type = "_type"
        case duration = "_duration"
        case width = "_width"
    }
}

/**
 * 音符处理方法
 */
func generateStart(note: Note) -> Point3D {
    var x = 0.0
    switch note.lineIndex {
    case .leftmost:
        x = -0.3
    case .left:
        x = -0.3
    case .right:
        x = 0.3
    case .rightmost:
        x = 0.3
    }
    var y = 0.0
    switch note.lineLayer {
    case .bottom:
        y = 1.1
    case .middle:
        y = 1.5
    case .top:
        y = 1.9
    }
    return Point3D(
        x: x,
        y: y,
        z: -(BoxSpawnParameters.deltaZ / 2) - 0.5
    )
}

func generateBoxMovementAnimation(start: Point3D) -> AnimationResource {
    let end = Point3D(
        x: start.x + BoxSpawnParameters.deltaX,
        y: start.y + BoxSpawnParameters.deltaY,
        z: start.z + BoxSpawnParameters.deltaZ
    )

    let line = FromToByAnimation<Transform>(
        name: "line",
        from: .init(scale: .init(repeating: 1), translation: simd_float(start.vector)),
        to: .init(scale: .init(repeating: 1), translation: simd_float(end.vector)),
        duration: BoxSpawnParameters.lifeTime,
        bindTarget: .transform
    )

    let animation = try! AnimationResource
        .generate(with: line)
    return animation
}

enum BoxSpawnParameters {
    static var deltaX = 0.02
    static var deltaY = -0.12
    static var deltaZ = 12.0

    static var lifeTime = 3.0
}
