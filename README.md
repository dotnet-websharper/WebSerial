# WebSharper Web Serial API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Web Serial API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Serial_API), enabling seamless communication with serial devices in WebSharper applications.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Web Serial API.

2. **Sample Project**:
   - Demonstrates how to use the Web Serial API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/WebSerial/).

## Installation

To use this package in your WebSharper project, add the NuGet package:

```bash
   dotnet add package WebSharper.WebSerial
```

## Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/WebSerial.git
   cd WebSerial
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.WebSerial/WebSharper.WebSerial.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.WebSerial.Sample
   dotnet build
   dotnet run
   ```

4. Open the hosted demo to see the Sample project in action:
   [https://dotnet-websharper.github.io/WebSerial/](https://dotnet-websharper.github.io/WebSerial/)

## Example Usage

Below is an example of how to use the Web Serial API in a WebSharper project:

```fsharp
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
    // Define the connection to the HTML template
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    // Variable to hold the serial port
    let port = Var.Create<SerialPort> JS.Undefined

    // Variable to display the connection status
    let statusMessage = Var.Create "Waiting..."

    // Function to request a serial connection and send data
    let connectAndSend () =
        promise {
            try
                // Request a serial port from the user
                let! port = JS.Window.Navigator.Serial.RequestPort()

                // Open the serial port with specified baud rate
                do! port.Open(SerialPortOpenOptions(
                    baudRate = 9600
                ))

                // Write data to the serial port
                let writer = port.Writable.GetWriter()
                let encoder = JS.Eval "new TextEncoder()"
                do! writer.Write(encoder?encode("Hello\n")) |> Promise.AsAsync
                writer.ReleaseLock()

                statusMessage.Value <- "Message Sent!"
            with ex ->
                // Handle errors during connection or data transmission
                statusMessage.Value <- $"Error: {ex.Message}"
        }

    [<SPAEntryPoint>]
    let Main () =
        // Initialize the UI template and bind serial connection status
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
```

This example demonstrates how to request and connect to a serial device, send data through the serial connection, and handle errors using the Web Serial API in a WebSharper project.

## Important Considerations

- **Permissions**: Users must grant permission for serial access when prompted by the browser.
- **Secure Context**: The Web Serial API only works on secure origins (HTTPS) for security reasons.
- **Limited Browser Support**: Some browsers may not fully support the Web Serial API; check [MDN Web Serial API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Serial_API) for the latest compatibility information.
