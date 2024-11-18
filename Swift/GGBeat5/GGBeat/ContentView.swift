//
//  ContentView.swift
//  GGBeat
//
//  Created by xuchi on 2024/11/16.
//

import RealityKit
import RealityKitContent
import SwiftUI

struct ContentView: View {
    var body: some View {
        VStack {
            Text("GG Beat")
                .font(.title)

            ToggleImmersiveSpaceButton()
        }
        .padding()
    }
}

#Preview(windowStyle: .automatic) {
    ContentView()
        .environment(AppModel())
}
