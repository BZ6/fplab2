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
    // Создаем пустую хеш-таблицу с начальной емкостью 10
    let map = createEmpty 10

    // Добавляем несколько элементов
    let map = add "key1" "value1" map
    let map = add "key2" "value2" map
    let map = add "key3" "value3" map
    let map = add "key4" "value4" map

    // Получаем значения по ключам
    let value1 = getValue "key1" map
    let value2 = getValue "key2" map
    let value3 = getValue "key3" map
    let value4 = getValue "key4" map

    // Выводим значения
    printfn "Value for 'key1': %A" value1
    printfn "Value for 'key2': %A" value2
    printfn "Value for 'key3': %A" value3
    printfn "Value for 'key4': %A" value4

    // Удаляем элемент
    let map = remove "key2" map

    // Получаем значение после удаления
    let value2AfterRemove = getValue "key2" map
    printfn "Value for 'key2' after removal: %A" value2AfterRemove

    // Фильтруем элементы
    let filteredMap = filter (fun (k: string, v: string) -> k.StartsWith "key") map

    // Выводим отфильтрованные значения
    let filteredValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] filteredMap
    printfn "Filtered values: %A" filteredValues

    // Проверяем слияние двух хеш-таблиц
    let map2 = createEmpty 10
    let map2 = add "key5" "value5" map2
    let map2 = add "key6" "value6" map2

    let mergedMap = merge map map2

    // Выводим значения после слияния
    let mergedValues = foldL (fun acc (k, v) -> (k, v) :: acc) [] mergedMap
    printfn "Merged values: %A" mergedValues

    0 // Возвращаем 0 для успешного завершения программы
