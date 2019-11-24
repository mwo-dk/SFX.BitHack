module SFX.BitHack

open SFX.ROP
open SFX.BitHack.CSharp

type BitAccessError =
| InvalidArrayLength of int
| IndexOutOfRange of int
| IndicesOutOfRange of int*int
| IncompatipleLengths of int*int

type BitVector = int64 array
type BitArray = {
    Length: int
    Data: BitVector
}

let private getArrayLength n div = if n > 0 then ((n-1)/div) + 1 else 0

let create n defaultValue =
    match n with
    | n when n < 0 -> n |> InvalidArrayLength |> fail
    | _ -> 
        let length = getArrayLength n 64
        {
            Length = n;
            Data = if defaultValue then Array.init length (fun _ -> ~~~0L) else  Array.zeroCreate length
        } |> succeed

let zeroCreate n = false |> create n

let get n array = 
    let struct (ok, result) = BitVectorHelpers.GetSafe(array.Data, array.Length, n)
    if ok then result |> succeed
    else n |> IndexOutOfRange |> fail

let set n value array =
    let ok = BitVectorHelpers.SetSafe(array.Data, array.Length, n, value)
    if ok then () |> succeed
    else n |> IndexOutOfRange |> fail

let getRange from _to array =
    let struct (ok, result) = BitVectorHelpers.GetRangeSafe(array.Data, array.Length, from, _to)
    if ok then result |> succeed
    else (from, _to) |> IndicesOutOfRange |> fail

let setRange from _to value array =
    let ok = BitVectorHelpers.SetRangeSafe(array.Data, array.Length, from, _to, value)
    if ok then () |> succeed
    else (from, _to) |> IndicesOutOfRange |> fail

let invert array = 
    let data = Array.zeroCreate (getArrayLength array.Length 64)
    BitVectorHelpers.Not(array.Data, data)
    {array with Data = data}

let private bitwiseOp x y op =
    let xLength, yLength = x.Length, y.Length
    if xLength <> yLength then (xLength, yLength) |> IncompatipleLengths |> fail
    else
        let data = Array.zeroCreate xLength
        op(x.Data, y.Data, data)
        {Length = xLength; Data = data} |> succeed

let bitwiseAndOp x y = BitVectorHelpers.And |> bitwiseOp x y
let bitwiseOrOp x y = BitVectorHelpers.Or |> bitwiseOp x y
let bitwiseXorOp x y = BitVectorHelpers.Xor |> bitwiseOp x y

module Operators =
    let (~~~) = invert
    let (&&&) = bitwiseAndOp
    let (|||) = bitwiseOrOp
    let (^^^) = bitwiseXorOp
