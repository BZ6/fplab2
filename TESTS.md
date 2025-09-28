# Отчет по тестам

`Захаркин Богдан`

`367224`

[титульник](title.pdf)

## Описание проекта

[ссылка на ридми](README.md)

## Какие функциональности вы тестировали и почему

В данной работе были протестированы все ключевые функциональности реализованной хеш-таблицы с раздельным цепочкованием (SeparateChainingHashMap):

### Базовые операции:
- **Добавление и получение значений (add/getValue)** - основные операции любой хеш-таблицы, тестировались как unit-тестами, так и property-based тестами для проверки корректности работы с различными входными данными
- **Удаление элементов (remove)** - критически важная операция, проверялась корректность удаления и возврата None при попытке получить удаленное значение
- **Создание пустой таблицы (createEmpty)** - тестировалась инициализация структуры данных с правильными параметрами

### Функциональные операции:
- **Свёртки (foldL/foldR)** - важные функциональные операции для обхода элементов таблицы, проверялся правильный порядок обхода и корректность накопления результата
- **Фильтрация (filter)** - операция для создания новой таблицы с элементами, удовлетворяющими предикату
- **Слияние (merge)** - операция объединения двух таблиц, тестировалась с помощью property-based тестов для различных комбинаций

### Свойства структуры данных:
- **Ассоциативность слияния** - математическое свойство, проверяющее, что (a merge b) merge c = a merge (b merge c)
- **Тождественный элемент** - проверка того, что слияние с пустой таблицей не изменяет исходную таблицу
- **Сравнение таблиц (compare)** - функция для определения равенства двух таблиц

### Граничные случаи:
- **Обновление значения по существующему ключу** - проверка корректности перезаписи значений
- **Увеличение размера при добавлении элементов** - тестирование правильности подсчета размера таблицы
- **Фильтрация пустой таблицы** - проверка работы с пустыми структурами

**Property-based тестирование** использовалось для автоматической генерации большого количества тестовых случаев (по 100 для каждого свойства), что позволило проверить корректность работы алгоритмов на широком спектре входных данных и выявить потенциальные ошибки, которые могли бы остаться незамеченными при ручном написании тестов.

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

## Покрытие кода

![покрытие кода тестами](images/report2.png)

## Вывод по работе

В ходе выполнения лабораторной работы была успешно реализована и протестирована хеш-таблица с раздельным цепочкованием на языке F#. Все 17 написанных тестов прошли успешно, что свидетельствует о корректности реализации основных операций структуры данных.

**Ключевые достижения:**

1. **Полнота тестирования** - покрыты все основные операции хеш-таблицы: добавление, получение, удаление, фильтрация, свёртки и слияние
2. **Двойной подход к тестированию** - использованы как unit-тесты для проверки конкретных сценариев, так и property-based тесты для автоматизированной проверки математических свойств структуры данных
3. **Проверка математических свойств** - успешно протестированы ассоциативность операции слияния и наличие тождественного элемента, что подтверждает соответствие реализации математическим требованиям к структурам данных
4. **Высокое покрытие кода** - по изображению отчёта видно хорошее покрытие реализованного кода тестами
5. **Быстродействие** - все тесты выполняются за разумное время (общее время выполнения 1.3056 секунды)

**Property-based тестирование** с использованием FsCheck оказалось особенно ценным, так как позволило автоматически сгенерировать и проверить по 100 различных тестовых случаев для каждого свойства, значительно повысив уверенность в корректности реализации.

## Инструкция

Чтобы запустить сборку отчета
```bash
./build_report.sh
```
дальше локально разворачиваете например с помощью плагина vscode `Live server`.

Чтобы запустить тесты с информацией о каждом тесте
```bash
dotnet test --logger "console;verbosity=detailed" ./tests
```
