namespace AllTheKeys

open System
open BaboonAPI.Hooks.Initializer
open BepInEx
open HarmonyLib
open UnityEngine

[<HarmonyPatch(typeof<GameController>, "Start")>]
type AllKeysPatch() =
    static let extras =
        [ KeyCode.LeftArrow
          KeyCode.RightArrow
          KeyCode.LeftControl
          KeyCode.RightControl
          KeyCode.Space ]

    static member Postfix(___toot_keys: KeyCode ResizeArray) =
        ___toot_keys.Clear()

        [ 'A' .. 'Z' ]
        |> Seq.map (string >> KeyCode.Parse)
        |> ___toot_keys.AddRange

        ___toot_keys.AddRange extras

[<BepInPlugin("ch.offbeatwit.allthekeys", "AllTheKeys", "1.0.0")>]
[<BepInDependency("ch.offbeatwit.baboonapi.plugin", "2.0.0")>]
type AllTheKeysPlugin() =
    inherit BaseUnityPlugin()

    let harmony =
        Harmony("ch.offbeatwit.allthekeys.harmony")

    member this.Awake() =
        GameInitializationEvent.EVENT.Register this

    interface GameInitializationEvent.Listener with
        member this.Initialize() =
            GameInitializationEvent.attempt this.Info (fun () -> harmony.PatchAll typeof<AllKeysPatch>)
