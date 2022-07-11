# SyncUtil
Sync Utilities For UNet


:warning: [UNet is depreciated by Unity](https://support.unity.com/hc/en-us/articles/360001252086-UNet-Deprecation-FAQ).  
if you use [Mirror](https://github.com/vis2k/Mirror) instead, please see [SyncUtilForMirror](https://github.com/fuqunaga/SyncUtilForMirror)
  

## Install Dependencies
 - [UNet(Multplayer HLAPI)](https://docs.unity3d.com/Manual/UNetUsingHLAPI.html) via PakcageManager.

## Install

Download a `.unitypackage` file from [Release page](https://github.com/fuqunaga/SyncUtil/releases).

or

Using Pacakge Manager:  
Add following line to the `dependencies` section in the `Packages/manifest.json`.
```
"ga.fuquna.syncutil": "https://github.com/fuqunaga/SyncUtil.git"
```

## How to run Examples

1. Add all scene files(*.unity) to `Scenes In Build` of Build Settings.
1. Open and run SyncUtilExamples scene. this is a example scene launcher.

## Syncing Parameters

### Primitive Members
[![](http://img.youtube.com/vi/RoescKd70Fs/0.jpg)](https://www.youtube.com/watch?v=RoescKd70Fs)

### Behaviour.enabled, GameObject active
[![](http://img.youtube.com/vi/C39lSQUmYyY/0.jpg)](https://www.youtube.com/watch?v=C39lSQUmYyY)

## Random Per Instance
[![](http://img.youtube.com/vi/Jml_K5ipCZI/0.jpg)](https://www.youtube.com/watch?v=Jml_K5ipCZI)

## Spawner, ServerOrStandAlone
[![](http://img.youtube.com/vi/_fBlCKlia4A/0.jpg)](https://www.youtube.com/watch?v=_fBlCKlia4A)  
auto regist spawnable prefab  
  
[![](http://img.youtube.com/vi/2qMK0PuPIHY/0.jpg)](https://www.youtube.com/watch?v=2qMK0PuPIHY)  
Spawner: spawn prefabs  
ServerOrStandAlone: disable children on client  
  
## Scene Load Helper
[![](http://img.youtube.com/vi/RQmx5Dr5_MQ/0.jpg)](https://www.youtube.com/watch?v=RQmx5Dr5_MQ)  
unload online/offline scene on hierarchy when application play  

## Latency Check, Delay Rendering
[![](http://img.youtube.com/vi/WXi7Jfautpw/0.jpg)](https://www.youtube.com/watch?v=WXi7Jfautpw)  
dynamic delay rendering according to network latency 

## LockStep, LockStepGPU
[![](http://img.youtube.com/vi/NmddY56bRPk/0.jpg)](https://www.youtube.com/watch?v=NmddY56bRPk)  


# Reference
 - https://github.com/fuqunaga/PrefsGUISyncUNET
 - https://github.com/nobnak/SyncTransform  
 - https://github.com/nobnak/Gist/blob/master/PreRendering.cs
