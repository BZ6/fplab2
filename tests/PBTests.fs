module PBTests

open Xunit
open FsCheck
open FsCheck.Xunit

open SeparateChainingHashMapDict

[<Property>]
let ``Adding and getting a value should return the same value`` (key: string) (value: string) =
    let map = createEmpty 10
    let map = add key value map
    let result = getValue key map
    result = Some value

[<Property>]
let ``Removing a value should result in None`` (key: string) (value: string) =
    let map = createEmpty 10
    let map = add key value map
    let map = remove key map
    let result = getValue key map
    result = None

[<Property>]
let ``Filtering should return the correct values`` (pairs: (string * string) list) =
    let map = createEmpty 10
    let map = List.fold (fun map (key, value) -> add key value map) map pairs
    let filteredMap = filter (fun (k, v) -> k = "key") map
    let filteredValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] filteredMap
    let expectedValues = List.filter (fun (k, v) -> k = "key") pairs
    // Сортируем списки перед сравнением
    let sortedFilteredValues = List.sort filteredValues
    let sortedExpectedValues = List.sort expectedValues
    Assert.Equal<(string * string) list>(sortedExpectedValues, sortedFilteredValues)

[<Property>]
let ``Merging two maps should combine their values`` (pairs1: (int * int) list) (pairs2: (int * int) list) =
    let map1 = createEmpty 10
    let map1 = List.fold (fun map (key, value) -> add key value map) map1 pairs1
    let map2 = createEmpty 10
    let map2 = List.fold (fun map (key, value) -> add key value map) map2 pairs2
    let mergedMap = merge map1 map2
    let mergedValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] mergedMap

    // Ensure values from the second map overwrite values from the first map
    let expectedValues =
        pairs2 @ (List.filter (fun (k, _) -> not (List.exists (fun (k2, _) -> k = k2) pairs2)) pairs1)
        |> List.rev
        |> List.distinctBy fst // Удаляем дубликаты ключей

    // Сортируем списки перед сравнением
    let sortedMergedValues = List.sort mergedValues
    let sortedExpectedValues = List.sort expectedValues
    Assert.Equal<(int * int) list>(sortedExpectedValues, sortedMergedValues)
