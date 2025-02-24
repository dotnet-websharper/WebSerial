namespace WebSharper.WebSerial

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    module Enum =
        let FlowControl =
            Pattern.EnumStrings "FlowControl" [
                "none"
                "hardware"
            ]

        let Parity =
            Pattern.EnumStrings "Parity" [
                "none"
                "even"
                "odd"
            ]

    let StringOrUnsignedInt = T<string> + T<uint>
    
    let SerialPortFilters =
        Pattern.Config "SerialPortFilters" {
            Required = []
            Optional = [
                "bluetoothServiceClassId", StringOrUnsignedInt 
                "usbVendorId", T<uint> 
                "usbProductId", T<uint> 
                 
            ]
        }
    
    let SerialPortRequestOptions =
        Pattern.Config "SerialPortRequestOptions" {
            Required = []
            Optional = [
                "filters", !| SerialPortFilters 
                "allowedBluetoothServiceClassIds", !| StringOrUnsignedInt
            ]
        }

    let SerialPortInfo =
        Pattern.Config "SerialPortInfo" {
            Required = []
            Optional = [
                "usbVendorId", T<uint> 
                "usbProductId", T<uint> 
                "bluetoothServiceClassId", StringOrUnsignedInt
            ]
        }
    
    let SerialPortSignals =
        Pattern.Config "SerialPortSignals" {
            Required = []
            Optional = [
                "clearToSend", T<bool>
                "dataCarrierDetect", T<bool>
                "dataSetReady", T<bool>
                "ringIndicator", T<bool>
            ]
        }

    let SerialPortOpenOptions =
        Pattern.Config "SerialPortOpenOptions" {
            Required = [
                "baudRate", T<int> 
            ]
            Optional = [
                "bufferSize", T<int> 
                "dataBits", T<int> 
                "flowControl", Enum.FlowControl.Type 
                "parity", Enum.Parity.Type 
                "stopBits", T<int> 
            ]
        }

    let SerialPortSignalOptions =
        Pattern.Config "SerialPortSignalOptions" {
            Required = []
            Optional = [
                "dataTerminalReady", T<bool> 
                "requestToSend", T<bool> 
                "break", T<bool> 
            ]
        }
    
    let SerialPort =
        Class "SerialPort"
        |=> Inherits T<Dom.EventTarget> 
        |+> Instance [
            "connected" =? T<bool> 
            "readable" =? T<ReadableStream> 
            "writable" =? T<WritableStream> 

            "forget" => T<unit> ^-> T<Promise<unit>> 
            "getInfo" => T<unit> ^-> SerialPortInfo.Type 
            "open" => SerialPortOpenOptions?options ^-> T<Promise<unit>> 
            "close" => T<unit> ^-> T<Promise<unit>> 
            "setSignals" => !?SerialPortSignalOptions?options ^-> T<Promise<unit>> 
            "getSignals" => T<unit> ^-> T<Promise<_>>[SerialPortSignals] 

            "onconnect" =@ T<unit> ^-> T<unit>
            "onconnect" =@ T<Dom.Event> ^-> T<unit>
            "ondisconnect" =@ T<unit> ^-> T<unit>
            "ondisconnect" =@ T<Dom.Event> ^-> T<unit>
        ]

    let Serial =
        Class "Serial"
        |=> Inherits T<Dom.EventTarget> 
        |+> Instance [
            "requestPort" => !?SerialPortRequestOptions?options ^-> T<Promise<_>>[SerialPort] 
            "getPorts" => T<unit> ^-> T<Promise<_>>[!|SerialPort] 
        ]
    
    let Navigator =
        Class "Navigator"
        |+> Instance [
            "serial" =? Serial.Type
        ]

    let WorkerNavigator =
        Class "WorkerNavigator"
        |+> Instance [
            "serial" =? Serial.Type
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.WebSerial" [
                WorkerNavigator
                Navigator
                Serial
                SerialPort
                SerialPortSignalOptions
                SerialPortOpenOptions
                SerialPortSignals
                SerialPortInfo
                SerialPortRequestOptions
                SerialPortFilters
                Enum.Parity
                Enum.FlowControl
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
