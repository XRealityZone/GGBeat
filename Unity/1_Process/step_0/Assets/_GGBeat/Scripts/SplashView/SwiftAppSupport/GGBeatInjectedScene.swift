// Any swift file whose name ends in "InjectedScene" is expected to contain
// a computed static "scene" property like the one below. It will be injected to the top
// level App's Scene. The name of the class/struct must match the name of the file.

import Foundation
import SwiftUI

struct GGBeatInjectedScene {
    @SceneBuilder
    static var scene: some Scene {
        WindowGroup(id: "Main") {
            // The sample defines a custom view, but you can also put your entire window's
            // structure here as you can with SwiftUI.
            MainContentView()
        }
        .defaultSize(width: 500.0, height: 750.0)
        .windowStyle(.plain)
    }
}
