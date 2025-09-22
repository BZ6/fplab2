# Лабораторная работа №2

`Захаркин Богдан`
`367224`

## Вариант - sc-dict

Реализован интерфейс взаимодействия Dict на основе структуры данных Separate Chaining HashMap

## Требования

1. Функции:
    - добавление и удаление элементов;
    - фильтрация;
    - отображение (map);
    - свертки (левая и правая);
    - структура должна быть [моноидом](https://ru.m.wikipedia.org/wiki/Моноид).
2. Структуры данных должны быть неизменяемыми.
3. Библиотека должна быть протестирована в рамках unit testing.
4. Библиотека должна быть протестирована в рамках property-based тестирования (как минимум 3 свойства, включая свойства моноида).
5. Структура должна быть полиморфной.
6. Требуется использовать идиоматичный для технологии стиль программирования. Примечание: некоторые языки позволяют получить
большую часть API через реализацию небольшого интерфейса. Так как лабораторная работа про ФП, а не про экосистему языка -- необходимо
реализовать их вручную и по возможности -- обеспечить совместимость.

## Ключевые элементы реализации

### Добавление элемента

```fsharp
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
```

### Удаление элемента

```fsharp
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
```

### Получение значения по ключу

```fsharp
let getValue (key: 'Key) (map: SeparateChainingHashMap<'Key, 'Value>) : 'Value option =
    let index = hash map.Capacity key

    map.Table.[index]
    |> List.tryFind (fun (k, _) -> k = key)
    |> Option.map snd
```

### Фильтрация

```fsharp
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
```

### Отображение

```fsharp
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
```

### Свертки (левая и правая)

```fsharp
let foldL
    (folder: 'State -> ('Key * 'Value) -> 'State)
    (state: 'State)
    (map: SeparateChainingHashMap<'Key, 'Value>)
    : 'State =
    Array.fold (fun acc list -> List.fold folder acc list) state map.Table
```

```fsharp
let foldR
    (folder: ('Key * 'Value) -> 'State -> 'State)
    (state: 'State)
    (map: SeparateChainingHashMap<'Key, 'Value>)
    : 'State =
    Array.foldBack (fun list acc -> List.foldBack folder list acc) map.Table state
```

### Структура должна быть моноидом

#### Нейтральный элемент

```fsharp
let createEmpty (capacity: int) : SeparateChainingHashMap<'Key, 'Value> =
    { Capacity = capacity
      Table = Array.create capacity []
      Size = 0 }
```

#### Бинарная операция (слияние)

Если ключ совпал, то выбираем значение у правого словаря. Т.е. просто перезаписываем новое значение вместо старого.

```fsharp
let merge
    (dict1: SeparateChainingHashMap<'Key, 'Value>)
    (dict2: SeparateChainingHashMap<'Key, 'Value>)
    : SeparateChainingHashMap<'Key, 'Value> =
    let newCapacity = max dict1.Capacity dict2.Capacity
    let newMap = createEmpty newCapacity

    let addAll (map1: SeparateChainingHashMap<'Key, 'Value>) (map2: SeparateChainingHashMap<'Key, 'Value>) =
        Array.fold (fun updatedMap list -> List.fold (fun acc (k, v) -> add k v acc) updatedMap list) map2 map1.Table

    newMap |> addAll dict1 |> addAll dict2
```

### Сравнение

```fsharp
let compare (map1: SeparateChainingHashMap<'Key, 'Value>) (map2: SeparateChainingHashMap<'Key, 'Value>) : bool =
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
```

## Вывод

В ходе выполнения данной лабораторной работы я более подробно изучил хеш таблицы и операции с ними, а так же
познакомился с концепцией Property-based тестов. Попытался реализовать свою структуру Separate Chaining HashMap,
что вышло далеко не спервого раза. В написании той реализации которой я хотел, мне сильно помогли Unit и
Property-based тесты, так как я благодаря  ним находил ошибки или недочеты в коде.

Так же я хотел бы заметить, что встроенная функция `hash` не гарантирует идентичность выдаваемых хеш кодов между
запусками, чтобы с этим справиться мне пришлось переписать все ключи в тестах с `string` на `int`, так как иначе
они сортировались по этим ключам в хеш таблице в рандомном порядке, что с помощью тестов было невозможно
предугадать без применения функции `hash`, что сильно бы усложнило тесты.
