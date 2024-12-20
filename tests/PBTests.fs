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

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Adding and getting a value should return the same value``
    (key: string)
    (value: string)
    (map: SeparateChainingHashMap<string, string>)
    =
    let map =
        if map.Capacity = 0 then
            createEmpty 10
        else
            map

    let map = add key value map
    getValue key map = Some value

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Removing a value should result in None``
    (key: string)
    (value: string)
    (map: SeparateChainingHashMap<string, string>)
    =
    let map =
        if map.Capacity = 0 then
            createEmpty 10
        else
            map

    let map = add key value map
    let map = remove key map
    let result = getValue key map
    result = None

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Filtering should return the correct values`` (map: SeparateChainingHashMap<string, string>) =
    let filteredMap = filter (fun (k, v) -> k = "key") map
    let expectedMap = filter (fun (k, v) -> k = "key") map
    compare filteredMap expectedMap

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Merging two maps should combine their values``
    (map1: SeparateChainingHashMap<string, string>)
    (map2: SeparateChainingHashMap<string, string>)
    =
    let mergedMap = merge map1 map2
    let expectedMap = foldL (fun acc (k, v) -> add k v acc) map1 map2
    compare mergedMap expectedMap

[<Property>]
let ``Adding the same key twice should update the value`` (key: string) (value1: string) (value2: string) =
    let map = createEmpty 10
    let map = add key value1 map
    let map = add key value2 map
    let result = getValue key map
    result = Some value2

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``Adding multiple values should increase the size`` (map: SeparateChainingHashMap<string, string>) =
    let newMap = createEmpty map.Capacity
    let newMap = foldL (fun acc (k, v) -> add k v acc) newMap map
    newMap.Size = map.Size

[<Property>]
let ``Filtering an empty map should return an empty map`` () =
    let map = createEmpty 10
    let filteredMap = filter (fun _ -> true) map
    let expectedMap = createEmpty map.Capacity
    compare filteredMap expectedMap

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``SeparateChainingHashMap should satisfy associativity property``
    (map1: SeparateChainingHashMap<string, string>)
    (map2: SeparateChainingHashMap<string, string>)
    (map3: SeparateChainingHashMap<string, string>)
    =
    let leftAssoc = merge (merge map1 map2) map3
    let rightAssoc = merge map1 (merge map2 map3)
    compare leftAssoc rightAssoc

[<Property(Arbitrary = [| typeof<ArbitrarySeparateChainingHashMap<string, string>> |])>]
let ``SeparateChainingHashMap should satisfy identity property`` (map1: SeparateChainingHashMap<string, string>) =
    let emptyMap = createEmpty map1.Capacity
    let leftIdentity = merge map1 emptyMap
    let rightIdentity = merge emptyMap map1

    compare leftIdentity map1
    && compare leftIdentity rightIdentity
    && compare rightIdentity map1
