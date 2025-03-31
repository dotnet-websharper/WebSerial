namespace WebSharper.WebSerial

open WebSharper
open WebSharper.JavaScript

[<JavaScript; AutoOpen>]
module Extensions =

    type Navigator with
        [<Inline "$this.serial">]
        member this.Serial with get(): Serial = X<Serial>

    type WorkerNavigator with
        [<Inline "$this.serial">]
        member this.Serial with get(): Serial = X<Serial>
