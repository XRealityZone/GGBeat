using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BeatMapData
{
  // 谱面版本
  public string _version;

  // 事件列表,可能包含灯光、特效等事件
  public List<BeatMapEvent> _events;

  // 音符列表,包含所有需要玩家击打的音符
  public List<BeatMapNote> _notes;

  // 障碍物列表,可能包含需要玩家躲避的障碍物
  public List<BeatMapObstacle> _obstacles;
}

[Serializable]
public class BeatMapEvent
{
  // 根据需要添加事件属性
}

[Serializable]
public class BeatMapNote
{
  // 音符出现的时间(以节拍数为单位)
  public float _time;

  // 音符的水平位置索引(0-3,从左到右)
  public int _lineIndex;

  // 音符的垂直位置索引(0-2,从下到上)
  public int _lineLayer;

  // 音符类型(0 为左手方块，1 为右手方块，3 为炸弹)
  public int _type;

  // 切割方向(0-8,表示不同的切割方向)
  public int _cutDirection;
}

[Serializable]
public class BeatMapObstacle
{
  // 根据需要添加障碍物属性
}
