# 🔁 Deadlock Demo — задача с дедлоком и его устранением

## 📋 Описание задачи

В проекте реализован класс `TransferHelper`, который содержит метод `Transfer`, отвечающий за перевод денег между двумя счетами (`Account`).  
Код специально написан так, чтобы в конкурентной среде (многопоточности) **вызвать дедлок**.

Пример:

```csharp
public void Transfer(Account a, Account b, decimal amount)
{
    lock (a)
    {
        Thread.Sleep(5000); // искусственная задержка
        lock (b)
        {
            a.Amount -= amount;
            b.Amount += amount;
        }
    }
}
```

## 💥 Как воспроизвести дедлок
```csharp
[Fact]
public async Task ShouldDeadlock()
{
    var ivan = new Account { AccountNumber = 1, Amount = 1000, ownerName = "Ivan" };
    var olga = new Account { AccountNumber = 2, Amount = 500, ownerName = "Olga" };

    var helper = new TransferHelper();

    var t1 = Task.Run(() => helper.Transfer(ivan, olga, 100));
    var t2 = Task.Run(() => helper.Transfer(olga, ivan, 10));

    await Task.WhenAll(t1, t2); // зависнет навсегда!
}
```

## ✅ Как устранить дедлок
Классическая стратегия — ввести глобальный порядок блокировки ресурсов.
В нашем случае — сортировать аккаунты по AccountNumber:
```csharp
public void TransferSafe(Account a, Account b, decimal amount)
{
    var first = a.AccountNumber < b.AccountNumber ? a : b;
    var second = a.AccountNumber < b.AccountNumber ? b : a;

    lock (first)
    {
        Thread.Sleep(1000); // эмуляция задержки
        lock (second)
        {
            a.Amount -= amount;
            b.Amount += amount;
        }
    }
}
```

## 🧪 Тест без дедлока
```csharp
[Fact]
public async Task TransferSafe_ShouldNotDeadlock()
{
    var ivan = new Account { AccountNumber = 1, Amount = 1000, ownerName = "Ivan" };
    var olga = new Account { AccountNumber = 2, Amount = 500, ownerName = "Olga" };

    var helper = new TransferHelper();

    var t1 = Task.Run(() => helper.TransferSafe(ivan, olga, 100));
    var t2 = Task.Run(() => helper.TransferSafe(olga, ivan, 10));

    var completed = await Task.WhenAny(Task.WhenAll(t1, t2), Task.Delay(3000));
    Assert.Equal(TaskStatus.RanToCompletion, completed?.Status);
}
```
