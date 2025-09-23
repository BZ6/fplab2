# Вывод тестов

## Покрытие кода

![покрытие кода тестами](images/report2.png)

## Вывод тестов

```bash
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.8.2+699d445a1a (64-bit .NET 8.0.19)
[xUnit.net 00:00:00.05]   Discovering: tests
[xUnit.net 00:00:00.09]   Discovered:  tests
[xUnit.net 00:00:00.09]   Starting:    tests
  Passed UTests.Add and Get Value [7 ms]
  Passed UTests.Compare Maps [4 ms]
  Passed UTests.Merge Maps [4 ms]
  Passed UTests.foldL should correctly fold the map [< 1 ms]
  Passed UTests.foldR should correctly fold the map [1 ms]
  Passed UTests.Filter Values [1 ms]
  Passed UTests.Remove Value [< 1 ms]
  Passed UTests.Create Empty Map [< 1 ms]
  Passed PBTests.SeparateChainingHashMap should satisfy associativity property [170 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

[xUnit.net 00:00:00.92]   Finished:    tests
  Passed PBTests.Adding and getting a value should return the same value [37 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

  Passed PBTests.Adding multiple values should increase the size [222 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

  Passed PBTests.SeparateChainingHashMap should satisfy identity property [119 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

  Passed PBTests.Filtering an empty map should return an empty map [1 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

  Passed PBTests.Filtering should return the correct values [106 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

  Passed PBTests.Removing a value should result in None [17 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

  Passed PBTests.Adding the same key twice should update the value [4 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

  Passed PBTests.Merging two maps should combine their values [72 ms]
  Standard Output Messages:
 Ok, passed 100 tests.

Test Run Successful.
Total tests: 17
     Passed: 17
 Total time: 1.3056 Seconds
```

## Примеры написанных тестов

### Add and Get Value

```fs
[<Fact>]
let ``Add and Get Value`` () =
    let map = createEmpty 10
    let map = add "key1" "value1" map
    let value = getValue "key1" map

    ifEqual (Some "value1") value
```

### Adding and getting a value should return the same value

```fs
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
```

# Инструкция

Чтобы запустить сборку отчета
```bash
./build_report.sh
```
дальше локально разворачиваете например с помощью плагина vscode `Live server`.

Чтобы запустить тесты с информацией о каждом тесте
```bash
dotnet test --logger "console;verbosity=detailed" ./tests
```
