namespace WebSharper.WebSerial.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.WebSerial

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let port = Var.Create<SerialPort> JS.Undefined

    let serial = As<Navigator>(JS.Window.Navigator).Serial
    
    let statusMessage = Var.Create "Waiting..."
    
    let connectAndSend () =
        promise {
            try
                // Request a serial port
                let! port = serial.RequestPort()
                
                do! port.Open(SerialPortOpenOptions(
                    baudRate = 9600
                ))

                // Write data
                let writer = port.Writable.GetWriter()
                let encoder = JS.Eval "new TextEncoder()"
                do! writer.Write(encoder?encode("Hello\n")) |> Promise.AsAsync
                writer.ReleaseLock()

                statusMessage.Value <- "Message Sent!"
            with ex ->
                statusMessage.Value <- $"Error: {ex.Message}"
        }

    [<SPAEntryPoint>]
    let Main () =

        IndexTemplate.Main()
            .connectAndSend(fun _ ->
                async {
                    do! connectAndSend().AsAsync()
                }
                |> Async.Start
            )
            .status(statusMessage.V)
            .Doc()
        |> Doc.RunById "main"
