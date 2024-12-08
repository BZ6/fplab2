module SeparateChainingHashMapDict

// Define the SeparateChainingHashMap type
type SeparateChainingHashMap<'Key, 'Value when 'Key: comparison> =
    { Capacity: int
      Table: ('Key * 'Value) list array
      Size: int }

// Compute the hash for a given key
let hash (capacity: int) (key: 'Key) : int =
    (hash key) % capacity
    |> (fun x -> if x < 0 then x + capacity else x)

// Add a key-value pair to the map
let add
    (key: 'Key)
    (value: 'Value)
    (map: SeparateChainingHashMap<'Key, 'Value>)
    : SeparateChainingHashMap<'Key, 'Value> =
    let index = hash map.Capacity key
    let newTable = Array.copy map.Table

    // Update the list at the hashed index
    let updateList (list: ('Key * 'Value) list) : ('Key * 'Value) list * bool =
        match list with
        | [] -> [ (key, value) ], true
        | _ ->
            let exists = List.exists (fun (k, _) -> k = key) list

            if exists then
                (List.map (fun (k, v) -> if k = key then (key, value) else (k, v)) list, false)
            else
                ((key, value) :: list, true)

    let updatedList, added = updateList newTable.[index]
    newTable.[index] <- updatedList

    // Resize the map if necessary
    let resizedMap =
        if (map.Size + 1) > map.Capacity * 3 / 4 then
            { map with Table = newTable }
        else
            { map with Table = newTable }

    { resizedMap with
        Size =
            if added then
                resizedMap.Size + 1
            else
                resizedMap.Size }

// Remove a key-value pair from the map
let remove (key: 'Key) (map: SeparateChainingHashMap<'Key, 'Value>) : SeparateChainingHashMap<'Key, 'Value> =
    let index = hash map.Capacity key
    let newTable = Array.copy map.Table

    // Determine the new size after removal
    let newSize =
        let existingEntry = List.tryFind (fun (k, _) -> k = key) newTable.[index]

        match existingEntry with
        | Some _ -> map.Size - 1
        | None -> map.Size

    // Update the list at the hashed index
    let updateList (list: ('Key * 'Value) list) : ('Key * 'Value) list =
        list |> List.filter (fun (k, _) -> k <> key)

    newTable.[index] <- updateList newTable.[index]

    { map with
        Table = newTable
        Size = newSize }

// Get the value associated with a key
let getValue (key: 'Key) (map: SeparateChainingHashMap<'Key, 'Value>) : 'Value option =
    let index = hash map.Capacity key

    map.Table.[index]
    |> List.tryFind (fun (k, _) -> k = key)
    |> Option.map snd

// Filter the map based on a predicate
let filter
    (predicate: ('Key * 'Value) -> bool)
    (map: SeparateChainingHashMap<'Key, 'Value>)
    : SeparateChainingHashMap<'Key, 'Value> =
    let filteredItems =
        Array.fold (fun acc list -> List.filter predicate list @ acc) [] map.Table

    let newDict =
        { Capacity = map.Capacity
          Table = Array.create map.Capacity []
          Size = 0 }

    List.fold (fun acc (k, v) -> add k v acc) newDict filteredItems

// Apply a function to each key-value pair in the map
let map
    (mapper: ('Key * 'Value) -> ('Key * 'Value))
    (map: SeparateChainingHashMap<'Key, 'Value>)
    : SeparateChainingHashMap<'Key, 'Value> =
    let newTable = Array.create map.Capacity []

    let updatedTable =
        Array.fold
            (fun (table: ('Key * 'Value) list array) list -> 
                List.fold
                    (fun (table: ('Key * 'Value) list array) (k, v) -> 
                        let (newK, newV) = mapper (k, v)
                        let index = hash map.Capacity newK
                        table.[index] <- (newK, newV) :: table.[index]
                        table)
                    table
                    list)
            newTable
            map.Table

    { map with Table = updatedTable }

// Fold the map from left to right
let foldL
    (folder: 'State -> ('Key * 'Value) -> 'State)
    (state: 'State)
    (map: SeparateChainingHashMap<'Key, 'Value>)
    : 'State =
    Array.fold (fun acc list -> List.fold folder acc list) state map.Table

// Fold the map from right to left
let foldR
    (folder: ('Key * 'Value) -> 'State -> 'State)
    (state: 'State)
    (map: SeparateChainingHashMap<'Key, 'Value>)
    : 'State =
    Array.foldBack (fun list acc -> List.foldBack folder list acc) map.Table state

// Create an empty map with a given capacity
let createEmpty (capacity: int) : SeparateChainingHashMap<'Key, 'Value> =
    { Capacity = capacity
      Table = Array.create capacity []
      Size = 0 }

// Merge two maps into one
let merge
    (dict1: SeparateChainingHashMap<'Key, 'Value>)
    (dict2: SeparateChainingHashMap<'Key, 'Value>)
    : SeparateChainingHashMap<'Key, 'Value> =
    let newCapacity = max dict1.Capacity dict2.Capacity
    let newMap = createEmpty newCapacity

    let addAll (map1: SeparateChainingHashMap<'Key, 'Value>) (map2: SeparateChainingHashMap<'Key, 'Value>) =
        Array.fold (fun updatedMap list -> List.fold (fun acc (k, v) -> add k v acc) updatedMap list) map2 map1.Table

    newMap |> addAll dict1 |> addAll dict2

// Compare two SeparateChainingHashMap instances
let compare
    (map1: SeparateChainingHashMap<'Key, 'Value>)
    (map2: SeparateChainingHashMap<'Key, 'Value>)
    : bool =
    if map1.Size <> map2.Size then
        false
    else
        let compareLists list1 list2 =
            let rec compareElements l1 l2 =
                match l1, l2 with
                | [], [] -> true
                | (k1, v1) :: t1, _ ->
                    match List.tryFind (fun (k2, v2) -> k1 = k2 && v1 = v2) l2 with
                    | Some _ -> compareElements t1 (List.filter (fun (k2, _) -> k1 <> k2) l2)
                    | None -> false
                | _ -> false
            compareElements list1 list2

        Array.forall2 compareLists map1.Table map2.Table
