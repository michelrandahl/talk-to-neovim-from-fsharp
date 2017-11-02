module MyModule

open System
open System.Net.Sockets
open System.IO
open System.Text
open Utf8Json
//open Utf8Json.Resolvers
//open Utf8Json.FSharp
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

let receive _ : byte [] =
    let bytes: byte [] = Array.create my_sock.ReceiveBufferSize (byte(0))
    reader.Read (bytes, 0, int my_sock.ReceiveBufferSize) |> ignore
    bytes

let parser1 (bytes : byte []) = MessagePackSerializer.ToJson(bytes)

[<MessagePackObject(true)>]
type MyType<'t1,'t2,'t3,'t4> =
    { version: 't1; functions: 't2; error_types: 't3; types: 't4 }

let parser2 (bytes : byte []) = MessagePackSerializer.Deserialize<Tuple<_,_,_,Tuple<_, MyType<_,_,_,_>>>>(bytes)


[<EntryPoint>]
let main argv =
    send (0, 1337, "vim_get_api_info", [||]) |> ignore
    let bytes = receive ()
    bytes
    |> parser1
    |> printfn "%A"

    0 // return an integer exit code
