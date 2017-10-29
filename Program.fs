module MyModule

open System
open System.Net.Sockets
open System.IO
open System.Text
// TODO: are all these imports needed? and does the order of imports matter?
open Utf8Json
open Utf8Json.Resolvers
open Utf8Json.FSharp
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp

CompositeResolver.RegisterAndSetAsDefault(
  FSharpResolver.Instance,
  StandardResolver.Instance
)

let my_sock =
    let client = TcpClient()
    client.Connect ("localhost", 6666)
    client

let reader = new BinaryReader(my_sock.GetStream())
let writer = new BinaryWriter(my_sock.GetStream())

let send stuff =
    let bytes = MessagePackSerializer.Serialize(stuff)
    writer.Write(bytes)

let receive _ =
    let bytes: byte[] = Array.create my_sock.ReceiveBufferSize (byte(0))
    reader.Read (bytes, 0, int my_sock.ReceiveBufferSize) |> ignore
    MessagePackSerializer.ToJson(bytes)

[<EntryPoint>]
let main argv =
    send (0, 1337, "vim_get_api_info", [||]) |> ignore
    receive ()
    |> printfn "%s"
    0 // return an integer exit code
