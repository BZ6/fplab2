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
    printfn "Create an empty hash map:"
    printTimeFunc "Create empty:" (fun _ -> createEmpty 10) 0
    let map = createEmpty 10

    // Add some elements
    printfn "Add some elements:"
    printTimeFunc "Add key1:" (fun _ -> add "key1" "value1" map) 0
    let map = add "key1" "value1" map
    printTimeFunc "Add key2:" (fun _ -> add "key2" "value2" map) 0
    let map = add "key2" "value2" map
    printTimeFunc "Add key3:" (fun _ -> add "key3" "value3" map) 0
    let map = add "key3" "value3" map
    printTimeFunc "Add key4:" (fun _ -> add "key4" "value4" map) 0
    let map = add "key4" "value4" map

    // Get values by keys
    printfn "Get values by keys:"
    printTimeFunc "Get value1:" (fun _ -> getValue "key1" map) 0
    let value1 = getValue "key1" map
    printTimeFunc "Get value2:" (fun _ -> getValue "key2" map) 0
    let value2 = getValue "key2" map
    printTimeFunc "Get value3:" (fun _ -> getValue "key3" map) 0
    let value3 = getValue "key3" map
    printTimeFunc "Get value4:" (fun _ -> getValue "key4" map) 0
    let value4 = getValue "key4" map

    // Remove an element
    printfn "Remove an element:"
    printTimeFunc "Remove key2:" (fun _ -> remove "key2" map) 0
    let map = remove "key2" map

    // Get value after removal
    printfn "Get value after removal:"
    printTimeFunc "Get value2 after removal:" (fun _ -> getValue "key2" map) 0
    let value2AfterRemove = getValue "key2" map

    // Filter elements
    printfn "Filter elements:"
    printTimeFunc "Filter key1:" (fun _ -> filter (fun (k: string, v: string) -> k.StartsWith "key1") map) 0
    let filteredMap = filter (fun (k: string, v: string) -> k.StartsWith "key1") map

    // Check the merging of two hash tables
    printfn "Check the merging of two hash tables:"
    let map2 = createEmpty 10
    printTimeFunc "Add key5 to map2:" (fun _ -> add "key5" "value5" map2) 0
    let map2 = add "key5" "value5" map2
    printTimeFunc "Add key6 to map2:" (fun _ -> add "key6" "value6" map2) 0
    let map2 = add "key6" "value6" map2
    printTimeFunc "Merge maps:" (fun _ -> merge map map2) 0
    let mergedMap = merge map map2

    0
