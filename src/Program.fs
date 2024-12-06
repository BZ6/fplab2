open System.Diagnostics
open SeparateChainingHashMapDict

// Calculate time of solution
let printTimeFunc message f num =
    let stopwatch = Stopwatch.StartNew()
    let result = f num
    stopwatch.Stop()
    printfn "%s %A" message result
    printfn "Time taken: %A ms" stopwatch.ElapsedMilliseconds

[<EntryPoint>]
let main argv =
    // Create an empty hash map
    let map = createEmpty 10

    // Add some elements
    let map = add "key1" "value1" map
    let map = add "key2" "value2" map
    let map = add "key3" "value3" map
    let map = add "key4" "value4" map

    // Get values by keys
    let value1 = getValue "key1" map
    let value2 = getValue "key2" map
    let value3 = getValue "key3" map
    let value4 = getValue "key4" map

    // Output the values
    printfn "Value for 'key1': %A" value1
    printfn "Value for 'key2': %A" value2
    printfn "Value for 'key3': %A" value3
    printfn "Value for 'key4': %A" value4

    // Remove an element
    let map = remove "key2" map

    // Get value after removal
    let value2AfterRemove = getValue "key2" map
    printfn "Value for 'key2' after removal: %A" value2AfterRemove

    // Filter elements
    let filteredMap = filter (fun (k: string, v: string) -> k.StartsWith "key") map

    // Output the filtered values
    let filteredValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] filteredMap
    printfn "Filtered values: %A" filteredValues

    // Check the merging of two hash tables
    let map2 = createEmpty 10
    let map2 = add "key5" "value5" map2
    let map2 = add "key6" "value6" map2

    let mergedMap = merge map map2

    // Output the values after merging
    let mergedValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] mergedMap
    printfn "Merged values: %A" mergedValues

    0
