module UTests

open Xunit
open FsCheck
open FsCheck.Xunit

open SeparateChainingHashMapDict

let ifEqual expected actual =
    if expected <> actual then
        failwithf "Expected: %A but got: %A" expected actual

[<Fact>]
let ``Add and Get Value`` () =
    let map = createEmpty 10
    let map = add "key1" "value1" map
    let value = getValue "key1" map

    ifEqual (Some "value1") value

[<Fact>]
let ``Remove Value`` () =
    let map = createEmpty 10
    let map = add "key1" "value1" map
    let map = remove "key1" map
    let value = getValue "key1" map

    ifEqual None value

[<Fact>]
let ``foldL should correctly fold the map`` () =
    let map = createEmpty 10
    let map = add 1 "value1" map
    let map = add 2 "value2" map
    let map = add 3 "value3" map
    let foldedValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] map

    let expectedValues =
        [ (3, "value3")
          (2, "value2")
          (1, "value1") ]
    ifEqual expectedValues foldedValues

[<Fact>]
let ``foldR should correctly fold the map`` () =
    let map = createEmpty 10
    let map = add 1 "value1" map
    let map = add 2 "value2" map
    let map = add 3 "value3" map
    let foldedValues = foldR (fun (k, v) acc -> (k, v) :: acc) [] map

    let expectedValues =
        [ (1, "value1")
          (2, "value2")
          (3, "value3") ]
    ifEqual expectedValues foldedValues

[<Fact>]
let ``Filter Values`` () =
    let map = createEmpty 10
    let map = add "key1" "value1" map
    let map = add "key2" "value2" map
    let map = add "key3" "value3" map
    let filteredMap = filter (fun (k, v) -> k = "key2") map
    let values = foldL (fun acc (k, v) -> (k, v) :: acc) [] filteredMap

    let expectedValues = [ ("key2", "value2") ]
    ifEqual expectedValues values

[<Fact>]
let ``Merge Maps`` () =
    let map1 = createEmpty 10
    let map1 = add 1 "value1" map1
    let map2 = createEmpty 10
    let map2 = add 2 "value2" map2
    let mergedMap = merge map1 map2
    let values = foldL (fun acc (k, v) -> (k, v) :: acc) [] mergedMap

    let expectedValues = [ (2, "value2"); (1, "value1") ]
    ifEqual expectedValues values

[<Fact>]
let ``Create Empty Map`` () =
    let map = createEmpty 10
    
    ifEqual 10 map.Capacity
    ifEqual 0 map.Size
    ifEqual true (Array.forall List.isEmpty map.Table)

[<Fact>]
let ``Compare Maps`` () =
    let map1 = createEmpty 10
    let map1 = add "key1" "value1" map1
    let map1 = add "key2" "value2" map1

    let map2 = createEmpty 10
    let map2 = add "key1" "value1" map2
    let map2 = add "key2" "value2" map2

    let map3 = createEmpty 10
    let map3 = add "key1" "value1" map3
    let map3 = add "key3" "value3" map3

    ifEqual true (compare map1 map2)
    ifEqual false (compare map1 map3)
