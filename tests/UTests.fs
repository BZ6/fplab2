module UTests

open Xunit
open FsCheck
open FsCheck.Xunit

open SeparateChainingHashMapDict

[<Fact>]
let ``Add and Get Value`` () =
    let map = createEmpty 10
    let map = add "key1" "value1" map
    let value = getValue "key1" map
    Assert.Equal(Some "value1", value)

[<Fact>]
let ``Remove Value`` () =
    let map = createEmpty 10
    let map = add "key1" "value1" map
    let map = remove "key1" map
    let value = getValue "key1" map
    Assert.Equal(None, value)

[<Fact>]
let ``foldL should correctly fold the map`` () =
    let map = createEmpty 10
    let map = add 1 "value1" map
    let map = add 2 "value2" map
    let map = add 3 "value3" map
    let foldedValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] map
    let expectedValues = [(3, "value3"); (2, "value2"); (1, "value1")]
    Assert.Equal<(int * string) list>(expectedValues, foldedValues)

[<Fact>]
let ``foldR should correctly fold the map`` () =
    let map = createEmpty 10
    let map = add 1 "value1" map
    let map = add 2 "value2" map
    let map = add 3 "value3" map
    let foldedValues = foldR (fun (k, v) acc -> (k, v) :: acc) [] map
    let expectedValues = [(1, "value1"); (2, "value2"); (3, "value3")]
    Assert.Equal<(int * string) list>(expectedValues, foldedValues)

[<Fact>]
let ``Filter Values`` () =
    let map = createEmpty 10
    let map = add "key1" "value1" map
    let map = add "key2" "value2" map
    let map = add "key3" "value3" map
    let filteredMap = filter (fun (k, v) -> k = "key2") map
    let values = foldL (fun acc (k, v) -> (k, v) :: acc) [] filteredMap
    Assert.Collection(values,
        fun (k, v) ->
            Assert.Equal("key2", k)
            Assert.Equal("value2", v))

[<Fact>]
let ``Merge Maps`` () =
    let map1 = createEmpty 10
    let map1 = add 1 "value1" map1
    let map2 = createEmpty 10
    let map2 = add 2 "value2" map2
    let mergedMap = merge map1 map2
    let values = foldL (fun acc (k, v) -> (k, v) :: acc) [] mergedMap
    let expectedValues = [(2, "value2"); (1, "value1")]
    Assert.Equal<(int * string) list>(expectedValues, values)

[<Fact>]
let ``Create Empty Map`` () =
    let map = createEmpty 10
    Assert.Equal(10, map.Capacity)
    Assert.Equal(0, map.Size)
    Assert.True(Array.forall List.isEmpty map.Table)
