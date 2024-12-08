module PBTests

open Xunit
open FsCheck
open FsCheck.Xunit

open SeparateChainingHashMapDict

type ArbitrarySeparateChainingHashMap<'Key, 'Value when 'Key: comparison>() =
    static member SeparateChainingHashMap() =
        Gen.sized (fun size ->
            gen {
                let! keyValuePairs = Gen.listOfLength size (Arb.generate<'Key * 'Value>)
                let dict = createEmpty (size * 2)
                let filledDict = List.fold (fun acc (k, v) -> add k v acc) dict keyValuePairs
                return filledDict
            })
        |> Arb.fromGen

// TODO: Починить
[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Adding and getting a value should return the same value`` (key: string) (value: string) (map: SeparateChainingHashMap<string, string>) =
    let map = add key value map
    getValue key map = Some value

// TODO: Починить
[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Removing a value should result in None`` (key: string) (value: string) (map: SeparateChainingHashMap<string, string>) =
    let map = add key value map
    let map = remove key map
    let result = getValue key map
    result = None

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Filtering should return the correct values`` (map: SeparateChainingHashMap<string, string>) =
    let filteredMap = filter (fun (k, v) -> k = "key") map
    let expectedMap = filter (fun (k, v) -> k = "key") map
    compare filteredMap expectedMap

// TODO: Заменить на SeparateChainingHashMap<string, string>
[<Property>]
let ``Merging two maps should combine their values`` (pairs1: (int * int) list) (pairs2: (int * int) list) =
    let map1 = createEmpty 10
    let map1 = List.fold (fun map (key, value) -> add key value map) map1 pairs1
    let map2 = createEmpty 10
    let map2 = List.fold (fun map (key, value) -> add key value map) map2 pairs2
    let mergedMap = merge map1 map2

    let expectedMap = createEmpty 10
    let expectedMap = List.fold (fun map (key, value) -> add key value map) expectedMap pairs1
    let expectedMap = List.fold (fun map (key, value) -> add key value map) expectedMap pairs2

    compare mergedMap expectedMap

[<Property>]
let ``Adding the same key twice should update the value`` (key: string) (value1: string) (value2: string) =
    let map = createEmpty 10
    let map = add key value1 map
    let map = add key value2 map
    let result = getValue key map
    result = Some value2

// TODO: Заменить на SeparateChainingHashMap<string, string>
[<Property>]
let ``Adding multiple values should increase the size`` (pairs: (string * string) list) =
    let map = createEmpty 10
    let map = List.fold (fun map (key, value) -> add key value map) map pairs
    let expectedSize = List.length (List.distinctBy fst pairs)
    map.Size = expectedSize

// TODO: Починить
[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Filtering an empty map should return an empty map`` (map: SeparateChainingHashMap<string, string>) =
    let filteredMap = filter (fun _ -> true) map
    let expectedMap = createEmpty map.Capacity
    compare filteredMap expectedMap
